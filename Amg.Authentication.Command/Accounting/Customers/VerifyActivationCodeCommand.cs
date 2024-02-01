using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class VerifyActivationCodeCommand : ValidateCaptchaCommand
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }

        public override void Validate()
        {
            base.Validate();
            new VerifyActivationCodeCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class VerifyActivationCodeCommandValidator : AbstractValidator<VerifyActivationCodeCommand>
    {
        public VerifyActivationCodeCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.Code).NotEmpty().WithMessage("کد اعتبار سنجی الزامی است.");
        }
    }
}
