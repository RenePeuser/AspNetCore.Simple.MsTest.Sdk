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
            serviceCollection.AddDeletePersonEndpoint();
            serviceCollection.AddDeletePersonWithResponseEndpoint();
        }
    }
}