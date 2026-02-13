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

            var currentRoot = JToken.Parse(context.CurrentResponseAsString);

            var contextParameters = context.Parameters.OrderByDescending(p => p.Value?.ToString()?.Length).ToArray();
            ApplySmartReplacements(currentRoot, contextParameters);

            var currentRootAsJson = currentRoot.ToString(Formatting.Indented);

            // Fallback :/ we have full text replace ments which does not full fill word matching
            foreach (var parameter in contextParameters)
            {
                var oldValue = parameter.Value?.ToString();

                if (oldValue.IsNotNull())
                {
                    currentRootAsJson = currentRootAsJson.Replace(oldValue, parameter.key);
                }
            }

            currentRoot = JToken.Parse(currentRootAsJson);

            var expectedRoot = JToken.Parse(context.ExpectedResult.Content);

            var diffs = jsonDiffer.FindDifferences(expectedRoot, currentRoot);

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

            foreach (var diff in diffs)
            {
                if (diff.MemberPath.IsNullOrWhiteSpace() || IsIgnoredPath(diff.MemberPath, ignoredPaths).IsFalse())
                {
                    continue;
                }

                switch (diff.MismatchType)
                {
                    case MismatchType.MissingInFirst:
                    {
                        jsonPathWriter.Remove(resultRoot, diff.MemberPath);

                        break;
                    }

                    case MismatchType.MissingInSecond:
                    case MismatchType.ValueDifference:
                    {
                        var source = expectedRoot.SelectToken(diff.MemberPath);

                        if (source != null)
                        {
                            jsonPathWriter.AddOrUpdate(resultRoot, diff.MemberPath, source);
                        }

                        break;
                    }
                }
            }

            var output = resultRoot.ToString(Formatting.Indented);

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              output);
        }

        private static bool IsIgnoredPath(string memberPath,
                                          ImmutableHashSet<string> ignoredPaths)
        {
            foreach (var ignoredPath in ignoredPaths)
            {
                if (memberPath.Equals(ignoredPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (memberPath.StartsWith(ignoredPath, StringComparison.OrdinalIgnoreCase))
                {
                    var nextIndex = ignoredPath.Length;

                    if (memberPath.Length > nextIndex && (memberPath[nextIndex] == '.' || memberPath[nextIndex] == '['))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Smart Replace bleibt wie zuvor
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

                var propertyName = key.Trim('$');

                ReplaceByProperty(root, propertyName, key,
                                  value);

                if (value != null)
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

            if (token is JValue value && value.Type == JTokenType.String)
            {
                var s = (string?)value.Value;

                if (s.IsNullOrWhiteSpace())
                {
                    return;
                }

                var escaped = Regex.Escape(originalValue.ToString()!);
                var pattern = $@"\b{escaped}\b";

                var updated = Regex.Replace(s,
                                            pattern,
                                            placeholder,
                                            RegexOptions.CultureInvariant);

                value.Value = updated.Replace(originalValue.ToString()!,
                                              placeholder,
                                              StringComparison.Ordinal);
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
