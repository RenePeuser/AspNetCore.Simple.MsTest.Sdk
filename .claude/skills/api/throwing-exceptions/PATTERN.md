# Throwing Exceptions Pattern

## Core Rule

Use `ProblemDetailsException` from `AspNetCore.Simple.MsTest.Sdk` instead of generic exceptions.

This follows RFC 7807 (Problem Details for HTTP APIs) for consistent error responses.

```csharp
using AspNetCore.Simple.MsTest.Sdk;
using System.Net;
```

## Constructor Shape

```csharp
throw new ProblemDetailsException(
    HttpStatusCode.{StatusCode},  // or use int: 404
    "Constant title",
    $"Detailed message with {dynamicValues}",
    [
        ("Key1", value1.ToString()),
        ("Key2", value2.ToString())
    ]);
```

## Rule 1: Title Must Be Constant

The title parameter is used for dashboards, alerts, and aggregation.

✅ Correct:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Error adding user to external workspace",
    $"Response came back from service as type {response.Status}",
    [("WorkspaceId", workspaceId.ToString())]);
```

❌ Wrong:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    $"Error adding user {user.Email} to external workspace",
    $"Response came back from service as type {response.Status}",
    [("WorkspaceId", workspaceId.ToString())]);
```

## Rule 2: Detail Explains the Situation

The details parameter should contain the dynamic values needed to understand the failure.

✅ Good detail:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.Conflict,
    "Duplicate resource name",
    $"A resource named '{request.Name}' already exists in project '{request.ProjectId}'",
    [
        ("ResourceName", request.Name),
        ("ProjectId", request.ProjectId.ToString())
    ]);
```

❌ Weak detail:
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.Conflict,
    "Duplicate resource name",
    "Resource already exists",
    [...]);
```

## Rule 3: Extensions Are for Structured Logging

Include IDs, state, and external references that will help during troubleshooting.

✅ Prefer:
```csharp
[
    ("ResourceId", resourceId.ToString()),
    ("ProjectId", projectId.ToString()),
    ("CurrentStatus", currentStatus),
    ("ExpectedStatus", expectedStatus)
]
```

Avoid:
- Empty extension arrays
- Large object dumps
- Secrets, tokens, passwords, connection strings
- Full sensitive payloads

## Rule 4: Convert Extension Values to Strings

✅ Correct:
```csharp
[
    ("ResourceId", resourceId.ToString()),
    ("Count", count.ToString()),
    ("IsActive", isActive.ToString())
]
```

❌ Wrong:
```csharp
[
    ("ResourceId", resourceId),
    ("Count", count),
    ("IsActive", isActive)
]
```

## Rule 5: Pick the Status Code by Responsibility

### 4xx - client or domain request problem

- `HttpStatusCode.BadRequest` (400) - invalid format or request parameters
- `HttpStatusCode.Unauthorized` (401) - missing or invalid authentication
- `HttpStatusCode.Forbidden` (403) - authenticated but not allowed
- `HttpStatusCode.NotFound` (404) - requested resource does not exist
- `HttpStatusCode.Conflict` (409) - duplicate or conflicting state
- `HttpStatusCode.UnprocessableEntity` (422) - valid request, invalid business rule outcome

### 5xx - server or downstream problem

- `HttpStatusCode.InternalServerError` (500) - unexpected internal failure
- `HttpStatusCode.BadGateway` (502) - downstream service returned an invalid response or failed
- `HttpStatusCode.ServiceUnavailable` (503) - temporary unavailability or maintenance
- `HttpStatusCode.GatewayTimeout` (504) - downstream timeout

## Quick Decision Guide

- Invalid input? → `HttpStatusCode.BadRequest`
- Missing authentication? → `HttpStatusCode.Unauthorized`
- No permission? → `HttpStatusCode.Forbidden`
- Missing entity? → `HttpStatusCode.NotFound`
- Duplicate or conflicting entity? → `HttpStatusCode.Conflict`
- Business rule violated? → `HttpStatusCode.UnprocessableEntity`
- Unexpected internal failure? → `HttpStatusCode.InternalServerError`
- Downstream call failed or returned junk? → `HttpStatusCode.BadGateway`
- Temporary system outage? → `HttpStatusCode.ServiceUnavailable`
- Downstream timeout? → `HttpStatusCode.GatewayTimeout`

## Rule 6: Keep Titles Consistent Within a Domain

Use the same title for the same failure category across handlers in the same domain.

✅ Good:
- `"Resource not found"`
- `"Duplicate resource name"`
- `"Error processing external request"`

❌ Avoid drift:
- `"Resource type not found"`
- `"ResourceType does not exist"`
- `"Cannot find resource type"`

## Anti-Patterns to Avoid

### Generic exception
```csharp
throw new Exception($"Resource {resourceId} not found");
```

### Dynamic title
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.NotFound,
    $"Resource {resourceId} not found",
    "...",
    [...]);
```

### Empty extensions
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Database update failed",
    "Unable to update entity",
    []);
```

### Wrong status code
```csharp
throw new ProblemDetailsException(
    HttpStatusCode.InternalServerError,
    "Resource not found",
    $"No resource exists with ID '{resourceId}'",
    [("ResourceId", resourceId.ToString())]);
```

## Review Order

1. Is `ProblemDetailsException` used instead of generic `Exception`?
2. Is the HTTP status code correct for the failure type?
3. Is the title constant (not containing dynamic values)?
4. Does the detail explain the failure with dynamic values?
5. Do the extensions contain the minimum useful IDs and state?
6. Are all extension values strings?
7. Are secrets excluded from extensions?
