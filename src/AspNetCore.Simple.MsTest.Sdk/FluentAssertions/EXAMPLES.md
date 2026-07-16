# Fluent Assert API — Examples

> ## ⚠️ ALPHA — released in alpha versions only
>
> All examples here use the **experimental fluent API**, available **exclusively in alpha (prerelease)
> versions**. Signatures may change until the final shape is decided (see `DESIGN_VISION.md`). For stable
> test suites, use the classic `AssertPostAsync<T>(…)` overload API.

Every example follows the one chain:
`AssertX(url)` → `Accepts…` → `Produces<T>(code)` → `ExpectedResponse…` → `ExecuteAsync()`.

```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.AspNetCore.Http;   // StatusCodes
```

---

## Basics

### GET with body comparison
```csharp
await Client.AssertGet("api/persons")
    .Produces<List<Person>>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
    .ExecuteAsync();
```

### POST with object body
```csharp
var created = await Client.AssertPost("api/persons")
    .Accepts(person)
    .Produces<Person>(StatusCodes.Status201Created)
    .ExpectedResponseFromEmbeddedJson("Created.json")
    .ExecuteAsync();
```

### DELETE without body (204)
```csharp
await Client.AssertDelete($"api/persons/{id}")
    .Produces(StatusCodes.Status204NoContent)   // no <T> → no ExpectedResponse allowed
    .ExecuteAsync();
```

---

## Body sources (Schema A)

```csharp
// C# object
.Accepts(person)

// raw JSON, verbatim
.AcceptsFromJsonString("{ \"name\": \"Goku\", \"age\": 42 }")

// embedded-resource file
.AcceptsFromEmbeddedJson("CreatePersonFull.json")
```

---

## Expected sources (mirror the request side)

```csharp
// C# object
.ExpectedResponse(expectedPerson)

// raw JSON, verbatim
.ExpectedResponseFromJsonString("{ … }")

// embedded-resource file
.ExpectedResponseFromEmbeddedJson("Expected.json")
```

### No expected = body-less path
```csharp
// Assert the status only, still get the real response back typed — no golden file.
var person = await Client.AssertGet($"api/persons/{id}")
    .Produces<Person>(StatusCodes.Status200OK)
    .ExecuteAsync();

Assert.IsNotNull(person);
```

---

## Comparison configuration (only after `ExpectedResponse…`)

### Type-safe property ignore
```csharp
await Client.AssertGet("api/persons")
    .Produces<List<Person>>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
        .IgnoreProperty<Person>(p => p.Id)
    .ExecuteAsync();
```

### Free difference filtering
```csharp
await Client.AssertPut("api/persons/1")
    .AcceptsFromEmbeddedJson("Update.json")
    .Produces<Person>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
        .IgnoreDifferences(diffs => diffs.Where(d => d.MemberPath != "UpdatedDate"))
    .ExecuteAsync();
```

### Normalize the response before comparison
```csharp
await Client.AssertGet("api/persons")
    .Produces<List<Person>>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
        .FilterResponse(list => list.OrderBy(p => p.Id).ToList())
    .ExecuteAsync();
```

### Write a snapshot (update the expected file)
```csharp
await Client.AssertGet("api/persons")
    .Produces<List<Person>>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
        .WriteSnapshot()
    .ExecuteAsync();
```

---

## Placeholder parameters

Naked names — the SDK escapes internally (`"Id"` becomes `$Id$`). Already-escaped names (`"$Id$"`) are
accepted too.

```csharp
// single
await Client.AssertPost("api/persons")
    .AcceptsFromEmbeddedJson("Payload.json")
    .WithParameter("Name", "Goku")
    .WithParameter("Age", 42)
    .Produces<Person>(StatusCodes.Status201Created)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
    .ExecuteAsync();

// as tuples
.WithParameters(("Name", "Goku"), ("Age", 42))

// from a whole object (each property → placeholder, PascalCase)
.WithParameters(person)
```

---

## Custom headers
```csharp
await Client.AssertGet("api/persons")
    .WithHeader("X-Custom-Header", "value")
    .WithHeader("X-Request-Id", Guid.NewGuid().ToString())
    .Produces<List<Person>>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
    .ExecuteAsync();
```

---

## Error outcomes (own body type, same verb)

Every status code has its own body type — `Produces<T>(code)` is generic:

```csharp
// 400 with a validation-error type
await Client.AssertPost("api/persons")
    .Accepts(invalidPerson)
    .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
    .ExpectedResponseFromEmbeddedJson("ValidationError.json")
    .ExecuteAsync();

// 404 with ProblemDetails
await Client.AssertGet($"api/persons/{nonExistentId}")
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
    .ExecuteAsync();
```

> **One `Produces` per chain:** each test call asserts exactly ONE outcome. An endpoint with several
> `Produces` declarations (201, 400, 409, …) gets a corresponding number of **tests** — one per outcome.
> That keeps the return type of `ExecuteAsync()` unambiguous.

---

## CRUD lifecycle (the core story)

The real, deserialized response flows into the next request:

```csharp
[TestMethod]
public async Task Person_Lifecycle()
{
    var person = TestHelpers.CreateValidPerson();

    // CREATE → real Id back
    var created = await Client.AssertPost("api/v1/persons")
        .Accepts(person)
        .Produces<Person>(StatusCodes.Status201Created)
        .ExecuteAsync();

    // READ(id) with comparison
    await Client.AssertGet($"api/v1/persons/{created.Id}")
        .Produces<Person>(StatusCodes.Status200OK)
        .ExpectedResponseFromEmbeddedJson("GetPerson.json")
            .IgnoreProperty<Person>(p => p.Id)
        .ExecuteAsync();

    // DELETE(id) → no body
    await Client.AssertDelete($"api/v1/persons/{created.Id}")
        .Produces(StatusCodes.Status204NoContent)
        .ExecuteAsync();

    // READ(id) → 404
    await Client.AssertGet($"api/v1/persons/{created.Id}")
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .ExecuteAsync();
}
```

---

## Migration from the overload API

**Before (overload):**
```csharp
await Client.AssertPostAsync<Person>(
    url: "api/persons",
    payloadAsJson: personJson,
    expectedResult: "Expected.json",
    parameters: new[] { ("$Id$", 0) },
    expectedStatusCode: HttpStatusCode.Created);
```

**After (fluent, alpha):**
```csharp
await Client.AssertPost("api/persons")
    .AcceptsFromJsonString(personJson)
    .WithParameter("Id", 0)
    .Produces<Person>(StatusCodes.Status201Created)
    .ExpectedResponseFromEmbeddedJson("Expected.json")
    .ExecuteAsync();
```

> `Produces<T>(code)` requires the **exact** status code (no "any 2xx"). This is intentional: a contract
> test names the code the endpoint promises in its spec. When migrating, watch whether your endpoint
> returns `200 OK` or `201 Created` — the old `ExpectSuccess()` masked that difference.
