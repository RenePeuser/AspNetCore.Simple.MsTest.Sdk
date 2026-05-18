using System.Text;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    /// <summary>
    /// Extension method for registering the JSON serialization error handler.
    /// </summary>
    internal static class AddJsonSerializationErrorHandlerExtension
    {
        public static void AddJsonSerializationErrorHandler(this IServiceCollection services)
        {
            services.AddTableBuilder();
            services.AddCurlBuilder();
            services.AddCurlFormatter();
            services.AddSourceCodeExtractor();
            services.AddSingletonIfNotExists<ITestErrorHandler, JsonSerializationErrorHandler>();
        }
    }

    /// <summary>
    /// Handles JsonException by showing detailed information about the JSON that failed to parse.
    /// This exception occurs when JSON deserialization fails due to invalid format, type mismatch, etc.
    /// </summary>
    internal sealed class JsonSerializationErrorHandler(ICurlBuilder curlBuilder,
                                                        ICurlFormatter curlFormatter,
                                                        ISourceCodeExtractor sourceCodeExtractor)
        : TestErrorHandler<JsonException>
    {
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             JsonException exception)
        {
            var errorOutput = BuildJsonSerializationError(context, exception);
            return Task.FromResult(errorOutput);
        }

        /// <summary>
        /// Builds a comprehensive error message for JSON serialization failures.
        /// </summary>
        private string BuildJsonSerializationError(IHttpAssertContext context, JsonException exception)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine("❌ JSON SERIALIZATION ERROR");
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            BuildHttpInfo(sb, context);
            sb.AppendLine();

            BuildJsonErrorDetails(sb, exception);
            sb.AppendLine();

            BuildJsonContent(sb, context);
            sb.AppendLine();

            BuildExplanation(sb, exception);
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

        private static void BuildTestInfo(StringBuilder sb, IHttpAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = Path.GetFileNameWithoutExtension(context.CallerFilePath).Replace(".cs", string.Empty);

            sb.AppendLine("📦 Test Information");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Project",-15} : {projectName}");
            sb.AppendLine($"{"Class",-15} : {className}");
            sb.AppendLine($"{"Method",-15} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-15} : {context.CallerLineNumber}");
        }

        private static void BuildHttpInfo(StringBuilder sb, IHttpAssertContext context)
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

        private static void BuildJsonErrorDetails(StringBuilder sb, JsonException exception)
        {
            sb.AppendLine("⚠️ JSON Error Details");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Error Message",-15} : {exception.Message}");

            // Try to extract JSON path from exception message
            var jsonPath = ExtractJsonPath(exception.Message);
            if (jsonPath.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"JSON Path",-15} : {jsonPath}");
            }

            // Try to extract line/position info
            var lineInfo = ExtractLineInfo(exception);
            if (lineInfo.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Position",-15} : {lineInfo}");
            }

            if (exception.InnerException.IsNotNull())
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"{"Type",-15} : {exception.InnerException.GetType().Name}");
                sb.AppendLine($"{"Message",-15} : {exception.InnerException.Message}");
            }
        }

        private void BuildJsonContent(StringBuilder sb, IHttpAssertContext context)
        {
            sb.AppendLine("📄 JSON Content");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();

            // Show request payload if available
            if (context.ResolvedPayload.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine("Request Payload:");
                var formattedPayload = TryFormatJson(context.ResolvedPayload);
                sb.AppendLine(IndentJson(formattedPayload, 2));
                sb.AppendLine();
            }

            // Show response content if available (from HttpResponseContext if it exists)
            if (context is IHttpResponseContext httpResponseContext)
            {
                var responseContent = httpResponseContext.ContentAsString;
                if (responseContent.IsNotNullOrWhiteSpace())
                {
                    sb.AppendLine("Response Content:");
                    var formattedResponse = TryFormatJson(responseContent);
                    sb.AppendLine(IndentJson(formattedResponse, 2));
                    sb.AppendLine();
                }
            }

            // Show expected JSON if available
            if (context.ResolvedExpectedJson.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine("Expected JSON (from file):");
                var formattedExpected = TryFormatJson(context.ResolvedExpectedJson);
                sb.AppendLine(IndentJson(formattedExpected, 2));
            }
        }

        private static void BuildExplanation(StringBuilder sb, JsonException exception)
        {
            sb.AppendLine("💡 What This Means");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("The JSON response could not be deserialized. Common causes:");
            sb.AppendLine();

            // Analyze exception message for specific guidance
            var message = exception.Message.ToLowerInvariant();

            if (message.Contains("unexpected character") || message.Contains("invalid token"))
            {
                sb.AppendLine("  • Invalid JSON format - check for missing quotes, commas, or brackets");
                sb.AppendLine("  • The response might not be valid JSON at all");
            }
            else if (message.Contains("could not convert") || message.Contains("cannot convert"))
            {
                sb.AppendLine("  • Type mismatch - the JSON value doesn't match the expected C# type");
                sb.AppendLine("  • Example: trying to parse \"abc\" as an integer");
            }
            else if (message.Contains("required property") || message.Contains("missing"))
            {
                sb.AppendLine("  • Missing required property in the JSON");
                sb.AppendLine("  • The expected type has [Required] properties that aren't in the response");
            }
            else if (message.Contains("duplicate"))
            {
                sb.AppendLine("  • Duplicate property names in the JSON");
            }
            else
            {
                sb.AppendLine("  • Type mismatch between JSON and expected C# type");
                sb.AppendLine("  • Invalid JSON format or structure");
                sb.AppendLine("  • Missing required properties");
                sb.AppendLine("  • Unexpected additional properties");
            }

            sb.AppendLine();
            sb.AppendLine("Suggestions:");
            sb.AppendLine("  1. Compare the Response Content with the Expected Type");
            sb.AppendLine("  2. Check if property names match (case-sensitive!)");
            sb.AppendLine("  3. Verify the response is actually JSON (not HTML error page)");
            sb.AppendLine("  4. Use a JSON validator to check the response format");
        }

        private static void BuildAssertCall(StringBuilder sb, string sourceCode)
        {
            sb.AppendLine("📝 Assert Call");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine(sourceCode);
        }

        /// <summary>
        /// Tries to format JSON, returns original if it fails.
        /// </summary>
        private static string TryFormatJson(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json;
            }

            try
            {
                // Try to parse and format
                using var doc = JsonDocument.Parse(json);
                using var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                doc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                // If it fails, return original (it's probably not valid JSON)
                return json;
            }
        }

        /// <summary>
        /// Indents each line of the JSON string.
        /// </summary>
        private static string IndentJson(string json, int spaces)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json;
            }

            var indent = new string(' ', spaces);
            var lines = json.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Environment.NewLine, lines.Select(line => indent + line));
        }

        /// <summary>
        /// Tries to extract JSON path from exception message.
        /// Example: "Error at $.data.items[0].name" -> "$.data.items[0].name"
        /// </summary>
        private static string ExtractJsonPath(string message)
        {
            if (message.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // Common patterns in JsonException messages
            var patterns = new[]
            {
                @"Path: ([\$\.\[\]\w]+)",           // "Path: $.data.items[0]"
                @"at path '([^']+)'",                // "at path '$.data.items[0]'"
                @"JSON path ([\$\.\[\]\w]+)",        // "JSON path $.data.items[0]"
                @"\$[\.\[\]\w]+"                     // Just the path itself: $.data.items[0]
            };

            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(message, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Success ? match.Groups[1].Value : match.Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Tries to extract line/position info from exception.
        /// </summary>
        private static string ExtractLineInfo(JsonException exception)
        {
            // JsonException has LineNumber and BytePositionInLine properties
            if (exception.LineNumber.HasValue)
            {
                var line = exception.LineNumber.Value;
                var pos = exception.BytePositionInLine ?? 0;
                return $"Line {line}, Position {pos}";
            }

            return string.Empty;
        }
    }
}