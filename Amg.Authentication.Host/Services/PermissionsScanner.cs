using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.Shared.Attributes;

namespace Amg.Authentication.Host.Services
{
    public class PermissionsScanner
    {

        public static List<PermissionInfo> GetAllPermissions(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(i => i.IsEnum && i.GetAttributeOfType<PermissionsDefinitionAttribute>() != null)
                .SelectMany(i => i.GetEnumValues().Cast<Enum>(), (t, v) =>
                    new
                    {
                        Definition = t.GetAttributeOfType<PermissionsDefinitionAttribute>(),
                        Description = v.GetAttributeOfType<PermissionDescriptionAttribute>(),
                        Permission = v
                    }).Select(i => new PermissionInfo()
                    {
                        ServiceName = i.Definition?.ServiceName,
                        Category = i.Description?.Category,
                        Description = i.Description?.Name,
                        Name = $"{i.Definition?.ServiceName}-{i.Permission}"
                    }).ToList();
        }
    }


    public class PermissionInfo
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ServiceName { get; set; }
    }
}