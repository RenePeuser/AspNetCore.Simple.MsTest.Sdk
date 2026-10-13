# Fluent Assert API — Next Steps (Handover)

> Self-contained work list for the next agent. Written 2026-10-13. Check items off / delete them as they land;
> record design decisions in the decision log at the end of `DESIGN_VISION.md` (append-only).

## Current state — read first

- **Commit state:** items 1–7, 13 and the builder review are committed on `master` (`39f3565`). The **three open
  review findings (OPTIONS headers, header-name case, route check of `Produces(code)`) are done but UNCOMMITTED**
  (check `git status`). The user commits themselves — do not commit unless asked.
- **Design rule (from the user):** the builders GUIDE the user so nothing can be used wrongly — a method only where
  it means something, a once-only call is a stage switch, a wrong chain does not compile. Runtime throws only for
  what no type can see (repeated names), and then with the sdk's what/why/fix output.
- **NEXT TASK = item 8 (analyzer for `WriteSnapshot()`).** Open question for the user, see "Review findings": read
  RESPONSE headers raw too (consistency) or keep the parsed form?
- **Working rules (from the user):**
  - Every fix is proven with **automated consumer tests through a real API call** (sample APIs `src/MinimalApi`
    + `src/MinimalApi.Test`, `src/Controllers` + `src/Controllers.Test`), each positive case paired with a test
    that MUST fail (`Assert.That.ThrowsExactlyAsync<AssertFailedException>(…, because:, fix:)`).
  - Counter-check: revert the fix temporarily and confirm the new tests go red. `git stash push -- <sdk files>`
    only works when the tests compile without the fix; when they reference new types (as in the builder review),
    back up `FluentAssertions/`, revert the fix by hand into its OLD shape, build, test, restore the backup.
  **Restoring with `Copy-Item` keeps the backup's OLD timestamp → MSBuild thinks the sdk is up to date and the next
  run still tests the reverted code.** Touch restored files (`(Get-Item f).LastWriteTime = Get-Date`). Never run
  a counter-check with `--no-build` in another test project — it uses the sdk dll copied there earlier.
  - Present findings + a proposal and get the user's go BEFORE changing signatures or behavior.
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
- **IDE0005 fails the build** — an unnecessary `using` is an error, not a warning. Other CA rules are errors too
  (seen: CA1510, CA1859, CA2016) — a counter-check edit that fails to compile silently re-runs OLD binaries; always
  confirm "Build succeeded" before reading test results. Also errors: block-bodied methods (no `=> …` on methods,
  `.editorconfig`), IDE0055 formatting (e.g. `[..x]` spread), CS8604 nullability.
- **Tooling:** in this Windows/Git-Bash setup, multi-line Python or text with quotes inside a Bash heredoc breaks the
  shell — write the script to a file (Write tool) and run it, or use the Edit tool.

## Builder review ✅ done 2026-10-13 (committed `39f3565`) — findings below ✅ done (uncommitted)

Decided with the user and implemented (decision log entry of 2026-10-13, "Builder führen den Nutzer"):

```
IHttpBodyRequestConfiguring        (AssertPost / AssertPut / AssertPatch / AssertQuery)
  ├─ Accepts… ──────────────────► IHttpRequestConfiguring   (no Accepts… anymore → body once)
  ├─ WithParameter/WithHeader/SkipEndpointValidation ► IHttpBodyRequestConfiguring   (order free)
  └─ Produces…
IHttpRequestConfiguring            (AssertGet / AssertDelete — no body at all — or after Accepts…)
  ├─ WithParameter/WithHeader/SkipEndpointValidation ► IHttpRequestConfiguring
  └─ Produces…
```

- `HttpRequestBuilder` implements both; `With…` are explicit interface implementations sharing `AddParameters` /
  `AddParametersFrom` / `AddHeader`. `Produces…` and `Accepts…` are public (same signature on both stages).
- `SkipEndpointValidation()` stays on the request stage — the `EndpointValidator` checks the ROUTE (verb/url/version)
  before the response type, so it is about the whole endpoint, not `Produces<T>`. Idempotent, so not once-only
  (once-only would double the request interfaces for no information gained).
- Repeated parameter name (ordinal, after `PlaceholderName.Wrap`) / header name (case-insensitive) →
  `ArgumentException` built by `Builders/FluentUsageError.cs` (what / why / how-to-fix / test info). Before: parameter
  first-wins (`ParameterReplacer` replaces the token once), header last-wins (`SetItem`). Consequence: overriding a
  name a stored base stage holds is an error — `FluentBuilderBranchingTests` header tests now branch siblings.
- `FilterResponse` composes in call order (runs on actual AND expected — see `JsonComparisonStep`).
- `HttpOptionsBuilder` is copy-on-write (it still mutated `this` — missed by item 3).
- Kept: `Produces(code).ExpectedResponse<T>(obj)` vs. `Produces<T>(code).ExpectedResponse…` split; free order;
  `WriteSnapshot(bool)` (user: not in this round).
- New sample endpoint `src/MinimalApi/Api/Persons/V1/EchoRequest/` — `POST persons/echo-request` echoes
  `X-Correlation-Id` + body name, `OPTIONS persons/echo-request` answers 204 with
  `Access-Control-Allow-Methods: POST, OPTIONS` (item 13).
- Tests (`src/MinimalApi.Test/FluentBuilders/`): `FluentTypeStateCompileTests` (Roslyn compiles snippets against
  the REAL sdk assembly via `TRUSTED_PLATFORM_ASSEMBLIES` + sdk location — wrong chains → CS1061, allowed orders
  compile, each wrong chain paired with "asserting it compiles must fail"), `FluentRequestStageTests`,
  `FluentChainRulesTests`, `FluentOptionsTests`; analyzer test `Fires_When_BodyRequestBuilder_Dangles_Before_Accepts`.
  Counter-check (targeted reverts with backup, NOT `git stash` — the tests reference the new interface): 16 of 65
  builder tests red without the fixes.

### Review findings ✅ done 2026-10-13 (uncommitted) — decision log entries "OPTIONS vergleicht…" and "`Produces(code)` validiert die ROUTE"

- ✅ **OPTIONS compares response AND content headers** (`AssertHttpExtensions/OptionsHeaders.cs`, classic + fluent).
  Content headers raw (`NonValidated`), response headers unchanged (parsed). Sample `OPTIONS persons/echo-request`
  now sends `Allow` + `Access-Control-Allow-Methods`. **Open question for the user:** read response headers raw too?
  Consistent, but changes existing expectations of parsed list headers (`Vary`, `Cache-Control`).
- ✅ **Header names case-insensitive**, dictionary overloads without key policy, duplicate spelling → what/why/fix.
  Tests: `src/MinimalApi.Test/FluentBuilders/OptionsHeaders/OptionsHeaderTests.cs` (+ embedded
  `EchoRequestOptionsHeaders.json`). Counter-check: 14 / 11 / 7 / 1 red (see decision log).
- ✅ **`Produces(code)` validates the ROUTE, not the type** (`HttpAssertContext<T>.ValidateRouteOnly`, internal);
  `.SkipEndpointValidation()` is forwarded now. Tests: `src/MinimalApi.Test/FluentBuilders/StatusOnlyRouteValidationTests.cs`,
  `src/Controllers.Test/Api/Routing/StatusOnlyRouteValidationTests_Fluent.cs`. Counter-check: 10/27, 4/9 red.
  **Known limit (unchanged, existing design):** the route matcher falls back to ignoring constraints, so
  `GET persons/echo-request` matches `GET persons/{id:long}` — wrong-verb samples use paths without such a sibling.
- `Accepts(string)` (obsolete, error) exists on the body stage only — fine; GET/DELETE do not even show it.

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

7. ~~**Second `Accepts…` silently overwrites the first.**~~ ✅ done 2026-10-13 (uncommitted) via type-state —
   see "Builder review". `.Accepts(a).Accepts(b)` and GET/DELETE with a body are CS1061 now.

8. **Analyzer for `WriteSnapshot()`.** Committed, it rewrites the expected file on every run → always green.
   → MSTESTSDK003 (warning). Document the environment variable `TestSdkSettings__WriteResponse=true` as the way
   to bulk-update.

## Priority 3 — additive, fine after launch

9. **`.WithQuery(key, value)`** — central URL encoding, `null` skipped (most frequent need, `DESIGN_VISION.md` §13.2).
10. **`.WithBearerToken(token)` / `.WithoutAuthorization()`** — per-request auth (today bound to the static client).
11. **`.ExpectHeader(name, value)`** — response header assertions (`Location` on 201, `ETag`).
12. **Status aliases** `ProducesOk<T>()`, `ProducesCreated<T>()`, `ProducesNoContent()` — build before you
    document (`DESIGN_VISION.md` §15.7 warning).
13. ~~**Positive OPTIONS test.**~~ ✅ done 2026-10-13 (uncommitted): `OPTIONS persons/echo-request` +
    `src/MinimalApi.Test/FluentBuilders/FluentOptionsTests.cs`. See the OPTIONS findings under "Builder review".

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

~~Commit → 1 + 2 → 5 → 3 → 4 → 6 → Builder review → 7 → open review findings~~ (done) → **8** → Priority 3 →
cleanup.
