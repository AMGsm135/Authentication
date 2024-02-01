using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserOtpStatusChangedEvent : UserActivityEvent
    {
        public bool IsEnabled { get; set; }
    }
}