# Object Assertion Error Handling

## 🎯 Überblick

Error Handling wurde auch für **native Object Assertions** implementiert - für `Assert.That.ObjectsAreEqual<T>()` ohne HTTP Context.

## 📍 Wo implementiert?

**File:** `AssertExtensions/Assert.That.ObjectsAreEqual.cs`

```csharp
public static void ObjectsAreEqual<T>(this Assert _, ObjectAssertContext<T> context)
{
    try
    {
        ObjectsAreEqualInternal(context);
    }
    catch (AssertFailedException)
    {
        // Expected failures pass through
        throw;
    }
    catch (Exception exception)
    {
        // GLOBAL EXCEPTION HANDLER FOR OBJECT ASSERTIONS
        var errorOutput = BuildObjectAssertionError(context, exception);
        Assert.That.Fail(errorOutput);
    }
}
```

## 🔄 Flow

```
Assert.That.ObjectsAreEqual<T>(expected, current)
    ↓
ObjectsAreEqual(context)
    ↓
try {
    ObjectsAreEqualInternal(context)
    ↓
    AssertService.ObjectsAreEqual(context)
    ↓
    ComparisonStrategy.Compare(context)
}
catch (AssertFailedException) {
    throw;  // Expected - pass through
}
catch (Exception exception) {
    BuildObjectAssertionError(context, exception)
    ↓
    Assert.Fail(formatted error)
}
```

## 🎨 Error Output

### Beispiel für TestSdkProblemDetailsException

```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED ASSERTION ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
Project    : MyTest.Project
Class      : SomeTestClass
Method     : Should_Compare_Objects
Line       : 42

🔍 Assert Details
──────────────────────────────────────────────────────────────
Type       : MyResponseDto
Expected   : expectedResult
Current    : actualResult

⚠️ Exception Details
──────────────────────────────────────────────────────────────
Type       : AspNetCore.Simple.MsTest.Sdk.TestSdkProblemDetailsException
Message    : API returned ProblemDetails response

Stack Trace:
   at AspNetCore.Simple.MsTest.Sdk.Comparison.JsonComparisonStrategy...

💡 What This Means
──────────────────────────────────────────────────────────────
An unexpected error occurred during object comparison. This could indicate:

  • A bug in the Test SDK assertion logic
  • Serialization/deserialization issues
  • Invalid object structure
  • Type mismatch between expected and actual objects

If this appears to be a Test SDK bug, please report it with the
exception details and stack trace shown above.
══════════════════════════════════════════════════════════════
```

## 🔀 Unterschied zu HTTP Error Handling

### HTTP Assertions (AssertableHttpClient)
```csharp
await Client.AssertPostAsync<T>(...);
    ↓
ITestErrorHandlingStrategy (Strategy Pattern)
    ↓
    ├─ ProblemDetailsErrorHandler (HTTP-specific)
    ├─ NetworkErrorHandler (HTTP-specific)
    └─ DefaultErrorHandler (Generic)
    ↓
Voller HTTP Context verfügbar:
    - Request URL
    - HTTP Method
    - Status Code
    - Headers
    - Curl Command
```

### Object Assertions (Assert.That.ObjectsAreEqual)
```csharp
Assert.That.ObjectsAreEqual<T>(expected, actual);
    ↓
BuildObjectAssertionError (Simple Error Builder)
    ↓
Limitierter Context verfügbar:
    - Test Information
    - Type Information
    - Parameter Names
    - Exception Details
```

## 💡 Warum zwei verschiedene Ansätze?

### HTTP Assertions
- ✅ **Strategy Pattern** - Flexibel, erweiterbar
- ✅ **DI-basiert** - Alle Dependencies verfügbar
- ✅ **Rich Context** - HTTP Request/Response Details
- ✅ **Spezifische Handler** - ProblemDetails, Network, etc.
- ⚠️ **Komplexer** - Mehr Setup nötig

### Object Assertions
- ✅ **Einfach** - Inline Error Builder
- ✅ **Kein DI** - Funktioniert überall
- ✅ **Lightweight** - Keine Dependencies
- ✅ **Ausreichend** - Für Object Comparison genug Info
- ⚠️ **Weniger Context** - Kein HTTP Info

## 📋 Was wird gefangen?

### AssertFailedException
```csharp
catch (AssertFailedException)
{
    throw;  // Pass through - expected failures
}
```
- ✅ Normale Test-Failures gehen durch
- ✅ Keine unnötige Wrapping
- ✅ Test Explorer sieht die echten Failures

### Alle anderen Exceptions
```csharp
catch (Exception exception)
{
    var errorOutput = BuildObjectAssertionError(context, exception);
    Assert.That.Fail(errorOutput);
}
```
- ✅ `TestSdkProblemDetailsException` - API returned ProblemDetails
- ✅ `JsonException` - Serialization errors
- ✅ `InvalidOperationException` - SDK bugs
- ✅ Alle anderen - Unexpected errors

## 🎯 Verwendung

### Direkt
```csharp
var expected = new MyDto { Name = "Test" };
var actual = await GetDataAsync();

Assert.That.ObjectsAreEqual(expected, actual);
```

### Mit Ordering
```csharp
Assert.That.ObjectsAreEqual(
    expected, 
    actual,
    orderFunc: x => x.OrderBy(i => i.Id));
```

### Mit Custom Comparison
```csharp
Assert.That.ObjectsAreEqual(
    expected,
    actual,
    differenceFunc: diffs => diffs.Where(d => d.MemberPath != "Timestamp"));
```

## ✅ Benefits

### 1. Consistency
Beide Error Handling Ansätze:
- ✅ Strukturierte Ausgabe
- ✅ Test Information
- ✅ Exception Details
- ✅ Helpful Explanation
- ✅ Professional Look

### 2. Completeness
- ✅ HTTP Assertions covered (Strategy Pattern)
- ✅ Object Assertions covered (Simple Builder)
- ✅ Alle Exceptions gefangen
- ✅ Kein unformatierter Output mehr

### 3. Debugging
- ✅ Stack Trace sichtbar
- ✅ Exception Typ sichtbar
- ✅ Context Information
- ✅ Einfach zu reproducieren

## 🚀 Future Enhancements

### Possible für Object Assertions

1. **Strategy Pattern auch hier?**
   - Pro: Consistency mit HTTP Assertions
   - Pro: Erweiterbar
   - Con: Mehr Komplexität
   - Con: Weniger lightweight

2. **Specific Handler für JsonException?**
   - Könnte JSON Path zeigen
   - Könnte expected vs actual Type zeigen

3. **Specific Handler für TestSdkProblemDetailsException?**
   - Könnte ProblemDetails Details zeigen
   - Ähnlich wie ProblemDetailsErrorHandler

## 📊 Coverage

| Assertion Type | Error Handling | Strategy | Context |
|---------------|----------------|----------|---------|
| HTTP (Client.Assert*) | ✅ Full | Strategy Pattern | IHttpAssertContext |
| Object (Assert.That.ObjectsAreEqual) | ✅ Full | Simple Builder | ObjectAssertContext<T> |

**Beide Wege sind jetzt covered!** 🎉

## 🎓 Decision: Warum kein Strategy Pattern für Objects?

### Gründe gegen Strategy Pattern hier:

1. **Kein HTTP Context** - Die meisten spezifischen Handler brauchen HTTP Info
2. **Selten benötigt** - Object Assertions sind meist einfacher
3. **Lightweight halten** - Keine DI Dependencies nötig
4. **YAGNI** - You Aren't Gonna Need It (zumindest jetzt noch nicht)

### Wenn später nötig:
- ✅ Kann jederzeit hinzugefügt werden
- ✅ Interface bereits vorhanden (`ITestErrorHandler`)
- ✅ Strategy Pattern ready to use
- ✅ Backward compatible

**Für jetzt: Simple is better!** 👍
