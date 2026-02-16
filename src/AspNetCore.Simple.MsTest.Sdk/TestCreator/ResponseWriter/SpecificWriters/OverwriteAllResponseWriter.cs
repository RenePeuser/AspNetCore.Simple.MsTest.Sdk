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
            var canHandle = context.ExpectedResult.EmbeddedFileName.EndsWith(".json") &&
                            context.Mode == ResponseWriteMode.OverwriteAll &&
                            (context.ExpectedResult.EmbeddedFile.IsNull() ||
                             context.ExpectedResult.EmbeddedFile.NotExists());

            return canHandle;
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

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              root.ToString(Formatting.Indented));
        }

        // =============================================================
        // IDENTISCH zur DifferenceWriter-Logik
        // =============================================================

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

                if (value == null)
                {
                    continue;
                }

                var propertyName = key.Trim('$');

                ReplaceByProperty(root, propertyName, key,
                                  value);

                if (value.ToString()?.Length >= 3)
                {
                    ReplaceFullText(root, key, value);
                }
            }
        }

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

                if (originalValue != null && JToken.DeepEquals(prop.Value, JToken.FromObject(originalValue)))
                {
                    prop.Value = placeholder;

                    return true;
                }
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    replaced |= ReplaceByProperty(child, propertyName, placeholder,
                                                  originalValue);
                }
            }

            return replaced;
        }

        private static void ReplaceFullText(JToken token,
                                            string placeholder,
                                            object? originalValue)
        {
            if (originalValue == null)
            {
                return;
            }

            var originalText = originalValue.ToString();
            if (originalText.IsNullOrWhiteSpace() || originalText.Length < 3)
            {
                return;
            }

            if (token is JValue value && value.Type == JTokenType.String)
            {
                var s = (string?)value.Value;

                if (s.IsNullOrWhiteSpace() || s.Contains('$'))
                {
                    return;
                }

                var escaped = Regex.Escape(originalText);
                var pattern = $@"\b{escaped}\b";

                var updated = ReplaceOutsidePlaceholders(s,
                                                         text => Regex.Replace(text,
                                                                               pattern,
                                                                               placeholder,
                                                                               RegexOptions.CultureInvariant));

                value.Value = ReplaceOutsidePlaceholders(updated,
                                                         text => text.Replace(originalText,
                                                                              placeholder,
                                                                              StringComparison.Ordinal));
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    ReplaceFullText(child, placeholder, originalValue);
                }
            }
        }

        private static string ReplaceOutsidePlaceholders(string input, Func<string, string> replacer)
        {
            if (input.Contains('$').IsFalse())
            {
                return replacer(input);
            }

            var parts = input.Split('$');
            for (var i = 0; i < parts.Length; i += 2)
            {
                parts[i] = replacer(parts[i]);
            }

            return string.Join("$", parts);
        }
    }
}
