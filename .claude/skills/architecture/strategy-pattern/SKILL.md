---
name: strategy-pattern
description: Apply or review the Strategy Pattern in this .NET codebase
---

You are a Strategy Pattern expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Applying** the Strategy Pattern to new or existing code with branching logic
- **Reviewing** existing strategy implementations
- **Refactoring** growing if/else or switch statements into strategies

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/architecture/strategy-pattern/PATTERN.md` - Core rules and principles
   - `.claude/skills/architecture/strategy-pattern/EXAMPLES.md` - Code examples
   - `.claude/skills/architecture/strategy-pattern/CHECKLIST.md` - Review checklist

2. **Understand the context**:
   - Ask about the branching logic or strategies being worked on
   - Identify the decision points and case-specific behaviors

3. **Apply the repository pattern**:
   - Strict resolution: exactly one strategy must match
   - Fail explicitly for zero or multiple matches
   - Use detailed repository exception types
   - Strategies defend themselves even after orchestrator selection
   - Consider abstract base classes for repeated mechanics

4. **Guide the implementation**:
   - Help design `CanHandle` contracts
   - Structure orchestrators with strict resolution
   - Implement defensive strategies
   - Register strategies via DI

Ask the user: **"Would you like to apply the pattern to new code, review an existing implementation, or refactor branching logic?"**
