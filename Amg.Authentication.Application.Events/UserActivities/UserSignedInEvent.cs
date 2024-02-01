using Amg.Authentication.Application.Events.Enums;
using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserSignedInEvent : UserActivityEvent
    {
        public SignInType SignInType { get; set; }

        public SignInResultType ResultType { get; set; }
    }
}