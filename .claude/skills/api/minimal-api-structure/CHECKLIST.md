# Minimal API Structure Review Checklist

Use this checklist when reviewing Minimal API structure.

## Registration

- [ ] Does startup register domains through services only?
- [ ] No separate `MapXyz()` calls required in startup?
- [ ] Is registration hierarchical: API → domain → version → action?

## Endpoints

- [ ] Does each endpoint register itself as `IEndpoint`?
- [ ] Does each endpoint implement its own `Map(...)` logic?
- [ ] Are endpoints self-contained with nearby models/commands/queries?

## Structure

- [ ] Is the action organized with horizontal slicing (by feature)?
- [ ] Is each action easy to locate and navigate?
- [ ] Are related files (endpoint, models, logic) grouped together?

## Consistency

- [ ] Does the solution follow ONE structure (old vs new)?
- [ ] No mixing of vertical and horizontal slicing?
- [ ] If migrating, is it complete (not partial)?

## Anti-Patterns

- [ ] No dual registration (AddXyz + MapXyz) in new structure
- [ ] No central mapping that requires manual updates per action
- [ ] No spreading one action across unrelated folders
- [ ] No hidden endpoint mapping outside endpoint implementation
