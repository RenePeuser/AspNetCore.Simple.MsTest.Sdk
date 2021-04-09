using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class GenericExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        internal static string ToJson<T>(this T source)
        {
            return JsonSerializer.Serialize(source, JsonSerializerOptions);
        }

        internal static IEnumerable<T> ToEnumerable<T>(this T source)
        {
            yield return source;
        }
    }
}
