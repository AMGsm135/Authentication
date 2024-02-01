using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Infrastructure.Settings
{
    public class AuthSettings
    {
        public LockoutSettings Lockout { get; set; }
        public PasswordSettings Password { get; set; }
        public TokenSettings Token { get; set; }
        public OtpAuthenticatorSettings OtpAuthenticator { get; set; }
        public VerifyAccountType VerifyAccountType { get; set; }
        public List<TwoFactorTypeItem> TwoFactorTypes { get; set; }
    }

    public class LockoutSettings
    {
        public bool Enabled { get; set; }
        public int Threshold { get; set; }
        public int LockTime { get; set; }
    }

    public class PasswordSettings
    {
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireDigits { get; set; }
        public bool RequireSymbols { get; set; }
        public int MinimumLength { get; set; }
        public int MinimumUniqueChars { get; set; }
        public bool ForceForExistingUsers { get; set; }
        public string DefaultPassword { get; set; }
    }


    public class TokenSettings
    {
        public int TokenLifeTime { get; set; }
        public int RefreshTimeout { get; set; }
        public bool IsSecure { get; set; }
        public string Domain { get; set; }
        public int MaxActiveSessions { get; set; }
        public bool RevokeOnIpChange { get; set; }
        public bool RevokeOnClientChange { get; set; }
    }
    
    public class OtpAuthenticatorSettings
    {
        public int ClockDriftTolerance { get; set; }
        public string Issuer { get; set; }
    }

    public class TwoFactorTypeItem
    {
        public int Order { get; set; }
        public TwoFactorType Type { get; set; }
    }


}
