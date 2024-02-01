using System.ComponentModel;

namespace Amg.Authentication.Shared.Enums
{
    public enum RoleType
    {
        Any = 0,

        /// <summary>
        /// مدیر ارشد
        /// </summary>
        [Description("مدیر ارشد")]
        SuperAdmin = 1,

        /// <summary>
        /// کاربر صندوق
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
