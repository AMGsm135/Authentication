using System.ComponentModel;

namespace Amg.Authentication.Infrastructure.Enums.UserActivities
{
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
