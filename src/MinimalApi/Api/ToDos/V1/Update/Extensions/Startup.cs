namespace MinimalApi.Api.ToDos.V1.Update
{
    public static class Startup
    {
        internal static void AddUpdate(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddUpdateToDo();
        }

        internal static void UseUpdate(this IEndpointRouteBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapUpdateToDo();
        }
    }
}
