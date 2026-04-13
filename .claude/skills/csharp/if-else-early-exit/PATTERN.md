# If/Else Early Exit

## Objective

Avoid deep `if/else` cascades by making each condition answer one question:

- should execution continue?
- or should it stop here?

Use early exits so the happy path stays flat and easy to read.

## Apply These Rules

- Prefer early `return`, `throw`, `continue`, or `break` over nested `else` blocks.
- Keep the main happy path at the lowest possible indentation level.
- Invert conditions when that makes the normal flow clearer.
- Avoid `else` after a branch that already exits control flow.
- Separate boundary checks, stop conditions, and core processing.
- Use helper methods when one method still contains too many decisions after flattening.
- Preserve behavior; this is a structural refactoring, not a logic rewrite.

## Prefer

```csharp
if (request is null)
{
    throw new ArgumentNullException(nameof(request));
}

if (!permissionService.CanEdit(userId))
{
    return Results.Forbid();
}

if (!entity.IsActive)
{
    return Results.BadRequest();
}

return Process(entity);
```

## Avoid

```csharp
if (request is not null)
{
    if (permissionService.CanEdit(userId))
    {
        if (entity.IsActive)
        {
            return Process(entity);
        }
        else
        {
            return Results.BadRequest();
        }
    }
    else
    {
        return Results.Forbid();
    }
}
else
{
    throw new ArgumentNullException(nameof(request));
}
```

## Typical Early-Exit Shapes

### Guard clauses
Use early `throw` or `return` for invalid preconditions.

### Loop filtering
Use early `continue` to skip irrelevant items.

### Search flows
Use early `return` once the answer is known.

### Failure handling
Exit immediately when a required step fails instead of wrapping the rest of the method in `else`.

## Decision Heuristic

A method usually becomes clearer when it follows this order:

1. Validate input
2. Reject invalid state
3. Reject unauthorized or impossible paths
4. Execute the main happy path
5. Return the result

## Good Signs

- The happy path is visually obvious
- Indentation remains shallow
- Each condition has one clear purpose
- Failure paths are handled close to the condition that detects them

## Review Checklist

- Can one nesting level be removed by exiting early?
- Is there an `else` after `return`, `throw`, `continue`, or `break`?
- Is the happy path easy to spot?
- Are validation and failure paths separated from the main processing?
- Would a helper method make the branch structure clearer?

## Notes

This pattern is about readability and control flow, not about reducing the number of conditions at any cost.

A method may still need multiple decisions, but those decisions should guide execution explicitly instead of burying the main logic under nested `if/else` blocks.
