# Fluent Assert API

Type-safe fluent API for HTTP testing that mirrors ASP.NET Core endpoint definitions —
the endpoint *describes* the contract, the test *verifies* it.

> ## ⚠️ ALPHA — released in alpha versions only
>
> This fluent API is **experimental** and shipped **exclusively in alpha (prerelease) versions** of the
> package. Until its final shape is decided (see `DESIGN_VISION.md`), we deliberately **do not release it
> in stable versions** — signatures may change without notice.
>
> **Do not use it in production test suites** that run against stable releases. Feedback on the API is
> explicitly welcome — that is exactly what the alpha is for.
>
> For stable tests, keep using the classic `AssertPostAsync<T>(…)` overload API.

---

## The one chain

One style (endpoint), one terminal (`ExecuteAsync()`). Type-state enforces the order:

```csharp
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.AspNetCore.Http;   // StatusCodes

var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)                                       // request body (C# object)
    .Produces<Person>(StatusCodes.Status201Created)        // type + status + return type in ONE
    .ExpectedResponseFromEmbeddedJson("CreatePerson.json") // optional comparison template
        .IgnoreProperty<Person>(p => p.Id)                 // comparison config
    .ExecuteAsync();                                       // the only terminal → Task<Person>

// `created` is the REAL server response (with server-generated Id) — not the expected template.
```

**Rule of thumb:** *Every chain ends with exactly one `ExecuteAsync()`.*

## Building blocks

### Entry

`Client.AssertPost / AssertGet / AssertPut / AssertPatch / AssertDelete(url)`

### Request body — explicit, no heuristic

```csharp
.Accepts(person)                          // C# object (serialized)
.AcceptsFromJsonString("{ \"name\": … }") // raw JSON, verbatim
.AcceptsFromEmbeddedJson("Create.json")   // embedded-resource file
```

### Placeholder parameters — naked names, the SDK escapes internally

```csharp
.WithParameter("Id", 0)                    // → internally $Id$; "$Id$" is also accepted
.WithParameters(("Name", "Goku"), ("Age", 42))
.WithParameters(person)                    // all properties of an object (PascalCase)
.WithHeader("X-Correlation-Id", id)
```

### Status + type + return type

```csharp
.Produces<Person>(StatusCodes.Status201Created)  // body as Person → Task<Person>
.Produces<Person>(HttpStatusCode.OK)             // HttpStatusCode overload
.Produces(StatusCodes.Status204NoContent)        // no body → Task; no ExpectedResponse allowed
```

The body type is generic — error outcomes carry their own type:

```csharp
.Produces<ProblemDetails>(StatusCodes.Status404NotFound)
.Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
```

### Expected body (optional) — mirrors the request side (Schema A)

```csharp
.ExpectedResponse(personObject)                    // C# object
.ExpectedResponseFromJsonString("{ … }")           // raw JSON
.ExpectedResponseFromEmbeddedJson("Expected.json") // embedded-resource file
```

**Omitting it = body-less path:** assert the status only, still get the real response back typed.

### Comparison config — only reachable after `ExpectedResponse…` (type-state!)

```csharp
.IgnoreProperty<Person>(p => p.Id)               // type-safe hard skip
.IgnoreDifferences(diffs => diffs.Where(…))      // free difference filtering
.FilterResponse(list => list.OrderBy(p => p.Id)) // normalize before comparison
.WriteSnapshot()                                 // rewrite the expected file
```

### Terminal

`.ExecuteAsync()` → `Task<T>` (with body) or `Task` (body-less). The only `await` point.

## Type-state (the compiler as the first line of defense)

```
IHttpRequestConfiguring
  ├─ Accepts… / WithParameter… / WithHeader  → stays here
  ├─ Produces<T>(code)   → IHttpResponseConfiguring<T>
  └─ Produces(code)      → IHttpExpectationConfiguring   (no body, no ExpectedResponse)

IHttpResponseConfiguring<T>
  ├─ ExpectedResponse…   → IHttpComparisonConfiguring<T>
  └─ ExecuteAsync()      → Task<T>   (body-less path)

IHttpComparisonConfiguring<T>
  ├─ IgnoreProperty / IgnoreDifferences / FilterResponse / WriteSnapshot → stays here
  └─ ExecuteAsync()      → Task<T>

IHttpExpectationConfiguring
  └─ ExecuteAsync()      → Task
```

`IgnoreProperty` deliberately lives on `IHttpComparisonConfiguring<T>` — without an `ExpectedResponse…`
there is no comparison to configure, so an `IgnoreProperty` without an expected body is a **compile
error**, not a silent runtime no-op.

## Why a forgotten terminal cannot slip through

The builder interfaces are marked with `[FluentBuilder]`, and two Roslyn analyzers ship with the package
— both **errors**, because for a test SDK a silently green test is the worst failure mode:

| Id | Fires on | Why the compiler misses it |
|----|----------|----------------------------|
| `MSTESTSDK001` | Chain without `ExecuteAsync()` | The builder is not a `Task`, so there is not even a CS4014 |
| `MSTESTSDK002` | `ExecuteAsync()` present but never awaited/returned | CS4014 only fires inside an `async` method — in a synchronous test method nothing warns at all |

```csharp
// MSTESTSDK001 — request never sent
Client.AssertPost("api/persons").Accepts(person).Produces(Created);

// MSTESTSDK002 — assertion runs detached, its failure lands on no test
Client.AssertPost("api/persons").Accepts(person).Produces(Created).ExecuteAsync();
```

Both come with a one-click code fix (`await` + `.ExecuteAsync()`, `void` → `async Task` where needed),
with FixAll for whole-file/project migration. `return …ExecuteAsync();` and `var t = …ExecuteAsync();`
are consuming forms and are never flagged. Details in `DESIGN_VISION.md`.

## Example: CRUD lifecycle

The real deserialized response flows into the next request:

```csharp
// create → real Id back
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)
    .Produces<Person>(StatusCodes.Status201Created)
    .ExecuteAsync();

// get(id) with comparison
await Client.AssertGet($"api/v1/persons/{created.Id}")
    .Produces<Person>(StatusCodes.Status200OK)
    .ExpectedResponseFromEmbeddedJson("GetPerson.json")
    .ExecuteAsync();

// delete(id) → no body
await Client.AssertDelete($"api/v1/persons/{created.Id}")
    .Produces(StatusCodes.Status204NoContent)
    .ExecuteAsync();

// get(id) → 404 with its own error type
await Client.AssertGet($"api/v1/persons/{created.Id}")
    .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
    .ExecuteAsync();
```

More examples in [`EXAMPLES.md`](EXAMPLES.md). A task-oriented walkthrough plus the full method idea
collection lives in [`HOWTO.md`](HOWTO.md). Design background and open questions in
[`DESIGN_VISION.md`](DESIGN_VISION.md).
