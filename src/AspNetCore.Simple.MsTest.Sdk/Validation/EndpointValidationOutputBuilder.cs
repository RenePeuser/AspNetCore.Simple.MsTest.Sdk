using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddEndpointValidationOutputBuilderExtension
    {
        public static void AddEndpointValidationOutputBuilder(this IServiceCollection services)
        {
            services.AddCurlFormatter();
            services.AddSourceCodeExtractor();
            services.AddSingletonIfNotExists<IEndpointValidationOutputBuilder, EndpointValidationOutputBuilder>();
        }
    }

    public interface IEndpointValidationOutputBuilder
    {
        /// <summary>
        /// Builds error message for endpoint not found.
        /// </summary>
        string BuildEndpointNotFound(IHttpAssertContext context,
                                     ImmutableList<EndpointInfo> availableEndpoints);

        /// <summary>
        /// Builds error message for multiple matching endpoints.
        /// </summary>
        string BuildMultipleMatches(IHttpAssertContext context,
                                    ImmutableList<EndpointInfo> matchingEndpoints);

        /// <summary>
        /// Builds error message for response type mismatch.
        /// </summary>
        string BuildResponseTypeMismatch(IHttpAssertContext context,
                                         EndpointInfo endpoint,
                                         Type expectedType);
    }

    internal sealed class EndpointValidationOutputBuilder(
        ICurlFormatter curlFormatter,
        ISourceCodeExtractor sourceCodeExtractor) : IEndpointValidationOutputBuilder
    {
        public string BuildEndpointNotFound(IHttpAssertContext context,
                                            ImmutableList<EndpointInfo> availableEndpoints)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("ENDPOINT NOT FOUND");
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            BuildRequestInfo(sb, context);
            sb.AppendLine();

            sb.AppendLine("AVAILABLE ENDPOINTS");
            sb.AppendLine(" " + new string('-', 100));

            if (availableEndpoints.IsEmpty())
            {
                sb.AppendLine(" No endpoints registered");
            }
            else
            {
                var table = new ConsoleTable("Method", "URL", "API Version",
                                             "Response Type");

                foreach (var endpoint in availableEndpoints.OrderBy(e => e.Url).ThenBy(e => e.HttpMethod))
                {
                    var version = endpoint.ApiVersion?.ToString() ?? "N/A";
                    var responseType = endpoint.ResponseType?.Name ?? "N/A";

                    table.AddRow(endpoint.HttpMethod, endpoint.Url, version,
                                 responseType);
                }

                sb.AppendLine(table.ToMinimalString());
            }

            return sb.ToString();
        }

        public string BuildMultipleMatches(IHttpAssertContext context,
                                           ImmutableList<EndpointInfo> matchingEndpoints)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("AMBIGUOUS ENDPOINT MATCH");
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            BuildRequestInfo(sb, context);
            sb.AppendLine();

            sb.AppendLine("MATCHING ENDPOINTS");
            sb.AppendLine(" " + new string('-', 100));

            var table = new ConsoleTable("Method", "URL", "API Version",
                                         "Response Type");

            foreach (var endpoint in matchingEndpoints)
            {
                var version = endpoint.ApiVersion?.ToString() ?? "N/A";
                var responseType = endpoint.ResponseType?.Name ?? "N/A";

                table.AddRow(endpoint.HttpMethod, endpoint.Url, version,
                             responseType);
            }

            sb.AppendLine(table.ToMinimalString());
            sb.AppendLine();
            sb.AppendLine("SUMMARY");
            sb.AppendLine();
            sb.AppendLine($"Multiple endpoints matched the request. Found {matchingEndpoints.Count} candidates.");
            sb.AppendLine("Please ensure your endpoint routes are unique.");

            return sb.ToString();
        }

        public string BuildResponseTypeMismatch(IHttpAssertContext context,
                                                EndpointInfo endpoint,
                                                Type expectedType)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine("HTTP RESPONSE TYPE MISMATCH");
            sb.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // ASSERT CALL - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine("ASSERT CALL");
                sb.AppendLine();
                sb.AppendLine(new string('-', 75));
                sb.AppendLine(sourceCode);
                sb.AppendLine(new string('-', 75));
                sb.AppendLine();
            }

            // SUGGESTED FIX - Generate corrected code
            var actualEndpointTypeName = endpoint.ResponseType.IsNotNull() ? FormatTypeName(endpoint.ResponseType) : "object";
            var declaredTestTypeName = FormatTypeName(expectedType);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                var suggestedFix = sourceCode.Replace($"<{declaredTestTypeName}>", $"<{actualEndpointTypeName}>");

                sb.AppendLine("SUGGESTED FIX");
                sb.AppendLine();
                sb.AppendLine(new string('-', 75));
                sb.AppendLine(suggestedFix);
                sb.AppendLine(new string('-', 75));
                sb.AppendLine();
            }

            // TYPE VALIDATION Table
            var typeTable = new ConsoleTable { Options = { EnableCount = false } };
            typeTable.AddColumn(new[] { "Source", "Actual Endpoint Type", "Declared Test Type" });
            typeTable.AddRow("ResponseType", actualEndpointTypeName, declaredTestTypeName);

            sb.AppendLine("TYPE VALIDATION");
            sb.AppendLine();
            sb.Append(typeTable.ToString().TrimEnd());
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("SUMMARY");
            sb.AppendLine();
            sb.AppendLine($"The test declares response type '{declaredTestTypeName}', but the endpoint exposes");
            sb.AppendLine($"'{actualEndpointTypeName}' for HTTP 200 OK.");
            sb.AppendLine();
            sb.AppendLine("Suggested action:");
            sb.AppendLine("- Update the test response type to match the endpoint contract");
            sb.AppendLine();
            sb.AppendLine("Alternative:");
            sb.AppendLine("- If the endpoint contract is wrong, update the endpoint instead");
            sb.AppendLine();

            // Curl command
            var curl = BuildCurlCommand(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        private static void BuildTestInfo(StringBuilder sb,
                                          IHttpAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var classPath = BuildClassPath(context);
            var methodName = context.CallerMemberName;

            sb.AppendLine($"Project      : {projectName}");
            sb.AppendLine($"Class        : {classPath}");
            sb.AppendLine($"Method       : {methodName}");
            sb.AppendLine($"LineNumber   : {context.CallerLineNumber}");
        }

        private static string BuildClassPath(IHttpAssertContext context)
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

        private static void BuildRequestInfo(StringBuilder sb,
                                             IHttpAssertContext context)
        {
            var httpMethod = context.HttpMethod.Method;
            var url = context.Url;
            var apiVersion = context.ApiVersion.IsNotNullOrWhiteSpace() ? $" (API Version: {context.ApiVersion})" : string.Empty;

            sb.AppendLine("HTTP REQUEST");
            sb.AppendLine(" " + new string('-', 100));
            sb.AppendLine($" {httpMethod} {url}{apiVersion}");
        }

        private static void BuildHttpCallTable(StringBuilder sb,
                                               IHttpAssertContext context)
        {
            var table = new ConsoleTable { Options = { EnableCount = false } };

            // Add columns
            table.AddColumn(new[] { "HttpMethod", "Url" });

            // Build full URL from client base address
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            // Add data row
            table.AddRow(context.HttpMethod.Method, fullUrl);

            sb.AppendLine("HTTP CALL");
            sb.AppendLine();
            sb.Append(table.ToString().TrimEnd());
        }

        private static string FormatTypeName(Type type)
        {
            // Handle generic types like IEnumerable<Person> instead of IEnumerable`1
            if (type.IsGenericType.IsFalse())
            {
                return type.Name;
            }

            var typeName = type.Name;
            var backtickIndex = typeName.IndexOf('`');

            if (backtickIndex > 0)
            {
                typeName = typeName.Substring(0, backtickIndex);
            }

            var genericArgs = type.GetGenericArguments();
            var genericArgNames = string.Join(", ", genericArgs.Select(FormatTypeName));

            return $"{typeName}<{genericArgNames}>";
        }

        private static string BuildCurlCommand(IHttpAssertContext context)
        {
            var sb = new StringBuilder();

            // Build full URL
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            sb.Append("curl \\");
            sb.Append($"\n--location \\");
            sb.Append($"\n--request {context.HttpMethod.Method} '{fullUrl}'");

            // Add authorization header if present
            var authHeader = context.Client.DefaultRequestHeaders.Authorization;

            if (authHeader.IsNotNull())
            {
                sb.Append(" \\");
                sb.Append($"\n--header 'Authorization: {authHeader.Scheme} {authHeader.Parameter}'");
            }

            // Add payload if present
            if (context.ResolvedPayload.IsNotNullOrWhiteSpace())
            {
                sb.Append(" \\");
                sb.Append($"\n--header 'Content-Type: application/json' \\");
                sb.Append($"\n--data-raw '{context.ResolvedPayload}'");
            }

            return sb.ToString();
        }
    }
}
