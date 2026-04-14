using System.Collections.Immutable;
using Asp.Versioning;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddEndpointInfoParserExtension
    {
        public static void AddEndpointInfoParser(this IServiceCollection services)
        {
            // Register specific parsers
            services.AddControllerEndpointParser();
            services.AddMinimalApiEndpointParser();

            // EndpointDataSource is registered by the host application
            services.AddSingletonIfNotExists<IEndpointInfoParser, EndpointInfoParser>();
        }
    }

    public interface IEndpointInfoParser
    {
        /// <summary>
        /// Gets all parsed endpoints with resolved metadata.
        /// Results are cached after first call.
        /// </summary>
        ImmutableList<EndpointInfo> GetEndpoints();
    }

    /// <summary>
    /// Strategy interface for parsing specific types of route endpoints.
    /// </summary>
    public interface IRouteEndpointParser
    {
        /// <summary>
        /// Determines if this parser can handle the given route endpoint.
        /// </summary>
        bool CanHandle(RouteEndpoint routeEndpoint);

        /// <summary>
        /// Parses the route endpoint into one or more EndpointInfo objects.
        /// </summary>
        ImmutableList<EndpointInfo> Parse(RouteEndpoint routeEndpoint);
    }

    /// <summary>
    /// Shared utilities for endpoint parsing.
    /// </summary>
    internal static class EndpointParsingHelpers
    {
        public static Type UnwrapTaskType(Type type)
        {
            // Check if type is Task<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // Extract T from Task<T>
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        public static string ResolvePlaceholders(string routePattern,
                                                 ApiVersion? apiVersion)
        {
            var resolved = routePattern;

            if (apiVersion.IsNotNull())
            {
                // Format: "1.0" or "1" (without 'v' prefix)
                var versionString = apiVersion.MajorVersion.ToString();

                // Common placeholders for API version
                resolved = resolved.Replace("{apiVersion}", versionString, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{version}", versionString, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{documentName}", $"v{versionString}", StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{version:apiversion}", $"{versionString}", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // If no version is found, replace with empty or remove placeholder
                resolved = resolved.Replace("{apiVersion}", string.Empty, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{version}", string.Empty, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{documentName}", "v1", StringComparison.OrdinalIgnoreCase);
            }

            // Clean up double slashes
            while (resolved.Contains("//"))
            {
                resolved = resolved.Replace("//", "/");
            }

            return resolved;
        }

        /// <summary>
        /// Extracts all response types with their status codes from ProducesResponseTypeAttribute.
        /// Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static ImmutableDictionary<int, Type> ExtractResponseTypesByStatusCode(RouteEndpoint routeEndpoint)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, Type>();

            // Get all ProducesResponseTypeAttribute
            var producesMetadata = routeEndpoint.Metadata.GetOrderedMetadata<ProducesResponseTypeAttribute>();

            foreach (var metadata in producesMetadata)
            {
                if (metadata.Type.IsNotNull())
                {
                    var statusCode = metadata.StatusCode;
                    var responseType = UnwrapTaskType(metadata.Type);

                    // Add or update - later entries win (more specific)
                    builder[statusCode] = responseType;
                }
            }

            return builder.ToImmutable();
        }

        /// <summary>
        /// Extracts tags from endpoint metadata.
        /// Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static ImmutableList<string> ExtractTags(RouteEndpoint routeEndpoint)
        {
            // Try ITagsMetadata (Minimal API: WithTags())
            var tagsMetadata = routeEndpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Http.Metadata.ITagsMetadata>();

            if (tagsMetadata.IsNotNull())
            {
                return tagsMetadata.Tags.ToImmutableList();
            }

            return ImmutableList<string>.Empty;
        }

        /// <summary>
        /// Extracts endpoint name from metadata.
        /// Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static string? ExtractName(RouteEndpoint routeEndpoint)
        {
            // Try IEndpointNameMetadata (Minimal API: WithName())
            var nameMetadata = routeEndpoint.Metadata.GetMetadata<IEndpointNameMetadata>();

            if (nameMetadata.IsNotNull())
            {
                return nameMetadata.EndpointName;
            }

            // Fallback: RouteEndpoint.DisplayName
            return routeEndpoint.DisplayName;
        }

        /// <summary>
        /// Extracts endpoint description from metadata.
        /// Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static string? ExtractDescription(RouteEndpoint routeEndpoint)
        {
            // Try IEndpointDescriptionMetadata (Minimal API: WithDescription())
            var descriptionMetadata = routeEndpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Http.Metadata.IEndpointDescriptionMetadata>();

            if (descriptionMetadata.IsNotNull())
            {
                return descriptionMetadata.Description;
            }

            return null;
        }

        /// <summary>
        /// Extracts endpoint summary from metadata.
        /// Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static string? ExtractSummary(RouteEndpoint routeEndpoint)
        {
            // Try IEndpointSummaryMetadata (Minimal API: WithSummary())
            var summaryMetadata = routeEndpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Http.Metadata.IEndpointSummaryMetadata>();

            if (summaryMetadata.IsNotNull())
            {
                return summaryMetadata.Summary;
            }

            return null;
        }
    }

    /// <summary>
    /// Empty endpoint info parser for cases where no EndpointDataSource is available.
    /// Used as a fallback in static contexts.
    /// </summary>
    internal sealed class EmptyEndpointInfoParser : IEndpointInfoParser
    {
        public ImmutableList<EndpointInfo> GetEndpoints()
        {
            return ImmutableList<EndpointInfo>.Empty;
        }
    }

    // ==================== Controller Endpoint Parser ====================

    public static class AddControllerEndpointParserExtension
    {
        public static void AddControllerEndpointParser(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IRouteEndpointParser, ControllerEndpointParser>();
        }
    }

    /// <summary>
    /// Parser for MVC Controller endpoints.
    /// Identifies endpoints by presence of ControllerActionDescriptor metadata.
    /// </summary>
    internal sealed class ControllerEndpointParser : IRouteEndpointParser
    {
        public bool CanHandle(RouteEndpoint routeEndpoint)
        {
            var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            return controllerActionDescriptor.IsNotNull();
        }

        public ImmutableList<EndpointInfo> Parse(RouteEndpoint routeEndpoint)
        {
            var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor.IsNull())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            // Get HTTP methods
            var httpMethodMetadata = routeEndpoint.Metadata.GetMetadata<HttpMethodMetadata>();

            if (httpMethodMetadata.IsNull())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var httpMethods = httpMethodMetadata.HttpMethods;

            if (httpMethods.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            // Get API version
            var apiVersion = ExtractApiVersion(routeEndpoint);

            // Get route pattern
            var routePattern = routeEndpoint.RoutePattern.RawText ?? string.Empty;

            // Resolve placeholders
            var resolvedUrl = EndpointParsingHelpers.ResolvePlaceholders(routePattern, apiVersion);

            // Get response type from controller action method (deprecated, for backward compatibility)
            var responseType = ExtractResponseType(controllerActionDescriptor);

            // Extract all metadata
            var responseTypesByStatusCode = EndpointParsingHelpers.ExtractResponseTypesByStatusCode(routeEndpoint);
            var tags = EndpointParsingHelpers.ExtractTags(routeEndpoint);
            var name = EndpointParsingHelpers.ExtractName(routeEndpoint);
            var description = EndpointParsingHelpers.ExtractDescription(routeEndpoint);
            var summary = EndpointParsingHelpers.ExtractSummary(routeEndpoint);

            // Create EndpointInfo for each HTTP method
            var endpointInfos = httpMethods.Select(httpMethod => new EndpointInfo
                                                                 {
                                                                     HttpMethod = httpMethod,
                                                                     Url = resolvedUrl,
                                                                     ApiVersion = apiVersion,
                                                                     ResponseType = responseType,
                                                                     ResponseTypesByStatusCode = responseTypesByStatusCode,
                                                                     Tags = tags,
                                                                     Name = name,
                                                                     Description = description,
                                                                     Summary = summary
                                                                 }).ToImmutableList();

            return endpointInfos;
        }

        private static ApiVersion? ExtractApiVersion(RouteEndpoint routeEndpoint)
        {
            // Try ApiVersionAttribute (MVC Controllers)
            var apiVersionAttribute = routeEndpoint.Metadata.FirstOrDefault(f => f.GetType().FullName == "Microsoft.AspNetCore.Mvc.ApiVersionAttribute");

            if (apiVersionAttribute.IsNotNull())
            {
                var enumerable = apiVersionAttribute.GetType().GetProperty("Versions")?.GetValue(apiVersionAttribute) as System.Collections.IEnumerable;

                if (enumerable.IsNullOrEmpty())
                {
                    return null;
                }

                // ApiVersionAttribute can have multiple versions, take the first one
                var version = enumerable.FirstOfType<object>();
                var major = version.GetType().GetProperty("MajorVersion")?.GetValue(version)?.Cast<int?>();
                var minor = version.GetType().GetProperty("MinorVersion")?.GetValue(version)?.Cast<int?>();

                return new ApiVersion(major ?? 0, minor ?? 1);
            }

            // Try IApiVersionProvider interface
            var apiVersionProvider = routeEndpoint.Metadata.GetMetadata<IApiVersionProvider>();

            if (apiVersionProvider.IsNotNull())
            {
                var versions = apiVersionProvider.Versions;

                if (versions.Count > 0)
                {
                    return versions[0];
                }
            }

            // Try ApiVersionMetadata (for neutrality check)
            var apiVersionMetadata = routeEndpoint.Metadata.GetMetadata<ApiVersionMetadata>();

            if (apiVersionMetadata.IsNotNull() && apiVersionMetadata.IsApiVersionNeutral)
            {
                // Endpoint is version-neutral (supports all versions)
                return null;
            }

            return null;
        }

        private static Type? ExtractResponseType(ControllerActionDescriptor controllerActionDescriptor)
        {
            var controllerReturnType = controllerActionDescriptor.MethodInfo.ReturnType;

            // Unwrap Task<T> to T
            return EndpointParsingHelpers.UnwrapTaskType(controllerReturnType);
        }
    }

    // ==================== Minimal API Endpoint Parser ====================

    public static class AddMinimalApiEndpointParserExtension
    {
        public static void AddMinimalApiEndpointParser(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IRouteEndpointParser, MinimalApiEndpointParser>();
        }
    }

    /// <summary>
    /// Parser for Minimal API endpoints.
    /// Identifies endpoints by absence of ControllerActionDescriptor metadata.
    /// </summary>
    internal sealed class MinimalApiEndpointParser : IRouteEndpointParser
    {
        public bool CanHandle(RouteEndpoint routeEndpoint)
        {
            // Minimal API endpoints don't have ControllerActionDescriptor
            var controllerActionDescriptor = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            return controllerActionDescriptor.IsNull();
        }

        public ImmutableList<EndpointInfo> Parse(RouteEndpoint routeEndpoint)
        {
            // Get HTTP methods
            var httpMethodMetadata = routeEndpoint.Metadata.GetMetadata<HttpMethodMetadata>();

            if (httpMethodMetadata.IsNull())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var httpMethods = httpMethodMetadata.HttpMethods;

            if (httpMethods.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            // Get API version
            var apiVersion = ExtractApiVersion(routeEndpoint);

            // Get route pattern
            var routePattern = routeEndpoint.RoutePattern.RawText ?? string.Empty;

            // Resolve placeholders
            var resolvedUrl = EndpointParsingHelpers.ResolvePlaceholders(routePattern, apiVersion);

            // Get response type from ProducesAttribute (deprecated, for backward compatibility)
            var responseType = ExtractResponseType(routeEndpoint);

            // Extract all metadata
            var responseTypesByStatusCode = EndpointParsingHelpers.ExtractResponseTypesByStatusCode(routeEndpoint);
            var tags = EndpointParsingHelpers.ExtractTags(routeEndpoint);
            var name = EndpointParsingHelpers.ExtractName(routeEndpoint);
            var description = EndpointParsingHelpers.ExtractDescription(routeEndpoint);
            var summary = EndpointParsingHelpers.ExtractSummary(routeEndpoint);

            // Create EndpointInfo for each HTTP method
            var endpointInfos = httpMethods.Select(httpMethod => new EndpointInfo
                                                                 {
                                                                     HttpMethod = httpMethod,
                                                                     Url = resolvedUrl,
                                                                     ApiVersion = apiVersion,
                                                                     ResponseType = responseType,
                                                                     ResponseTypesByStatusCode = responseTypesByStatusCode,
                                                                     Tags = tags,
                                                                     Name = name,
                                                                     Description = description,
                                                                     Summary = summary
                                                                 }).ToImmutableList();

            return endpointInfos;
        }

        private static ApiVersion? ExtractApiVersion(RouteEndpoint routeEndpoint)
        {
            // Try IApiVersionProvider interface (Minimal API)
            var apiVersionProvider = routeEndpoint.Metadata.GetMetadata<IApiVersionProvider>();

            if (apiVersionProvider.IsNotNull())
            {
                var versions = apiVersionProvider.Versions;

                if (versions.Count > 0)
                {
                    return versions[0];
                }
            }

            // Try ApiVersionMetadata (for neutrality check)
            var apiVersionMetadata = routeEndpoint.Metadata.GetMetadata<ApiVersionMetadata>();

            if (apiVersionMetadata.IsNotNull() && apiVersionMetadata.IsApiVersionNeutral)
            {
                // Endpoint is version-neutral (supports all versions)
                return null;
            }

            return null;
        }

        private static Type? ExtractResponseType(RouteEndpoint routeEndpoint)
        {
            // Try to find ProducesAttribute for HTTP 200 OK
            var producesAttributes = routeEndpoint.Metadata.GetOrderedMetadata<ProducesAttribute>();

            // Find the success response type (HTTP 200)
            var successResponse = producesAttributes.FirstOrDefault(m => m.StatusCode >= 200 && m.StatusCode < 300);

            if (successResponse?.Type.IsNull() ?? true)
            {
                return null;
            }

            // Unwrap Task<T> to T
            return EndpointParsingHelpers.UnwrapTaskType(successResponse.Type);
        }
    }

    // ==================== Main EndpointInfoParser ====================

    internal sealed class EndpointInfoParser(EndpointDataSource endpointDataSource,
                                             IEnumerable<IRouteEndpointParser> routeEndpointParsers) : IEndpointInfoParser
    {
        private readonly Lazy<ImmutableList<EndpointInfo>> _endpoints = new(() => ParseEndpoints(endpointDataSource, routeEndpointParsers));

        public ImmutableList<EndpointInfo> GetEndpoints()
        {
            return _endpoints.Value;
        }

        private static ImmutableList<EndpointInfo> ParseEndpoints(
            EndpointDataSource endpointDataSource,
            IEnumerable<IRouteEndpointParser> routeEndpointParsers)
        {
            var endpoints = endpointDataSource.Endpoints;

            if (endpoints.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var routeEndpoints = endpoints.OfType<RouteEndpoint>();

            var parsedEndpoints = routeEndpoints
                                  .SelectMany(routeEndpoint => ParseRouteEndpoint(routeEndpoint, routeEndpointParsers))
                                  .ToImmutableList();

            return parsedEndpoints;
        }

        private static ImmutableList<EndpointInfo> ParseRouteEndpoint(
            RouteEndpoint routeEndpoint,
            IEnumerable<IRouteEndpointParser> routeEndpointParsers)
        {
            // Find the first parser that can handle this endpoint
            var parser = routeEndpointParsers.FirstOrDefault(p => p.CanHandle(routeEndpoint));

            if (parser.IsNull())
            {
                // No parser found - log or handle gracefully
                return ImmutableList<EndpointInfo>.Empty;
            }

            return parser.Parse(routeEndpoint);
        }
    }
}
