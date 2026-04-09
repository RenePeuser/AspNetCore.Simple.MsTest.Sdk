using System;
using System.Collections.Immutable;
using System.Linq;
using Asp.Versioning;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddEndpointInfoParserExtension
    {
        public static void AddEndpointInfoParser(this IServiceCollection services)
        {
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

    internal sealed class EndpointInfoParser(EndpointDataSource endpointDataSource) : IEndpointInfoParser
    {
        private readonly Lazy<ImmutableList<EndpointInfo>> _endpoints = new(() => ParseEndpoints(endpointDataSource));

        public ImmutableList<EndpointInfo> GetEndpoints()
        {
            return _endpoints.Value;
        }

        private static ImmutableList<EndpointInfo> ParseEndpoints(EndpointDataSource endpointDataSource)
        {
            var endpoints = endpointDataSource.Endpoints;

            if (endpoints.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var routeEndpoints = endpoints.OfType<RouteEndpoint>();

            var parsedEndpoints = routeEndpoints
                                  .SelectMany(ParseRouteEndpoint)
                                  .ToImmutableList();

            return parsedEndpoints;
        }

        private static ImmutableList<EndpointInfo> ParseRouteEndpoint(RouteEndpoint routeEndpoint)
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
            var resolvedUrl = ResolvePlaceholders(routePattern, apiVersion);

            // Get response type
            var responseType = ExtractResponseType(routeEndpoint);

            // Create EndpointInfo for each HTTP method
            var endpointInfos = httpMethods.Select(httpMethod => new EndpointInfo
                                                                 {
                                                                     HttpMethod = httpMethod,
                                                                     Url = resolvedUrl,
                                                                     ApiVersion = apiVersion,
                                                                     ResponseType = responseType
                                                                 }).ToImmutableList();

            return endpointInfos;
        }

        private static ApiVersion? ExtractApiVersion(RouteEndpoint routeEndpoint)
        {
            // Try new Asp.Versioning.ApiVersionAttribute (modern approach)
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

            // Try Minimal API - IApiVersionProvider interface
            var apiVersionProvider = routeEndpoint.Metadata.GetMetadata<IApiVersionProvider>();

            if (apiVersionProvider.IsNotNull())
            {
                var versions = apiVersionProvider.Versions;

                if (versions.Count > 0)
                {
                    return versions[0];
                }
            }

            // Try Minimal API - ApiVersionMetadata (for neutrality check)
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
            // Try to find ProducesResponseTypeAttribute for HTTP 200 OK
            // Both MVC and Minimal API use this attribute
            var controllerActionDescriptors = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionDescriptors.IsNotNull())
            {
                var controllerReturnType = controllerActionDescriptors.MethodInfo.ReturnType;

                // Unwrap Task<T> to T
                return UnwrapTaskType(controllerReturnType);
            }

            var producesAttributes = routeEndpoint.Metadata.GetOrderedMetadata<ProducesAttribute>();


            // Find the success response type (HTTP 200)
            var successResponse = producesAttributes.FirstOrDefault(m => m.StatusCode >= 200 && m.StatusCode < 300);

            if (successResponse?.Type.IsNull() ?? true)
            {
                return null;
            }

            // Unwrap Task<T> to T
            return UnwrapTaskType(successResponse.Type);
        }

        private static Type UnwrapTaskType(Type type)
        {
            // Check if type is Task<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // Extract T from Task<T>
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        private static string ResolvePlaceholders(string routePattern,
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
    }
}
