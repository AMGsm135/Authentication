using Amg.Authentication.Application.Events.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amg.Authentication.Application.Events.UserActivities
{
    internal class UserRegisteredWithPhoneNumberEvent
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public PersonType PersonType { get; set; }

        public bool ByAdmin { get; set; }
    }
}
