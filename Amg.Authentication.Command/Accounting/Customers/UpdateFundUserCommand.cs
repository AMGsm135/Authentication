using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class UpdateCustomerCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Email { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string PostalAddress { get; set; }

        public string PostalCode { get; set; }

        public string AccessToken { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new UpdateCustomerCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("نام الزامی است");
        }
    }
}
