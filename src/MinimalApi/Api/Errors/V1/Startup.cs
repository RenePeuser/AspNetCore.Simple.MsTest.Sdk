using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Api.Errors.V1.NotImplemented.Endpoint;
using MinimalApi.Api.Errors.V1.QueryNotImplemented.Endpoint;

namespace MinimalApi.Api.Errors.V1
{
    internal static class Startup
    {
        internal static void AddErrorsV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddNotImplementedEndpoint();
            serviceCollection.AddQueryNotImplementedEndpoint();
        }
    }
}