using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.GetAll
{
    internal static class Startup
    {
        internal static void AddGetAll(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, GetAllPersonsEndpoint>();
        }
    }
}
