using Microsoft.AspNetCore.Mvc;

namespace MinimalApi.Api.ToDos.V1.Update
{
    internal static class UpdateToDoEndpoint
    {
        internal static RouteHandlerBuilder MapUpdateToDo(this IEndpointRouteBuilder routeGroupBuilder)
        {
            return routeGroupBuilder.MapPut("todos/{id}",
                    (int id, [FromBody] Todo todo, [FromServices] UpdateToDo updateToDo) =>
                    {
                        return Results.Ok((object?)updateToDo.Execute(id, todo));
                    }).WithSummary("Update ToDo by Id")
                .Produces<Todo>(200)
                .Produces<ProblemDetails>(400)
                .Produces<ProblemDetails>(500)
                .WithName("updateToDoByIdV1")
                .WithSummary("Provide a specific ToDo by its Id")
                .WithTags("ToDo")
                .WithDescription("Test Description")
                .WithOpenApi()
                .WithMetadata(new ObsoleteAttribute("This endpoint is obsolete. Use /new-endpoint instead."))
                .MapToApiVersion(1);
        }
    }
}
