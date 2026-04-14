using StrategyPattern.Evolution;

namespace MinimalApi.Api.NativeTypes.V1.GetInt
{
    internal static class Startup
    {
        internal static void AddGetInt(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, GetIntEndpoint>();
        }
    }
}
