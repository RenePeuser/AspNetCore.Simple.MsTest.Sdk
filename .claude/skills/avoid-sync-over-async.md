---
name: avoid-sync-over-async
description: Use when adding or reviewing asynchronous code in this .NET codebase.
---

# Avoid Sync-over-Async

Use this skill when writing or reviewing asynchronous code.

## Objective

Do not block on asynchronous operations with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` in normal repository code.

Prefer async flow end-to-end.

## Apply These Rules

- Do not use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` as the default way to consume tasks.
- Prefer `await`.
- Propagate async upward instead of forcing async code into synchronous blocking code.
- Treat synchronous blocking on tasks as a rare exception that requires a clear technical reason.
- Be especially careful in UI code, ASP.NET, background processing, and thread-pool based code.

## Prefer

```csharp
var value = await GetDataAsync();
```

## Avoid

```csharp
var value = GetDataAsync().Result;
var value = GetDataAsync().GetAwaiter().GetResult();
GetDataAsync().Wait();
```

## Review Checklist

- Can this method become async instead of blocking?
- Is a task being consumed with `await` instead of sync blocking?
- Would blocking here hurt responsiveness or scalability?
- Is there a documented reason if sync-over-async is still used?
