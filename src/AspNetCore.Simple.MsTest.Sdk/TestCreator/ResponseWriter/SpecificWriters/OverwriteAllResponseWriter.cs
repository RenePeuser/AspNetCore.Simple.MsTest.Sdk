using System.Text.RegularExpressions;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddOverwriteAllResponseWriterExtension
    {
        internal static void AddOverwriteAllResponseWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ISpecificResponseWriter, OverwriteAllResponseWriter>();
        }
    }

    internal sealed class OverwriteAllResponseWriter : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            return context.Mode == ResponseWriteMode.OverwriteAll ||
                   context.ExpectedResult.EmbeddedFile.IsNull() ||
                   context.ExpectedResult.EmbeddedFile.NotExists();
        }

        public void Write(WriteResponseRequest context)
        {
            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            if (context.ExpectedResult.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return;
            }

            var root = JToken.Parse(context.CurrentResponseAsString);

            ApplySmartReplacements(root, context.Parameters);

            var formattedCurrent = root.ToString(Formatting.Indented);

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              formattedCurrent);
        }

        // -------------------------------------------------------------
        // SMART 2-STAGE REPLACEMENT (identisch wie DifferenceWriter)
        // -------------------------------------------------------------
        private static void ApplySmartReplacements(JToken root,
                                                   params (string key, object? Value)[] parameters)
        {
            if (parameters.IsNull() || parameters.Length == 0)
            {
                return;
            }

            foreach (var (key, value) in parameters)
            {
                if (key.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var propertyName = NormalizePlaceholderToProperty(key);

                var replaced = ReplaceByProperty(root, propertyName, key, value);

                if (!replaced)
                {
                    ReplaceFullText(root, key, value);
                }
            }
        }

        private static string NormalizePlaceholderToProperty(string placeholder)
        {
            return placeholder.Trim('$');
        }

        // -------------------------------------------------------------
        // PROPERTY MODE (PRIORITÄT)
        // -------------------------------------------------------------

        private static bool ReplaceByProperty(JToken token,
                                              string propertyName,
                                              string placeholder,
                                              object? originalValue)
        {
            var replaced = false;

            if (token is JProperty prop &&
                string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                if (originalValue == null &&
                    prop.Value.Type == JTokenType.Null)
                {
                    prop.Value = placeholder;
                    return true;
                }

                if (prop.Value.Type == JTokenType.String &&
                    (string?)prop.Value == originalValue?.ToString())
                {
                    prop.Value = placeholder;
                    return true;
                }
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    replaced |= ReplaceByProperty(child, propertyName, placeholder, originalValue);
                }
            }

            return replaced;
        }

        // -------------------------------------------------------------
        // FULLTEXT FALLBACK (STRING VALUES ONLY)
        // -------------------------------------------------------------

        private static void ReplaceFullText(JToken token,
                                            string placeholder,
                                            object? originalValue)
        {
            if (originalValue == null)
            {
                return;
            }

            if (token is JValue value && value.Type == JTokenType.String)
            {
                var s = (string?)value.Value;
                if (s.IsNullOrWhiteSpace())
                {
                    return;
                }

                var escaped = Regex.Escape(originalValue.ToString()!);
                var pattern = $@"\b{escaped}\b";

                value.Value = Regex.Replace(s,
                                            pattern,
                                            placeholder,
                                            RegexOptions.CultureInvariant);
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    ReplaceFullText(child, placeholder, originalValue);
                }
            }
        }
    }
}
