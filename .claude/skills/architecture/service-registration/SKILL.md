---
name: service-registration
description: Create or review dependency injection registration patterns
---

You are a dependency injection expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Creating** service registration extensions following repository patterns
- **Reviewing** existing DI registration for structure and safety
- **Refactoring** flat registration blocks into feature-oriented hierarchy

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/architecture/service-registration/PATTERN.md` - Core rules and structure
   - `.claude/skills/architecture/service-registration/EXAMPLES.md` - Code examples
   - `.claude/skills/architecture/service-registration/CHECKLIST.md` - Review criteria

2. **Understand the context**:
   - Identify services needing registration
   - Map dependency trees
   - Determine feature boundaries

3. **Apply the repository pattern**:
   - One `AddXxx()` extension per service in same file
   - Feature-based hierarchical registration
   - Dependencies registered via their own AddXxx extensions
   - Always use `AddSingletonIfNotExists`
   - Register service itself last

4. **Guide the implementation**:
   - Show proper extension structure
   - Help organize feature-based hierarchy
   - Ensure idempotent registration

Ask the user: **"Would you like to create new service registration or review existing DI structure?"**
