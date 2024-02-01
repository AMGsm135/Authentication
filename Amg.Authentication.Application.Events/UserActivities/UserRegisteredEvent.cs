using Amg.Authentication.Application.Events.Enums;
using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserRegisteredEvent : UserActivityEvent
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public PersonType PersonType { get; set; }

        public bool ByAdmin { get; set; }
    }
}