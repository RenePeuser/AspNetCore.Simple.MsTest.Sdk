# Complete Context API for HTTP Assertions

## Übersicht
Alle HTTP-Assert-Methoden (GET, POST, PUT, PATCH, DELETE) haben jetzt eine **Complete Context API (Level 3)**, wo ALLE Parameter im Context-Objekt enthalten sind.

---

## API-Evolution

### Level 1: Parameter-Heavy (Original)
```csharp
var user = await client.AssertGetAsync<User>(
    "/api/users/{userId}",
    "expected-user.json",
    new[] { ("{userId}", 123) },
    Assembly.GetCallingAssembly(),
    writeResponse: true,
    expectedResultParameterName: "expected-user.json",
    callerFilePath: "UserTests.cs"
);
```

**Probleme:**
- 😱 7+ Parameter
- 🤯 Schwer zu lesen
- 😓 Parameter-Reihenfolge wichtig

---

### Level 2: Partial Context (Hybrid)
```csharp
var context = new HttpAssertContext<User>
{
    Parameters = new[] { ("{userId}", 123) },
    FilterFunc = user => user with { Id = default },
    WriteResponse = true
};

var user = await client.AssertGetAsync<User>(
    "/api/users/{userId}",
    "expected-user.json",
    context
);
```

**Verbesserungen:**
- ✅ Context für optionale Parameter
- ⚠️ Client, URL, ExpectedResult noch separat

---

### Level 3: Complete Context ⭐ (NEU!)
```csharp
var context = new HttpAssertContext<User>
{
    Client = httpClient,
    Url = "/api/users/{userId}",
    ExpectedResult = "expected-user.json",
    Parameters = new[] { ("{userId}", 123) },
    FilterFunc = user => user with { Id = default },
    WriteResponse = true
};

var user = await HttpClientAssertExtensions.AssertGetAsync(context);
```

**Vorteile:**
- ✅ Alles im Context!
- ✅ Nur 1 Parameter
- ✅ Maximum Klarheit
- ✅ IntelliSense-friendly
- ✅ Wiederverwendbar

---

## Verfügbare Methoden (Level 3)

### GET

```csharp
// Non-generic
public static Task AssertGetAsync(HttpAssertContext context)

// Generic
public static Task<TResult> AssertGetAsync<TResult>(HttpAssertContext<TResult> context)
```

#### Beispiel:
```csharp
var context = new HttpAssertContext<User>
{
    Client = _httpClient,
    Url = "/api/users/123",
    ExpectedResult = "expected-user.json",
    FilterFunc = user => user with { Id = default, CreatedAt = default },
    DifferenceFunc = diffs => diffs.Where(d => d.MemberPath != "UpdatedAt"),
    WriteResponse = true
};

var user = await HttpClientAssertExtensions.AssertGetAsync(context);
```

---

### POST

```csharp
// Non-generic
public static Task AssertPostAsync(HttpAssertContext context)

// Generic
public static Task<TResult> AssertPostAsync<TResult>(HttpAssertContext<TResult> context)
```

#### Beispiel:
```csharp
var context = new HttpAssertContext<User>
{
    Client = _httpClient,
    Url = "/api/users",
    PayloadAsJson = "new-user-request.json",
    ExpectedResult = "created-user-response.json",
    FilterFunc = user => user with { Id = default, CreatedAt = default },
    WriteResponse = true
};

var createdUser = await HttpClientAssertExtensions.AssertPostAsync(context);
```

---

### PUT

```csharp
// Non-generic
public static Task AssertPutAsync(HttpAssertContext context)

// Generic
public static Task<TResult> AssertPutAsync<TResult>(HttpAssertContext<TResult> context)
```

#### Beispiel:
```csharp
var context = new HttpAssertContext<User>
{
    Client = _httpClient,
    Url = "/api/users/123",
    PayloadAsJson = "update-user-request.json",
    ExpectedResult = "updated-user-response.json",
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
};

var updatedUser = await HttpClientAssertExtensions.AssertPutAsync(context);
```

---

### PATCH

```csharp
// Non-generic
public static Task AssertPatchAsync(HttpAssertContext context)

// Generic
public static Task<TResult> AssertPatchAsync<TResult>(HttpAssertContext<TResult> context)
```

#### Beispiel:
```csharp
var context = new HttpAssertContext<User>
{
    Client = _httpClient,
    Url = "/api/users/123",
    PayloadAsJson = "patch-user-request.json",
    ExpectedResult = "patched-user-response.json",
    WriteResponse = true
};

var patchedUser = await HttpClientAssertExtensions.AssertPatchAsync(context);
```

---

### DELETE

```csharp
// Non-generic
public static Task AssertDeleteAsync(HttpAssertContext context)

// Generic  
public static Task<TResult> AssertDeleteAsync<TResult>(HttpAssertContext<TResult> context)
```

#### Beispiel:
```csharp
var context = new HttpAssertContext
{
    Client = _httpClient,
    Url = "/api/users/123",
    Parameters = new[] { ("{userId}", 123) }
};

await HttpClientAssertExtensions.AssertDeleteAsync(context);
```

---

## HttpAssertContext Properties

```csharp
public sealed record HttpAssertContext<TResult>
{
    // ⭐ Required
    public required HttpClient Client { get; init; }
    public required string Url { get; init; }

    // Optional
    public string? PayloadAsJson { get; init; }
    public string? ExpectedResult { get; init; }
    
    public Func<TResult, TResult>? FilterFunc { get; init; }
    public Func<ImmutableList<Difference>, IEnumerable<Difference>>? DifferenceFunc { get; init; }
    
    public (string Key, object? Value)[] Parameters { get; init; } = [];
    public Assembly? CallingAssembly { get; init; }
    
    public bool WriteResponse { get; init; }
    public bool IsSuccessStatusCode { get; init; } = true;
    
    public string? CallerFilePath { get; init; }
    public string? PayloadParameterName { get; init; }
    public string? ExpectedResultParameterName { get; init; }
}
```

---

## Real-World Beispiele

### Beispiel 1: Simple GET Request
```csharp
[TestMethod]
public async Task GetUser_ReturnsCorrectUser()
{
    var context = new HttpAssertContext<User>
    {
        Client = _httpClient,
        Url = "/api/users/123",
        ExpectedResult = "expected-user.json"
    };

    var user = await HttpClientAssertExtensions.AssertGetAsync(context);
    
    Assert.IsNotNull(user);
}
```

---

### Beispiel 2: POST with Filtering
```csharp
[TestMethod]
public async Task CreateUser_ReturnsCreatedUser()
{
    var context = new HttpAssertContext<User>
    {
        Client = _httpClient,
        Url = "/api/users",
        PayloadAsJson = "create-user-request.json",
        ExpectedResult = "created-user-response.json",
        FilterFunc = user => user with 
        { 
            Id = default,           // Ignore generated ID
            CreatedAt = default,    // Ignore timestamp
            UpdatedAt = default     // Ignore timestamp
        },
        WriteResponse = true        // Update snapshot if needed
    };

    var createdUser = await HttpClientAssertExtensions.AssertPostAsync(context);
}
```

---

### Beispiel 3: GET with Parameters
```csharp
[TestMethod]
public async Task GetUserById_WithDynamicId()
{
    var userId = 456;
    
    var context = new HttpAssertContext<User>
    {
        Client = _httpClient,
        Url = "/api/users/{userId}",
        ExpectedResult = "user-template.json",
        Parameters = new[]
        {
            ("{userId}", userId),
            ("{timestamp}", DateTime.UtcNow.ToString("O"))
        },
        FilterFunc = user => user with { UpdatedAt = default }
    };

    var user = await HttpClientAssertExtensions.AssertGetAsync(context);
}
```

---

### Beispiel 4: Context Builder Pattern
```csharp
public static class UserContexts
{
    public static HttpAssertContext<User> ForGet(string url, string expectedFile)
    {
        return new HttpAssertContext<User>
        {
            Client = TestFixture.HttpClient,
            Url = url,
            ExpectedResult = expectedFile,
            FilterFunc = user => user with 
            { 
                Id = default,
                CreatedAt = default,
                UpdatedAt = default 
            },
            DifferenceFunc = diffs => diffs.Where(d => 
                !d.MemberPath.Contains("_metadata")
            )
        };
    }

    public static HttpAssertContext<User> ForPost(string url, string payload, string expected)
    {
        return new HttpAssertContext<User>
        {
            Client = TestFixture.HttpClient,
            Url = url,
            PayloadAsJson = payload,
            ExpectedResult = expected,
            FilterFunc = user => user with { Id = default, CreatedAt = default },
            WriteResponse = false
        };
    }
}

// Usage:
[TestMethod]
public async Task GetUser_UsesContextBuilder()
{
    var user = await HttpClientAssertExtensions.AssertGetAsync(
        UserContexts.ForGet("/api/users/123", "expected-user.json")
    );
}
```

---

### Beispiel 5: Complex Scenario mit Fehlerhandling
```csharp
[TestMethod]
public async Task GetNonExistentUser_Returns404()
{
    var context = new HttpAssertContext<ProblemDetails>
    {
        Client = _httpClient,
        Url = "/api/users/999999",
        ExpectedResult = "not-found-error.json",
        IsSuccessStatusCode = false,  // Erwarten einen Fehler
        FilterFunc = pd => pd with { TraceId = default }
    };

    var error = await HttpClientAssertExtensions.AssertGetAsync(context);
    
    Assert.AreEqual(404, error.Status);
}
```

---

### Beispiel 6: Batch-Tests mit Context-Collection
```csharp
[TestMethod]
public async Task TestMultipleEndpoints()
{
    var contexts = new[]
    {
        new HttpAssertContext<User>
        {
            Client = _httpClient,
            Url = "/api/users/1",
            ExpectedResult = "user-1.json"
        },
        new HttpAssertContext<User>
        {
            Client = _httpClient,
            Url = "/api/users/2",
            ExpectedResult = "user-2.json"
        },
        new HttpAssertContext<User>
        {
            Client = _httpClient,
            Url = "/api/users/3",
            ExpectedResult = "user-3.json"
        }
    };

    foreach (var context in contexts)
    {
        var user = await HttpClientAssertExtensions.AssertGetAsync(context);
        Assert.IsNotNull(user);
    }
}
```

---

## Migration Guide

### Von Level 1 → Level 3

**Vorher (Level 1):**
```csharp
var user = await client.AssertGetAsync<User>(
    "/api/users/123",
    "expected-user.json",
    new[] { ("{userId}", 123) },
    Assembly.GetCallingAssembly(),
    writeResponse: true,
    expectedResultParameterName: "expected-user.json",
    callerFilePath: "UserTests.cs"
);
```

**Nachher (Level 3):**
```csharp
var user = await HttpClientAssertExtensions.AssertGetAsync(
    new HttpAssertContext<User>
    {
        Client = client,
        Url = "/api/users/123",
        ExpectedResult = "expected-user.json",
        Parameters = new[] { ("{userId}", 123) },
        WriteResponse = true
    }
);
```

---

### Von Level 2 → Level 3

**Vorher (Level 2):**
```csharp
var context = new HttpAssertContext<User>
{
    Parameters = new[] { ("{userId}", 123) },
    WriteResponse = true
};

var user = await client.AssertGetAsync<User>(
    "/api/users/123",
    "expected-user.json",
    context
);
```

**Nachher (Level 3):**
```csharp
var user = await HttpClientAssertExtensions.AssertGetAsync(
    new HttpAssertContext<User>
    {
        Client = client,                      // ← Im Context!
        Url = "/api/users/123",               // ← Im Context!
        ExpectedResult = "expected-user.json", // ← Im Context!
        Parameters = new[] { ("{userId}", 123) },
        WriteResponse = true
    }
);
```

---

## Vorteile der Complete Context API

### ✅ Klarheit
Alles an einem Ort - keine separaten Parameter mehr

### ✅ Wiederverwendbarkeit
Context-Objekte können gespeichert, geteilt und wiederverwendet werden

### ✅ Context Builders
Ermöglicht das Builder-Pattern für häufige Szenarien

### ✅ Testbarkeit
Context-Objekte sind einfach zu mocken und zu testen

### ✅ IntelliSense
IDE hilft beim Erstellen des Context mit Auto-Complete

### ✅ Self-Documenting
Property-Namen machen klar, was jeder Parameter bedeutet

### ✅ Fluent Extensions (Zukünftig)
```csharp
// Mögliche zukünftige Fluent API:
var user = await HttpClientAssertExtensions
    .AssertGetAsync(new HttpAssertContext<User>()
        .WithClient(_httpClient)
        .WithUrl("/api/users/123")
        .WithExpected("expected.json")
        .IgnoringProperty(u => u.Id)
        .WithWriteResponse());
```

---

## Vergleichstabelle

| Feature | Level 1 | Level 2 | Level 3 ⭐ |
|---------|---------|---------|-----------|
| Parameter Count | 7+ | 3 | 1 |
| Readability | 😱 Poor | 😊 Good | 🎉 Excellent |
| Reusability | ❌ Hard | ✅ Good | ✅ Excellent |
| Self-documenting | ❌ No | ✅ Yes | ✅ Yes |
| IntelliSense | ⚠️ Limited | ✅ Good | ✅ Excellent |
| Context Builders | ❌ No | ⚠️ Limited | ✅ Yes |
| Backward Compatible | ✅ Original | ✅ Yes | ✅ Yes |

---

## Build Status

✅ **Build succeeded**
- 0 Warnings
- 0 Errors
- All HTTP methods support Level 3 API

---

## Zusammenfassung

### Was wurde hinzugefügt:

1. **HttpAssertContext erweitert**:
   - `required HttpClient Client`
   - `required string Url`
   - `string? PayloadAsJson`
   - `string? ExpectedResult`

2. **Neue Complete Context API für alle HTTP-Methoden**:
   - `AssertGetAsync(HttpAssertContext<T> context)`
   - `AssertPostAsync(HttpAssertContext<T> context)`
   - `AssertPutAsync(HttpAssertContext<T> context)`
   - `AssertPatchAsync(HttpAssertContext<T> context)`
   - `AssertDeleteAsync(HttpAssertContext<T> context)`

3. **Factory-Methode**:
   - `HttpAssertContextInternalFactory.FromContext()` - Keine Null-Checks nötig dank `required`

### Vorteile:

- ✅ **Maximale Klarheit** - Alles im Context
- ✅ **Konsistent** - Gleiches Pattern wie ObjectAssertContext
- ✅ **Wiederverwendbar** - Context Builder Pattern möglich
- ✅ **Backward Compatible** - Alle alten APIs funktionieren
- ✅ **Type-Safe** - `required` verhindert vergessene Properties
- ✅ **Clean Code** - Von 7+ Parametern zu 1 Context

Die HTTP-Assert-API ist jetzt auf dem gleichen Level wie ObjectsAreEqual - sauber, konsistent und benutzerfreundlich! 🎉
