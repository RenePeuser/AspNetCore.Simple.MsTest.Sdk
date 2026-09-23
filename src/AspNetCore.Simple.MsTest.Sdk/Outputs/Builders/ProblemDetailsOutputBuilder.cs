using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddProblemDetailsOutputBuilderExtension
    {
        public static void AddProblemDetailsOutputBuilder(this IServiceCollection services)
        {
            services.AddTableBuilder();
            services.AddCurlBuilder();
            services.AddCurlFormatter();
            services.AddSourceCodeExtractor();
            services.AddTestContextHelper();
            services.AddEndpointSourceResolver();
            services.AddSingletonIfNotExists<IProblemDetailsOutputBuilder, ProblemDetailsOutputBuilder>();
        }
    }

    public interface IProblemDetailsOutputBuilder
    {
        /// <summary>
        /// Builds error message for unexpected ProblemDetails exception during deserialization.
        /// </summary>
        string BuildUnexpectedError(IHttpAssertContext context,
                                    TestSdkProblemDetailsException exception,
                                    EndpointInfo? endpoint = null);
    }

    internal sealed class ProblemDetailsOutputBuilder(ITableBuilder tableBuilder,
                                                      ICurlBuilder curlBuilder,
                                                      ICurlFormatter curlFormatter,
                                                      ISourceCodeExtractor sourceCodeExtractor,
                                                      ITextDecorator textDecorator,
                                                      ITestContextHelper testContextHelper,
                                                      IEndpointSourceResolver endpointSourceResolver) : IProblemDetailsOutputBuilder
    {
        public string BuildUnexpectedError(IHttpAssertContext context,
                                           TestSdkProblemDetailsException exception,
                                           EndpointInfo? endpoint = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ UNEXPECTED API ERROR"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            BuildHttpInfo(sb, context, exception,
                          endpoint);

            sb.AppendLine();

            BuildProblemDetailsInfo(sb, exception);
            sb.AppendLine();

            // Only show extension data if there are extensions
            if (exception.ProblemDetails.Extensions.Count > 0)
            {
                BuildExtensionData(sb, exception);
                sb.AppendLine();
            }

            BuildExplanation(sb);
            sb.AppendLine();

            // Show the assert call if available
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                BuildAssertCall(sb, sourceCode);
                sb.AppendLine();
            }

            // Curl command
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        private void BuildTestInfo(StringBuilder sb,
                                   IHttpAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = testContextHelper.ExtractFullyQualifiedClassName(context.CallerFilePath, context.CallingAssembly);
            var methodName = context.CallerMemberName;

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            var fileUri = $"file:///{context.CallerFilePath.Replace('\\', '/')}:{context.CallerLineNumber}";
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {className}");
            sb.AppendLine($"{"Method",-10} : {methodName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine($"{"File",-10} : {fileUri}");
        }

        private void BuildHttpInfo(StringBuilder sb,
                                   IHttpAssertContext context,
                                   TestSdkProblemDetailsException exception,
                                   EndpointInfo? endpoint)
        {
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            var statusCode = exception.ProblemDetails.Status ?? 500;
            var statusText = GetStatusText(statusCode);

            sb.AppendLine(textDecorator.SectionTitle("🌍 HTTP"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"Url",-10} : {fullUrl}");
            sb.AppendLine($"{"Status",-10} : {DecorateStatusCode(statusCode, statusText)}");

            var endpointSource = endpointSourceResolver.Resolve(context, endpoint);

            if (endpointSource.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Endpoint",-10} : {endpointSource}");
            }
        }

        private void BuildProblemDetailsInfo(StringBuilder sb,
                                             TestSdkProblemDetailsException exception)
        {
            var problemDetails = exception.ProblemDetails;

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            if (problemDetails.Status.HasValue)
            {
                sb.AppendLine($"{"Status",-10} : {problemDetails.Status}");
            }

            if (problemDetails.Title.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Title",-10} : {problemDetails.Title}");
            }

            if (problemDetails.Detail.IsNotNullOrWhiteSpace())
            {
                // Word wrap the detail if it's long
                var detailLines = WrapText(problemDetails.Detail, 60);
                sb.AppendLine($"{"Detail",-10} : {detailLines[0]}");

                for (var i = 1; i < detailLines.Count; i++)
                {
                    sb.AppendLine($"{"",-10}   {detailLines[i]}");
                }
            }

            if (problemDetails.Type.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine();
                sb.AppendLine($"{"Type",-10} : {problemDetails.Type}");
            }

            if (problemDetails.Instance.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Instance",-10} : {problemDetails.Instance}");
            }
        }

        private void BuildExtensionData(StringBuilder sb,
                                        TestSdkProblemDetailsException exception)
        {
            var extensions = exception.ProblemDetails.Extensions;

            sb.AppendLine(textDecorator.SectionTitle("📋 Extension Data"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var columns = new[] { "Key", "Value" };

            var rows = new List<object[]>();

            foreach (var kvp in extensions.OrderBy(e => e.Key))
            {
                var value = kvp.Value?.ToString() ?? "null";

                rows.Add(new object[] { kvp.Key, value });
            }

            var table = tableBuilder.BuildTable(columns, rows, enableCount: false);
            sb.Append(table.TrimEnd());
            sb.AppendLine();
        }

        private void BuildExplanation(StringBuilder sb)
        {
            sb.AppendLine(textDecorator.SectionTitle("💡 What This Means"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("The API endpoint returned an error response (ProblemDetails), but the test");
            sb.AppendLine("expected a different response type. This typically indicates:");
            sb.AppendLine();
            sb.AppendLine("  • The endpoint encountered an unexpected error");
            sb.AppendLine("  • The test's expected response type doesn't match what the API returned");
            sb.AppendLine("  • There may be a validation or server-side processing issue");
        }

        private void BuildAssertCall(StringBuilder sb,
                                     string sourceCode)
        {
            sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(sourceCode);
        }

        private string DecorateStatusCode(int statusCode,
                                          string statusText)
        {
            var fullText = $"{statusCode} {statusText}";

            if (statusCode is >= 200 and < 300)
            {
                return textDecorator.Success(fullText);
            }

            if (statusCode is >= 400 and < 500)
            {
                return textDecorator.SectionTitle(fullText);
            }

            if (statusCode >= 500)
            {
                return textDecorator.Error(fullText);
            }

            return textDecorator.Highlight(fullText);
        }

        private static string GetStatusText(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                429 => "Too Many Requests",
                500 => "Internal Server Error",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                _ => "Error"
            };
        }

        private static List<string> WrapText(string text,
                                             int maxLength)
        {
            var lines = new List<string>();

            if (text.IsNullOrWhiteSpace())
            {
                return lines;
            }

            var words = text.Split(' ');
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxLength && currentLine.Length > 0)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                }

                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                }

                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString());
            }

            return lines;
        }
    }
}