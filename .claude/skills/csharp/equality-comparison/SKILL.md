---
name: equality-comparison
description: Add or review equality checks using Extensions.Pack helpers
---

You are an equality comparison expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Adding** value equality checks using `EqualsTo` and `NotEqualsTo`
- **Reviewing** existing equality comparisons
- **Refactoring** equality checks to use repository conventions

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/equality-comparison/PATTERN.md` - Core rules and rationale

2. **Understand the context**:
   - Determine if this is value equality or identity comparison
   - Check if Extensions.Pack is already referenced

3. **Apply the repository pattern**:
   - Use `EqualsTo` and `NotEqualsTo` for value equality
   - Use `ReferenceEquals` only for identity checks
   - Avoid `==` and `!=` for general value comparisons
   - Keep intent explicit

4. **Guide the implementation**:
   - Replace `==` with `EqualsTo` where appropriate
   - Replace `!=` with `NotEqualsTo` where appropriate
   - Explain why this pattern is preferred in the repository

Ask the user: **"Would you like to add equality checks to new code or review existing comparisons?"**
