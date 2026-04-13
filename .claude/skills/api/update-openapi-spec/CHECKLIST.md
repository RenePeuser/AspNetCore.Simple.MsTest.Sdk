# Update OpenAPI Spec Checklist

Use this checklist when refreshing OpenAPI response fixtures.

## Execution

- [ ] The targeted OpenAPI test is run first to confirm whether regeneration is needed
- [ ] `writeResponse: true` is added only temporarily
- [ ] Only the expected OpenAPI test flow is used for fixture generation

## Files

- [ ] `src/YourApi.Test/OpenApi/OpenApiTest.cs` is restored to its normal committed form
- [ ] Updated fixture files are written under `src/YourApi.Test/OpenApi/Responses/`
- [ ] No unrelated test code changes are left behind

## Verification

- [ ] The targeted OpenAPI test is re-run after removing `writeResponse: true`
- [ ] The final test run passes cleanly, or remaining errors are reported clearly
- [ ] The resulting diff is summarized in API terms, not only as raw JSON changes

## Handoff

- [ ] The user receives a short summary of added, removed, or changed API surface
- [ ] A sensible conventional commit suggestion is provided when appropriate
