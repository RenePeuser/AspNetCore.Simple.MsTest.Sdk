using System;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddJsonSectionBuilderExtension
    {
        public static void AddJsonSectionBuilder(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately
            services.AddSingletonIfNotExists<IJsonSectionBuilder, JsonSectionBuilder>();
        }
    }

    internal interface IJsonSectionBuilder
    {
        /// <summary>
        /// Builds expected result section with label including response filename.
        /// Format: "EXPECTED RESULT (NewPerson.json):\n\n{json}"
        /// </summary>
        string BuildExpected(IHttpResponseContext context,
                             string expectedJson);

        /// <summary>
        /// Builds current result section with label.
        /// Format: "CURRENT RESULT:\n\n{json}"
        /// </summary>
        string BuildCurrent(string currentJson);
    }

    internal sealed class JsonSectionBuilder(ITextDecorator textDecorator) : IJsonSectionBuilder
    {
        public string BuildExpected(IHttpResponseContext context,
                                    string expectedJson)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📄 Expected Snapshot"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();

            // Check if expected file doesn't exist or is empty
            if (expectedJson.IsNullOrWhiteSpace())
            {
                var fileName = context.ExpectedResultFile?.EmbeddedFileName ?? "expected.json";
                var fileExists = context.ExpectedResultFile?.EmbeddedFile?.Exists ?? false;

                if (!fileExists)
                {
                    stringBuilder.Append(textDecorator.Dim($"{fileName} (not exist)"));
                }
                else
                {
                    stringBuilder.Append(textDecorator.Dim($"{fileName} (empty)"));
                }
            }
            else
            {
                var normalizeJsonToSingleLine = NormalizeJsonToSingleLine(expectedJson);
                stringBuilder.Append(normalizeJsonToSingleLine);
            }

            return stringBuilder.ToString();
        }

        public string BuildCurrent(string currentJson)
        {
            var normalizeJsonToSingleLine = NormalizeJsonToSingleLine(currentJson);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📄 Current Result"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(normalizeJsonToSingleLine);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Normalizes JSON to single-line format (removes indentation and newlines).
        /// Also normalizes line endings within string values (\r\n -> \n).
        /// </summary>
        private static string NormalizeJsonToSingleLine(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json;
            }

            try
            {
                var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                NormalizeLineEndings(token);

                return token.ToString(Newtonsoft.Json.Formatting.None);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return json;
            }
        }

        /// <summary>
        /// Recursively normalizes line endings in all string values within a JSON token tree.
        /// Converts \r\n to \n for cross-platform consistency.
        /// </summary>
        private static void NormalizeLineEndings(Newtonsoft.Json.Linq.JToken token)
        {
            switch (token.Type)
            {
                case Newtonsoft.Json.Linq.JTokenType.String:
                    {
                        var stringValue = token.ToObject<string>();

                        if (stringValue.IsNotNullOrWhiteSpace() && stringValue.Contains("\r\n"))
                        {
                            var jValue = (Newtonsoft.Json.Linq.JValue)token;
                            jValue.Value = stringValue.Replace("\r\n", "\n");
                        }

                        break;
                    }
                case Newtonsoft.Json.Linq.JTokenType.Object:
                    {
                        foreach (var property in ((Newtonsoft.Json.Linq.JObject)token).Properties())
                        {
                            NormalizeLineEndings(property.Value);
                        }

                        break;
                    }
                case Newtonsoft.Json.Linq.JTokenType.Array:
                    {
                        foreach (var item in (Newtonsoft.Json.Linq.JArray)token)
                        {
                            NormalizeLineEndings(item);
                        }

                        break;
                    }
            }
        }
    }
}