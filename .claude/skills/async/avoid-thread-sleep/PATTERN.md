# Avoid Thread.Sleep

## Objective

Avoid blocking waits with `Thread.Sleep(...)` in normal repository code.

Prefer non-blocking asynchronous delays with `await Task.Delay(...)` so the current thread is not unnecessarily blocked.

## Apply These Rules

- Do not use `Thread.Sleep(...)` as the default way to wait.
- Prefer `await Task.Delay(...)` in asynchronous code.
- Treat `Thread.Sleep(...)` as an exception that requires a clear technical reason.
- Be especially careful in UI code, server code, background workers, and thread-pool based code.
- Keep the intent explicit: non-blocking delay vs real thread blocking.

## Prefer

```csharp
await Task.Delay(TimeSpan.FromMilliseconds(1000));
```

## Avoid

```csharp
Thread.Sleep(TimeSpan.FromMilliseconds(1000));
```

## Review Checklist

- Is this wait really needed?
- Can the code use `await Task.Delay(...)` instead of blocking the current thread?
- Would `Thread.Sleep(...)` reduce responsiveness or scalability here?
- Is there a documented reason if `Thread.Sleep(...)` is still used?
- Is the waiting style consistent with the rest of the repository?

## Notes

`Thread.Sleep(...)` suspends the current thread for the specified time.
`await Task.Delay(...)` works with the async/await model and does not block the current thread while the delay is in progress.

Use `Thread.Sleep(...)` only in rare low-level or highly specific scenarios where real thread blocking is intentionally required.
