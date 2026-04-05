# Pipeline Pattern Review Checklist

Use this checklist when reviewing pipeline implementations.

## Interface Design

- [ ] Step interface has a clear, focused contract (e.g., `Execute`)
- [ ] No `CanHandle` method in step interface (use Strategy Pattern if needed)
- [ ] Step interface accepts a context object with all required data
- [ ] Context object is immutable or treated as read-only by steps
- [ ] Step interface is generic if steps work with different result types

## Pipeline Orchestrator

- [ ] Pipeline injects all steps via `IEnumerable<IXxxStep>`
- [ ] Pipeline iterates through steps in registration order
- [ ] Pipeline has no selection logic - just executes all steps
- [ ] Pipeline handles exceptions appropriately (usually lets them bubble up for fail-fast)
- [ ] Pipeline returns the final result after all steps succeed

## Individual Steps

- [ ] Each step has a single, clear responsibility
- [ ] Steps return early if their validation/processing passes
- [ ] Steps fail fast with meaningful exceptions when validation fails
- [ ] Steps do not modify shared state or context in ways that affect other steps
- [ ] Steps do not have `CanHandle` logic (if they do, consider Strategy Pattern instead)
- [ ] Steps inject dependencies via constructor
- [ ] Step names clearly indicate what they validate or process

## Service Registration

- [ ] Steps are registered in execution order
- [ ] Execution order is documented with comments
- [ ] Each step has its own `AddXxxStep()` extension method
- [ ] Step registration extensions register step dependencies first, then the step itself
- [ ] Pipeline is registered after all steps
- [ ] Step registrations use `AddSingletonIfNotExists` to prevent duplicates

## Step Registration Extensions

- [ ] Each step has a dedicated registration extension
- [ ] Extension follows naming: `AddXxxStep`
- [ ] Extension registers dependencies before the step
- [ ] Extension uses `AddSingletonIfNotExists` for the step
- [ ] Extension includes XML documentation

## Error Handling

- [ ] Steps throw meaningful exceptions with context
- [ ] Error messages include what was expected vs. what was found
- [ ] Steps use repository-specific exception types when available
- [ ] Error messages are formatted for clear test failure output

## Testing

- [ ] Each step can be tested independently
- [ ] Tests verify step behavior with valid input (early return)
- [ ] Tests verify step behavior with invalid input (exception thrown)
- [ ] Pipeline integration tests verify execution order
- [ ] Tests verify fail-fast behavior (pipeline stops at first failure)

## Documentation

- [ ] Pipeline interface has clear XML documentation
- [ ] Step interface explains the execution contract
- [ ] Each step has XML documentation explaining its purpose
- [ ] Registration extension documents the execution order
- [ ] Comments indicate step sequence (1, 2, 3, etc.)

## When to Use Pipeline vs Strategy

- [ ] Verified that Pipeline Pattern is appropriate (multiple sequential operations)
- [ ] Considered Strategy Pattern if only one implementation should execute
- [ ] All steps should execute sequentially (not conditionally)
- [ ] Order of execution matters and is intentional
- [ ] No `CanHandle` logic is needed

## Common Issues to Avoid

- [ ] Steps do NOT have `CanHandle` logic
- [ ] Steps do NOT modify context in ways that affect other steps
- [ ] Steps do NOT fail silently - they throw exceptions
- [ ] Steps do NOT have multiple responsibilities
- [ ] Registration order is NOT arbitrary - it's intentional
- [ ] Pipeline does NOT have selection logic - it just iterates

## Performance Considerations

- [ ] Steps are registered as singletons (if stateless)
- [ ] Steps do not perform expensive operations unnecessarily
- [ ] Steps use early return to skip unnecessary work
- [ ] Context object doesn't carry excessive data

## Extension Scenarios

- [ ] Adding a new step requires only creating the step + registration
- [ ] No `CanHandle` logic needed for new steps
- [ ] Clear where in the sequence a new step should be inserted
- [ ] Existing steps don't need modification when adding new steps

## Pattern Consistency

- [ ] Follows repository's service registration pattern (feature-based DI)
- [ ] Uses repository's exception types
- [ ] Naming conventions match other pipelines in the codebase
- [ ] Code style matches existing steps
