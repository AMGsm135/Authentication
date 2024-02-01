using System;
using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Authorization.Groups
{
    public class AssignUserToGroupsCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public Guid[] GroupIds { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new AssignUserToGroupsCommandValidator().Validate(this).RaiseExceptionIfRequired();
        }
    }

    public class AssignUserToGroupsCommandValidator : AbstractValidator<AssignUserToGroupsCommand>
    {
        public AssignUserToGroupsCommandValidator()
        {
            RuleFor(i => i.UserId).NotEmpty().WithMessage("شناسه کاربر الزامی می باشد.");
            RuleFor(i => i.GroupIds).NotNull().WithMessage("شناسه گروه الزامی می باشد.")
                .NotEmpty().WithMessage("شناسه گروه الزامی می باشد.");
        }
    }
}
