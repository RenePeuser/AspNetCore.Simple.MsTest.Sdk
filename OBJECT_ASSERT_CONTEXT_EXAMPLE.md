# ObjectAssertContext API - Usage Examples

## Overview
The new `ObjectAssertContext<T>` class provides a cleaner API for object equality assertions by bundling common parameters into a single context object.

## Before vs After

### Before (Old API with many parameters)
```csharp
Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser,
    user => user with { CreatedAt = default },  // orderFunc
    "User comparison",                          // title
    Assembly.GetExecutingAssembly(),            // callingAssembly
    diffs => diffs.Where(d => d.MemberPath != "id"),  // differenceFunc
    curlCommand,                                // curl
    new[] { ("{userId}", 123) },                // parameters
    true,                                       // writeResponse
    "expected-user.json",                       // expectedResultParameterName
    nameof(currentUser),                        // currentResultParameterName
    "UserTests.cs"                              // callerFilePath
);
```

### After (New Context-based API)
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = user => user with { CreatedAt = default },
    Title = "User comparison",
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "id"),
    Curl = curlCommand,
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
};

Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser,
    context
);
```

## Usage Examples

### 1. Simple Comparison (No Context Needed)
```csharp
// For simple cases, use the existing overloads
Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser
);
```

### 2. With Filtering Dynamic Properties
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = user => user with
    {
        Id = default,
        CreatedAt = default,
        UpdatedAt = default
    }
};

Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser,
    context
);
```

### 3. With Difference Filtering
```csharp
var context = new ObjectAssertContext<OrderResponse>
{
    DifferenceFunc = diffs => diffs.Where(d =>
        !d.MemberPath.Contains("timestamp") &&
        !d.MemberPath.Contains("metadata")
    ),
    Title = "Order comparison - ignoring timestamps"
};

Assert.That.ObjectsAreEqual(
    expectedOrderJson,
    currentOrder,
    context
);
```

### 4. With Parameters Replacement
```csharp
var context = new ObjectAssertContext<User>
{
    Parameters = new[]
    {
        ("{userId}", testUserId),
        ("{email}", "test@example.com"),
        ("{timestamp}", DateTime.UtcNow.ToString("o"))
    }
};

Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser,
    context
);
```

### 5. Complex Scenario with All Options
```csharp
var context = new ObjectAssertContext<OrderResponse>
{
    // Normalize the data before comparison
    OrderFunc = order => order with
    {
        Id = default,
        ProcessedAt = default,
        Items = order.Items.OrderBy(i => i.Sku).ToList()
    },

    // Filter out differences we don't care about
    DifferenceFunc = diffs => diffs.Where(d =>
        !d.MemberPath.Contains("audit") &&
        !d.MemberPath.Contains("version") &&
        d.MismatchType == MismatchType.ValueDifference
    ),

    // Replace placeholders in JSON
    Parameters = new[]
    {
        ("{orderId}", orderId),
        ("{customerId}", customerId),
        ("{totalAmount}", expectedTotal.ToString("F2"))
    },

    // Enable snapshot writing for test updates
    WriteResponse = true,

    // Add context for error messages
    Title = "Order verification after payment",
    Curl = $"curl -X POST {apiUrl}/orders/{orderId}/pay",

    // Custom assembly if needed
    CallingAssembly = typeof(OrderTests).Assembly
};

Assert.That.ObjectsAreEqual(
    "expected-paid-order.json",
    actualOrder,
    context
);
```

### 6. Reusable Context Patterns
```csharp
// Create helper methods for common contexts
public static class TestContexts
{
    public static ObjectAssertContext<T> IgnoreDynamicFields<T>() =>
        new ObjectAssertContext<T>
        {
            DifferenceFunc = diffs => diffs.Where(d =>
                !d.MemberPath.EndsWith(".id") &&
                !d.MemberPath.EndsWith(".createdAt") &&
                !d.MemberPath.EndsWith(".updatedAt")
            )
        };

    public static ObjectAssertContext<T> WithSnapshot<T>(bool enabled = true) =>
        new ObjectAssertContext<T>
        {
            WriteResponse = enabled
        };

    public static ObjectAssertContext<T> OrderCollections<T>() =>
        new ObjectAssertContext<T>
        {
            OrderFunc = obj =>
            {
                // Sort all collections in the object
                // ... implementation
                return obj;
            }
        };
}

// Usage:
Assert.That.ObjectsAreEqual(
    "expected.json",
    actual,
    TestContexts.IgnoreDynamicFields<User>()
);
```

### 7. Combining with Test Data Builders
```csharp
[TestMethod]
public void VerifyUserCreation()
{
    // Arrange
    var newUser = new UserBuilder()
        .WithEmail("test@example.com")
        .WithRole("admin")
        .Build();

    var context = new ObjectAssertContext<User>
    {
        OrderFunc = u => u with { Id = default, CreatedAt = default },
        Parameters = new[]
        {
            ("{email}", newUser.Email),
            ("{role}", newUser.Role)
        },
        Title = "New user verification"
    };

    // Act
    var result = await userService.CreateUserAsync(newUser);

    // Assert
    Assert.That.ObjectsAreEqual(
        "expected-created-user.json",
        result,
        context
    );
}
```

## Migration Guide

### From Individual Parameters to Context

**Old Code:**
```csharp
Assert.That.ObjectsAreEqual(
    expectedJson,
    actual,
    item => item,
    "My Test",
    Assembly.GetCallingAssembly(),
    diff => diff,
    "",
    new[] { ("key", "value") },
    true
);
```

**New Code:**
```csharp
var context = new ObjectAssertContext<MyType>
{
    Title = "My Test",
    Parameters = new[] { ("key", "value") },
    WriteResponse = true
};

Assert.That.ObjectsAreEqual(expectedJson, actual, context);
```

## Benefits

1. **Cleaner Code**: Fewer parameters to pass
2. **Named Properties**: Clear intent with property names
3. **Optional Everything**: Only specify what you need
4. **Reusable Contexts**: Create common contexts for similar tests
5. **Type Safety**: Strongly typed context objects
6. **Backward Compatible**: All existing overloads still work

## Common Context Patterns

### Ignore Dynamic Fields
```csharp
var ignoreDynamic = new ObjectAssertContext<T>
{
    OrderFunc = obj => obj with
    {
        Id = default,
        CreatedAt = default,
        UpdatedAt = default
    }
};
```

### Enable Snapshot Writing
```csharp
var snapshot = new ObjectAssertContext<T>
{
    WriteResponse = true
};
```

### Parameterized Comparison
```csharp
var withParams = new ObjectAssertContext<T>
{
    Parameters = new[]
    {
        ("{id}", testId),
        ("{date}", testDate)
    }
};
```

### Complex Filtering
```csharp
var filtered = new ObjectAssertContext<T>
{
    DifferenceFunc = diffs => diffs
        .Where(d => d.MismatchType == MismatchType.ValueDifference)
        .Where(d => !d.MemberPath.Contains("internal"))
};
```
