using System.Collections;
using System.Collections.Immutable;
using Asp.Versioning;
using Extensions.Pack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
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
        ///     Gets all parsed endpoints with resolved metadata.
        ///     Results are cached after first call.
        /// </summary>
        ImmutableList<EndpointInfo> GetEndpoints();
    }

    /// <summary>
    ///     Strategy interface for parsing specific types of route endpoints.
    /// </summary>
    public interface IRouteEndpointParser
    {
        /// <summary>
        ///     Determines if this parser can handle the given route endpoint.
        /// </summary>
        bool CanHandle(RouteEndpoint routeEndpoint);

        /// <summary>
        ///     Parses the route endpoint into one or more EndpointInfo objects.
        /// </summary>
        ImmutableList<EndpointInfo> Parse(RouteEndpoint routeEndpoint);
    }

    /// <summary>
    ///     Shared utilities for endpoint parsing.
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
                // Format: "1.0" or "1" (without 'v' prefix unless explicitly needed)
                var versionString = apiVersion.MajorVersion.ToString();

                // Common placeholders for API version
                // {apiVersion:apiVersion} is used in patterns like "v{apiVersion:apiVersion}/users"
                // so we replace with just the number, not "v1"
                resolved = resolved.Replace("{apiVersion:apiVersion}", versionString, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{apiVersion}", versionString, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{version}", versionString, StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{documentName}", $"v{versionString}", StringComparison.OrdinalIgnoreCase);
                resolved = resolved.Replace("{version:apiversion}", versionString, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // If no version is found, replace with default
                resolved = resolved.Replace("{apiVersion:apiVersion}", "1", StringComparison.OrdinalIgnoreCase);
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
        ///     Extracts all response types with their status codes from ProducesResponseTypeAttribute.
        ///     Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static ImmutableDictionary<int, Type> ExtractResponseTypesByStatusCode(RouteEndpoint routeEndpoint)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, Type>();

            foreach (var metadata in GetProducesResponseMetadata(routeEndpoint))
            {
                if (TryGetStatusCodeAndType(metadata, out var statusCode, out var responseType))
                {
                    // Add or update - later entries win (more specific)
                    builder[statusCode] = responseType;
                }
            }

            return builder.ToImmutable();
        }

        internal static IEnumerable<object> GetProducesResponseMetadata(RouteEndpoint routeEndpoint)
        {
            foreach (var metadata in routeEndpoint.Metadata)
            {
                if (metadata is IProducesResponseTypeMetadata or ProducesResponseTypeMetadata)
                {
                    yield return metadata;
                }
            }
        }

        internal static bool TryGetStatusCodeAndType(object metadata,
                                                     out int statusCode,
                                                     out Type responseType)
        {
            statusCode = default;
            responseType = default!;

            if (metadata is IProducesResponseTypeMetadata producesResponseTypeMetadata)
            {
                if (producesResponseTypeMetadata.Type.IsNull())
                {
                    return false;
                }

                statusCode = producesResponseTypeMetadata.StatusCode;
                responseType = UnwrapTaskType(producesResponseTypeMetadata.Type);

                return true;
            }

            if (metadata is ProducesResponseTypeMetadata producesMetadata && producesMetadata.Type.IsNotNull())
            {
                statusCode = producesMetadata.StatusCode;
                responseType = UnwrapTaskType(producesMetadata.Type);

                return true;
            }

            var statusCodeProperty = metadata.GetType().GetProperty("StatusCode");
            var typeProperty = metadata.GetType().GetProperty("Type");

            if (statusCodeProperty?.GetValue(metadata) is int reflectedStatusCode &&
                typeProperty?.GetValue(metadata) is Type reflectedType)
            {
                statusCode = reflectedStatusCode;
                responseType = UnwrapTaskType(reflectedType);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Extracts tags from endpoint metadata.
        ///     Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static ImmutableList<string> ExtractTags(RouteEndpoint routeEndpoint)
        {
            // Try ITagsMetadata (Minimal API: WithTags())
            var tagsMetadata = routeEndpoint.Metadata.GetMetadata<ITagsMetadata>();

            if (tagsMetadata.IsNotNull())
            {
                return tagsMetadata.Tags.ToImmutableList();
            }

            return ImmutableList<string>.Empty;
        }

        /// <summary>
        ///     Extracts endpoint name from metadata.
        ///     Works for both MVC Controllers and Minimal APIs.
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
        ///     Extracts endpoint description from metadata.
        ///     Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static string? ExtractDescription(RouteEndpoint routeEndpoint)
        {
            // Try IEndpointDescriptionMetadata (Minimal API: WithDescription())
            var descriptionMetadata = routeEndpoint.Metadata.GetMetadata<IEndpointDescriptionMetadata>();

            if (descriptionMetadata.IsNotNull())
            {
                return descriptionMetadata.Description;
            }

            return null;
        }

        /// <summary>
        ///     Extracts endpoint summary from metadata.
        ///     Works for both MVC Controllers and Minimal APIs.
        /// </summary>
        public static string? ExtractSummary(RouteEndpoint routeEndpoint)
        {
            // Try IEndpointSummaryMetadata (Minimal API: WithSummary())
            var summaryMetadata = routeEndpoint.Metadata.GetMetadata<IEndpointSummaryMetadata>();

            if (summaryMetadata.IsNotNull())
            {
                return summaryMetadata.Summary;
            }

            return null;
        }

        /// <summary>
        ///     Builds the full route pattern including any base path from the route endpoint.
        ///     This handles cases where UsePathBase or similar middleware adds a path prefix.
        /// </summary>
        public static string BuildFullRoutePattern(RouteEndpoint routeEndpoint,
                                                   ApiVersion? apiVersion)
        {
            if (routeEndpoint.RoutePattern.PathSegments.Count == 0)
            {
                return "/";
            }

            // Reconstruct the full path from segments
            var segments = new List<string>();

            foreach (var segment in routeEndpoint.RoutePattern.PathSegments)
            {
                if (segment.IsSimple && segment.Parts.Count == 1)
                {
                    var part = segment.Parts[0];

                    if (part is RoutePatternLiteralPart literal)
                    {
                        segments.Add(literal.Content);
                    }
                    else if (part is RoutePatternParameterPart parameter)
                    {
                        var paramName = "{" + parameter.Name;

                        // Include constraints if present
                        if (parameter.ParameterPolicies.Count > 0)
                        {
                            var policyNames = string.Join(":", parameter.ParameterPolicies.Select(p => p.Content));
                            paramName += ":" + policyNames;
                        }

                        paramName += "}";
                        segments.Add(paramName);
                    }
                }
                else
                {
                    // Complex segment with multiple parts
                    var segmentText = string.Join(string.Empty, segment.Parts.Select(p =>
                    {
                        if (p is RoutePatternLiteralPart lit)
                        {
                            return lit.Content;
                        }

                        if (p is RoutePatternParameterPart param)
                        {
                            var paramName = "{" + param.Name;

                            if (param.ParameterPolicies.Count > 0)
                            {
                                var policyNames = string.Join(":", param.ParameterPolicies.Select(pp => pp.Content));
                                paramName += ":" + policyNames;
                            }

                            paramName += "}";

                            return paramName;
                        }

                        return string.Empty;
                    }));

                    segments.Add(segmentText);
                }
            }

            var fullPattern = "/" + string.Join("/", segments);

            // Resolve placeholders
            var resolvedUrl = ResolvePlaceholders(fullPattern, apiVersion);

            return resolvedUrl;
        }
    }

    /// <summary>
    ///     Empty endpoint info parser for cases where no EndpointDataSource is available.
    ///     Used as a fallback in static contexts.
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
    ///     Parser for MVC Controller endpoints.
    ///     Identifies endpoints by presence of ControllerActionDescriptor metadata.
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

            // Build the full route pattern including route groups and prefixes
            var resolvedUrl = EndpointParsingHelpers.BuildFullRoutePattern(routeEndpoint, apiVersion);

            // Get response type from controller action method (deprecated, for backward compatibility)
            var responseType = ExtractResponseType(controllerActionDescriptor);

            // Extract all metadata
            var responseTypesByStatusCode = ExtractResponseTypesByStatusCodeFromController(routeEndpoint, controllerActionDescriptor);
            var tags = EndpointParsingHelpers.ExtractTags(routeEndpoint);
            var name = EndpointParsingHelpers.ExtractName(routeEndpoint);
            var description = EndpointParsingHelpers.ExtractDescription(routeEndpoint);
            var summary = EndpointParsingHelpers.ExtractSummary(routeEndpoint);
            var sourceLocation = ExtractSourceLocation(controllerActionDescriptor);

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
                                                                     Summary = summary,
                                                                     SourceLocation = sourceLocation
                                                                 }).ToImmutableList();

            return endpointInfos;
        }

        private static ApiVersion? ExtractApiVersion(RouteEndpoint routeEndpoint)
        {
            // Try ApiVersionAttribute (MVC Controllers)
            var apiVersionAttribute = routeEndpoint.Metadata.FirstOrDefault(f => f.GetType().FullName == "Microsoft.AspNetCore.Mvc.ApiVersionAttribute");

            if (apiVersionAttribute.IsNotNull())
            {
                var enumerable = apiVersionAttribute.GetType().GetProperty("Versions")?.GetValue(apiVersionAttribute) as IEnumerable;

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

        private static string ExtractSourceLocation(ControllerActionDescriptor controllerActionDescriptor)
        {
            var controllerName = controllerActionDescriptor.ControllerTypeInfo.FullName;
            var actionName = controllerActionDescriptor.ActionName;

            // Format: ControllerName.ActionName
            // Example: "PersonController.CreatePerson"
            return $"{controllerName}.{actionName}";
        }

        private static ImmutableDictionary<int, Type> ExtractResponseTypesByStatusCodeFromController(RouteEndpoint routeEndpoint,
                                                                                                     ControllerActionDescriptor controllerActionDescriptor)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, Type>();

            // First try to extract from endpoint metadata (standard way)
            foreach (var metadata in EndpointParsingHelpers.GetProducesResponseMetadata(routeEndpoint))
            {
                if (EndpointParsingHelpers.TryGetStatusCodeAndType(metadata, out var statusCode, out var responseType))
                {
                    builder[statusCode] = responseType;
                }
            }

            // If we found metadata, return it
            if (builder.Count > 0)
            {
                return builder.ToImmutable();
            }

            // Fallback: Extract from method attributes directly
            // This is needed because MVC Controllers don't always populate endpoint metadata
            var methodInfo = controllerActionDescriptor.MethodInfo;
            var producesResponseTypeAttributes = methodInfo.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), inherit: true);

            foreach (var attribute in producesResponseTypeAttributes)
            {
                if (attribute is ProducesResponseTypeAttribute producesAttr)
                {
                    var statusCode = producesAttr.StatusCode;
                    var type = producesAttr.Type;

                    if (type.IsNotNull())
                    {
                        builder[statusCode] = EndpointParsingHelpers.UnwrapTaskType(type);
                    }
                }
            }

            return builder.ToImmutable();
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
    ///     Parser for Minimal API endpoints.
    ///     Identifies endpoints by absence of ControllerActionDescriptor metadata.
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

            // Build the full route pattern including route groups and prefixes
            var resolvedUrl = EndpointParsingHelpers.BuildFullRoutePattern(routeEndpoint, apiVersion);

            // Get response type from ProducesAttribute (deprecated, for backward compatibility)
            var responseType = ExtractResponseType(routeEndpoint);

            // Extract all metadata
            var responseTypesByStatusCode = EndpointParsingHelpers.ExtractResponseTypesByStatusCode(routeEndpoint);
            var tags = EndpointParsingHelpers.ExtractTags(routeEndpoint);
            var name = EndpointParsingHelpers.ExtractName(routeEndpoint);
            var description = EndpointParsingHelpers.ExtractDescription(routeEndpoint);
            var summary = EndpointParsingHelpers.ExtractSummary(routeEndpoint);
            var sourceLocation = ExtractSourceLocation(routeEndpoint);

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
                                                                     Summary = summary,
                                                                     SourceLocation = sourceLocation
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

            if (apiVersionMetadata.IsNotNull())
            {
                apiVersionMetadata.Deconstruct(out var model, out _);

                if (model.DeclaredApiVersions.Count > 0)
                {
                    return model.DeclaredApiVersions[0];
                }
            }

            return null;
        }

        private static Type? ExtractResponseType(RouteEndpoint routeEndpoint)
        {
            // Try to find ProducesAttribute for HTTP 200 OK
            var producesAttributes = EndpointParsingHelpers.GetProducesResponseMetadata(routeEndpoint)
                                                           .Select(metadata => EndpointParsingHelpers.TryGetStatusCodeAndType(metadata, out var statusCode, out var responseType)
                                                                                   ? new
                                                                                     {
                                                                                         StatusCode = statusCode,
                                                                                         Type = responseType
                                                                                     }
                                                                                   : null)
                                                           .Where(x => x.IsNotNull())
                                                           .Select(x => x!)
                                                           .ToList();

            // Find the success response type (HTTP 200)
            var successResponse = producesAttributes.FirstOrDefault(m => m.StatusCode is >= 200 and < 300);

            if (successResponse?.Type.IsNull() ?? true)
            {
                return null;
            }

            return successResponse.Type;
        }

        private static string? ExtractSourceLocation(RouteEndpoint routeEndpoint)
        {
            // Try to extract from MethodInfo metadata (best source for Minimal APIs)
            var methodInfo = routeEndpoint.Metadata.OfType<System.Reflection.MethodInfo>().FirstOrDefault();

            if (methodInfo.IsNotNull())
            {
                var declaringType = methodInfo.DeclaringType;

                if (declaringType.IsNotNull())
                {
                    // Check if it's a compiler-generated type (lambda/anonymous)
                    if (declaringType.Name.Contains('<') || declaringType.Name.Contains("DisplayClass"))
                    {
                        // It's a lambda - try to get the parent type
                        var parentType = declaringType.DeclaringType;

                        if (parentType.IsNotNull() && parentType.Name != "Program")
                        {
                            // Extension method class (e.g., PersonEndpoints)
                            return parentType.FullName;
                        }

                        // Lambda in Program.cs
                        return "Program.cs (Inline Lambda)";
                    }

                    // Normal method (e.g., extension method in separate class)
                    var fullName = declaringType.FullName?.Replace("+", ".");

                    if (fullName.IsNotNullOrWhiteSpace())
                    {
                        return $"{fullName}.{methodInfo.Name}";
                    }

                    return $"{declaringType.Name}.{methodInfo.Name}";
                }
            }

            // Fallback: Try to parse display name
            var displayName = routeEndpoint.DisplayName;

            if (displayName.IsNotNullOrWhiteSpace())
            {
                // Minimal API endpoints often have display names like:
                // "HTTP: POST api/v1/persons => MapPersonEndpoints"
                var arrowIndex = displayName.IndexOf(" => ", StringComparison.Ordinal);

                if (arrowIndex > 0)
                {
                    var sourceLocation = displayName.Substring(arrowIndex + 4).Trim();

                    // Skip lambda indicators
                    if (sourceLocation.Contains("Program>$") || sourceLocation.Contains("<>c"))
                    {
                        return "Program.cs (Inline Lambda)";
                    }

                    return sourceLocation;
                }
            }

            // Fallback to endpoint name if available
            var endpointName = EndpointParsingHelpers.ExtractName(routeEndpoint);

            if (endpointName.IsNotNullOrWhiteSpace())
            {
                return endpointName;
            }

            // Last resort
            return "Minimal API (Unknown Source)";
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

        private static ImmutableList<EndpointInfo> ParseEndpoints(EndpointDataSource endpointDataSource,
                                                                  IEnumerable<IRouteEndpointParser> routeEndpointParsers)
        {
            var endpoints = endpointDataSource.Endpoints;

            if (endpoints.IsEmpty())
            {
                return ImmutableList<EndpointInfo>.Empty;
            }

            var routeEndpoints = endpoints.OfType<RouteEndpoint>();

            var parsedEndpoints = routeEndpoints.SelectMany(routeEndpoint => ParseRouteEndpoint(routeEndpoint, routeEndpointParsers))
                                                .ToImmutableList();

            return parsedEndpoints;
        }

        private static ImmutableList<EndpointInfo> ParseRouteEndpoint(RouteEndpoint routeEndpoint,
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