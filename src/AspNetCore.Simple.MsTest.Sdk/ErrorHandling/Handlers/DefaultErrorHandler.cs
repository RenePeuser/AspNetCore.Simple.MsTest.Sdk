using System.Text;
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
            services.AddSingletonIfNotExists<ITestErrorHandler, DefaultErrorHandler>();
        }
    }

    /// <summary>
    /// Catch-all error handler for any exception that doesn't have a specific handler.
    /// This handles SDK bugs, network errors, serialization issues, and any other unexpected exceptions.
    /// </summary>
    internal sealed class DefaultErrorHandler : TestErrorHandler<Exception>
    {
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             Exception exception)
        {
            var errorOutput = BuildUnexpectedSdkError(context, exception);
            return Task.FromResult(errorOutput);
        }

        /// <summary>
        /// Builds a formatted error message for unexpected SDK errors (bugs, network issues, etc.)
        /// </summary>
        private static string BuildUnexpectedSdkError(IHttpAssertContext context, Exception exception)
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

            BuildHttpInfo(sb, context);
            sb.AppendLine();

            BuildExceptionDetails(sb, exception);
            sb.AppendLine();

            BuildExplanation(sb);
            sb.AppendLine();

            sb.AppendLine("══════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        private static void BuildTestInfo(StringBuilder sb, IHttpAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var fullClassName = GetFullClassName(context.CallerFilePath, projectName);

            sb.AppendLine("📦 Test Information");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {fullClassName}");
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private static string GetFullClassName(string callerFilePath, string projectName)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
                var pathSegments = callerFilePath.Replace("\\", "/").Split('/');
                var projectIndex = Array.FindIndex(pathSegments, s => s.Equals(projectName, StringComparison.OrdinalIgnoreCase));

                if (projectIndex >= 0 && projectIndex < pathSegments.Length - 1)
                {
                    var namespaceParts = pathSegments.Skip(projectIndex + 1).Take(pathSegments.Length - projectIndex - 2).ToList();

                    if (namespaceParts.Count > 0)
                    {
                        var namespaceStr = string.Join(".", namespaceParts.Select(s => s.Replace(" ", "")));
                        return $"{projectName}.{namespaceStr}.{fileName}";
                    }

                    return $"{projectName}.{fileName}";
                }

                return fileName;
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                // Fallback to full caller file path on any error
                return callerFilePath;
            }
        }

        private static void BuildHttpInfo(StringBuilder sb, IHttpAssertContext context)
        {
            sb.AppendLine("🌍 HTTP Request");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            sb.AppendLine($"{"Method",-10} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"Url",-10} : {fullUrl}");
        }

        private static void BuildExceptionDetails(StringBuilder sb, Exception exception)
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