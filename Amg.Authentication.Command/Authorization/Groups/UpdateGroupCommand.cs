using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Authorization.Groups
{
    /// <summary>
    /// فرمان ویرایش گروه
    /// </summary>
    public class UpdateGroupCommand : CommandBase
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new UpdateGroupCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
    {
        public UpdateGroupCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("شناسه گروه الزامی است.");
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام گروه الزامی است.");
        }
    }

}
