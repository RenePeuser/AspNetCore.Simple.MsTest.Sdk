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
            var canHandle = context.Mode == ResponseWriteMode.DifferencesOnly &&
                            // A response file have to be exists
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

            // Parse + normalize current
            var currentJson = JToken.Parse(context.CurrentResponseAsString);
            var formattedCurrent = currentJson.ToString(Formatting.Indented);

            var sortedParameters = context.Parameters.Select(p => new
            {
                Key = p.key,
                Value = p.Value?.ToString()
            }).OrderByDescending(p => p.Value?.Length).ToList();

            foreach (var parameter in sortedParameters)
            {
                var oldValue = parameter.Value;
                if (!oldValue.IsNullOrEmpty())
                {
                    formattedCurrent = Regex.Replace(formattedCurrent,
                                                     $@"\b{Regex.Escape(oldValue)}\b",
                                                     parameter.Key);
                }
            }

            var expected = JToken.Parse(context.ExpectedResult.Content);
            var current = JToken.Parse(formattedCurrent);

            var diffs = jsonDiffer.FindDifferences(expected, current);
            var scopedDifferences = context.DifferenceFunc(diffs).ToImmutableList();
            // ToDo Static must go soon
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

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName, expected.ToString(Formatting.Indented));
        }
    }
}
