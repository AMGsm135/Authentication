using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Extensions;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class SignInActivity : Activity
    {
        public SignInActivity(SignInType signInType, SignInResultType resultType) : base(false)
        {
            ResultType = resultType;
            SignInType = signInType;
        }


        public SignInType SignInType { get; private set; }

        public SignInResultType ResultType { get; private set; }

        public override string GetDescription() => SignInType.GetDescription();


        // for ORM
        private SignInActivity(SignInType signInType) : base(false)
        {
            SignInType = signInType;
        }
    }
}