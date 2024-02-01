namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ChangeUserStatusActivity : Activity
    {
        /// <summary>
        /// آیا فعال هست؟
        /// </summary>
        public bool IsActive { get; private set; }

        public ChangeUserStatusActivity(bool isActive) : base(false)
        {
            IsActive = isActive;
        }

        public override string GetDescription() => $" {(IsActive ? "" : "غیر ")}فعال سازی حساب کاربری";


        // for orm
        private ChangeUserStatusActivity() : base(false)
        {
        }
    }
}
