using System.ComponentModel.DataAnnotations;

namespace MDAI.Contracts
{
    public class RefreshTokenDto
    {

        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }

    }
}
