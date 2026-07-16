# Fluent Assert API — How-To and Design Ideas

> ⚠️ **Alpha:** The fluent entry points are `public` only in prerelease builds (`FLUENT_ALPHA`).
> In stable packages, the API remains `internal` until its final shape has been established. Signatures may still
> change.
> For stable tests, continue using the classic `AssertPostAsync<T>(…)` API.
> For more details, see [`README.md`](README.md) and [`EXAMPLES.md`](EXAMPLES.md).

```bash
dotnet add package AspNetCore.Simple.MsTest.Sdk --version 9.6.0-alpha.19
```

## Purpose

The Fluent Assert API provides a declarative way to test HTTP endpoints. A test should resemble the endpoint contract it
verifies and make the request, expected response contract, and expected response content immediately visible.

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponse(expectedPerson)
    .ExecuteAsync();
```

The intended reading is:

> Assert that this POST endpoint accepts the supplied person, produces a `Person` with status `201 Created`, and returns
> the expected response.

## Core Concept

```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
```

- **Entry point:** `Client.AssertPost`, `AssertGet`, `AssertPut`, `AssertPatch`, or `AssertDelete`.
- **Request contract:** `Accepts…` describes the payload accepted by the endpoint.
- **Response contract:** `Produces…` describes the expected status code and optional response type.
- **Response content:** `ExpectedResponse…` defines the expected response body.
- **Terminal operation:** Every chain ends with exactly one `ExecuteAsync()` call.
- **Analyzer protection:** If the terminal call is missing, analyzer rule **MSTESTSDK001** reports an error and prevents
  a silently passing test.

## Why `Accepts` Is Better Than `WithBody`

Because this is an assertion API, the chain should describe the expected endpoint contract rather than merely the
technical construction of an HTTP request:

```csharp
var person = new Person("FirstName", "LastName");

await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExecuteAsync();
```

This reads naturally:

> The endpoint accepts this person and produces a `Person` with status `201 Created`.

`WithBody(person)` would be technically understandable, but it describes only how the HTTP request is assembled.
`Accepts(person)` expresses what the endpoint under test is expected to accept and therefore fits the declarative nature
of an assertion API better.

It also mirrors ASP.NET Core Minimal API metadata:

```csharp
// Endpoint contract
.Accepts<Person>(MediaTypeNames.Application.Json)
.Produces<Person>(StatusCodes.Status201Created)

// Test contract
.Accepts(person)
.Produces<Person>(HttpStatusCode.Created)
```

The chain therefore has three clearly separated responsibilities:

- `Accepts`: request contract and request payload
- `Produces`: response status and response type
- `ExpectedResponse`: expected response content

### Terminology Caveat

At the HTTP protocol level, the `Accept` request header describes the desired response media type, whereas the request
body itself is described by `Content-Type`. ASP.NET Core already uses `.Accepts<T>()` to describe accepted request-body
types and media types, however. Using `Accepts` is therefore consistent with Minimal API vocabulary and creates readable
symmetry with `Produces`.

The plural form `.Accepts(...)` is intentional. A method named `.Accept(...)` could easily be mistaken for configuration
of the HTTP `Accept` header.

---

## 1. POST with Golden-File Comparison

```csharp
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
    .ExecuteAsync();
```

The request object is serialized as JSON. The response must have status `201 Created`, must be deserializable as
`Person`, and must structurally match the embedded JSON resource. `ExecuteAsync()` returns the actual deserialized
`Person`, including server-generated values.

## 2. Explicit Request-Body Sources

Explicit methods avoid guessing whether a string represents JSON text or a resource name:

```csharp
.Accepts(person)
.AcceptsFromJsonString("{\"name\":\"Son\"}")
.AcceptsFromEmbeddedJson("CreatePersonFull.json")
```

Possible shorter names under consideration are:

```csharp
.Accepts(person)
.AcceptsJson("{\"name\":\"Son\"}")
.AcceptsEmbeddedJson("CreatePersonFull.json")
```

Implicit string interpretation is possible but not recommended as the primary API:

```csharp
.Accepts(person)
.Accepts("{\"name\":\"Son\"}")
.Accepts("CreatePersonFull.json")
```

The last two calls are both strings. Their meaning would have to be inferred from their contents, which can make
behavior surprising and refactoring less safe.

## 3. Expected-Response Sources

The current explicit naming family is:

```csharp
.ExpectedResponse(expected)
.ExpectedResponseFromJsonString(json)
.ExpectedResponseFromEmbeddedJson("CreatePerson.json")
```

Two shorter naming families are being considered:

```csharp
.Returns(expected)
.ReturnsJson(json)
.ReturnsEmbeddedJson("CreatePerson.json")
```

```csharp
.RespondsWith(expected)
.RespondsWithJson(json)
.RespondsWithEmbeddedJson("CreatePerson.json")
```

`Returns` is concise and reads naturally after `AssertPost`. `RespondsWith` is more explicitly HTTP-oriented.
`ExpectedResponse` is the most explicit assertion terminology but also the most verbose. These should remain
alternatives until one complete naming family is selected; mixing them in the same public API would add unnecessary
cognitive overhead.

If the shorter family is chosen, the grammatically correct form is `Returns`, not `Return`, because the endpoint is the
implicit subject of the chain.

## 4. GET with a Generic Collection Type

```csharp
var persons = await Client.AssertGet("api/v1/persons?name=Son")
    .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
    .ExpectedResponseFromEmbeddedJson("GetPersonByQuery.json")
    .ExecuteAsync();
```

The complete response is deserialized and compared as `IEnumerable<Person>`.

## 5. Ignore a Property in a Type-Safe Way

```csharp
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
    .IgnoreProperty<Person>(p => p.Id)
    .ExecuteAsync();
```

This excludes a dynamic property from comparison without relying on a magic string. Renaming `Person.Id` remains
refactoring-safe.

## 6. Ignore a Property Conditionally

Property selection and condition should be separate expressions:

```csharp
.IgnorePropertyWhen<Person, string?>(
    p => p.ParentConsent,
    p => p.Age >= 18)
```

The first expression unambiguously identifies the property to ignore. The second expression can inspect the complete
response object.

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponse(expectedPerson)
    .IgnorePropertyWhen<Person, string?>(
        p => p.ParentConsent,
        p => p.Age >= 18)
    .ExecuteAsync();
```

A single expression such as this is deliberately avoided:

```csharp
.IgnorePropertyWhen(p => p.Age < 18 && p.Country == "DE")
```

It does not reveal whether `Age`, `Country`, or both properties should be ignored.

## 7. Normalize the Response Before Comparison (`FilterResponse`)

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponse(expected)
    .FilterResponse(p => p is null
        ? null
        : p with
        {
            Emails = p.Emails
                .OrderBy(e => e.EmailAddress)
                .ToImmutableList()
        })
    .ExecuteAsync();
```

`FilterResponse` transforms the actual response before comparison. It is useful for deterministic ordering or
normalization while keeping the expected object readable.

## 8. Tolerate Specific Dynamic Differences (`DifferenceFilter`)

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponse(expected)
    .DifferenceFilter(d =>
        !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase))
    .ExecuteAsync();
```

Only differences rejected by the filter are ignored. Unrelated differences—for example an incorrect `Name`—still fail
the test.

For common cases, strongly typed helpers such as `IgnoreProperty` or `MatchesProperty` are preferable because their
intention is clearer.

## 9. Placeholder Parameters (`$Token$` Substitution)

```csharp
await Client.AssertPost("api/v1/persons")
    .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
    .WithParameters(new { Name = "Son", Age = 42 })
    .Produces<Person>(HttpStatusCode.Created)
    .ExecuteAsync();
```

Alternative parameter syntax:

```csharp
.WithParameter("Name", "Son")
.WithParameter("Age", 42)
```

```csharp
.WithParameters(
    ("Name", "Son"),
    ("Age", 42))
```

The values replace `$Name$` and `$Age$` placeholders in the referenced JSON resource.

## 10. Bodyless Path — Verify Status Only

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .WithHeader("X-Correlation-Id", "abc-123")
    .Produces(HttpStatusCode.Created)
    .ExecuteAsync();
```

The non-generic `Produces` overload verifies the status code without activating typed response-body comparison.

## 11. Typed Response Without a Golden File

```csharp
var result = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(StatusCodes.Status201Created)
    .ExecuteAsync();
```

The `int` overload supports ASP.NET Core status constants. The response is deserialized and returned without being
compared against an expected body.

## 12. Snapshot Mode — Update Expectations

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
    .WriteSnapshot()
    .ExecuteAsync();
```

`WriteSnapshot` writes the actual response as the new baseline. Because this changes a test artifact, its behavior
should be intentional and clearly visible in the test or centrally controlled through test configuration.

---

## Fluent API Idea Collection

The following table is a design backlog, not a promise that every method should become part of the public API. The
recommended approach is to keep the initial core small and add methods only when they remove recurring HTTP-test
boilerplate.

| Area                      | Code sample                                                                    | Intended behavior                                                                         | Priority / note                                                |
|---------------------------|--------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------|----------------------------------------------------------------|
| Object request            | `.Accepts(person)`                                                             | Serializes the object as the request body and uses JSON content by default.               | **Core**                                                       |
| Raw JSON request          | `.AcceptsJson(json)`                                                           | Sends the supplied JSON text as the request body without treating it as a resource name.  | **Core candidate**; shorter than `AcceptsFromJsonString`.      |
| Embedded JSON request     | `.AcceptsEmbeddedJson("CreatePerson.json")`                                    | Loads the request body from an embedded resource.                                         | **Core candidate**; shorter than `AcceptsFromEmbeddedJson`.    |
| Content type              | `.Accepts(person, MediaTypeNames.Application.Json)`                            | Sends the object using an explicit request content type.                                  | Useful when JSON is not the only supported format.             |
| Request header            | `.WithHeader("X-Tenant-Id", tenantId)`                                         | Adds a header to the outgoing request; it does not perform an assertion.                  | **Core**                                                       |
| Bearer token              | `.WithBearerToken(token)`                                                      | Adds an `Authorization: Bearer …` request header.                                         | Convenience extension.                                         |
| Query parameter           | `.WithQueryParameter("active", true)`                                          | Adds a URL-encoded query parameter.                                                       | Useful, although the URL can already contain the query.        |
| Typed response contract   | `.Produces<Person>(HttpStatusCode.Created)`                                    | Verifies status, expected response type, deserialization, and normally JSON content type. | **Core**; exact guarantees must be documented.                 |
| Status-only contract      | `.Produces(HttpStatusCode.NoContent)`                                          | Verifies the status without enabling typed body comparison.                               | **Core**                                                       |
| Problem response          | `.ProducesProblem(HttpStatusCode.BadRequest)`                                  | Verifies status and deserializes an RFC 7807 `ProblemDetails` body.                       | Strong HTTP-specific addition.                                 |
| Validation problem        | `.ProducesValidationProblem()`                                                 | Verifies a validation problem response, normally with status `400`.                       | Strong ASP.NET Core-specific addition.                         |
| Object response           | `.ExpectedResponse(expected)`                                                  | Compares the typed actual response with an expected object.                               | **Current core naming**                                        |
| Concise object response   | `.Returns(expected)`                                                           | Same behavior as `ExpectedResponse`, with more natural sentence flow.                     | Naming alternative; use `Returns`, not `Return`.               |
| HTTP-oriented response    | `.RespondsWith(expected)`                                                      | Same behavior as `ExpectedResponse`, emphasizing the HTTP response.                       | Naming alternative; slightly more verbose.                     |
| Raw JSON response         | `.ReturnsJson<Person>(json)`                                                   | Structurally compares the response with supplied JSON text.                               | Prefer explicit JSON naming over string guessing.              |
| Embedded JSON response    | `.ReturnsEmbeddedJson<Person>("CreatePerson.json")`                            | Loads expected JSON from an embedded resource and compares it structurally.               | Strong golden-file use case.                                   |
| Empty body                | `.ReturnsNoContent()`                                                          | Verifies that the response body is empty.                                                 | Define independently from status `204`.                        |
| Ignore one property       | `.IgnoreProperty<Person>(p => p.CreatedAt)`                                    | Excludes a strongly typed property from response comparison.                              | **Core**                                                       |
| Ignore several properties | `.IgnoreProperties<Person>(p => p.Id, p => p.CreatedAt)`                       | Excludes multiple properties from response comparison.                                    | Useful convenience addition.                                   |
| Conditional ignore        | `.IgnorePropertyWhen<Person, string?>(p => p.ParentConsent, p => p.Age >= 18)` | Ignores the selected property only when the condition on the complete response is true.   | Strong candidate; keep selector and condition separate.        |
| Nested property           | `.IgnoreProperty<Person>(p => p.Address.CreatedAt)`                            | Excludes a property inside a nested object.                                               | Should work naturally if member paths are supported.           |
| Collection item property  | `.IgnoreEach(p => p.Orders, o => o.CreatedAt)`                                 | Excludes a property for every item in a selected collection.                              | Later extension; expression implementation is more complex.    |
| Include only              | `.IncludingOnly(p => p.Name, p => p.Email)`                                    | Compares only selected properties and ignores every other member.                         | Useful for partial contracts; use carefully.                   |
| Predicate matcher         | `.MatchesProperty(p => p.Id, id => id != Guid.Empty)`                          | Validates a dynamic property with a predicate instead of a fixed expected value.          | High-value alternative to ignoring the property completely.    |
| Numeric tolerance         | `.WithTolerance(p => p.Price, 0.01m)`                                          | Allows a defined numeric difference for the selected property.                            | Useful for monetary and floating-point values.                 |
| Time tolerance            | `.WithTolerance(p => p.CreatedAt, TimeSpan.FromSeconds(2))`                    | Allows a defined temporal difference.                                                     | Common API-test requirement.                                   |
| Case-insensitive string   | `.IgnoreCase(p => p.Email)`                                                    | Compares a selected string property without casing differences.                           | Later comparison option.                                       |
| Normalize whitespace      | `.NormalizeWhitespace(p => p.Description)`                                     | Normalizes whitespace before comparing a selected string.                                 | Later comparison option.                                       |
| Ignore collection order   | `.IgnoreOrder(p => p.Tags)`                                                    | Compares collection elements independently of their order.                                | High-value for nondeterministic API results.                   |
| Compare collection by key | `.IgnoreOrderBy(p => p.Persons, p => p.Id)`                                    | Matches or orders collection items using a stable key before comparison.                  | Useful for complex collections.                                |
| Response header exists    | `.HasHeader("Location")`                                                       | Verifies that the response contains the specified header.                                 | **Core candidate**                                             |
| Response header value     | `.HasHeader("Location", expectedLocation)`                                     | Verifies header existence and value.                                                      | **Core candidate**                                             |
| Content type              | `.HasContentType(MediaTypeNames.Application.Json)`                             | Explicitly verifies the response content type.                                            | Possibly redundant when `Produces<T>` already guarantees it.   |
| Location header           | `.HasLocation($"persons/{person.Id}")`                                         | Specialized assertion for the response `Location` header.                                 | Useful convenience for `201 Created`.                          |
| Problem title             | `.HasProblemTitle("Validation failed")`                                        | Verifies `ProblemDetails.Title`.                                                          | Available only after a problem response is selected.           |
| Problem detail            | `.HasProblemDetail("Name is required")`                                        | Verifies `ProblemDetails.Detail`.                                                         | Available only after a problem response is selected.           |
| Validation error          | `.HasValidationError("name", "Name is required")`                              | Verifies a field-specific validation error.                                               | High-value ASP.NET Core helper.                                |
| Response filter           | `.FilterResponse(p => Normalize(p))`                                           | Transforms the actual response before structural comparison.                              | Existing advanced escape hatch.                                |
| Difference filter         | `.DifferenceFilter(d => KeepDifference(d))`                                    | Decides which comparison differences should remain relevant.                              | Existing low-level escape hatch.                               |
| Collection count          | `.HasCount(3)`                                                                 | Verifies the number of items in a typed response collection.                              | Avoid growing into a full general-purpose assertion library.   |
| Collection contains       | `.Contains(p => p.Id == expectedId)`                                           | Verifies that at least one response item satisfies the predicate.                         | Optional collection-focused extension.                         |
| All collection items      | `.AllSatisfy(p => p.Name is not null)`                                         | Applies a predicate to all response items.                                                | Optional; general assertion libraries may already cover this.  |
| Raw response callback     | `.AndAssert(response => Assert.IsTrue(...))`                                   | Provides an escape hatch over the raw `HttpResponseMessage`.                              | **Core candidate**                                             |
| Typed body callback       | `.AndAssertBody<Person>(p => Assert.AreNotEqual(Guid.Empty, p.Id))`            | Provides an escape hatch over the deserialized response body.                             | **Core candidate**                                             |
| Snapshot comparison       | `.MatchesSnapshot("CreatePerson")`                                             | Compares the response with a named stored snapshot.                                       | Optional if embedded JSON already covers the primary use case. |
| Snapshot update           | `.WriteSnapshot()`                                                             | Writes the actual response as the new baseline.                                           | Must be explicit and safely controlled.                        |
| Execution                 | `.ExecuteAsync(cancellationToken)`                                             | Sends the request, runs all configured assertions, and returns the typed actual response. | **Core terminal operation**                                    |

## Idea Samples

### Successful Create with Dynamic Properties

```csharp
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .Returns(expectedPerson)
    .IgnoreProperties<Person>(
        p => p.Id,
        p => p.CreatedAt)
    .HasLocation($"api/v1/persons/{person.Id}")
    .ExecuteAsync(cancellationToken);
```

### Conditional Property Comparison

```csharp
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .Returns(expectedPerson)
    .IgnorePropertyWhen<Person, string?>(
        p => p.ParentConsent,
        p => p.Age >= 18)
    .MatchesProperty(
        p => p.Id,
        id => id != Guid.Empty)
    .ExecuteAsync();
```

This is stronger than ignoring `Id`: the exact value is dynamic, but the test still verifies that the server generated a
valid ID.

### Validation Problem

```csharp
await Client.AssertPost("api/v1/persons")
    .Accepts(new Person(Name: "", Age: 42))
    .ProducesValidationProblem()
    .HasValidationError("name", "Name is required")
    .ExecuteAsync();
```

### Unordered Collection Response

```csharp
var persons = await Client.AssertGet("api/v1/persons")
    .Produces<IReadOnlyCollection<Person>>(HttpStatusCode.OK)
    .ReturnsEmbeddedJson<IReadOnlyCollection<Person>>("GetPersons.json")
    .IgnoreOrderBy(p => p, person => person.Id)
    .ExecuteAsync();
```

### Property Tolerances

```csharp
var invoice = await Client.AssertGet($"api/v1/invoices/{invoiceId}")
    .Produces<Invoice>(HttpStatusCode.OK)
    .Returns(expectedInvoice)
    .WithTolerance(p => p.Total, 0.01m)
    .WithTolerance(p => p.UpdatedAt, TimeSpan.FromSeconds(2))
    .ExecuteAsync();
```

### Raw Response Escape Hatch

```csharp
var person = await Client.AssertGet($"api/v1/persons/{personId}")
    .Produces<Person>(HttpStatusCode.OK)
    .AndAssert(response =>
    {
        Assert.IsTrue(response.Headers.Contains("X-Correlation-Id"));
    })
    .AndAssertBody<Person>(body =>
    {
        Assert.AreNotEqual(Guid.Empty, body.Id);
    })
    .ExecuteAsync();
```

---

## Recommended Initial Surface

The first public version should remain intentionally small:

```csharp
.Accepts(body)
.AcceptsJson(json)
.AcceptsEmbeddedJson(resource)

.WithHeader(name, value)
.WithParameter(name, value)
.WithParameters(values)

.Produces<T>(status)
.Produces(status)

.ExpectedResponse(expected)
.ExpectedResponseFromJsonString(json)
.ExpectedResponseFromEmbeddedJson(resource)

.IgnoreProperty(selector)
.IgnoreProperties(selectors)
.IgnorePropertyWhen(selector, condition)
.FilterResponse(filter)
.DifferenceFilter(filter)

.AndAssert(callback)
.AndAssertBody<T>(callback)

.ExecuteAsync(cancellationToken)
```

The initial metadata surface can remain equally focused:

```csharp
Client.AssertEndpoint(httpMethod, route)

.HasName(name)
.HasSummary(summary)
.HasDescription(description)
.HasTags(tags)

.Accepts<T>(contentTypes)
.Produces<T>(status, contentTypes)
.Produces(status)

.RequiresAuthorization(policy)
.AllowsAnonymous()
.IsIncludedInDescription()
.IsExcludedFromDescription()

.HasMetadata<T>()
.DoesNotHaveMetadata<T>()
.MetadataSatisfies<T>(predicate)
.AndAssertMetadata(callback)

.ExecuteAsync(cancellationToken)
```

Before the stable release, one response naming family should be selected consistently:

1. `ExpectedResponse…` — explicit assertion terminology
2. `Returns…` — shortest and most natural sentence flow
3. `RespondsWith…` — explicit HTTP terminology

The API should not expose all three as permanent aliases unless backward compatibility requires it.

## Naming Principles

- `Accepts…` describes the request body contract.
- `With…` configures request metadata or supplies template parameters.
- `Produces…` describes the response status and type contract.
- `ExpectedResponse…`, `Returns…`, or `RespondsWith…` defines expected response content.
- `Has…` asserts response metadata or specialized response members.
- `Ignore…`, `WithTolerance…`, and `Filter…` configure response comparison.
- `AndAssert…` provides an escape hatch without turning the SDK into a complete general-purpose assertion framework.
- `ExecuteAsync()` remains the single terminal operation and the only `await` point.

---

## Endpoint Metadata Assertions

The SDK can also verify the metadata registered for an endpoint. This complements runtime HTTP assertions: a runtime
test proves what the endpoint actually does, while a metadata test proves what ASP.NET Core, OpenAPI generators, API
explorers, authorization middleware, and other infrastructure can discover about it.

Given this endpoint:

```csharp
routeBuilder.MapPost("persons", (Person person) =>
    {
        if (string.IsNullOrWhiteSpace(person.Name))
        {
            return Results.BadRequest(new
            {
                StatusCode = 400,
                Message = "Invalid request"
            });
        }

        return Results.Created($"persons/{person.Id}", person);
    })
    .WithName("createPersonV1")
    .WithSummary("Creates a new person")
    .WithTags("Persons")
    .Accepts<Person>(MediaTypeNames.Application.Json)
    .Produces<Person>(StatusCodes.Status201Created)
    .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
    .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
    .MapToApiVersion(1);
```

a complete metadata assertion could look like this:

```csharp
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .HasName("createPersonV1")
    .HasSummary("Creates a new person")
    .HasTags("Persons")
    .Accepts<Person>(MediaTypeNames.Application.Json)
    .Produces<Person>(HttpStatusCode.Created)
    .Produces<ProblemDetails>(HttpStatusCode.BadRequest)
    .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
    .MapsToApiVersion(1)
    .ExecuteAsync();
```

`AssertEndpoint` resolves the registered `RouteEndpoint` through `EndpointDataSource`. It does not send an HTTP request.
`ExecuteAsync()` starts the test application if necessary, resolves one matching endpoint, and evaluates its metadata.

### Why a Separate Entry Point?

Runtime behavior and endpoint metadata are related but are not the same assertion:

```csharp
// Executes an HTTP request
await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(HttpStatusCode.Created)
    .Returns(person)
    .ExecuteAsync();

// Inspects the registered endpoint descriptor without sending a request
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .Accepts<Person>(MediaTypeNames.Application.Json)
    .Produces<Person>(HttpStatusCode.Created)
    .ExecuteAsync();
```

On the runtime chain, `.Accepts(person)` supplies a concrete request payload. On the metadata chain,
`.Accepts<Person>(contentType)` asserts `IAcceptsMetadata`. Keeping separate type-states makes the distinction visible
in IntelliSense and prevents ambiguous overloads.

### Metadata Assertion Ideas

| ASP.NET Core concern         | Fluent assertion                                                        | What it verifies                                                                                                             |
|------------------------------|-------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|
| Endpoint exists              | `.Exists()`                                                             | Exactly one endpoint matches the HTTP method and normalized route pattern. This can be implicit in every metadata assertion. |
| HTTP method                  | `.HasHttpMethod(HttpMethod.Post)`                                       | `HttpMethodMetadata` contains the expected HTTP method. Usually implied by `AssertEndpoint`.                                 |
| Route pattern                | `.HasRoutePattern("api/v1/persons")`                                    | The registered `RouteEndpoint.RoutePattern.RawText` matches the expected normalized pattern.                                 |
| Endpoint name                | `.HasName("createPersonV1")`                                            | `IEndpointNameMetadata.EndpointName` has the expected value.                                                                 |
| Display name                 | `.HasDisplayName("Create person")`                                      | `Endpoint.DisplayName` has the expected value.                                                                               |
| Summary                      | `.HasSummary("Creates a new person")`                                   | `IEndpointSummaryMetadata.Summary` has the expected value.                                                                   |
| Description                  | `.HasDescription("Creates and returns a person")`                       | `IEndpointDescriptionMetadata.Description` has the expected value.                                                           |
| Tags                         | `.HasTags("Persons")`                                                   | `ITagsMetadata.Tags` contains exactly the expected tags.                                                                     |
| Tags contain                 | `.ContainsTags("Persons", "Public API")`                                | The endpoint contains at least the specified tags while allowing additional tags.                                            |
| Accepted request             | `.Accepts<Person>(MediaTypeNames.Application.Json)`                     | `IAcceptsMetadata` declares `Person` and the expected request content type.                                                  |
| Optional request body        | `.Accepts<Person>(isOptional: true, MediaTypeNames.Application.Json)`   | The accepts metadata declares an optional request body.                                                                      |
| Produced response            | `.Produces<Person>(HttpStatusCode.Created)`                             | A matching `IProducesResponseTypeMetadata` entry declares status `201` and type `Person`.                                    |
| Produced content type        | `.Produces<Person>(HttpStatusCode.OK, MediaTypeNames.Application.Json)` | The produced response metadata also declares the expected content type.                                                      |
| Empty response               | `.Produces(HttpStatusCode.NoContent)`                                   | A produced-response entry declares the status without a response model.                                                      |
| All responses exactly        | `.ProducesExactly(expectedResponses)`                                   | The complete set of declared response status/type/content-type combinations matches, with no undocumented extras.            |
| OpenAPI inclusion            | `.IsIncludedInDescription()`                                            | The endpoint is visible to API Explorer and is not excluded from endpoint descriptions.                                      |
| OpenAPI exclusion            | `.IsExcludedFromDescription()`                                          | `IExcludeFromDescriptionMetadata.ExcludeFromDescription` is enabled.                                                         |
| Authorization required       | `.RequiresAuthorization()`                                              | The endpoint contains authorization metadata and is not anonymous.                                                           |
| Authorization policy         | `.RequiresAuthorization("PersonWriter")`                                | At least one `IAuthorizeData` item references the expected policy.                                                           |
| Roles                        | `.RequiresRoles("Admin", "PersonWriter")`                               | Authorization metadata contains the expected role requirements.                                                              |
| Authentication schemes       | `.UsesAuthenticationSchemes("Bearer")`                                  | Authorization metadata contains the expected authentication scheme.                                                          |
| Anonymous access             | `.AllowsAnonymous()`                                                    | `IAllowAnonymous` metadata is present.                                                                                       |
| CORS enabled                 | `.HasCorsPolicy("Frontend")`                                            | The endpoint contains CORS metadata for the expected policy.                                                                 |
| CORS disabled                | `.DisablesCors()`                                                       | Endpoint metadata explicitly disables CORS.                                                                                  |
| Rate limiting                | `.HasRateLimitPolicy("write-api")`                                      | Endpoint metadata enables the expected rate-limiter policy.                                                                  |
| Rate limiting disabled       | `.DisablesRateLimiting()`                                               | Endpoint metadata explicitly disables rate limiting.                                                                         |
| Antiforgery required         | `.RequiresAntiforgery()`                                                | Antiforgery metadata requires token validation.                                                                              |
| Antiforgery disabled         | `.DisablesAntiforgery()`                                                | Antiforgery validation is explicitly disabled for the endpoint.                                                              |
| Request timeout              | `.HasRequestTimeout(TimeSpan.FromSeconds(30))`                          | Request-timeout metadata contains the expected timeout or policy.                                                            |
| Output caching               | `.HasOutputCachePolicy("PersonById")`                                   | Output-cache metadata references the expected policy.                                                                        |
| API version                  | `.MapsToApiVersion(1)`                                                  | API-version metadata declares version 1. This comes from API Versioning integration, not ASP.NET Core itself.                |
| API version neutrality       | `.IsApiVersionNeutral()`                                                | The versioning integration marks the endpoint as version-neutral.                                                            |
| Arbitrary metadata present   | `.HasMetadata<MyMetadata>()`                                            | At least one metadata item of the requested type is registered.                                                              |
| Arbitrary metadata absent    | `.DoesNotHaveMetadata<MyMetadata>()`                                    | No metadata item of the requested type is registered.                                                                        |
| Arbitrary metadata predicate | `.MetadataSatisfies<MyMetadata>(m => m.Value == "expected")`            | Provides a typed escape hatch for Microsoft, third-party, or application-specific metadata.                                  |
| Complete metadata callback   | `.AndAssertMetadata(metadata => Assert.IsTrue(...))`                    | Provides access to `Endpoint.Metadata` for cases that do not justify a dedicated method.                                     |

Not every metadata type requires a dedicated fluent method. The generic `HasMetadata<T>`, `DoesNotHaveMetadata<T>`, and
`MetadataSatisfies<T>` methods make the API future-proof and support metadata introduced by later ASP.NET Core versions
or third-party libraries.

### Exact Versus Contains Semantics

Plural assertions should make their comparison semantics explicit:

```csharp
.HasTags("Persons")
```

means that the endpoint has exactly the specified tag set, whereas:

```csharp
.ContainsTags("Persons")
```

allows additional tags. The same principle can be applied to produced responses:

```csharp
// Verifies that this declaration exists; additional responses are allowed.
.Produces<Person>(HttpStatusCode.Created)

// Verifies the entire declared response contract and detects extra or missing entries.
.ProducesExactly(
    ResponseMetadata.For<Person>(HttpStatusCode.Created),
    ResponseMetadata.For<ProblemDetails>(HttpStatusCode.BadRequest),
    ResponseMetadata.For<ProblemDetails>(HttpStatusCode.InternalServerError))
```

### Authorization Metadata

```csharp
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .RequiresAuthorization("PersonWriter")
    .UsesAuthenticationSchemes("Bearer")
    .ExecuteAsync();
```

For an intentionally public endpoint:

```csharp
await Client.AssertEndpoint(HttpMethod.Get, "api/v1/persons/{id}")
    .AllowsAnonymous()
    .ExecuteAsync();
```

`RequiresAuthorization()` should fail when `IAllowAnonymous` is present, even if inherited group metadata also contains
`IAuthorizeData`, because the effective endpoint permits anonymous access.

### Generic Metadata Escape Hatch

```csharp
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .HasMetadata<IAcceptsMetadata>()
    .MetadataSatisfies<IAcceptsMetadata>(metadata =>
        metadata.RequestType == typeof(Person) &&
        metadata.ContentTypes.Contains(MediaTypeNames.Application.Json))
    .DoesNotHaveMetadata<IAllowAnonymous>()
    .ExecuteAsync();
```

Application-specific metadata works in exactly the same way:

```csharp
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .MetadataSatisfies<AuditMetadata>(metadata =>
        metadata.EventName == "PersonCreated")
    .ExecuteAsync();
```

### Route Groups and Effective Metadata

Metadata can be inherited from route groups:

```csharp
var api = app.MapGroup("api/v1")
    .WithTags("Public API")
    .RequireAuthorization("ApiUser");

api.MapPost("persons", CreatePerson)
    .WithName("createPersonV1")
    .WithTags("Persons");
```

Metadata assertions should inspect the final `Endpoint.Metadata` collection after conventions and route-group metadata
have been applied. This allows the test to validate the effective endpoint contract rather than only metadata declared
directly on `MapPost`.

```csharp
await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
    .ContainsTags("Public API", "Persons")
    .RequiresAuthorization("ApiUser")
    .ExecuteAsync();
```

### Metadata-Only Test for the Complete Example

```csharp
[TestMethod]
public async Task CreatePerson_endpoint_metadata_is_complete()
{
    await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
        .HasName("createPersonV1")
        .HasSummary("Creates a new person")
        .HasTags("Persons")
        .Accepts<Person>(MediaTypeNames.Application.Json)
        .ProducesExactly(
            ResponseMetadata.For<Person>(HttpStatusCode.Created),
            ResponseMetadata.For<ProblemDetails>(HttpStatusCode.BadRequest),
            ResponseMetadata.For<ProblemDetails>(HttpStatusCode.InternalServerError))
        .MapsToApiVersion(1)
        .IsIncludedInDescription()
        .ExecuteAsync();
}
```

### Combined Runtime and Metadata Contract

Keeping two focused tests usually produces the clearest failure messages:

```csharp
[TestMethod]
public async Task CreatePerson_returns_created_person()
{
    await Client.AssertPost("api/v1/persons")
        .Accepts(person)
        .Produces<Person>(HttpStatusCode.Created)
        .Returns(expectedPerson)
        .ExecuteAsync();
}

[TestMethod]
public async Task CreatePerson_metadata_describes_the_contract()
{
    await Client.AssertEndpoint(HttpMethod.Post, "api/v1/persons")
        .HasName("createPersonV1")
        .HasSummary("Creates a new person")
        .Accepts<Person>(MediaTypeNames.Application.Json)
        .Produces<Person>(HttpStatusCode.Created)
        .Produces<ProblemDetails>(HttpStatusCode.BadRequest)
        .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
        .MapsToApiVersion(1)
        .ExecuteAsync();
}
```

This separation makes failures immediately actionable: runtime failures indicate behavior or serialization problems;
metadata failures indicate documentation, discoverability, security declaration, or endpoint-registration problems.

## Two Things You Should Know

- **Type-state is real:** `IgnoreProperty`, `FilterResponse`, and `DifferenceFilter` appear in IntelliSense only after
  an `ExpectedResponse…` method. Without an expected body, there is nothing to configure, so invalid combinations cannot
  be expressed.
- **One terminal, always:** `.ExecuteAsync()` is the only `await` point. It returns the actual typed server
  response—including server-generated values such as an ID—not the expected-response template.

## Design Boundary

The Fluent Assert API should simplify HTTP contract assertions, response comparison, golden files, and common ASP.NET
Core response patterns. It should not attempt to replace MSTest or a complete object assertion library. Features should
be added where they make API tests materially more expressive, safer, or less repetitive.




