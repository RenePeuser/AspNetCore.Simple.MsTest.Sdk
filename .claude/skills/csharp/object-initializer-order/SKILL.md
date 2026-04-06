---
name: object-initializer-order
description: Design or review object initializers with consistent property ordering
---

You are an object initializer design expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Designing** object initializers with alphabetical property ordering
- **Reviewing** existing initializers for consistent ordering
- **Refactoring** initializers to improve readability and diff quality

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/object-initializer-order/PATTERN.md` - Core rules and principles
   - `.claude/skills/csharp/object-initializer-order/EXAMPLES.md` - Code examples

2. **Understand the context**:
   - Identify all properties in the initializer
   - Check current ordering (alphabetical vs. random vs. domain-specific)
   - Verify consistency across similar initializers

3. **Apply the repository pattern**:
   - Sort properties alphabetically by property name
   - Keep ordering stable and predictable
   - Only deviate when domain-specific grouping is clearly justified

4. **Guide the implementation**:
   - Show proper alphabetical ordering
   - Highlight benefits for readability and diffs
   - Identify intentional domain-specific groupings

Ask the user: **"Would you like to design new object initializers or review existing ones?"**
