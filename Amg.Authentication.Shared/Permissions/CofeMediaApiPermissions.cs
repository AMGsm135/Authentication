using Amg.Authentication.Shared.Attributes;


namespace Amg.Authentication.Shared.Permissions
{
    [PermissionsDefinition("CofeMediaApi")]
    public enum CofeMediaApiPermissions
    {
        /// <summary>
        /// دریافت اطلاعات درخواست
        /// </summary>
        [PermissionDescription("تست پرمیژن ها", "دریافت اطلاعات درخواست")]
        ViewRequestInfo,
    }
}
