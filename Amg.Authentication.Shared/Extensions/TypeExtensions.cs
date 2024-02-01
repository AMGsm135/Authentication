using System;
using System.Linq;
using System.Reflection;

namespace Amg.Authentication.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static T GetAttributeOfType<T>(this Type type, bool inherit = true) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), inherit);
            return (T)attributes.FirstOrDefault();
        }

        public static T GetAttributeOfType<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return (T)attributes.FirstOrDefault();
        }

        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            return member?.GetAttributeOfType<T>(false);
        }

    }
}
