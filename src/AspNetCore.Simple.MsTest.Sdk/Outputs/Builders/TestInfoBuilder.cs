using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddTestInfoBuilderExtension
    {
        public static void AddTestInfoBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITestInfoBuilder, TestInfoBuilder>();
        }
    }

    public interface ITestInfoBuilder
    {
        /// <summary>
        /// Builds test information section from HTTP response context and differences.
        /// Includes: Project, Class, Method, Snapshot, Errors count, ErrorTypes.
        /// </summary>
        string Build<TResult>(HttpResponseContext<TResult> context,
                              ImmutableList<Difference> differences);
    }

    internal sealed class TestInfoBuilder : ITestInfoBuilder
    {
        public string Build<TResult>(HttpResponseContext<TResult> context,
                                     ImmutableList<Difference> differences)
        {
            var projectName = context.Request.CallingAssembly.GetName().Name ?? "Unknown";
            var classPath = context.Request.CallerFilePath;
            var snapshot = GetSnapshotName(context);
            var errorCount = differences.Count;
            var errorTypes = differences.Select(d => d.MismatchType).Distinct().ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SNAPSHOT TEST FAILED");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Project   : {projectName}");
            stringBuilder.AppendLine($"Class     : {classPath}");

            // Method name is not available from context - would need StackTrace or additional parameter
            // stringBuilder.AppendLine($"Method    : {methodName}");

            stringBuilder.AppendLine($"Snapshot  : {snapshot}");
            stringBuilder.AppendLine($"Errors    : {errorCount}");

            if (errorTypes.Any())
            {
                var errorTypesStr = string.Join(", ", errorTypes);
                stringBuilder.AppendLine($"ErrorTypes: {errorTypesStr}");
            }

            return stringBuilder.ToString();
        }

        private static string GetSnapshotName<TResult>(HttpResponseContext<TResult> context)
        {
            // Prefer ExpectedResult file name
            var expectedFileName = context.Request.ExpectedResultFile.EmbeddedFileName;

            if (expectedFileName.IsNotNullOrWhiteSpace())
            {
                return Path.GetFileName(expectedFileName);
            }

            // Fallback to Payload file name
            var payloadFileName = context.Request.PayloadFile?.EmbeddedFileName;

            if (payloadFileName.IsNotNullOrWhiteSpace())
            {
                return Path.GetFileName(payloadFileName);
            }

            return "N/A";
        }
    }
}
