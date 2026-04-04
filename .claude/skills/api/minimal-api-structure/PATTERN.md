# Minimal API Structure

## Objective

Keep Minimal APIs modular, self-contained, and easy to extend.

Use service-only registration, self-mapping endpoints, and action-focused horizontal slicing where this repository follows the new structure.

## Apply These Rules

- Register API domains through service collection extensions only.
- Do not require separate `MapXyz()` calls in startup when the solution uses the new structure.
- Let each endpoint register itself through DI and map itself through `IEndpoint`.
- Keep registration flow hierarchical: API → domain → version → action endpoint.
- Group actions by domain and API version.
- Keep each action self-contained with its endpoint, request/response models, and command/query logic nearby.
- Prefer horizontal slicing by action when the solution uses the new structure.
- Do not introduce horizontal slicing into a solution that still intentionally follows the older vertical slicing structure.
- Do not mix old and new structures within the same solution.
- If migration is not planned or not feasible, continue using the existing structure consistently.

## Avoid

- Dual registration with both `AddXyz()` and `MapXyz()` in the new structure
- Central endpoint mapping that must be manually updated for every action
- Mixing vertical slicing and horizontal slicing in the same solution
- Spreading one action across many unrelated folders
- Hiding endpoint mapping logic outside the endpoint implementation
- Forcing migration patterns into a solution that is intentionally staying on the old structure

## Review Checklist

- Does startup register domains through services only?
- Does each endpoint register itself as `IEndpoint`?
- Does each endpoint implement its own `Map(...)` logic?
- Is registration organized from domain to version to action?
- Is the action self-contained and easy to navigate?
- Does the solution consistently follow either the old or the new structure?
- Has the code avoided mixing vertical and horizontal slicing?

## Notes

The preferred new model is:

- service-only registration
- endpoint self-mapping through `IEndpoint`
- horizontal slicing by action within domain and version

However, consistency is more important than partial migration.
If a solution already uses the older vertical slicing approach and cannot be migrated, keep extending that existing structure instead of mixing models.
