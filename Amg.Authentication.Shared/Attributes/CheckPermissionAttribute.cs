using System;
using Amg.Authentication.Shared.Enums;

namespace Amg.Authentication.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CheckPermissionAttribute : Attribute
    {
        public RoleType Role { get; }

        // a.ammari : multiple types of Permission exists
        public object Permission { get; }

        public CheckPermissionAttribute(RoleType role, object permission)
        {
            Role = role;
            Permission = permission;
        }
    }
}
