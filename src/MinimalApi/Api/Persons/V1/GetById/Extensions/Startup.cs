using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.GetById
{
    internal static class Startup
    {
        internal static void AddGetById(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, GetPersonByIdEndpoint>();
        }
    }
}
