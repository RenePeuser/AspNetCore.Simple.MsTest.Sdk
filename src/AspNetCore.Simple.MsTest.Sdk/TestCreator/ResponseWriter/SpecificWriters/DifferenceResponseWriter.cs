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

            // ---------------------------------------------------------
            // 1️⃣ Parse Current JSON
            // ---------------------------------------------------------

            var currentRoot = JToken.Parse(context.CurrentResponseAsString);

            // Optional: Remove volatile fields globally
            RemoveVolatileFields(currentRoot);

            // ---------------------------------------------------------
            // 2️⃣ Apply Smart Placeholder Replacement
            // ---------------------------------------------------------

            ApplySmartReplacements(currentRoot, context.Parameters);

            var formattedCurrent = currentRoot.ToString(Formatting.Indented);

            var expected = JToken.Parse(context.ExpectedResult.Content);
            var current = JToken.Parse(formattedCurrent);

            // ---------------------------------------------------------
            // 3️⃣ Diff
            // ---------------------------------------------------------

            var diffs = jsonDiffer.FindDifferences(expected, current);
            var scopedDifferences = context.DifferenceFunc(diffs).ToImmutableList();
            var allToApply = AssertObjectExtensions
                             .DifferenceFunc(scopedDifferences)
                             .ToImmutableList();

            if (!allToApply.Any())
            {
                return;
            }

            // ---------------------------------------------------------
            // 4️⃣ Apply Differences
            // ---------------------------------------------------------

            foreach (var diff in allToApply)
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

            // ---------------------------------------------------------
            // 5️⃣ Write back
            // ---------------------------------------------------------

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              expected.ToString(Formatting.Indented));
        }

        // =============================================================
        // SMART 2-STAGE REPLACEMENT
        // =============================================================

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

                // 1️⃣ Immer zuerst Property-basiert ersetzen
                ReplaceByProperty(root, propertyName, key, value);

                // 2️⃣ Zusätzlich FullText in String-Werten ersetzen
                ReplaceFullText(root, key, value);
            }
        }

        private static string NormalizePlaceholderToProperty(string placeholder)
        {
            return placeholder.Trim('$');
        }

        // =============================================================
        // PROPERTY MODE (PRIMARY)
        // =============================================================

        private static void ReplaceByProperty(JToken token,
                                              string propertyName,
                                              string placeholder,
                                              object? originalValue)
        {
            if (token is JProperty prop &&
                string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                // Null-Fall
                if (originalValue == null &&
                    prop.Value.Type == JTokenType.Null)
                {
                    prop.Value = placeholder;
                    return;
                }

                // String-Fall
                if (prop.Value.Type == JTokenType.String &&
                    (string?)prop.Value == originalValue?.ToString())
                {
                    prop.Value = placeholder;
                    return;
                }
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children())
                {
                    ReplaceByProperty(child, propertyName, placeholder, originalValue);
                }
            }
        }

        // =============================================================
        // FULLTEXT FALLBACK (STRING VALUES ONLY)
        // =============================================================

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

        // =============================================================
        // VOLATILE FIELD CLEANUP (OPTIONAL BUT RECOMMENDED)
        // =============================================================

        private static void RemoveVolatileFields(JToken token)
        {
            RemoveProperties(token, "CreatedAt", "LastModifiedAt");
        }

        private static void RemoveProperties(JToken token, params string[] names)
        {
            if (token is JProperty prop &&
                names.Any(n => prop.Name.Equals(n, StringComparison.OrdinalIgnoreCase)))
            {
                prop.Remove();
                return;
            }

            if (token is JContainer container)
            {
                foreach (var child in container.Children().ToList())
                {
                    RemoveProperties(child, names);
                }
            }
        }
    }
}
