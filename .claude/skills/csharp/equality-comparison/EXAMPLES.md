# Equality Comparison Examples

## Good

```csharp
if (current.Id.EqualsTo(expected.Id))
{
    return current;
}

if (current.Status.NotEqualsTo(previous.Status))
{
    audit.LogStatusChange(current.Status);
}
```

Why this is good:
- Uses the repository default for value equality
- Keeps value equality distinct from reference identity

---

## Bad

```csharp
if (current.Id == expected.Id)
{
    return current;
}
```

Better:
```csharp
if (current.Id.EqualsTo(expected.Id))
{
    return current;
}
```
