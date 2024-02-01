using System.ComponentModel;

namespace Amg.Authentication.Infrastructure.Enums.UserActivities
{
    public enum ChangePasswordType
    {
        /// <summary>
        /// تغییر رمز عبور توسط کاربر
        /// </summary>
        [Description("تغییر رمز عبور توسط کاربر")]
        ChangeByUser,

        /// <summary>
        /// بازنشانی رمز عبور توسط کاربر
        /// </summary>
        [Description("بازنشانی رمز عبور توسط کاربر")]
        ResetByUser,

        /// <summary>
        /// بازنشانی رمز عبور توسط مدیر سیستم
        /// </summary>
        [Description("بازنشانی رمز عبور توسط مدیر سیستم")]
        ResetByAdmin,

        /// <summary>
        /// تغییر رمز عبور به مقدار پیش فرض توسط مدیر سیستم
        /// </summary>
        [Description("تغییر رمز عبور به مقدار پیش فرض توسط مدیر سیستم")]
        SetToDefaultByAdmin,

        /// <summary>
        /// تغییر رمز عبور به مقدار قبلی توسط مدیر سیستم
        /// </summary>
        [Description("تغییر رمز عبور به مقدار قبلی توسط مدیر سیستم")]
        RestoreByAdmin

    }
}
