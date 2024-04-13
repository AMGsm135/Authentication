using Amg.Authentication.Shared.Attributes;

namespace Amg.Authentication.Shared.Permissions
{
    [PermissionsDefinition("Shop")]
    public enum ShopPermissions
    {

        /// <summary>
        /// مشاهده گزارشات
        /// </summary>
        [PermissionDescription("مدیریت گزارشات", "گزارشات")]
        Report,

        /// <summary>
        /// مشاهده نظرات
        /// </summary>
        [PermissionDescription("مدیریت نظرات", "نظرات")]
        Comment,

        /// <summary>
        /// مشاهده بنر
        /// </summary>
        [PermissionDescription("مدیریت بنرها", "بنر")]
        Banner,


        /// <summary>
        /// مشاهده سفارشات
        /// </summary>
        [PermissionDescription("مدیریت سفارشات", "سفارشات")]
        Order,


        /// <summary>
        /// مشاهده استوری ها
        /// </summary>
        [PermissionDescription("مدیریت استوری", "استوری")]
        Story,


        /// <summary>
        /// مشاهده دسته بندی ها
        /// </summary>
        [PermissionDescription("مدیریت دسته بندی ها", "دسته بندی ها")]
        Category,

    }
}
