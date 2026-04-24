using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddEndpointValidationOutputBuilderExtension
    {
        public static void AddEndpointValidationOutputBuilder(this IServiceCollection services)
        {
            services.AddCurlBuilder();
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

        /// <summary>
        /// Builds error message for response type mismatch with status code details.
        /// </summary>
        string BuildResponseTypeMismatch(IHttpAssertContext context,
                                         EndpointInfo endpoint,
                                         Type expectedType,
                                         ImmutableDictionary<int, Type> relevantStatusCodes);
    }

    internal sealed class EndpointValidationOutputBuilder(ICurlBuilder curlBuilder,
                                                          ICurlFormatter curlFormatter,
                                                          ISourceCodeExtractor sourceCodeExtractor,
                                                          ITextDecorator textDecorator) : IEndpointValidationOutputBuilder
    {
        public string BuildEndpointNotFound(IHttpAssertContext context,
                                            ImmutableList<EndpointInfo> availableEndpoints)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            AppendErrorBanner(sb, "ENDPOINT NOT FOUND");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "404 NotFound");
            sb.AppendLine();
            sb.AppendLine();

            // Curl command
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        public string BuildMultipleMatches(IHttpAssertContext context,
                                           ImmutableList<EndpointInfo> matchingEndpoints)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            AppendErrorBanner(sb, "AMBIGUOUS ENDPOINT MATCH");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "Ambiguous");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("MATCHING ENDPOINTS"));
            sb.AppendLine(textDecorator.Dim(" " + new string('-', 100)));

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
            sb.AppendLine(textDecorator.SectionTitle("SUMMARY"));
            sb.AppendLine();
            sb.AppendLine($"Multiple endpoints matched the request. Found {textDecorator.Error(matchingEndpoints.Count.ToString())} candidates.");
            sb.AppendLine("Please ensure your endpoint routes are unique.");
            sb.AppendLine();

            // Curl command
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        public string BuildResponseTypeMismatch(IHttpAssertContext context,
                                                EndpointInfo endpoint,
                                                Type expectedType)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            AppendErrorBanner(sb, "HTTP RESPONSE TYPE MISMATCH");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // ASSERT CALL - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("ASSERT CALL"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine(sourceCode);
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine();
            }

            // SUGGESTED FIX - Generate corrected code
            var actualEndpointTypeName = endpoint.ResponseType.IsNotNull() ? FormatTypeName(endpoint.ResponseType) : "object";
            var declaredTestTypeName = FormatTypeName(expectedType);
            var endpointReturnsVoid = endpoint.ResponseType.IsNull();

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                string suggestedFix;

                // Check if source code already has generic type parameter
                if (sourceCode.Contains($"<{declaredTestTypeName}>"))
                {
                    if (endpointReturnsVoid)
                    {
                        // Endpoint returns void (204 NoContent) - remove type parameter
                        suggestedFix = sourceCode.Replace($"<{declaredTestTypeName}>", string.Empty);
                    }
                    else
                    {
                        // Replace existing type parameter
                        suggestedFix = sourceCode.Replace($"<{declaredTestTypeName}>", $"<{actualEndpointTypeName}>");
                    }
                }
                else
                {
                    if (endpointReturnsVoid)
                    {
                        // Already non-generic and endpoint returns void - correct!
                        suggestedFix = sourceCode;
                    }
                    else
                    {
                        // Non-generic call - need to add type parameter
                        // Find method name (AssertGetAsync, AssertPostAsync, etc.)
                        var methodMatch = GlobalRegex.AssertMethodPattern().Match(sourceCode);
                        if (methodMatch.Success)
                        {
                            var methodName = methodMatch.Groups[1].Value;
                            suggestedFix = sourceCode.Replace($"{methodName}(", $"{methodName}<{actualEndpointTypeName}>(");
                        }
                        else
                        {
                            suggestedFix = sourceCode; // Fallback - no change
                        }
                    }
                }

                sb.AppendLine(textDecorator.SectionTitle("SUGGESTED FIX"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine(textDecorator.Success(suggestedFix));
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine();
            }

            // TYPE VALIDATION Table
            var typeTable = new ConsoleTable { Options = { EnableCount = false } };
            typeTable.AddColumn(new[] { "Source", "Actual Endpoint Type", "Declared Test Type" });
            typeTable.AddRow("ResponseType", actualEndpointTypeName, declaredTestTypeName);

            sb.AppendLine(textDecorator.SectionTitle("TYPE VALIDATION"));
            sb.AppendLine();
            sb.Append(typeTable.ToString().TrimEnd());
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("SUMMARY"));
            sb.AppendLine();
            sb.AppendLine($"The test declares response type '{textDecorator.Error(declaredTestTypeName)}', but the endpoint exposes");
            sb.AppendLine($"'{textDecorator.Success(actualEndpointTypeName)}' for HTTP 200 OK.");
            sb.AppendLine();
            sb.AppendLine(textDecorator.SectionTitle("Suggested action:"));
            sb.AppendLine(textDecorator.Success("- Update the test response type to match the endpoint contract"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.SectionTitle("Alternative:"));
            sb.AppendLine(textDecorator.Highlight("- If the endpoint contract is wrong, update the endpoint instead"));
            sb.AppendLine();

            // Curl command
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }

        public string BuildResponseTypeMismatch(IHttpAssertContext context,
                                                EndpointInfo endpoint,
                                                Type expectedType,
                                                ImmutableDictionary<int, Type> relevantStatusCodes)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            AppendErrorBanner(sb, "HTTP RESPONSE TYPE MISMATCH");
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // ASSERT CALL - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("ASSERT CALL"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine(sourceCode);
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine();
            }

            // SUGGESTED FIX - Generate corrected code
            var declaredTestTypeName = FormatTypeName(expectedType);
            var isSuccessTest = context.IsSuccessStatusCode;

            // Find first matching type as suggestion
            var firstRelevantType = relevantStatusCodes.FirstOrDefault().Value;
            var suggestedTypeName = firstRelevantType.IsNotNull() ? FormatTypeName(firstRelevantType) : "object";
            var endpointReturnsVoid = firstRelevantType.IsNull();

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                string suggestedFix;

                // Check if source code already has generic type parameter
                if (sourceCode.Contains($"<{declaredTestTypeName}>"))
                {
                    if (endpointReturnsVoid)
                    {
                        // Endpoint returns void (204 NoContent) - remove type parameter
                        suggestedFix = sourceCode.Replace($"<{declaredTestTypeName}>", string.Empty);
                    }
                    else
                    {
                        // Replace existing type parameter
                        suggestedFix = sourceCode.Replace($"<{declaredTestTypeName}>", $"<{suggestedTypeName}>");
                    }
                }
                else
                {
                    if (endpointReturnsVoid)
                    {
                        // Already non-generic and endpoint returns void - correct!
                        suggestedFix = sourceCode;
                    }
                    else
                    {
                        // Non-generic call - need to add type parameter
                        // Find method name (AssertGetAsync, AssertPostAsync, etc.)
                        var methodMatch = GlobalRegex.AssertMethodPattern().Match(sourceCode);
                        if (methodMatch.Success)
                        {
                            var methodName = methodMatch.Groups[1].Value;
                            suggestedFix = sourceCode.Replace($"{methodName}(", $"{methodName}<{suggestedTypeName}>(");
                        }
                        else
                        {
                            suggestedFix = sourceCode; // Fallback - no change
                        }
                    }
                }

                sb.AppendLine(textDecorator.SectionTitle("SUGGESTED FIX"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine(textDecorator.Success(suggestedFix));
                sb.AppendLine(textDecorator.Dim(new string('-', 75)));
                sb.AppendLine();
            }

            // TYPE VALIDATION Table - Show all relevant status codes
            sb.AppendLine(textDecorator.SectionTitle("TYPE VALIDATION"));
            sb.AppendLine();

            var typeTable = new ConsoleTable { Options = { EnableCount = false } };

            typeTable.AddColumn(new[]
                                {
                                    "Status Code", "Endpoint Response Type", "Declared Test Type",
                                    "Match"
                                });

            foreach (var statusCode in relevantStatusCodes.OrderBy(kvp => kvp.Key))
            {
                var actualTypeName = FormatTypeName(statusCode.Value);
                var isMatch = statusCode.Value == expectedType ? "✓" : "✗";

                typeTable.AddRow(statusCode.Key.ToString(), actualTypeName, declaredTestTypeName,
                                 isMatch);
            }

            sb.Append(typeTable.ToString().TrimEnd());
            sb.AppendLine();
            sb.AppendLine();

            // SUMMARY
            sb.AppendLine(textDecorator.SectionTitle("SUMMARY"));
            sb.AppendLine();

            var testTypeDescription = isSuccessTest ? "success (2xx)" : "error (4xx/5xx)";
            sb.AppendLine($"The test is a {testTypeDescription} test and declares response type '{textDecorator.Error(declaredTestTypeName)}',");
            sb.AppendLine($"but none of the endpoint's {testTypeDescription} status codes return this type.");
            sb.AppendLine();

            if (relevantStatusCodes.Count == 1)
            {
                var singleStatus = relevantStatusCodes.First();
                sb.AppendLine($"Endpoint defines: {singleStatus.Key} → {FormatTypeName(singleStatus.Value)}");
            }
            else
            {
                sb.AppendLine("Endpoint defines:");

                foreach (var statusCode in relevantStatusCodes.OrderBy(kvp => kvp.Key))
                {
                    sb.AppendLine($"  - {statusCode.Key} → {FormatTypeName(statusCode.Value)}");
                }
            }

            sb.AppendLine();
            sb.AppendLine(textDecorator.SectionTitle("Suggested action:"));
            sb.AppendLine(textDecorator.Success($"- Update the test response type to '{suggestedTypeName}' to match one of the status codes above"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.SectionTitle("Alternative:"));
            sb.AppendLine(textDecorator.Highlight("- If the endpoint contract is wrong, update the endpoint's ProducesResponseType attributes"));
            sb.AppendLine();

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
            var classPath = BuildClassPath(context);
            var methodName = context.CallerMemberName;

            sb.AppendLine($"{textDecorator.Highlight("Project")}      : {projectName}");
            sb.AppendLine($"{textDecorator.Highlight("Class")}        : {classPath}");
            sb.AppendLine($"{textDecorator.Highlight("Method")}       : {methodName}");
            sb.AppendLine($"{textDecorator.Highlight("LineNumber")}   : {context.CallerLineNumber}");
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

        private void BuildHttpCallTable(StringBuilder sb,
                                        IHttpAssertContext context,
                                        string statusCode)
        {
            var table = new ConsoleTable { Options = { EnableCount = false } };

            // Add columns
            table.AddColumn(new[] { "HttpMethod", "Url", "HttpStatusCode" });

            // Build full URL from client base address
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            // Add data row
            table.AddRow(context.HttpMethod.Method, fullUrl, statusCode);

            sb.AppendLine(textDecorator.SectionTitle("HTTP CALL"));
            sb.AppendLine();
            sb.AppendLine($"{textDecorator.Highlight("Outcome")} : {DecorateStatusCode(statusCode)}");
            sb.AppendLine();
            sb.Append(table.ToString().TrimEnd());
        }

        private void AppendErrorBanner(StringBuilder sb,
                                       string title)
        {
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error(title));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════════════════════"));
        }

        private string DecorateStatusCode(string statusCode)
        {
            var numericPart = statusCode.Split(' ', 2)[0];

            if (int.TryParse(numericPart, out var statusCodeNumber))
            {
                if (statusCodeNumber is >= 200 and < 300)
                {
                    return textDecorator.Success(statusCode);
                }

                if (statusCodeNumber is >= 400 and < 500)
                {
                    return textDecorator.SectionTitle(statusCode);
                }

                if (statusCodeNumber >= 500)
                {
                    return textDecorator.Error(statusCode);
                }
            }

            return textDecorator.Highlight(statusCode);
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

    }
}
