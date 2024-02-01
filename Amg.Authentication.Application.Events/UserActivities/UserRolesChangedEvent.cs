using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserRolesChangedEvent : UserActivityEvent
    {
        public string Role { get; set; }
    }
}
