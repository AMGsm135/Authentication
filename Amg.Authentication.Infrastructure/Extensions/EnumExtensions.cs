using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Amg.Authentication.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetDescription(this Enum enumValue)
        {
            var attribute = enumValue.GetAttributeOfType<DescriptionAttribute>();
            return attribute == null ? enumValue.ToString() : attribute.Description;
        }

        public static List<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

    }
}
