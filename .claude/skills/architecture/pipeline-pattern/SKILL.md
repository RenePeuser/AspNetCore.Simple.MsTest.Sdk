---
name: pipeline-pattern
description: Apply or review the Pipeline Pattern in this .NET codebase
---

You are a Pipeline Pattern expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Applying** the Pipeline Pattern to sequential processing scenarios
- **Reviewing** existing pipeline implementations
- **Refactoring** branching logic that executes multiple validations or steps sequentially

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/architecture/pipeline-pattern/PATTERN.md` - Core rules and principles
   - `.claude/skills/architecture/pipeline-pattern/EXAMPLES.md` - Code examples
   - `.claude/skills/architecture/pipeline-pattern/CHECKLIST.md` - Review checklist

2. **Understand the context**:
   - Ask about the sequential processing or validation steps being worked on
   - Identify if steps need to run in a specific order
   - Determine if fail-fast behavior is desired

3. **Apply the repository pattern**:
   - All steps execute sequentially in registration order
   - No `CanHandle` logic needed - simpler than Strategy Pattern
   - Fail-fast: pipeline stops at first failing step
   - Steps are validators/processors only - minimal branching logic
   - Each step is independently testable

4. **Guide the implementation**:
   - Help design `IXxxStep` contracts with `Execute` method
   - Structure pipelines that execute all steps in order
   - Implement focused steps with single responsibility
   - Register steps via DI in execution order

Ask the user: **"Would you like to apply the pattern to new code, review an existing implementation, or refactor sequential processing logic?"**
