namespace MinimalApi.Api.ToDos.V1.DeleteById
{
    public static class Startup
    {
        internal static void AddDeleteById(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDeleteToDoById();
        }

        internal static void UseDeleteById(this IEndpointRouteBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapDeleteById();
        }
    }
}
