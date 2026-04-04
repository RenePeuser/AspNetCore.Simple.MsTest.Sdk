# Service Registration Review Checklist

Use this checklist when reviewing service registration code.

## Structure

- [ ] Is the registration feature-oriented (not flat technical blocks)?
- [ ] Does each service own its dependency registration?
- [ ] Is the `AddXxx()` extension in the same file as the service class?
- [ ] Is the registration hierarchy easy to navigate?

## Registration Order

- [ ] Are dependencies registered before the service?
- [ ] Are settings/configuration registered before the service when required?
- [ ] Is the primary service registered last?

## Dependencies

- [ ] Are dependencies registered through their own `AddXxx()` extensions?
- [ ] Is the dependency tree encapsulated (not exposed to consumers)?

## Safety

- [ ] Is `AddSingletonIfNotExists` used for idempotent registration?
- [ ] Is the registration safe to call multiple times?
- [ ] Would adding a new dependency require changes in only one place?

## Anti-Patterns

- [ ] No large flat registration blocks in startup
- [ ] No direct registration of implementation details from outside
- [ ] No separate extension files for registration
- [ ] No scattered registration logic
