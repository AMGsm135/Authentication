using System.ComponentModel;

namespace Amg.Authentication.Infrastructure.Enums.UserActivities
{
    public enum SignInType
    {
        /// <summary>
        /// ورود به سیستم توسط رمز عبور
        /// </summary>
        [Description("ورود به سیستم توسط رمز عبور")]
        ByPassword,

        /// <summary>
        /// ورود به سیستم توسط کد تایید دو عاملی
        /// </summary>
        [Description("ورود به سیستم توسط کد تایید دو عاملی")]
        ByTwoFactorCode,

        /// <summary>
        /// ورود با شماره همراه
        /// </summary>
        [Description("ورود به سیستم توسط شماره همراه")]
        ByPhoneNumber,

        [Description("ورود به وسیله سرویس دهنده خارجی")]
        ByExternalProvider
    }
}
