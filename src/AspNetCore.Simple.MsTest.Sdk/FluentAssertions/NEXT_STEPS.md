# Fluent Assert API — Next Steps (Handover)

> Self-contained work list for the next agent. Written 2026-10-13. Check items off / delete them as they land;
> record design decisions in the decision log at the end of `DESIGN_VISION.md` (append-only).

## Current state — read first

- **Everything below marked ✅ is committed on `master`** (last: `d073d47`). The user commits themselves — do not
  commit unless asked.
- **Working rules (from the user):**
  - Every fix is proven with **automated consumer tests through a real API call** (sample APIs `src/MinimalApi`
    + `src/MinimalApi.Test`, `src/Controllers` + `src/Controllers.Test`), each positive case paired with a test
    that MUST fail (`Assert.That.ThrowsExactlyAsync<AssertFailedException>(…, because:, fix:)`).
  - Counter-check: revert the fix temporarily (`git stash push -- <sdk files>`) and confirm the new tests go red.
  - Add a small deterministic sample endpoint when no existing response shape can prove the behavior
    (example: `src/MinimalApi/Api/Persons/V1/Profile/`).
  - Run the full suite + `dotnet build src/AspNetCore.Simple.MsTest.Sdk/AspNetCore.Simple.MsTest.Sdk.csproj -c Release`
    (Release = no `FLUENT_ALPHA`, fluent types internal) before reporting.
  - Docs are English; update Fluent `README.md` / `EXAMPLES.md` / `HOWTO.md`, root `README.md`,
    `RELEASE-NOTES.md`, this file, and append to the decision log in `DESIGN_VISION.md`.
- **Already done:**
  - Fluent parity with the classic API: `AssertQuery`, `AssertOptions(...).ExpectedHeaders…`,
    `.SkipEndpointValidation()`, `Produces(code).ExpectedResponse(anonymousObject)`.
  - Precise comparison vocabulary (items 1 + 2) and failure-output parity (item 5) — see below.
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

## Priority 1 — before stable (signature changes) ✅ done 2026-10-13

1. ~~`IgnoreDifferences` semantics inverted~~ and 2. ~~`IgnoreProperty<T>` ignores more than it promises~~ —
   replaced by `IgnoreProperty(Expression<Func<TResult, object?>>)` (full path), `IgnorePaths`,
   `IgnoreDifferencesWhere` (true = ignore) and `FilterDifferences` (returns what counts); see the decision log
   entry of 2026-10-13. Path matching: `Builders/MemberPathPattern.cs`. Consumer tests:
   `src/MinimalApi.Test/Api/Persons/V1/Profile/PersonProfileComparisonTests_Fluent.cs` against the new sample
   endpoint `GET persons/{id}/profile`.
   - **Path format correction:** on the HTTP path the differ compares the response ENVELOPE, so difference paths
     read `content.value.name`, `content.value.contacts[0].value` (not `name`). `IgnorePaths` accepts both forms;
     `IgnoreDifferencesWhere` sees the raw envelope path.
   - **Testing rule (from the user):** every fix is proven with automated consumer tests through a real API call
     (MinimalApi / Controllers sample APIs), each positive case paired with a test that MUST fail.
   - Note: `ApiTestBase` registers `TestHelpers.IgnoreIdDifferences` globally (drops every `….id` path) — don't use
     nested `id` to prove an ignore rule is precise; the global func hides it anyway.

## Priority 2 — should land

3. ~~**Builders share mutable state.**~~ ✅ done 2026-10-13 (uncommitted). All three builders are copy-on-write
   (`MemberwiseClone` + `ImmutableList` / `ImmutableDictionary`); every configuring call returns a new builder.
   Copy-on-transition alone was rejected: it does not fix sibling branches accumulating parameters
   (`base.WithParameter("Name", "Son")` / `base.WithParameter("Name", "Vegeta")`). Consumer tests:
   `src/MinimalApi.Test/FluentBuilders/FluentBuilderBranchingTests.cs` (headers via `echo-header`, parameters via
   `persons?name=$Name$`, strict vs. lenient comparison via `profile`); counter-check: 7 of 8 red without the fix
   (the 8th, a lenient positive case, is green either way). Consequence: a discarded result does nothing —
   MSTESTSDK001 already flags bare `builder.WithX(…);` statements; tests use `_ =` to branch deliberately.
   **Item 7 note:** with immutable builders, "second `Accepts…` overwrites" only concerns the SAME chain
   (`.Accepts(a).Accepts(b)`) — sibling branches are now independent by design.

4. ~~**Harden `WithParameters(object)`**~~ ✅ done 2026-10-13 (uncommitted). Indexers skipped; new
   `WithParameters(IReadOnlyDictionary<string, object?>)`; the `object` overload also maps any `IDictionary`
   (needed: `Dictionary<string, string>` does not convert to the `object?` overload); values (`string`, primitives,
   enum, `decimal`, date/time types, `Guid`, `Uri`) and other `IEnumerable` → `ArgumentException`. Consumer tests:
   `src/MinimalApi.Test/FluentBuilders/WithParametersTests.cs` (real calls against `persons?name=$Name$`);
   counter-check: 9 of 11 red without the fix (the 2 green ones are the unchanged anonymous-object path).

5. ~~**Failure-output parity with the classic API**~~ ✅ done 2026-10-13. Verified: only the "file not found"
   and JSON-type-mismatch reports leaked builder internals (`Parameter : expectedResult`,
   `Parameter : _body ?? string.Empty`); diff tables and status-code reports were already identical. The body and
   expected methods (`Accepts…`, `ExpectedResponse…`) now take an optional `[CallerArgumentExpression]` argument,
   which the builders pass explicitly to the engine. Consumer tests:
   `src/MinimalApi.Test/FailureOutput/FluentFailureOutputTests.cs` (counter-checked: all red without the fix).

6. ~~**No `CancellationToken`.**~~ ✅ done 2026-10-13 (uncommitted). `ExecuteAsync(CancellationToken
   cancellationToken = default)` on all four terminal interfaces; carried by `HttpAssertContext<T>.CancellationToken`
   (public, additive) to `SendAsync` / `ReadAsStringAsync`; OPTIONS forwards it directly. `AssertableHttpClient`
   rethrows `OperationCanceledException` when the token is cancelled (otherwise the catch-all turned it into
   `Assert.Fail`). Analyzer stub updated + test `Fires_When_Terminal_With_CancellationToken_Not_Awaited`; analyzer
   (21) and code-fix (8) tests green. Consumer tests: `src/MinimalApi.Test/FluentBuilders/CancellationTests.cs`;
   counter-check: without forwarding 4 of 6 red, without the rethrow 3 of 6 red. The classic API still uses
   `None` (not in scope).

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

~~Commit → 1 + 2 → 5 → 3 → 4 → 6~~ (done) → 7 → 8 → Priority 3 → cleanup.
