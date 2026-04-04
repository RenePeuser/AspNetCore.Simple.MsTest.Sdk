# Strategy Pattern Review Checklist

Use this checklist when reviewing Strategy Pattern implementations.

## Pattern Fit

- [ ] Are there already more than two meaningful cases?
- [ ] Is future growth of cases likely?
- [ ] Would a strategy remove branching from the orchestration layer?
- [ ] Is each strategy focused on one responsibility?

## Contract Design

- [ ] Is selection based on a clear `CanHandle(...)` rule or equivalent?
- [ ] Are the strategy interfaces well-defined and focused?

## Orchestrator

- [ ] Does the orchestrator enforce exactly one match?
- [ ] Does the orchestrator fail explicitly for zero matches?
- [ ] Does the orchestrator fail explicitly for multiple matches?
- [ ] Are exceptions detailed with type, title, message, and metadata?

## Strategy Implementation

- [ ] Do strategies defend themselves against invalid direct execution?
- [ ] Do strategies validate via `CanHandle` even if already selected?
- [ ] Are strategy exceptions using repository-specific exception types?
- [ ] Do exceptions include a useful type, a title, a meaningful detail, and structured metadata?

## Code Organization

- [ ] Are strategies registered via DI?
- [ ] Would a shared abstract base class reduce duplication across many strategies?
- [ ] Is the design more open for extension and less dependent on central modification?

## Anti-Patterns

- [ ] No growing `if/else` or `switch` blocks in orchestration
- [ ] No silent fallback behavior
- [ ] No generic exceptions when detailed ones are available
- [ ] No strategies that know about other strategies
