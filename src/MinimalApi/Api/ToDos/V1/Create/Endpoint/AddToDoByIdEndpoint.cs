using Microsoft.AspNetCore.Mvc;

namespace MinimalApi.Api.ToDos.V1.Create
{
    /// <summary>
    ///     Provides an endpoint for adding new ToDo items.
    /// </summary>
    internal static class AddNewToDoEndpoint
    {
        /// <summary>
        ///     Maps the "Add New ToDo" endpoint to the specified route group builder.
        /// </summary>
        /// <param name="routeGroupBuilder">The route group builder to which the endpoint is added.</param>
        /// <returns>A <see cref="RouteHandlerBuilder" /> for further configuration.</returns>
        internal static RouteHandlerBuilder MapAddNewToDo(this IEndpointRouteBuilder routeGroupBuilder)
        {
            return routeGroupBuilder.MapPost("todos", async ([FromBody] Todo todo,
                                                             [FromServices] AddNewToDoCommand addNewToDo) =>
                                                      {
                                                          // 1. Execute domain command to add new todo
                                                          var addedTodo = await addNewToDo.ExecuteAsync(todo).ConfigureAwait(false);

                                                          // 2. Return ok result with created todo
                                                          return Results.Ok(addedTodo);
                                                      })
                                    .WithSummary("Add new todo")
                                    .Produces<Todo>() // Specifies that the endpoint produces a Todo object.
                                    .Produces<ProblemDetails>(400) // Specifies that the endpoint produces a 400 status code with ProblemDetails.
                                    .Produces<ProblemDetails>(500) // Specifies that the endpoint produces a 500 status code with ProblemDetails.
                                    .WithName("addToDoV1") // Assigns a name to the endpoint.
                                    .WithSummary("Provide a specific ToDo by its Id") // Adds a summary for the endpoint.
                                    .WithTags("ToDo") // Tags the endpoint for grouping in API documentation.
                                    .WithDescription("Test Description") // Adds a description for the endpoint.
                                    .WithOpenApi() // Enables OpenAPI documentation for the endpoint.
                                    .WithMetadata(new ObsoleteAttribute("This endpoint is obsolete. Use /new-endpoint instead.")) // Marks the endpoint as obsolete.
                                    .MapToApiVersion(1); // Maps the endpoint to API version 1.
        }
    }
}
