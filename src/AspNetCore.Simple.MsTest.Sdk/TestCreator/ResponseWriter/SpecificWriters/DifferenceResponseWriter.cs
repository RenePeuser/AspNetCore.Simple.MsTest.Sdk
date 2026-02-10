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

            foreach (var (key, value) in context.Parameters)
            {
                var oldValue = value?.ToString();
                if (!oldValue.IsNullOrEmpty())
                {
                    formattedCurrent = formattedCurrent.Replace(oldValue, key);
                }
            }

            var expected = JToken.Parse(context.ExpectedResult.Content);
            var current = JToken.Parse(formattedCurrent);

            var diffs = jsonDiffer.FindDifferences(expected, current);
            var relevantDiffs = context.DifferenceFunc(diffs).ToList();

            if (!relevantDiffs.Any())
            {
                return;
            }

            foreach (var diff in relevantDiffs)
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
