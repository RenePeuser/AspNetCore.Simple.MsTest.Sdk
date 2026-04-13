# Loops Use Continue

## Objective

Keep loop bodies small and readable by handling skip conditions early with `continue`.

Inside a loop, each condition should answer one question:
- should this item continue through the processing path?
- or should this iteration stop here?

Use early `continue` so the real loop work stays flat and easy to scan.

## Apply These Rules

- Prefer early `continue` over nested `if/else` blocks inside `foreach` and `for` loops.
- Put skip conditions at the top of the loop body.
- Keep the actual processing logic at the lowest possible indentation level.
- Avoid `else` when a branch already ends the current iteration with `continue`.
- Use clear, focused checks for filtering, state rejection, and preconditions.
- Combine conditions only when that improves readability.
- Extract helper methods if a loop still contains too many decisions.
- Preserve behavior; this is a control-flow refactoring, not a logic change.

## Prefer

```csharp
foreach (var deployment in deployments)
{
    if (deployment.IsDeleted)
    {
        continue;
    }

    if (deployment.ProjectId != projectId)
    {
        continue;
    }

    Process(deployment);
}
```

## Avoid

```csharp
foreach (var deployment in deployments)
{
    if (!deployment.IsDeleted)
    {
        if (deployment.ProjectId == projectId)
        {
            Process(deployment);
        }
    }
}
```

## Typical Continue-First Shapes

### Filtering irrelevant items
Use `continue` when the current item should not participate further.

### Guarding incomplete state
Skip items that are missing data or are in an unsupported state.

### Avoiding multi-level branching
Move rejection logic upward so the main loop action remains flat.

### Index-based loops
Use `continue` in `for` loops the same way as in `foreach` loops when an iteration should be skipped.

## Decision Heuristic

A loop usually becomes clearer when it follows this order:

1. Skip deleted, disabled, invalid, or irrelevant items
2. Skip items that do not match the current filter or state
3. Execute the actual per-item work

## Good Signs

- The real work is easy to find in the loop body
- Most conditions are short skip checks
- Indentation stays shallow
- Each `continue` removes one level of nesting

## Review Checklist

- Can a nested loop condition become an early `continue`?
- Is there an `else` after `continue`?
- Is the actual loop work visually obvious?
- Are skip conditions grouped near the top of the loop?
- Would a helper method make the loop clearer?

## Notes

This pattern is especially useful when loops perform filtering before one central action.

Not every loop needs multiple `continue` statements, but if nested conditions hide the actual work, a continue-first structure is usually easier to read.
