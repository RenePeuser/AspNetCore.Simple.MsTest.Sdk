using System;
using System.Globalization;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddPrimitiveTypeConverterExtension
    {
        public static void AddPrimitiveTypeConverter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IPrimitiveTypeConverter, PrimitiveTypeConverter>();
        }
    }

    public interface IPrimitiveTypeConverter
    {
        object ConvertTo(object source, Type targetType);
        T ConvertTo<T>(object source);
        T ConvertTo<T>(string source);
    }

    internal sealed class PrimitiveTypeConverter : IPrimitiveTypeConverter
    {
        public object ConvertTo(object source,
                                Type targetType)
        {
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, source.ToString() ?? string.Empty, true);
            }

            return Convert.ChangeType(source, targetType, CultureInfo.InvariantCulture);
        }

        public T ConvertTo<T>(object source)
        {
            return (T)Convert.ChangeType(source, typeof(T), CultureInfo.InvariantCulture);
        }

        public T ConvertTo<T>(string source)
        {
            // This is a real dirty workaround => this has to be removed soon => background => Some UI specific flags comes currently as number !!!
            // TableCreated and more
            var targetType = typeof(T);

            if (targetType.EqualsTo(typeof(bool)))
            {
                if (bool.TryParse(source, out var boolResult))
                {
                    return boolResult.Cast<T>();
                }

                if (int.TryParse(source, out var intResult))
                {
                    return (intResult > 0).Cast<T>();
                }
            }

            if (targetType.EqualsTo(typeof(string)))
            {
                return (T)(object)source.Trim('\"');
            }

            // ToDo: a lot of checks are needed here, because we just can transform primitive types
            // no enumerations or others
            var changedType = Convert.ChangeType(source, targetType, CultureInfo.InvariantCulture);

            if (changedType.IsNotNull())
            {
                return (T)changedType;
            }

            throw new InvalidCastException($"Can not convert '{source}' to '{targetType.Name}'");
        }
    }
}
