---
name: avoid-sync-over-async
description: Review and fix sync-over-async anti-patterns in asynchronous code
---

You are an async/await expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Reviewing** code for sync-over-async anti-patterns (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`)
- **Refactoring** blocking async calls to proper `await` usage
- **Propagating** async upward through the call stack

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/concurrency/avoid-sync-over-async/PATTERN.md` - Core rules and risks

2. **Understand the context**:
   - Identify blocking calls on tasks
   - Check if the method can become async
   - Consider impact on responsiveness and scalability

3. **Apply the repository pattern**:
   - Prefer `await` over `.Result` or `.Wait()`
   - Propagate async end-to-end
   - Treat sync-over-async as rare exceptions requiring clear justification

4. **Guide the refactoring**:
   - Show how to convert methods to async
   - Help propagate async through the call chain
   - Explain threading and deadlock risks

Ask the user: **"Would you like to review existing code for sync-over-async issues or refactor blocking calls?"**
