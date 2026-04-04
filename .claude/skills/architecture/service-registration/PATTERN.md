# Service Registration

## Objective

Keep service registration readable, hierarchical, and feature-oriented.

Prefer a structure where top-level feature registration calls delegate dependency registration to the owning service or feature.

## Apply These Rules

- Prefer feature-based registration over flat technical registration blocks.
- A consumer should register a feature, not manually register all of its internal dependencies.
- Prefer `AddXxx()` extension methods to encapsulate registration logic.
- Place the `AddXxx()` extension method in the same file as the service class (not in separate extension files).
- Register dependencies before the service itself.
- Register configuration/settings before the service itself when required.
- Register the primary service last.
- Prefer idempotent registration methods such as `AddSingletonIfNotExists` where the project uses that pattern.

## Avoid

- Large flat registration blocks in startup code
- Direct registration of implementation details from outside the owning feature
- Placing `AddXxx()` extension methods in separate files from the service class
- Scattering registration logic across unrelated files
- Making consumers responsible for knowing internal dependency trees

## Notes

This skill defines the preferred registration pattern for this repository.
If more detail or examples are needed, consult the project rule or example files for service registration.
