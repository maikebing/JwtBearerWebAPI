using System.ComponentModel.DataAnnotations;

namespace MDAI.Contracts
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 邮箱地址， 也是用户名，一个邮箱只能注册平台的一个客户，如果你在平台有两个租户都有账号，则需要两个邮箱地址。 
        /// </summary>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }
 
        /// <summary>
        /// 用户名密码
        /// </summary>
        [Required]
        public string Password { get; set; }
    }

}
