# Fluent Assert API — Next Steps (Handover)

> Self-contained work list for the next agent. Written 2026-10-13. Check items off / delete them as they land;
> record design decisions in the decision log at the end of `DESIGN_VISION.md` (append-only).

## Current state — read first

- **Commit state:** everything up to the review findings is committed on `master` ("prepare fluent api release").
  Item 8 (MSTESTSDK003) is done but UNCOMMITTED (check `git status`). Older "(uncommitted)" marks below are
  historical. The user commits themselves — do not commit unless asked.
- **Design rule (from the user):** the builders GUIDE the user so nothing can be used wrongly — a method only where
  it means something, a once-only call is a stage switch, a wrong chain does not compile. Runtime throws only for
  what no type can see (repeated names), and then with the sdk's what/why/fix output.
- **NEXT TASK = Priority 3 (items 11, 12).** Open question for the user, see "Review findings": read
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
  - Run the full suite + `dotnet build AspNetCore.Simple.MsTest.Sdk.sln -c Release` before reporting.
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
- **Next release is 10.1.0 (`+semver: minor`) and ships the fluent API publicly** (user, 2026-10-13: the breaking
  changes of this release go into 10.1 — 10.x was already breaking vs. 9.x and is barely adopted) — `FLUENT_ALPHA` is removed (2026-10-13,
  uncommitted). From the stable release on, signature changes are breaking changes.
- **Code:** `src/AspNetCore.Simple.MsTest.Sdk/FluentAssertions/` (`Builders/`, `Interfaces/`, `Extensions/`).
- **Tests:** `src/MinimalApi.Test`, `src/Controllers.Test` (`*_Fluent.cs`), `Core.Test`, analyzer + code-fix tests.
- **Build:** `dotnet build AspNetCore.Simple.MsTest.Sdk.sln -c Debug`.
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
  ├─ WithParameter/WithHeader ► IHttpBodyRequestConfiguring   (order free)
  └─ Produces… ► SkipEndpointValidation() lives here (response / comparison / expectation stage)
IHttpRequestConfiguring            (AssertGet / AssertDelete — no body at all — or after Accepts…)
  ├─ WithParameter/WithHeader ► IHttpRequestConfiguring
  └─ Produces… ► SkipEndpointValidation() lives here (response / comparison / expectation stage)
```

- `HttpRequestBuilder` implements both; `With…` are explicit interface implementations sharing `AddParameters` /
  `AddParametersFrom` / `AddHeader`. `Produces…` and `Accepts…` are public (same signature on both stages).
- `SkipEndpointValidation()` lives AFTER `Produces…` (`IHttpResponseConfiguring<T>`, `IHttpComparisonConfiguring<T>`,
  `IHttpExpectationConfiguring`) - it drops the route AND the response-type check, so on the request stage it read as
  "request only". Not on the request stages (compile tests in `FluentTypeStateCompileTests`). Idempotent, so not
  once-only.
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

8. ~~**Analyzer for `WriteSnapshot()`.**~~ ✅ done 2026-10-13 (uncommitted). `WriteSnapshotInReleaseAnalyzer`
   (MSTESTSDK003, warning) reports `WriteSnapshot()` / `WriteSnapshot(true)` (literal, named, const) on a
   `[FluentBuilder]` type — **in Release builds only** (user: Debug is for recording/debugging; the runtime records
   in Debug only anyway). Not reported: `false`, non-constant argument. Fluent only — the classic
   `writeResponse: true` is not covered (possible follow-up). Known gap: pipelines that test in Debug only (like
   this repo's CI) never see it. `FluentWriteSnapshotTests` suppresses it via `#pragma`. Tests:
   `Analyzers.Test/WriteSnapshotInReleaseAnalyzerTests.cs` (13); counter-check: gate removed 3 red, inverted 9 red;
   real build of `Controllers.Test` without the pragma: Debug green, Release `error MSTESTSDK003`.
   **Gotcha:** after rebuilding the analyzer, run `dotnet build-server shutdown` — VBCSCompiler keeps the old
   analyzer dll loaded and reports stale results in the consumer projects.

## Priority 3 — additive, fine after launch

9. ~~**`.WithQueryParameter(key, value)`**~~ ✅ done 2026-10-13 (uncommitted). URL-encodes key and value, `null`
   adds nothing, a collection repeats the key, culture-invariant formatting, continues an existing query with `&`,
   a repeated key (case-insensitive) throws `FluentUsageError.DuplicateQueryParameter`. Sample endpoint
   `GET|POST persons/echo-query` (raw + decoded). Consumer tests:
   `src/MinimalApi.Test/FluentBuilders/WithQueryParameterTests.cs` (15); counter-check: without encoding 5 red,
   current-culture formatting 1 red, null not skipped 3 red.
10. ~~**`.WithBearerToken(token)` / `.WithoutAuthorization()`**~~ ✅ done 2026-10-13 (uncommitted). The
    `Authorization` header goes into the builder's request headers → on the `HttpRequestMessage`, never on
    `DefaultRequestHeaders` (nothing to restore, parallel-safe). `WithoutAuthorization()` = empty header (blocks the
    default merge; the TestServer endpoint receives none). Once per chain via `AddHeader` →
    `FluentUsageError.DuplicateAuthorization` (token masked). `CurlBuilder.BuildCurlFromContext` prefers the
    per-request credentials. Also fixed: the six classic `Assert…AsUnauthorizedAsync` swapped `DefaultRequestHeaders`
    (race + no restore on throw) → `SendWithInvalidTokenAsync`. Sample endpoints `persons/echo-authorization`,
    `persons/requires-token`; `ApiTestBase.CreateClient()`. Consumer tests: `FluentBuilders/WithBearerTokenTests.cs`
    (13), `Classic/AsUnauthorizedTests.cs` (4); counter-check: old swap → parallel test red, per-request header
    dropped → 6 red. Decided with the user: runtime throw (not type-state) for repeated credentials.
11. **`.ExpectHeader(name, value)`** — response header assertions (`Location` on 201, `ETag`).
12. **Status aliases** `ProducesOk<T>()`, `ProducesCreated<T>()`, `ProducesNoContent()` — build before you
    document (`DESIGN_VISION.md` §15.7 warning).
13. ~~**Positive OPTIONS test.**~~ ✅ done 2026-10-13 (uncommitted): `OPTIONS persons/echo-request` +
    `src/MinimalApi.Test/FluentBuilders/FluentOptionsTests.cs`. See the OPTIONS findings under "Builder review".

## Cleanup

14. ~~**Duplicate tests**~~ ✅ done 2026-10-13 (uncommitted). Controllers: `_With_Explicit_Status` and
    `_With_Status_And_Filtering` removed. MinimalApi: `_With_Object` (== `_ExpectedResponseFromEmbeddedJson`) and
    `_With_Json` (== `_AcceptsFromEmbeddedJson`) removed; the variant-section tests stay.
15. ~~**Repo root**~~ ✅ done 2026-10-13 (uncommitted, confirmed by the user): deleted `.tmp-fluent/`,
    `fix_remaining_skip.py`, `fix_warnings.sh`, `test_diff.sh`, `-/format-report.json` (empty `dotnet format`
    report), and the stale concept docs `FINAL_BUILD_FIX.md`, `MIGRATION_PLAN.md`, `IMPLEMENTATION_SUMMARY.md`,
    `AGENT_PROMPT_IMPLEMENT_ASSERTS.md`, `ASSERT_EXTENSIONS_ADVANCED_CREATIVE.md`, `ASSERT_EXTENSIONS_COMPLETE_LIST.md`,
    `ASSERT_FOLDER_STRUCTURE.md`, `CONCEPT_AI_FRIENDLY_ASSERTS.md`, `ERROR_HANDLING_STRATEGY.md`,
    `FILE_NAMING_GUIDE.md` (none referenced outside each other; committed as `a73a405`). Restored afterwards,
    unchanged, into `docu/concepts/` (user: keep the useful ideas): `ERROR_HANDLING_STRATEGY.md`,
    `FILE_NAMING_GUIDE.md`, `CONCEPT_AI_FRIENDLY_ASSERTS.md`, `ASSERT_EXTENSIONS_COMPLETE_LIST.md`,
    `ASSERT_EXTENSIONS_ADVANCED_CREATIVE.md`. The rest stays in history (`0cb4b63`). Still open, ask the user:
    `MIGRATION_PLAN.md`, `ASSERT_FOLDER_STRUCTURE.md` (deleted, undecided), `test-pack/` (untracked local
    `10.0.0-alpha.189.nupkg`). `LinkedIn-Article.txt` → `docu/linkedin-article.md`, rewritten for the 10.1 fluent
    launch (theme: "a test must never go silently green"). If items 11/12 land before posting, add them there.
16. ~~**Root `README.md`**~~ ✅ done 2026-10-13 (uncommitted): duplicate badges removed, Build badge pointed at
    the existing `build-and-publish.yml`, QUERY dropped from the fluent roadmap (it is implemented), install line
    uses `--prerelease` instead of the stale `9.6.0-alpha.19`. The "Alpha only" notes are gone too (fluent API
    released, `FLUENT_ALPHA` removed).
17. ~~**`RELEASE-NOTES.md`**~~ ✅ done 2026-10-13 (uncommitted): the "Own error handlers" entry is gone — the
    handlers are public since v10.0.0 and the registration order was already irrelevant in v10.0.3; the namespace
    table covers the only real change.
18. ~~**`Outputs/Builders/TestInfoBuilder.cs`**~~ ✅ done 2026-10-13 (uncommitted): the whole file was commented
    out and unreferenced — deleted.

## Pre-release coverage audit ✅ done 2026-10-13 (uncommitted)

Every public fluent method was mapped to consumer tests; gaps were probed with throw-away tests first (all four red
before the fix). Fixed, tests in `src/MinimalApi.Test/FluentBuilders/FluentRequestSafetyTests.cs` (14):

- `ParameterReplacer.Format` - placeholder values culture-invariant in url, body, expected json and snapshot writing
  (was `ToString()` → `1,5` in the url, invalid json in a body). Shared engine → classic API changes too.
- `HttpRequestMessageBuilder.AddRequestHeaders` - ignored `TryAddWithoutValidation`'s `false`: `Content-Type` & co.
  were dropped. Now set on the content (replacing its value); refused on both sides → exception → the engine reports
  it as `AssertFailedException`. Note: the engine always creates a (possibly empty) content, also for GET.
- `HttpRequestBuilder.AddParameters` / `AddHeader` - blank names throw (`Throw.IfNullOrWhiteSpace` from
  Argument.Check - **the user wants `Throw.*` for every guard**, not `ArgumentException.ThrowIf*`).

Second round (user: "alle Punkte prüfen, validieren, verbessern"), tests in
`src/MinimalApi.Test/FluentBuilders/FluentParameterAndInputTests.cs` (25), every gap probed red first:

- Url placeholder values URL-encoded and unused parameters reported - FLUENT ONLY via the engine flag
  `fluentParameters` (`AssertHttpCallAsync`, both overloads) → `ParameterReplacer.ReplaceInUrl(url, encodeValues, …)`
  and `EnsureEveryParameterIsUsed` (url + body + expected, skipped while recording) →
  `FluentUsageError.UnusedParameters`. The classic API is untouched (it passes recording-only parameters).
  Counter-check: flag off → 3 red.
- `PlaceholderName.Wrap` rejects names with `$` except the full legacy wrap.
- Blank `Accepts…`, `ExpectedResponse…`, `ExpectedHeaders…`, empty `IgnorePaths()` → `Throw.*`. The blank expected
  response went GREEN without comparing.
- Pinned (unchanged): `Accepts(null)` sends the json `null`; typed `"{}"` expectation still compares.
- **Gotcha:** never rewrite source files with Windows PowerShell `Get-Content -Raw` / `Set-Content` — it reads
  BOM-less UTF-8 as ANSI and writes mojibake (`§` → `Â§`). Use the Edit tool.
- Local env: `dotnet restore` fails with NU1507 because the user's global NuGet config has two sources
  (`nuget.org`, `pulse`). Restore with a config that only lists nuget.org (`--configfile`).

## Test gaps (found 2026-10-13, grep-based sample — not a full audit)

Before 10.1:

- **G1 — the package itself is untested.** Local proof ✅ 2026-10-13, **CI step waits for the user's go.**
  - `dotnet pack -c Release` (local and the CI artifact of run `38420572591`, 10.1.0) contains `lib/net10.0` (dll +
    xml), `analyzers/dotnet/cs` (Analyzers, CodeFixes, five `System.Composition*`), `readme.md`, `License.txt`.
  - Consumer `tests/PackageSmoke/` (outside the solution; own `Directory.Build.props` / `Directory.Packages.props` /
    `.editorconfig` stop the repo's settings; `nuget.config` maps the sdk to `../../nupkgs` ONLY). Restore with
    `--packages <empty dir>` (the global cache may hold an older build of the same version). `dotnet build` → green;
    `dotnet build -p:SmokeBroken=true` → `error MSTESTSDK001` (Broken/DanglingChain.cs(19,13)). Counter-check:
    package without `analyzers/` → the broken build goes GREEN.
  - **Open:** `tests/PackageSmoke/nuget.config` is git-ignored (`.gitignore` `Nuget.config`, case-insensitive) — it
    is missing from the commit; needs `git add -f` or a `!tests/PackageSmoke/nuget.config` exception.
  - **Cause of "Package Description":** the csproj has `<Descriptiotn>` (typo), not a missing element. Text from the
    user. Also stale: `<PackageReleaseNotes>` still reads "BREAKING CHANGES (11.0.0)".
- **G2 — CI tests in Debug only.** Local proof ✅ 2026-10-13, **workflow change waits for the user's go.**
  `dotnet build AspNetCore.Simple.MsTest.Sdk.sln -c Release` builds every test project green, 0 warnings
  (`FluentWriteSnapshotTests`' `#pragma` keeps MSTESTSDK003 out). Release *test runs* are NOT an option as is:
  24 Controllers tests fail by design (snapshot writers are Debug-only; `TheseTestsOnlyProveSomethingInADebugBuild`
  guards say so). So "`WriteSnapshot` is ignored in Release" stays unproven by a test run — would need a category to
  exclude the writer tests in Release (user decision).
- ~~**G3 — fluent features proven against MinimalApi only, not Controllers**~~ ✅ done 2026-10-13. No sdk difference
  between controllers and minimal api found. New controller actions in `PersonController` (twins of the MinimalApi
  samples): `GET|POST persons/echo-query`, `GET|POST persons/echo-authorization`, `persons/requires-token` (all
  verbs), `POST|OPTIONS persons/echo-request`; `ApiTestBase.CreateClient()`. Tests
  `src/Controllers.Test/Api/Persons/V1/Echo/*_Fluent.cs` (27: query 8 incl. `[FromQuery]` binding, bearer 9,
  OPTIONS 4, cancellation 6). The controllers host does not send `api-supported-versions` (sample config). Counter-check
  (encoding off, fluent request headers dropped, OPTIONS content headers dropped, token not forwarded): 16 of 27 red —
  3/8, 6/9, 3/4, 4/6; the green ones are must-fail partners and live-token cases that do not depend on the break.

Legacy features advertised but without any test — probed 2026-10-13, **each waits for the user's decision**
(secure / repair / stop advertising):

- **G4 — `DynamicRequestLocator` works.** Probe `src/MinimalApi.Test/Probes/DynamicRequestLocator/Status_200_Ok/`
  (9 cases green): one case per `Requests/*.json`, classic and fluent with ONE file name for payload and expected
  (body from `Requests/`, snapshot from `Responses/` — the response file carries a field the request lacks), extra
  attribute parameters follow the file name, crossed request/response must fail. Counter-check (class name not
  stripped from the namespace): 5 of 5 test methods red.
- **G5 — TestCreator works for CONTROLLERS only.** Probe `src/Controllers.Test/Probes/TestCreatorProbeTests.cs`
  (3 green, own host via `IStartupFilter`, no `AddAssertableHttpClient` → does not take over the static provider):
  GET → `Client.AssertGetAsync<Person>("/api/v1/persons/1", …)`, POST with payload, 404 →
  `AssertGetAsErrorAsync<ProblemDetails>`. Counter-check (creator returns empty): 3 of 3 red. A minimal api
  endpoint generates NOTHING (temporary probe: 200, 0 outputs — `TestCreatorMiddleware` needs a
  `ControllerActionDescriptor`); the README does not say so. It generates CLASSIC code only, no fluent chain.
- **G6 — OutputMode `Ai` works, `Hybrid` has a bug.** Temporary probe (one `dotnet test` process per
  `TestSdkSettings__OutputMode`; a second host in-process would take over the static provider for every parallel test):
  `Ai` = valid JSON with `HTTP_SNAPSHOT_MISMATCH`, differences, suggested fix. Deviations: `class` is
  assembly + file name (`MinimalApi.Test.OutputModeProbeTests`, namespace segment lost), `httpFailureType` is the enum
  NUMBER (README shows `"SnapshotMismatch"`), `actualStatusCode` is `null` on a snapshot mismatch
  (`AssertableHttpClient.cs:268`). `Hybrid`: the separator prints `System.Linq.Enumerable+RepeatIterator`1[System.Char]`
  twice (`HybridModeRenderStrategy.cs:40`, `'='.Repeat(80)` interpolated as an enumerable); the heading differs from
  the README. No permanent test yet — needs a per-process mode switch (decision).

## Deliberately NOT ported (do not reopen)

- `AssertUrlExists` stays classic (not a chain).
- `new {}` C# code generation stays classic (marked PROTOTYPE in the engine; untested for fluent).
- One `Produces` per chain and `ExecuteAsync()` as the single terminal — fixed in the decision log.

## Recommended order

~~Commit → 1 + 2 → 5 → 3 → 4 → 6 → Builder review → 7 → open review findings → 8~~ (done) → **Priority 3** →
cleanup.
