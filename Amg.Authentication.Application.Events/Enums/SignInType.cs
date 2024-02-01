using System.ComponentModel;

namespace Amg.Authentication.Application.Events.Enums
{
    /// <summary>
    /// decoupled from <see cref="SignInType"/>
    /// </summary>
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
        ByTwoFactorCode
    }
}