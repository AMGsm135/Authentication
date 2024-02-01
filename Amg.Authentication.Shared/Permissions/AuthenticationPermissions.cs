using Amg.Authentication.Shared.Attributes;

namespace Amg.Authentication.Shared.Permissions
{
    [PermissionsDefinition("Authentication")]
    public enum AuthenticationPermissions
    {
        /// <summary>
        /// ایجاد کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "ایجاد کاربر")]
        CreateUser,

        /// <summary>
        /// بازگردانی رمز به کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "بازگردانی رمز به کاربر")]
        DefaultPassword,

        /// <summary>
        /// فعال سازی حساب کاربری مشتریان
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "فعال سازی حساب کاربری مشتریان")]
        ConfirmUserPhoneNumber,

        /// <summary>
        /// ویرایش کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "ویرایش کاربر")]
        ModifyUser,

        /// <summary>
        /// مشاهده جزئیات کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "مشاهده جزئیات کاربر")]
        ViewUserDetails,

        /// <summary>
        /// مشاهده لیست کاربران
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "مشاهده لیست کاربران")]
        ViewAllUsers,

        /// <summary>
        /// غیر فعال سازی کاربران
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "غیر فعال سازی کاربران")]
        ActivationFundUser,

        /// <summary>
        /// انتصاب گروه به کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "انتصاب گروه به کاربر")]
        AssignGroup,

        /// <summary>
        /// مشاهده گروه های کاربر
        /// </summary>
        [PermissionDescription("مدیریت کاربران", "مشاهده گروه های کاربر")]
        ViewUserGroups,

        /// <summary>
        /// ویرایش سطوح دسترسی گروه
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "ویرایش سطوح دسترسی گروه")]
        ModifyGroupPermissions,

        /// <summary>
        /// مشاهده دسترسی های گروه
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "مشاهده دسترسی های گروه")]
        ViewGroupPermissions,

        /// <summary>
        /// مشاهده جزئیات گروه
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "مشاهده جزئیات گروه")]
        ViewGroupDetails,

        /// <summary>
        /// مشاهده همه گروه ها
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "مشاهده همه گروه ها")]
        ViewAllGroups,

        /// <summary>
        /// ایجاد گروه
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "ایجاد گروه")]
        CreateGroup,

        /// <summary>
        /// ویرایش گروه
        /// </summary>
        [PermissionDescription("مدیریت نقش ها", "ویرایش گروه")]
        ModifyGroup,

    }
}
