using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddDifferenceResponseWriterExtension
    {
        internal static void AddDifferenceResponseWriter(this IServiceCollection services)
        {
            services.AddJsonDiffer();
            services.AddJsonPathWriter();

            services.AddSingletonIfNotExists<ISpecificResponseWriter, DifferenceResponseWriter>();
        }
    }

    internal sealed class DifferenceResponseWriter(IJsonDiffer jsonDiffer,
                                                   JsonPathWriter jsonPathWriter) : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            return context.Mode == ResponseWriteMode.DifferencesOnly &&
                   context.ExpectedResult.EmbeddedFile.IsNotNull() &&
                   context.ExpectedResult.EmbeddedFile.Exists;
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

            // Parse current JSON
            var currentRoot = JToken.Parse(context.CurrentResponseAsString);

            // SMART 2-STAGE REPLACEMENT
            ApplySmartReplacements(currentRoot, context.Parameters);

            var formattedCurrent = currentRoot.ToString(Formatting.Indented);

            var expected = JToken.Parse(context.ExpectedResult.Content);
            var current = JToken.Parse(formattedCurrent);

            var diffs = jsonDiffer.FindDifferences(expected, current);
            var scopedDifferences = context.DifferenceFunc(diffs).ToImmutableList();

            var allToIgnore = AssertObjectExtensions.DifferenceFunc(scopedDifferences).ToImmutableList();

            if (!allToIgnore.Any())
            {
                return;
            }

            foreach (var diff in allToIgnore)
            {
                if (diff.MemberPath.IsNullOrWhiteSpace())
                {
                    continue;
                }

                switch (diff.MismatchType)
                {
                    case MismatchType.MissingInFirst:
                    case MismatchType.ValueDifference:
                        {
                            var source = current.SelectToken(diff.MemberPath);
                            if (source != null)
                            {
                                jsonPathWriter.AddOrUpdate(expected, diff.MemberPath, source);
                            }

                            break;
                        }

                    case MismatchType.MissingInSecond:
                        {
                            jsonPathWriter.Remove(expected, diff.MemberPath);
                            break;
                        }
                }
            }

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              expected.ToString(Formatting.Indented));
        }

        // -------------------------------------------------------------
        // SMART 2-STAGE ENGINE
        // -------------------------------------------------------------

        private static void ApplySmartReplacements(JToken root,
                                                   params (string key, object? Value)[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
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

                // Stage 1: Try property-based replacement
                var replaced = ReplaceByProperty(root, propertyName, key, value);

                // Stage 2: Fallback to full-text (only inside string values)
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
        // PROPERTY-BASED REPLACEMENT (PRIMARY MODE)
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
                // Handle NULL
                if (originalValue == null &&
                    prop.Value.Type == JTokenType.Null)
                {
                    prop.Value = placeholder;
                    return true;
                }

                // Handle string match
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
        // FULLTEXT FALLBACK (WORD-BOUNDARY, STRING VALUES ONLY)
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
