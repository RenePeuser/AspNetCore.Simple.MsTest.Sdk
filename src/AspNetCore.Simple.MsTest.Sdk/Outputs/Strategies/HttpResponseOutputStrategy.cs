using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpResponseOutputStrategyExtension
    {
        public static void AddHttpResponseOutputStrategy(this IServiceCollection services)
        {
            // Register dependencies - all the builders needed for HTTP report
            services.AddHttpCallInfoTableBuilder();
            services.AddDifferencesTableBuilder();
            services.AddJsonSectionBuilder();
            services.AddCurlBuilder();
            services.AddCurlFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, HttpResponseOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for HTTP response contexts.
    /// Builds comprehensive HTTP assertion failure output including test info,
    /// HTTP call details, differences, JSON comparison, and curl reproduction.
    /// </summary>
    internal sealed class HttpResponseOutputStrategy(IHttpCallInfoTableBuilder httpCallInfoTableBuilder,
                                                     IDifferencesTableBuilder differencesTableBuilder,
                                                     IJsonSectionBuilder jsonSectionBuilder,
                                                     ICurlBuilder curlBuilder,
                                                     ICurlFormatter curlFormatter)
        : AssertOutputStrategyBase<IHttpResponseContext>
    {
        protected override string BuildOutput(IHttpResponseContext context,
                                              ImmutableList<Difference> differences,
                                              string expectedJson,
                                              string currentJson)
        {
            var stringBuilder = new StringBuilder();

            // Section 1: Header + Test Info
            BuildHeader(stringBuilder, context, differences);
            stringBuilder.AppendLine();

            // Section 2: HTTP Call Table
            var httpCallInfo = httpCallInfoTableBuilder.Build(context);
            stringBuilder.AppendLine(httpCallInfo);
            stringBuilder.AppendLine();

            // Section 3: Differences Table (if any)
            var differencesTable = differencesTableBuilder.Build(context, differences);

            if (differencesTable.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(differencesTable);
                stringBuilder.AppendLine();
            }

            // Section 4: Expected Result
            var expectedSection = jsonSectionBuilder.BuildExpected(context, expectedJson);
            stringBuilder.AppendLine(expectedSection);
            stringBuilder.AppendLine();

            // Section 5: Current Result
            var currentSection = jsonSectionBuilder.BuildCurrent(currentJson);
            stringBuilder.AppendLine(currentSection);
            stringBuilder.AppendLine();

            // Section 6: Curl
            var curl = curlBuilder.BuildFrom(context);

            if (curl.IsNotNullOrWhiteSpace())
            {
                var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
                stringBuilder.AppendLine(curlFormatted);
            }

            return stringBuilder.ToString();
        }

        private static void BuildHeader(StringBuilder stringBuilder,
                                        IHttpResponseContext context,
                                        ImmutableList<Difference> differences)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var classPath = Path.GetFileName(context.CallerFilePath);
            var requestName = GetRequestName(context);
            var responseName = GetResponseName(context);
            var errorCount = differences.Count;
            var errorTypes = differences.Select(d => d.MismatchType).Distinct().ToList();

            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine("SNAPSHOT TEST FAILED");
            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Project   : {projectName}");
            stringBuilder.AppendLine($"Class     : {classPath}");
            stringBuilder.AppendLine($"Request   : {requestName}");
            stringBuilder.AppendLine($"Response  : {responseName}");
            stringBuilder.AppendLine($"Errors    : {errorCount}");

            if (errorTypes.Any())
            {
                var errorTypesStr = string.Join(", ", errorTypes);
                stringBuilder.AppendLine($"ErrorTypes: {errorTypesStr}");
            }
        }

        private static string GetRequestName(IHttpResponseContext context)
        {
            var payloadFileName = context.PayloadFile?.EmbeddedFile?.Name;

            if (payloadFileName.IsNotNullOrWhiteSpace())
            {
                return payloadFileName;
            }

            return "N/A";
        }

        private static string GetResponseName(IHttpResponseContext context)
        {
            var expectedFileName = context.ExpectedResultFile.EmbeddedFile?.Name;

            if (expectedFileName.IsNotNullOrWhiteSpace())
            {
                return expectedFileName;
            }

            return "Expected";
        }
    }
}
