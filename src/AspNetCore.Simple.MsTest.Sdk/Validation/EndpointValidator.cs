using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddEndpointValidatorExtension
    {
        public static void AddEndpointValidator(this IServiceCollection services)
        {
            // EndpointDataSource is registered by the host application
            services.AddEndpointProvider();
            services.AddEndpointValidationOutputBuilder();
            services.AddSingletonIfNotExists<IEndpointValidator, EndpointValidator>();
        }
    }

    public interface IEndpointValidator
    {
        /// <summary>
        /// Validates the HTTP assert context against the registered endpoints.
        /// Checks if the endpoint exists and if the expected response type matches.
        /// Fails the test with Assert.That.Fail() if validation fails.
        /// </summary>
        /// <typeparam name="TResult">The expected result type</typeparam>
        /// <param name="context">The HTTP assert context containing method, URL, and expected type</param>
        void Validate<TResult>(IHttpAssertContext context);
    }

    internal sealed class EndpointValidator(IEndpointProvider endpointProvider,
                                            IEndpointValidationOutputBuilder outputBuilder) : IEndpointValidator
    {
        public void Validate<TResult>(IHttpAssertContext context)
        {
            // Scope or global skip endpoint validation
            if (context.SkipEndpointValidation ||
                HttpClientAssertExtensions.SkipEndpointValidation)
            {
                HttpClientAssertExtensions
                    .LogAction($"Endpoint validation skipped for this test. Context.SkipEndpointValidation: {context.SkipEndpointValidation}, HttpClientAssertExtensions.SkipEndpointValidation: {HttpClientAssertExtensions.SkipEndpointValidation}");

                return;
            }

            var httpMethod = context.HttpMethod.Method;
            var url = context.Url;
            var expectedResponse = context.ExpectedType;
            var requestedVersion = context.ApiVersion;

            // 2. Check if any endpoints are registered
            var allEndpoints = endpointProvider.GetAllEndpoints();

            if (allEndpoints.IsEmpty())
            {
                var error = outputBuilder.BuildEndpointNotFound(context, allEndpoints);
                Assert.That.Fail(error);

                return;
            }

            // 2. Find matching endpoint
            var endpoint = endpointProvider.FindEndpointFor(httpMethod, url, requestedVersion);

            if (endpoint.IsNull())
            {
                // Check if we have multiple matches (ambiguous) or no matches
                var allMatches = endpointProvider.FindAllMatchingEndpoints(httpMethod, url, requestedVersion);

                if (allMatches.IsEmpty())
                {
                    var error = outputBuilder.BuildEndpointNotFound(context, allEndpoints);
                    Assert.That.Fail(error);

                    return;
                }

                // Multiple matches - ambiguous
                var ambiguousError = outputBuilder.BuildMultipleMatches(context, allMatches);
                Assert.That.Fail(ambiguousError);

                return;
            }

            // 3. Validate response type
            ValidateResponseType<TResult>(context, endpoint, expectedResponse);

            // Validation passed - endpoint exists and type matches
        }

        private void ValidateResponseType<TResult>(IHttpAssertContext context,
                                                   EndpointInfo endpoint,
                                                   Type expectedResponse)
        {
            // Determine which status codes to check based on test type
            var isSuccessTest = context.IsSuccessStatusCode;

            // Priority: Use explicit ExpectedHttpStatusCode parameter first, then try to extract from JSON
            var expectedHttpStatusCode = TryExtractStatusCodeFromContext(context);

            if (expectedHttpStatusCode.HasValue)
            {
                ValidateTestTypeMatchesStatusCode(context, expectedHttpStatusCode.Value, isSuccessTest);
            }

            var statusCodesToCheck = GetStatusCodesToCheck(endpoint.ResponseTypesByStatusCode,
                                                           isSuccessTest,
                                                           expectedHttpStatusCode);

            // If we have explicit status code mappings, validate against them
            if (statusCodesToCheck.Any())
            {
                var matchingTypes = statusCodesToCheck.Values.Distinct().ToList();

                // Special case: 204 NoContent with Void type is compatible with String
                // This happens when non-generic AssertDeleteAsync() forwards to generic version with <string>
                var has204NoContent = statusCodesToCheck.ContainsKey(204);
                var endpointReturnsVoid = matchingTypes.Any(t => t == typeof(void) || t.Name == "Void");
                var testExpectsString = expectedResponse == typeof(string);

                if (has204NoContent && endpointReturnsVoid && testExpectsString)
                {
                    // Allow this combination - it's the non-generic overload pattern
                    return;
                }

                // Check if expected response type matches any of the relevant response types
                var isMatch = matchingTypes.Any(type => type == expectedResponse);

                if (isMatch.IsFalse())
                {
                    var typeMismatchError = outputBuilder.BuildResponseTypeMismatch(context,
                                                                                    endpoint,
                                                                                    expectedResponse,
                                                                                    statusCodesToCheck);

                    Assert.That.Fail(typeMismatchError);
                }

                // Type matches one of the valid response types for this test type
                return;
            }

            // Fallback 1: Try to extract status code from Expected Response JSON
            if (expectedHttpStatusCode.HasValue)
            {
                // Success: Expected status code aligns with test type
                return;
            }

            // Fallback 2: Use old ResponseType property (backward compatibility)
            if (endpoint.ResponseType.IsNotNull())
            {
                // An action returning bare Task/ValueTask/void has no response body. Reaching this
                // fallback means it also declares no ProducesResponseType, so asp.net answers
                // 200 OK with an EMPTY body instead of 204 No Content. Point that out explicitly -
                // the generic type table would otherwise suggest an impossible '<Task>'.
                if (EndpointParsingHelpers.ReturnsNoResponseBody(endpoint.ResponseType))
                {
                    var noBodyError = outputBuilder.BuildNoResponseBodyMismatch(context, endpoint, expectedResponse);

                    Assert.That.Fail(noBodyError);
                }

                // Check if TResult matches the declared response type
                if (endpoint.ResponseType != expectedResponse)
                {
                    var typeMismatchError = outputBuilder.BuildResponseTypeMismatch(context, endpoint, expectedResponse);
                    Assert.That.Fail(typeMismatchError);
                }
            }
        }

        /// <summary>
        /// Extracts the expected status code from the context.
        /// Priority:
        /// 1. ExpectedHttpStatusCode parameter (if explicitly set, i.e., not null)
        /// 2. Status code from expected JSON file
        /// This allows explicit parameter to override JSON file, maintaining backward compatibility.
        /// </summary>
        private static int? TryExtractStatusCodeFromContext(IHttpAssertContext context)
        {
            // Priority 1: Check if ExpectedHttpStatusCode was explicitly set (not null)
            if (context.ExpectedHttpStatusCode.HasValue)
            {
                return (int)context.ExpectedHttpStatusCode.Value;
            }

            // Priority 2: Try to extract from expected JSON file (backward compatibility)
            return TryExtractStatusCodeFromExpectedResponse(context);
        }

        /// <summary>
        /// Tries to extract the HTTP status code from the expected response JSON.
        /// Expected response format (SimpleHttpResponseMessage):
        /// {
        ///   "StatusCode": "InternalServerError",  // or 500
        ///   "IsSuccessStatusCode": false,
        ///   "Content": { "Value": { ... } }
        /// }
        /// </summary>
        private static int? TryExtractStatusCodeFromExpectedResponse(IHttpAssertContext context)
        {
            try
            {
                var expectedJson = context.ExpectedResultFile.Content;

                if (expectedJson.IsNullOrWhiteSpace())
                {
                    return null;
                }

                var json = JObject.Parse(expectedJson);

                // Try to get statusCode property (support PascalCase and camelCase)
                var statusCodeToken = json.GetValue("StatusCode", StringComparison.OrdinalIgnoreCase);

                if (statusCodeToken.IsNull())
                {
                    return null;
                }

                // StatusCode can be either a number (200) or a string ("OK", "InternalServerError")
                if (statusCodeToken.Type == JTokenType.Integer)
                {
                    return statusCodeToken.Value<int>();
                }

                if (statusCodeToken.Type == JTokenType.String)
                {
                    var statusCodeString = statusCodeToken.Value<string>();

                    // Try to parse as HttpStatusCode enum
                    if (Enum.TryParse<HttpStatusCode>(statusCodeString, ignoreCase: true, out var statusCode))
                    {
                        return (int)statusCode;
                    }
                }

                return null;
            }
            catch (Exception ex) when (ex is Newtonsoft.Json.JsonException or ArgumentException or InvalidOperationException)
            {
                // Parsing failed - not a SimpleHttpResponseMessage format
                return null;
            }
        }

        /// <summary>
        /// Validates that the test type (success vs error) matches the expected status code.
        /// </summary>
        private void ValidateTestTypeMatchesStatusCode(IHttpAssertContext context,
                                                       int expectedHttpStatusCode,
                                                       bool isSuccessTest)
        {
            var isSuccessStatusCode = expectedHttpStatusCode is >= 200 and < 300;

            // Test type should match the expected status code range
            if (isSuccessTest && !isSuccessStatusCode)
            {
                // Success test but expected status code is error (4xx/5xx)
                var error = outputBuilder.BuildTestTypeMismatch(context,
                                                                expectedHttpStatusCode,
                                                                isSuccessTest: true);

                Assert.That.Fail(error);
            }
            else if (!isSuccessTest && isSuccessStatusCode)
            {
                // Error test but expected status code is success (2xx)
                var error = outputBuilder.BuildTestTypeMismatch(context,
                                                                expectedHttpStatusCode,
                                                                isSuccessTest: false);

                Assert.That.Fail(error);
            }
        }

        /// <summary>
        /// Filters response types by status code range based on test type.
        /// </summary>
        /// <param name="responseTypes">All response types by status code</param>
        /// <param name="isSuccessTest">True for success tests (2xx), false for error tests (4xx/5xx)</param>
        /// <returns>Filtered response types</returns>
        private static ImmutableDictionary<int, Type> GetRelevantStatusCodes(ImmutableDictionary<int, Type> responseTypes,
                                                                             bool isSuccessTest)
        {
            if (!responseTypes.Any())
            {
                return responseTypes;
            }

            if (isSuccessTest)
            {
                // Success test: only check 2xx status codes
                return responseTypes
                       .Where(kvp => kvp.Key is >= 200 and < 300)
                       .ToImmutableDictionary();
            }

            // Error test: only check 4xx and 5xx status codes
            return responseTypes
                   .Where(kvp => kvp.Key is >= 400 and < 600)
                   .ToImmutableDictionary();
        }

        private static ImmutableDictionary<int, Type> GetStatusCodesToCheck(ImmutableDictionary<int, Type> responseTypes,
                                                                            bool isSuccessTest,
                                                                            int? expectedHttpStatusCode)
        {
            if (expectedHttpStatusCode.HasValue && responseTypes.TryGetValue(expectedHttpStatusCode.Value, out var responseType))
            {
                return ImmutableDictionary<int, Type>.Empty.Add(expectedHttpStatusCode.Value, responseType);
            }

            return GetRelevantStatusCodes(responseTypes, isSuccessTest);
        }
    }
}