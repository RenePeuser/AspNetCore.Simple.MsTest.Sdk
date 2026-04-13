using MinimalApi.Api.ToDos.V1.Create;
using MinimalApi.Api.ToDos.V1.DeleteById;
using MinimalApi.Api.ToDos.V1.GetAll;
using MinimalApi.Api.ToDos.V1.GetById;
using MinimalApi.Api.ToDos.V1.Update;

namespace MinimalApi.Api.ToDos.V1
{
    internal static class Startup
    {
        internal static void AddToDosV1(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetAll();
            serviceCollection.AddGetById();
            serviceCollection.AddAddNewToDo();
            serviceCollection.AddUpdateToDo();
            serviceCollection.AddDeleteById();
        }

        internal static void UseToDosV1(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            //var v1TodoGroup = endpointRouteBuilder.MapGroup("todos")
            //                                      .WithTags("ToDo")
            //                                      .MapToApiVersion(1);

            var v1TodoGroup = endpointRouteBuilder;

            v1TodoGroup.UseGetAll();
            v1TodoGroup.UseGetById();
            v1TodoGroup.UseAdd();
            v1TodoGroup.UseUpdate();
            v1TodoGroup.UseDeleteById();
        }
    }
}
