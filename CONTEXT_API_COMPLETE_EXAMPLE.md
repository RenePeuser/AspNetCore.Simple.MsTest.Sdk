# Complete Context API Example

## Overview
This document shows the evolution of the `ObjectsAreEqual` API from parameter-heavy to a clean, context-based approach.

## API Evolution

### Level 1: Original Parameter-Heavy API (Still Supported)

```csharp
[TestMethod]
public void TestUser_OldStyle()
{
    var currentUser = new User { Id = 123, Name = "John", Email = "john@example.com" };
    
    Assert.That.ObjectsAreEqual(
        "expected-user.json",
        currentUser,
        orderFunc: user => user with { Id = default },
        title: "User comparison",
        callingAssembly: Assembly.GetExecutingAssembly(),
        differenceFunc: diffs => diffs.Where(d => d.MemberPath != "timestamp"),
        curl: string.Empty,
        parameters: new[] { ("{userId}", 123) },
        writeResponse: false,
        expectedResultParameterName: "expected-user.json",
        currentResultParameterName: nameof(currentUser),
        callerFilePath: "UserTests.cs"
    );
}
```

**Problems:**
- 😱 12 parameters
- 🤯 Hard to read
- 😓 Easy to get parameter order wrong
- 📝 Verbose and repetitive

---

### Level 2: Partial Context API (Still Supported)

```csharp
[TestMethod]
public void TestUser_PartialContext()
{
    var currentUser = new User { Id = 123, Name = "John", Email = "john@example.com" };
    
    var context = new ObjectAssertContext<User>
    {
        OrderFunc = user => user with { Id = default },
        Title = "User comparison",
        DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
        Parameters = new[] { ("{userId}", 123) },
        WriteResponse = false
    };
    
    Assert.That.ObjectsAreEqual("expected-user.json", currentUser, context);
}
```

**Improvements:**
- ✅ Context bundles optional parameters
- ✅ More readable
- ⚠️ Still requires passing expected and current separately

---

### Level 3: Complete Context API (NEW! ⭐)

```csharp
[TestMethod]
public void TestUser_CompleteContext()
{
    var currentUser = new User { Id = 123, Name = "John", Email = "john@example.com" };
    
    var context = new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "expected-user.json",
        CurrentObject = currentUser,
        OrderFunc = user => user with { Id = default },
        Title = "User comparison",
        DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
        Parameters = new[] { ("{userId}", 123) },
        WriteResponse = false
    };
    
    Assert.That.ObjectsAreEqual(context);  // 🎉 Super clean!
}
```

**Benefits:**
- ✅ Everything in one context object
- ✅ Crystal clear what's being tested
- ✅ Easy to reuse contexts
- ✅ Self-documenting code
- ✅ IntelliSense-friendly

---

## Real-World Examples

### Example 1: Simple Comparison

```csharp
[TestMethod]
public void GetUser_ReturnsExpectedUser()
{
    // Arrange
    var userId = 123;
    var actualUser = _userService.GetUser(userId);
    
    // Act & Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "expected-user.json",
        CurrentObject = actualUser
    });
}
```

### Example 2: With Filtering

```csharp
[TestMethod]
public void GetUsers_ReturnsExpectedList()
{
    // Arrange
    var actualUsers = _userService.GetAllUsers();
    
    // Act & Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<List<User>>
    {
        ExpectedObjectAsJson = "expected-users.json",
        CurrentObject = actualUsers,
        OrderFunc = users => users.OrderBy(u => u.Id).ToList(),
        DifferenceFunc = diffs => diffs.Where(d => !d.MemberPath.Contains("CreatedAt"))
    });
}
```

### Example 3: With Parameters

```csharp
[TestMethod]
public void GetUserById_WithDynamicId()
{
    // Arrange
    var userId = 456;
    var actualUser = _userService.GetUser(userId);
    
    // Act & Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "expected-user-template.json",
        CurrentObject = actualUser,
        Parameters = new[] 
        { 
            ("{userId}", userId),
            ("{timestamp}", DateTime.UtcNow.ToString("O"))
        },
        Title = $"User {userId} comparison"
    });
}
```

### Example 4: Snapshot Testing with WriteResponse

```csharp
[TestMethod]
public void GetUser_SnapshotTest()
{
    // Arrange
    var actualUser = _userService.GetUser(123);
    
    // Act & Assert - WriteResponse will update the expected file
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "user-snapshot.json",
        CurrentObject = actualUser,
        WriteResponse = true,  // Updates the snapshot file
        OrderFunc = user => user with { Id = default, UpdatedAt = default }
    });
}
```

### Example 5: Reusable Context Builder Pattern

```csharp
public static class UserAssertContexts
{
    public static ObjectAssertContext<User> ForUser(User actual, string expectedFile)
    {
        return new ObjectAssertContext<User>
        {
            ExpectedObjectAsJson = expectedFile,
            CurrentObject = actual,
            OrderFunc = user => user with 
            { 
                Id = default, 
                CreatedAt = default,
                UpdatedAt = default 
            },
            DifferenceFunc = diffs => diffs.Where(d => 
                !d.MemberPath.Contains("_metadata") &&
                !d.MemberPath.Contains("_links")
            )
        };
    }
}

[TestMethod]
public void GetUser_UsesReusableContext()
{
    var actualUser = _userService.GetUser(123);
    
    Assert.That.ObjectsAreEqual(
        UserAssertContexts.ForUser(actualUser, "expected-user.json")
    );
}
```

### Example 6: Complex Nested Object

```csharp
[TestMethod]
public void GetOrderWithItems_ComplexComparison()
{
    // Arrange
    var actualOrder = _orderService.GetOrder(789);
    
    // Act & Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<Order>
    {
        ExpectedObjectAsJson = "expected-order.json",
        CurrentObject = actualOrder,
        OrderFunc = order => order with
        {
            Items = order.Items
                .OrderBy(i => i.ProductId)
                .Select(i => i with { Id = default })
                .ToList()
        },
        DifferenceFunc = diffs => diffs.Where(d =>
            d.MemberPath != "Order.CreatedAt" &&
            d.MemberPath != "Order.UpdatedAt" &&
            !d.MemberPath.Contains("Items[").Contains("].Id")
        ),
        Parameters = new[]
        {
            ("{orderId}", 789),
            ("{totalAmount}", actualOrder.TotalAmount)
        },
        Title = "Order with items comparison"
    });
}
```

## Comparison Table

| Feature | Level 1 (Parameters) | Level 2 (Partial) | Level 3 (Complete) ⭐ |
|---------|---------------------|-------------------|----------------------|
| Parameters | 12 individual | 2 + context | 1 context |
| Readability | 😱 Poor | 😊 Good | 🎉 Excellent |
| Reusability | ❌ Hard | ✅ Good | ✅ Excellent |
| Self-documenting | ❌ No | ✅ Yes | ✅ Yes |
| IntelliSense | ⚠️ Limited | ✅ Good | ✅ Excellent |
| Type safety | ✅ Yes | ✅ Yes | ✅ Yes |
| Backward compatible | ✅ Original | ✅ Yes | ✅ Yes |

## Migration Guide

### From Level 1 to Level 3

**Before:**
```csharp
Assert.That.ObjectsAreEqual(
    "expected.json",
    actual,
    x => x with { Id = default },
    "Title",
    Assembly.GetExecutingAssembly(),
    d => d.Where(x => x.MemberPath != "timestamp"),
    "",
    new[] { ("{id}", 123) },
    false
);
```

**After:**
```csharp
Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected.json",
    CurrentObject = actual,
    OrderFunc = x => x with { Id = default },
    Title = "Title",
    DifferenceFunc = d => d.Where(x => x.MemberPath != "timestamp"),
    Parameters = new[] { ("{id}", 123) }
});
```

### From Level 2 to Level 3

**Before:**
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = x => x with { Id = default }
};

Assert.That.ObjectsAreEqual("expected.json", actual, context);
```

**After:**
```csharp
var context = new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected.json",
    CurrentObject = actual,
    OrderFunc = x => x with { Id = default }
};

Assert.That.ObjectsAreEqual(context);
```

## Best Practices

### ✅ DO

```csharp
// Use property initializer for clarity
var context = new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected.json",
    CurrentObject = actual,
    OrderFunc = user => user with { Id = default }
};
```

```csharp
// Create reusable context builders
public static ObjectAssertContext<T> WithoutTimestamps<T>(this ObjectAssertContext<T> ctx)
{
    var existing = ctx.DifferenceFunc ?? (d => d);
    return ctx with 
    { 
        DifferenceFunc = diffs => existing(diffs)
            .Where(d => !d.MemberPath.Contains("Timestamp"))
    };
}
```

### ❌ DON'T

```csharp
// Don't create incomplete contexts
var context = new ObjectAssertContext<User>();  // Missing required fields!
Assert.That.ObjectsAreEqual(context);  // Will throw ArgumentNullException
```

```csharp
// Don't mix old and new style without reason
var context = new ObjectAssertContext<User>
{
    OrderFunc = x => x
};
Assert.That.ObjectsAreEqual("expected.json", actual, context);  // Use Level 3 instead!
```

## Summary

The new **Complete Context API** (Level 3) provides:

- 🎯 **Maximum clarity** - Everything in one place
- 🧹 **Cleaner code** - From 12 parameters to 1 context
- 🔄 **Reusability** - Easy to create and share context builders
- 📚 **Self-documenting** - Property names make intent clear
- ⚡ **IntelliSense-friendly** - IDE helps you build the context
- ✅ **100% Backward compatible** - All old code still works

Choose the level that fits your needs, but for new code, **Level 3 is recommended**! 🎉
