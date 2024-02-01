namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ForgetPasswordActivity : Activity
    {
        public ForgetPasswordActivity() : base(false)
        {
        }

        public override string GetDescription() => "درخواست فراموشی رمز عبور";
    }
}