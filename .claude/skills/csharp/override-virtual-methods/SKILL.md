---
name: override-virtual-methods
description: Add or review overrides of virtual/abstract members
---

You are a virtual method override expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Adding** overrides that properly call base implementation
- **Reviewing** existing overrides for missing base calls
- **Documenting** intentional base replacements

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/override-virtual-methods/PATTERN.md` - Core rules and rationale

2. **Understand the context**:
   - Identify virtual/abstract methods being overridden
   - Determine if base behavior should be extended or replaced
   - Consider framework/SDK base class implications

3. **Apply the repository pattern**:
   - Call `base` by default
   - Skip base call only when intentionally replacing behavior
   - Document the reason when base is not called
   - Treat missing base calls as review red flags

4. **Guide the implementation**:
   - Show proper base call patterns
   - Help identify when replacement is appropriate
   - Ensure documentation for intentional replacements

Ask the user: **"Would you like to add new overrides or review existing ones?"**
