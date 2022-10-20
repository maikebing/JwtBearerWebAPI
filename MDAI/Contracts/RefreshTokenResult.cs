using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace MDAI.Contracts
{
    public class RefreshTokenResult
    {

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public long ExpiresIn { get; set; }

        public IdentityUser AppUser { get; set; }
        public IList<string> Roles { get; set; }
        public DateTime Expires { get; set; }
    }

}
