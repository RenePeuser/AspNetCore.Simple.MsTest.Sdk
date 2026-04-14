using StrategyPattern.Evolution;

namespace MinimalApi.Api.Errors.V1.NotImplemented
{
    internal static class Startup
    {
        internal static void AddNotImplemented(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, NotImplementedEndpoint>();
        }
    }
}
