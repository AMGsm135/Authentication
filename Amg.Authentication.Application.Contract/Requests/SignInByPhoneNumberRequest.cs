using Amg.Authentication.Command.Extensions;
using FluentValidation;
using System;

namespace Amg.Authentication.Application.Contract.Requests
{
    public class SignInByPhoneNumberRequest
    {
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; }

        public string VerifyCode { get; set; }


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
            RuleFor(p => p.VerifyCode).NotNull().WithMessage("کد اعتبار سنجی الزامی است.").NotEmpty().WithMessage("کد اعتبار سنجی الزامی است.");
            RuleFor(p => p.PhoneNumber).Matches("^(\\+98|0)?9\\d{9}$").NotEmpty().WithMessage("شماره تماس الزامی است.");
        }
    }
}
