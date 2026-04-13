# Throwing Exceptions - Examples

## Good Examples

### Example 1: Resource Not Found
```csharp
public async Task<Resource> GetByIdAsync(Guid resourceId, Guid projectId, CancellationToken cancellationToken)
{
    var resource = await repository.GetByIdAsync(resourceId, cancellationToken).ConfigureAwait(false);

    if (resource is null)
    {
        throw new ProblemDetailsException(
            HttpStatusCode.NotFound,
            "Resource not found",
            $"No resource exists with ID '{resourceId}' in project '{projectId}'",
            [
                ("ResourceId", resourceId.ToString()),
                ("ProjectId", projectId.ToString())
            ]);
    }

    return resource;
}
```

Why this is good:
- Constant title
- Clear detail with both IDs
- Relevant extensions for troubleshooting
- Correct `404` mapping

---

### Example 2: Domain Conflict
```csharp
public async Task CreateAsync(CreateResourceRequest request, CancellationToken cancellationToken)
{
    var existing = await repository.GetByNameAsync(request.Name, request.ProjectId, cancellationToken).ConfigureAwait(false);

    if (existing is not null)
    {
        throw new ProblemDetailsException(
            HttpStatusCode.Conflict,
            "Duplicate resource name",
            $"A resource named '{request.Name}' already exists in project '{request.ProjectId}' with ID '{existing.Id}'",
            [
                ("ResourceName", request.Name),
                ("ProjectId", request.ProjectId.ToString()),
                ("ExistingResourceId", existing.Id.ToString())
            ]);
    }
}
```

Why this is good:
- Clear conflict title used for aggregation
- Detail explains what conflicts and where
- Extensions include the existing entity identifier
- Correct `409` mapping

---

### Example 3: Unexpected External Service Response
```csharp
public async Task AddUserToWorkspaceAsync(Guid workspaceId, User user, CancellationToken cancellationToken)
{
    var response = await externalClient.AddUserAsync(workspaceId, user, cancellationToken).ConfigureAwait(false);

    if (response.Status != "Active")
    {
        throw new ProblemDetailsException(
            HttpStatusCode.InternalServerError,
            "Error adding user to external workspace",
            $"Response came back from service as type {response.Status}",
            [
                ("WorkspaceId", workspaceId.ToString()),
                ("ResponseStatus", response.Status),
                ("UserEmail", user.Email)
            ]);
    }
}
```

Why this is good:
- Stable title for monitoring
- Useful downstream state in detail and extensions
- Small but sufficient extension set
- Correct `500` for unexpected response handling in this flow

---

### Example 4: Invalid Business Rule
```csharp
public async Task UpdateStatusAsync(Guid resourceId, string newStatus, CancellationToken cancellationToken)
{
    var resource = await repository.GetByIdAsync(resourceId, cancellationToken).ConfigureAwait(false);
    
    if (resource.Status == "Archived" && newStatus != "Active")
    {
        throw new ProblemDetailsException(
            HttpStatusCode.UnprocessableEntity,
            "Invalid status transition",
            $"Cannot change status from 'Archived' to '{newStatus}'. Archived resources can only be reactivated.",
            [
                ("ResourceId", resourceId.ToString()),
                ("CurrentStatus", resource.Status),
                ("RequestedStatus", newStatus)
            ]);
    }
}
```

Why this is good:
- Correct `422` for business rule violation
- Clear explanation of why the transition failed
- Context shows the state machine issue

## Bad Examples

### Bad Example 1: Dynamic Title
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.NotFound,
    $"Resource {resourceId} not found",
    $"No resource exists with ID '{resourceId}'",
    [("ResourceId", resourceId.ToString())]);
```

Fix:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.NotFound,
    "Resource not found",
    $"No resource exists with ID '{resourceId}'",
    [("ResourceId", resourceId.ToString())]);
```

---

### Bad Example 2: Missing Extensions
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Error adding user to external workspace",
    "Response came back from service with unexpected status",
    []);
```

Fix:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Error adding user to external workspace",
    $"Response came back from service as type {response.Status}",
    [
        ("WorkspaceId", workspaceId.ToString()),
        ("ResponseStatus", response.Status),
        ("UserEmail", user.Email)
    ]);
```

---

### Bad Example 3: Wrong Status Code
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Invalid resource type",
    $"The resource type '{resourceType}' is not supported",
    [("ResourceType", resourceType)]);
```

Fix:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.BadRequest,
    "Invalid resource type",
    $"The resource type '{resourceType}' is not supported",
    [("ResourceType", resourceType)]);
```

---

### Bad Example 4: Non-String Extensions
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.NotFound,
    "Resource not found",
    $"No resource exists with ID '{resourceId}'",
    [
        ("ResourceId", resourceId),  // Wrong: not converted to string
        ("Count", count)              // Wrong: not converted to string
    ]);
```

Fix:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.NotFound,
    "Resource not found",
    $"No resource exists with ID '{resourceId}'",
    [
        ("ResourceId", resourceId.ToString()),
        ("Count", count.ToString())
    ]);
```

## Before and After

### Before
```csharp
public async Task<Resource> UpdateAsync(Guid id, UpdateResourceRequest request)
{
    try
    {
        var resource = await repository.GetByIdAsync(id);
        if (resource == null)
        {
            throw new Exception($"Resource {id} not found");
        }

        resource.Name = request.Name;
        await repository.SaveAsync(resource);
        return resource;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error updating resource: {ex.Message}");
    }
}
```

### After
```csharp
public async Task<Resource> UpdateAsync(Guid id, UpdateResourceRequest request, CancellationToken cancellationToken)
{
    var resource = await repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

    if (resource is null)
    {
        throw new ProblemDetailsException(
            HttpStatusCode.NotFound,
            "Resource not found",
            $"No resource exists with ID '{id}'",
            [("ResourceId", id.ToString())]);
    }

    try
    {
        resource.Name = request.Name;
        await repository.SaveAsync(resource, cancellationToken).ConfigureAwait(false);
        return resource;
    }
    catch (DbUpdateException ex)
    {
        throw new ProblemDetailsException(
            HttpStatusCode.InternalServerError,
            "Resource update failed",
            $"Failed to update resource '{id}' in the database: {ex.Message}",
            [
                ("ResourceId", id.ToString()),
                ("RequestedName", request.Name)
            ]);
    }
}
```

Key improvements:
- Replaces generic exceptions with `ProblemDetailsException`
- Uses constant titles
- Uses appropriate HTTP status codes
- Adds structured, queryable extensions
- Follows RFC 7807 Problem Details standard
