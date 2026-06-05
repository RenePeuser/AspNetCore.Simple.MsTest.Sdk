# Complete Assert Extension Library
**Pattern inspired by `Throw.If*` - Fluent, intuitive, and comprehensive**

---

## 🎯 **Core Design Pattern**

```csharp
Assert.That.[Condition]<T>(
    value: T,
    because: string,
    fix: string,
    [CallerFilePath] string callerFilePath = "",
    [CallerMemberName] string callerMemberName = "",
    [CallerLineNumber] int callerLineNumber = 0
)
```

All methods use `CallerArgumentExpression` to capture variable names automatically.

---

## 📋 **1. NULL CHECKS**

### 1.1 Basic Null
```csharp
// Value must be null
Assert.That.IsNull<T>(
    value: T?,
    because: string,
    fix: string
) where T : class

// Value must NOT be null
Assert.That.IsNotNull<T>(
    value: T?,
    because: string,
    fix: string
) where T : class
```

### 1.2 Null with Fallback
```csharp
// Value must be null, otherwise use default
Assert.That.IsNullOrDefault<T>(
    value: T?,
    defaultValue: T,
    because: string,
    fix: string
) where T : class
```

---

## 📋 **2. BOOLEAN CHECKS**

```csharp
// Condition must be true
Assert.That.IsTrue(
    condition: bool,
    because: string,
    fix: string
)

// Condition must be false
Assert.That.IsFalse(
    condition: bool,
    because: string,
    fix: string
)

// All conditions must be true
Assert.That.AllTrue(
    conditions: params bool[],
    because: string,
    fix: string
)

// At least one condition must be true
Assert.That.AnyTrue(
    conditions: params bool[],
    because: string,
    fix: string
)

// None of the conditions should be true
Assert.That.NoneTrue(
    conditions: params bool[],
    because: string,
    fix: string
)
```

---

## 📋 **3. EQUALITY CHECKS**

### 3.1 Primitive Equality
```csharp
// Values must be equal
Assert.That.AreEqual<T>(
    expected: T,
    actual: T,
    because: string,
    fix: string
)

// Values must NOT be equal
Assert.That.AreNotEqual<T>(
    expected: T,
    actual: T,
    because: string,
    fix: string
)

// Values must be equal (with comparer)
Assert.That.AreEqual<T>(
    expected: T,
    actual: T,
    comparer: IEqualityComparer<T>,
    because: string,
    fix: string
)
```

### 3.2 Reference Equality
```csharp
// Must be same reference
Assert.That.AreSame<T>(
    expected: T,
    actual: T,
    because: string,
    fix: string
) where T : class

// Must NOT be same reference
Assert.That.AreNotSame<T>(
    expected: T,
    actual: T,
    because: string,
    fix: string
) where T : class
```

### 3.3 Object Equality (Enhanced Existing)
```csharp
// Deep equality with differences
Assert.That.ObjectsAreEqual<T>(
    expected: T,
    actual: T,
    because: string,
    fix: string
)

// With difference function
Assert.That.ObjectsAreEqual<T>(
    expected: T,
    actual: T,
    differenceFunc: Func<ImmutableList<Difference>, IEnumerable<Difference>>,
    because: string,
    fix: string
)
```

---

## 📋 **4. NUMERIC COMPARISONS**

### 4.1 Greater Than
```csharp
// Value > threshold
Assert.That.IsGreaterThan<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>

// Value >= threshold
Assert.That.IsGreaterThanOrEqual<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>
```

### 4.2 Less Than
```csharp
// Value < threshold
Assert.That.IsLessThan<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>

// Value <= threshold
Assert.That.IsLessThanOrEqual<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>
```

### 4.3 Range Checks
```csharp
// min <= value <= max
Assert.That.IsInRange<T>(
    value: T,
    min: T,
    max: T,
    because: string,
    fix: string
) where T : IComparable<T>

// value < min OR value > max
Assert.That.IsOutOfRange<T>(
    value: T,
    min: T,
    max: T,
    because: string,
    fix: string
) where T : IComparable<T>

// min < value < max (exclusive)
Assert.That.IsInRangeExclusive<T>(
    value: T,
    min: T,
    max: T,
    because: string,
    fix: string
) where T : IComparable<T>
```

### 4.4 Tolerance-Based
```csharp
// |actual - expected| <= tolerance
Assert.That.IsCloseTo(
    actual: double,
    expected: double,
    tolerance: double,
    because: string,
    fix: string
)

// For decimals
Assert.That.IsCloseTo(
    actual: decimal,
    expected: decimal,
    tolerance: decimal,
    because: string,
    fix: string
)

// For floats
Assert.That.IsCloseTo(
    actual: float,
    expected: float,
    tolerance: float,
    because: string,
    fix: string
)
```

### 4.5 Sign Checks
```csharp
// Value > 0
Assert.That.IsPositive<T>(
    value: T,
    because: string,
    fix: string
) where T : IComparable<T>

// Value < 0
Assert.That.IsNegative<T>(
    value: T,
    because: string,
    fix: string
) where T : IComparable<T>

// Value == 0
Assert.That.IsZero<T>(
    value: T,
    because: string,
    fix: string
) where T : IComparable<T>

// Value != 0
Assert.That.IsNotZero<T>(
    value: T,
    because: string,
    fix: string
) where T : IComparable<T>
```

---

## 📋 **5. STRING CHECKS**

### 5.1 Null/Empty/Whitespace
```csharp
// String is null or empty
Assert.That.IsNullOrEmpty(
    value: string?,
    because: string,
    fix: string
)

// String is NOT null or empty
Assert.That.IsNotNullOrEmpty(
    value: string?,
    because: string,
    fix: string
)

// String is null or whitespace
Assert.That.IsNullOrWhiteSpace(
    value: string?,
    because: string,
    fix: string
)

// String is NOT null or whitespace
Assert.That.IsNotNullOrWhiteSpace(
    value: string?,
    because: string,
    fix: string
)
```

### 5.2 Content Checks
```csharp
// String contains substring
Assert.That.Contains(
    text: string,
    substring: string,
    because: string,
    fix: string,
    comparison: StringComparison = StringComparison.Ordinal
)

// String does NOT contain substring
Assert.That.DoesNotContain(
    text: string,
    substring: string,
    because: string,
    fix: string,
    comparison: StringComparison = StringComparison.Ordinal
)

// String starts with prefix
Assert.That.StartsWith(
    text: string,
    prefix: string,
    because: string,
    fix: string,
    comparison: StringComparison = StringComparison.Ordinal
)

// String ends with suffix
Assert.That.EndsWith(
    text: string,
    suffix: string,
    because: string,
    fix: string,
    comparison: StringComparison = StringComparison.Ordinal
)
```

### 5.3 Pattern Matching
```csharp
// String matches regex pattern
Assert.That.Matches(
    text: string,
    pattern: string,
    because: string,
    fix: string,
    options: RegexOptions = RegexOptions.None
)

// String does NOT match regex pattern
Assert.That.DoesNotMatch(
    text: string,
    pattern: string,
    because: string,
    fix: string,
    options: RegexOptions = RegexOptions.None
)
```

### 5.4 Length Checks
```csharp
// String has exact length
Assert.That.HasLength(
    text: string,
    expectedLength: int,
    because: string,
    fix: string
)

// String length is in range
Assert.That.HasLengthInRange(
    text: string,
    minLength: int,
    maxLength: int,
    because: string,
    fix: string
)

// String is longer than minimum
Assert.That.IsLongerThan(
    text: string,
    minLength: int,
    because: string,
    fix: string
)

// String is shorter than maximum
Assert.That.IsShorterThan(
    text: string,
    maxLength: int,
    because: string,
    fix: string
)
```

### 5.5 Case Checks
```csharp
// String is all uppercase
Assert.That.IsUpperCase(
    text: string,
    because: string,
    fix: string
)

// String is all lowercase
Assert.That.IsLowerCase(
    text: string,
    because: string,
    fix: string
)
```

---

## 📋 **6. COLLECTION CHECKS**

### 6.1 Null/Empty
```csharp
// Collection is null
Assert.That.IsNull<T>(
    collection: IEnumerable<T>?,
    because: string,
    fix: string
)

// Collection is NOT null
Assert.That.IsNotNull<T>(
    collection: IEnumerable<T>?,
    because: string,
    fix: string
)

// Collection is empty
Assert.That.IsEmpty<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
)

// Collection is NOT empty
Assert.That.IsNotEmpty<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
)

// Collection is null or empty
Assert.That.IsNullOrEmpty<T>(
    collection: IEnumerable<T>?,
    because: string,
    fix: string
)

// Collection is NOT null or empty
Assert.That.IsNotNullOrEmpty<T>(
    collection: IEnumerable<T>?,
    because: string,
    fix: string
)
```

### 6.2 Count Checks
```csharp
// Collection has exact count
Assert.That.HasCount<T>(
    collection: IEnumerable<T>,
    expectedCount: int,
    because: string,
    fix: string
)

// Collection count is in range
Assert.That.HasCountInRange<T>(
    collection: IEnumerable<T>,
    minCount: int,
    maxCount: int,
    because: string,
    fix: string
)

// Collection count is greater than
Assert.That.HasCountGreaterThan<T>(
    collection: IEnumerable<T>,
    minCount: int,
    because: string,
    fix: string
)

// Collection count is less than
Assert.That.HasCountLessThan<T>(
    collection: IEnumerable<T>,
    maxCount: int,
    because: string,
    fix: string
)
```

### 6.3 Contains Checks
```csharp
// Collection contains item
Assert.That.Contains<T>(
    collection: IEnumerable<T>,
    item: T,
    because: string,
    fix: string
)

// Collection contains item (with comparer)
Assert.That.Contains<T>(
    collection: IEnumerable<T>,
    item: T,
    comparer: IEqualityComparer<T>,
    because: string,
    fix: string
)

// Collection does NOT contain item
Assert.That.DoesNotContain<T>(
    collection: IEnumerable<T>,
    item: T,
    because: string,
    fix: string
)

// Collection contains all items
Assert.That.ContainsAll<T>(
    collection: IEnumerable<T>,
    items: IEnumerable<T>,
    because: string,
    fix: string
)

// Collection contains any of the items
Assert.That.ContainsAny<T>(
    collection: IEnumerable<T>,
    items: IEnumerable<T>,
    because: string,
    fix: string
)
```

### 6.4 Predicate Checks
```csharp
// All items satisfy predicate
Assert.That.All<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// Any item satisfies predicate
Assert.That.Any<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// No item satisfies predicate
Assert.That.None<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// Exactly N items satisfy predicate
Assert.That.Exactly<T>(
    collection: IEnumerable<T>,
    count: int,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// Single item satisfies predicate
Assert.That.Single<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)
```

### 6.5 Uniqueness
```csharp
// All items are unique
Assert.That.AllUnique<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
)

// All items are unique (by selector)
Assert.That.AllUniqueBy<T, TKey>(
    collection: IEnumerable<T>,
    keySelector: Func<T, TKey>,
    because: string,
    fix: string
)

// Has duplicates
Assert.That.HasDuplicates<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
)

// No duplicates
Assert.That.NoDuplicates<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
)
```

### 6.6 Ordering
```csharp
// Collection is ordered ascending
Assert.That.IsOrdered<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
) where T : IComparable<T>

// Collection is ordered descending
Assert.That.IsOrderedDescending<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
) where T : IComparable<T>

// Collection is ordered by selector
Assert.That.IsOrderedBy<T, TKey>(
    collection: IEnumerable<T>,
    keySelector: Func<T, TKey>,
    because: string,
    fix: string
) where TKey : IComparable<TKey>
```

### 6.7 Equality
```csharp
// Collections are equal (order matters)
Assert.That.AreEqual<T>(
    expected: IEnumerable<T>,
    actual: IEnumerable<T>,
    because: string,
    fix: string
)

// Collections are equivalent (order doesn't matter)
Assert.That.AreEquivalent<T>(
    expected: IEnumerable<T>,
    actual: IEnumerable<T>,
    because: string,
    fix: string
)

// Collections are NOT equal
Assert.That.AreNotEqual<T>(
    expected: IEnumerable<T>,
    actual: IEnumerable<T>,
    because: string,
    fix: string
)
```

---

## 📋 **7. TYPE CHECKS**

```csharp
// Object is of exact type
Assert.That.IsOfType<TExpected>(
    obj: object,
    because: string,
    fix: string
)

// Object is NOT of type
Assert.That.IsNotOfType<TNotExpected>(
    obj: object,
    because: string,
    fix: string
)

// Object is assignable to type
Assert.That.IsAssignableTo<TExpected>(
    obj: object,
    because: string,
    fix: string
)

// Object is NOT assignable to type
Assert.That.IsNotAssignableTo<TNotExpected>(
    obj: object,
    because: string,
    fix: string
)

// Type implements interface
Assert.That.Implements<TInterface>(
    type: Type,
    because: string,
    fix: string
)

// Type does NOT implement interface
Assert.That.DoesNotImplement<TInterface>(
    type: Type,
    because: string,
    fix: string
)

// Type inherits from base
Assert.That.InheritsFrom<TBase>(
    type: Type,
    because: string,
    fix: string
)
```

---

## 📋 **8. EXCEPTION CHECKS**

```csharp
// Action throws specific exception
Assert.That.Throws<TException>(
    action: Action,
    because: string,
    fix: string
) where TException : Exception

// Action throws with message
Assert.That.Throws<TException>(
    action: Action,
    expectedMessage: string,
    because: string,
    fix: string
) where TException : Exception

// Action throws with message containing
Assert.That.ThrowsWithMessageContaining<TException>(
    action: Action,
    messageSubstring: string,
    because: string,
    fix: string
) where TException : Exception

// Async action throws
Assert.That.ThrowsAsync<TException>(
    action: Func<Task>,
    because: string,
    fix: string
) where TException : Exception

// Action does NOT throw
Assert.That.DoesNotThrow(
    action: Action,
    because: string,
    fix: string
)

// Async action does NOT throw
Assert.That.DoesNotThrowAsync(
    action: Func<Task>,
    because: string,
    fix: string
)

// Action throws any exception
Assert.That.ThrowsAny(
    action: Action,
    because: string,
    fix: string
)
```

---

## 📋 **9. DATE/TIME CHECKS**

```csharp
// DateTime is after
Assert.That.IsAfter(
    actual: DateTime,
    expected: DateTime,
    because: string,
    fix: string
)

// DateTime is before
Assert.That.IsBefore(
    actual: DateTime,
    expected: DateTime,
    because: string,
    fix: string
)

// DateTime is within range
Assert.That.IsInRange(
    actual: DateTime,
    start: DateTime,
    end: DateTime,
    because: string,
    fix: string
)

// DateTime is close to (within tolerance)
Assert.That.IsCloseTo(
    actual: DateTime,
    expected: DateTime,
    tolerance: TimeSpan,
    because: string,
    fix: string
)

// DateTime is today
Assert.That.IsToday(
    actual: DateTime,
    because: string,
    fix: string
)

// DateTime is in the past
Assert.That.IsInPast(
    actual: DateTime,
    because: string,
    fix: string
)

// DateTime is in the future
Assert.That.IsInFuture(
    actual: DateTime,
    because: string,
    fix: string
)

// TimeSpan is longer than
Assert.That.IsLongerThan(
    actual: TimeSpan,
    minimum: TimeSpan,
    because: string,
    fix: string
)

// TimeSpan is shorter than
Assert.That.IsShorterThan(
    actual: TimeSpan,
    maximum: TimeSpan,
    because: string,
    fix: string
)
```

---

## 📋 **10. GUID CHECKS**

```csharp
// GUID is empty
Assert.That.IsEmpty(
    guid: Guid,
    because: string,
    fix: string
)

// GUID is NOT empty
Assert.That.IsNotEmpty(
    guid: Guid,
    because: string,
    fix: string
)

// GUID equals
Assert.That.AreEqual(
    expected: Guid,
    actual: Guid,
    because: string,
    fix: string
)
```

---

## 📋 **11. FILE/PATH CHECKS**

```csharp
// File exists
Assert.That.FileExists(
    path: string,
    because: string,
    fix: string
)

// File does NOT exist
Assert.That.FileDoesNotExist(
    path: string,
    because: string,
    fix: string
)

// Directory exists
Assert.That.DirectoryExists(
    path: string,
    because: string,
    fix: string
)

// Directory does NOT exist
Assert.That.DirectoryDoesNotExist(
    path: string,
    because: string,
    fix: string
)

// Path is absolute
Assert.That.IsAbsolutePath(
    path: string,
    because: string,
    fix: string
)

// Path is relative
Assert.That.IsRelativePath(
    path: string,
    because: string,
    fix: string
)

// File has extension
Assert.That.HasExtension(
    path: string,
    expectedExtension: string,
    because: string,
    fix: string
)

// File size is within range
Assert.That.FileSizeInRange(
    path: string,
    minBytes: long,
    maxBytes: long,
    because: string,
    fix: string
)
```

---

## 📋 **12. JSON CHECKS**

```csharp
// String is valid JSON
Assert.That.IsValidJson(
    json: string,
    because: string,
    fix: string
)

// JSON contains path
Assert.That.JsonContainsPath(
    json: string,
    jsonPath: string,
    because: string,
    fix: string
)

// JSON path equals value
Assert.That.JsonPathEquals(
    json: string,
    jsonPath: string,
    expectedValue: object,
    because: string,
    fix: string
)

// JSON matches schema
Assert.That.MatchesJsonSchema(
    json: string,
    schemaJson: string,
    because: string,
    fix: string
)
```

---

## 📋 **13. DICTIONARY/KEY-VALUE CHECKS**

```csharp
// Dictionary contains key
Assert.That.ContainsKey<TKey, TValue>(
    dictionary: IDictionary<TKey, TValue>,
    key: TKey,
    because: string,
    fix: string
)

// Dictionary does NOT contain key
Assert.That.DoesNotContainKey<TKey, TValue>(
    dictionary: IDictionary<TKey, TValue>,
    key: TKey,
    because: string,
    fix: string
)

// Dictionary contains value
Assert.That.ContainsValue<TKey, TValue>(
    dictionary: IDictionary<TKey, TValue>,
    value: TValue,
    because: string,
    fix: string
)

// Dictionary key has value
Assert.That.KeyHasValue<TKey, TValue>(
    dictionary: IDictionary<TKey, TValue>,
    key: TKey,
    expectedValue: TValue,
    because: string,
    fix: string
)

// Dictionary has count
Assert.That.HasCount<TKey, TValue>(
    dictionary: IDictionary<TKey, TValue>,
    expectedCount: int,
    because: string,
    fix: string
)
```

---

## 📋 **14. ASYNC/TASK CHECKS**

```csharp
// Task completes within timeout
Assert.That.CompletesWithin(
    task: Task,
    timeout: TimeSpan,
    because: string,
    fix: string
)

// Task is completed
Assert.That.IsCompleted(
    task: Task,
    because: string,
    fix: string
)

// Task is NOT completed
Assert.That.IsNotCompleted(
    task: Task,
    because: string,
    fix: string
)

// Task is faulted
Assert.That.IsFaulted(
    task: Task,
    because: string,
    fix: string
)

// Task is canceled
Assert.That.IsCanceled(
    task: Task,
    because: string,
    fix: string
)
```

---

## 📋 **15. CUSTOM/PREDICATE CHECKS**

```csharp
// Value satisfies predicate
Assert.That.Satisfies<T>(
    value: T,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// Value does NOT satisfy predicate
Assert.That.DoesNotSatisfy<T>(
    value: T,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
)

// Custom assertion with message builder
Assert.That.Custom<T>(
    value: T,
    validation: Func<T, (bool success, string? errorDetails)>,
    because: string,
    fix: string
)
```

---

## 📋 **16. FAIL (Enhanced)**

```csharp
// Explicit test failure
Assert.That.Fail(
    message: string,
    because: string,
    fix: string
)

// Conditional failure
Assert.That.FailIf(
    condition: bool,
    message: string,
    because: string,
    fix: string
)

// Fail unless
Assert.That.FailUnless(
    condition: bool,
    message: string,
    because: string,
    fix: string
)
```

---

## 🚀 **BONUS: FLUENT COMBINATIONS**

```csharp
// Chain multiple assertions
Assert.That.Value(user.Age)
    .IsNotNull("User age is required for adult verification", "Set user.Age in the registration flow")
    .IsGreaterThan(18, "Users must be adults", "Add age validation")
    .IsLessThan(120, "Age must be realistic", "Check data input validation");

// Collection fluent
Assert.That.Collection(users)
    .IsNotEmpty("Must have users in test data", "Seed test database")
    .HasCount(5, "Test expects 5 users", "Update test data setup")
    .All(u => u.IsActive, "All test users should be active", "Activate users in seed data");

// String fluent
Assert.That.Text(email)
    .IsNotNullOrEmpty("Email is required", "Add email to user object")
    .Contains("@", "Email must be valid format", "Validate email format")
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$", "Email regex validation", "Use EmailValidator class");
```

---

## 📊 **SUMMARY**

**Total Assert Methods: 150+**

Categories:
- Null checks: 3
- Boolean checks: 5
- Equality checks: 8
- Numeric comparisons: 18
- String checks: 20
- Collection checks: 35
- Type checks: 7
- Exception checks: 8
- Date/Time checks: 9
- GUID checks: 3
- File/Path checks: 8
- JSON checks: 4
- Dictionary checks: 5
- Async/Task checks: 5
- Custom/Predicate: 3
- Fail: 3
- **Plus fluent combinations!**

Every method follows the pattern:
✅ Descriptive name
✅ Generic where applicable
✅ `because` parameter (context)
✅ `fix` parameter (resolution)
✅ CallerArgumentExpression for auto variable names
✅ Consistent beautiful output format
