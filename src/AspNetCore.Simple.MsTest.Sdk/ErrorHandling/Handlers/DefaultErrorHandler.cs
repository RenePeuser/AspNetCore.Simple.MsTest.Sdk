using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    /// <summary>
    /// Extension method for registering the default error handler.
    /// </summary>
    internal static class AddDefaultErrorHandlerExtension
    {
        public static void AddDefaultErrorHandler(this IServiceCollection services)
        {
            // Default handler must be registered LAST so it acts as a catch-all
            services.AddTestClassNameResolver();
            services.AddEndpointSourceResolver();
            services.AddSingletonIfNotExists<ITestErrorHandler, DefaultErrorHandler>();
        }
    }

    /// <summary>
    /// Catch-all error handler for any exception that doesn't have a specific handler.
    /// This handles SDK bugs, network errors, serialization issues, and any other unexpected exceptions.
    /// </summary>
    public sealed class DefaultErrorHandler(ITestClassNameResolver testClassNameResolver,
                                            IEndpointSourceResolver endpointSourceResolver) : TestErrorHandler<Exception>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             Exception exception)
        {
            var errorOutput = BuildUnexpectedSdkError(context, exception);

            return Task.FromResult(errorOutput);
        }

        /// <summary>
        /// Builds a formatted error message for unexpected SDK errors (bugs, network issues, etc.)
        /// </summary>
        private string BuildUnexpectedSdkError(IObjectAssertContext context,
                                               Exception exception)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine("❌ UNEXPECTED TEST SDK ERROR");
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // The direct object route has no request and no response - printing empty http sections
            // there would only be noise.
            if (context is IHttpAssertContext httpContext)
            {
                BuildHttpInfo(sb, httpContext);
                sb.AppendLine();

                BuildResponseContent(sb, httpContext);
                sb.AppendLine();
            }

            BuildExceptionDetails(sb, exception);
            sb.AppendLine();

            BuildExplanation(sb);
            sb.AppendLine();

            sb.AppendLine("══════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        private void BuildTestInfo(StringBuilder sb,
                                   IObjectAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var fullClassName = testClassNameResolver.Resolve(context.CallerFilePath, projectName);

            sb.AppendLine("📦 Test Information");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {fullClassName}");
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private void BuildHttpInfo(StringBuilder sb,
                                   IHttpAssertContext context)
        {
            sb.AppendLine("🌍 HTTP Request");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            sb.AppendLine($"{"Method",-10} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"Url",-10} : {fullUrl}");

            var endpointSource = endpointSourceResolver.Resolve(context);

            if (endpointSource.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Endpoint",-10} : {endpointSource}");
            }
        }

        private static void BuildResponseContent(StringBuilder sb,
                                                 IHttpAssertContext context)
        {
            sb.AppendLine("📄 Response Content");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            // Try to extract response content if available
            if (context is IHttpResponseContext httpResponseContext)
            {
                var responseContent = httpResponseContext.ContentAsString;

                if (responseContent.IsNotNullOrWhiteSpace())
                {
                    sb.AppendLine($"{"Length",-10} : {responseContent.Length} characters");
                    sb.AppendLine();

                    // Show first 200 characters
                    var preview = responseContent.Length <= 200
                                      ? responseContent
                                      : string.Concat(responseContent.AsSpan(0, 200), "...");

                    sb.AppendLine("Preview (first 200 chars):");
                    sb.AppendLine($"  \"{preview}\"");
                }
                else
                {
                    sb.AppendLine("  [Empty or null]");
                }
            }
            else
            {
                // Reaching this means the call never came back - a connect error, a hang, an exception
                // thrown before the request went out.
                sb.AppendLine("  [The request did not produce a response]");
            }
        }

        private static void BuildExceptionDetails(StringBuilder sb,
                                                  Exception exception)
        {
            sb.AppendLine("⚠️ Exception Details");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Type",-10} : {exception.GetType().FullName}");
            sb.AppendLine($"{"Message",-10} : {exception.Message}");

            if (exception.InnerException.IsNotNull())
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"{"Type",-10} : {exception.InnerException.GetType().FullName}");
                sb.AppendLine($"{"Message",-10} : {exception.InnerException.Message}");
            }

            if (exception.StackTrace.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);
            }
        }

        private static void BuildExplanation(StringBuilder sb)
        {
            sb.AppendLine("💡 What This Means");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("An unexpected error occurred in the Test SDK. This could indicate:");
            sb.AppendLine();
            sb.AppendLine("  • A bug in the Test SDK itself");
            sb.AppendLine("  • Network connectivity issues");
            sb.AppendLine("  • Invalid test configuration");
            sb.AppendLine("  • Serialization/deserialization problems");
            sb.AppendLine("  • An error in the endpoint validation logic");
            sb.AppendLine();
            sb.AppendLine("If this appears to be a Test SDK bug, please report it with the");
            sb.AppendLine("exception details and stack trace shown above.");
        }
    }
}