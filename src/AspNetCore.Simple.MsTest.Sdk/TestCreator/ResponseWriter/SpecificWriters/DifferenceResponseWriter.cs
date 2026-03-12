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
            var canHandle = context.ExpectedResult.EmbeddedFileName.EndsWith(".json") &&
                            context.Mode == ResponseWriteMode.DifferencesOnly &&
                            context.ExpectedResult.EmbeddedFile.IsNotNull() &&
                            context.ExpectedResult.EmbeddedFile.Exists;

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

            var currentRoot = JToken.Parse(context.CurrentResponseAsString);

            var contextParameters = context.Parameters.OrderByDescending(p => p.Value?.ToString()?.Length).ToArray();
            ApplySmartReplacements(currentRoot, contextParameters);

            var currentRootAsJson = currentRoot.ToString(Formatting.Indented);

            // New we can have also indexer properties. Values[0] -> Values[$Index$]
            foreach (var parameter in contextParameters)
            {
                currentRootAsJson = currentRootAsJson.Replace($"[{parameter.Value}]", $"[{parameter.key}]");
            }

            currentRoot = JToken.Parse(currentRootAsJson);

            var expectedRoot = JToken.Parse(context.ExpectedResult.Content);

            var diffs = jsonDiffer.FindDifferences(expectedRoot.ToString(), currentRoot.ToString());

            if (!diffs.Any())
            {
                return;
            }

            var scoped = context.DifferenceFunc(diffs).ToImmutableList();
            var finalDiffs = AssertObjectExtensions.DifferenceFunc(scoped).ToImmutableList();

            var ignoredPaths = diffs.Except(finalDiffs)
                                    .Select(diff => diff.MemberPath)
                                    .Where(path => path.IsNullOrWhiteSpace().IsFalse())
                                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            var resultRoot = currentRoot.DeepClone();

            // For ignored paths we keep the expected snapshot state to avoid noise.
            foreach (var ignoredPath in ignoredPaths)
            {
                var source = expectedRoot.SelectToken(ignoredPath);
                if (source != null)
                {
                    jsonPathWriter.AddOrUpdate(resultRoot, ignoredPath, source);
                }
            }

            var output = resultRoot.ToString(Formatting.Indented);

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              output);
        }

        // Smart Replace bleibt wie zuvor
        private static void ApplySmartReplacements(JToken root,
                                                   params (string key, object? Value)[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return;
            }

            foreach (var (key, value) in parameters)
            {
                if (key.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var propertyName = key.Trim('$');

                // 🔥 Property-Replacement IMMER versuchen – auch bei null
                ReplaceByProperty(root, propertyName, key, value);

                // FullText nur wenn value != null
                if (value != null && value.ToString()?.Length >= 3)
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
                if (originalValue == null && prop.Value.Type == JTokenType.Null)
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

                if (s.IsNullOrWhiteSpace() || s.Contains(placeholder))
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
