using Microsoft.AspNetCore.Mvc;

namespace MinimalApi.Api.ToDos.V1.GetById
{
    internal static class GetToDoByIdEndpoint
    {
        internal static RouteHandlerBuilder MapGetById(this IEndpointRouteBuilder routeGroupBuilder)
        {
            return routeGroupBuilder.MapGet("todos/{id}", (int id, [FromServices] GetToDoById getToDoById) =>
                {
                    var result = getToDoById.Execute(id);

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                }).WithSummary("Get ToDo by Id")
                .Produces<Todo>(200)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(500)
                .WithName("getToDoByIdV1")
                .WithSummary("Provide a specific ToDo by its Id")
                .WithTags("ToDo")
                .WithDescription("Test Description")
                .WithOpenApi()
                .WithMetadata(new ObsoleteAttribute("This endpoint is obsolete. Use /new-endpoint instead."))
                .MapToApiVersion(1);
        }
    }
}
