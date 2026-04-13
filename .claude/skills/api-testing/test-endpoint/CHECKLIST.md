# Test Endpoint Checklist

Use this checklist when generating or reviewing endpoint tests.

## Structure

- [ ] The test path mirrors the endpoint path under `src/YourApi.Test/Api/...`
- [ ] Tests are organized by status-code folders
- [ ] Only the relevant status-code suites are generated

## Test Classes

- [ ] Each test class inherits from `ApiTestBase`
- [ ] Each Minimal API test class includes `[TestCategory("MinimalApi")]`
- [ ] Existing MVP, domain, and contextual categories are preserved where applicable
- [ ] Test names and file names follow the existing repository conventions

## Fixtures and Assertions

- [ ] Request JSON files are created only for body-based operations
- [ ] Response fixtures are generated with `writeResponse: true` rather than hand-authored when applicable
- [ ] `writeResponse: true` is removed before the final committed version
- [ ] Assertion helpers from the repository are used instead of ad hoc HTTP checks
- [ ] Difference functions are used when auto-generated values should be ignored

## Setup and Cleanup

- [ ] Setup is explicit only when the scenario needs it
- [ ] Cleanup is explicit for data-creating scenarios
- [ ] Test data uses placeholders or unique values where needed

## Verification

- [ ] The generated tests run successfully after fixture capture
- [ ] The final re-run without `writeResponse: true` passes cleanly
- [ ] The resulting tests are focused, readable, and aligned with the endpoint metadata
