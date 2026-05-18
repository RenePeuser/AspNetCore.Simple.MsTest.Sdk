using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
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

            // Note: ITextDecorator is registered separately based on build configuration

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
                                                     ICurlFormatter curlFormatter,
                                                     ITextDecorator textDecorator)
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

        private void BuildHeader(StringBuilder stringBuilder,
                                 IHttpResponseContext context,
                                 ImmutableList<Difference> differences)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = ExtractClassName(context);
            var methodName = context.CallerMemberName;
            var requestName = GetRequestName(context);
            var responseName = GetResponseName(context);
            var errorCount = differences.Count;

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine(textDecorator.Error("❌ SNAPSHOT TEST FAILED"));
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{"Project",-10} : {projectName}");
            stringBuilder.AppendLine($"{"Class",-10} : {className}");
            stringBuilder.AppendLine($"{"Method",-10} : {methodName}");
            stringBuilder.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private static string GetRequestName(IHttpResponseContext context)
        {
            if (context.PayloadFile.IsNull())
            {
                return "N/A";
            }

            if (context.PayloadFile.EmbeddedFile.IsNull())
            {
                return context.PayloadFile.Content;
            }

            if (context.PayloadFile.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return context.PayloadFile.Content;
            }

            return context.PayloadFile.EmbeddedFileName;
        }

        private static string GetResponseName(IHttpResponseContext context)
        {
            if (context.ExpectedResultFile.EmbeddedFile.IsNull())
            {
                return context.ExpectedResultFile.Content;
            }

            if (context.ExpectedResultFile.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return context.ExpectedResultFile.Content;
            }

            return context.ExpectedResultFile.EmbeddedFileName;
        }

        private static string ExtractClassName(IObjectAssertContext context)
        {
            var callerFilePath = context.CallerFilePath;
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);

            // Remove .cs extension if present
            return fileName.Replace(".cs", string.Empty);
        }

        private static string BuildClassPath(IObjectAssertContext context)
        {
            var assemblyName = context.CallingAssembly.GetName().Name;
            var callerFilePath = context.CallerFilePath;

            if (assemblyName.IsNullOrWhiteSpace())
            {
                return Path.GetFileName(callerFilePath);
            }

            // Try to find assembly name in path
            var assemblyIndex = callerFilePath.IndexOf(assemblyName, StringComparison.OrdinalIgnoreCase);

            if (assemblyIndex >= 0)
            {
                // Found! Build namespace-style path
                var relativePath = callerFilePath.Substring(assemblyIndex + assemblyName.Length)
                                                 .TrimStart('\\', '/');

                return $"{assemblyName}.{relativePath.Replace('\\', '.').Replace('/', '.')}";
            }

            // Fallback: Original path
            return callerFilePath;
        }
    }
}
