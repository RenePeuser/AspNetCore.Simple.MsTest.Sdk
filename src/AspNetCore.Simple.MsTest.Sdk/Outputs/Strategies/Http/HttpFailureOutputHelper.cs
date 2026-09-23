using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    public static class AddHttpFailureOutputHelperExtension
    {
        /// <summary>
        /// Registers the HTTP failure output helper service.
        /// </summary>
        public static void AddHttpFailureOutputHelper(this IServiceCollection services)
        {
            services.AddTestContextHelper();
            services.AddSingletonIfNotExists<IHttpFailureOutputHelper, HttpFailureOutputHelper>();
        }
    }

    /// <summary>
    /// Provides helper methods for building HTTP failure output.
    /// This interface allows customers to override default formatting behavior.
    /// </summary>
    public interface IHttpFailureOutputHelper
    {
        /// <summary>
        /// Builds the standard Test Information section with project, class, method, line, file, and failure type.
        /// </summary>
        void BuildTestInfoSection(StringBuilder sb,
                                  IHttpResponseContext context,
                                  ITextDecorator textDecorator);

        /// <summary>
        /// Gets the status code range category (Success, Client Error, Server Error).
        /// </summary>
        string GetStatusCodeRange(int statusCode);

        /// <summary>
        /// Gets the human-readable status text for common HTTP status codes.
        /// </summary>
        string GetStatusText(int statusCode);
    }

    /// <summary>
    /// Default implementation of HTTP failure output helper.
    /// Provides common formatting and utility functions used across all failure strategies.
    /// </summary>
    internal sealed class HttpFailureOutputHelper(ITestContextHelper testContextHelper) : IHttpFailureOutputHelper
    {
        public void BuildTestInfoSection(StringBuilder sb,
                                         IHttpResponseContext context,
                                         ITextDecorator textDecorator)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = testContextHelper.ExtractFullyQualifiedClassName(context.CallerFilePath, context.CallingAssembly);
            var methodName = context.CallerMemberName;
            var fileUri = $"file:///{context.CallerFilePath.Replace('\\', '/')}:{context.CallerLineNumber}";

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {className}");
            sb.AppendLine($"{"Method",-10} : {methodName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine($"{"File",-10} : {fileUri}");
            sb.AppendLine($"{"Failure",-10} : {GetFailureTypeText(context.FailureType)}");
            sb.AppendLine();
        }

        public string GetStatusCodeRange(int statusCode)
        {
            return statusCode switch
            {
                >= 200 and < 300 => "Success",
                >= 400 and < 500 => "Client Error",
                >= 500 => "Server Error",
                _ => "Unknown"
            };
        }

        public string GetStatusText(int statusCode)
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

        private static string GetFailureTypeText(HttpAssertionFailureType failureType)
        {
            return failureType switch
            {
                HttpAssertionFailureType.None => "None",
                HttpAssertionFailureType.SchemaMismatch => "Schema Mismatch",
                HttpAssertionFailureType.StatusCodeMismatch => "Status Code Mismatch",
                HttpAssertionFailureType.SnapshotMismatch => "Snapshot Mismatch",
                HttpAssertionFailureType.ContentTypeMismatch => "Content Type Mismatch",
                _ => failureType.ToString()
            };
        }
    }
}