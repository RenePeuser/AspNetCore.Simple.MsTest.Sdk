# Update OpenAPI Spec Examples

## Example 1: Temporary Test Change

Before:
```csharp
await Client.AssertGetAsync<object>($"openapi/v{major}.json",
                                    $"V{major}.json",
                                    differenceFunc: DifferenceFunc).ConfigureAwait(false);
```

Temporary regeneration step:
```csharp
await Client.AssertGetAsync<object>($"openapi/v{major}.json",
                                    $"V{major}.json",
                                    differenceFunc: DifferenceFunc,
                                    writeResponse: true).ConfigureAwait(false);
```

Final committed state:
```csharp
await Client.AssertGetAsync<object>($"openapi/v{major}.json",
                                    $"V{major}.json",
                                    differenceFunc: DifferenceFunc).ConfigureAwait(false);
```

---

## Example 2: Targeted Test Command

```powershell
dotnet test --filter "FullyQualifiedName~YourApi.Test.OpenApi.OpenApiTest"
```

Why this is good:
- Runs only the OpenAPI verification path
- Keeps fixture refresh fast and focused

---

## Example 3: Good Change Summary

```text
OpenAPI fixtures updated.

Detected changes:
- Added endpoint: `PUT /admin/api-keys/apigee/usage-plans/{id}`
- Updated operation tag for `updateUsagePlanV1`
- Modified one request schema in `V1.json`

Suggested commit message:
chore(openapi): update admin usage plan schema
```

Why this is good:
- Summarizes API meaning, not only raw JSON edits
- Gives the user a commit message starting point
