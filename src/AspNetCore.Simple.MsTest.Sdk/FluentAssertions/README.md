# Fluent Assert API

Type-safe fluent API for HTTP testing that mirrors ASP.NET Core endpoint definitions —
the endpoint *describes* the contract, the test *verifies* it.

Part of the regular package since 10.1, next to the classic `AssertPostAsync<T>(…)` overload API. Both run
the same pipeline, so a suite can migrate test by test.

---

## The one chain

One style (endpoint), one terminal (`ExecuteAsync()`). Type-state enforces the order:

```csharp
using AspNetCore.Simple.MsTest.Sdk;     // one namespace for the whole package
using Microsoft.AspNetCore.Http;   // StatusCodes

var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)                                       // request body (C# object)
    .Produces<Person>(StatusCodes.Status201Created)        // type + status + return type in ONE
    .ExpectedResponseFromEmbeddedJson("CreatePerson.json") // optional comparison template
        .IgnoreProperty(p => p.Id)                         // comparison config
    .ExecuteAsync();                                       // the only terminal → Task<Person>

// `created` is the REAL server response (with server-generated Id) — not the expected template.
```

**Rule of thumb:** *Every chain ends with exactly one `ExecuteAsync()`.*

## Building blocks

### Entry

`Client.AssertPost / AssertGet / AssertPut / AssertPatch / AssertDelete / AssertQuery(url)`

`AssertQuery` sends the HTTP `QUERY` verb (RFC 10008) — a safe request that carries a body, so the
`Accepts…` methods apply just like for POST. OPTIONS has its own short chain, see
[OPTIONS](#options--headers-are-the-payload).

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
.WithParameters(person)                    // all properties of an object (PascalCase), indexers skipped
.WithParameters(dictionary)                // entries of any dictionary (key = naked name)
.WithHeader("X-Correlation-Id", id)
```

`WithParameters("Goku")`, `WithParameters(42)` or `WithParameters(list)` throw `ArgumentException`: a value or a
collection has no properties that name placeholders.

### Query parameters — built from parts, the SDK encodes

```csharp
Client.AssertGet("api/v1/persons")
      .WithQueryParameter("name", "Son Goku & Co")   // → ?name=Son%20Goku%20%26%20Co
      .WithQueryParameter("age", filter.Age)         // null adds nothing - no if for optional filters
      .WithQueryParameter("tag", new[] { "a", "b" }) // → &tag=a&tag=b
```

Key and value are URL-encoded; a url that already has a query is continued with `&`. Values are formatted
culture-invariant (`1.5`, `true`, enums by name, `DateOnly` as `yyyy-MM-dd`, `DateTime`/`DateTimeOffset` ISO 8601
round-trip) - no `$Token$` substitution. A key given twice (case-insensitive, like the model binding) throws
`ArgumentException`; pass a collection for several values. An object value throws, too - it has no single query value.

### Credentials — per request, the shared client stays untouched

```csharp
Client.AssertGet("api/v1/admin/report").WithBearerToken(adminToken)   // Authorization: Bearer … for THIS call
Client.AssertGet("api/v1/admin/report").WithoutAuthorization()         // no credentials for THIS call → 401 test
```

The header goes on the request message, never on `HttpClient.DefaultRequestHeaders`. `HttpClient` only adds default
headers a request does not carry, so the per-request value wins for this call, and a client shared by the whole session
(one `[AssemblyInitialize]`, tests in parallel) keeps its own token - before, during and after the call, also when it
throws. There is nothing to save and restore. `WithoutAuthorization()` puts an empty `Authorization` header on the
message; the endpoint receives none. Credentials are set once per chain: `WithBearerToken` twice, together with
`WithoutAuthorization()` or with `WithHeader("Authorization", …)` throws `ArgumentException` - the message never prints
the token. A token for the whole session belongs on the client, in the setup.

### Endpoint validation

Before the call, the chain checks that the ROUTE exists (verb, url, api version). `Produces<T>(code)` also checks
`T` against the type the endpoint declares; `Produces(code)` names no type, so it checks the route only — a
misspelled url or a wrong verb is reported as "endpoint not found", not as a green 404 / 405. For outcomes the
endpoint metadata cannot describe (an error raised by middleware, a route that deliberately does not exist), opt
out — after `Produces…` (also after `ExpectedResponse…`), because it drops the response-type check, too:

```csharp
.SkipEndpointValidation()                  // classic: skipEndpointValidation: true
```

```csharp
await Client.AssertGet("api/v1/unknown-route")      // a deliberate 404 test
    .Produces(HttpStatusCode.NotFound)
    .SkipEndpointValidation()
    .ExecuteAsync();
```

### Status + type + return type

```csharp
.Produces<Person>(StatusCodes.Status201Created)  // body as Person → Task<Person>
.Produces<Person>(HttpStatusCode.OK)             // HttpStatusCode overload
.Produces(StatusCodes.Status204NoContent)        // no type named → status only, Task
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

### Anonymous (partial) expected object — the type comes from the object

When only a few properties matter, skip the type in `Produces` and let the expected object define it —
the fluent counterpart of the classic `AssertGetAsErrorAsync(url, new { … })`:

```csharp
var error = await Client.AssertGet("api/v1/persons/999")
    .Produces(StatusCodes.Status404NotFound)
    .ExpectedResponse(new { Title = "Person not found", Status = 404, Id = 999 })
    .ExecuteAsync();                                // → Task<anonymous type>, error.Id == 999
```

The response is deserialized into the anonymous type, so only its properties are compared. Endpoint
validation checks the route only here, because `Produces(code)` named no type to validate against. File and
raw-JSON sources carry no type, so they stay on `Produces<T>(code)`.

### Comparison config — only reachable after `ExpectedResponse…` (type-state!)

```csharp
.IgnoreProperty(p => p.Address.Street)                // exactly this position (full path)
.IgnoreProperty(p => p.Emails.Select(e => e.EmailAddress)) // [*] — in every element
.IgnoreProperty(list => list.Select(p => p.Id))       // collection response
.IgnorePaths("name", "emails[*].type")                // difference path syntax, [*] = any index
.IgnoreDifferencesWhere(d => d.MemberPath.EndsWith("At")) // predicate: true = IGNORE
.FilterDifferences(diffs => …)                        // escape hatch: returns the diffs that still COUNT
.FilterResponse(list => list.OrderBy(p => p.Id))      // normalize before comparison
.WriteSnapshot()                                      // rewrite the expected file (Debug only, MSTESTSDK003)
```

Every ignore rule is **precise**: `IgnoreProperty(p => p.Name)` ignores the root `name`, not
`address.name`. JSON names follow `[JsonPropertyName]` and the naming policy. Ignoring an object ignores
its subtree. Several rules combine — a difference is ignored as soon as one rule matches.

Paths: on the HTTP path the differences table shows the envelope path (`content.value.address.street`).
`IgnorePaths` accepts both that and the body-relative form (`address.street`); `IgnoreDifferencesWhere`
sees the raw `Difference.MemberPath`, i.e. the envelope path from the table.

### Terminal

`.ExecuteAsync()` → `Task<T>` (with body) or `Task` (body-less). The only `await` point.

Every terminal takes an optional `CancellationToken` (`.ExecuteAsync(TestContext.CancellationToken)`). It is passed
to the HTTP call; a cancellation surfaces as `OperationCanceledException`, never as an assertion failure.

### Every stage is immutable — store and branch freely

Each configuring call returns a NEW builder; the one you called it on stays unchanged. A stored stage can be the
base of several chains without one leaking into another:

```csharp
var request = Client.AssertGet("api/v1/persons?name=$Name$").WithHeader("X-Tenant-Id", "a");

var sons    = request.WithParameter("Name", "Son").Produces<List<Person>>(HttpStatusCode.OK);
var vegetas = request.WithParameter("Name", "Vegeta").Produces<List<Person>>(HttpStatusCode.OK);  // sends ONLY Name=Vegeta

var strict  = sons.ExpectedResponseFromEmbeddedJson("Sons.json");
var lenient = strict.IgnoreProperty(p => p.Select(x => x.Age));                                   // strict keeps comparing Age
```

The flip side: a configuring call whose result is discarded does nothing (`request.WithHeader(…);` on its own line
changes no chain) — always continue from the returned builder.

### OPTIONS — headers are the payload

An OPTIONS call has no body to compare; it must answer `204 NoContent` and its headers are checked:

```csharp
await Client.AssertOptions("api/v1/persons/echo-request")
    .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
        .Add("Allow", "POST, OPTIONS")
        .Add("Access-Control-Allow-Methods", "POST, OPTIONS")
        .Add("api-supported-versions", "1"))
    // or: .ExpectedHeadersFromJsonString("""{ "allow": [ "POST, OPTIONS" ], … }""")
    //     .ExpectedHeadersFromEmbeddedJson("Options.json")
    .ExecuteAsync();                                // → Task<HttpResponseMessage>
```

How the headers are compared (classic `AssertOptionsAsync` and fluent alike):

- **Response AND content headers.** `Allow` and every `Content-*` header are content headers in `HttpClient`;
  they are compared like any other header. Content headers are taken as sent (`Allow: POST, OPTIONS` is one value,
  not two).
- **Names are case-insensitive**, as in HTTP: `allow`, `Allow` and `ALLOW` all match. The dictionary overloads keep
  the names exactly as written (no JSON key policy). Values are compared exactly.
- **Exact set.** Every header the endpoint sends has to be expected, and every expected header has to be sent.
- **One name, one entry.** Listing a header twice in another spelling (`"Allow"` and `"allow"`) fails with
  "EXPECTED HEADER LISTED TWICE" — put several values into its array instead.

## Type-state (the compiler as the first line of defense)

The builders guide you: a method shows up only where it means something, and a call that may happen once becomes
a stage switch. A wrong chain does not compile — it never reaches the test run.

```
IHttpBodyRequestConfiguring              (AssertPost / AssertPut / AssertPatch / AssertQuery)
  ├─ Accepts…            → IHttpRequestConfiguring        (the body is set once - no second Accepts…)
  ├─ WithParameter… / WithQueryParameter / WithHeader / WithBearerToken / WithoutAuthorization /
  │  SkipEndpointValidation  → stays here (order is free)
  └─ Produces…           (a body verb without a body is fine)

IHttpRequestConfiguring                  (AssertGet / AssertDelete, or a body verb after Accepts…)
  ├─ WithParameter… / WithQueryParameter / WithHeader / WithBearerToken / WithoutAuthorization /
  │  SkipEndpointValidation  → stays here   (no Accepts… at all)
  ├─ Produces<T>(code)   → IHttpResponseConfiguring<T>
  └─ Produces(code)      → IHttpExpectationConfiguring   (no type named)

IHttpResponseConfiguring<T>
  ├─ ExpectedResponse…   → IHttpComparisonConfiguring<T>
  └─ ExecuteAsync()      → Task<T>   (body-less path)

IHttpComparisonConfiguring<T>
  ├─ IgnoreProperty / IgnoreDifferences / FilterResponse / WriteSnapshot → stays here
  └─ ExecuteAsync()      → Task<T>

IHttpExpectationConfiguring
  ├─ ExpectedResponse(obj) → IHttpComparisonConfiguring<TObj>   (type taken from the object)
  └─ ExecuteAsync()      → Task

IHttpOptionsConfiguring                  (Client.AssertOptions(url))
  └─ ExpectedHeaders…    → IHttpOptionsExpectationConfiguring
                              └─ ExecuteAsync() → Task<HttpResponseMessage>
```

What does NOT compile (proven by `MinimalApi.Test/FluentBuilders/FluentTypeStateCompileTests.cs`):

```csharp
Client.AssertGet(url).Accepts(body)                    // CS1061 - GET carries no body
Client.AssertDelete(url).AcceptsFromJsonString(json)   // CS1061 - DELETE carries no body
Client.AssertPost(url).Accepts(a).Accepts(b)           // CS1061 - the body is set once
Client.AssertPut(url).Accepts(a).WithHeader(…).Accepts(b)  // CS1061 - With… does not reopen the body
```

### Rules no type can check — they fail at the repeating line

A name is a runtime value, so the compiler cannot see it twice. These throw an `ArgumentException` with the sdk's
usual output (what happened, both values, why it fails, how to fix it, test file + line):

- **A parameter name twice** (`WithParameter("Name", "Son").WithParameter("Name", "Vegeta")`, also inside one
  `WithParameters` call, also `"Name"` vs. `"$Name$"`) — the placeholder is replaced once, so the second value was
  silently dropped.
- **A header name twice** (case-insensitive, `X-Id` = `x-id`) — the second value silently replaced the first. Send
  several values comma-separated.

Sibling branches of a stored stage may each set the same name — that is branching, not repeating. Overriding a
name the stored stage already holds is repeating: leave it out of the base and set it per branch.

Comparison rules compose instead: several `IgnoreProperty` / `IgnorePaths` / `IgnoreDifferencesWhere` /
`FilterDifferences` calls all apply, and several `FilterResponse` calls run in call order (the second gets the first's
output).

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

A third analyzer guards the recording switch:

| Id | Severity | Fires on |
|----|----------|----------|
| `MSTESTSDK003` | Warning, **Release builds only** | `WriteSnapshot()` / `WriteSnapshot(true)` left on a chain |

Recording is Debug-only: in Debug a committed `WriteSnapshot()` re-records the expected file on every run, so the
test stays green whatever the endpoint returns; in Release the SDK ignores it. The analyzer stays silent in Debug,
so recording and debugging are undisturbed, and catches the forgotten call in the Release build.
`WriteSnapshot(false)` and a non-constant argument are not reported. To update snapshots in bulk, set the
environment variable `TestSdkSettings__WriteResponse=true` instead of editing tests. Note: a pipeline that builds
and runs its tests in Debug only never sees this warning.

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
[`DESIGN_VISION.md`](DESIGN_VISION.md). Open work items and the handover list: [`NEXT_STEPS.md`](NEXT_STEPS.md).
