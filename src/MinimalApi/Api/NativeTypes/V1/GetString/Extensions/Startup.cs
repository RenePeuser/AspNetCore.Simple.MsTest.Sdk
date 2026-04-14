using StrategyPattern.Evolution;

namespace MinimalApi.Api.NativeTypes.V1.GetString
{
    internal static class Startup
    {
        internal static void AddGetString(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, GetStringEndpoint>();
        }
    }
}
