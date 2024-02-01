using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Authorization.Groups
{
    public class AssignPermissionToGroupCommand : CommandBase
    {
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// شناسه های دسترسی ها جهت انتصاب
        /// </summary>
        public Guid[] PermissionIds { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new AssignPermissionToGroupCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class AssignPermissionToGroupCommandValidator : AbstractValidator<AssignPermissionToGroupCommand>
    {
        public AssignPermissionToGroupCommandValidator()
        {
            RuleFor(p => p.GroupId).NotEmpty().WithMessage("شناسه گروه الزامی است.");
        }
    }


}
