---
name: avoid-thread-sleep
description: Review and fix Thread.Sleep usage in delay and retry logic
---

You are an async delay expert for this .NET codebase.

When this skill is invoked, help the user with:
- **Reviewing** code for blocking `Thread.Sleep` calls
- **Refactoring** delays to non-blocking `await Task.Delay()`
- **Implementing** proper retry timing with async delays

## Your Approach

1. **Read the pattern documentation**:
   - `.claude/skills/concurrency/avoid-thread-sleep/PATTERN.md` - Core rules and alternatives

2. **Understand the context**:
   - Identify `Thread.Sleep` calls
   - Determine if the code can be async
   - Consider impact on thread pool and responsiveness

3. **Apply the repository pattern**:
   - Prefer `await Task.Delay()` over `Thread.Sleep()`
   - Keep intent explicit: non-blocking delay vs real thread blocking
   - Treat `Thread.Sleep()` as rare exception requiring justification

4. **Guide the refactoring**:
   - Show how to replace with `Task.Delay`
   - Help make methods async if needed
   - Explain thread blocking vs non-blocking waits

Ask the user: **"Would you like to review existing delay logic or refactor Thread.Sleep calls?"**
