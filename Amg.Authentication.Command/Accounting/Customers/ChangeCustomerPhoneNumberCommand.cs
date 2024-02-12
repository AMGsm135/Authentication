using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;
using System;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class ChangeCustomerPhoneNumberCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public string PhoneNumber { get; set; }

        public override void Validate()
        {
            base.Validate();
            new ChangeCustomerPhoneNumberCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class ChangeCustomerPhoneNumberCommandValidator : AbstractValidator<ChangeCustomerPhoneNumberCommand>
    {
        public ChangeCustomerPhoneNumberCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
        }
    }
}
