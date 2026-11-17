using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
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
            services.AddTextDecoratorProvider();
            services.AddSingletonIfNotExists<ITestErrorHandler, DefaultErrorHandler>();
        }
    }

    /// <summary>
    /// Catch-all error handler for any exception that doesn't have a specific handler.
    /// This handles SDK bugs, network errors, serialization issues, and any other unexpected exceptions.
    /// </summary>
    internal sealed class DefaultErrorHandler(ITestClassNameResolver testClassNameResolver,
                                            IEndpointSourceResolver endpointSourceResolver,
                                            ITextDecoratorProvider textDecoratorProvider) : TestErrorHandler<Exception>
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
            var textDecorator = textDecoratorProvider.For(context.CallingAssembly);

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error(OutputSymbol.Failure, "UNEXPECTED TEST SDK ERROR"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, textDecorator, context);
            sb.AppendLine();

            // The direct object route has no request and no response - printing empty http sections
            // there would only be noise.
            if (context is IHttpAssertContext httpContext)
            {
                BuildHttpInfo(sb, textDecorator, httpContext);
                sb.AppendLine();

                BuildResponseContent(sb, textDecorator, httpContext);
                sb.AppendLine();
            }

            BuildExceptionDetails(sb, textDecorator, exception);
            sb.AppendLine();

            BuildExplanation(sb, textDecorator);
            sb.AppendLine();

            sb.AppendLine("══════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

        private void BuildTestInfo(StringBuilder sb,
                                   ITextDecorator textDecorator,
                                   IObjectAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var fullClassName = testClassNameResolver.Resolve(context.CallerFilePath, projectName);

            sb.AppendLine(textDecorator.SectionTitle(OutputSymbol.Information, "Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {fullClassName}");
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private void BuildHttpInfo(StringBuilder sb,
                                   ITextDecorator textDecorator,
                                   IHttpAssertContext context)
        {
            sb.AppendLine(textDecorator.SectionTitle(OutputSymbol.Http, "HTTP Request"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
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
                                                 ITextDecorator textDecorator,
                                                 IHttpAssertContext context)
        {
            sb.AppendLine(textDecorator.SectionTitle(OutputSymbol.Document, "Response Content"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
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
                                                  ITextDecorator textDecorator,
                                                  Exception exception)
        {
            sb.AppendLine(textDecorator.SectionTitle(OutputSymbol.Warning, "Exception Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
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

        private static void BuildExplanation(StringBuilder sb,
                                             ITextDecorator textDecorator)
        {
            sb.AppendLine(textDecorator.SectionTitle(OutputSymbol.Hint, "What This Means"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
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