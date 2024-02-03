using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.FundUsers
{
    public class RegisterSystemUserCommand : CommandBase
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
        
        public string Email { get; set; }
        
        public bool TwoFactorEnabled { get; set; }

        public string Password { get; set; }

        public override void Validate()
        {
            base.Validate();
            new RegisterFundUserCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class RegisterFundUserCommandValidator : AbstractValidator<RegisterSystemUserCommand>
    {
        public RegisterFundUserCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه الزامی است");
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("نام الزامی است");
            RuleFor(p => p.LastName).NotEmpty().WithMessage("نام خانوادگی الزامی است");
            RuleFor(p => p.UserName).NotEmpty().WithMessage("نام کاربری الزامی است");
            RuleFor(p => p.Password).NotEmpty().WithMessage("رمز عبور الزامی است");
        }
    }
}