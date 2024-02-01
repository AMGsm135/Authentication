namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ChangeUserOtpStatusActivity : Activity
    {
        /// <summary>
        /// آیا فعال هست؟
        /// </summary>
        public bool IsEnabled { get; private set; }

        public ChangeUserOtpStatusActivity(bool isEnabled) : base(false)
        {
            IsEnabled = isEnabled;
        }

        public override string GetDescription() => $" {(IsEnabled ? "" : "غیر ")}فعال سازی کد یکبار مصرف";


        // for orm
        private ChangeUserOtpStatusActivity() : base(false)
        {
        }
    }
}
