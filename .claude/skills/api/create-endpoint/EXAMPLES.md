# Create Endpoint Examples

## Example 1: Cloud Capability POST Operation

Folder shape:
```
src/YourApi.Contracts/Api/Gcp/Capabilities/VertexAiModels/ApiKeys/Create/
├── Requests/
│   └── CreateVertexAiModelsApiKeyRequest.cs
└── Responses/
    └── CreateVertexAiModelsApiKeyResponse.cs

src/YourApi/Api/Gcp/Capabilities/VertexAiModels/V1/ApiKeys/Create/
├── Commands/
│   └── CreateVertexAiModelsApiKeyCommand.cs
├── Endpoints/
│   └── CreateVertexAiModelsApiKeyEndpoint.cs
├── Extensions/
│   └── CreateStartup.cs
├── Validations/
│   └── CreateVertexAiModelsApiKeyRequestValidator.cs
└── Documentations/
    ├── Summary.md
    └── Description.md
```

Why this is good:
- Contracts are kept in `YourApi.Contracts`
- The API operation lives under versioned API code
- The operation has explicit registration and documentation

---

## Example 2: Core Domain GET Operation

Folder shape:
```
src/YourApi.Contracts/Api/Projects/V1/GetCurrent/
└── Responses/
    └── GetCurrentProjectResponse.cs

src/YourApi/Api/Projects/V1/GetCurrent/
├── Endpoints/
│   └── GetCurrentEndpoint.cs
├── Extensions/
│   └── GetCurrentStartup.cs
└── Queries/
    └── GetCurrentProjectQuery.cs
```

Why this is good:
- Core domain contracts include `V1`
- GET uses a query instead of a command
- Registration stays explicit and local to the operation

---

## Example 3: GET Endpoint with Complete Metadata

```csharp
internal static class AddGetResourceByIdEndpointExtension
{
    internal static void AddGetResourceByIdEndpoint(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<IEndpoint, GetResourceByIdEndpoint>();
    }
}

internal sealed class GetResourceByIdEndpoint(GetResourceByIdQuery query) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("resources/{id:guid}", HandleAsync)
                 .Produces<GetResourceByIdResponse>()
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Resources")
                 .WithName("getResourceByIdV1")
                 .MapToApiVersion(1)
                 .WithSummaryFromFile("Summary.md")
                 .WithDescriptionFromFile("Description.md");

        return;

        async Task<GetResourceByIdResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await query.ExecuteAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

Why this is good:
- All possible status codes are documented
- OpenAPI will show complete response documentation
- Includes route constraints (`{id:guid}`)

---

## Example 4: POST Endpoint with Full Validation Responses

```csharp
internal static class AddCreateResourceEndpointExtension
{
    internal static void AddCreateResourceEndpoint(this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<IEndpoint, CreateResourceEndpoint>();
    }
}

internal sealed class CreateResourceEndpoint(CreateResourceCommand command) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("resources", HandleAsync)
                 .Accepts<CreateResourceRequest>(MediaTypeNames.Application.Json)
                 .Produces<CreateResourceResponse>(StatusCodes.Status201Created)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Resources")
                 .WithName("createResourceV1")
                 .MapToApiVersion(1)
                 .WithSummaryFromFile("Summary.md")
                 .WithDescriptionFromFile("Description.md");

        return;

        async Task<CreateResourceResponse> HandleAsync(CreateResourceRequest request, CancellationToken cancellationToken = default)
        {
            return await command.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

Why this is good:
- `.Accepts<>()` documents request body type
- 201 Created for successful resource creation
- Both 400 and 422 use `ValidationProblemDetailsExtended`
- Complete error response documentation

---

## Example 5: PUT Endpoint for Full Updates

```csharp
internal sealed class UpdateResourceEndpoint(UpdateResourceCommand command) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("resources/{id:guid}", HandleAsync)
                 .Accepts<UpdateResourceRequest>(MediaTypeNames.Application.Json)
                 .Produces<UpdateResourceResponse>()
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Resources")
                 .WithName("updateResourceV1")
                 .MapToApiVersion(1)
                 .WithSummaryFromFile("Summary.md")
                 .WithDescriptionFromFile("Description.md");

        return;

        async Task<UpdateResourceResponse> HandleAsync(Guid id, UpdateResourceRequest request, CancellationToken cancellationToken = default)
        {
            return await command.ExecuteAsync(id, request, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

---

## Example 6: PATCH Endpoint for Partial Updates

```csharp
internal sealed class PatchResourceEndpoint(PatchResourceCommand command) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("resources/{id:guid}", HandleAsync)
                 .Accepts<PatchResourceRequest>(MediaTypeNames.Application.Json)
                 .Produces<PatchResourceResponse>()
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                 .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Resources")
                 .WithName("patchResourceV1")
                 .MapToApiVersion(1)
                 .WithSummaryFromFile("Summary.md")
                 .WithDescriptionFromFile("Description.md");

        return;

        async Task<PatchResourceResponse> HandleAsync(Guid id, PatchResourceRequest request, CancellationToken cancellationToken = default)
        {
            return await command.ExecuteAsync(id, request, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

---

## Example 7: DELETE Endpoint with No Content Response

```csharp
internal sealed class DeleteResourceEndpoint(DeleteResourceCommand command) : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("resources/{id:guid}", HandleAsync)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
                 .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
                 .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                 .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                 .WithTags("Resources")
                 .WithName("deleteResourceV1")
                 .MapToApiVersion(1)
                 .WithSummaryFromFile("Summary.md")
                 .WithDescriptionFromFile("Description.md");

        return;

        async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await command.ExecuteAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }
}
```

Why this is good:
- DELETE returns 204 No Content
- No `.Accepts<>()` needed (no request body)
- `Task` instead of `Task<T>` for void return

---

## Example 8: Operation Startup

```csharp
internal static class CreateStartup
{
    internal static void AddCreate(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCreateVertexAiModelsApiKeyEndpoint();
        services.AddCreateVertexAiModelsApiKeyCommand(configuration);
    }
}
```

Why this is good:
- Operation startup coordinates registrations only
- The real DI details stay with the endpoint and command implementations
