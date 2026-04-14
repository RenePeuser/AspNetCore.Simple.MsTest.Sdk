using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddApiVersionResolverExtension
    {
        public static void AddApiVersionResolver(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IApiVersionResolver, ApiVersionResolver>();
        }
    }

    public interface IApiVersionResolver
    {
        /// <summary>
        /// Resolves the API version from the URL and HTTP client headers.
        /// Checks multiple sources in priority order:
        /// 1. URL path segment (e.g., /v1/, /v2/, /api/v3/)
        /// 2. Query string parameter (e.g., ?api-version=1.0)
        /// 3. HTTP client default headers (e.g., api-version: 1.0, x-api-version: 2)
        /// </summary>
        /// <param name="url">The request URL</param>
        /// <param name="httpClient">The HTTP client (to check default headers)</param>
        /// <returns>The resolved API version or null if no version was found</returns>
        string? Resolve(string url,
                        HttpClient httpClient);
    }

    internal sealed class ApiVersionResolver : IApiVersionResolver
    {
        // Common API version header names
        private static readonly string[] VersionHeaderNames = ["api-version", "x-api-version", "version"];

        // Common query string parameter names
        private static readonly string[] VersionQueryNames = ["api-version", "version", "v"];

        public string? Resolve(string url,
                               HttpClient httpClient)
        {
            // 1. Try to extract version from URL path segment (highest priority)
            var versionFromPath = ResolveFromUrlPath(url);

            if (versionFromPath.IsNotNullOrWhiteSpace())
            {
                return versionFromPath;
            }

            // 2. Try to extract version from query string
            var versionFromQuery = ResolveFromQueryString(url);

            if (versionFromQuery.IsNotNullOrWhiteSpace())
            {
                return versionFromQuery;
            }

            // 3. Try to extract version from HTTP client default headers (lowest priority)
            var versionFromHeaders = ResolveFromHeaders(httpClient);

            if (versionFromHeaders.IsNotNullOrWhiteSpace())
            {
                return versionFromHeaders;
            }

            // No version found
            return null;
        }

        private static string? ResolveFromUrlPath(string url)
        {
            if (url.IsNullOrWhiteSpace())
            {
                return null;
            }

            // Match version pattern in URL path using global regex
            var match = ApiVersionRegex.UrlVersionPattern().Match(url);

            if (match.Success && match.Groups.Count > 1)
            {
                // Return the captured version number (without 'v' prefix)
                return match.Groups[1].Value;
            }

            return null;
        }

        private static string? ResolveFromQueryString(string url)
        {
            if (url.IsNullOrWhiteSpace())
            {
                return null;
            }

            // Check if URL contains query string
            var queryStartIndex = url.IndexOf('?');

            if (queryStartIndex == -1)
            {
                return null;
            }

            var queryString = url.Substring(queryStartIndex + 1);

            // Parse query string parameters (simple approach)
            var parameters = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var parameter in parameters)
            {
                var keyValue = parameter.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

                if (keyValue.Length != 2)
                {
                    continue;
                }

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                // Check if this is a version parameter
                if (VersionQueryNames.Any(name => name.Equals(key, StringComparison.OrdinalIgnoreCase)))
                {
                    // Remove 'v' prefix if present
#pragma warning disable CA1867
                    return value.StartsWith("v", StringComparison.OrdinalIgnoreCase)
#pragma warning restore CA1867
                               ? value.Substring(1)
                               : value;
                }
            }

            return null;
        }

        private static string? ResolveFromHeaders(HttpClient httpClient)
        {
            if (httpClient.IsNull())
            {
                return null;
            }

            // Check default request headers
            foreach (var headerName in VersionHeaderNames)
            {
                if (httpClient.DefaultRequestHeaders.TryGetValues(headerName, out var values))
                {
                    var version = values.FirstOrDefault();

                    if (version.IsNotNullOrWhiteSpace())
                    {
                        // Remove 'v' prefix if present
#pragma warning disable CA1867
                        return version.StartsWith("v", StringComparison.OrdinalIgnoreCase)
#pragma warning restore CA1867
                                   ? version.Substring(1)
                                   : version;
                    }
                }
            }

            return null;
        }
    }
}
