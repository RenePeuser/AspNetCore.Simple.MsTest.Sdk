---
name: minimal-api-structure
description: Use when creating, reviewing, or restructuring Minimal API endpoints and service registration in this .NET codebase.
---

# Minimal API Structure

Use this skill when working on Minimal API structure, endpoint registration, and folder organization.

## Objective

Keep Minimal APIs modular, self-contained, and easy to extend.

Use service-only registration, self-mapping endpoints, and action-focused horizontal slicing where this repository follows the new structure.

## Apply These Rules

- Register API domains through service collection extensions only.
- Do not require separate `MapXyz()` calls in startup when the solution uses the new structure.
- Let each endpoint register itself through DI and map itself through `IEndpoint`.
- Keep registration flow hierarchical: API → domain → version → action endpoint.
- Group actions by domain and API version.
- Keep each action self-contained with its endpoint, request/response models, and command/query logic nearby.
- Prefer horizontal slicing by action when the solution uses the new structure.
- Do not introduce horizontal slicing into a solution that still intentionally follows the older vertical slicing structure.
- Do not mix old and new structures within the same solution.
- If migration is not planned or not feasible, continue using the existing structure consistently.

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

## Avoid

- Dual registration with both `AddXyz()` and `MapXyz()` in the new structure
- Central endpoint mapping that must be manually updated for every action
- Mixing vertical slicing and horizontal slicing in the same solution
- Spreading one action across many unrelated folders
- Hiding endpoint mapping logic outside the endpoint implementation
- Forcing migration patterns into a solution that is intentionally staying on the old structure

## Review Checklist

- Does startup register domains through services only?
- Does each endpoint register itself as `IEndpoint`?
- Does each endpoint implement its own `Map(...)` logic?
- Is registration organized from domain to version to action?
- Is the action self-contained and easy to navigate?
- Does the solution consistently follow either the old or the new structure?
- Has the code avoided mixing vertical and horizontal slicing?

## Notes

The preferred new model is:

- service-only registration
- endpoint self-mapping through `IEndpoint`
- horizontal slicing by action within domain and version

However, consistency is more important than partial migration.
If a solution already uses the older vertical slicing approach and cannot be migrated, keep extending that existing structure instead of mixing models.
