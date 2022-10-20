using MDAI.Contracts;
using MDAI.Data;
using MDAI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {

        private ApplicationDbContext _context;
        private readonly AppSettings _settings;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, ILogger<AuthController> logger, ApplicationDbContext context,
            IOptions<AppSettings> options
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _settings = options.Value;
        }
       
    
        [AllowAnonymous]
        [HttpPost]
        [ProducesDefaultResponseType]
        public async Task<ApiResult<LoginResult>> Login([FromBody] LoginDto model)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    var SignInResult = await CreateToken(model.UserName);
                    return new ApiResult<LoginResult>(ApiCode.Success, "Ok", new LoginResult()
                    {
                        Code = ApiCode.Success,
                        Succeeded = result.Succeeded,
                        Token = new TokenEntity
                        {
                            access_token = SignInResult.Token,
                            expires_in = SignInResult.ExpiresIn,
                            refresh_token = SignInResult.RefreshToken,
                            expires = SignInResult.Expires
                        },
                        UserName = model.UserName,
                        SignIn = result,
                        Roles = SignInResult.Roles,
                        Avatar = SignInResult.AppUser.Gravatar()
                    });
                }
                else
                {
                    return new ApiResult<LoginResult>(ApiCode.Unauthorized, "Unauthorized", null);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<LoginResult>(ApiCode.Exception, ex.Message, null);
            }
        }

  
        private async Task<RefreshTokenResult> CreateToken(string name)
        {
            var appUser = _userManager.Users.SingleOrDefault(r => r.Email == name);
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, appUser.Email),
                new(ClaimTypes.NameIdentifier, appUser.Id),
                new(ClaimTypes.Name,  appUser.UserName),

            };
            var lstclaims = await _userManager.GetClaimsAsync(appUser);
            claims.AddRange(lstclaims);
            var roles = await _userManager.GetRolesAsync(appUser);
            if (roles != null)
            {
                claims.AddRange(from role in roles
                                select new Claim(ClaimTypes.Role, role));
            }
            var expires = DateTime.Now.AddHours(_settings.JwtExpireHours);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JwtIssuer"],
                Audience = _configuration["JwtAudience"],
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = signinCredentials
            };
            var _token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtSecurityTokenHandler.WriteToken(_token);
            var refreshToken = new RefreshToken()
            {
                JwtId = _token.Id,
                Used = false,
                Revoked = false,
                UserId = appUser.Id,
                CreateDateTime = DateTime.UtcNow,
                ExpiryDateTime = DateTime.UtcNow.AddHours(_settings.JwtExpireHours),
                Token = Guid.NewGuid().ToString()
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return new RefreshTokenResult() { RefreshToken = refreshToken.Token, Token = jwtToken, ExpiresIn = (long)(_settings.JwtExpireHours * 3600), AppUser = appUser, Roles = roles, Expires = expires };
        }


        /// <summary>
        /// 刷新JWT Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]

        public async Task<ApiResult<LoginResult>> RefreshToken([FromBody] RefreshTokenDto model)
        {
            try
            {
                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
                if (storedRefreshToken == null)
                {
                    return new ApiResult<LoginResult>(ApiCode.InValidData, "RefreshToken does not exist", null);
                }
                if (storedRefreshToken.Revoked)
                {
                    return new ApiResult<LoginResult>(ApiCode.InValidData, "RefreshToken is revorked", null);
                }

                storedRefreshToken.Used = true;
                 _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();
                var signInResult = await CreateToken(this.GetUserName());
                return new ApiResult<LoginResult>(ApiCode.Success, "Ok", new LoginResult()
                {
                    Code = ApiCode.Success,
                    Succeeded = true,
                    UserName = this.GetUserName(),
                    Roles = this.GetUserRoles(),
                    Token = new TokenEntity
                    {
                        access_token = signInResult.Token,
                        expires_in = signInResult.ExpiresIn,
                        refresh_token = signInResult.RefreshToken,
                        expires = signInResult.Expires
                    },
                });

            }
            catch (Exception ex)
            {
                return new ApiResult<LoginResult>(ApiCode.Exception, ex.Message, null);
            }
        }
   
        [AllowAnonymous]
        [HttpPost]
        [ProducesDefaultResponseType]
        public async Task<ApiResult<bool>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();

                return new ApiResult<bool>(ApiCode.InValidData, "Ok", true);
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(ApiCode.InValidData, ex.Message, true);
            }

        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns >返回登录结果</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResult<RegisterResult>> Register([FromBody] RegisterDto model)
        {
            try
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    await _signInManager.UserManager.AddClaimAsync(user, new  Claim(ClaimTypes.Email, model.Email));
                    return new ApiResult<RegisterResult>(ApiCode.Success, "Ok", new RegisterResult()
                    {
                        Code = ApiCode.Success,
                        Succeeded = result.Succeeded,
                        UserName = model.Email,
                         
                    });
                }
                else
                {
                    var msg = from e in result.Errors select $"{e.Code}:{e.Description}\r\n";
                    return new ApiResult<RegisterResult>(ApiCode.InValidData, string.Join(';', msg.ToArray()), null);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<RegisterResult>(ApiCode.InValidData, ex.Message, null);
            }
        }

        [HttpGet(), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
            })
            .ToArray();
        }
    }
}
