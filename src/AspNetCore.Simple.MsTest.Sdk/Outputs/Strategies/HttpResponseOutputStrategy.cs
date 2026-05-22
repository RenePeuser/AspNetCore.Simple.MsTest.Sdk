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

            // Build context-specific header based on failure type
            var (icon, title, failureInfo) = GetHeaderInfo(context);

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine(textDecorator.Error($"{icon} {title}"));
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine();

            // Add failure-specific information if available
            if (failureInfo.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
                stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(failureInfo);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{"Project",-10} : {projectName}");
            stringBuilder.AppendLine($"{"Class",-10} : {className}");
            stringBuilder.AppendLine($"{"Method",-10} : {methodName}");
            stringBuilder.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private static (string Icon, string Title, string? FailureInfo) GetHeaderInfo(IHttpResponseContext context)
        {
            return context.FailureType switch
            {
                HttpAssertionFailureType.SchemaMismatch => (
                    "📋",
                    "SCHEMA MISMATCH",
                    "Structure doesn't match expected type schema.\nProperties missing, extra properties, or type mismatches detected."
                ),

                HttpAssertionFailureType.StatusCodeMismatch => (
                    "🚫",
                    "UNEXPECTED STATUS CODE",
                    context.ExpectedStatusCode.HasValue && context.ActualStatusCode.HasValue
                        ? $"{"Expected",-10} : {context.ExpectedStatusCode} ({GetStatusCodeRange(context.ExpectedStatusCode.Value)})\n{"Actual",-10} : {context.ActualStatusCode} ({GetStatusText(context.ActualStatusCode.Value)})"
                        : "Status code doesn't match expected value."
                ),

                HttpAssertionFailureType.ContentTypeMismatch => (
                    "📄",
                    "CONTENT TYPE MISMATCH",
                    "The Content-Type header indicates non-JSON content (text/html, image/*, etc.)."
                ),

                HttpAssertionFailureType.SnapshotMismatch => (
                    "📸",
                    "SNAPSHOT MISMATCH",
                    "JSON values differ from the expected snapshot.\nAll properties exist but have different values."
                ),

                _ => (
                    "❌",
                    "API CONTRACT TEST FAILED",
                    null
                )
            };
        }

        private static string GetStatusCodeRange(int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => "Success",
                >= 400 and < 500 => "Client Error",
                >= 500 => "Server Error",
                _ => "Unknown"
            };
        }

        private static string GetStatusText(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                500 => "Internal Server Error",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                _ => string.Empty
            };
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
    }
}