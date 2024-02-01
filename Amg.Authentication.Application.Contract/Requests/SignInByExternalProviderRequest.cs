using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;


namespace Amg.Authentication.Application.Contract.Requests
{
    public class SignInByExternalProviderRequest : ValidateCaptchaCommand
    {
        public string Email { get; set; }


        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new SignInByExternalProviderRequestValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class SignInByExternalProviderRequestValidator : AbstractValidator<SignInByExternalProviderRequest>
    {
        public SignInByExternalProviderRequestValidator()
        {
            RuleFor(p => p.Email).EmailAddress()
               .When(i => !string.IsNullOrEmpty(i.Email)).WithMessage("فرمت ایمیل نامعتبر است");
        }
    }
}
