namespace MinimalApi.Api.ToDos.V1.GetById
{
    public static class Startup
    {
        internal static void AddGetById(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddGetToDoById();
        }

        internal static void UseGetById(this IEndpointRouteBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGetById();
        }
    }
}
