using Microsoft.AspNetCore.Mvc;
using MinimalApi.Extensionmethods;
using ApiVersion = Asp.Versioning.ApiVersion;

namespace MinimalApi.Api.ToDos.V1.GetAll
{
    public static class GetAllToDoEndpoints
    {
        public static RouteHandlerBuilder MapGetAll(this IEndpointRouteBuilder routeGroupBuilder)
        {
            return routeGroupBuilder.MapGet("todos", ([FromServices] GetAllToDosQuery getAllToDosQuery) =>
                                    {
                                        return Results.Ok(getAllToDosQuery.Execute());
                                    }).WithSummary("Get all ToDos")
                                    .Produces<IEnumerable<Todo>>(200)
                                    .Produces<ProblemDetails>(400)
                                    .Produces<ProblemDetails>(500)
                                    .WithName("getAllToDosV1")
                                    .WithSummary("Returns all available ToDos")
                                    .WithTags("ToDo")
                                    .WithDescriptionFromFile("V1.GetAll.Documentation.GetAllTodoDescription.txt")
                                    .WithDescription("Legacy version")
                                    .WithOpenApi()
                                    .MapToApiVersion(1)
                                    .MapToApiVersion(new ApiVersion(1.0));
        }
    }
}
