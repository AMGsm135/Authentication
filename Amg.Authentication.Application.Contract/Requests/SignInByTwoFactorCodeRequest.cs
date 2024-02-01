using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Application.Contract.Requests
{
    public class SignInByTwoFactorCodeRequest : ValidateCaptchaCommand
    {
        public Guid UserId { get; set; }
        public string VerifyCode { get; set; }

        public override void Validate()
        {
            base.Validate();
            new SignInByTwoFactorCodeRequestValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class SignInByTwoFactorCodeRequestValidator : AbstractValidator<SignInByTwoFactorCodeRequest>
    {
        public SignInByTwoFactorCodeRequestValidator()
        {
            RuleFor(p => p.VerifyCode).NotEmpty().WithMessage("کد تایید الزامی است");
            RuleFor(p => p.VerifyCode).Length(6).WithMessage("کد تایید 6 کارکتر باید باشد");
        }
    }
}
