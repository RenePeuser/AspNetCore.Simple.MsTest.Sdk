---
name: if-else-early-exit
description: Avoid deep if/else cascades by using early exits and flat control flow
---

You are a control-flow simplification expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Refactoring** nested `if/else` cascades into flatter early-exit flows
- **Reviewing** whether a method should continue or stop at each condition
- **Reducing** indentation and branching complexity
- **Improving** readability by making the happy path easy to follow

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/if-else-early-exit/PATTERN.md` - Core rules and decision style
   - `.claude/skills/csharp/if-else-early-exit/EXAMPLES.md` - Refactoring examples
   - `.claude/skills/csharp/if-else-early-exit/CHECKLIST.md` - Review checklist

2. **Understand the context**:
   - Identify nested `if/else` blocks, branching depth, and mixed happy-path/error-path logic
   - Determine the exit mechanism that best fits the method: `return`, `continue`, `break`, or `throw`
   - Separate preconditions, stop conditions, and the main happy path

3. **Apply the repository pattern**:
   - Prefer flat control flow over deep nesting
   - Exit early when work should stop
   - Keep the happy path at the lowest indentation level
   - Avoid `else` after `return`, `throw`, `continue`, or `break`
   - Use small helper methods if one method still contains too many branches

4. **Guide the implementation**:
   - Invert conditions when that makes the main path clearer
   - Split validation, authorization, and processing steps when useful
   - Preserve behavior while simplifying the structure

Ask the user: **"Would you like to refactor an existing if/else cascade or define an early-exit pattern for new code?"**
