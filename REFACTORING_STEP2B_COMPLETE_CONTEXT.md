# Refactoring Step 2B: Complete Context API

## Übersicht
Dieser Schritt vervollständigt das Context-Pattern für `ObjectsAreEqual` durch Hinzufügen der `ExpectedObjectAsJson` und `CurrentObject` Properties zum Context-Objekt.

## Motivation

### Vorher (Step 2A):
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = user => user with { Id = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    WriteResponse = true
};

// expectedObjectAsJson und currentObject mussten noch separat übergeben werden
Assert.That.ObjectsAreEqual("expected-user.json", currentUser, context);
```

**Problem:** Die zu vergleichenden Objekte waren noch nicht Teil des Context-Objekts.

### Nachher (Step 2B):
```csharp
var context = new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected-user.json",
    CurrentObject = currentUser,
    OrderFunc = user => user with { Id = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    WriteResponse = true
};

// Alles ist im Context! 🎉
Assert.That.ObjectsAreEqual(context);
```

**Lösung:** ALLE Parameter sind jetzt im Context-Objekt enthalten.

---

## Änderungen im Detail

### 1. ObjectAssertContext<T> erweitert

**Datei:** `ObjectAssertContext.cs`

```csharp
public sealed class ObjectAssertContext<T>
{
    /// <summary>
    /// The expected object as JSON string or file name.
    /// Can be a JSON string, a file name like "expected.json", or an embedded resource path.
    /// </summary>
    public string? ExpectedObjectAsJson { get; init; }

    /// <summary>
    /// The current/actual object to compare against the expected object.
    /// </summary>
    public T? CurrentObject { get; init; }

    // ... alle anderen Properties wie vorher ...
}
```

**Neue Properties:**
- `ExpectedObjectAsJson` - Der erwartete JSON String oder Dateiname
- `CurrentObject` - Das aktuelle/tatsächliche Objekt zum Vergleich

---

### 2. Neue Complete Context API Überladung

**Datei:** `Assert.That.ObjectAreEqual.cs`

```csharp
/// <summary>
/// Asserts that two objects are equal using a context object that contains all parameters.
/// This is the cleanest API - everything is in the context object.
/// </summary>
public static void ObjectsAreEqual<T>(this Assert assert,
                                      ObjectAssertContext<T> context)
{
    var expectedObjectAsJson = context.ExpectedObjectAsJson
        ?? throw new ArgumentNullException(nameof(context), 
            "ExpectedObjectAsJson must be set in the context");

    var currentObject = context.CurrentObject;

    ObjectsAreEqualInternal(assert, expectedObjectAsJson, currentObject!, context);
}
```

**Features:**
- Nimmt nur den Context-Parameter
- Validiert, dass `ExpectedObjectAsJson` gesetzt ist
- Extrahiert beide Objekte aus dem Context
- Ruft die interne Implementierung auf

---

### 3. Alte Context API bleibt erhalten

```csharp
// Diese Überladung bleibt für Backward Compatibility
public static void ObjectsAreEqual<T>(this Assert assert,
                                      string expectedObjectAsJson,
                                      T currentObject,
                                      ObjectAssertContext<T> context)
{
    ObjectsAreEqualInternal(assert, expectedObjectAsJson, currentObject, context);
}
```

---

## API-Level Übersicht

Jetzt haben wir **3 Levels** der API:

### Level 1: Original (Parameter-Heavy)
```csharp
Assert.That.ObjectsAreEqual(
    expectedObjectAsJson: "expected.json",
    currentObject: actual,
    orderFunc: x => x with { Id = default },
    title: "Comparison",
    callingAssembly: Assembly.GetExecutingAssembly(),
    differenceFunc: d => d,
    curl: string.Empty,
    parameters: Array.Empty<(string, object?)>(),
    writeResponse: false,
    expectedResultParameterName: "expected.json",
    currentResultParameterName: nameof(actual),
    callerFilePath: "Test.cs"
);
```

**Eigenschaften:**
- ❌ 12 Parameter
- ❌ Schwer lesbar
- ❌ Fehleranfällig
- ✅ Volle Kontrolle

---

### Level 2: Partial Context (Hybrid)
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = x => x with { Id = default },
    DifferenceFunc = d => d.Where(x => x.MemberPath != "timestamp")
};

Assert.That.ObjectsAreEqual("expected.json", actual, context);
```

**Eigenschaften:**
- ✅ Context für optionale Parameter
- ✅ Besser lesbar
- ⚠️ Zwei separate Parameter
- ✅ Backward compatible

---

### Level 3: Complete Context (NEW! ⭐)
```csharp
var context = new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected.json",
    CurrentObject = actual,
    OrderFunc = x => x with { Id = default },
    DifferenceFunc = d => d.Where(x => x.MemberPath != "timestamp")
};

Assert.That.ObjectsAreEqual(context);
```

**Eigenschaften:**
- ✅ Alles im Context
- ✅ Maximum Klarheit
- ✅ Wiederverwendbar
- ✅ IntelliSense-freundlich
- ✅ Selbst-dokumentierend
- ✅ Backward compatible

---

## Vorteile der Complete Context API

### 1. 📦 Alles in einem Objekt
```csharp
// Alle Parameter sind logisch gruppiert
var context = new ObjectAssertContext<User>
{
    // Was wird verglichen?
    ExpectedObjectAsJson = "expected-user.json",
    CurrentObject = actualUser,
    
    // Wie wird verglichen?
    OrderFunc = user => user with { Id = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    
    // Zusätzliche Optionen
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true,
    Title = "User comparison"
};
```

### 2. 🔄 Wiederverwendbarkeit
```csharp
// Context Builder Pattern
public static class AssertContexts
{
    public static ObjectAssertContext<User> ForUserComparison(
        string expectedFile, 
        User actual)
    {
        return new ObjectAssertContext<User>
        {
            ExpectedObjectAsJson = expectedFile,
            CurrentObject = actual,
            OrderFunc = user => user with 
            { 
                Id = default, 
                CreatedAt = default 
            },
            DifferenceFunc = diffs => diffs.Where(d => 
                !d.MemberPath.Contains("_metadata")
            )
        };
    }
}

// Verwendung
[TestMethod]
public void TestUser()
{
    var user = GetUser();
    Assert.That.ObjectsAreEqual(
        AssertContexts.ForUserComparison("expected.json", user)
    );
}
```

### 3. 🧪 Testdaten-Factories
```csharp
public class UserTestContexts
{
    public static ObjectAssertContext<User> StandardUser(User actual) =>
        new()
        {
            ExpectedObjectAsJson = "standard-user.json",
            CurrentObject = actual,
            OrderFunc = NormalizeUser,
            DifferenceFunc = IgnoreTimestamps
        };

    public static ObjectAssertContext<User> AdminUser(User actual) =>
        new()
        {
            ExpectedObjectAsJson = "admin-user.json",
            CurrentObject = actual,
            OrderFunc = NormalizeUser,
            DifferenceFunc = IgnoreTimestamps.And(IgnorePermissions)
        };

    private static User NormalizeUser(User u) => 
        u with { Id = default, CreatedAt = default };
    
    private static Func<IEnumerable<Difference>, IEnumerable<Difference>> 
        IgnoreTimestamps => 
        d => d.Where(x => !x.MemberPath.Contains("Timestamp"));
}
```

### 4. 💡 Fluent Extensions (Zukünftig möglich)
```csharp
// Mögliche zukünftige Extension Methods
var context = new ObjectAssertContext<User>()
    .WithExpected("expected.json")
    .WithCurrent(actualUser)
    .IgnoringProperty(u => u.Id)
    .IgnoringProperty(u => u.CreatedAt)
    .WithParameters(("{userId}", 123))
    .WritingResponse();

Assert.That.ObjectsAreEqual(context);
```

---

## Migration Guide

### Von Level 1 → Level 3

**Vorher (Level 1):**
```csharp
Assert.That.ObjectsAreEqual(
    "expected-user.json",
    currentUser,
    orderFunc: user => user with { Id = default },
    title: "User comparison",
    callingAssembly: Assembly.GetExecutingAssembly(),
    differenceFunc: diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    curl: string.Empty,
    parameters: new[] { ("{userId}", 123) },
    writeResponse: true
);
```

**Nachher (Level 3):**
```csharp
Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected-user.json",
    CurrentObject = currentUser,
    OrderFunc = user => user with { Id = default },
    Title = "User comparison",
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "timestamp"),
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
});
```

**Ersparnis:** 9 Parameter-Namen weniger, 1 Context-Objekt

---

### Von Level 2 → Level 3

**Vorher (Level 2):**
```csharp
var context = new ObjectAssertContext<User>
{
    OrderFunc = user => user with { Id = default },
    WriteResponse = true
};

Assert.That.ObjectsAreEqual("expected-user.json", currentUser, context);
```

**Nachher (Level 3):**
```csharp
var context = new ObjectAssertContext<User>
{
    ExpectedObjectAsJson = "expected-user.json",  // ← Neu im Context
    CurrentObject = currentUser,                   // ← Neu im Context
    OrderFunc = user => user with { Id = default },
    WriteResponse = true
};

Assert.That.ObjectsAreEqual(context);  // ← Nur noch Context!
```

**Änderungen:**
1. Füge `ExpectedObjectAsJson` zum Context hinzu
2. Füge `CurrentObject` zum Context hinzu
3. Entferne die separaten Parameter beim Assert-Aufruf

---

## Real-World Beispiele

### Beispiel 1: Einfacher Test
```csharp
[TestMethod]
public void GetUser_ReturnsCorrectUser()
{
    // Arrange
    var userId = 123;
    
    // Act
    var actual = _userService.GetUser(userId);
    
    // Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "expected-user.json",
        CurrentObject = actual
    });
}
```

### Beispiel 2: Mit Normalisierung
```csharp
[TestMethod]
public void GetUsers_ReturnsAllUsers()
{
    // Arrange & Act
    var actual = _userService.GetAllUsers();
    
    // Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<List<User>>
    {
        ExpectedObjectAsJson = "expected-users.json",
        CurrentObject = actual,
        OrderFunc = users => users
            .OrderBy(u => u.Id)
            .Select(u => u with { CreatedAt = default })
            .ToList()
    });
}
```

### Beispiel 3: Mit Parametern
```csharp
[TestMethod]
public void GetUserById_WithDynamicData()
{
    // Arrange
    var userId = 456;
    var timestamp = DateTime.UtcNow;
    
    // Act
    var actual = _userService.GetUser(userId);
    
    // Assert
    Assert.That.ObjectsAreEqual(new ObjectAssertContext<User>
    {
        ExpectedObjectAsJson = "user-template.json",
        CurrentObject = actual,
        Parameters = new[]
        {
            ("{userId}", userId),
            ("{timestamp}", timestamp.ToString("O"))
        }
    });
}
```

---

## Best Practices

### ✅ DO: Use Context Builders

```csharp
public static class UserContexts
{
    public static ObjectAssertContext<User> Standard(User actual, string expectedFile)
    {
        return new ObjectAssertContext<User>
        {
            ExpectedObjectAsJson = expectedFile,
            CurrentObject = actual,
            OrderFunc = user => user with { Id = default, CreatedAt = default },
            DifferenceFunc = diffs => diffs.Where(d => 
                !d.MemberPath.Contains("_links")
            )
        };
    }
}
```

### ✅ DO: Group Related Tests

```csharp
public class UserTests
{
    private readonly ObjectAssertContext<User> _baseContext = new()
    {
        OrderFunc = user => user with { Id = default },
        DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "UpdatedAt")
    };

    [TestMethod]
    public void GetUser_Standard()
    {
        var user = _service.GetUser(1);
        
        Assert.That.ObjectsAreEqual(_baseContext with
        {
            ExpectedObjectAsJson = "user-1.json",
            CurrentObject = user
        });
    }
}
```

### ❌ DON'T: Create Incomplete Contexts

```csharp
// BAD: Missing required fields
var context = new ObjectAssertContext<User>
{
    OrderFunc = x => x
};

Assert.That.ObjectsAreEqual(context);  // ❌ Throws ArgumentNullException!
```

### ❌ DON'T: Mix Levels Without Reason

```csharp
// BAD: Why use Level 2 when Level 3 is cleaner?
var context = new ObjectAssertContext<User>
{
    OrderFunc = x => x
};

Assert.That.ObjectsAreEqual("expected.json", actual, context);  
// Better: Use Level 3!
```

---

## Zusammenfassung

### Was wurde hinzugefügt:

1. **ObjectAssertContext<T>** erhielt zwei neue Properties:
   - `ExpectedObjectAsJson` - Der erwartete JSON/Dateiname
   - `CurrentObject` - Das aktuelle Objekt

2. **Neue Überladung** `ObjectsAreEqual(context)`:
   - Nimmt nur den Context
   - Extrahiert alle Werte aus dem Context
   - Validiert erforderliche Felder

3. **Alte Überladungen bleiben** für Backward Compatibility:
   - Level 1 (12 Parameter)
   - Level 2 (Partial Context)
   - Level 3 (Complete Context) ⭐ NEW

### Vorteile:

- ✅ **Maximale Klarheit** - Alles an einem Ort
- ✅ **Wiederverwendbarkeit** - Context Builder Pattern
- ✅ **Lesbarkeit** - Property-Namen statt Parameter-Position
- ✅ **IntelliSense** - IDE hilft beim Erstellen des Context
- ✅ **Selbst-dokumentierend** - Code erklärt sich selbst
- ✅ **100% Backward Compatible** - Kein Breaking Change

### Build Status:

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## Nächste Schritte (Phase 3)

Mit dieser sauberen Context-API können wir nun:

1. **EmbeddedFileLocalizer** refactoren
   - Context-basierte Überladungen hinzufügen
   - Direkt den Context durchreichen

2. **OutputFormatter** refactoren
   - `OutputContext` erstellen
   - Reduziert Parameter-Chaos

3. **WriteResponseService** refactoren
   - Context-basierte Methoden

Die Clean Context API ist jetzt vollständig und bereit für Phase 3! 🚀
