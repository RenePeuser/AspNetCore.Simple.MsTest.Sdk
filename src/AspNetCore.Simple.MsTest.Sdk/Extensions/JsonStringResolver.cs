using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddJsonStringResolverExtension
    {
        public static void AddJsonStringResolver(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IJsonStringResolver, JsonStringResolver>();
        }
    }

    /// <summary>
    /// Service for resolving and validating JSON strings with enhanced error formatting.
    /// Uses the globally initialized JsonTypeMismatchOutputBuilder from HttpClientAssertExtensions.Setup().
    /// </summary>
    public interface IJsonStringResolver
    {
        /// <summary>
        /// Gets and validates JSON string from various sources with type checking.
        /// </summary>
        string GetJsonStringFrom<T>(string expectedObjectAsJson,
                                    string currentObject,
                                    Assembly callingAssembly,
                                    [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                    string expectedResultParameterName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0,
                                    [CallerMemberName] string memberName = "");

        /// <summary>
        /// Gets and validates JSON string from various sources, returns null if file not found.
        /// </summary>
        string? GetJsonStringOrDefaultFrom<T>(string expectedObjectAsJson,
                                              string currentObject,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerFilePath] string sourceFilePath = "",
                                              [CallerLineNumber] int sourceLineNumber = 0,
                                              [CallerMemberName] string memberName = "");
    }

    internal sealed class JsonStringResolver : IJsonStringResolver
    {
        public string GetJsonStringFrom<T>(string expectedObjectAsJson,
                                           string currentObject,
                                           Assembly callingAssembly,
                                           [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                           string expectedResultParameterName = "",
                                           [CallerFilePath] string sourceFilePath = "",
                                           [CallerLineNumber] int sourceLineNumber = 0,
                                           [CallerMemberName] string memberName = "")
        {
            return GetJsonStringFromInternal<T>(expectedObjectAsJson,
                                                currentObject,
                                                callingAssembly,
                                                throwOnMissing: true,
                                                expectedResultParameterName,
                                                sourceFilePath,
                                                sourceLineNumber,
                                                memberName)!;
        }

        public string? GetJsonStringOrDefaultFrom<T>(string expectedObjectAsJson,
                                                     string currentObject,
                                                     Assembly callingAssembly,
                                                     [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                                     string expectedResultParameterName = "",
                                                     [CallerFilePath] string sourceFilePath = "",
                                                     [CallerLineNumber] int sourceLineNumber = 0,
                                                     [CallerMemberName] string memberName = "")
        {
            return GetJsonStringFromInternal<T>(expectedObjectAsJson,
                                                currentObject,
                                                callingAssembly,
                                                throwOnMissing: false,
                                                expectedResultParameterName,
                                                sourceFilePath,
                                                sourceLineNumber,
                                                memberName);
        }

        private string? GetJsonStringFromInternal<T>(string expectedObjectAsJson,
                                                     string currentObject,
                                                     Assembly callingAssembly,
                                                     bool throwOnMissing,
                                                     string expectedResultParameterName = "",
                                                     string sourceFilePath = "",
                                                     int sourceLineNumber = 0,
                                                     string memberName = "")
        {
            // 1. Get target type
            var targetTypeInfo = typeof(T);

            // 2. Check if target type is an enumerable
            var isDictionary = targetTypeInfo.GetInterfaces().Any(t => t.Name.Contains("IReadOnlyDictionary"));
            var mustBeAnArray = targetTypeInfo.IsEnumerable() && isDictionary.IsFalse();

            // 3. If the given json value is null or empty then return it
            if (expectedObjectAsJson.IsNullOrWhiteSpace())
            {
                return expectedObjectAsJson;
            }

            // 4. Trim the strings
            var trimmedJsonValue = expectedObjectAsJson.Trim()
                                                       .TrimEnd(Environment.NewLine.ToCharArray())
                                                       .Trim('"');

            // 5. If it is a json file then read the content of the file
            if (trimmedJsonValue.EndWith(".json"))
            {
                if (throwOnMissing.IsFalse())
                {
                    var fileNames = callingAssembly.GetManifestResourceNames();
                    var foundFile = fileNames.FirstOrDefault(name => name.Contains(trimmedJsonValue));

                    if (foundFile.IsNull())
                    {
                        return null;
                    }
                }

                trimmedJsonValue = callingAssembly.GetFileContentFrom(trimmedJsonValue)
                                                  .Trim()
                                                  .TrimEnd(Environment.NewLine.ToCharArray());
            }

            // 6. Handle SimpleHttpResponseMessage special case
            if (targetTypeInfo.NotEqualsTo(typeof(SimpleHttpResponseMessage)))
            {
                var httpResponseMessage = trimmedJsonValue.FromJsonStringOrDefault<SimpleHttpResponseMessage>();

                if (httpResponseMessage.IsNotNull())
                {
                    var jsonContent = httpResponseMessage.Content?.Value?.ToString();

                    trimmedJsonValue = (jsonContent.IsNull() || jsonContent.Trim('"').IsNullOrWhiteSpace())
                                           ? trimmedJsonValue
                                           : jsonContent;
                }
            }

            // 7. JSON ARRAY to NON-ARRAY TYPE - Enhanced error formatting!
            if (trimmedJsonValue.StartWith("[") &&
                trimmedJsonValue.EndWith("]") &&
                mustBeAnArray.IsFalse())
            {
                Assert.That.FailWithObjectToArrayMismatch(expectedResultParameterName,
                                                          targetTypeInfo.FullName ?? targetTypeInfo.Name,
                                                          trimmedJsonValue,
                                                          currentObject,
                                                          sourceFilePath,
                                                          sourceLineNumber,
                                                          memberName);
            }

            // 8. JSON OBJECT to ARRAY TYPE - Enhanced error formatting!
            if (trimmedJsonValue.StartWith("{") &&
                trimmedJsonValue.EndWith("}") &&
                mustBeAnArray)
            {
                Assert.That.FailWithArrayToObjectMismatch(expectedResultParameterName,
                                                          targetTypeInfo.FullName ?? targetTypeInfo.Name,
                                                          trimmedJsonValue,
                                                          currentObject,
                                                          sourceFilePath,
                                                          sourceLineNumber,
                                                          memberName);
            }

            // 9. If json notation is fine so return it
            if ((trimmedJsonValue.StartWith("{") && trimmedJsonValue.EndWith("}")) ||
                (trimmedJsonValue.StartWith("[") && trimmedJsonValue.EndWith("]")))
            {
                return trimmedJsonValue;
            }

            // 10. If the target type is a primitive type or a string then return the json value
            if (targetTypeInfo.IsPrimitive || targetTypeInfo.EqualsTo(typeof(string)))
            {
                return expectedObjectAsJson;
            }

            throw new
                InvalidJsonException($"Your given json string does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []{Environment.NewLine}Your invalid string is:{Environment.NewLine}{trimmedJsonValue}");
        }
    }
}