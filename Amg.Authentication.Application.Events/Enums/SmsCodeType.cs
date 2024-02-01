using System.ComponentModel;

namespace Amg.Authentication.Application.Events.Enums
{
    /// <summary>
    /// decoupled from <see cref="SmsCodeType"/>
    /// </summary>
    public enum SmsCodeType
    {
        /// <summary>
        /// کد فعال سازی
        /// </summary>
        [Description("کد فعال سازی")]
        ActivationCode,

        /// <summary>
        /// کد تایید دو عاملی
        /// </summary>
        [Description("کد تایید دو عاملی")]
        TwoFactorCode
    }
}
