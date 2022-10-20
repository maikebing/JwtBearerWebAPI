namespace MDAI.Contracts
{
    public class AppSettings
    {
        public int JwtExpireHours { get;  set; }

        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
    }
}