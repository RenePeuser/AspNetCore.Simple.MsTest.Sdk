# Internal Refactoring Summary - Step 1

## Overview
This document describes the first step of refactoring the internal implementation to use context objects instead of long parameter lists.

## Goals
- Make internal code cleaner and more maintainable
- Reduce parameter passing throughout the call stack
- Make it easier to add new features in the future
- Keep 100% backward compatibility

## What Changed

### 1. Created Internal Context Classes

#### `HttpAssertContextInternal<TResult>`
Internal context used by the master `AssertHttpCallAsync<TResult>` method. Contains:
- HttpClient
- Url, PayloadAsJson, ExpectedResult
- HttpMethod
- FilterFunc, DifferenceFunc
- Parameters, CallingAssembly
- WriteResponse, IsSuccessStatusCode
- CallerFilePath, PayloadParameterName, ExpectedResultParameterName

#### `HttpAssertContextInternal` (non-generic)
Simplified version for non-generic assertions.

Both have factory methods:
- `FromPublicContext()` - Converts public `HttpAssertContext` to internal context
- `FromParameters()` - Creates context from individual parameters (backward compatibility)

### 2. Refactored Master Methods

#### Before:
```csharp
private static async Task<TResult> AssertHttpCallAsync<TResult>(
    this HttpClient client,
    string url,
    string payloadAsJson,
    string expectedResult,
    Func<TResult, TResult> filterFunc,
    HttpMethod httpMethod,
    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
    (string Key, object? Value)[] parameters,
    Assembly callingAssembly,
    string payloadAsJsonParameterName = "",
    string expectedResultParameterName = "",
    string callerFilePath = "",
    bool isSuccessStatusCode = true,
    bool writResponse = false)
{
    // ... 13 parameters! 😱
}
```

#### After:
```csharp
// Old signature still exists for backward compatibility
private static Task<TResult> AssertHttpCallAsync<TResult>(...)
{
    var context = HttpAssertContextInternal<TResult>.FromParameters(...);
    return AssertHttpCallAsync(context);
}

// New context-based method (actual implementation)
private static async Task<TResult> AssertHttpCallAsync<TResult>(
    HttpAssertContextInternal<TResult> context)
{
    // Clean implementation using context properties
    var payloadAsJson = EmbeddedFileLocalizer.LocalizeRequest(
        context.PayloadAsJson,
        context.CallerFilePath,
        context.CallingAssembly);
    // ... much cleaner! ✨
}
```

### 3. Updated Internal Methods

All internal methods now have two versions:
1. **Parameter-based** (for backward compatibility) - Converts parameters to context and calls context-based version
2. **Context-based** (actual implementation) - Does the work using the context object

Methods refactored:
- `AssertHttpCallAsync<TResult>` (master method)
- `AssertHttpCallInternalAsync<TResult>` (core implementation)
- `AssertCustomHttpCallAsync<TResult>` (custom assert support)
- `AssertHttpCallAsync` (non-generic version)

## Benefits

### ✅ Cleaner Code
- Method signatures are much simpler
- No more counting 13 parameters to make sure you pass them in the right order
- Easy to see what data is being used at a glance

### ✅ Easier Maintenance
- Adding new context properties doesn't require updating 20+ method signatures
- Changes propagate through one context object instead of many parameter lists
- Less chance of parameter order mistakes

### ✅ Better Testability
- Context objects are easier to mock and test
- Can create test helper methods that return pre-configured contexts

### ✅ 100% Backward Compatible
- All existing APIs work exactly as before
- No breaking changes
- Existing tests don't need updates

## Code Quality Improvements

### Before:
```csharp
// Hard to read, easy to make mistakes
await AssertHttpCallInternalAsync(
    client,
    url,
    payloadAsJson,
    expectedResult,
    filterFunc,
    httpMethod,
    differenceFunc,
    parameters,
    callingAssembly,
    payloadAsJsonParameterName,
    expectedResultParameterName,
    callerFilePath,
    isSuccessStatusCode,
    writResponse);
```

### After:
```csharp
// Clean and obvious
await AssertHttpCallInternalAsync(context);
```

## Next Steps

### Phase 2: Refactor More Internal Helpers
- Update `EmbeddedFileLocalizer` to work with contexts
- Refactor `WriteResponseService` to use contexts
- Update `OutputFormatter` methods

### Phase 3: Consolidate Context Usage
- Remove parameter-based overloads where safe
- Create context builders for common scenarios
- Add more factory methods for convenience

### Phase 4: Public API Enhancement
- Consider exposing context builders publicly
- Add fluent API for complex scenarios
- Create extension methods for common context patterns

## Example Usage (Public API)

The public API already uses the context objects:

```csharp
var context = new HttpAssertContext<User>
{
    FilterFunc = user => user with { Id = default, CreatedAt = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
};

var user = await client.AssertGetAsync<User>(
    "/api/users/{userId}",
    "expected-user.json",
    context
);
```

Internally, this gets converted to `HttpAssertContextInternal<User>` and flows through all the methods cleanly!

## Summary

This refactoring represents a major quality improvement to the codebase:
- ✅ **Reduced complexity**: 13 parameters → 1 context object
- ✅ **Improved readability**: Clear property names instead of positional params
- ✅ **Maintained compatibility**: All existing code works unchanged
- ✅ **Foundation for future**: Easy to add new features
- ✅ **Zero build warnings**: Clean compilation

The code is now much easier to understand, maintain, and extend! 🎉
