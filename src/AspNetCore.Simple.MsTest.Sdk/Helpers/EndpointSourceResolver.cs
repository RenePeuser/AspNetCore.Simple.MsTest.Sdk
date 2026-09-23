using System;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class AddEndpointSourceResolverExtension
    {
        public static void AddEndpointSourceResolver(this IServiceCollection services)
        {
            services.AddEndpointProvider();
            services.AddSourceLocationHelper();
            services.AddSingletonIfNotExists<IEndpointSourceResolver, EndpointSourceResolver>();
        }
    }

    public interface IEndpointSourceResolver
    {
        /// <summary>
        /// Resolves the source file of the endpoint that serves the http call, as a clickable file:/// URI.
        /// Returns an empty string when the call does not hit one of our own endpoints - an external api,
        /// no host, an unknown or ambiguous route.
        /// </summary>
        /// <param name="context">The http call.</param>
        /// <param name="knownEndpoint">An endpoint the caller already matched - skips the lookup.</param>
#pragma warning disable CA1055 // Returns string for output purposes, not for navigation
        string Resolve(IHttpAssertContext context,
                       EndpointInfo? knownEndpoint = null);
#pragma warning restore CA1055
    }

    /// <remarks>
    /// The endpoint registry is resolved lazily and treated as optional: without a host - an external api,
    /// a container built only for error handling - there is no EndpointDataSource, and the error output
    /// must still build. Such calls simply get no endpoint link.
    /// </remarks>
    internal sealed class EndpointSourceResolver(IServiceProvider serviceProvider,
                                                 ISourceLocationHelper sourceLocationHelper) : IEndpointSourceResolver
    {
        private readonly Lazy<IEndpointProvider?> _endpointProvider = new(() => TryGetEndpointProvider(serviceProvider));

#pragma warning disable CA1055
        public string Resolve(IHttpAssertContext context,
                              EndpointInfo? knownEndpoint = null)
#pragma warning restore CA1055
        {
            // Only enriches error output - it must never replace the failure it is decorating.
            try
            {
                var endpoint = knownEndpoint ?? _endpointProvider.Value?.FindEndpointFor(context.HttpMethod.Method, context.Url, context.ApiVersion);

                if (endpoint.IsNull() ||
                    endpoint.SourceLocation.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }

                return sourceLocationHelper.ToClickableUri(endpoint.SourceLocation, context.CallingAssembly);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return string.Empty;
            }
        }

        private static IEndpointProvider? TryGetEndpointProvider(IServiceProvider serviceProvider)
        {
            try
            {
                return serviceProvider.GetService<IEndpointProvider>();
            }
            catch (InvalidOperationException)
            {
                // No EndpointDataSource registered - there is no host.
                return null;
            }
        }
    }
}
