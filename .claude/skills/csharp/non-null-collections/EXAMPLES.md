# Non-Null Collections Examples

## Good

```csharp
public sealed record SearchResponse
{
    public ImmutableList<ResultItem> Items { get; init; } = ImmutableList<ResultItem>.Empty;
}
```

```csharp
public ImmutableArray<User> GetUsers()
{
    return ImmutableArray<User>.Empty;
}
```

Why this is good:
- Consumers can use the collection safely without null checks
- Empty means "no items" clearly

---

## Bad

```csharp
public List<string>? Values { get; init; }

return null;
```

Better:
```csharp
public List<string> Values { get; init; } = [];

return [];
```
