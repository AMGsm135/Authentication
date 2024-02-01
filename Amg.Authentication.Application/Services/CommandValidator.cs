using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Services
{
    public class CommandValidator : ICommandValidator
    {
        private readonly LazyDependency<ICaptchaManager> _captchaManager;

        public CommandValidator(LazyDependency<ICaptchaManager> captchaManager)
        {
            _captchaManager = captchaManager;
        }

        public void Validate(ICommandBase command)
        {
            command.Validate();
            if (command is ICaptchaValidation cmd)
            {
                _captchaManager.Instance.ValidateCaptcha(cmd);
            }
        }
    }
}
