# Strategy Pattern

## Objective

Use the Strategy Pattern when behavior varies by case and the system is expected to grow.

By default, strategy resolution must be strict:

- exactly one strategy must match
- zero matches must fail explicitly
- multiple matches must fail explicitly

The default repository model is not silent fallback behavior.

## Apply These Rules

- Consider the Strategy Pattern when there are already more than two meaningful cases.
- Consider it earlier when future extension is likely.
- Prefer strategies over growing `if/else` or `switch` logic when behavior should be open for extension.
- Keep orchestration separate from case-specific behavior.
- Use a clear contract such as `CanHandle(...)` and `Execute(...)` or a domain-specific equivalent.
- Register strategies via DI and resolve them centrally.
- By default, strategy resolution must be strict: exactly one strategy must match.
- If no strategy matches, fail explicitly.
- If more than one strategy matches, fail explicitly.
- Do not introduce silent fallback behavior unless the use case explicitly requires it.
- Strategies should defensively validate that they are actually allowed to handle the given input, even if the orchestrator already selected them.
- Introduce a specific abstract base class when many strategies share repeated mechanics such as type checks, casting, guards, or shared helpers.

## When a Specific Base Class Makes Sense

Introduce a specific abstract base class when many strategies repeat the same mechanics.

Typical examples:

- type checks
- casting
- guard clauses
- shared validation
- shared helper methods
- repeated exception-building logic

The base class should reduce duplication without hiding the strategy's actual decision logic.

## Exception Guidance

Prefer a specific repository exception type for invalid strategy resolution or invalid strategy execution.

Examples:

- `InternalServerErrorDetailsException`
- another repository-specific exception that can carry title, detail, and structured metadata

Avoid throwing plain `InvalidOperationException` when the repository already has a richer internal error type.

### Exception Rules

- The exception type should communicate that this is an internal strategy-resolution or strategy-execution failure.
- The exception should include both a short title and a meaningful detail message.
- The exception should include structured metadata whenever possible.
- The detail message should explain what was expected and what was actually found.
- Metadata should help debugging without forcing the reader to inspect the code first.

### Good Exception Content

Include details such as:

- requested type
- strategy type
- matching strategy count
- matched strategy names
- supported types
- relevant status code or mode
- other key selection inputs

## Avoid

- Growing `if/else` chains for case-specific behavior
- Large `switch` statements that must be edited for every new case
- Central services that know every implementation detail
- Silent fallback behavior in the default strategy model
- Returning without a result when exactly one strategy is expected
- Silent ambiguity when multiple strategies match unexpectedly
- Generic exceptions when a richer repository exception type exists
- Exceptions that only contain a title but no useful detail or metadata

## Notes

The default strategy model in this repository is:

- one orchestrator
- many specific strategies
- one clear selection rule
- exactly one match
- explicit failure for zero or multiple matches
- defensive strategy execution checks
- detailed repository exceptions
- optional abstract base class for repeated mechanics

Use the Strategy Pattern to support the Open/Closed Principle:
new behavior should usually be added by introducing a new strategy, not by modifying a growing central `if/else` or `switch`.
