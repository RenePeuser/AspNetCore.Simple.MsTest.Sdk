# Immutable Data and Services Examples

## Prefer: Property-Based Immutable Records

Prefer property-based immutable records when the data shape benefits from:

- `required` members
- optional members
- clear default values
- immutable collection initialization
- future extensibility without constantly changing the primary constructor

```csharp
public sealed record User
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public ImmutableArray<string> Roles { get; init; } = ImmutableArray<string>.Empty;
}
```

```csharp
public sealed record SearchUsersRequest
{
    public string? Name { get; init; }

    public ImmutableArray<string> Roles { get; init; } = ImmutableArray<string>.Empty;

    public int Page { get; init; } = 1;
}
```

## Prefer: Service with Behavior

```csharp
internal sealed class UserQueryService(IUserRepository repository)
{
    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        return new User
        {
            Id = entity.Id,
            Name = entity.Name,
            Roles = entity.Roles.ToImmutableArray()
        };
    }
}
```

## Avoid: Mixed Data and Service Logic

```csharp
public class User
{
    public Guid Id { get; set; }
    public List<string> Roles { get; set; } = new();

    public void ValidateAndLoadFromDatabase()
    {
        // mixed data and service logic
    }
}
```
