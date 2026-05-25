# FluentAssertions API

Modern, type-safe fluent API for HTTP testing that reads like natural language.

## Architecture

```
FluentAssertions/
├── Interfaces/           # Core fluent interfaces (state machine)
│   ├── IHttpRequestConfiguring.cs
│   ├── IHttpResponseConfiguring.cs
│   └── IHttpStatusAssertable.cs
├── Builders/            # Implementation of fluent interfaces
│   ├── HttpRequestBuilder.cs
│   ├── HttpResponseBuilder.cs
│   └── HttpStatusOnlyBuilder.cs
├── Extensions/          # Entry points (AssertPost, AssertGet, etc.)
│   └── HttpClientFluentExtensions.cs
└── EndpointStyle/       # Opt-in endpoint-symmetric extensions
    └── EndpointStyleExtensions.cs
```

## Usage

### Neutral Style (Core API)

Clean, framework-agnostic API:

```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;

// Simple POST with response validation (classic)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json")
    .ExpectSuccess();

// Simple POST with response validation (terminal - shorter!)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json", expectSuccess: true);

// With explicit status code (terminal)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json", HttpStatusCode.Created);

// Multiple accepted status codes (terminal)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json", HttpStatusCode.OK, HttpStatusCode.Created);

// With filtering and transformation (classic style required)
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .FilterResponse(list => list.OrderBy(p => p.Id).ToList())
    .IgnoreProperty<Person>(p => p.CreatedDate)
    .ExpectSuccess();

// Status code only (no response body check)
await Client.AssertDelete($"api/persons/{id}")
    .ExpectNoContent();
```

### Endpoint Style (Opt-In)

Mirrors ASP.NET Core endpoint definitions for maximum symmetry:

```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using Microsoft.AspNetCore.Http;

// Happy path - mirrors endpoint definition with status code
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(request)
    .WithResponseType<CreateNodeResponse>()
    .Produces(StatusCodes.Status201Created, "Expected.json");

// With filtering
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(request)
    .WithResponseType<CreateNodeResponse>()
    .FilterResponse(r => r with { Id = 0 })
    .Produces(StatusCodes.Status201Created, "Expected.json");

// Validation error
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(invalidRequest)
    .WithResponseType<ValidationProblemDetails>()
    .Produces(StatusCodes.Status400BadRequest, "Error.json");

// Or using classic style with separate Expect
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(invalidRequest)
    .Produces<ValidationProblemDetails>("Error.json")
    .ExpectError(HttpStatusCode.BadRequest);
```

## State Machine

The fluent API enforces correct usage through a type-state pattern:

```
IHttpRequestConfiguring (Initial State)
  ├─ WithBody() → stays in IHttpRequestConfiguring
  ├─ WithParameters() → stays in IHttpRequestConfiguring
  ├─ WithHeader() → stays in IHttpRequestConfiguring
  ├─ WithResponse<T>() → transitions to IHttpResponseConfiguring<T>
  ├─ ExpectSuccess() → terminal (returns IHttpStatusAssertable)
  ├─ Expect(codes) → terminal (returns IHttpStatusAssertable)
  └─ ExpectNoContent() → terminal (returns IHttpStatusAssertable)

IHttpResponseConfiguring<T> (Response Config State)
  ├─ FilterResponse() → stays in IHttpResponseConfiguring<T>
  ├─ IgnoreDifferences() → stays in IHttpResponseConfiguring<T>
  ├─ IgnoreProperty() → stays in IHttpResponseConfiguring<T>
  ├─ WithParameters() → stays in IHttpResponseConfiguring<T>
  ├─ WriteSnapshot() → stays in IHttpResponseConfiguring<T>
  ├─ ExpectSuccess() → terminal (returns Task<T>)
  ├─ Expect(codes) → terminal (returns Task<T>)
  ├─ ExpectStatus(code) → terminal (returns Task<T>)
  └─ ExpectError(code) → terminal (returns Task<T>)

IHttpStatusAssertable (Terminal State)
  ├─ ExecuteAsync() → returns Task
  └─ GetAwaiter() → enables direct await
```

## Design Principles

1. **Terminal Flexibility**: Methods can be terminal (WithResponse with status code) or non-terminal (classic with .Expect()) depending on your needs
2. **Type Safety**: Compiler enforces correct chain order through interfaces
3. **No Overload Explosion**: Fluent API avoids the combinatorial explosion of extension method overloads
4. **Opt-In Styles**: Core API is neutral; endpoint-symmetric style requires explicit namespace import
5. **Discoverable**: IntelliSense guides you through the available options at each step
6. **Backward Compatible**: All existing tests continue to work - new terminal overloads are additive

## Examples

### Complete Endpoint Test Suite

```csharp
// Endpoint definition
endpoints.MapPost("nodes", HandleAsync)
    .Accepts<CreateNodeRequest>(MediaTypeNames.Application.Json)
    .Produces<CreateNodeResponse>()
    .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
    .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
    .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

// Test: Happy Path
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(validRequest)
    .Produces<CreateNodeResponse>("HappyPath.json")
    .ExpectSuccess();

// Test: Validation Error
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(invalidRequest)
    .ProducesBadRequest<ValidationProblemDetailsExtended>("ValidationError.json")
    .ExpectError(HttpStatusCode.BadRequest);

// Test: Unauthorized
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(request)
    .ProducesUnauthorized<ProblemDetails>("Unauthorized.json")
    .ExpectError(HttpStatusCode.Unauthorized);

// Test: Conflict
await Client.AssertPost("nodes")
    .Accepts<CreateNodeRequest>(duplicateRequest)
    .ProducesConflict<ProblemDetails>("Conflict.json")
    .ExpectError(HttpStatusCode.Conflict);
```

## Comparison: Old vs New API

### Before (Extension Methods)
```csharp
await Client.AssertPostAsync<Person>(
    url: "api/persons",
    payloadAsJson: personJson,
    expectedResult: "Expected.json",
    parameters: new[] { ("$Id$", 0) },
    writeResponse: false,
    skipEndpointValidation: false,
    expectedStatusCode: HttpStatusCode.Created);
```

### After (Fluent API - Neutral Style)
```csharp
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithParameters(("$Id$", 0))
    .WithResponse<Person>("Expected.json")
    .Expect(HttpStatusCode.Created);
```

### After (Fluent API - Endpoint Style with StatusCode)
```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using Microsoft.AspNetCore.Http;

await Client.AssertPost("api/persons")
    .Accepts(person)
    .WithParameters(("$Id$", 0))
    .WithResponseType<Person>()
    .Produces(StatusCodes.Status201Created, "Expected.json");
```

## Migration Path

The fluent API is **fully compatible** with existing extension methods. Both APIs call the same underlying `AssertHttpCallAsync` implementation. You can:

1. Use both APIs side-by-side
2. Gradually migrate tests to the fluent API
3. Choose the API that best fits your team's style

No breaking changes required!
