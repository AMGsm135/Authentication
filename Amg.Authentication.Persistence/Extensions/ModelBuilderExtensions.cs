using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Amg.Authentication.Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder UseEnumToStringConverter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.ClrType.IsEnum)
                    {
                        var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                        var converter = (ValueConverter)Activator.CreateInstance(converterType, (object)null);
                        property.SetValueConverter(converter);
                    }
                }
            }

            return modelBuilder;
        }
    }
}
