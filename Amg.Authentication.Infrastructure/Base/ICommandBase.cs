namespace Amg.Authentication.Infrastructure.Base
{
    public interface ICommandBase
    {
        void Validate();
    }


    public abstract class CommandBase : ICommandBase
    {
        public virtual void Validate()
        {
            // default validator for commands
        }
    }

    public abstract class ValidateCaptchaCommand : CommandBase, ICaptchaValidation
    {
        public string CaptchaId { get; set; }

        public string CaptchaCode { get; set; }

        public override void Validate()
        {
            base.Validate();
            // a.ammari : does not need validate captcha because it will be validated in validation pipeline
        }
    }


}
