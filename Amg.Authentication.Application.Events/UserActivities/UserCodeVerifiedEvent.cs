using Amg.Authentication.Application.Events.Enums;
using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserCodeVerifiedEvent : UserActivityEvent
    {
        public SmsCodeType CodeType { get; set; }
        public string  Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

    }
}