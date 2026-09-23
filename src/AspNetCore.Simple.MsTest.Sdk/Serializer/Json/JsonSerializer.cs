using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Serializer.Json
{
    internal static class JsonSerializerExtension
    {
        internal static void AddJsonSerializer(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingletonIfNotExists<JsonSerializer>();
            serviceCollection.AddSingletonIfNotExists(CreateDefaultOptions());
        }

        internal static JsonSerializerOptions CreateDefaultOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Converters = { new JsonStringEnumConverter() }
            };
        }
    }

#pragma warning disable CA1031
    internal sealed class JsonSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        internal string Serialize<T>(T source)
        {
            return System.Text.Json.JsonSerializer.Serialize(source, jsonSerializerOptions);
        }

        internal string? SerializeOrDefault<T>(T source,
                                               string? defaultValue = default)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(source, jsonSerializerOptions);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        internal T Deserialize<T>(string json)
        {
            T? deserializeResult = default;
            var errorMessage = string.Empty;

            try
            {
                deserializeResult = System.Text.Json.JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            if (deserializeResult.IsNull())
            {
                throw new TestSdkProblemDetailsException("Could not deserialize your json string into expected type",
                                                         $"Could not deserialize your json string into expected type: {typeof(T).Name}",
                                                         ("Exception", errorMessage),
                                                         ("JsonString", json),
                                                         ("Type", typeof(T).Name),
                                                         ("TypeFullName", typeof(T).FullName ?? string.Empty));
            }

            return deserializeResult;
        }

        internal T? DeserializeOrDefault<T>(string json,
                                            T? defaultValue = default)
        {
            try
            {
                var deserializeResult = System.Text.Json.JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);

                return deserializeResult;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        internal object Deserialize<T>(string json,
                                       Type returnType)
        {
            object? deserializeResult = default;
            var errorMessage = string.Empty;

            try
            {
                deserializeResult = System.Text.Json.JsonSerializer.Deserialize(json, returnType, jsonSerializerOptions);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            if (deserializeResult.IsNull())
            {
                throw new TestSdkProblemDetailsException("Could not deserialize your json string into expected type",
                                                         $"Could not deserialize your json string into expected type: {typeof(T).Name}",
                                                         ("Exception", errorMessage),
                                                         ("JsonString", json),
                                                         ("Type", typeof(T).Name),
                                                         ("TypeFullName", typeof(T).FullName ?? string.Empty));
            }

            return deserializeResult;
        }

        internal object? DeserializeOrDefault(string json,
                                              Type returnType,
                                              object? defaultValue = default)
        {
            try
            {
                var deserializeResult = System.Text.Json.JsonSerializer.Deserialize(json, returnType, jsonSerializerOptions);

                return deserializeResult;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
#pragma warning restore CA1031
}