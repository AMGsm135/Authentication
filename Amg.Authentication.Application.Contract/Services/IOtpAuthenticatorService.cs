using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface IOtpAuthenticatorService : IApplicationService
    {
        byte[] GenerateActivationQrCode(string userAccount, string secretKey, int size = 512);
        
        string GenerateOtpCode(string secretKey);

        bool ValidateTwoFactorPin(string secretKey, string otpCode);
    }
}
