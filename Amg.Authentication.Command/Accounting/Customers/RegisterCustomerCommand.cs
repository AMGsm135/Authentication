using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class RegisterCustomerCommand : ValidateCaptchaCommand
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string PhoneNumber { get; set; }

        public override void Validate()
        {
            base.Validate();
            new RegisterCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه الزامی است");
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("نام الزامی است");
            RuleFor(p => p.LastName).NotEmpty().WithMessage("نام خانوادگی الزامی است");
            RuleFor(p => p.City).NotEmpty().WithMessage("شهر الزامی است");
            RuleFor(p => p.Province).NotEmpty().WithMessage("استان الزامی است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
        }
    }
}