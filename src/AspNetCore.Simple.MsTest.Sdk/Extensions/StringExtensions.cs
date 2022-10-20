using System;
using System.Reflection;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class InvalidJsonException : Exception
    {
        public InvalidJsonException(string message) : base(message)
        {
        }
    }

    internal static class StringExtensions
    {
        internal static string GetJsonString(this string jsonValueOrEmbeddedFile, Assembly callingAssembly)
        {
            // ToDo: Current exception string.empty have to fixed soon
            if (jsonValueOrEmbeddedFile.IsNullOrWhiteSpace())
            {
                return jsonValueOrEmbeddedFile;
            }

            var trimmedJsonValue = jsonValueOrEmbeddedFile.Trim();

            if (trimmedJsonValue.EndWith(".json"))
            {
                var jsonValueFromEmbeddedFile = callingAssembly.GetFileContentFrom(trimmedJsonValue);
                if ((jsonValueFromEmbeddedFile.StartWith("{") && jsonValueFromEmbeddedFile.EndWith("}")) || (jsonValueFromEmbeddedFile.StartWith("[") && jsonValueFromEmbeddedFile.EndWith("]")))
                {
                    return jsonValueFromEmbeddedFile;
                }

                throw new InvalidJsonException($"Your given embedded file: '{jsonValueOrEmbeddedFile}' does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []");
            }

            if ((trimmedJsonValue.StartWith("{") && trimmedJsonValue.EndWith("}")) || (trimmedJsonValue.StartWith("[") && trimmedJsonValue.EndWith("]")))
            {
                return trimmedJsonValue;
            }

            throw new InvalidJsonException($"Your given json string does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []{Environment.NewLine}Your invalid string is:{Environment.NewLine}{trimmedJsonValue}");
        }
    }
}
