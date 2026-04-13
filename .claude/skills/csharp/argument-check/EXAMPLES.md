# Argument Check Examples

## Good

```csharp
public sealed class ProjectService
{
    private readonly IProjectRepository repository;

    public ProjectService(IProjectRepository repository)
    {
        this.repository = Throw.IfNull(repository);
    }

    public Task<Project> GetAsync(string projectName, CancellationToken cancellationToken)
    {
        Throw.IfNullOrWhiteSpace(projectName);
        return repository.GetAsync(projectName, cancellationToken);
    }
}
```

Why this is good:
- Guard clauses are placed at clear boundaries
- Uses specific helpers instead of generic checks

---

## Bad

```csharp
if (repository is null)
{
    throw new ArgumentNullException(nameof(repository));
}
```

Better:
```csharp
this.repository = Throw.IfNull(repository);
```
