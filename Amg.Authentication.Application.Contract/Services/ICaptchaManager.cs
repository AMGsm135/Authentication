using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface ICaptchaManager : IApplicationService
    {
        CaptchaItem GenerateCaptcha();
        void ValidateCaptcha(ICaptchaValidation captcha);
        CaptchaItem RefreshCaptcha(string oldCaptchaId);
    }
}