# Refactoring: Separation of Concerns - Data vs Logic

## Übersicht
Dieses Refactoring trennt sauber zwischen Datenobjekten und Logic/Factory-Methoden, gemäß dem Prinzip der "Separation of Concerns".

## Motivation

### Problem - Vermischung von Data und Logic

**Vorher:** Data-Objekte enthielten Factory-Methoden (Logic)

```csharp
internal sealed class HttpAssertContextInternal<TResult>
{
    // Data: Properties
    public required HttpClient Client { get; init; }
    public required string Url { get; init; }
    // ... weitere Properties ...

    // Logic: Factory Methods (❌ gehört nicht hierher!)
    public static HttpAssertContextInternal<TResult> FromPublicContext(...) { }
    public static HttpAssertContextInternal<TResult> FromParameters(...) { }
}
```

**Probleme:**
- ❌ Vermischt Datenhaltung mit Objekterzeugung
- ❌ Schwieriger zu testen
- ❌ Verletzt Single Responsibility Principle
- ❌ Unklare Verantwortlichkeiten

---

## Lösung: Separation of Concerns

### 1. Data-Objekte (reine Datenhaltung)

**Datei:** `HttpAssertContextInternal.cs`

```csharp
/// <summary>
/// Internal context object used by the master AssertHttpCallAsync method.
/// Contains all parameters needed for HTTP assertion logic.
/// This is a pure data object - no logic or factory methods.
/// </summary>
internal sealed class HttpAssertContextInternal<TResult>
{
    public required HttpClient Client { get; init; }
    public required string Url { get; init; }
    public required string PayloadAsJson { get; init; }
    public required string ExpectedResult { get; init; }
    public required HttpMethod HttpMethod { get; init; }
    
    public Func<TResult, TResult> FilterFunc { get; init; } = item => item;
    public Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; } = difference => difference;
    
    public (string Key, object? Value)[] Parameters { get; init; } = [];
    public required Assembly CallingAssembly { get; init; }
    
    public bool WriteResponse { get; init; }
    public bool IsSuccessStatusCode { get; init; } = true;
    
    public string CallerFilePath { get; init; } = string.Empty;
    public string PayloadParameterName { get; init; } = string.Empty;
    public string ExpectedResultParameterName { get; init; } = string.Empty;
}

/// <summary>
/// Internal context object for non-generic HTTP assertions.
/// This is a pure data object - no logic or factory methods.
/// </summary>
internal sealed class HttpAssertContextInternal
{
    public required HttpClient Client { get; init; }
    public required string Url { get; init; }
    public required string PayloadAsJson { get; init; }
    public required HttpMethod HttpMethod { get; init; }
    
    public (string Key, object? Value)[] Parameters { get; init; } = [];
    public required Assembly CallingAssembly { get; init; }
    
    public bool WriteResponse { get; init; }
    public bool IsSuccessStatusCode { get; init; } = true;
    
    public required string CallerFilePath { get; init; } = string.Empty;
    public required string PayloadParameterName { get; init; } = string.Empty;
}
```

**Eigenschaften:**
- ✅ Nur Properties (Data)
- ✅ Keine Methoden (Logic)
- ✅ Klare Verantwortlichkeit: Datenhaltung
- ✅ Leicht zu verstehen

---

### 2. Factory-Klasse (reine Logic)

**Datei:** `HttpAssertContextInternalFactory.cs`

```csharp
/// <summary>
/// Factory methods for creating HttpAssertContextInternal instances.
/// Separates object creation logic from data objects.
/// </summary>
internal static class HttpAssertContextInternalFactory
{
    // ============================================================
    // Generic Context Factory Methods
    // ============================================================

    /// <summary>
    /// Creates an internal context from the public context.
    /// All required values should be in the context (Clean API - Level 3).
    /// </summary>
    public static HttpAssertContextInternal<TResult> FromContext<TResult>(
        HttpAssertContext<TResult> publicContext,
        HttpMethod httpMethod)
    {
        if (publicContext.Client == null)
            throw new ArgumentNullException(nameof(publicContext), "Client must be set in the context");
        if (string.IsNullOrWhiteSpace(publicContext.Url))
            throw new ArgumentNullException(nameof(publicContext), "Url must be set in the context");

        return new HttpAssertContextInternal<TResult>
               {
                   Client = publicContext.Client,
                   Url = publicContext.Url,
                   PayloadAsJson = publicContext.PayloadAsJson ?? string.Empty,
                   ExpectedResult = publicContext.ExpectedResult ?? string.Empty,
                   HttpMethod = httpMethod,
                   FilterFunc = publicContext.FilterFunc ?? (item => item),
                   DifferenceFunc = publicContext.DifferenceFunc ?? (difference => difference),
                   Parameters = publicContext.Parameters,
                   CallingAssembly = publicContext.CallingAssembly ?? Assembly.GetCallingAssembly(),
                   WriteResponse = publicContext.WriteResponse,
                   IsSuccessStatusCode = publicContext.IsSuccessStatusCode,
                   CallerFilePath = publicContext.CallerFilePath ?? string.Empty,
                   PayloadParameterName = publicContext.PayloadParameterName ?? string.Empty,
                   ExpectedResultParameterName = publicContext.ExpectedResultParameterName ?? string.Empty
               };
    }

    /// <summary>
    /// Creates an internal context from the public context and explicit parameters (Backward compatibility - Level 2).
    /// Parameters passed explicitly take precedence over context values.
    /// </summary>
    public static HttpAssertContextInternal<TResult> CreateFrom<TResult>(
        HttpClient client,
        string url,
        string payloadAsJson,
        string expectedResult,
        HttpMethod httpMethod,
        HttpAssertContext<TResult> publicContext,
        string defaultPayloadParameterName = "",
        string defaultExpectedResultParameterName = "")
    {
        return new HttpAssertContextInternal<TResult>
               {
                   Client = client,
                   Url = url,
                   PayloadAsJson = payloadAsJson,
                   ExpectedResult = expectedResult,
                   HttpMethod = httpMethod,
                   FilterFunc = publicContext.FilterFunc ?? (item => item),
                   DifferenceFunc = publicContext.DifferenceFunc ?? (difference => difference),
                   Parameters = publicContext.Parameters,
                   CallingAssembly = publicContext.CallingAssembly ?? Assembly.GetCallingAssembly(),
                   WriteResponse = publicContext.WriteResponse,
                   IsSuccessStatusCode = publicContext.IsSuccessStatusCode,
                   CallerFilePath = publicContext.CallerFilePath ?? string.Empty,
                   PayloadParameterName = publicContext.PayloadParameterName ?? defaultPayloadParameterName,
                   ExpectedResultParameterName = publicContext.ExpectedResultParameterName ?? defaultExpectedResultParameterName
               };
    }

    /// <summary>
    /// Creates an internal context from individual parameters (for backward compatibility).
    /// </summary>
    public static HttpAssertContextInternal<TResult> FromParameters<TResult>(
        HttpClient client,
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
        bool writeResponse = false)
    {
        return new HttpAssertContextInternal<TResult>
               {
                   Client = client,
                   Url = url,
                   PayloadAsJson = payloadAsJson,
                   ExpectedResult = expectedResult,
                   FilterFunc = filterFunc,
                   HttpMethod = httpMethod,
                   DifferenceFunc = differenceFunc,
                   Parameters = parameters,
                   CallingAssembly = callingAssembly,
                   PayloadParameterName = payloadAsJsonParameterName,
                   ExpectedResultParameterName = expectedResultParameterName,
                   CallerFilePath = callerFilePath,
                   IsSuccessStatusCode = isSuccessStatusCode,
                   WriteResponse = writeResponse
               };
    }

    // Non-generic versions follow the same pattern...
}
```

**Eigenschaften:**
- ✅ Nur Factory-Methoden (Logic)
- ✅ Keine Properties (Data)
- ✅ Klare Verantwortlichkeit: Objekterzeugung
- ✅ Einfach zu erweitern
- ✅ Leicht testbar

---

## Bonus: Complete Context API

Wir haben auch die öffentlichen Context-Objekte erweitert, sodass ALLE Parameter im Context sind:

### HttpAssertContext<TResult> erweitert

```csharp
public sealed record HttpAssertContext<TResult>
{
    // ⭐ NEU: HttpClient, Url, Payload, ExpectedResult jetzt im Context!
    public HttpClient? Client { get; init; }
    public string? Url { get; init; }
    public string? PayloadAsJson { get; init; }
    public string? ExpectedResult { get; init; }
    
    // Bestehende Properties
    public Func<TResult, TResult>? FilterFunc { get; init; }
    public Func<ImmutableList<Difference>, IEnumerable<Difference>>? DifferenceFunc { get; init; }
    public (string Key, object? Value)[] Parameters { get; init; } = [];
    public Assembly? CallingAssembly { get; init; }
    public bool WriteResponse { get; init; }
    public bool IsSuccessStatusCode { get; init; } = true;
    // ... weitere Properties ...
}
```

### Zukünftige Clean API (Level 3)

**Ziel:**
```csharp
// Alles im Context - nur ein Parameter! 🎉
var context = new HttpAssertContext<User>
{
    Client = _httpClient,
    Url = "/api/users/{userId}",
    PayloadAsJson = "request.json",
    ExpectedResult = "expected-user.json",
    Parameters = new[] { ("{userId}", 123) },
    FilterFunc = user => user with { Id = default },
    WriteResponse = true
};

var user = await client.AssertGetAsync(context);
```

Statt vorher:
```csharp
// Viele separate Parameter 😱
var context = new HttpAssertContext<User>
{
    FilterFunc = user => user with { Id = default },
    WriteResponse = true
};

var user = await client.AssertGetAsync(
    "/api/users/{userId}",      // ← Separat
    "request.json",              // ← Separat
    "expected-user.json",        // ← Separat
    context                      // ← Context nur für optionale Params
);
```

---

## Vorteile

### ✅ Klarheit
- **Data-Objekte** haben nur Properties
- **Factory-Klassen** haben nur Methoden
- Jede Klasse hat genau eine Verantwortlichkeit

### ✅ Wartbarkeit
- Änderungen an Factory-Logic beeinflussen Data-Objekte nicht
- Neue Factory-Methoden können hinzugefügt werden ohne Data-Objekte zu ändern
- Testbarkeit verbessert sich

### ✅ Lesbarkeit
```csharp
// Vorher (unklar)
var context = HttpAssertContextInternal<User>.FromPublicContext(...);
// Ist FromPublicContext Teil der Daten? Oder Logic?

// Nachher (klar)
var context = HttpAssertContextInternalFactory.FromContext(...);
// Factory-Klasse → offensichtlich Logic!
```

### ✅ Erweiterbarkeit
- Neue Factory-Methoden einfach hinzufügen
- Data-Objekte bleiben stabil
- Keine Breaking Changes bei Logic-Änderungen

### ✅ Testbarkeit
- Data-Objekte sind reine DTOs → keine Tests nötig
- Factory-Methoden isoliert testbar
- Mocking einfacher

---

## Dateistruktur

```
AssertExtensions/
├── HttpAssertContext.cs                    ← Öffentliche Context (Data + record)
├── HttpAssertContextInternal.cs            ← Interne Context (nur Data)
├── HttpAssertContextInternalFactory.cs     ← Factory-Methoden (nur Logic)
├── ObjectAssertContext.cs                  ← Öffentliche Context (Data + record)
└── ... weitere Files ...
```

---

## Migration der Aufrufe

### Vorher:
```csharp
var context = HttpAssertContextInternal<User>.FromParameters(
    client, url, payload, expected, filter, method, diff, params, assembly, ...);
```

### Nachher:
```csharp
var context = HttpAssertContextInternalFactory.FromParameters(
    client, url, payload, expected, filter, method, diff, params, assembly, ...);
```

**Automatisch geändert** via `sed` in allen Files:
- `HttpAssertContextInternal<T>.FromParameters` → `HttpAssertContextInternalFactory.FromParameters<T>`
- `HttpAssertContextInternal.FromParameters` → `HttpAssertContextInternalFactory.FromParameters`
- `HttpAssertContextInternal.FromPublicContext` → `HttpAssertContextInternalFactory.CreateFrom` / `FromContext`

---

## Design Principles

Dieses Refactoring folgt mehreren wichtigen Prinzipien:

### 1. Single Responsibility Principle (SRP)
- **Data-Objekte**: Verantwortlich für Datenhaltung
- **Factory-Klassen**: Verantwortlich für Objekterzeugung

### 2. Separation of Concerns (SoC)
- Data und Logic sind sauber getrennt
- Änderungen an einem Concern beeinflussen den anderen nicht

### 3. Open/Closed Principle (OCP)
- Data-Objekte sind geschlossen für Änderungen
- Factory-Klassen sind offen für Erweiterungen (neue Factory-Methoden)

### 4. Don't Repeat Yourself (DRY)
- Factory-Logic zentralisiert in einer Klasse
- Keine Duplikation von Objekterzeugung-Code

---

## Status

✅ **Abgeschlossen**
- Data-Objekte sind rein (nur Properties)
- Factory-Klasse erstellt (nur Methoden)
- Alle Referenzen aktualisiert
- Build erfolgreich (0 Warnings, 0 Errors)
- Complete Context API vorbereitet (Client, Url, Payload, ExpectedResult im Context)

---

## Nächste Schritte

1. **Complete Context API implementieren** (Level 3)
   - Neue Überladungen erstellen, die nur Context nehmen
   - Beispiel: `await client.AssertGetAsync(context)` statt `await client.AssertGetAsync(url, payload, expected, context)`

2. **Weitere Separations** (falls gewünscht)
   - `ObjectAssertContext` könnte auch eine Factory bekommen
   - Andere Helper-Klassen prüfen

3. **Tests schreiben**
   - Factory-Methoden Unit-Tests
   - Integration-Tests für Complete Context API

---

## Zusammenfassung

| Aspekt | Vorher | Nachher |
|--------|--------|---------|
| **Struktur** | Data + Logic vermischt | Data und Logic getrennt |
| **Verantwortlichkeit** | Unklar | Klar definiert |
| **Wartbarkeit** | Schwierig | Einfach |
| **Testbarkeit** | Kompliziert | Straightforward |
| **Lesbarkeit** | Verwirrend | Intuitiv |
| **SRP** | ❌ Verletzt | ✅ Eingehalten |
| **SoC** | ❌ Vermischt | ✅ Getrennt |

Die Codebasis ist jetzt sauberer, wartbarer und folgt Best Practices! 🎉
