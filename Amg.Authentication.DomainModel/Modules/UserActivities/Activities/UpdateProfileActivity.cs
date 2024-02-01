namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class UpdateProfileActivity : Activity
    {
        public UpdateProfileActivity(string name, string phoneNumber, string email, bool? twoFactorEnabled)
            : base(false)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            TwoFactorEnabled = twoFactorEnabled;
        }

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; private set; }

        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// نیاز به تایید دوعاملی است؟
        /// </summary>
        public bool? TwoFactorEnabled { get; private set; }

        public override string GetDescription()
        {
            var description =  "به روز رسانی اطلاعات حساب کاربری";

            if (!string.IsNullOrEmpty(Name))
                description += $"\r\nتغییر نام به {Name}.";
            
            if (!string.IsNullOrEmpty(PhoneNumber))
                description += $"\r\nتغییر شماره تماس به {PhoneNumber}.";
            
            if (!string.IsNullOrEmpty(Email))
                description += $"\r\nتغییر آدرس ایمیل به {Email}.";
            
            if (TwoFactorEnabled.HasValue)
                description += $"\r\n{(TwoFactorEnabled.Value ? "فعال سازی" : "غیر فعال سازی")} تایید دو عاملی.";
            
            return description;
        }

        // for ORM
        private UpdateProfileActivity() : base(false)
        {
        }
    }
}