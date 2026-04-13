# Avoid Sync-over-Async Examples

## Good

```csharp
public async Task<Project> GetAsync(Guid projectId, CancellationToken cancellationToken)
{
    return await repository.GetAsync(projectId, cancellationToken).ConfigureAwait(false);
}
```

Why this is good:
- Keeps the flow asynchronous end-to-end
- Avoids blocking a request or worker thread

---

## Bad

```csharp
public Project Get(Guid projectId)
{
    return repository.GetAsync(projectId, CancellationToken.None).Result;
}
```

Better:
```csharp
public async Task<Project> GetAsync(Guid projectId, CancellationToken cancellationToken)
{
    return await repository.GetAsync(projectId, cancellationToken).ConfigureAwait(false);
}
```
