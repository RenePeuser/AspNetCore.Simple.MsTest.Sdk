# Fluent API Examples

Comprehensive examples demonstrating both neutral and endpoint-style APIs.

## Table of Contents
- [Basic Examples](#basic-examples)
- [Neutral Style Examples](#neutral-style-examples)
- [Endpoint Style Examples](#endpoint-style-examples)
- [Advanced Scenarios](#advanced-scenarios)
- [Side-by-Side Comparison](#side-by-side-comparison)

---

## Basic Examples

### Simple GET Request
```csharp
// Neutral style (classic)
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .ExpectSuccess();

// Neutral style (terminal - shorter!)
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json", expectSuccess: true);

// Endpoint style with status code (terminal)
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using Microsoft.AspNetCore.Http;

await Client.AssertGet("api/persons")
    .WithResponseType<List<Person>>()
    .Produces(StatusCodes.Status200OK, "Expected.json");
```

### Simple POST Request
```csharp
// Neutral style (classic)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json")
    .ExpectSuccess();

// Neutral style (terminal with status code)
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json", HttpStatusCode.Created);

// Endpoint style with status code (terminal)
await Client.AssertPost("api/persons")
    .Accepts(person)
    .WithResponseType<Person>()
    .Produces(StatusCodes.Status201Created, "Expected.json");
```

### DELETE with No Content
```csharp
// Both styles are identical for this case
await Client.AssertDelete($"api/persons/{id}")
    .ExpectNoContent();
```

---

## Neutral Style Examples

Clean, framework-agnostic API without endpoint-specific naming.

### Example 1: POST with Filtering
```csharp
var result = await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json")
    .FilterResponse(p => p with { Id = 0, CreatedDate = default })
    .ExpectSuccess();

// result contains the actual response
Assert.IsNotNull(result);
```

### Example 2: GET with Sorting
```csharp
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .FilterResponse(list => list.OrderBy(p => p.Id).ToList())
    .ExpectSuccess();
```

### Example 3: PUT with Difference Filtering
```csharp
await Client.AssertPut("api/persons/1")
    .WithBody(updatedPerson)
    .WithResponse<Person>("Expected.json")
    .IgnoreDifferences(diffs => diffs.Where(d => d.MemberPath != "UpdatedDate"))
    .ExpectSuccess();
```

### Example 4: Type-Safe Property Ignoring
```csharp
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .IgnoreProperty<Person>(p => p.Id)
    .IgnoreProperty<Person>(p => p.CreatedDate)
    .ExpectSuccess();
```

### Example 5: Parametrized JSON
```csharp
await Client.AssertPost("api/persons")
    .WithBody("Payload.json")
    .WithParameters(("$Name$", "Goku"), ("$Age$", 42))
    .WithResponse<Person>("Expected.json")
    .WithParameters(("$Id$", 0))
    .ExpectSuccess();
```

### Example 6: Multiple Accepted Status Codes
```csharp
await Client.AssertPost("api/persons")
    .WithBody(person)
    .WithResponse<Person>("Expected.json")
    .Expect(HttpStatusCode.OK, HttpStatusCode.Created);
```

### Example 7: Custom Headers
```csharp
await Client.AssertGet("api/persons")
    .WithHeader("X-Custom-Header", "value")
    .WithHeader("X-Request-Id", Guid.NewGuid().ToString())
    .WithResponse<List<Person>>("Expected.json")
    .ExpectSuccess();
```

### Example 8: Snapshot Writing
```csharp
// Write actual response to disk for updating expectations
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .WriteSnapshot(true)
    .ExpectSuccess();
```

### Example 9: Status Code Only (No Response Validation)
```csharp
// Just check status code, don't validate response body
await Client.AssertPost("api/persons")
    .WithBody(person)
    .ExpectSuccess();

// Or specific code
await Client.AssertPost("api/persons")
    .WithBody(person)
    .Expect(HttpStatusCode.Created);
```

---

## Endpoint Style Examples

Mirrors ASP.NET Core endpoint definitions for maximum symmetry.

### Example 1: Complete CRUD API Testing

#### Endpoint Definitions
```csharp
// CREATE
endpoints.MapPost("api/nodes", CreateNodeAsync)
    .Accepts<CreateNodeRequest>(MediaTypeNames.Application.Json)
    .Produces<NodeResponse>(StatusCodes.Status201Created)
    .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
    .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

// READ
endpoints.MapGet("api/nodes/{id}", GetNodeAsync)
    .Produces<NodeResponse>()
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

// UPDATE
endpoints.MapPut("api/nodes/{id}", UpdateNodeAsync)
    .Accepts<UpdateNodeRequest>(MediaTypeNames.Application.Json)
    .Produces<NodeResponse>()
    .Produces<ValidationProblemDetailsExtended>(StatusCodes.Status400BadRequest)
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

// DELETE
endpoints.MapDelete("api/nodes/{id}", DeleteNodeAsync)
    .Produces(StatusCodes.Status204NoContent)
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
```

#### Tests (Endpoint Style)
```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;

[TestClass]
public class NodeEndpointsTests : ApiTestBase
{
    // CREATE - Happy Path
    [TestMethod]
    public Task Should_Create_Node_Successfully()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(validRequest)
            .ProducesCreated<NodeResponse>("Created.json")
            .Expect(HttpStatusCode.Created);
    }

    // CREATE - Validation Error
    [TestMethod]
    public Task Should_Return_BadRequest_When_Request_Invalid()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(invalidRequest)
            .ProducesBadRequest<ValidationProblemDetailsExtended>("ValidationError.json")
            .ExpectError(HttpStatusCode.BadRequest);
    }

    // CREATE - Conflict
    [TestMethod]
    public Task Should_Return_Conflict_When_Node_Already_Exists()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(duplicateRequest)
            .ProducesConflict<ProblemDetails>("Conflict.json")
            .ExpectError(HttpStatusCode.Conflict);
    }

    // READ - Happy Path
    [TestMethod]
    public Task Should_Get_Node_By_Id()
    {
        return Client.AssertGet($"api/nodes/{existingId}")
            .ProducesOk<NodeResponse>("Node.json")
            .ExpectSuccess();
    }

    // READ - Not Found
    [TestMethod]
    public Task Should_Return_NotFound_When_Node_Does_Not_Exist()
    {
        return Client.AssertGet($"api/nodes/{nonExistentId}")
            .ProducesNotFound<ProblemDetails>("NotFound.json")
            .ExpectError(HttpStatusCode.NotFound);
    }

    // UPDATE - Happy Path
    [TestMethod]
    public Task Should_Update_Node_Successfully()
    {
        return Client.AssertPut($"api/nodes/{existingId}")
            .Accepts<UpdateNodeRequest>(updateRequest)
            .ProducesOk<NodeResponse>("Updated.json")
            .ExpectSuccess();
    }

    // DELETE - Happy Path
    [TestMethod]
    public Task Should_Delete_Node_Successfully()
    {
        return Client.AssertDelete($"api/nodes/{existingId}")
            .ExpectNoContent();
    }

    // DELETE - Not Found
    [TestMethod]
    public Task Should_Return_NotFound_When_Deleting_NonExistent_Node()
    {
        return Client.AssertDelete($"api/nodes/{nonExistentId}")
            .ProducesNotFound<ProblemDetails>("NotFound.json")
            .ExpectError(HttpStatusCode.NotFound);
    }
}
```

### Example 2: Error Handling Suite
```csharp
[TestClass]
public class ErrorHandlingTests : ApiTestBase
{
    [TestMethod]
    public Task Should_Handle_BadRequest()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(invalidRequest)
            .ProducesBadRequest<ValidationProblemDetailsExtended>("BadRequest.json")
            .ExpectError(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public Task Should_Handle_Unauthorized()
    {
        return Client.AssertGet("api/nodes")
            .ProducesUnauthorized<ProblemDetails>("Unauthorized.json")
            .ExpectError(HttpStatusCode.Unauthorized);
    }

    [TestMethod]
    public Task Should_Handle_Forbidden()
    {
        return Client.AssertGet("api/admin/nodes")
            .ProducesForbidden<ProblemDetails>("Forbidden.json")
            .ExpectError(HttpStatusCode.Forbidden);
    }

    [TestMethod]
    public Task Should_Handle_Conflict()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(duplicateRequest)
            .ProducesConflict<ProblemDetails>("Conflict.json")
            .ExpectError(HttpStatusCode.Conflict);
    }

    [TestMethod]
    public Task Should_Handle_UnprocessableEntity()
    {
        return Client.AssertPost("api/nodes")
            .Accepts<CreateNodeRequest>(semanticallyInvalidRequest)
            .ProducesUnprocessableEntity<ValidationProblemDetailsExtended>("UnprocessableEntity.json")
            .ExpectError(HttpStatusCode.UnprocessableEntity);
    }

    [TestMethod]
    public Task Should_Handle_InternalServerError()
    {
        return Client.AssertPost("api/nodes/trigger-error")
            .ProducesInternalServerError<ProblemDetails>("InternalError.json")
            .ExpectError(HttpStatusCode.InternalServerError);
    }
}
```

---

## Advanced Scenarios

### Scenario 1: Collection Filtering and Ordering
```csharp
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .FilterResponse(persons => persons
        .Where(p => p.Age > 18)
        .OrderBy(p => p.Name)
        .ThenBy(p => p.Age)
        .ToList())
    .IgnoreProperty<Person>(p => p.Id)
    .ExpectSuccess();
```

### Scenario 2: Nested Property Ignoring
```csharp
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .IgnoreDifferences(diffs => diffs.Where(d =>
        !d.MemberPath.StartsWith("Emails[") &&
        !d.MemberPath.EndsWith(".CreatedDate")))
    .ExpectSuccess();
```

### Scenario 3: Conditional Response Transformation
```csharp
await Client.AssertGet("api/persons")
    .WithResponse<List<Person>>("Expected.json")
    .FilterResponse(persons =>
    {
        if (persons == null) return null;
        
        return persons
            .Select(p => p with
            {
                Id = 0,
                CreatedDate = default,
                Emails = p.Emails.OrderBy(e => e.EmailAddress).ToList()
            })
            .OrderBy(p => p.Name)
            .ToList();
    })
    .ExpectSuccess();
```

### Scenario 4: Multi-Status with Response Body
```csharp
// Accept either 200 OK or 201 Created, both with response body
await Client.AssertPost("api/persons")
    .Accepts<Person>(person)
    .Produces<Person>("Expected.json")
    .Expect(HttpStatusCode.OK, HttpStatusCode.Created);
```

### Scenario 5: PATCH with Partial Updates
```csharp
await Client.AssertPatch($"api/persons/{id}")
    .WithBody(new { Name = "Updated Name" })
    .WithResponse<Person>("Expected.json")
    .IgnoreProperty<Person>(p => p.UpdatedDate)
    .ExpectSuccess();
```

---

## Side-by-Side Comparison

### Old Extension API vs New Fluent API

#### Scenario: POST with filtering and parameters

**Old API:**
```csharp
await Client.AssertPostAsync<Person>(
    url: "api/persons",
    payloadAsJson: personJson,
    expectedResult: "Expected.json",
    filterFunc: p => p with { Id = 0 },
    parameters: new[] { ("$Name$", "Goku"), ("$Age$", 42) },
    callingAssembly: Assembly.GetExecutingAssembly(),
    writeResponse: false,
    skipEndpointValidation: false,
    expectedStatusCode: HttpStatusCode.Created,
    callerFilePath: "",
    callerMemberName: "",
    callerLineNumber: 0);
```

**New Fluent API (Neutral):**
```csharp
await Client.AssertPost("api/persons")
    .WithBody(personJson)
    .WithParameters(("$Name$", "Goku"), ("$Age$", 42))
    .WithResponse<Person>("Expected.json")
    .FilterResponse(p => p with { Id = 0 })
    .Expect(HttpStatusCode.Created);
```

**New Fluent API (Endpoint Style):**
```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;

await Client.AssertPost("api/persons")
    .Accepts(personJson)
    .WithParameters(("$Name$", "Goku"), ("$Age$", 42))
    .ProducesCreated<Person>("Expected.json")
    .FilterResponse(p => p with { Id = 0 })
    .Expect(HttpStatusCode.Created);
```

#### Benefits:
- ✅ **Readable**: Reads left-to-right, top-to-bottom
- ✅ **Discoverable**: IntelliSense guides you through options
- ✅ **Type-safe**: Compiler enforces correct chain order
- ✅ **Flexible**: Only specify what you need
- ✅ **Consistent**: Same pattern for all HTTP methods
- ✅ **Symmetric**: Endpoint style mirrors your API definitions

---

## Best Practices

1. **Choose Your Style Consistently**
   - Use neutral style for framework-agnostic tests
   - Use endpoint style when symmetry with API definitions is valuable
   - Don't mix styles within the same test class

2. **Expect Last**
   - Always end your chain with an `Expect*` method
   - This makes it clear what outcome you're asserting

3. **Filter Before Asserting**
   - Apply all transformations (FilterResponse, IgnoreProperty) before the Expect call
   - The Expect call is terminal and executes the request

4. **Use Type-Safe Property Ignoring**
   - Prefer `IgnoreProperty<T>(p => p.PropertyName)` over string-based paths
   - Gets refactoring support and compile-time checking

5. **Leverage Parameters**
   - Use parameters for dynamic test data
   - Makes tests more maintainable and reusable
