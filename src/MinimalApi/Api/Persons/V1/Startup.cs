using MinimalApi.Api.Persons.V1.Create;
using MinimalApi.Api.Persons.V1.GetAll;
using MinimalApi.Api.Persons.V1.GetById;
using MinimalApi.Api.Persons.V1.Patch;
using MinimalApi.Api.Persons.V1.Update;

namespace MinimalApi.Api.Persons.V1
{
    internal static class Startup
    {
        internal static void AddPersonsV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetAllPersonsEndpoint();
            serviceCollection.AddGetPersonByIdEndpoint();
            serviceCollection.AddCreatePersonEndpoint();
            serviceCollection.AddUpdatePersonEndpoint();
            serviceCollection.AddPatchPersonEndpoint();
        }
    }
}
