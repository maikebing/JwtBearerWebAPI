using System.Collections.Generic;

namespace MDAI.Contracts
{
    public class LoginResult
    {
        /// <summary>
        /// 登录结果
        /// </summary>
        public ApiCode Code { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录结果
        /// </summary>
        public Microsoft.AspNetCore.Identity.SignInResult SignIn { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public TokenEntity Token { get; set; }
        /// <summary>
        /// 用户所具备权限
        /// </summary>
        public IList<string> Roles { get; set; }
        public string Avatar { get; internal set; }
    }
    public class RegisterResult
    {
        /// <summary>
        /// 登录结果
        /// </summary>
        public ApiCode Code { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录结果
        /// </summary>
        public Microsoft.AspNetCore.Identity.SignInResult SignIn { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public TokenEntity Token { get; set; }
        /// <summary>
        /// 用户所具备权限
        /// </summary>
        public IList<string> Roles { get; set; }
        public string Avatar { get; internal set; }
    }
    
}
