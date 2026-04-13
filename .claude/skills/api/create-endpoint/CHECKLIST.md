# Create Endpoint Checklist

Use this checklist when creating or reviewing an endpoint operation.

## Structure

- [ ] The operation is placed in the correct cloud or core folder structure
- [ ] Request and response DTOs are placed in `src/YourApi.Contracts`
- [ ] The operation contains the right components for the HTTP method
- [ ] `Summary.md` and `Description.md` exist when the endpoint is exposed through OpenAPI

## Code Shape

- [ ] The endpoint implements `IEndpoint`
- [ ] The endpoint registration extension is in the same file as the endpoint
- [ ] The command or query registration extension is in the same file as the implementation
- [ ] Commands and queries use `ExecuteAsync(..., CancellationToken cancellationToken)`
- [ ] `CancellationToken` and `.ConfigureAwait(false)` are used consistently where applicable

## Contracts and Validation

- [ ] Request DTOs use the existing validation attributes where appropriate
- [ ] Request and response contracts use repository naming conventions
- [ ] Collections use immutable collection types where appropriate
- [ ] Sample values or sample value providers are added where needed for OpenAPI examples

## Registration

- [ ] The operation is wired explicitly in the correct `Startup.cs`
- [ ] Operation startup only orchestrates registrations
- [ ] No registration step is hidden or inferred implicitly

## Safety

- [ ] Business logic is only implemented if the user requested it
- [ ] Scaffolding uses `NotImplementedException` instead of invented behavior when needed
- [ ] The resulting operation matches existing patterns in the target domain
