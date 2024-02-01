using Amg.Authentication.Application.Events.Enums;
using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserPasswordChangedEvent : UserActivityEvent
    {
        public ChangePasswordType Type { get; set; }
    }
}
