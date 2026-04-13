namespace MinimalApi.Api.ToDos.V1.GetAll
{
    internal static class Startup
    {
        internal static void AddGetAll(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetAllToDosQuery();
            serviceCollection.AddToDoArraySerializerContext();
            serviceCollection.AddToDoEnumerableSerializerContext();
        }

        internal static void UseGetAll(this IEndpointRouteBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGetAll();
        }
    }
}
