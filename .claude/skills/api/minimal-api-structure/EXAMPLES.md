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
                 .WithTags("Nodes")
                 .WithName("getAllNodesV1")
                 .MapToApiVersion(1)
                 .WithDescriptionFromFile("Description.md")
                 .WithSummaryFromFile("Summary.md");

        async Task<GetAllNodesResponse> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = await query.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            return new GetAllNodesResponse(result);
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
