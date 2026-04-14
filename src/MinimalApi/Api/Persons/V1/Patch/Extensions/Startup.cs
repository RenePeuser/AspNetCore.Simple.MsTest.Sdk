using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Patch
{
    internal static class Startup
    {
        internal static void AddPatch(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEndpoint, PatchPersonEndpoint>();
        }
    }
}
