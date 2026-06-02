using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            services.AddSingletonIfNotExists<ITestErrorHandler, InvalidJsonErrorHandler>();
        }
    }

    /// <summary>
    ///     Handles InvalidJsonException by showing what JSON was expected vs what was received.
    ///     This exception occurs when the JSON format is invalid (doesn't start with { or [).
    /// </summary>
    internal sealed class InvalidJsonErrorHandler(ICurlBuilder curlBuilder,
                                                  ICurlFormatter curlFormatter,
                                                  ISourceCodeExtractor sourceCodeExtractor)
        : TestErrorHandler<InvalidJsonException>
    {
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             InvalidJsonException exception)
        {
            var errorOutput = BuildInvalidJsonError(context, exception);

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

        private static void BuildHttpInfo(StringBuilder sb,
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
            sb.AppendLine($"{"Expected Type",-15} : {context.ExpectedType?.Name ?? "Unknown"}");
        }

        private static void BuildJsonFormatError(StringBuilder sb,
                                                 InvalidJsonException exception)
        {
            sb.AppendLine("⚠️ Format Validation Error");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("The provided content is not valid JSON format.");
            sb.AppendLine();
            sb.AppendLine("Valid JSON must:");
            sb.AppendLine("  • Start with '{' for objects  OR");
            sb.AppendLine("  • Start with '[' for arrays");
            sb.AppendLine();
            sb.AppendLine($"{"Error",-15} : {exception.Message}");
        }

        private void BuildJsonContent(StringBuilder sb,
                                      IHttpAssertContext context,
                                      InvalidJsonException exception)
        {
            sb.AppendLine("📄 Content Analysis");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            // Extract the invalid string from the exception message
            var invalidString = ExtractInvalidStringFromMessage(exception.Message);

            // Show response content if available
            if (context is IHttpResponseContext httpResponseContext)
            {
                var responseContent = httpResponseContext.ContentAsString;

                if (responseContent.IsNotNullOrWhiteSpace())
                {
                    sb.AppendLine("Response Content (Raw):");
                    var preview = GetContentPreview(responseContent);
                    sb.AppendLine($"  {preview}");
                    sb.AppendLine();

                    // Analyze what it looks like
                    var contentType = AnalyzeContentType(responseContent);
                    sb.AppendLine($"{"Detected Type",-15} : {contentType}");
                    sb.AppendLine();

                    // Show first few characters
                    var firstChars = GetFirstCharacters(responseContent, 50);
                    sb.AppendLine("First 50 characters:");
                    sb.AppendLine($"  \"{firstChars}\"");
                    sb.AppendLine();
                }
            }
            else if (invalidString.IsNotNullOrWhiteSpace())
            {
                // Show the invalid string from exception message
                sb.AppendLine("Invalid Content:");
                var preview = GetContentPreview(invalidString);
                sb.AppendLine($"  {preview}");
                sb.AppendLine();

                var contentType = AnalyzeContentType(invalidString);
                sb.AppendLine($"{"Detected Type",-15} : {contentType}");
                sb.AppendLine();
            }

            // Show expected JSON structure if available
            if (context.ResolvedExpectedJson.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine("Expected JSON structure (from file):");
                var expectedPreview = GetContentPreview(context.ResolvedExpectedJson);
                sb.AppendLine($"  {expectedPreview}");
            }
        }

        private static void BuildExplanation(StringBuilder sb)
        {
            sb.AppendLine("💡 What This Means");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("The response is not in valid JSON format. Common causes:");
            sb.AppendLine();
            sb.AppendLine("  • The API returned an HTML error page (404, 500, etc.)");
            sb.AppendLine("  • The response is plain text instead of JSON");
            sb.AppendLine("  • The endpoint returned XML instead of JSON");
            sb.AppendLine("  • Empty or whitespace-only response");
            sb.AppendLine("  • The response starts with a BOM (Byte Order Mark)");
            sb.AppendLine();
            sb.AppendLine("Suggestions:");
            sb.AppendLine("  1. Check the actual HTTP status code (might be an error)");
            sb.AppendLine("  2. Verify the endpoint URL is correct");
            sb.AppendLine("  3. Check if the API expects specific headers (Accept: application/json)");
            sb.AppendLine("  4. Look at the 'Response Content' above - is it HTML/XML/plain text?");
            sb.AppendLine("  5. Use the curl command below to test the endpoint manually");
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