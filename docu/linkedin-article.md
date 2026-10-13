# The Worst Test Is the One That Goes Green for the Wrong Reason

After years of building and testing ASP.NET Core APIs, I'm convinced of one thing: a failing test is not the
problem. A test that passes *without testing anything* is. It sits in your suite, glowing green, while the bug it
was written for ships to production.

**AspNetCore.Simple.MsTest.Sdk 10.1** is built around that idea. It ships a new fluent API for HTTP snapshot
tests — and almost every design decision in it answers one question: *how could this test go silently green, and
how do we make that impossible?*

## One chain that reads like your endpoint

A Minimal API endpoint *describes* a contract: what it accepts, what it produces. The test should *verify* that
same contract, line by line:

```csharp
var created = await Client.AssertPost("api/v1/persons")
    .Accepts(person)                                       // request body
    .Produces<Person>(StatusCodes.Status201Created)        // status + type + return type
    .ExpectedResponseFromEmbeddedJson("CreatePerson.json") // optional snapshot comparison
        .IgnoreProperty(p => p.Id)                         // server-generated value
    .ExecuteAsync();                                       // the only terminal

// `created` is the REAL server response - feed its Id into the next request.
```

`Accepts` / `Produces` / `ExpectedResponse` mirror the endpoint metadata. One style, one terminal, and the return
value is the actual deserialized response — so a full create → get → delete → 404 lifecycle is four short chains.

## The compiler as the first line of defense

The builders *guide* you. A method only shows up where it means something; a call that may only happen once
switches the stage. A wrong chain doesn't fail at runtime — it doesn't compile:

```csharp
Client.AssertGet(url).Accepts(body)            // CS1061 - GET carries no body
Client.AssertPost(url).Accepts(a).Accepts(b)   // CS1061 - the body is set once
```

Comparison settings like `IgnoreProperty` only exist *after* an expected response. Without something to compare,
you can't even write the option — instead of a silent no-op that looks like a check.

## A forgotten `ExecuteAsync()` is a build error

Every fluent API has the same trap: a chain that is configured but never executed. The builder isn't a `Task`, so
the compiler doesn't warn — and the test passes without sending a single request.

The package ships Roslyn analyzers for exactly this:

- **MSTESTSDK001** (error): a chain without `ExecuteAsync()`
- **MSTESTSDK002** (error): `ExecuteAsync()` that is never awaited — its failure would land on no test
- **MSTESTSDK003** (warning, Release builds only): a `WriteSnapshot()` left in the code, which would re-record the
  expected file on every Debug run and keep the test green whatever the API returns

Both errors come with a one-click code fix, including FixAll for migrating a whole project.

## Ignore exactly what you mean — nothing more

Ignoring fields is where snapshot tests quietly lose their teeth. "Ignore `id`" often means "ignore *every* `id`,
everywhere" — including the one that matters.

```csharp
.IgnoreProperty(p => p.Address.Street)                     // exactly this path
.IgnoreProperty(p => p.Emails.Select(e => e.EmailAddress)) // in every element
.IgnorePaths("emails[*].type")                             // difference path syntax
.IgnoreDifferencesWhere(d => d.MemberPath.EndsWith("At"))  // true = ignore
```

`IgnoreProperty(p => p.Name)` ignores the root `name` — not `address.name`. JSON names follow
`[JsonPropertyName]` and your naming policy.

## A typo in the URL is not a passing 404 test

Before the call, the SDK checks your endpoint metadata: does a route exist for this verb, URL and API version? And
does `Produces<T>` match the type the endpoint declares? A misspelled route is reported as *endpoint not found* —
not as a green "expected 404". When you really want to test a route that doesn't exist, you say so explicitly with
`.SkipEndpointValidation()`.

## The request is sent exactly as the test line reads

Some of the nastiest silent bugs live in the request itself. 10.1 closes them:

- Placeholder values are formatted culture-invariant — `1.5`, never `1,5` on a German build agent.
- URL values are encoded, so `Son Goku & Co` arrives as one value instead of breaking the query.
- A parameter that fills no placeholder fails instead of sending nothing.
- `WithQueryParameter("age", filter.Age)` skips `null`, so optional filters need no `if`.
- `WithBearerToken(token)` / `WithoutAuthorization()` apply to *this* request only — a shared client keeps its
  token, even with tests running in parallel.
- A header or parameter set twice fails at that line with *what* happened, *why* it's a problem and *how to fix
  it* — instead of one value silently winning.

## Store a stage, branch freely

Every builder stage is immutable. A configured base request can feed several tests without one branch leaking
into another:

```csharp
var request = Client.AssertGet("api/v1/persons?name=$Name$");

var sons    = request.WithParameter("Name", "Son").Produces<List<Person>>(HttpStatusCode.OK);
var vegetas = request.WithParameter("Name", "Vegeta").Produces<List<Person>>(HttpStatusCode.OK);
```

## Failures that tell you what to do

What made the SDK useful from the start is still there — and both APIs share it:

- **Deep diffs** down to the exact member path:

  ```
  | MemberPath         | Expected | Current | MismatchType    |
  | content.value.name | Son Test | Son     | ValueDifference |
  ```

- **A ready-to-run `curl`** command for every failure.
- **Suggested code** when your declared type doesn't match the endpoint.
- **Status, headers and body** in one snapshot.
- **Output modes** `Human`, `Ai` and `Hybrid`, so an AI assistant can read a failure as well as you can.

## Migrate test by test

The classic `AssertPostAsync<T>(…)` API stays. Both APIs run the same pipeline, so an existing suite can move one
test at a time — or not at all.

## Try it

```bash
dotnet add package AspNetCore.Simple.MsTest.Sdk
```

[NuGet](https://www.nuget.org/packages/AspNetCore.Simple.MsTest.Sdk) ·
[GitHub](https://github.com/RenePeuser/AspNetCore.Simple.MsTest.Sdk)

## How I built it

Every fix in this release was proven the same way: first a test that shows the gap (red), then the fix, then the
fix reverted to confirm the new tests really go red without it. Every positive test has a partner that *must* fail.
A safety net is only as good as the proof that it catches something.

---

## Key takeaways

✅ **One chain** that mirrors the endpoint: `Accepts` → `Produces<T>` → `ExpectedResponse` → `ExecuteAsync()`
✅ **Type-state builders**: wrong chains don't compile
✅ **Analyzers** turn a forgotten or unawaited `ExecuteAsync()` into a build error
✅ **Precise ignores**: exactly the path you name, nothing more
✅ **Endpoint validation**: a typo in the route is not a passing 404
✅ **Requests sent as written**: culture-invariant, encoded, no silently unused parameters
✅ **Immutable stages**: store and branch without side effects
✅ **Actionable failures**: deep diffs, curl, suggested fixes, AI-readable output

---

What's the most convincing *green* test you've ever seen that turned out to test nothing? I'd love to hear your
stories in the comments. 👇

---

**#dotnet #aspnetcore #testing #csharp #webapi #snapshottesting #roslyn #softwareengineering #cleancode**

---

*René Peuser - Software Architect | Passionate about developer productivity and clean architecture*
