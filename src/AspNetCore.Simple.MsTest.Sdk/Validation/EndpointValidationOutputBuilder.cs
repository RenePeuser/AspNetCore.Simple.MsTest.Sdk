using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddEndpointValidationOutputBuilderExtension
    {
        public static void AddEndpointValidationOutputBuilder(this IServiceCollection services)
        {
            services.AddTableBuilder();
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

    internal sealed class EndpointValidationOutputBuilder(ITableBuilder tableBuilder,
                                                          ICurlBuilder curlBuilder,
                                                          ICurlFormatter curlFormatter,
                                                          ISourceCodeExtractor sourceCodeExtractor,
                                                          ITextDecorator textDecorator) : IEndpointValidationOutputBuilder
    {
        public string BuildEndpointNotFound(IHttpAssertContext context,
                                            ImmutableList<EndpointInfo> availableEndpoints)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ ENDPOINT NOT FOUND"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "404 NotFound");
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
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ AMBIGUOUS ENDPOINT MATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "Ambiguous");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🔍 Matching Endpoints (Count " + matchingEndpoints.Count + ")"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var columns = new[] { "Method", "URL", "API Version", "Response Type" };
            var rows = new List<object[]>();

            foreach (var endpoint in matchingEndpoints)
            {
                var version = endpoint.ApiVersion?.ToString() ?? "N/A";
                var responseType = endpoint.ResponseType?.Name ?? "N/A";

                rows.Add(new object[] { endpoint.HttpMethod, endpoint.Url, version, responseType });
            }

            var table = tableBuilder.BuildTable(columns, rows, enableCount: false);
            sb.AppendLine(table);
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
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ HTTP RESPONSE TYPE MISMATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table - validation happens before HTTP call, so status is pending
            BuildHttpCallTable(sb, context, "Type Mismatch");
            sb.AppendLine();

            // ASSERT CALL - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
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

                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success(suggestedFix));
                sb.AppendLine();
            }

            // TYPE VALIDATION Table
            var typeColumns = new[] { "Source", "Actual Endpoint Type", "Declared Test Type" };
            var typeRows = new List<object[]>
                           {
                               new object[] { "ResponseType", actualEndpointTypeName, declaredTestTypeName }
                           };

            var typeTable = tableBuilder.BuildTable(typeColumns, typeRows, enableCount: false);

            sb.AppendLine(textDecorator.SectionTitle("🔍 Type Validation"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.Append(typeTable.TrimEnd());
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"The test declares response type '{textDecorator.Error(declaredTestTypeName)}', but the endpoint exposes");
            sb.AppendLine($"'{textDecorator.Success(actualEndpointTypeName)}' for HTTP 200 OK.");
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
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ HTTP RESPONSE TYPE MISMATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // HTTP Call Table - validation happens before HTTP call, so status is pending
            BuildHttpCallTable(sb, context, "Type Mismatch");
            sb.AppendLine();

            // ASSERT CALL - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
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

                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success(suggestedFix));
                sb.AppendLine();
            }

            // TYPE VALIDATION Table - Show all relevant status codes
            sb.AppendLine(textDecorator.SectionTitle("🔍 Type Validation"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var typeColumns = new[] { "Status Code", "Endpoint Response Type", "Declared Test Type", "Match" };
            var typeRows = new List<object[]>();

            foreach (var statusCode in relevantStatusCodes.OrderBy(kvp => kvp.Key))
            {
                var actualTypeName = FormatTypeName(statusCode.Value);
                var isMatch = statusCode.Value == expectedType ? "✓" : "✗";

                typeRows.Add(new object[]
                             {
                                 statusCode.Key.ToString(),
                                 actualTypeName,
                                 declaredTestTypeName,
                                 isMatch
                             });
            }

            var typeTable = tableBuilder.BuildTable(typeColumns, typeRows, enableCount: false);
            sb.Append(typeTable.TrimEnd());
            sb.AppendLine();
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
            var className = ExtractClassName(context);
            var methodName = context.CallerMemberName;

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {className}");
            sb.AppendLine($"{"Method",-10} : {methodName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private static string ExtractClassName(IHttpAssertContext context)
        {
            var callerFilePath = context.CallerFilePath;
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
            return fileName.Replace(".cs", string.Empty);
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
            // Build full URL from client base address
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            sb.AppendLine(textDecorator.SectionTitle("🌍 HTTP"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {context.HttpMethod.Method}");
            sb.AppendLine($"{"Url",-10} : {fullUrl}");
            sb.AppendLine($"{"Status",-10} : {DecorateStatusCode(statusCode)}");
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
