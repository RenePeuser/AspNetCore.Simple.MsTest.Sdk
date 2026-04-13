# If/Else Early Exit Examples

## Example 1: Nested Validation and Permission Checks

### Before

```csharp
public IResult Update(Guid userId, UpdateRequest? request, Entity entity)
{
    if (request is not null)
    {
        if (permissionService.CanEdit(userId))
        {
            if (entity.IsActive)
            {
                return Process(entity, request);
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

    throw new ArgumentNullException(nameof(request));
}
```

### After

```csharp
public IResult Update(Guid userId, UpdateRequest? request, Entity entity)
{
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

    return Process(entity, request);
}
```

Why this is good:
- The happy path is at the bottom with minimal indentation
- Each condition answers whether execution should continue
- Failure handling is close to the decision point

---

## Example 2: Early Continue in a Loop

### Before

```csharp
foreach (var node in nodes)
{
    if (node.IsDeleted)
    {
    }
    else
    {
        if (node.ProjectId == projectId)
        {
            result.Add(Map(node));
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

    result.Add(Map(node));
}
```

Why this is good:
- Filtering logic is explicit
- The actual work is no longer nested
- The loop body is easier to scan

---

## Example 3: Remove Redundant Else After Return

### Before

```csharp
if (deployment is null)
{
    return Results.NotFound();
}
else
{
    return Results.Ok(Map(deployment));
}
```

### After

```csharp
if (deployment is null)
{
    return Results.NotFound();
}

return Results.Ok(Map(deployment));
```

Why this is good:
- The `else` adds no value after an exiting branch
- The method becomes flatter without changing behavior

---

## Example 4: Split Branching into a Helper

### Before

```csharp
public async Task HandleAsync(Request request, CancellationToken cancellationToken)
{
    if (request is not null)
    {
        if (request.Items.Count > 0)
        {
            if (request.Items.All(CanProcess))
            {
                await ProcessAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
```

### After

```csharp
public async Task HandleAsync(Request request, CancellationToken cancellationToken)
{
    if (!CanProcessRequest(request))
    {
        return;
    }

    await ProcessAsync(request, cancellationToken).ConfigureAwait(false);
}

private static bool CanProcessRequest(Request? request)
{
    if (request is null)
    {
        return false;
    }

    if (request.Items.Count == 0)
    {
        return false;
    }

    return request.Items.All(CanProcess);
}
```

Why this is good:
- The main method shows intent immediately
- Branch-heavy preconditions are isolated
- The processing path stays simple
