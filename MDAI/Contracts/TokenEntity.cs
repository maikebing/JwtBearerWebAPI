using System;

namespace MDAI.Contracts
{
    public class TokenEntity
    {
        /// <summary>
        /// token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long expires_in { get; set; }


        public string refresh_token { get; set; }
        public DateTime expires { get; set; }
    }

}
