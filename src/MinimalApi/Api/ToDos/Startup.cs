using MinimalApi.Api.ToDos.V1;
using MinimalApi.Api.ToDos.V2;

namespace MinimalApi.Api.ToDos
{
    internal static class Startup
    {
        internal static void AddToDos(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddToDosV1();
            serviceCollection.AddToDosV2();
        }

        internal static void UseToDos(this IEndpointRouteBuilder application)
        {
            application.UseToDosV1();
            application.UseToDosV2();
        }
    }
}
