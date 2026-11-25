using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    internal static class AddApiVersionResolverExtension
    {
        public static void AddApiVersionResolver(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IApiVersionResolver, ApiVersionResolver>();
        }
    }

    internal interface IApiVersionResolver
    {
        /// <summary>
        /// Resolves the API version a request asks for - from the same places the api reads it.
        /// Checks multiple sources in priority order:
        /// 1. URL path segment (e.g., /v1/, /v2/, /api/v3/)
        /// 2. Query string parameter (e.g., ?api-version=1.0)
        /// 3. Request headers of this call (e.g., WithHeader("x-api-version", "2")) - header names
        ///    first, then a media type parameter in Accept / Content-Type (e.g., application/json;v=2)
        /// 4. HTTP client default headers, same order
        /// <para>
        /// The query, header and media type names come from the api's own ApiVersionReader (or
        /// <see cref="TestSdkSettings.ApiVersionReader" />). Only when none can be detected the
        /// common names are used: query api-version / version / v, header api-version / x-api-version / version.
        /// </para>
        /// </summary>
        /// <param name="url">The request URL</param>
        /// <param name="httpClient">The HTTP client (to check default headers)</param>
        /// <param name="requestHeaders">The headers sent with this request only</param>
        /// <returns>The resolved API version or null if no version was found</returns>
        string? Resolve(string url,
                        HttpClient httpClient,
                        IReadOnlyDictionary<string, string>? requestHeaders = null);
    }

    internal sealed class ApiVersionResolver : IApiVersionResolver
    {
        // Common API version header names - only used when the api's reader is unknown
        private static readonly ImmutableList<string> CommonHeaderNames =
        [
            "api-version",
            "x-api-version",
            "version"
        ];

        // Common query string parameter names - only used when the api's reader is unknown
        private static readonly ImmutableList<string> CommonQueryNames =
        [
            "api-version",
            "version",
            "v"
        ];

        private static readonly string[] MediaTypeHeaderNames =
        [
            "Accept",
            "Content-Type"
        ];

        private readonly Lazy<VersionParameterNames> _names;

        public ApiVersionResolver(IServiceProvider serviceProvider,
                                  TestSdkSettings testSdkSettings)
        {
            // Lazy: the api's versioning services are complete only once the host is built
            _names = new Lazy<VersionParameterNames>(() => VersionParameterNames.From(ApiVersionParameterDetector.Detect(serviceProvider, testSdkSettings)));
        }

        public string? Resolve(string url,
                               HttpClient httpClient,
                               IReadOnlyDictionary<string, string>? requestHeaders = null)
        {
            var names = _names.Value;

            // 1. URL path segment (highest priority) - the route template carries it
            return ResolveFromUrlPath(url)
                   // 2. Query string
                   ?? ResolveFromQueryString(url, names.Query)
                   // 3. This request - it overrides the client defaults on the wire
                   ?? ResolveFromHeaders(ToLookup(requestHeaders), names)
                   // 4. Client default headers (lowest priority)
                   ?? ResolveFromHeaders(ToLookup(httpClient?.DefaultRequestHeaders), names);
        }

        private static string? ResolveFromUrlPath(string url)
        {
            if (url.IsNullOrWhiteSpace())
            {
                return null;
            }

            // Match version pattern in URL path using global regex
            var match = GlobalRegex.UrlVersionPattern().Match(url);

            if (match.Success && match.Groups.Count > 1)
            {
                // Return the captured version number (without 'v' prefix)
                return match.Groups[1].Value;
            }

            return null;
        }

        private static string? ResolveFromQueryString(string url,
                                                      ImmutableList<string> queryNames)
        {
            if (url.IsNullOrWhiteSpace() || queryNames.IsEmpty)
            {
                return null;
            }

            var queryStartIndex = url.IndexOf('?');

            if (queryStartIndex == -1)
            {
                return null;
            }

            var parameters = url.Substring(queryStartIndex + 1)
                                .Split('#', 2)[0]
                                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                                .Select(parameter => parameter.Split('=', 2))
                                .Where(keyValue => keyValue.Length == 2)
                                .Select(keyValue => (Key: Uri.UnescapeDataString(keyValue[0].Trim()),
                                                     Value: Uri.UnescapeDataString(keyValue[1].Trim())))
                                .ToList();

            // The name order decides, not the order in the url - same as the reader does
            foreach (var name in queryNames)
            {
                var value = parameters.FirstOrDefault(p => p.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;

                if (value.IsNotNullOrWhiteSpace())
                {
                    return StripVersionPrefix(value);
                }
            }

            return null;
        }

        private static string? ResolveFromHeaders(ILookup<string, string> headers,
                                                  VersionParameterNames names)
        {
            foreach (var headerName in names.Header)
            {
                var version = headers[headerName].FirstOrDefault(value => value.IsNotNullOrWhiteSpace());

                if (version.IsNotNull())
                {
                    return StripVersionPrefix(version);
                }
            }

            return ResolveFromMediaType(headers, names.MediaTypeParameter);
        }

        /// <summary>
        ///     Media type versioning: "Accept: application/json;v=2" - the version is a parameter of
        ///     the media type in Accept or Content-Type.
        /// </summary>
        private static string? ResolveFromMediaType(ILookup<string, string> headers,
                                                    ImmutableList<string> parameterNames)
        {
            if (parameterNames.IsEmpty)
            {
                return null;
            }

            var mediaTypes = MediaTypeHeaderNames.SelectMany(headerName => headers[headerName])
                                                 .SelectMany(value => value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                                 .Select(value => MediaTypeHeaderValue.TryParse(value.Trim(), out var mediaType) ? mediaType : null)
                                                 .Where(mediaType => mediaType.IsNotNull())
                                                 .ToList();

            foreach (var parameterName in parameterNames)
            {
                var version = mediaTypes.SelectMany(mediaType => mediaType!.Parameters)
                                        .FirstOrDefault(parameter => parameter.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                                        ?.Value?.Trim('"');

                if (version.IsNotNullOrWhiteSpace())
                {
                    return StripVersionPrefix(version);
                }
            }

            return null;
        }

        private static ILookup<string, string> ToLookup(IReadOnlyDictionary<string, string>? headers)
        {
            return (headers ?? ImmutableDictionary<string, string>.Empty).ToLookup(header => header.Key,
                                                                                   header => header.Value,
                                                                                   StringComparer.OrdinalIgnoreCase);
        }

        private static ILookup<string, string> ToLookup(HttpRequestHeaders? headers)
        {
            return (headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                   .SelectMany(header => header.Value.Select(value => (header.Key, Value: value)))
                   .ToLookup(header => header.Key,
                             header => header.Value,
                             StringComparer.OrdinalIgnoreCase);
        }

        private static string StripVersionPrefix(string version)
        {
            version = version.Trim();

#pragma warning disable CA1867
            return version.StartsWith("v", StringComparison.OrdinalIgnoreCase)
#pragma warning restore CA1867
                       ? version.Substring(1)
                       : version;
        }

        /// <summary>
        ///     The names to look at per location. Detected from the api's reader; when nothing was
        ///     detected, the common names stand in - and media type versioning stays off.
        /// </summary>
        private sealed record VersionParameterNames(ImmutableList<string> Query,
                                                    ImmutableList<string> Header,
                                                    ImmutableList<string> MediaTypeParameter)
        {
            public static VersionParameterNames From(ImmutableList<ApiVersionParameter> detected)
            {
                if (detected.IsEmpty)
                {
                    return new VersionParameterNames(CommonQueryNames, CommonHeaderNames, ImmutableList<string>.Empty);
                }

                return new VersionParameterNames(NamesOf(detected, ApiVersionParameterKind.Query),
                                                 NamesOf(detected, ApiVersionParameterKind.Header),
                                                 NamesOf(detected, ApiVersionParameterKind.MediaTypeParameter));
            }

            private static ImmutableList<string> NamesOf(ImmutableList<ApiVersionParameter> detected,
                                                         ApiVersionParameterKind kind)
            {
                return detected.Where(parameter => parameter.Kind == kind)
                               .Select(parameter => parameter.Name)
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToImmutableList();
            }
        }
    }
}
