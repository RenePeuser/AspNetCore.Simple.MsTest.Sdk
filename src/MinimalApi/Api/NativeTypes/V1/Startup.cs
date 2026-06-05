using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Api.NativeTypes.V1
{
    internal static class Startup
    {
        internal static void AddNativeTypesV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetStringEndpoint();
            serviceCollection.AddGetIntEndpoint();
            serviceCollection.AddGetAnotherStringEndpoint();
            serviceCollection.AddGetAnotherIntEndpoint();
        }
    }
}