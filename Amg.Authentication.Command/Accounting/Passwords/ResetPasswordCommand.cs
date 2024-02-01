using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Passwords
{
    public class ResetPasswordCommand : ValidateCaptchaCommand
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// کد
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// کلمه عبور جدید
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// تکرار کلمه عبور جدید
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new ResetPasswordCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.Code).NotEmpty().WithMessage("کد اعتبار سنجی الزامی است.");
            RuleFor(p => p.NewPassword).NotEmpty().WithMessage("رمز عبور جدید الزامی است");
            RuleFor(p => p.ConfirmPassword)
                .Must((cmd, cp) => cp == cmd.NewPassword)
                .WithMessage("رمز عبور و تکرار آن باید با هم برابر باشند");
        }
    }
}
