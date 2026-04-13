# Test Endpoint Pattern

## Core Goal

Generate endpoint tests that mirror the API operation structure and use the repository's test infrastructure consistently.

## Test Location Mirrors Endpoint Location

Endpoint:
```
src/YourApi/Api/[Domain]/V[Version]/[Operation]/Endpoints/[Operation][Domain]Endpoint.cs
```

Tests:
```
src/YourApi.Test/Api/[Domain]/V[Version]/[Operation]/
```

Under the operation, organize tests by status code:
```
Status_200_Ok/
Status_400_BadRequest/
Status_401_Unauthorized/
Status_403_Forbidden/
Status_404_NotFound/
Status_422_UnprocessableContent/
```

## Coverage Source of Truth

Use the endpoint file to determine:
- HTTP method from `MapGet`, `MapPost`, `MapPut`, `MapPatch`, `MapDelete`
- Route pattern from the route mapping call
- Request type from the handler signature for body-based methods
- Response type and error shapes from `.Produces<>()`

Rule:
- Generate only the status-code suites that are actually declared or intentionally expected

## Test Class Rules

- Inherit from `ApiTestBase`
- Use MSTest attributes
- Add `[TestCategory("MinimalApi")]`
- Add the existing repository-specific categories already used in the area, such as MVP or domain categories
- Keep one test class per status-code folder
- Use repository naming conventions for namespaces, files, and test classes

## Assertion Rules

Prefer repository helpers over raw client code.

Typical helpers:
- `AssertGetAsync<TResponse>`
- `AssertPostAsync<TResponse>`
- `AssertPutAsync<TResponse>`
- `AssertPatchAsync<TResponse>`
- `AssertDeleteAsync`
- `AssertGetAsErrorAsync<TProblemDetails>`
- Unauthorized helpers such as `AssertGetAsUnauthorizedAsync`

Use `differenceFunc:` when auto-generated values should be ignored.

## Fixture Rules

### Requests

- Create request JSON files manually for `POST`, `PUT`, and `PATCH`
- Use placeholders for dynamic values
- Keep request shapes aligned with the contract DTOs

### Responses

- Do not author response fixtures by hand when the repository flow supports generation
- Temporarily use `writeResponse: true` to capture the actual response
- Remove `writeResponse: true` after the fixture is generated
- Re-run the same tests to verify the final committed version passes cleanly

## Status-Code Guidance

- `200` - happy path and successful variants
- `400` - invalid query or transport-level request issues
- `401` - caller is not authenticated
- `403` - caller is authenticated but forbidden, or a guarded input pattern is blocked in that area
- `404` - resource does not exist
- `422` - body is syntactically valid but semantically invalid or violates business rules

Rule:
- Use `422` only when it matches the repository behavior for body validation and business rules
- Do not force every endpoint to have every status-code suite

## Setup and Cleanup

- Use `TestClient` sub-clients for setup and cleanup when available
- Keep test data isolated with placeholders and unique values
- Override `TestInitializeAsync` and `TestCleanupAsync` only when needed
- Call the base implementations in the correct order

## Naming Rules

- Test class: `{Operation}Status{StatusCode}{StatusName}Test`
- Test method: `Should_...`
- Request and response file names should describe the scenario
- Namespace should follow the mirrored status-code folder structure

## Generation Flow

1. Read the endpoint
2. Determine status-code coverage
3. Create the mirrored test folders
4. Scaffold request files where needed
5. Generate the test classes and methods
6. Add `writeResponse: true` temporarily
7. Run the targeted tests to capture response fixtures
8. Remove `writeResponse: true`
9. Re-run the targeted tests and keep only the stable fixtures

## Common Pitfalls

- Missing `[TestCategory("MinimalApi")]`
- Writing response fixtures manually when they should be captured
- Forgetting to remove `writeResponse: true`
- Mixing multiple status-code concerns into one file
- Skipping cleanup for create or update flows
- Using raw HTTP assertions when repository helpers already exist
