# Avoid Thread.Sleep Examples

## Good

```csharp
await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
```

Why this is good:
- Does not block the current thread
- Fits naturally into async flows

---

## Bad

```csharp
Thread.Sleep(TimeSpan.FromSeconds(1));
```

Better:
```csharp
await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
```
