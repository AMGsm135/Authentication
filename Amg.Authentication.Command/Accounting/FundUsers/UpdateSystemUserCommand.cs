using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Accounting.FundUsers
{
    public class UpdateSystemUserCommand : CommandBase
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// نیاز به تایید دوعاملی است؟
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// آیا فعال هست؟
        /// </summary>
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new UpdateFundUserCommandValidator().Validate(this).RaiseExceptionIfRequired();
        } 
    }

    public class UpdateFundUserCommandValidator : AbstractValidator<UpdateSystemUserCommand>
    {
        public UpdateFundUserCommandValidator()
        {
            RuleFor(p => p.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی است");
            RuleFor(p => p.FirstName).NotEmpty().WithMessage("نام الزامی است");
            RuleFor(p => p.LastName).NotEmpty().WithMessage("نام خانوادگی الزامی است");
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("شماره موبایل الزامی است");
        }
    }
}
