using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Pack;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddEndpointValidatorExtension
    {
        public static void AddEndpointValidator(this IServiceCollection services)
        {
            // EndpointDataSource is registered by the host application
            services.AddSingletonIfNotExists<IEndpointValidator, EndpointValidator>();
        }
    }

    public interface IEndpointValidator
    {
        /// <summary>
        /// Validates the HTTP assert context against the registered endpoints.
        /// Checks if the endpoint exists and if the expected response type matches.
        /// </summary>
        /// <typeparam name="TResult">The expected result type</typeparam>
        /// <param name="context">The HTTP assert context containing method, URL, and expected type</param>
        /// <returns>Validation result with detailed information</returns>
        EndpointValidationResult Validate<TResult>(IHttpAssertContext context);
    }

    internal sealed class EndpointValidator(EndpointDataSource endpointDataSource) : IEndpointValidator
    {
        public EndpointValidationResult Validate<TResult>(IHttpAssertContext context)
        {
            var httpMethod = context.HttpMethod.Method;
            var url = context.Url;
            var expectedType = typeof(TResult);

            // 1. Get all endpoints
            var endpoints = endpointDataSource.Endpoints;

            if (endpoints.IsEmpty())
            {
                return EndpointValidationResult.NoEndpointsFound();
            }

            // 2. Try to match endpoint by HTTP method, route pattern, and API version
            var routeEndpoints = endpoints.OfType<RouteEndpoint>().ToList();
            var requestedVersion = context.ApiVersion;

            // Simple matching by URL (can be improved with route parameter matching)
            var matchingEndpoints = routeEndpoints.Where(e =>
            {
                // Get HTTP method metadata
                var httpMethodMetadata = e.Metadata.GetMetadata<HttpMethodMetadata>();

                if (httpMethodMetadata.IsNull())
                {
                    return false;
                }

                // Check if HTTP method matches
                var methodMatches = httpMethodMetadata.HttpMethods.Contains(httpMethod, StringComparer.OrdinalIgnoreCase);

                if (methodMatches.IsFalse())
                {
                    return false;
                }

                // Check API version if specified
                if (requestedVersion.IsNotNullOrWhiteSpace())
                {
                    // Try to extract version from endpoint metadata or route pattern
                    // Support for Asp.Versioning.Http (Microsoft.AspNetCore.Mvc.Versioning)
                    var versionMetadata = e.Metadata.FirstOrDefault(m => m.GetType().Name.Contains("ApiVersion"));

                    if (versionMetadata.IsNotNull())
                    {
                        // Try to get version string from metadata (simplified for now)
                        var versionString = versionMetadata.ToString();

                        if (versionString.IsNotNullOrWhiteSpace() && versionString.Contains(requestedVersion, StringComparison.OrdinalIgnoreCase).IsFalse())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Check if version is in route pattern (e.g., "v1/users", "api/v2/products")
                        var routePattern = e.RoutePattern.RawText ?? string.Empty;

                        if (routePattern.Contains($"v{requestedVersion}", StringComparison.OrdinalIgnoreCase).IsFalse() &&
                            routePattern.Contains($"/{requestedVersion}/", StringComparison.OrdinalIgnoreCase).IsFalse())
                        {
                            // Version was requested but not found in route
                            return false;
                        }
                    }
                }

                // Check if route pattern matches (simple starts-with for now)
                var routePattern2 = e.RoutePattern.RawText ?? string.Empty;

                // Remove leading slash for comparison
                var normalizedUrl = url.TrimStart('/');
                var normalizedRoute = routePattern2.TrimStart('/');

                return normalizedUrl.StartsWith(normalizedRoute, StringComparison.OrdinalIgnoreCase);
            }).ToList();

            if (matchingEndpoints.IsEmpty())
            {
                return EndpointValidationResult.EndpointNotFound(httpMethod, url, requestedVersion, routeEndpoints);
            }

            // 3. Validate response type (if we have exactly one match)
            if (matchingEndpoints.Count == 1)
            {
                var endpoint = matchingEndpoints[0];

                // TODO: Extract expected response type from endpoint metadata
                // This requires ProducesResponseTypeAttribute or similar metadata
                // For now, return success with endpoint info
                return EndpointValidationResult.Success(endpoint, expectedType, requestedVersion);
            }

            // Multiple matches - ambiguous
            return EndpointValidationResult.MultipleMatches(httpMethod, url, requestedVersion, matchingEndpoints);
        }
    }

    /// <summary>
    /// Result of endpoint validation containing match information and validation status.
    /// </summary>
    public sealed class EndpointValidationResult
    {
        public bool IsValid { get; init; }
        public EndpointValidationError? Error { get; init; }
        public RouteEndpoint? MatchedEndpoint { get; init; }
        public Type? ExpectedResponseType { get; init; }
        public string? ApiVersion { get; init; }

        public static EndpointValidationResult Success(RouteEndpoint endpoint, Type expectedType, string? apiVersion)
        {
            return new EndpointValidationResult
                   {
                       IsValid = true,
                       MatchedEndpoint = endpoint,
                       ExpectedResponseType = expectedType,
                       ApiVersion = apiVersion
                   };
        }

        public static EndpointValidationResult NoEndpointsFound()
        {
            return new EndpointValidationResult
                   {
                       IsValid = false,
                       Error = new EndpointValidationError
                               {
                                   ErrorType = EndpointValidationErrorType.NoEndpointsRegistered,
                                   Message = "No endpoints are registered in the EndpointDataSource. Make sure the application is properly configured."
                               }
                   };
        }

        public static EndpointValidationResult EndpointNotFound(string httpMethod, string url, string? apiVersion, IEnumerable<RouteEndpoint> availableEndpoints)
        {
            var availableRoutes = string.Join("\n", availableEndpoints.Select(e =>
            {
                var methods = e.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? Array.Empty<string>();
                return $"  {string.Join(", ", methods)} {e.RoutePattern.RawText}";
            }));

            var versionInfo = apiVersion.IsNotNullOrWhiteSpace() ? $" (API Version: {apiVersion})" : string.Empty;

            return new EndpointValidationResult
                   {
                       IsValid = false,
                       Error = new EndpointValidationError
                               {
                                   ErrorType = EndpointValidationErrorType.EndpointNotFound,
                                   Message = $"No endpoint found for: {httpMethod} {url}{versionInfo}\n\nAvailable endpoints:\n{availableRoutes}"
                               }
                   };
        }

        public static EndpointValidationResult MultipleMatches(string httpMethod, string url, string? apiVersion, IEnumerable<RouteEndpoint> matchingEndpoints)
        {
            var matches = string.Join("\n", matchingEndpoints.Select(e => $"  {e.RoutePattern.RawText}"));
            var versionInfo = apiVersion.IsNotNullOrWhiteSpace() ? $" (API Version: {apiVersion})" : string.Empty;

            return new EndpointValidationResult
                   {
                       IsValid = false,
                       Error = new EndpointValidationError
                               {
                                   ErrorType = EndpointValidationErrorType.MultipleMatches,
                                   Message = $"Multiple endpoints matched for: {httpMethod} {url}{versionInfo}\n\nMatching endpoints:\n{matches}"
                               }
                   };
        }
    }

    public sealed class EndpointValidationError
    {
        public EndpointValidationErrorType ErrorType { get; init; }
        public string Message { get; init; } = string.Empty;
    }

    public enum EndpointValidationErrorType
    {
        NoEndpointsRegistered,
        EndpointNotFound,
        MultipleMatches,
        ResponseTypeMismatch
    }
}
