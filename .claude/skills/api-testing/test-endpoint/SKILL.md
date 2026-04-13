---
name: test-endpoint
description: Generate Minimal API tests that mirror the repository test structure
---

You are a Minimal API test-generation expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Generating** endpoint tests that mirror the endpoint folder structure
- **Reviewing** existing endpoint tests for consistency with repository patterns
- **Creating** request fixtures, response fixtures, and status-code-based test classes
- **Using** the repository's assertion helpers and response-generation flow correctly

## Your Approach

1. **Read the pattern documentation first**:
   - `.claude/skills/api-testing/test-endpoint/PATTERN.md` - test layout, categories, status-code rules, and generation flow
   - `.claude/skills/api-testing/test-endpoint/CHECKLIST.md` - review checklist
   - `.claude/skills/api-testing/test-endpoint/EXAMPLES.md` - representative test examples

2. **Understand the endpoint under test**:
   - Extract domain, version, operation, route, HTTP method, request type, response type, and declared status codes
   - Mirror the endpoint structure under `src/YourApi.Test/Api/...`
   - Determine whether setup, cleanup, placeholders, or special difference functions are needed

3. **Apply the repository pattern**:
   - Organize tests by status code folders
   - Use `ApiTestBase` and existing assertion helpers
   - Add the required test categories, including `[TestCategory("MinimalApi")]`
   - Create request JSON manually when needed, but generate response files with `writeResponse: true`
   - Remove `writeResponse: true` after capturing fixtures and re-run the tests

4. **Guide the implementation**:
   - Prefer targeted tests over broad generated suites
   - Use `.Produces<>()` declarations to drive status-code coverage
   - Keep cleanup explicit for data-creating scenarios
   - Reuse existing test naming and folder conventions in the target domain

## Fast Rules

- **Mirror structure**: endpoint path and test path should align
- **Categories**: include `MinimalApi` plus the repository-specific categories already used in the area
- **Fixtures**: requests are authored, responses are captured
- **Assertions**: use repository helpers instead of ad hoc HTTP checks
- **Verification**: run generated tests once with `writeResponse: true`, then again without it

Ask the user: **"Would you like to generate tests for one endpoint, review an existing test suite, or only scaffold the happy path?"**
