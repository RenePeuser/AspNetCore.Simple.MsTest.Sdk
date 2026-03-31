# HttpAssertContext API - Usage Examples

## Overview
The new `HttpAssertContext` and `HttpAssertContext<TResult>` classes provide a cleaner API for HTTP assertions by bundling common parameters into a single context object.

## Before vs After

### Before (Old API with many parameters)
```csharp
await client.AssertPostAsync<UserResponse>(
    "/api/users",
    userPayload,
    "expected-response.json",
    user => user with { CreatedAt = default },  // filterFunc
    item => item.Where(d => d.MemberPath != "id"),  // differenceFunc
    new[] { ("{userId}", 123), ("{name}", "John") },  // parameters
    Assembly.GetExecutingAssembly(),
    true,  // writeResponse
    nameof(userPayload),
    nameof(expectedResponse),
    "TestFile.cs"
);
```

### After (New Context-based API)
```csharp
var context = new HttpAssertContext<UserResponse>
{
    FilterFunc = user => user with { CreatedAt = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "id"),
    Parameters = new[] { ("{userId}", 123), ("{name}", "John") },
    WriteResponse = true
};

await client.AssertPostAsync(
    "/api/users",
    userPayload,
    "expected-response.json",
    context
);
```

## Usage Examples

### 1. Simple GET Request (No Result)
```csharp
var context = new HttpAssertContext
{
    Parameters = new[] { ("{id}", userId) }
};

await client.AssertGetAsync("/api/users/{id}", context);
```

### 2. GET with Result and Filter
```csharp
var context = new HttpAssertContext<User>
{
    FilterFunc = user => user with
    {
        CreatedAt = default,
        ModifiedAt = default
    },
    WriteResponse = true
};

var result = await client.AssertGetAsync<User>(
    "/api/users/123",
    "expected-user.json",
    context
);
```

### 3. POST with Parameters and Difference Filter
```csharp
var context = new HttpAssertContext<CreateUserResponse>
{
    Parameters = new[]
    {
        ("{email}", "test@example.com"),
        ("{role}", "admin")
    },
    DifferenceFunc = diffs => diffs.Where(d =>
        d.MemberPath != "id" &&
        d.MemberPath != "createdTimestamp"
    ),
    WriteResponse = true
};

await client.AssertPostAsync<CreateUserResponse>(
    "/api/users",
    newUserPayload,
    "expected-create-response.json",
    context
);
```

### 4. Error Response Testing
```csharp
var context = new HttpAssertContext<ErrorResponse>
{
    IsSuccessStatusCode = false,  // Expect error response
    Parameters = new[] { ("{id}", invalidId) }
};

await client.AssertGetAsync<ErrorResponse>(
    "/api/users/{id}",
    "expected-error.json",
    context
);
```

### 5. PUT with Custom Assembly
```csharp
var context = new HttpAssertContext<User>
{
    CallingAssembly = typeof(MyTests).Assembly,
    FilterFunc = u => u with { UpdatedAt = default },
    WriteResponse = false
};

await client.AssertPutAsync<User>(
    "/api/users/123",
    updatePayload,
    "expected-updated-user.json",
    context
);
```

### 6. DELETE Request
```csharp
var context = new HttpAssertContext
{
    Parameters = new[] { ("{id}", userId) },
    IsSuccessStatusCode = true
};

await client.AssertDeleteAsync("/api/users/{id}", context);
```

### 7. PATCH with Complex Filtering
```csharp
var context = new HttpAssertContext<OrderResponse>
{
    FilterFunc = order => order with
    {
        Timestamp = default,
        ProcessedBy = null
    },
    DifferenceFunc = diffs => diffs.Where(d =>
        !d.MemberPath.Contains("audit") &&
        !d.MemberPath.Contains("metadata")
    ),
    Parameters = new[]
    {
        ("{orderId}", orderId),
        ("{status}", "completed")
    },
    WriteResponse = true
};

await client.AssertPatchAsync<OrderResponse>(
    "/api/orders/{orderId}",
    patchPayload,
    "expected-patched-order.json",
    context
);
```

## Benefits

1. **Cleaner code**: Fewer parameters to pass
2. **Named properties**: Clear intent with property names
3. **Optional parameters**: Only specify what you need
4. **Reusable contexts**: Create common contexts for similar tests
5. **Type safety**: Strongly typed context objects
6. **Backward compatible**: All existing APIs still work

## Common Context Patterns

### Test Snapshot Context (for updating test files)
```csharp
var snapshotContext = new HttpAssertContext<T>
{
    WriteResponse = true
};
```

### Ignore Dynamic Fields Context
```csharp
var ignoreDynamicsContext = new HttpAssertContext<T>
{
    FilterFunc = obj => obj with
    {
        Id = default,
        CreatedAt = default,
        UpdatedAt = default
    }
};
```

### Parameterized Test Context
```csharp
var paramContext = new HttpAssertContext<T>
{
    Parameters = new[]
    {
        ("{userId}", testUserId),
        ("{token}", authToken)
    }
};
```
