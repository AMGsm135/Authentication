using Amg.Authentication.Application.Events.Enums;
using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserCodeResentEvent : UserActivityEvent
    {
        public string PhoneNumber { get; set; }
        public SmsCodeType CodeType { get; set; }

    }
}