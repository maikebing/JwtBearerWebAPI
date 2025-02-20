﻿using System.ComponentModel.DataAnnotations;

namespace MDAI.Contracts
{
    public class LoginDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

}
