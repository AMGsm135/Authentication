using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Passwords
{
    public class ForgetPasswordCommand : ValidateCaptchaCommand
    {
        public string UserName { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new ForgetPasswordCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(p => p.UserName).NotEmpty().WithMessage("نام کاربری الزامی می باشد.");
        }
    }
}
