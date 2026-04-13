# Update OpenAPI Spec Pattern

## Core Goal

Refresh the committed OpenAPI response fixtures through the existing targeted test flow without leaving temporary generation flags behind.

## Source Files

- Test entry point: `src/YourApi.Test/OpenApi/OpenApiTest.cs`
- Generated fixtures: `src/YourApi.Test/OpenApi/Responses/*.json`

## Normal Flow

1. Run the targeted OpenAPI test
2. If it passes, stop: the fixture is already current
3. If it fails due to schema drift, temporarily enable `writeResponse: true`
4. Run the same targeted test again to regenerate fixtures
5. Review the generated JSON changes
6. Remove `writeResponse: true`
7. Re-run the targeted test to confirm the clean committed state
8. Summarize the schema diff for the user

## Targeted Test Rule

Prefer the OpenAPI-specific test filter instead of running the whole test suite.

Typical command:
```powershell
dotnet test --filter "FullyQualifiedName~YourApi.Test.OpenApi.OpenApiTest"
```

## Temporary Toggle Rule

Expected call shape in `OpenApiTest.cs`:

```csharp
await Client.AssertGetAsync<object>($"openapi/v{major}.json",
                                    $"V{major}.json",
                                    differenceFunc: DifferenceFunc).ConfigureAwait(false);
```

Temporary regeneration form:

```csharp
await Client.AssertGetAsync<object>($"openapi/v{major}.json",
                                    $"V{major}.json",
                                    differenceFunc: DifferenceFunc,
                                    writeResponse: true).ConfigureAwait(false);
```

Rule:
- `writeResponse: true` is temporary
- Do not leave it committed after regeneration

## Review Rule

After regeneration, inspect the response fixture changes for meaningful API differences such as:
- Added endpoints
- Removed endpoints
- Changed routes
- Changed operation IDs, tags, request shapes, or response shapes
- Version-specific schema changes

## Commit Guidance Rule

If the fixtures changed because the API changed intentionally, summarize the change in user-facing terms.

Typical commit shapes:
- `chore(openapi): add ...`
- `chore(openapi): update ...`
- `chore(openapi): remove ...`
- `chore(openapi): bump spec`

## Error Handling Rule

If the test still fails after removing `writeResponse: true`:
- surface the failing output
- do not leave the temporary generation toggle in place
- guide the user toward the remaining schema or app-startup problem

## Common Pitfalls

- Running all tests instead of the targeted OpenAPI test
- Leaving `writeResponse: true` in the committed test file
- Treating raw JSON diff noise as the only result instead of summarizing API changes
- Forgetting to verify the final clean state after regeneration
