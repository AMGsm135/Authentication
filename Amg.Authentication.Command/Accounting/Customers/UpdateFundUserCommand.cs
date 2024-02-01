using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class UpdateCustomerCommand : CommandBase
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }

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
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام الزامی است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
        }
    }
}
