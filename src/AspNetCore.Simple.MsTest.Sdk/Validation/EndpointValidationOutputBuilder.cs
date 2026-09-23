using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
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
            services.AddTestContextHelper();
            services.AddSourceLocationHelper();
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
        /// Builds the error message for an action that cannot return a response body at all
        /// (void / Task / ValueTask) and does not declare 204 No Content.
        /// </summary>
        string BuildNoResponseBodyMismatch(IHttpAssertContext context,
                                           EndpointInfo endpoint,
                                           Type expectedType);

        /// <summary>
        /// Builds error message for response type mismatch with status code details.
        /// </summary>
        string BuildResponseTypeMismatch(IHttpAssertContext context,
                                         EndpointInfo endpoint,
                                         Type expectedType,
                                         ImmutableDictionary<int, Type> relevantStatusCodes);

        /// <summary>
        /// Builds error message for test type mismatch (success vs error).
        /// </summary>
        string BuildTestTypeMismatch(IHttpAssertContext context,
                                     int expectedHttpStatusCode,
                                     bool isSuccessTest);
    }

    internal sealed class EndpointValidationOutputBuilder(ITableBuilder tableBuilder,
                                                          ICurlBuilder curlBuilder,
                                                          ICurlFormatter curlFormatter,
                                                          ISourceCodeExtractor sourceCodeExtractor,
                                                          ITextDecorator textDecorator,
                                                          ITestContextHelper testContextHelper,
                                                          ISourceLocationHelper sourceLocationHelper) : IEndpointValidationOutputBuilder
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

            // Failure Details
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("The requested endpoint was not found.");
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "404 NotFound");
            sb.AppendLine();

            // Endpoint registration status
            BuildEndpointRegistrationInfo(sb, availableEndpoints.Count);
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

            // Failure Details
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"Multiple endpoints match the request. Found {matchingEndpoints.Count} matching endpoints.");
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "Ambiguous");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🔍 Matching Endpoints (Count " + matchingEndpoints.Count + ")"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var columns = new[]
                          {
                              "Method", "URL", "API Version",
                              "Response Type"
                          };

            var rows = new List<object[]>();

            foreach (var endpoint in matchingEndpoints)
            {
                var version = endpoint.ApiVersion?.ToString() ?? "N/A";
                var responseType = endpoint.ResponseType?.Name ?? "N/A";

                rows.Add(new object[]
                         {
                             endpoint.HttpMethod, endpoint.Url, version,
                             responseType
                         });
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

        /// <summary>
        /// An action returning void / Task / ValueTask produces no response body. Asp.net still answers
        /// 200 OK with an EMPTY body unless the action explicitly returns 204, which is almost never
        /// what a DELETE is supposed to do. The generic type table cannot help here - there is no type
        /// to put in it - so this renders dedicated guidance instead of suggesting an impossible
        /// '&lt;Task&gt;' type argument.
        /// </summary>
        public string BuildNoResponseBodyMismatch(IHttpAssertContext context,
                                                  EndpointInfo endpoint,
                                                  Type expectedType)
        {
            var sb = new StringBuilder();

            var returnTypeName = endpoint.ResponseType.IsNull()
                                     ? "void"
                                     : TypeNameFormatter.Format(endpoint.ResponseType);

            var httpMethod = context.HttpMethod.Method;
            var assertMethod = $"Assert{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(httpMethod.ToLowerInvariant())}Async";

            // SourceLocation is "Namespace.ControllerType.ActionName" - the snippet below shows the
            // CONTROLLER action, so it must not be labelled with the test method name.
            var actionName = endpoint.SourceLocation?.Split('.').LastOrDefault();

            if (actionName.IsNullOrWhiteSpace())
            {
                actionName = "YourAction";
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ ENDPOINT RETURNS NO RESPONSE BODY"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"The action returns '{textDecorator.Error(returnTypeName)}' - there is no response body,");
            sb.AppendLine("so no generic type argument can ever be correct for this call.");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("Note: No HTTP call was made. Validation failed before executing the request."));
            sb.AppendLine();

            BuildHttpCallTable(sb, context, "Endpoint Validation Failed",
                               endpoint);

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🚦 Status Code"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"The endpoint declares no {textDecorator.Success("[ProducesResponseType]")}, so asp.net answers");
            sb.AppendLine($"{textDecorator.Error("200 OK with an empty body")} - not {textDecorator.Success("204 No Content")}.");
            sb.AppendLine();
            sb.AppendLine($"An action without a response body should answer {textDecorator.Success("204 No Content")}.");
            sb.AppendLine();

            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
                sb.AppendLine();
            }

            sb.AppendLine(textDecorator.SectionTitle("✅ How To Fix"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("1. Endpoint (preferred) - declare and return 204:");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success($"     [Http{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(httpMethod.ToLowerInvariant())}]"));
            sb.AppendLine(textDecorator.Success("     [ProducesResponseType(StatusCodes.Status204NoContent)]"));
            sb.AppendLine(textDecorator.Success($"     public async Task<IActionResult> {actionName}(...)"));
            sb.AppendLine(textDecorator.Success("     {"));
            sb.AppendLine(textDecorator.Success("         await mediator.SendAsync(...);"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("         return NoContent();"));
            sb.AppendLine(textDecorator.Success("     }"));
            sb.AppendLine();

            // No response body means there is nothing to compare, so the expected result argument is
            // dropped entirely - neither a generic type argument nor an empty string belongs here.
            // Only the request body (POST/PUT/PATCH) stays, it is unrelated to the response.
            var hasRequestBody = string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(httpMethod, "PUT", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(httpMethod, "PATCH", StringComparison.OrdinalIgnoreCase);

            var callPrefix = $"     Client.{assertMethod}(";
            var argumentIndent = new string(' ', callPrefix.Length);

            sb.AppendLine("2. Test - drop the expected result, there is nothing to compare:");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success($"{callPrefix}url,"));

            if (hasRequestBody)
            {
                sb.AppendLine(textDecorator.Success($"{argumentIndent}payload,"));
            }

            sb.AppendLine(textDecorator.Success($"{argumentIndent}expectedHttpStatusCode: HttpStatusCode.NoContent);"));
            sb.AppendLine();

            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));

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

            // Failure reason
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("HTTP Response Type Mismatch");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("Note: No HTTP call was made. Validation failed before executing the request."));
            sb.AppendLine();

            // HTTP Call Table - validation happens before HTTP call
            BuildHttpCallTable(sb, context, "Endpoint Validation Failed",
                               endpoint);

            sb.AppendLine();

            // TYPE VALIDATION Table - Show problem first
            var peers = new[] { endpoint.ResponseType, expectedType }.OfType<Type>().ToList();

            var actualEndpointTypeName = endpoint.ResponseType.IsNotNull()
                                             ? TypeNameFormatter.Format(endpoint.ResponseType, peers)
                                             : "object";

            var declaredTestTypeName = TypeNameFormatter.Format(expectedType, peers);
            var endpointReturnsVoid = endpoint.ResponseType.IsNull();

            var typeColumns = new[] { "Source", "Actual Endpoint Type", "Declared Test Type" };

            var typeRows = new List<object[]> { new object[] { "ResponseType", actualEndpointTypeName, declaredTestTypeName } };

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

            // Failure reason
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("HTTP Response Type Mismatch");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("Note: No HTTP call was made. Validation failed before executing the request."));
            sb.AppendLine();

            // HTTP Call Table - validation happens before HTTP call
            BuildHttpCallTable(sb, context, "Endpoint Validation Failed",
                               endpoint);

            sb.AppendLine();

            // TYPE VALIDATION Table - Show all relevant status codes
            // Every type that appears anywhere in this section is a peer of every other one, so a
            // V1/V2 name clash is resolved consistently across table, narrative and suggested fix.
            var shownTypes = relevantStatusCodes.Values
                                                .Where(type => type.IsNotNull())
                                                .Append(expectedType)
                                                .Distinct()
                                                .ToList();

            var declaredTestTypeName = TypeNameFormatter.Format(expectedType, shownTypes);
            var isSuccessTest = context.IsSuccessStatusCode;

            // Find first matching type as suggestion
            var firstRelevantType = relevantStatusCodes.FirstOrDefault().Value;
            var suggestedTypeName = firstRelevantType.IsNotNull() ? TypeNameFormatter.Format(firstRelevantType, shownTypes) : "object";
            var endpointReturnsVoid = firstRelevantType.IsNull();

            sb.AppendLine(textDecorator.SectionTitle("🔍 Type Validation"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var typeColumns = new[]
                              {
                                  "Status Code", "Endpoint Response Type", "Declared Test Type",
                                  "Match"
                              };

            var typeRows = new List<object[]>();

            foreach (var statusCode in relevantStatusCodes.OrderBy(kvp => kvp.Key))
            {
                var actualTypeName = TypeNameFormatter.Format(statusCode.Value, shownTypes);
                var isMatch = statusCode.Value == expectedType ? "✓" : "✗";

                typeRows.Add(new object[]
                             {
                                 statusCode.Key.ToString(), actualTypeName, declaredTestTypeName,
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
                sb.AppendLine($"Endpoint defines: {singleStatus.Key} → {TypeNameFormatter.Format(singleStatus.Value, shownTypes)}");
            }
            else
            {
                sb.AppendLine("Endpoint defines:");

                foreach (var statusCode in relevantStatusCodes.OrderBy(kvp => kvp.Key))
                {
                    sb.AppendLine($"  - {statusCode.Key} → {TypeNameFormatter.Format(statusCode.Value, shownTypes)}");
                }
            }

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
            var className = testContextHelper.ExtractFullyQualifiedClassName(context.CallerFilePath, context.CallingAssembly);
            var methodName = context.CallerMemberName;

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            var fileUri = $"file:///{context.CallerFilePath.Replace('\\', '/')}:{context.CallerLineNumber}";
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {className}");
            sb.AppendLine($"{"Method",-10} : {methodName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine($"{"File",-10} : {fileUri}");
        }

        private void BuildEndpointRegistrationInfo(StringBuilder sb,
                                                   int endpointCount)
        {
            sb.AppendLine(textDecorator.SectionTitle("🔧 Endpoint Registration"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            if (endpointCount == 0)
            {
                sb.AppendLine($"{"Status",-15} : {textDecorator.Error("No endpoints registered")}");
                sb.AppendLine($"{"Count",-15} : {textDecorator.Error("0")}");
                sb.AppendLine();
                sb.AppendLine(textDecorator.Highlight("⚠️  MISSING REGISTRATION"));
                sb.AppendLine();
                sb.AppendLine("The AssertableHttpClient requires endpoint registration to validate HTTP calls.");
                sb.AppendLine("Please ensure the following registrations exist in your test setup:");
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success("  1. services.AddAssertableHttpClient(configuration);"));
                sb.AppendLine(textDecorator.Success("  2. HttpClientAssertExtensions.Setup(_apiTestBase.Services);"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Highlight("🌐 External API?"));
                sb.AppendLine();
                sb.AppendLine("If you're testing an external API that cannot be started via WebApplicationFactory,");
                sb.AppendLine("you can skip endpoint validation using:");
                sb.AppendLine();
                sb.AppendLine("  Per call:");
                sb.AppendLine(textDecorator.Success("    .AssertGetAsync<MyType>(url, skipEndpointValidation: true)"));
                sb.AppendLine();
                sb.AppendLine("  Globally:");
                sb.AppendLine(textDecorator.Success("    services.AddTestSdkSettings(configuration, settings => settings.SkipEndpointValidation = true);"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Error("    ⚠️  Use with caution! Skipping validation disables type-safety checks."));
            }
            else
            {
                sb.AppendLine($"{"Status",-15} : {textDecorator.Success("Endpoints registered")}");
                sb.AppendLine($"{"Count",-15} : {textDecorator.Success(endpointCount.ToString())}");
                sb.AppendLine();
                sb.AppendLine("The requested endpoint was not found among the registered endpoints.");
                sb.AppendLine("Please verify:");
                sb.AppendLine("  • URL pattern matches endpoint route");
                sb.AppendLine("  • HTTP method is correct");
                sb.AppendLine("  • API version (if used) is correct");
            }
        }

        private void BuildHttpCallTable(StringBuilder sb,
                                        IHttpAssertContext context,
                                        string statusCode,
                                        EndpointInfo? endpoint = null)
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

            // Add request body if available (from PayloadFile or PayloadAsJson)
            var requestBody = GetRequestBody(context);

            if (requestBody.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Request",-10} : {requestBody}");
            }

            if (endpoint?.SourceLocation.IsNotNullOrWhiteSpace() ?? false)
            {
                var clickableSource = sourceLocationHelper.ToClickableUri(endpoint.SourceLocation, context.CallingAssembly);
                sb.AppendLine($"{"Source",-10} : {clickableSource}");
            }
        }

        private static string GetRequestBody(IHttpAssertContext context)
        {
            // If we have a payload file, use the file name
            if (context.PayloadFile?.EmbeddedFile.IsNotNull() ?? false)
            {
                var fileName = context.PayloadFile.EmbeddedFileName;

                if (fileName.IsNotNullOrWhiteSpace())
                {
                    return fileName;
                }
            }

            // Otherwise, try to show the JSON content (truncated if too long)
            if (context.PayloadAsJson.IsNotNullOrWhiteSpace())
            {
                var json = context.PayloadAsJson;

                // Truncate if too long
                if (json.Length > 100)
                {
                    return string.Concat(json.AsSpan(0, 100), "...");
                }

                return json;
            }

            return string.Empty;
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

        public string BuildTestTypeMismatch(IHttpAssertContext context,
                                            int expectedHttpStatusCode,
                                            bool isSuccessTest)
        {
            var sb = new StringBuilder();
            var statusCodeName = Enum.GetName(typeof(System.Net.HttpStatusCode), expectedHttpStatusCode) ?? expectedHttpStatusCode.ToString();

            // Build dynamic header based on what the test expected
            var headerSuffix = isSuccessTest ? "SUCCESS EXPECTED" : "ERROR EXPECTED";

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error($"❌ ASSERT METHOD MISMATCH - {headerSuffix}"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            BuildTestInfo(sb, context);
            sb.AppendLine();

            // Failure reason
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"Assert Method Mismatch - {headerSuffix}");
            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("Note: No HTTP call was made. Validation failed before executing the request."));
            sb.AppendLine();

            // HTTP Call Table
            BuildHttpCallTable(sb, context, "Validation Failed - No HTTP call made");
            sb.AppendLine();

            // Problem explanation
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            if (isSuccessTest)
            {
                // Success test but expected status code is error (4xx/5xx)
                sb.AppendLine("The test is declared as a SUCCESS test (AssertPostAsync, AssertGetAsync, etc.)");
                sb.AppendLine($"but the expected response has status code {textDecorator.Error($"{expectedHttpStatusCode} ({statusCodeName})")} which is an ERROR status.");
            }
            else
            {
                // Error test but expected status code is success (2xx)
                sb.AppendLine("The test is declared as an ERROR test (AssertPostAsErrorAsync, AssertGetAsErrorAsync, etc.)");
                sb.AppendLine($"but the expected response has status code {textDecorator.Success($"{expectedHttpStatusCode} ({statusCodeName})")} which is a SUCCESS status.");
            }

            sb.AppendLine();

            // Details table
            sb.AppendLine(textDecorator.SectionTitle("📊 Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var testTypeText = isSuccessTest ? "Success (expects 2xx)" : "Error (expects 4xx/5xx)";
            var statusRange = expectedHttpStatusCode is >= 200 and < 300 ? "Success (2xx)" : "Error (4xx/5xx)";

            sb.AppendLine($"{"Test Type",-20} : {testTypeText}");
            sb.AppendLine($"{"Expected Status",-20} : {expectedHttpStatusCode} ({statusCodeName})");
            sb.AppendLine($"{"Status Range",-20} : {statusRange}");
            sb.AppendLine();

            // Assert call source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
                sb.AppendLine();
            }

            // Suggested fix with actual code
            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                var suggestedFix = sourceCode;

                if (isSuccessTest)
                {
                    // Success test but status code is error → Add "AsError"
                    // Example: AssertPostAsync → AssertPostAsErrorAsync
                    // Replace all Assert*Async patterns with Assert*AsErrorAsync
                    suggestedFix = suggestedFix.Replace("AssertGetAsync", "AssertGetAsErrorAsync")
                                               .Replace("AssertPostAsync", "AssertPostAsErrorAsync")
                                               .Replace("AssertPutAsync", "AssertPutAsErrorAsync")
                                               .Replace("AssertPatchAsync", "AssertPatchAsErrorAsync")
                                               .Replace("AssertDeleteAsync", "AssertDeleteAsErrorAsync");
                }
                else
                {
                    // Error test but status code is success → Remove "AsError"
                    // Example: AssertPostAsErrorAsync → AssertPostAsync
                    // Replace all Assert*AsErrorAsync patterns with Assert*Async
                    suggestedFix = suggestedFix.Replace("AssertGetAsErrorAsync", "AssertGetAsync")
                                               .Replace("AssertPostAsErrorAsync", "AssertPostAsync")
                                               .Replace("AssertPutAsErrorAsync", "AssertPutAsync")
                                               .Replace("AssertPatchAsErrorAsync", "AssertPatchAsync")
                                               .Replace("AssertDeleteAsErrorAsync", "AssertDeleteAsync");
                }

                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success(suggestedFix));
                sb.AppendLine();
            }
            else
            {
                // Fallback if no source code is available
                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();

                if (isSuccessTest)
                {
                    sb.AppendLine("Option 1: Use error assertion method instead");
                    sb.AppendLine(textDecorator.Success("  - Use AssertPostAsErrorAsync() or similar error assertion method"));
                    sb.AppendLine();
                    sb.AppendLine("Option 2: Update expected response status code");
                    sb.AppendLine(textDecorator.Success("  - Change the expected response to have a success status code (200, 201, etc.)"));
                }
                else
                {
                    sb.AppendLine("Option 1: Use success assertion method instead");
                    sb.AppendLine(textDecorator.Success("  - Use AssertPostAsync() or similar success assertion method"));
                    sb.AppendLine();
                    sb.AppendLine("Option 2: Update expected response status code");
                    sb.AppendLine(textDecorator.Success("  - Change the expected response to have an error status code (400, 404, 500, etc.)"));
                }

                sb.AppendLine();
            }

            // Curl command
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
            sb.AppendLine(curlFormatted);

            return sb.ToString();
        }
    }
}