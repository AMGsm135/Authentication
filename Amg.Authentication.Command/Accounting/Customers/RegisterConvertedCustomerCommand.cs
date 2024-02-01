using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class RegisterConvertedCustomerCommand : CommandBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// نوع مشتری
        /// </summary>
        public PersonType PersonType { get; set; }

        public override void Validate()
        {
            base.Validate();
            new RegisterConvertedCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class RegisterConvertedCustomerCommandValidator : AbstractValidator<RegisterConvertedCustomerCommand>
    {
        public RegisterConvertedCustomerCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه الزامی است");
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام الزامی است");
            RuleFor(p => p.UserName).NotEmpty().WithMessage("نام کاربری الزامی است");
            RuleFor(p => p.Email).EmailAddress()
                .When(i => !string.IsNullOrEmpty(i.Email)).WithMessage("فرمت ایمیل نامعتبر است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
            RuleFor(p => p.PersonType).IsInEnum().WithMessage("نوع مشتری معتبر نیست");
            RuleFor(p => p.Password).NotEmpty().WithMessage("رمز عبور الزامی است");
        }
    }
}