using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class RegisterUserActivity : Activity
    {
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }

        public PersonType PersonType { get; private set; }

        public bool ByAdmin { get; private set; }

        public RegisterUserActivity(string name, string phoneNumber, string email, PersonType personType, bool byAdmin) : base(false)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            PersonType = personType;
            ByAdmin = byAdmin;
        }

        public override string GetDescription() => $"ایجاد حساب کاربری {PersonType.GetDescription()} با نام \"{Name}\"{(ByAdmin ? " توسط مدیر سیستم" : " توسط کاربر")}";

        // for ORM
        private RegisterUserActivity() : base(false)
        {
        }
    }
}
