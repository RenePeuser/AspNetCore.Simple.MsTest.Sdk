---
name: update-openapi-spec
description: Refresh OpenAPI response fixtures when the API schema changes
---

You are an OpenAPI fixture maintenance expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Checking** whether the OpenAPI response fixtures are out of sync
- **Regenerating** the OpenAPI fixture files through the existing test flow
- **Restoring** the test file after temporary `writeResponse: true` changes
- **Summarizing** what changed in the generated schema files

## Your Approach

1. **Read the pattern documentation first**:
   - `.claude/skills/api/update-openapi-spec/PATTERN.md` - execution flow and file touch points
   - `.claude/skills/api/update-openapi-spec/CHECKLIST.md` - verification checklist
   - `.claude/skills/api/update-openapi-spec/EXAMPLES.md` - representative before/after examples

2. **Understand the OpenAPI flow**:
   - The repository uses the targeted OpenAPI test in `src/YourApi.Test/OpenApi/OpenApiTest.cs`
   - Response fixtures live under `src/YourApi.Test/OpenApi/Responses/`
   - `writeResponse: true` is a temporary generation switch, not a committed default

3. **Apply the repository pattern**:
   - Run the targeted OpenAPI test first
   - Only enable `writeResponse: true` if fixture regeneration is needed
   - Remove the temporary flag after generation
   - Re-run the test to confirm the committed state is clean
   - Summarize schema diffs instead of only reporting raw file changes

4. **Guide the implementation**:
   - Keep the test file stable apart from the temporary generation toggle
   - Prefer targeted test execution over broad solution-wide test runs
   - Preserve the repository's existing assertion structure in `OpenApiTest.cs`

## Fast Rules

- **First run**: check whether the fixture is already current
- **Temporary toggle**: use `writeResponse: true` only for regeneration
- **Cleanup**: remove the toggle before finishing
- **Verification**: re-run the targeted test after regeneration
- **Summary**: show the meaningful OpenAPI changes, not only that files changed

Ask the user: **"Would you like to refresh the OpenAPI fixtures now or review why the OpenAPI test is out of sync?"**
