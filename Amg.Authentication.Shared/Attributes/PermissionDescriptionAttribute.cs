using System;

namespace Amg.Authentication.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionDescriptionAttribute : Attribute
    {
        public PermissionDescriptionAttribute(string category, string name)
        {
            Category = category;
            Name = name;
        }

        public string Category { get; set; }

        public string Name { get; set; }
    }
}
