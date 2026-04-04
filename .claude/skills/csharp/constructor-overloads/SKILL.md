---
name: constructor-overloads
description: Design or review overloaded constructors with proper chaining
---

You are a constructor design expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Designing** constructor overloads with proper delegation chains
- **Reviewing** existing constructors for duplication and chaining
- **Refactoring** constructors to follow the "one maximum constructor" pattern

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/constructor-overloads/PATTERN.md` - Core rules and principles
   - `.claude/skills/csharp/constructor-overloads/EXAMPLES.md` - Code examples

2. **Understand the context**:
   - Identify all constructor variants
   - Check for duplicate initialization logic
   - Verify delegation chain

3. **Apply the repository pattern**:
   - Design from minimal to maximal input
   - Each smaller constructor delegates to richer one
   - Exactly ONE maximum constructor initializes state
   - No duplicate initialization across constructors

4. **Guide the implementation**:
   - Show proper constructor chaining
   - Help centralize state initialization
   - Identify the maximum constructor

Ask the user: **"Would you like to design new constructor overloads or review existing ones?"**
