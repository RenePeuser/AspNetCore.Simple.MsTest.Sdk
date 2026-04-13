# Minimal API Structure Examples

## Preferred Registration Flow

```csharp
webApi.RegisterServices = (services, config) =>
{
    services.AddApi(config);
};
```

```csharp
internal static class Startup
{
    internal static void AddToDos(this IServiceCollection services)
    {
        services.AddToDosV1();
        services.AddToDosV2();
    }
}
```

```csharp
internal static class Startup
{
    internal static void AddToDosV1(this IServiceCollection services)
    {
        services.AddGetAllEndpoint();
        services.AddGetByIdEndpoint();
        services.AddCreateEndpoint();
        services.AddUpdateEndpoint();
        services.AddDeleteByIdEndpoint();
    }
}
```

## Preferred Endpoint Shape

```csharp
internal static class AddGetAllNodesEndpointExtension
{
    internal static void AddGetAllNodesEndpoint(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<IEndpoint, GetAllNodesEndpoint>();
    }
}

internal sealed class GetAllNodesEndpoint(GetAllNodesQuery query) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("nodes", HandleAsync)
                 .Produces<GetAllNodesResponse>()
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Nodes")
                 .WithName("getAllNodesV1")
                 .MapToApiVersion(1)
                 .WithDescriptionFromFile("Description.md")
                 .WithSummaryFromFile("Summary.md");

        return;

        async Task<GetAllNodesResponse> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            return new GetAllNodesResponse(result);
        }
    }
}
```

## POST Endpoint with Complete Metadata

```csharp
internal static class AddCreateNodeEndpointExtension
{
    internal static void AddCreateNodeEndpoint(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<IEndpoint, CreateNodeEndpoint>();
    }
}

internal sealed class CreateNodeEndpoint(CreateNodeCommand command) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("nodes", HandleAsync)
                 .Accepts<CreateNodeRequest>(MediaTypeNames.Application.Json)
                 .Produces<CreateNodeResponse>(StatusCodes.Status201Created)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Nodes")
                 .WithName("createNodeV1")
                 .MapToApiVersion(1)
                 .WithDescriptionFromFile("Description.md")
                 .WithSummaryFromFile("Summary.md");

        return;

        async Task<CreateNodeResponse> HandleAsync(CreateNodeRequest request, CancellationToken cancellationToken = default)
        {
            return await command.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

## Preferred Structure

```text
Api/
└── User/
    └── V1/
        ├── Create/
        │   ├── Endpoints/
        │   ├── Models/
        │   └── Commands/
        ├── GetAll/
        │   ├── Endpoints/
        │   └── Queries/
        ├── GetById/
        │   ├── Endpoints/
        │   └── Queries/
        └── DeleteById/
            ├── Endpoints/
            └── Commands/
```
