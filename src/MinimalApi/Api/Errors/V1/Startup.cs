using MinimalApi.Api.Errors.V1.NotImplemented;
using MinimalApi.Api.Errors.V1.NotImplemented.Endpoint;

namespace MinimalApi.Api.Errors.V1
{
    internal static class Startup
    {
        internal static void AddErrorsV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddNotImplementedEndpoint();
        }
    }
}
