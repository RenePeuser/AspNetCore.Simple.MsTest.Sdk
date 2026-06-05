using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Api.NativeTypes.V1;

namespace MinimalApi.Api.NativeTypes
{
    internal static class Startup
    {
        internal static void AddNativeTypes(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddNativeTypesV1();
        }
    }
}