using System.ComponentModel;

namespace Amg.Authentication.Infrastructure.Enums
{
    public enum SignInResultType
    {
        /// <summary>
        /// درخواست نامعتبر است
        /// </summary>
        [Description("درخواست نامعتبر است")]
        InvalidRequest,

        /// <summary>
        /// نام کاربری یا رمز عبور اشتباه است
        /// </summary>
        [Description("نام کاربری یا رمز عبور اشتباه است")]
        InvalidCredentials,

        /// <summary>
        /// کاربر غیرفعال شده است
        /// </summary>
        [Description("کاربر غیرفعال شده است")]
        UserIsInactive,

        /// <summary>
        /// کاربر به طور موقت مسدود شده است
        /// </summary>
        [Description("کاربر به طور موقت مسدود شده است")]
        UserIsLockedOut,

        /// <summary>
        /// حساب کاربری هنوز فعال نشده است
        /// </summary>
        [Description("حساب کاربری هنوز فعال نشده است")]
        ActivationNeeded,

        /// <summary>
        /// محدودیت حداکثر ورود همزمان مجاز  
        /// </summary>
        [Description("محدودیت حداکثر ورود همزمان مجاز")]
        MaxActiveSessionsReached,

        /// <summary>
        /// نیاز به کد  تایید دو عاملی است
        /// </summary>
        [Description("نیاز به کد  تایید دو عاملی است")]
        TwoFactorCodeNeeded,

        /// <summary>
        /// کد تایید دو عاملی نامعتبر است
        /// </summary>
        [Description("کد تایید دو عاملی نامعتبر است")]
        TwoFactorCodeInvalid,

        [Description("کد تایید یک بار مصرف نامعتبر است")]
        OneTimePasswordInvalid,

        /// <summary>
        /// ورود با موفقیت انجام شد
        /// </summary>
        [Description("ورود با موفقیت انجام شد")]
        LoginSuccessful
    }
}
