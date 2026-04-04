# Non-Null Collections

## Objective

Collection and list types must never be `null`.

Always use an empty collection to represent "no items".
Do not use `null` to represent an empty collection.

## Apply These Rules

- Never expose collection or list types as `null`.
- Initialize collection properties with an empty default.
- Return empty collections instead of `null`.
- Accepting `null` collections from external input may require normalization, but internal code must not keep them as `null`.
- Treat `null` and empty as different concepts: `null` means missing reference, empty means no items.
- Prefer contracts that make collection usage safe without requiring repeated null checks.
- Apply this rule consistently to mutable and immutable collection types.

## Preferred Defaults

```csharp
public sealed record User
{
    public ImmutableArray<string> Roles { get; init; } = ImmutableArray<string>.Empty;
}
```

```csharp
public sealed record SearchResponse
{
    public ImmutableList<ResultItem> Items { get; init; } = ImmutableList<ResultItem>.Empty;
}
```

```csharp
public sealed class Example
{
    public List<string> Values { get; init; } = [];
}
```

## Normalize External Input

```csharp
public sealed record CreateUserRequest
{
    public ImmutableArray<string> Roles { get; init; } = ImmutableArray<string>.Empty;
}

var request = new CreateUserRequest
{
    Roles = incomingRoles?.ToImmutableArray() ?? ImmutableArray<string>.Empty
};
```

## Return Empty Collections

```csharp
public ImmutableArray<User> GetUsers()
{
    return ImmutableArray<User>.Empty;
}
```

## Avoid

```csharp
public List<string>? Values { get; init; }
public ImmutableArray<string>? Roles { get; init; }

return null;
```

## Review Checklist

- Can this collection ever be `null`?
- Does the property have a safe empty default?
- Does the method return an empty collection instead of `null`?
- Is external input normalized before entering internal code?
- Is the chosen collection type consistent with the rest of the repository?
- Would consumers need unnecessary null checks because of this API shape?

## Notes

This repository treats collections as always-present values.
"No items" must be represented by an empty collection, not by `null`.

This improves safety, reduces defensive noise, and makes contracts easier to understand and consume.
