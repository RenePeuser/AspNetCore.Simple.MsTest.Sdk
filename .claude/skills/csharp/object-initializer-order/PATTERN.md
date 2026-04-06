# Object Initializer Order

## Objective

Keep object initializers predictable, easy to scan, and easy to diff.

By default, order properties in object initializers alphabetically by property name.

## Apply These Rules

- Sort properties in object initializers alphabetically by property name.
- Use one consistent ordering rule across the repository.
- Prefer alphabetical ordering because it improves readability, diff quality, and merge behavior.
- Keep the ordering stable when adding or removing properties.
- Only deviate from alphabetical ordering when a clearly intentional domain-specific grouping is more important.
- If a non-alphabetical order is used, it should be obviously justified by the code structure or team convention.
- When properties are grouped by domain concepts, keep properties within each group alphabetically sorted.

## Why Alphabetical Ordering

Alphabetical ordering provides:

- **Predictability**: Any developer can find a property instantly without scanning the entire initializer.
- **Better diffs**: Adding or removing a property results in a minimal, localized diff at the correct alphabetical position.
- **Merge safety**: Reduces merge conflicts when multiple developers add properties simultaneously.
- **No cognitive load**: No need to understand domain relationships or debate where a new property should go.
- **Consistency**: Same rule applies to all initializers, making the entire codebase uniform.

## When to Deviate

Only use non-alphabetical ordering when:

- Properties have a clear, obvious domain-driven grouping (e.g., "identity properties", "audit properties", "configuration properties").
- The grouping is self-evident and would be immediately recognized by any team member.
- The benefit of semantic grouping clearly outweighs the loss of alphabetical predictability.

Even in grouped scenarios, keep properties within each group alphabetically sorted.

## Preferred Property Order

Use alphabetical sorting:

```csharp
var context = new ExampleContext
{
    AbsoluteUrl = url,
    Client = httpClient,
    ContentAsString = content,
    HttpMethod = method,
    HttpStatusCode = statusCode,
    IsSuccessStatusCode = isSuccess,
    Parameters = parameters,
    Url = relativeUrl,
};
```

## Avoid

Random or insertion-order property listing:

```csharp
var context = new ExampleContext
{
    HttpStatusCode = statusCode,
    Client = httpClient,
    Url = relativeUrl,
    IsSuccessStatusCode = isSuccess,
    ContentAsString = content,
    Parameters = parameters,
    AbsoluteUrl = url,
    HttpMethod = method,
};
```

## Review Checklist

- Are properties ordered alphabetically?
- Is the ordering stable and easy to scan?
- Would adding one property be straightforward without rethinking the whole initializer?
- Is any non-alphabetical order clearly intentional and justified?
- If grouped, are properties within each group alphabetically sorted?

## Notes

The preferred repository pattern is:

- Alphabetical property ordering by default
- Stable, predictable placement
- Minimal diffs when properties are added or removed
- Consistent across the entire codebase

This keeps object initializers maintainable and prevents cognitive overhead when reading or modifying them.
