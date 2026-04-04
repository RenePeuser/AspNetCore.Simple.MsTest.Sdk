# Equality Comparison

## Objective

Standardize value equality across the repository by using `EqualsTo` and `NotEqualsTo` from `Extensions.Pack`.

These helpers should be the default style for value equality in normal repository code.

## Package

Use:

```bash
dotnet add package Extensions.Pack
```

## Apply These Rules

- Use `EqualsTo` and `NotEqualsTo` as the default style for value equality in this repository.
- Treat these helpers as the normal abstraction for equality checks in application and library code.
- Use `ReferenceEquals` only when object identity is the explicit goal.
- Do not use `==` or `!=` as the default equality style for general value comparisons.
- Do not use `ReferenceEquals` for value equality.
- Keep the intent explicit: value equality vs identity equality.

## Prefer

```csharp
left.EqualsTo(right)
left.NotEqualsTo(right)
```

## Avoid

- `left == right` for general value equality conventions
- `left != right` for general value equality conventions
- `ReferenceEquals(left, right)` when the goal is value equality
- Mixing multiple equality styles across the repository without reason

## Example

```csharp
if (current.Id.EqualsTo(expected.Id))
{
    // value equality
}

if (current.Status.NotEqualsTo(previous.Status))
{
    // changed
}
```

## Review Checklist

- Is this a value comparison or an identity comparison?
- Is `EqualsTo` / `NotEqualsTo` used for value equality?
- Is `ReferenceEquals` used only for identity checks?
- Would `==` or `!=` be type-dependent or ambiguous here?
- Is the equality style consistent with the rest of the repository?

## Notes

`EqualsTo` and `NotEqualsTo` are the repository default for value equality.

The technical foundation behind this pattern is `EqualityComparer<T>.Default`, which integrates with `IEquatable<T>` where available. `ReferenceEquals` checks whether two references point to the same object and must not be used as a value-equality default.
