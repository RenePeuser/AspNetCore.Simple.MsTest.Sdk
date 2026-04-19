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

    /// <summary>
    /// Pre-processed endpoint data for fast lookup.
    /// </summary>
    internal sealed record PreProcessedEndpoint(EndpointInfo Endpoint,
                                                string NormalizedUrl,
                                                string[] UrlSegments,
                                                int? VersionSegmentIndex,
                                                string? VersionString);

    internal sealed class EndpointProvider(IEndpointInfoParser endpointInfoParser) : IEndpointProvider
    {
        private readonly Lazy<ImmutableList<PreProcessedEndpoint>> _cachedEndpoints = new(() =>
                                                                                          {
                                                                                              var allEndpoints = endpointInfoParser.GetEndpoints();

                                                                                              return PreProcessEndpoints(allEndpoints);
                                                                                          }, LazyThreadSafetyMode.ExecutionAndPublication);

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
            var processedEndpoints = _cachedEndpoints.Value;

            if (processedEndpoints.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var matchingEndpoints = FindMatches(processedEndpoints, httpMethod, url,
                                                apiVersion);

            return matchingEndpoints;
        }

        public ImmutableList<EndpointInfo> GetAllEndpoints()
        {
            return endpointInfoParser.GetEndpoints();
        }

        private static ImmutableList<PreProcessedEndpoint> PreProcessEndpoints(ImmutableList<EndpointInfo> endpoints)
        {
            var preProcessedEndpoints = endpoints.Select(endpoint =>
                                                         {
                                                             var normalizedUrl = endpoint.Url.TrimStart('/');
                                                             var segments = normalizedUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                                                             var versionIndex = FindVersionSegmentIndexInSegments(segments);
                                                             var versionString = endpoint.ApiVersion?.MajorVersion.ToString();

                                                             return new PreProcessedEndpoint(endpoint,
                                                                                             normalizedUrl,
                                                                                             segments,
                                                                                             versionIndex,
                                                                                             versionString);
                                                         })
                                                 .ToImmutableList();

            return preProcessedEndpoints;
        }

        private static ImmutableList<EndpointInfo> FindMatches(ImmutableList<PreProcessedEndpoint> processedEndpoints,
                                                               string httpMethod,
                                                               string url,
                                                               string? requestedVersion)
        {
            // Extract path from URL (remove scheme, host, query parameters)
            var urlPath = ExtractPathFromUrl(url);

            // Normalize: remove query parameters and leading slash
            var urlWithoutQuery = urlPath.Split('?', 2)[0];
            var normalizedUrl = urlWithoutQuery.TrimStart('/');
            var requestSegments = normalizedUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var requestVersionIndex = FindVersionSegmentIndexInSegments(requestSegments);

            // Filter candidates by HTTP method and version first
            var candidateEndpoints = processedEndpoints.Where(ep =>
                ep.Endpoint.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase) &&
                (requestedVersion.IsNullOrWhiteSpace() || ep.VersionString == requestedVersion)
            ).ToImmutableList();

            // Now match URL patterns only for filtered candidates
            var matches = candidateEndpoints.Where(processed =>
                                                   {
                                                       // Check URL pattern match using pre-processed data
                                                       var urlMatches = UrlMatchesOptimized(normalizedUrl,
                                                                                            requestSegments,
                                                                                            requestVersionIndex,
                                                                                            processed);

                                                       return urlMatches;
                                                   })
                                            .Select(p => p.Endpoint)
                                            .ToImmutableList();

            return matches;
        }

        private static bool UrlMatchesOptimized(string requestUrl,
                                                string[] requestSegments,
                                                int? requestVersionIndex,
                                                PreProcessedEndpoint processed)
        {
            // Fast path: exact match
            if (requestUrl.EqualsTo(processed.NormalizedUrl))
            {
                return true;
            }

            // Try suffix matching first - this handles base path scenarios
            // where request has prefix like /api/tests but endpoint doesn't
            // Example: Request: api/tests/v1/persons, Endpoint: v1/persons
            if (requestSegments.Length > processed.UrlSegments.Length)
            {
                var offset = requestSegments.Length - processed.UrlSegments.Length;
                var suffixSegments = requestSegments.Skip(offset).ToArray();

                if (SegmentMatchesOptimized(suffixSegments, processed.UrlSegments))
                {
                    return true;
                }
            }

            // If request has version segment, match from version onwards
            if (requestVersionIndex.HasValue)
            {
                return SegmentMatchesOptimized(requestSegments.Skip(requestVersionIndex.Value).ToArray(),
                                               processed.UrlSegments.Skip(processed.VersionSegmentIndex ?? 0).ToArray());
            }

            // No version in URL - exact segment matching
            return SegmentMatchesOptimized(requestSegments, processed.UrlSegments);
        }

        private static bool SegmentMatchesOptimized(string[] requestSegments,
                                                    string[] patternSegments)
        {
            // Must have same number of segments
            if (requestSegments.Length != patternSegments.Length)
            {
                return false;
            }

            // Compare each segment
            for (var i = 0; i < requestSegments.Length; i++)
            {
                var patternSegment = patternSegments[i];
                var requestSegment = requestSegments[i];

                // If pattern segment is a route parameter like {id} or {id:guid}, check constraints
                if (patternSegment.StartsWith('{') && patternSegment.EndsWith('}'))
                {
                    // Extract parameter name and constraint (e.g., "id:guid" -> "id", "guid")
                    var parameterDefinition = patternSegment.Trim('{', '}');
                    var parts = parameterDefinition.Split(':', 2);

                    // If there's a constraint, validate it
                    if (parts.Length == 2)
                    {
                        var constraint = parts[1].ToLowerInvariant();

                        // Check common constraints
                        if (constraint == "guid")
                        {
                            // Must be a valid GUID
                            if (Guid.TryParse(requestSegment, out _).IsFalse())
                            {
                                return false;
                            }
                        }
                        else if (constraint == "int" || constraint == "long")
                        {
                            // Must be a valid integer
                            if (long.TryParse(requestSegment, out _).IsFalse())
                            {
                                return false;
                            }
                        }
                        else if (constraint == "bool")
                        {
                            // Must be a valid boolean
                            if (bool.TryParse(requestSegment, out _).IsFalse())
                            {
                                return false;
                            }
                        }
                        // For other constraints, we accept any value (conservative approach)
                    }

                    // Parameter matches (either no constraint, or constraint validated)
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

        private static int? FindVersionSegmentIndexInSegments(string[] segments)
        {
            // Find v1, v2, v10, etc. in segments
            // Returns the segment index, or null if not found
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                // Check if segment starts with 'v' followed by digits
                if (segment.Length >= 2 &&
                    (segment[0] == 'v' || segment[0] == 'V') &&
                    char.IsDigit(segment[1]))
                {
                    return i;
                }
            }

            return null;
        }

        private static string ExtractPathFromUrl(string url)
        {
            // If URL contains scheme (http:// or https://), parse it as URI
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    // Return path + query (e.g., "/api/console/v1/capability-types?information=...")
                    return uri.PathAndQuery;
                }
            }

            // Otherwise, assume it's already a path
            return url;
        }
    }
}
