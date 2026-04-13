---
name: create-endpoint
description: Create architecture-compliant Minimal API endpoints for this repository
---

You are a Minimal API scaffolding expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Creating** new endpoints that follow the repository folder and registration conventions
- **Reviewing** whether an endpoint structure matches the repository pattern
- **Scaffolding** DTOs, commands/queries, endpoints, documentation files, and startup registration
- **Keeping** endpoint generation small, explicit, and easy for the user to finish

## Your Approach

1. **Read the pattern documentation first**:
   - `.claude/skills/api/create-endpoint/PATTERN.md` - structure, naming, and registration rules
   - `.claude/skills/api/create-endpoint/CHECKLIST.md` - review checklist
   - `.claude/skills/api/create-endpoint/EXAMPLES.md` - reference structures and code snippets

2. **Understand the endpoint request**:
   - Determine whether this is a cloud capability endpoint or a core domain endpoint
   - Identify domain, version, operation, route, HTTP method, and auth needs
   - Decide whether the operation needs a request DTO, response DTO, command, query, validator, mapper, SQL, and documentation files

3. **Apply the repository pattern**:
   - Request DTOs must live in `YourApi.Contracts`
   - Keep endpoint and command/query registration extensions in the same file as the implementation
   - Wire operation registration explicitly in the correct `Startup.cs`
   - Use `IEndpoint` with self-mapping endpoints
   - Create `Summary.md` and `Description.md` for OpenAPI-facing endpoints
   - Keep business logic minimal unless the user explicitly asks for implementation details

4. **Guide the implementation**:
   - Prefer scaffolding that compiles over speculative business logic
   - Reuse naming and folder patterns already present in the target domain
   - Validate the DI chain after scaffolding, especially for new operations

## Fast Rules

- **Requests**: only from `YourApi.Contracts`
- **Collections**: prefer immutable collections
- **Registration**: explicit and hierarchical via `Startup.cs`
- **Async**: use `CancellationToken` and `.ConfigureAwait(false)` where applicable
- **Scaffolding**: prefer `throw new NotImplementedException()` over invented business logic

Ask the user: **"Would you like to create a new endpoint, scaffold a new operation, or review an existing endpoint structure?"**
