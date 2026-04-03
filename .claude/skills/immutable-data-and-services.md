---
name: immutable-data-and-services
description: Use when designing or reviewing data models, services, and collection types in this .NET codebase.
---

# Immutable Data and Services

Use this skill when designing or reviewing data types, service types, and collection usage.

## Objective

Separate data and behavior clearly.

Use immutable records for pure data.
Use classes for services and behavior.

Prefer immutable collection types when the code should model fixed data, avoid accidental mutation, or communicate read-only intent clearly.

## Apply These Rules

- Use `record` for pure data objects that primarily carry values.
- Keep data records immutable by default.
- Prefer property-based record declarations over primary-constructor records when `required` members, optional values, or collection defaults are important.
- Do not place service logic, orchestration, side effects, or domain workflows into pure data records.
- Use `class` for services, handlers, coordinators, and other behavior-rich types with methods.
- Keep records focused on data shape and value transport.
- Keep services focused on logic and behavior.
- Do not blur the boundary by mixing rich behavior into data containers.
- Prefer immutable collection types when collection values should not change after creation.
- Choose one immutable collection type intentionally instead of mixing mutable and immutable styles without reason.

## Collection Guidance

Prefer immutable collections for stable data structures and outward-facing results.

Typical options include:

- `ImmutableArray<T>` for compact immutable ordered data
- `ImmutableList<T>` when list-style immutable updates are needed
- `ImmutableHashSet<T>` for immutable uniqueness
- `ImmutableDictionary<TKey, TValue>` for immutable key/value data

Use builders when constructing larger immutable collections efficiently.

## Prefer

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

## Avoid

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

## Review Checklist

- Is this type primarily data or primarily behavior?
- If it is pure data, should it be a `record`?
- Is the data type immutable by default?
- Would a property-based record be clearer here than a primary-constructor record?
- If it contains business logic or orchestration, should it be a `class` service instead?
- Does the collection need to be mutable, or should it be immutable?
- Is the chosen immutable collection type appropriate for the usage pattern?
- Is the code avoiding accidental mixing of transport/data concerns with service logic?

## Notes

Records are a good fit for data models with value equality and immutable design.
Classes remain the default for behavior-rich types.

Be careful with record members that are mutable reference types such as arrays or `List<T>`.
A record itself can be immutable in shape while still containing mutable internals.

For outward-facing APIs, query results, messages, and internal data transfer models, prefer immutable records and immutable collections unless there is a clear reason not to.
