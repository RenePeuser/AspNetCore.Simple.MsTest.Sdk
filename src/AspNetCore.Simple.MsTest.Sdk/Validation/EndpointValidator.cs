using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

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
            var httpMethod = context.HttpMethod.Method;
            var url = context.Url;
            var expectedResponse = typeof(TResult);
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
            var statusCodesToCheck = GetRelevantStatusCodes(endpoint.ResponseTypesByStatusCode, isSuccessTest);

            // If we have explicit status code mappings, validate against them
            if (statusCodesToCheck.Any())
            {
                var matchingTypes = statusCodesToCheck.Values.Distinct().ToList();

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

            // Fallback: Use old ResponseType property (backward compatibility)
            if (endpoint.ResponseType.IsNotNull())
            {
                // Check if TResult matches the declared response type
                if (endpoint.ResponseType != expectedResponse)
                {
                    var typeMismatchError = outputBuilder.BuildResponseTypeMismatch(context, endpoint, expectedResponse);
                    Assert.That.Fail(typeMismatchError);
                }
            }
        }

        /// <summary>
        /// Filters response types by status code range based on test type.
        /// </summary>
        /// <param name="responseTypes">All response types by status code</param>
        /// <param name="isSuccessTest">True for success tests (2xx), false for error tests (4xx/5xx)</param>
        /// <returns>Filtered response types</returns>
        private static ImmutableDictionary<int, Type> GetRelevantStatusCodes(
            ImmutableDictionary<int, Type> responseTypes,
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
    }
}
