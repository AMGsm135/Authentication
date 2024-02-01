using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Extensions;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ChangePasswordActivity : Activity
    {
        public ChangePasswordType Type { get; private set; }

        public ChangePasswordActivity(ChangePasswordType type) : base(false)
        {
            Type = type;
        }

        // for orm
        private ChangePasswordActivity() : base(false)
        {
        }

        public override string GetDescription() => Type.GetDescription();

    }
}
