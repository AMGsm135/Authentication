namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ChangeUserRolesActivity : Activity
    {
        public string Role { get; private set; }

        public ChangeUserRolesActivity(string role) : base(true)
        {
            Role = role;
        }

        // for orm
        private ChangeUserRolesActivity() : base(true)
        {
        }

        public override string GetDescription() => $"تغییر نقش کاربر به {Role}";

    }
}