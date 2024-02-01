using Amg.Authentication.Shared.Attributes;

namespace Amg.Authentication.Shared.Permissions
{
    [PermissionsDefinition("Pricing")]
    public enum PricingPermissions
    {
        /// <summary>
        /// مدیریت دسته بندی ها
        /// </summary>
        [PermissionDescription("دسته بندی ها", "مدیریت دسته بندی ها")]
        ManageProductCategories,

        /// <summary>
        /// مدیریت ترجمه های دسته بندی ها
        /// </summary>
        [PermissionDescription("دسته بندی ها", "مدیریت ترجمه های دسته بندی ها")]
        ManageProductCategoryTranslations,

        /// <summary>
        /// مدیریت مناطق
        /// </summary>
        [PermissionDescription("مناطق", "مدیریت مناطق")]
        ManageAreas,

        /// <summary>
        /// مدیریت ترجمه های مناطق
        /// </summary>
        [PermissionDescription("مناطق", "مدیریت ترجمه های مناطق")]
        ManageAreaTranslations,

        /// <summary>
        /// مدیریت منابع
        /// </summary>
        [PermissionDescription("منابع", "مدیریت منابع")]
        ManageSources,

        /// <summary>
        /// مدیریت ترجمه های منابع
        /// </summary>
        [PermissionDescription("منابع", "مدیریت ترجمه های منابع")]
        ManageSourceTranslations,

        /// <summary>
        /// مدیریت محصولات
        /// </summary>
        [PermissionDescription("محصولات", "مدیریت محصولات")]
        ManageProducts,

        /// <summary>
        /// مدیریت قیمت محصولات
        /// </summary>
        [PermissionDescription("محصولات", "مدیریت قیمت محصولات")]
        ManageProductPrices,

    }
}
