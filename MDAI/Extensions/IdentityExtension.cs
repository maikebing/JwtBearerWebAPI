using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MDAI.Extensions
{
    public static class IdentityExtension
    {

        public static Guid GetUserId(this ControllerBase @this) => Guid.Parse(@this.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public static string GetUserName(this ControllerBase @this) => @this.User.Identity.Name;
        public static string[] GetUserRoles(this ControllerBase @this) => @this.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
        public static string GetUserEmail(this ControllerBase @this) => @this.User.FindFirstValue(ClaimTypes.Email);

        /// Hashes an email with MD5.  Suitable for use with Gravatar profile
        /// image urls
        public static string Gravatar(this IdentityUser user)
        {
            string email = user.Email;
            // Create a new instance of the MD5CryptoServiceProvider object.  
            var _SHA512 = SHA512.Create();
            // Convert the input string to a byte array and compute the hash.  
            byte[] data = _SHA512.ComputeHash(Encoding.Default.GetBytes(email));
            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return string.Format("https://www.gravatar.cn/avatar/{0}", sBuilder.ToString()); ;  // Return the hexadecimal string. 
        }
    }
}