# Loops Use Continue Examples

## Example 1: Foreach with Multiple Skip Conditions

### Before

```csharp
foreach (var node in nodes)
{
    if (!node.IsDeleted)
    {
        if (node.ProjectId == projectId)
        {
            if (node.IsVisible)
            {
                result.Add(Map(node));
            }
        }
    }
}
```

### After

```csharp
foreach (var node in nodes)
{
    if (node.IsDeleted)
    {
        continue;
    }

    if (node.ProjectId != projectId)
    {
        continue;
    }

    if (!node.IsVisible)
    {
        continue;
    }

    result.Add(Map(node));
}
```

Why this is good:
- The real work is now the final line of the loop
- Each skip condition is explicit
- Nesting is removed without changing behavior

---

## Example 2: For Loop with Index-Based Filtering

### Before

```csharp
for (var index = 0; index < items.Count; index++)
{
    if (items[index] is not null)
    {
        if (items[index]!.IsActive)
        {
            Process(items[index]!);
        }
    }
}
```

### After

```csharp
for (var index = 0; index < items.Count; index++)
{
    var item = items[index];
    if (item is null)
    {
        continue;
    }

    if (!item.IsActive)
    {
        continue;
    }

    Process(item);
}
```

Why this is good:
- The indexed item is captured once
- Skip conditions are simple and visible
- The loop body is flatter and easier to maintain

---

## Example 3: Remove Redundant Else After Continue

### Before

```csharp
foreach (var relation in relations)
{
    if (relation.IsDeleted)
    {
        continue;
    }
    else
    {
        UpdateRelation(relation);
    }
}
```

### After

```csharp
foreach (var relation in relations)
{
    if (relation.IsDeleted)
    {
        continue;
    }

    UpdateRelation(relation);
}
```

Why this is good:
- The `else` is unnecessary after `continue`
- The loop becomes smaller without changing meaning

---

## Example 4: Extract Complex Skip Logic

### Before

```csharp
foreach (var deployment in deployments)
{
    if (!deployment.IsDeleted)
    {
        if (deployment.ProjectId == projectId)
        {
            if (deployment.TargetEnvironment == environment)
            {
                if (CanSelfHeal(deployment))
                {
                    SelfHeal(deployment);
                }
            }
        }
    }
}
```

### After

```csharp
foreach (var deployment in deployments)
{
    if (ShouldSkipDeployment(deployment, projectId, environment))
    {
        continue;
    }

    SelfHeal(deployment);
}

private bool ShouldSkipDeployment(Deployment deployment, Guid projectId, string environment)
{
    if (deployment.IsDeleted)
    {
        return true;
    }

    if (deployment.ProjectId != projectId)
    {
        return true;
    }

    if (deployment.TargetEnvironment != environment)
    {
        return true;
    }

    return !CanSelfHeal(deployment);
}
```

Why this is good:
- The loop shows intent immediately
- Complex branching is moved behind a well-named helper
- The per-item action stays obvious
