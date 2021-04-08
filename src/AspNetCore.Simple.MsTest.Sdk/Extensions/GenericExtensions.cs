using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class GenericExtensions
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        public static string ToJson<T>(this T source)
        {
            return JsonSerializer.Serialize(source, JsonSerializerOptions);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T source)
        {
            yield return source;
        }
    }
}
