using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class UpdateCustomerPhoneNumberCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public string PhoneNumber { get; set; }

        public override void Validate()
        {
            base.Validate();
            new UpdateCustomerPhoneNumberCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class UpdateCustomerPhoneNumberCommandValidator : AbstractValidator<UpdateCustomerPhoneNumberCommand>
    {
        public UpdateCustomerPhoneNumberCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
        }
    }


}
