using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    /// <summary>
    ///     Extension method for registering the invalid JSON error handler.
    /// </summary>
    internal static class AddInvalidJsonErrorHandlerExtension
    {
        public static void AddInvalidJsonErrorHandler(this IServiceCollection services)
        {
            services.AddCurlBuilder();
            services.AddCurlFormatter();
            services.AddSourceCodeExtractor();
            services.AddEndpointSourceResolver();
            services.AddSingletonIfNotExists<ITestErrorHandler, InvalidJsonErrorHandler>();
        }
    }

    /// <summary>
    ///     Handles InvalidJsonException by showing what JSON was expected vs what was received.
    ///     This exception occurs when the JSON format is invalid (doesn't start with { or [).
    /// </summary>
    internal sealed class InvalidJsonErrorHandler(ICurlBuilder curlBuilder,
                                                  ICurlFormatter curlFormatter,
                                                  ISourceCodeExtractor sourceCodeExtractor,
                                                  IEndpointSourceResolver endpointSourceResolver)
        : TestErrorHandler<InvalidJsonException>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             InvalidJsonException exception)
        {
            // The whole message is built around the call that produced the json - without an http
            // context there is nothing to show, so the next compatible handler takes over.
            if (context is not IHttpAssertContext httpContext)
            {
                return Task.FromResult(string.Empty);
            }

            var errorOutput = BuildInvalidJsonError(httpContext, exception);

            return Task.FromResult(errorOutput);
        }

        /// <summary>
        ///     Builds a comprehensive error message for invalid JSON format.
        /// </summary>
        private string BuildInvalidJsonError(IHttpAssertContext context,
                                             InvalidJsonException exception)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine("❌ INVALID JSON FORMAT ERROR");
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            BuildHttpInfo(sb, context);
            sb.AppendLine();

            BuildJsonFormatError(sb, exception);
            sb.AppendLine();

            BuildJsonContent(sb, context, exception);
            sb.AppendLine();

            BuildExplanation(sb);
            sb.AppendLine();

            // Show the assert call if available
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                BuildAssertCall(sb, sourceCode);
                sb.AppendLine();
            }

            // Curl command for reproduction
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        private static void BuildTestInfo(StringBuilder sb,
                                          IHttpAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var fullClassName = GetFullClassName(context.CallerFilePath, projectName);

            sb.AppendLine("📦 Test Information");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Project",-15} : {projectName}");
            sb.AppendLine($"{"Class",-15} : {fullClassName}");
            sb.AppendLine($"{"Method",-15} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-15} : {context.CallerLineNumber}");
        }

        private static string GetFullClassName(string callerFilePath,
                                               string projectName)
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

        private void BuildHttpInfo(StringBuilder sb,
                                   IHttpAssertContext context)
        {
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            sb.AppendLine("🌍 HTTP Request/Response");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Method",-15} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"URL",-15} : {fullUrl}");

            var endpointSource = endpointSourceResolver.Resolve(context);

            if (endpointSource.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Endpoint",-15} : {endpointSource}");
            }

            sb.AppendLine($"{"Expected Type",-15} : {context.ExpectedType?.Name ?? "Unknown"}");
        }

        private static void BuildJsonFormatError(StringBuilder sb,
                                                 InvalidJsonException exception)
        {
            sb.AppendLine("⚠️ What Went Wrong");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("The response is not valid JSON format.");
            sb.AppendLine();
            sb.AppendLine("Valid JSON must:");
            sb.AppendLine("  • Start with '{' for objects  OR");
            sb.AppendLine("  • Start with '[' for arrays");
        }

        private void BuildJsonContent(StringBuilder sb,
                                      IHttpAssertContext context,
                                      InvalidJsonException exception)
        {
            sb.AppendLine();
            sb.AppendLine("📄 Response Content");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            // Extract the invalid string from the exception message (fallback if no HttpResponseContext)
            var invalidString = ExtractInvalidStringFromMessage(exception.Message);

            // Show response content if available (preferred)
            if (context is IHttpResponseContext httpResponseContext)
            {
                var responseContent = httpResponseContext.ContentAsString;

                if (responseContent.IsNotNullOrWhiteSpace())
                {
                    // Analyze what it looks like
                    var contentType = AnalyzeContentType(responseContent);
                    sb.AppendLine($"{"Detected Type",-15} : {contentType}");
                    sb.AppendLine($"{"Length",-15} : {responseContent.Length} characters");
                    sb.AppendLine();

                    // Show first few characters
                    var firstChars = GetFirstCharacters(responseContent, 100);
                    sb.AppendLine("First 100 characters:");
                    sb.AppendLine($"  \"{firstChars}\"");
                    sb.AppendLine();

                    // Show full preview if short enough
                    if (responseContent.Length <= 500)
                    {
                        sb.AppendLine("Full Content:");
                        var preview = GetContentPreview(responseContent, 500);
                        sb.AppendLine($"  {preview}");
                    }
                    else
                    {
                        sb.AppendLine($"(Content too long - {responseContent.Length} chars total)");
                    }
                }
                else
                {
                    sb.AppendLine("[Empty or null]");
                }
            }
            else if (invalidString.IsNotNullOrWhiteSpace())
            {
                // Fallback: Show the invalid string from exception message
                var contentType = AnalyzeContentType(invalidString);
                sb.AppendLine($"{"Detected Type",-15} : {contentType}");
                sb.AppendLine();

                var preview = GetContentPreview(invalidString, 200);
                sb.AppendLine($"  {preview}");
            }
            else
            {
                sb.AppendLine("[No content available]");
            }
        }

        private static void BuildExplanation(StringBuilder sb)
        {
            sb.AppendLine();
            sb.AppendLine("💡 Common Causes");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("  • The API returned an HTML error page (404, 500, etc.)");
            sb.AppendLine("  • Wrong endpoint URL or HTTP method");
            sb.AppendLine("  • The endpoint returned XML instead of JSON");
            sb.AppendLine("  • Empty or whitespace-only response");
            sb.AppendLine("  • Missing 'Accept: application/json' header");
            sb.AppendLine();
            sb.AppendLine("Next Steps:");
            sb.AppendLine("  1. Check the 'Detected Type' and 'Response Content' above");
            sb.AppendLine("  2. Verify the HTTP status code and URL");
            sb.AppendLine("  3. Use the curl command below to reproduce manually");
        }

        private static void BuildAssertCall(StringBuilder sb,
                                            string sourceCode)
        {
            sb.AppendLine("📝 Assert Call");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine(sourceCode);
        }

        /// <summary>
        ///     Extracts the invalid string from the exception message.
        /// </summary>
        private static string ExtractInvalidStringFromMessage(string message)
        {
            if (message.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // The message format: "...Your invalid string is:\n{content}"
            var marker = "Your invalid string is:";
            var index = message.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                var content = message.Substring(index + marker.Length).Trim();

                return content;
            }

            return string.Empty;
        }

        /// <summary>
        ///     Gets a preview of the content (first 200 chars).
        /// </summary>
        private static string GetContentPreview(string content,
                                                int maxLength = 200)
        {
            if (content.IsNullOrWhiteSpace())
            {
                return "[Empty]";
            }

            var trimmed = content.Trim();

            if (trimmed.Length <= maxLength)
            {
                return trimmed;
            }

            return trimmed[..maxLength] + "...";
        }

        /// <summary>
        ///     Gets first N characters of content.
        /// </summary>
        private static string GetFirstCharacters(string content,
                                                 int count)
        {
            if (content.IsNullOrWhiteSpace())
            {
                return "[Empty]";
            }

            if (content.Length <= count)
            {
                return content;
            }

            var trimmed = content.Trim();

            return trimmed[..count] + "...";
        }

        /// <summary>
        ///     Analyzes content and detects what type it likely is.
        /// </summary>
        private static string AnalyzeContentType(string content)
        {
            if (content.IsNullOrWhiteSpace())
            {
                return "Empty/Whitespace";
            }

            var trimmed = content.TrimStart();

            if (trimmed.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
            {
                return "HTML (probably an error page)";
            }

            if (trimmed.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith('<'))
            {
                return "XML";
            }

            if (trimmed.StartsWith('{'))
            {
                return "JSON Object (but parsing failed)";
            }

            if (trimmed.StartsWith('['))
            {
                return "JSON Array (but parsing failed)";
            }

            if (trimmed.All(char.IsDigit))
            {
                return "Numeric value";
            }

            if (trimmed.StartsWith('\"') && trimmed.EndsWith('\"'))
            {
                return "Quoted string";
            }

            if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return "Boolean value";
            }

            if (trimmed.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return "Null value";
            }

            return "Plain Text / Unknown format";
        }
    }
}