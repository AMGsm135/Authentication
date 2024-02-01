using Amg.Authentication.Shared.Attributes;

namespace Amg.Authentication.Shared.Permissions
{
    [PermissionsDefinition("Sample")]
    public enum SamplePermissions
    {
        /// <summary>
        /// دریافت اطلاعات درخواست
        /// </summary>
        [PermissionDescription("تست پرمیژن ها", "دریافت اطلاعات درخواست")]
        ViewRequestInfo,
    }
}
