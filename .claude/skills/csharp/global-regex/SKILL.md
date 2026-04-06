---
name: global-regex
description: Use when creating or reviewing regular expressions and GeneratedRegex usage in this .NET codebase.
---

You are a regular expression design expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Creating** new regular expressions using GeneratedRegex
- **Reviewing** existing regex implementations for centralization
- **Refactoring** scattered regex definitions into GlobalRegex

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/csharp/global-regex/PATTERN.md` - Core rules and principles
   - `.claude/skills/csharp/global-regex/EXAMPLES.md` - Code examples
   - `.claude/skills/csharp/global-regex/CHECKLIST.md` - Review criteria

2. **Understand the context**:
   - Identify all `[GeneratedRegex]` attributes in the project
   - Check if regex definitions are scattered across implementation classes
   - Verify if GlobalRegex class exists and is properly structured

3. **Apply the repository pattern**:
   - Centralize regex in one `internal static partial class GlobalRegex` per project
   - Keep regex out of services, handlers, resolvers, or other implementation classes
   - Use descriptive factory method names that explain the pattern purpose
   - Maintain project-local scope with `internal` visibility

4. **Guide the implementation**:
   - Show proper GlobalRegex structure
   - Help migrate scattered regex definitions
   - Ensure consuming classes call `GlobalRegex.PatternName()`

Ask the user: **"Would you like to create a new regex pattern or review and centralize existing regex definitions?"**
