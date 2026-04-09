using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
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
            if (endpoint.ResponseType.IsNotNull())
            {
                // Check if TResult matches the declared response type
                if (endpoint.ResponseType != expectedResponse)
                {
                    var typeMismatchError = outputBuilder.BuildResponseTypeMismatch(context, endpoint, expectedResponse);
                    Assert.That.Fail(typeMismatchError);

                    return;
                }
            }

            // Validation passed - endpoint exists and type matches
        }
    }
}
