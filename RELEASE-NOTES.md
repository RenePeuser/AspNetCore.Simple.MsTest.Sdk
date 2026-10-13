# Release Notes

## Upcoming Release (10.1.0) - Fluent Assert API, one public entry point

Commit with `+semver: minor`. This release deliberately ships breaking changes to the public surface of 10.0.0 in
a minor version: 10.x was already the breaking step from 9.x and is barely adopted yet, so the cleanup lands in
10.1 instead of forcing an 11.0.0. The breaking entries below say what to change.

### Breaking: `AddAssertableHttpClient` is the only public registration

Configure everything through the one registration call - from `registerServices` of `ApiTestBase<T>`
or in your own host:

```csharp
services.AddAssertableHttpClient(configuration, settings =>
{
    settings.DifferenceFunc = GlobalDifferenceFunc;
    settings.WriteResponse = false;
});
```

| Removed from the public API (10.0.0) | Replacement |
|---|---|
| `HttpClientAssertExtensions.Setup(provider)` | nothing - the started host's provider is handed over automatically, for `ApiTestBase<T>` and your own `WebApplicationFactory` alike |
| `HttpClientAssertExtensions.Setup(settings => ...)` | `AddAssertableHttpClient(configuration, settings => ...)`; without any host the environment (`TestSdkSettings__OutputMode`, ...) configures the SDK |
| `services.AddTestSdkSettings(...)` | `AddAssertableHttpClient(configuration, settings => ...)` |
| `AddAssertableHttpClientFactory()` | `AddAssertableHttpClient` - the factory registered the client without its dependencies |
| the `consumerAssembly` parameter of `AddAssertableHttpClient` | none needed |
| every other `services.Add...()` building block (`AddJsonDiffer`, `AddCurlBuilder`, ...) and the service interfaces behind them | internal - `AddAssertableHttpClient` registers all of them |

Calling `AddAssertableHttpClient` more than once is safe and applies every `settings => ...` in call order.

### Breaking: one namespace for the whole package

Every public type now lives in `AspNetCore.Simple.MsTest.Sdk`. The sub-namespaces mirrored the SDK's folder
structure, so any internal restructuring would have deleted a namespace your code imports. That cannot happen
anymore - a contract test keeps every public type in the root namespace.

| Removed namespace (10.x) | Types | What to do |
|---|---|---|
| `AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient` | `AddAssertableHttpClientExtension`, `IAssertableHttpClient`, `HttpResponseContext<T>` | delete the `using` - `using AspNetCore.Simple.MsTest.Sdk;` covers them |
| `AspNetCore.Simple.MsTest.Sdk.ErrorHandling` | `ITestErrorHandler`, `TestErrorHandler<TException>` | same |
| `AspNetCore.Simple.MsTest.Sdk.Decorators` | `AddTextDecoratorExtension` | same |

The fix is mechanical: remove the old `using` lines and keep (or add) `using AspNetCore.Simple.MsTest.Sdk;`.

### New

- **Fluent Assert API is public in the regular package.** `Client.AssertPost(url).Accepts(…).Produces<T>(code)
  .ExpectedResponse…(…).ExecuteAsync()` and the other entry points (`AssertGet`, `AssertPut`, `AssertPatch`,
  `AssertDelete`, `AssertQuery`, `AssertOptions`) were public only in prerelease builds (`FLUENT_ALPHA`); the
  switch is gone. The classic `Assert…Async` API stays unchanged — both run the same pipeline. Users of the
  alpha: see the entries below, several signatures changed since the last prerelease.
- **Fluent API - analyzer `MSTESTSDK003` for a forgotten `WriteSnapshot()`.** Recording runs in
  Debug only, so a committed `WriteSnapshot()` re-records the expected file on every Debug run and the test can
  never fail there. The new warning reports `WriteSnapshot()` / `WriteSnapshot(true)` in **Release builds only** —
  Debug stays silent so you can record and debug. To update snapshots in bulk, set
  `TestSdkSettings__WriteResponse=true` instead of editing tests.
- **Fluent API - precise comparison vocabulary.** `IgnoreProperty(p => p.Address.Street)`
  ignores exactly that path (not every `street`), follows `[JsonPropertyName]`, and covers collections via
  `p => p.Emails.Select(e => e.EmailAddress)`. New: `IgnorePaths("emails[*].type")`,
  `IgnoreDifferencesWhere(d => …)` (true = ignore), `FilterDifferences(diffs => …)` (returns what counts).
  Removed: `IgnoreDifferences` (its lambda returned the differences to KEEP), `DifferenceFilter`, and
  `IgnoreProperty<T>` (matched the last path segment only). Migration: `.DifferenceFilter(d => !X)` →
  `.IgnoreDifferencesWhere(d => X)`, `.IgnoreDifferences(f)` → `.FilterDifferences(f)`,
  `.IgnoreProperty<Person>(p => p.Id)` → `.IgnoreProperty(p => p.Id)` (or `list => list.Select(p => p.Id)`).
- **Fluent API - immutable builders.** Every configuring call returns a new builder. A stored
  base request (or `Produces…` / `ExpectedResponse…` stage) can be branched safely: before, a later
  `WithHeader` / `WithParameter` on the base changed chains already derived from it, parameters of sibling
  branches accumulated, and an `IgnoreProperty` on a lenient branch also switched off the strict chain's
  comparison. A configuring call whose result is discarded now has no effect.
- **Fluent API - `WithParameters` hardened.** New overload
  `WithParameters(IReadOnlyDictionary<string, object?>)`; any other dictionary passed as `object`
  (`Dictionary<string, string>`, …) contributes its entries instead of `$Count$`/`$Keys$` and an indexer crash.
  Indexer properties of a parameter object are skipped. A value (`string`, number, enum, `Guid`, …) or a collection
  now throws `ArgumentException` - before, a string crashed with `TargetParameterCountException` and a number
  silently added nothing, leaving the placeholder unreplaced.
- **Fluent API - `ExecuteAsync(CancellationToken cancellationToken = default)`** on every
  terminal (typed, body-less, status-only, OPTIONS). The token reaches `HttpClient.SendAsync` and the response
  read - before, every call used `CancellationToken.None`. A cancellation surfaces as
  `OperationCanceledException`, not as an assertion failure. `HttpAssertContext<TResult>` gained the matching
  `CancellationToken` property (default `None`).
- **Fluent API - the builders guide you (breaking for alpha users).** POST/PUT/PATCH/QUERY start
  on the new `IHttpBodyRequestConfiguring`; every `Accepts…` leads to `IHttpRequestConfiguring`, which offers no
  `Accepts…`. GET/DELETE start there directly. So `Client.AssertGet(url).Accepts(…)` (GET/DELETE carry no body - the
  classic API never sends one) and `.Accepts(a).Accepts(b)` (the second body silently won) no longer compile. The order
  of `With…` and `Accepts…` stays free. Migration: if a variable was typed `IHttpRequestConfiguring` for a POST/PUT/
  PATCH/QUERY chain before its body, type it `IHttpBodyRequestConfiguring` (or use `var`).
- **Fluent API - a parameter or header name given twice throws** an `ArgumentException` with a
  what/why/how-to-fix message, at the repeating line. Before, a repeated parameter silently sent the FIRST value and
  a repeated header the LAST one. Header names compare case-insensitively. Overriding a name a stored base stage
  already holds counts as repeating - set it per branch instead.
- **Fluent API - `WithQueryParameter(key, value)`.** Builds the query string from parts instead of
  by hand in the url: key and value are URL-encoded (`"Son Goku & Co"` → `Son%20Goku%20%26%20Co`), `null` adds nothing,
  a collection repeats the key (`tag=a&tag=b`), values are formatted culture-invariant, and a url that already has a
  query is continued with `&`. A key given twice (case-insensitive) throws an `ArgumentException` with a
  what/why/how-to-fix message. Available on both request stages, before and after `Accepts…`.
- **Fluent API - `WithBearerToken(token)` / `WithoutAuthorization()`.** Credentials for one request:
  the `Authorization` header goes on the request message, so it wins over the client's default for this call and
  `HttpClient.DefaultRequestHeaders` stay untouched - safe on a client shared by the whole session and by parallel
  tests, nothing to restore. `WithoutAuthorization()` sends the request without the client's default credentials (for
  a 401 test on a signed-in client). Credentials are set once per chain; a repeat - also via
  `WithHeader("Authorization", …)` - throws an `ArgumentException` that never prints the token. The curl output shows
  the per-request credentials (masked unless `ShowTokenInCurl`).
- **Fluent API - `FilterResponse` composes.** A second call ran instead of the first; now both
  run in call order, like the ignore rules.
- **Fluent API - `AssertOptions(url)` is immutable, too.** `ExpectedHeaders…` mutated the stored
  stage, so two branches of one `AssertOptions(url)` both compared against the expectation set last.
- **Fluent API - `Produces(code)` validates the route (behavior change).** The status-only path
  and `Produces(code).ExpectedResponse(obj)` skipped the whole endpoint validation because they name no response
  type - which also skipped the ROUTE check: a misspelled url (404), a wrong verb (405) or an unknown api version
  went green. Now verb, url and version are validated; the type still is not. A test that hits an unknown route on
  purpose adds `.SkipEndpointValidation()` (the "endpoint not found" output says so).
- **Fluent API - `.SkipEndpointValidation()` comes after `Produces…`.** It switches off the route AND the
  response-type check, so it is offered on the stages behind `Produces…` - also after `ExpectedResponse…`, as the
  last call before `ExecuteAsync()`. Before `Produces…` it does not compile.

### Fixed: OPTIONS assertions - `Allow` and header name spelling (classic and fluent)

- **`Allow` and every `Content-*` header are compared.** `AssertOptionsAsync` / `AssertOptions(url)` read only
  `HttpResponseMessage.Headers`; `HttpClient` files `Allow` - the header OPTIONS exists for - under the content
  headers, so it could never be asserted. Response and content headers are compared together now; content headers
  as sent (`Allow: GET, POST` is one value). **Behavior change:** an expectation must now also list the content
  headers the endpoint sends.
- **Header names compare case-insensitively.** The dictionary overloads serialized the names with the app's
  dictionary key policy (camelCase turned `Access-Control-Allow-Methods` into `access-Control-Allow-Methods`) and the
  comparison was case-sensitive, so a dictionary expectation of a mixed-case header always failed, and so did raw
  JSON in another spelling. Names are kept as written and matched case-insensitively; values stay exact. A header
  listed twice in another spelling (`"Allow"` and `"allow"`) fails with a what/why/fix message.
- **"Endpoint not found" output** now shows how to test a deliberately unknown route (`.SkipEndpointValidation()` /
  `skipEndpointValidation: true`).

### Fixed: `Assert…AsUnauthorizedAsync` changed the shared client's credentials

`AssertGet/Post/Put/Patch/Delete/QueryAsUnauthorizedAsync` swapped `HttpClient.DefaultRequestHeaders.Authorization` to an
invalid token and restored it after the call. On a client shared by the test session, tests running in parallel sent
the invalid token meanwhile (random 401s), and a call that threw never restored the original - every later test ran
unauthorized. The invalid token now rides on the request message only; the client is never written.

### Fixed: a body carrying `content.value` was taken for the response envelope

A response body with its own `content.value` (a blog post, a CMS page) was read as the SDK's snapshot
envelope: the comparison unwrapped the fragment and failed with "INVALID JSON FORMAT" or an unexpected
parse error. Only a real envelope - `content.value` together with `statusCode` - is unwrapped now.

### Fixed: request details that were sent differently than the test reads

- **Placeholder values are formatted culture-invariant (classic and fluent).** `WithParameter("Amount", 1.5m)` /
  `("$Amount$", 1.5m)` used the machine's current culture: a German machine sent `amount=1,5` in the url and the
  invalid json `{ "amount": 1,5 }` in a body, while the build server sent `1.5`. Url, body and expected json now use
  `CultureInfo.InvariantCulture` - the same rule `WithQueryParameter` already followed. **Behavior change:** a
  `DateTime` placeholder now reads `10/13/2026 08:30:00` on every machine instead of the local date format.
- **Fluent API - content headers are sent.** `WithHeader("Content-Type", …)` (and every other `Content-*` header)
  was dropped silently: `HttpClient` keeps them on the content and refuses them on the request. They now replace the
  content's own value. A header `HttpClient` refuses altogether (e.g. a name with spaces) fails the test instead of
  going out missing.
- **Fluent API - blank parameter and header names throw** an `ArgumentException` at the line that gives them.
  Before, `WithParameter("", value)` / `WithHeader("", value)` were accepted and the value was never sent.
- **Fluent API - placeholder names are naked.** A name carrying the `$` delimiter anywhere but as a full wrap
  (`"$"`, `"$$"`, `"$Id"`, `"I$d"`) throws - it named a token nothing contains. `WithParameter("Id", 1)` is the
  spelling; the wrapped `"$Id$"` keeps working for migration.
- **Fluent API - placeholder values in the url are URL-encoded.** `WithParameter("Name", "Son Goku & Co")` in
  `?name=$Name$` split the query at `&`; now the whole value arrives. The classic API is unchanged.
- **Fluent API - a parameter no placeholder takes throws** (`PLACEHOLDER PARAMETER NOT USED`, what/why/fix) when the
  chain executes. Placeholder names are case-sensitive, so `WithParameter("Name", …)` for `$NAME$` sent nothing;
  neither did a placeholder inside `WithQueryParameter`, which sends its value literally. Url, body and expected
  response are searched; the check is off while recording a snapshot (the placeholder lands in the file only then).
- **Fluent API - blank inputs throw.** `AcceptsFromJsonString("")`, `ExpectedResponseFromJsonString("")` (went green
  without comparing anything), the `…FromEmbeddedJson("")` variants, `ExpectedHeadersFromJsonString("")` and an empty
  `IgnorePaths()` are rejected at the line that gives them.

## 10.0.0 - One settings class instead of static globals

### Breaking: all global settings moved into `TestSdkSettings`

The static settings are gone. Configure them once through `TestSdkSettings` instead - see
"Configuration via TestSdkSettings" and "Upgrading from 9.5.x" in the README.

| Removed | Replacement |
|---|---|
| `HttpClientAssertExtensions.JsonSerializerOptions` | `settings.JsonSerializerOptions` |
| `AssertObjectExtensions.JsonSerializerOptions` | `settings.JsonSerializerOptions` |
| `HttpClientAssertExtensions.SkipEndpointValidation` | `settings.SkipEndpointValidation` |
| `HttpClientAssertExtensions.ShowTokenInCurl` | `settings.ShowTokenInCurl` |
| `HttpClientAssertExtensions.LogAction` | `settings.LogAction` |
| `HttpClientAssertExtensions.CustomAssertableHttpClient` | register your own `IAssertableHttpClient` before `AddAssertableHttpClient` |
| `AssertObjectExtensions.DifferenceFunc` / `DifferenceFilter` / `OrderIndependentArrayFilter` | same names on `TestSdkSettings` |
| `AssertObjectExtensions.WriteResponse` | `settings.WriteResponse` |
| `AssertObjectExtensions.ResponseFileFullPath` | removed - it had no effect |
| environment variable `AspNetCoreSimpleMsTestSdk__WriteResponse` | `TestSdkSettings__WriteResponse` |
| `Tables.TableFormatter.From(...)` | removed - unused static helper; use `ITableBuilder` |
| `Outputs.Formatters.JsonTypeMismatchFormatter` | removed - duplicate of `IJsonTypeMismatchOutputBuilder` |

- `services.AddTestSdkSettings(configuration, settings => ...)` - with `ApiTestBase<T>`, from `registerServices`
- `services.AddAssertableHttpClient(configuration, settings => ...)` - with your own host
- `HttpClientAssertExtensions.Setup(settings => ...)` - without any host
- The provider handed to `HttpClientAssertExtensions.Setup(provider)` must stay alive for the whole test run.

### New

- **Output modes** - `TestSdkSettings.OutputMode` = `Human` (default), `Ai` (structured JSON with error codes
  and fix suggestions) or `Hybrid` (both). Configure via `TestSdkSettings__OutputMode=Ai` or appsettings.
- `HttpClientAssertExtensions.Setup(Action<TestSdkSettings>)` - configure the SDK for tests without a host.
- `services.AddTestSdkSettings(configuration, settings => ...)` is now public; the first configured
  registration wins, so calling it from `registerServices` of `ApiTestBase<T>` overrides the SDK defaults.
- `TestSdkSettings` properties are now settable (`set` instead of `init`).
- `ApiTestBase<T>` hands the running host's own provider to the assert extensions - asserts and the
  application under test share one container (previously a separate, immediately disposed provider).

### Fixed: dictionary responses in recording mode

With `writeResponse` a `Dictionary<,>`/`IReadOnlyDictionary<,>` response was treated as an array, so an
existing `{...}` snapshot failed with an "array/object mismatch". The expected json is now resolved by the
one `IJsonStringResolver`, which already handled dictionaries and empty envelope values correctly.

## 9.3.0 - Assert.That Extensions (2026-06-07)

Core `Assert.That.*` assertions were introduced in 9.2.0; 9.3.0 completed them with the DateTime /
DateTimeOffset extensions described below.

### New: AI-Friendly Assertion Library

We've added a comprehensive assertion library designed for maximum debuggability in both human and AI workflows.

#### What's new

**`Assert.That.*` - Modern assertion API**

- Mandatory `because` and `fix` parameters enforce test context documentation
- Rich structured failure output with sections: Test Info → Problem → Details → Context → Suggested Fix
- Automatic variable name capture via `CallerArgumentExpression` 
- Organized by category: Boolean, Numeric, String, Collection, DateTime, Exception, Type
- Beautiful console output with emoji decorators (opt-in plain text mode available)
- Clickable `file://` URIs in output for instant IDE navigation

#### Coverage

| Category | Assertions | Examples |
|----------|------------|----------|
| **Core** | Boolean, Null, Equality, Type | `IsTrue`, `IsNotNull`, `AreEqual`, `IsInstanceOfType` |
| **Numeric** | Comparison, Range, Math, Tolerance | `IsGreaterThan`, `IsInRange`, `IsPositive`, `IsWithinTolerance` |
| **String** | Content, Length, Pattern, Null | `Contains`, `StartsWith`, `Matches`, `IsEmpty` |
| **Collection** | Count, Contains, Equality, Predicate | `HasCount`, `Contains`, `AllMatch` |
| **DateTime** | Comparison, Range, Tolerance, Kind | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `IsUtc`, `IsLocal`, `IsUnspecified` |
| **DateTimeOffset** | Comparison, Range, Tolerance, Offset | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `HasOffset`, `IsUtc`, `IsLocal` |
| **Exception** | Throws, DoesNotThrow | `Throws<T>`, `DoesNotThrow` |

#### Example usage

```csharp
// Before
Assert.IsTrue(user.IsActive, "User should be active");

// After - with full context
Assert.That.IsTrue(user.IsActive,
    because: "Active users should have access to premium features",
    fix: "Check user activation logic in UserService.ActivateAsync()");
```

#### Why this matters

Traditional assertions fail with minimal context. `Assert.That.*` provides:

- **Why it matters** - business context via `because` parameter
- **How to fix it** - remediation guidance via `fix` parameter  
- **What failed** - captured expression shows `user.IsActive && user.IsVerified`
- **Where it failed** - clickable file path with line number
- **Additional suggestions** - context-specific fix options

This makes test failures **instantly debuggable** for humans and **fully parseable** for AI debugging tools.

#### Design decision: No `AiAssert` namespace

We considered creating a separate `AiAssert.*` namespace but decided against it:

- ✅ Mandatory context parameters already make assertions AI-friendly
- ✅ Structured output is easily parseable by tools
- ✅ Single API = no confusion about when to use what
- ✅ Human readability comes first, AI parsability follows naturally

#### Enhanced DateTime/DateTimeOffset assertions

**New DateTime Kind assertions:**
- `IsUtc()` - Validates `DateTime.Kind == DateTimeKind.Utc`
- `IsLocal()` - Validates `DateTime.Kind == DateTimeKind.Local`
- `IsUnspecified()` - Validates `DateTime.Kind == DateTimeKind.Unspecified`

These help catch common timezone bugs where APIs mix UTC and local times incorrectly.

**New DateTimeOffset support:**

Complete set of timezone-aware assertions for `DateTimeOffset`:
- `IsAfter()`, `IsBefore()` - Compare absolute points in time (timezone-aware)
- `IsInRange()` - Validate DateTimeOffset falls within range
- `IsCloseTo()` - Check if within tolerance (handles millisecond precision)
- `HasOffset()` - Validate specific UTC offset (e.g., `TimeSpan.FromHours(2)` for UTC+2)
- `IsUtc()` - Validates offset is `TimeSpan.Zero`
- `IsLocal()` - Validates offset matches current local timezone

**Why DateTimeOffset matters:**

`DateTimeOffset` is superior to `DateTime` for distributed systems because it preserves timezone context. These assertions help ensure:
- API responses include correct timezone offsets
- Comparisons work across different timezones
- Database timestamps maintain timezone information
- Event timestamps from different regions compare correctly

**Example usage:**

```csharp
// DateTime Kind validation
Assert.That.IsUtc(createdAt,
    because: "Database timestamps must be stored in UTC",
    fix: "Use DateTime.UtcNow instead of DateTime.Now");

// DateTimeOffset timezone validation
Assert.That.HasOffset(apiTimestamp, TimeSpan.FromHours(2),
    because: "API must return timestamps in Central European Time (UTC+2)",
    fix: "Configure the API timezone in appsettings.json");

// Timezone-aware comparison
Assert.That.IsAfter(eventTime, deadline,
    because: "Event must occur after the registration deadline",
    fix: "Adjust event scheduling to respect timezone differences");
```

**Test coverage:**
- 10 new tests for DateTime Kind assertions
- 22 new tests for DateTimeOffset assertions
- Full coverage of edge cases (negative offsets, millisecond precision, mixed timezones)

### Breaking changes

None - these are new additions to the SDK.

### Migration guide

Existing tests continue to work. To adopt `Assert.That.*`:

1. Replace traditional `Assert.*` calls with `Assert.That.*` equivalents
2. Add `because` parameter with business context
3. Add `fix` parameter with remediation guidance

Example:

```csharp
// Old
Assert.AreEqual(expected, actual, "Values should match");

// New
Assert.That.AreEqual(expected, actual,
    because: "API response should match expected snapshot after user creation",
    fix: "Check UserController.CreateAsync() return value mapping");
```

### Performance impact

Negligible. `CallerArgumentExpression` and caller info attributes are compile-time features with zero runtime cost. String formatting only happens when assertions fail.

### Future enhancements

- ~~Optional JSON output mode for CI/CD tool integration~~ - delivered as `TestSdkSettings.OutputMode` (see above)
- Custom assertion message templates
- Integration with test result analyzers

---

**Full documentation**: See README.md → "Assert.That - AI-Friendly Assertions" section
