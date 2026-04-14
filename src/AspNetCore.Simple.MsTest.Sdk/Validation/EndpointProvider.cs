using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddEndpointProviderExtension
    {
        public static void AddEndpointProvider(this IServiceCollection services)
        {
            services.AddEndpointInfoParser();
            services.AddSingletonIfNotExists<IEndpointProvider, EndpointProvider>();
        }
    }

    public interface IEndpointProvider
    {
        /// <summary>
        /// Finds the matching endpoint for the given HTTP request.
        /// </summary>
        /// <param name="httpMethod">HTTP method (GET, POST, etc.)</param>
        /// <param name="url">Request URL</param>
        /// <param name="apiVersion">Optional API version</param>
        /// <returns>Matching endpoint info, or null if not found or multiple matches exist</returns>
        EndpointInfo? FindEndpointFor(string httpMethod,
                                      string url,
                                      string? apiVersion);

        /// <summary>
        /// Finds all matching endpoints for the given HTTP request.
        /// Useful for debugging ambiguous matches.
        /// </summary>
        /// <param name="httpMethod">HTTP method (GET, POST, etc.)</param>
        /// <param name="url">Request URL</param>
        /// <param name="apiVersion">Optional API version</param>
        /// <returns>All matching endpoints</returns>
        ImmutableList<EndpointInfo> FindAllMatchingEndpoints(string httpMethod,
                                                             string url,
                                                             string? apiVersion);

        ImmutableList<EndpointInfo> GetAllEndpoints();
    }

    /// <summary>
    /// Empty endpoint provider for cases where no endpoints are available.
    /// Used as a fallback in static contexts.
    /// </summary>
    internal sealed class EmptyEndpointProvider : IEndpointProvider
    {
        public EndpointInfo? FindEndpointFor(string httpMethod,
                                             string url,
                                             string? apiVersion)
        {
            return null;
        }

        public ImmutableList<EndpointInfo> FindAllMatchingEndpoints(string httpMethod,
                                                                    string url,
                                                                    string? apiVersion)
        {
            return ImmutableList<EndpointInfo>.Empty;
        }

        public ImmutableList<EndpointInfo> GetAllEndpoints()
        {
            return ImmutableList<EndpointInfo>.Empty;
        }
    }

    internal sealed class EndpointProvider(IEndpointInfoParser endpointInfoParser) : IEndpointProvider
    {
        public EndpointInfo? FindEndpointFor(string httpMethod,
                                             string url,
                                             string? apiVersion)
        {
            var matches = FindAllMatchingEndpoints(httpMethod, url, apiVersion);

            // Return endpoint only if exactly one match
            return matches.Count == 1 ? matches[0] : null;
        }

        public ImmutableList<EndpointInfo> FindAllMatchingEndpoints(string httpMethod,
                                                                    string url,
                                                                    string? apiVersion)
        {
            var allEndpoints = endpointInfoParser.GetEndpoints();

            if (allEndpoints.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var matchingEndpoints = FindMatches(allEndpoints, httpMethod, url,
                                                apiVersion);

            return matchingEndpoints;
        }

        public ImmutableList<EndpointInfo> GetAllEndpoints()
        {
            return endpointInfoParser.GetEndpoints();
        }

        private static ImmutableList<EndpointInfo> FindMatches(ImmutableList<EndpointInfo> endpoints,
                                                               string httpMethod,
                                                               string url,
                                                               string? requestedVersion)
        {
            // Normalize URL for comparison
            var normalizedUrl = url.TrimStart('/');

            var matches = endpoints.Where(endpoint =>
                                          {
                                              // 1. Check HTTP method
                                              if (endpoint.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase).IsFalse())
                                              {
                                                  return false;
                                              }

                                              // 2. Check API version if specified
                                              if (requestedVersion.IsNotNullOrWhiteSpace())
                                              {
                                                  if (endpoint.ApiVersion.IsNull())
                                                  {
                                                      // Endpoint has no version, but version was requested
                                                      return false;
                                                  }

                                                  var endpointVersionString = endpoint.ApiVersion.MajorVersion.ToString();

                                                  if (endpointVersionString.NotEqualsTo(requestedVersion))
                                                  {
                                                      // Version mismatch
                                                      return false;
                                                  }
                                              }

                                              // 3. Check URL pattern match
                                              var normalizedEndpointUrl = endpoint.Url.TrimStart('/');

                                              return UrlMatches(normalizedUrl, normalizedEndpointUrl);
                                          })
                                   .ToImmutableList();

            return matches;
        }

        private static bool UrlMatches(string requestUrl,
                                       string endpointPattern)
        {
            // Multi-stage URL matching to handle different routing patterns:
            // 1. If version is in URL (e.g., /v1/, /v2/), split and match from version onwards
            // 2. Otherwise, do exact segment matching

            var normalizedRequest = requestUrl.TrimStart('/');
            var normalizedPattern = endpointPattern.TrimStart('/');

            if (normalizedRequest.EqualsTo(normalizedPattern))
            {
                return true;
            }

            // Stage 1: Check if there's a version segment like /v1/, /v2/, etc.
            var versionIndex = FindVersionSegmentIndex(normalizedRequest);

            if (versionIndex >= 0)
            {
                // Split at version - match from version onwards
                // Request:  api/tests/v1/persons → v1/persons
                // Pattern:  v1/persons            → v1/persons
                var requestFromVersion = normalizedRequest.Substring(versionIndex);

                var segmentMatches = SegmentMatches(requestFromVersion, normalizedPattern);

                return segmentMatches;
            }

            // Stage 2: No version in URL - exact segment matching
            return SegmentMatches(normalizedRequest, normalizedPattern);
        }

        private static int FindVersionSegmentIndex(string url)
        {
            // Find /v1/, /v2/, /v10/, etc. in URL
            // Returns the index of 'v' in the version segment, or -1 if not found
            var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var currentIndex = 0;

            foreach (var segment in segments)
            {
                // Check if segment starts with 'v' followed by digits
                if (segment.Length >= 2 &&
                    (segment[0] == 'v' || segment[0] == 'V') &&
                    char.IsDigit(segment[1]))
                {
                    return currentIndex;
                }

                currentIndex += segment.Length + 1; // +1 for the '/'
            }

            return -1;
        }

        private static bool SegmentMatches(string requestUrl,
                                           string endpointPattern)
        {
            // Split by '/' for segment-by-segment comparison
            var requestSegments = requestUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var patternSegments = endpointPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Must have same number of segments
            if (requestSegments.Length != patternSegments.Length)
            {
                return false;
            }

            // Compare each segment
            for (var i = 0; i < requestSegments.Length; i++)
            {
                var requestSegment = requestSegments[i];
                var patternSegment = patternSegments[i];

                // If pattern segment is a route parameter like {id}, it matches any value
                if (patternSegment.StartsWith('{') && patternSegment.EndsWith('}'))
                {
                    continue;
                }

                // Otherwise, must match exactly (case-insensitive)
                if (requestSegment.Equals(patternSegment, StringComparison.OrdinalIgnoreCase).IsFalse())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
