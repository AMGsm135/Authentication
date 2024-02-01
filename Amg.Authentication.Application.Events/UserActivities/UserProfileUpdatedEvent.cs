using Amg.Authentication.Application.Events.UserActivities.Base;

namespace Amg.Authentication.Application.Events.UserActivities
{
    public class UserProfileUpdatedEvent : UserActivityEvent
    {
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// آدرس ایمیل
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// نیاز به تایید دوعاملی است؟
        /// </summary>
        public bool? TwoFactorEnabled { get; set; }
    }
}