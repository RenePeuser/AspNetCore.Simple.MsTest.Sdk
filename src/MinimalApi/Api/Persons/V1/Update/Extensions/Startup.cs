using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Update
{
    internal static class Startup
    {
        internal static void AddUpdate(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, UpdatePersonEndpoint>();
        }
    }
}
