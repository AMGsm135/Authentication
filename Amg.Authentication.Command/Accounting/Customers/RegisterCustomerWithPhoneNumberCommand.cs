using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class RegisterCustomerWithPhoneNumberCommand : ValidateCaptchaCommand
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string PhoneNumber { get; set; }

        public override void Validate()
        {
            base.Validate();
            new RegisterCustomerWithPhoneNumberCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class RegisterCustomerWithPhoneNumberCommandValidator : AbstractValidator<RegisterCustomerWithPhoneNumberCommand>
    {
        public RegisterCustomerWithPhoneNumberCommandValidator()
        {
            RuleFor(p => p.PhoneNumber).Matches("^(\\+98|0)?9\\d{9}$").WithMessage("شماره همراه اشتباه می باشد.").NotEmpty().WithMessage("شماره همراه اشتباه می باشد.");
        }
    }
}
