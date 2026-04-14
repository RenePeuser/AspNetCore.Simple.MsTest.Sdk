using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Create
{
    internal static class Startup
    {
        internal static void AddCreate(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, CreatePersonEndpoint>();
        }
    }
}
