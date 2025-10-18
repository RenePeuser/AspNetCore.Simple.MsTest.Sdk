using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class StringExtensions
    {
        private static readonly CurlFormatter CurlFormatter = new();

        private static readonly OutputFormatter OutputFormatter = new(CurlFormatter);

        public static string GetJsonStringFrom(this string expectedObjectAsJson,
                                               Assembly callingAssembly)

        {
            var trimmedJsonValue = expectedObjectAsJson.Trim() // Trim whitespaces
                                                       .TrimEnd(Environment.NewLine.ToCharArray()) // Trim line breaks at the end if exists
                                                       .Trim('"'); // Trim " if exists cause not needed

            if (trimmedJsonValue.EndWith(".json"))
            {
                trimmedJsonValue = callingAssembly.GetFileContentFrom(trimmedJsonValue).Trim().TrimEnd(Environment.NewLine.ToCharArray());
            }

            return trimmedJsonValue;
        }

        public static string GetJsonStringFrom<T>(this string expectedObjectAsJson,
                                                  string currentObject,
                                                  Assembly callingAssembly,
                                                  string curl,
                                                  [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "")
        {
            // 1. Get target type
            var targeTypeInfo = typeof(T);

            // 2. Check if target type is an enumerable
            var isDictionary = targeTypeInfo.GetInterfaces().Any(t => t.Name.Contains("IReadOnlyDictionary"));
            var targetType = targeTypeInfo;
            var mustBeAnArray = targetType.IsEnumerable() && isDictionary.IsFalse();

            // 3. If the given json value is null or empty then return it
            //    Default serialization will be handled by the caller
            if (expectedObjectAsJson.IsNullOrWhiteSpace())
            {
                return expectedObjectAsJson;
            }

            // 4. Trim the strings
            var trimmedJsonValue = expectedObjectAsJson.Trim() // Trim whitespaces
                                                       .TrimEnd(Environment.NewLine.ToCharArray()) // Trim line breaks at the end if exists
                                                       .Trim('"'); // Trim " if exists cause not needed

            // 5. If it is a json file then read the content of the file
            if (trimmedJsonValue.EndWith(".json"))
            {
                trimmedJsonValue = callingAssembly.GetFileContentFrom(trimmedJsonValue).Trim().TrimEnd(Environment.NewLine.ToCharArray());
            }

            // 6. If we have our new response format we only allowed to check or compare the Content.Value property
            //    We need this all to keep all compatible with the older version which does not have full response 
            //    assertion.
            if (targetType.NotEqualsTo(typeof(SimpleHttpResponseMessage)))
            {
                var httpResponseMessage = trimmedJsonValue.FromJsonStringOrDefault<SimpleHttpResponseMessage>();

                if (httpResponseMessage.IsNotNull())
                {
                    var jsonContent = httpResponseMessage.Content?.Value?.ToString();
                    trimmedJsonValue = (jsonContent.IsNull() || jsonContent.Trim('"').IsNullOrWhiteSpace()) ? trimmedJsonValue : jsonContent;
                }
            }

            // 7. If the json string is an array but the target type is not an enumerable then throw an exception
            if (trimmedJsonValue.StartWith("[") &&
                trimmedJsonValue.EndWith("]") &&
                mustBeAnArray.IsFalse())
            {
                var output = OutputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {targeTypeInfo.FullName}",
                                                             "Invalid source type object {} to target array type [] json conversion",
                                                             trimmedJsonValue,
                                                             currentObject,
                                                             curl);

                Assert.Fail(output);
            }

            // 8. If the json string is an object but the target type is an enumerable then throw an exception
            if (trimmedJsonValue.StartWith("{") &&
                trimmedJsonValue.EndWith("}") &&
                mustBeAnArray)
            {
                var output = OutputFormatter.GetOutputString($"Your passed json string: {expectedResultParameterName} is an object notation {{}}, but your target type: {targetType} is an array so you can't deserialize it. Please fix your json string",
                                                             "Invalid source type array [] to target type object {} json conversion",
                                                             trimmedJsonValue,
                                                             currentObject,
                                                             curl);

                Assert.Fail(output);
            }

            // 9. If json notation is fine so return it.
            if ((trimmedJsonValue.StartWith("{") && trimmedJsonValue.EndWith("}")) ||
                (trimmedJsonValue.StartWith("[") && trimmedJsonValue.EndWith("]")))
            {
                return trimmedJsonValue;
            }

            // 10. If the target type is a primitive type or a string then return the json value
            var type = targeTypeInfo;

            if (type.IsPrimitive || type == typeof(string))
            {
                return expectedObjectAsJson;
            }

            throw new InvalidJsonException($"Your given json string does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []{Environment.NewLine}Your invalid string is:{Environment.NewLine}{trimmedJsonValue}");
        }

        public static string? GetJsonStringOrDefaultFrom<T>(this string expectedObjectAsJson,
                                                            string currentObject,
                                                            Assembly callingAssembly,
                                                            string curl,
                                                            [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                                            string expectedResultParameterName = "")
        {
            // 1. Get target type
            var targetType = typeof(T);

            // 2. Check if target type is an enumerable
            var isEnumerable = targetType.IsEnumerable();

            // 3. If the given json value is null or empty then return it
            //    Default serialization will be handled by the caller
            if (expectedObjectAsJson.IsNullOrWhiteSpace())
            {
                return expectedObjectAsJson;
            }

            // 4. Trim the strings
            var trimmedJsonValue = expectedObjectAsJson.Trim() // Trim whitespaces
                                                       .TrimEnd(Environment.NewLine.ToCharArray()) // Trim line breaks at the end if exists
                                                       .Trim('"'); // Trim " if exists cause not needed

            // 5. If it is a json file then read the content of the file
            if (trimmedJsonValue.EndWith(".json"))
            {
                var fileNames = callingAssembly.GetManifestResourceNames();
                var foundFile = fileNames.FirstOrDefault(name => name.Contains(trimmedJsonValue));

                if (foundFile.IsNull())
                {
                    return null;
                }

                trimmedJsonValue = callingAssembly.GetFileContentFrom(trimmedJsonValue).Trim().TrimEnd(Environment.NewLine.ToCharArray());
            }

            // 6. If we have our new response format we only allowed to check or compare the Content.Value property
            //    We need this all to keep all compatible with the older version which does not have full response 
            //    assertion.
            if (targetType.NotEqualsTo(typeof(SimpleHttpResponseMessage)))
            {
                var httpResponseMessage = trimmedJsonValue.FromJsonStringOrDefault<SimpleHttpResponseMessage>();

                if (httpResponseMessage.IsNotNull())
                {
                    trimmedJsonValue = httpResponseMessage.Content?.Value?.ToString() ?? trimmedJsonValue;
                }
            }

            // 7. If the json string is an array but the target type is not an enumerable then throw an exception
            if (trimmedJsonValue.StartWith("[") &&
                trimmedJsonValue.EndWith("]") &&
                isEnumerable.IsFalse())
            {
                var output = OutputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).FullName}",
                                                             "Invalid source type object {} to target array type [] json conversion",
                                                             trimmedJsonValue,
                                                             currentObject,
                                                             curl);

                Assert.Fail(output);
            }

            // 8. If the json string is an object but the target type is an enumerable then throw an exception
            if (trimmedJsonValue.StartWith("{") &&
                trimmedJsonValue.EndWith("}") &&
                isEnumerable)
            {
                var output = OutputFormatter.GetOutputString($"Your passed json string: {expectedResultParameterName} is an object notation {{}}, but your target type: {targetType} is an array so you can't deserialize it. Please fix your json string",
                                                             "Invalid source type array [] to target type object {} json conversion",
                                                             trimmedJsonValue,
                                                             currentObject,
                                                             curl);

                Assert.Fail(output);
            }

            // 9. If json notation is fine so return it.
            if ((trimmedJsonValue.StartWith("{") && trimmedJsonValue.EndWith("}")) ||
                (trimmedJsonValue.StartWith("[") && trimmedJsonValue.EndWith("]")))
            {
                return trimmedJsonValue;
            }

            // 10. If the target type is a primitive type or a string then return the json value
            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
            {
                return expectedObjectAsJson;
            }

            throw new InvalidJsonException($"Your given json string does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []{Environment.NewLine}Your invalid string is:{Environment.NewLine}{trimmedJsonValue}");
        }

        public static string ResolveParameters(this string value,
                                               (string Key, object? Value)[] parameters)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            var replacedString = value;

            foreach (var keyValue in parameters)
            {
                // We have to take care of int, bool, long and so on
                // Json sample
                // {
                //   "Id": "$projectId$",
                // }
                // -------------------------------------------------
                // Json sample
                // {
                //   "Id": $projectId$,
                // }
                // -------------------------------------------------
                // Json sample
                // {
                //   "ReferenceId": null,  < If value is null this must be thec
                // }

                if (keyValue.Value.IsNull())
                {
                    // We have to take care about "$MyParam$" <- So also the " have to gone with
                    replacedString = replacedString.Replace($"\"{keyValue.Key}\"", "null");

                    // And if someone use it correctly already $MyParam$ <- we have to replace it also
                    replacedString = replacedString.Replace(keyValue.Key, "null");
                    continue;
                }

                var type = keyValue.Value.GetType();

                if (keyValue.Value.IsNotNull() &&
                    type.IsPrimitive)
                {
                    var primitiveTypeValue = keyValue.Value.ToString();
                    if (primitiveTypeValue.IsNull())
                    {
                        continue;
                    }
                    
                    if (type == typeof(bool))
                    {
                        primitiveTypeValue = primitiveTypeValue.ToLowerInvariant();
                    }
                    
                    replacedString = replacedString.Replace($"\"{keyValue.Key}\"", primitiveTypeValue);
                }

                replacedString = replacedString.Replace(keyValue.Key, keyValue.Value.ToString());

            }

            return replacedString;
        }
    }
}
