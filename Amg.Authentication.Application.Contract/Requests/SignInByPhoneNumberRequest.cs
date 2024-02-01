using Amg.Authentication.Command.Extensions;
using FluentValidation;

namespace Amg.Authentication.Application.Contract.Requests
{
    public class SignInByPhoneNumberRequest 
    {
        public string PhoneNumber { get; set; }

        public string Code { get; set; }


        /// <inheritdoc />
        public void Validate()
        {
            new SignInByPhoneNumberValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class SignInByPhoneNumberValidator : AbstractValidator<SignInByPhoneNumberRequest>
    {
        public SignInByPhoneNumberValidator()
        {
            RuleFor(p => p.Code).NotNull().WithMessage("کد اعتبار سنجی الزامی است.").NotEmpty().WithMessage("کد اعتبار سنجی الزامی است.");
            RuleFor(p => p.PhoneNumber).Matches("^(\\+98|0)?9\\d{9}$").NotEmpty().WithMessage("شماره تماس الزامی است.");
        }
    }
}
