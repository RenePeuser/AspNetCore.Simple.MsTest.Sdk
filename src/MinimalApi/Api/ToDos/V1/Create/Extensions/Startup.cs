namespace MinimalApi.Api.ToDos.V1.Create
{
    public static class Startup
    {
        internal static void AddNew(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAddNewToDo();
        }

        internal static void UseAdd(this IEndpointRouteBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapAddNewToDo();
        }
    }
}
