namespace Amg.Authentication.Infrastructure.Settings
{
    public class JwtTokenSettings
    {
        public string DecryptionKey { get; set; }
        public string ValidationKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
