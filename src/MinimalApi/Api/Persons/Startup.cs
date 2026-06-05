using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Api.Persons
{
    internal static class Startup
    {
        internal static void AddPersons(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddPersonsV1();
        }
    }
}