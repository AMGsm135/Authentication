using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Application.Contract.Requests
{
    public class SignInByPasswordRequest : ValidateCaptchaCommand
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new SignInByPasswordRequestValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class SignInByPasswordRequestValidator : AbstractValidator<SignInByPasswordRequest>
    {
        public SignInByPasswordRequestValidator()
        {
            RuleFor(p => p.UserName).NotEmpty().WithMessage("نام کاربری یا رمز عبور صحیح نمی باشد");
            RuleFor(p => p.Password).NotEmpty().WithMessage("نام کاربری یا رمز عبور صحیح نمی باشد");
        }
    }
}
