using System.ComponentModel;

namespace Amg.Authentication.Infrastructure.Enums
{
    public enum RoleType
    {
        /// <summary>
        /// مدیر ارشد
        /// </summary>
        [Description("مدیر ارشد")]
        SuperAdmin = 1,

        /// <summary>
        /// کاربر سیستم
        /// </summary>
        [Description("کاربر سیستم")]
        SystemUser = 2,
        
        /// <summary>
        /// مشتری
        /// </summary>
        [Description("مشتری")]
        Customer = 3,


    }
}
