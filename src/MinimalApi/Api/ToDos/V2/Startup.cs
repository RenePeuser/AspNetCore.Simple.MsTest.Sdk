using MinimalApi.Api.ToDos.V2.GetAll;

namespace MinimalApi.Api.ToDos.V2
{
    internal static class Startup
    {
        internal static void AddToDosV2(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetAll();
        }

        internal static void UseToDosV2(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            //var v2ToDoGroup = endpointRouteBuilder.MapGroup("todos")
            //                                      .WithTags("ToDo")
            //                                      .MapToApiVersion(2);

            var v2ToDoGroup = endpointRouteBuilder;

            v2ToDoGroup.UseGetAll();
        }
    }
}
