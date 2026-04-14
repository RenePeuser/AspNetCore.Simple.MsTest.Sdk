using MinimalApi.Api.NativeTypes.V1.GetInt;
using MinimalApi.Api.NativeTypes.V1.GetString;

namespace MinimalApi.Api.NativeTypes.V1
{
    internal static class Startup
    {
        internal static void AddNativeTypesV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetString();
            serviceCollection.AddGetInt();
        }
    }
}
