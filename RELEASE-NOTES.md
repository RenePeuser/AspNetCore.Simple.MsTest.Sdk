# Release Notes

## Upcoming Release - Assert.That Extensions

### New: AI-Friendly Assertion Library

We've added a comprehensive assertion library designed for maximum debuggability in both human and AI workflows.

#### What's new

**`Assert.That.*` - Modern assertion API**

- Mandatory `because` and `fix` parameters enforce test context documentation
- Rich structured failure output with sections: Test Info → Problem → Details → Context → Suggested Fix
- Automatic variable name capture via `CallerArgumentExpression` 
- Organized by category: Boolean, Numeric, String, Collection, DateTime, Exception, Type
- Beautiful console output with emoji decorators (opt-in plain text mode available)
- Clickable `file://` URIs in output for instant IDE navigation

#### Coverage

| Category | Assertions | Examples |
|----------|------------|----------|
| **Core** | Boolean, Null, Equality, Type | `IsTrue`, `IsNotNull`, `AreEqual`, `IsInstanceOfType` |
| **Numeric** | Comparison, Range, Math, Tolerance | `IsGreaterThan`, `IsInRange`, `IsPositive`, `IsWithinTolerance` |
| **String** | Content, Length, Pattern, Null | `Contains`, `StartsWith`, `Matches`, `IsEmpty` |
| **Collection** | Count, Contains, Equality, Predicate | `HasCount`, `Contains`, `AllMatch` |
| **DateTime** | Comparison, Range, Tolerance, Kind | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `IsUtc`, `IsLocal`, `IsUnspecified` |
| **DateTimeOffset** | Comparison, Range, Tolerance, Offset | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `HasOffset`, `IsUtc`, `IsLocal` |
| **Exception** | Throws, DoesNotThrow | `Throws<T>`, `DoesNotThrow` |

#### Example usage

```csharp
// Before
Assert.IsTrue(user.IsActive, "User should be active");

// After - with full context
Assert.That.IsTrue(user.IsActive,
    because: "Active users should have access to premium features",
    fix: "Check user activation logic in UserService.ActivateAsync()");
```

#### Why this matters

Traditional assertions fail with minimal context. `Assert.That.*` provides:

- **Why it matters** - business context via `because` parameter
- **How to fix it** - remediation guidance via `fix` parameter  
- **What failed** - captured expression shows `user.IsActive && user.IsVerified`
- **Where it failed** - clickable file path with line number
- **Additional suggestions** - context-specific fix options

This makes test failures **instantly debuggable** for humans and **fully parseable** for AI debugging tools.

#### Design decision: No `AiAssert` namespace

We considered creating a separate `AiAssert.*` namespace but decided against it:

- ✅ Mandatory context parameters already make assertions AI-friendly
- ✅ Structured output is easily parseable by tools
- ✅ Single API = no confusion about when to use what
- ✅ Human readability comes first, AI parsability follows naturally

#### Enhanced DateTime/DateTimeOffset assertions

**New DateTime Kind assertions:**
- `IsUtc()` - Validates `DateTime.Kind == DateTimeKind.Utc`
- `IsLocal()` - Validates `DateTime.Kind == DateTimeKind.Local`
- `IsUnspecified()` - Validates `DateTime.Kind == DateTimeKind.Unspecified`

These help catch common timezone bugs where APIs mix UTC and local times incorrectly.

**New DateTimeOffset support:**

Complete set of timezone-aware assertions for `DateTimeOffset`:
- `IsAfter()`, `IsBefore()` - Compare absolute points in time (timezone-aware)
- `IsInRange()` - Validate DateTimeOffset falls within range
- `IsCloseTo()` - Check if within tolerance (handles millisecond precision)
- `HasOffset()` - Validate specific UTC offset (e.g., `TimeSpan.FromHours(2)` for UTC+2)
- `IsUtc()` - Validates offset is `TimeSpan.Zero`
- `IsLocal()` - Validates offset matches current local timezone

**Why DateTimeOffset matters:**

`DateTimeOffset` is superior to `DateTime` for distributed systems because it preserves timezone context. These assertions help ensure:
- API responses include correct timezone offsets
- Comparisons work across different timezones
- Database timestamps maintain timezone information
- Event timestamps from different regions compare correctly

**Example usage:**

```csharp
// DateTime Kind validation
Assert.That.IsUtc(createdAt,
    because: "Database timestamps must be stored in UTC",
    fix: "Use DateTime.UtcNow instead of DateTime.Now");

// DateTimeOffset timezone validation
Assert.That.HasOffset(apiTimestamp, TimeSpan.FromHours(2),
    because: "API must return timestamps in Central European Time (UTC+2)",
    fix: "Configure the API timezone in appsettings.json");

// Timezone-aware comparison
Assert.That.IsAfter(eventTime, deadline,
    because: "Event must occur after the registration deadline",
    fix: "Adjust event scheduling to respect timezone differences");
```

**Test coverage:**
- 10 new tests for DateTime Kind assertions
- 22 new tests for DateTimeOffset assertions
- Full coverage of edge cases (negative offsets, millisecond precision, mixed timezones)

### Breaking changes

None - these are new additions to the SDK.

### Migration guide

Existing tests continue to work. To adopt `Assert.That.*`:

1. Replace traditional `Assert.*` calls with `Assert.That.*` equivalents
2. Add `because` parameter with business context
3. Add `fix` parameter with remediation guidance

Example:

```csharp
// Old
Assert.AreEqual(expected, actual, "Values should match");

// New
Assert.That.AreEqual(expected, actual,
    because: "API response should match expected snapshot after user creation",
    fix: "Check UserController.CreateAsync() return value mapping");
```

### Performance impact

Negligible. `CallerArgumentExpression` and caller info attributes are compile-time features with zero runtime cost. String formatting only happens when assertions fail.

### Future enhancements

- Optional JSON output mode for CI/CD tool integration
- Custom assertion message templates
- Integration with test result analyzers

---

**Full documentation**: See README.md → "Assert.That - AI-Friendly Assertions" section
