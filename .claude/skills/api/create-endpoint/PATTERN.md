# Create Endpoint Pattern

## Core Goal

Create endpoints that match the repository's Minimal API structure, contract placement, and DI registration flow.

## Path Patterns

### Cloud capability API

API code:
```
src/YourApi/Api/[Cloud]/Capabilities/[Domain]/V[Version]/[SubResource?]/[Operation]/
```

Contracts:
```
src/YourApi.Contracts/Api/[Cloud]/Capabilities/[Domain]/[SubResource?]/[Operation]/
```

Important:
- Cloud capability contracts omit the version folder
- Operation folders contain the endpoint-specific components

### Core domain API

API code:
```
src/YourApi/Api/[Domain]/V[Version]/[Operation]/
```

Contracts:
```
src/YourApi.Contracts/Api/[Domain]/V[Version]/[Operation]/
```

## Standard Components

- `Endpoints/` - `IEndpoint` implementation and endpoint DI extension in the same file
- `Commands/` - write operation handler and DI extension in the same file
- `Queries/` - read operation handler and DI extension in the same file
- `Validations/` - request validation when needed
- `Mappers/` - explicit mapping logic when needed
- `Documentations/` - `Summary.md` and `Description.md`
- `Sql/` - SQL files when persistence uses embedded SQL
- `Extensions/` - operation startup registration only

## HTTP Method to Operation Shape

- `POST` → command-based create flow
- `GET` → query-based read flow
- `PUT` → command-based full update flow
- `PATCH` → command-based partial update flow
- `DELETE` → command-based delete flow

## DTO Rules

### Requests

- Must live in `src/YourApi.Contracts`
- Use `public sealed record`
- Use `required` members where needed
- Use existing validation attributes and sample values
- Use immutable collections instead of mutable lists

### Responses

- Must live in `src/YourApi.Contracts`
- Use `public sealed record` for simple wrappers
- Use `public class` when property attributes such as `[SampleValue]` are needed

## Command and Query Rules

- Use `internal sealed class`
- Keep one public execution method: `ExecuteAsync(..., CancellationToken cancellationToken)`
- Put the registration extension in the same file as the command or query
- Register dependencies before registering the command or query itself
- If the user did not ask for business logic, scaffold with `throw new NotImplementedException()`

## Endpoint Rules

- Endpoint class implements `IEndpoint`
- Use a local `HandleAsync` function inside `Map`
- Put the endpoint registration extension in the same file as the endpoint
- Use the correct `MapGet`, `MapPost`, `MapPut`, `MapPatch`, or `MapDelete`
- **Only for operations with request body** (POST, PUT, PATCH), add `.Accepts<TRequest>(MediaTypeNames.Application.Json)`
- Do **not** use `.Accepts<>()` for GET or DELETE operations
- Add **all relevant** `.Produces<T>()` responses with explicit status codes
- Use `.WithTags(...)`, `.WithName(...)`, and `.MapToApiVersion(...)`
- Load documentation from `Summary.md` and `Description.md`

### Complete Produces Metadata

Always include complete OpenAPI metadata for all possible responses:

**Success Response:**
- `.Produces<TResponse>()` or `.Produces<TResponse>(StatusCodes.Status200OK)`
- `.Produces(StatusCodes.Status204NoContent)` for DELETE operations

**Problem Details Responses:**
- `.Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)` - transport/format validation
- `.Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)` - missing authentication
- `.Produces<ProblemDetails>(StatusCodes.Status403Forbidden)` - insufficient permissions
- `.Produces<ProblemDetails>(StatusCodes.Status404NotFound)` - resource not found
- `.Produces<ProblemDetails>(StatusCodes.Status409Conflict)` - duplicate or conflict
- `.Produces<ValidationProblemDetailsExtended>(StatusCodes.Status422UnprocessableEntity)` - business rule violation
- `.Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)` - unexpected failure
- `.Produces<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)` - temporary unavailability

Include only the status codes your endpoint can actually return. This metadata is critical for:
- OpenAPI/Swagger documentation accuracy
- Client SDK generation
- API contract testing
- Developer understanding

## Registration Flow

Registration is hierarchical and explicit.

Typical flow:
1. `Program.cs` calls API registration
2. `Api/Startup.cs` calls domain or cloud provider registration
3. Domain/provider `Startup.cs` calls version registration
4. Version `Startup.cs` calls operation registration
5. Operation startup registers endpoint plus command/query

Rule:
- Operation-level `Startup.cs` should orchestrate registrations only
- It should not duplicate the DI extension implementations already defined in endpoint and command/query files

## Naming Rules

- Endpoint: `[Operation][Domain]Endpoint`
- Command: `[Operation][Domain]Command`
- Query: `Get[Domain]...Query`
- Request: `[Operation][Domain]Request`
- Response: `[Operation][Domain]Response`
- Validator: `[Operation][Domain]RequestValidator`
- Startup: `[Operation]Startup`

## OpenAPI Rules

- Add `Summary.md` and `Description.md` for the operation
- Use sample values on contracts where appropriate
- Keep tags and operation names aligned with existing domain conventions
- If sample value providers are needed, place them under `src/YourApi/Api/<Domain>/V1/<Action>/SampleValueProviders/` and keep provider plus DI extension in the same file

## Verification Order

1. Does the folder structure match cloud vs core placement?
2. Are request/response DTOs placed in `YourApi.Contracts`?
3. Is the operation shape correct for the HTTP method?
4. Are DI extensions colocated with endpoint and command/query?
5. Is the operation wired explicitly in the correct `Startup.cs`?
6. Are documentation files present?
7. Does the OpenAPI test at least start successfully after registration changes?
