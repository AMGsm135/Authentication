using Amg.Authentication.Command.Extensions;
using Amg.Authentication.Infrastructure.Base;
using FluentValidation;

namespace Amg.Authentication.Command.Authorization.Groups
{
    /// <summary>
    /// فرمان ایجاد گروه
    /// </summary>
    public class AddGroupCommand : CommandBase
    {
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            base.Validate();
            new AddGroupCommandValidator().Validate(this).RaiseExceptionIfRequired();
        } 
    }

    public class AddGroupCommandValidator : AbstractValidator<AddGroupCommand>
    {
        public AddGroupCommandValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("نام گروه الزامی است.");
        }
    }


}
