# Fluent Assert API — Next Steps (Handover)

> Self-contained work list for the next agent. Written 2026-10-13. Check items off / delete them as they land;
> record design decisions in the decision log at the end of `DESIGN_VISION.md` (append-only).

## Current state — read first

- **Uncommitted at the time of writing** (~100 files on `master`). If still uncommitted: commit on a branch,
  ideally as two commits — (1) classic-API parity + fluent namespace, (2) namespace cleanup of the stable
  types with `+semver: major`.
- **Already done:**
  - Fluent parity with the classic API: `AssertQuery`, `AssertOptions(...).ExpectedHeaders…`,
    `.SkipEndpointValidation()`, `Produces(code).ExpectedResponse(anonymousObject)`.
  - All public types live in the root namespace `AspNetCore.Simple.MsTest.Sdk`, guarded by
    `Core.Test/Core/PublicNamespace.Contract.Tests.cs`.
  - Analyzer marker is `AspNetCore.Simple.MsTest.Sdk.FluentBuilderAttribute`
    (`src/AspNetCore.Simple.MsTest.Sdk.Analyzers/FluentBuilderMarker.cs`, pinned by the contract test).
- **Next release is 11.0.0 (major).** Fluent signatures may change freely — the fluent API is public only in
  alpha builds (`FLUENT_ALPHA`).
- **Code:** `src/AspNetCore.Simple.MsTest.Sdk/FluentAssertions/` (`Builders/`, `Interfaces/`, `Extensions/`).
- **Tests:** `src/MinimalApi.Test`, `src/Controllers.Test` (`*_Fluent.cs`), `Core.Test`, analyzer + code-fix tests.
- **Build:** `dotnet build AspNetCore.Simple.MsTest.Sdk.sln -c Debug` (Debug sets `FLUENT_ALPHA` automatically).
- **IDE0005 fails the build** — an unnecessary `using` is an error, not a warning.

## Priority 1 — before stable (signature changes)

1. **`IgnoreDifferences` semantics are inverted relative to its name.**
   - Where: `Interfaces/IHttpComparisonConfiguring.cs`, implemented in `Builders/HttpResponseBuilder.cs`.
   - Problem: the lambda returns the differences to **keep** (`diffs.Where(d => d.MemberPath != "x")`). The
     intuitive `==` ignores everything else → silently green test. `DifferenceFilter(Predicate)` adds a second
     meaning ("true = counts as failure").
   - Proposal: `IgnoreDifferencesWhere(Predicate<Difference>)` (true = ignore) + `IgnorePaths(params string[])`.
     Rename the old ones (e.g. `TransformDifferences`) or make them non-public. Update
     `TestHelpers.IgnoreIdDifferences` and the docs.
   - Tests: one negative test per semantic that must fail.

2. **`IgnoreProperty<T>` ignores more than it promises.**
   - Where: `Builders/HttpResponseBuilder.cs` (`MemberPathTargets`, `ExtractPropertyName`).
   - Problem: compares only the last path segment by name → `p => p.Id` hits every `id` in the response,
     `p => p.Address.Street` every `street`. `T` is not tied to `TResult`. `[JsonPropertyName]` → silent no-op.
   - Proposal: build the full path from the expression (collection indices via wildcard), resolve the JSON name
     through the serializer options (`HttpClientAssertExtensions.JsonSerializerOptionsFor`), add an overload
     `IgnoreProperty(Expression<Func<TResult, object?>>)`. If "everywhere" is intended, name it
     `IgnorePropertyEverywhere`.
   - Difference path format: `id`, `[0].id`, `emails[0].emailAddress`.

## Priority 2 — should land

3. **Builders share mutable state.** `HttpRequestBuilder.Produces…` hands `_parameters` / `_headers` by
   reference to `HttpResponseBuilder` / `HttpExpectationBuilder`. Reusing a base request and configuring it
   further mutates already derived chains. → Immutable builders, or at least copy on the transition.

4. **Harden `WithParameters(object)`** (`Builders/HttpRequestBuilder.cs`):
   - `WithParameters("abc")` throws `TargetParameterCountException` (indexer `Chars`) → skip properties with
     `GetIndexParameters().Length > 0`.
   - A dictionary is reflected over (`Count`, `Keys`) → add an `IReadOnlyDictionary<string, object?>` overload.
   - Primitives / `string` → clear `ArgumentException`.

5. **Failure-output parity with the classic API (verify first).** `HttpResponseBuilder.ExecuteAsync` calls
   `AssertHttpCallAsync(... expectedResult: expectedResult ...)`; through `[CallerArgumentExpression]` the
   literal `"expectedResult"` (and the body expression) likely ends up in the failure output / "snapshot not
   found" hints, where the classic API shows the file name or `expectedResponse`. → Pass
   `expectedResultParameterName` / `payloadAsJsonParameterName` explicitly; add a test on the message.

6. **No `CancellationToken`.** `AssertableHttpClient.cs` calls `httpCallHandler.CallAsync(context,
   CancellationToken.None)`. → `ExecuteAsync(CancellationToken cancellationToken = default)` on every terminal
   interface, passed through the context. The analyzers match the terminal by the name `ExecuteAsync` — a
   parameter is fine, but run the analyzer tests.

7. **Second `Accepts…` / `ExpectedResponse…` silently overwrites the first.** → `InvalidOperationException`
   ("body already set").

8. **Analyzer for `WriteSnapshot()`.** Committed, it rewrites the expected file on every run → always green.
   → MSTESTSDK003 (warning). Document the environment variable `TestSdkSettings__WriteResponse=true` as the way
   to bulk-update.

## Priority 3 — additive, fine after launch

9. **`.WithQuery(key, value)`** — central URL encoding, `null` skipped (most frequent need, `DESIGN_VISION.md` §13.2).
10. **`.WithBearerToken(token)` / `.WithoutAuthorization()`** — per-request auth (today bound to the static client).
11. **`.ExpectHeader(name, value)`** — response header assertions (`Location` on 201, `ETag`).
12. **Status aliases** `ProducesOk<T>()`, `ProducesCreated<T>()`, `ProducesNoContent()` — build before you
    document (`DESIGN_VISION.md` §15.7 warning).
13. **Positive OPTIONS test.** Only a negative test exists (`src/MinimalApi.Test/Api/Errors/ErrorTests_Fluent.cs`),
    the sample APIs map no OPTIONS endpoint. Add one to `src/MinimalApi` (check endpoint-catalog tests).

## Cleanup

14. **Duplicate tests** in `src/Controllers.Test/Api/Persons/V1/Create/PersonCreateTests_Fluent.cs`:
    `_With_Explicit_Status` == `_With_Object`, `_With_Status_And_Filtering` == `_Ignore_Id`. Check the MinimalApi twin too.
15. **Repo root:** `.tmp-fluent/`, `fix_remaining_skip.py`, `fix_warnings.sh`, `test_diff.sh`, stale concept docs
    (`FINAL_BUILD_FIX.md`, `MIGRATION_PLAN.md`, `IMPLEMENTATION_SUMMARY.md`, …) — confirm with the user before deleting.
16. **Root `README.md`:** duplicated badge lines; update the "Alpha only" note on the fluent example once stable.
17. **`RELEASE-NOTES.md`:** "New" says `ITestErrorHandler` / `TestErrorHandler<T>` become public in 11.0.0, but
    they are already `public` in tag `v10.0.3` — clarify the wording with the user.
18. **`Outputs/Builders/TestInfoBuilder.cs:5`** — commented-out `//using …AssertableHttpClient;` can go.

## Deliberately NOT ported (do not reopen)

- `AssertUrlExists` stays classic (not a chain).
- `new {}` C# code generation stays classic (marked PROTOTYPE in the engine; untested for fluent).
- One `Produces` per chain and `ExecuteAsync()` as the single terminal — fixed in the decision log.

## Recommended order

Commit → **1 + 2 together** (comparison vocabulary, both can silently green a test) → 5 → 3 → 4 → 6 → rest as needed.
