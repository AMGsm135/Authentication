using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserStatusChangedEvent : UserActivityEvent
    {
        public bool IsActive { get; set; }
    }
}
