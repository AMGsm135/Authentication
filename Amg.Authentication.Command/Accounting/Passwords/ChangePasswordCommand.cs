using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Passwords
{
    public class ChangePasswordCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public override void Validate()
        {
            base.Validate();
            new ChangePasswordCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.CurrentPassword).NotEmpty().WithMessage("رمز عبور فعلی نمی تواند خالی باشد.");
            RuleFor(p => p.NewPassword).NotEmpty().WithMessage("رمز عبور جدید نمی تواند خالی باشد.");
            RuleFor(p => p.ConfirmPassword)
                .Must((cmd, cp) => cp == cmd.NewPassword)
                .WithMessage("رمز عبور جدید و تکرار آن باید با هم برابر باشند");
        }
    }
}
