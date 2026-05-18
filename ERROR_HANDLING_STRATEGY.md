# Error Handling Strategy - Strategy Pattern Implementation

## 🎯 Überblick

Wir haben das **Strategy Pattern** aus eurem Web API SDK (`Siemens.AspNet.ErrorHandling`) ins Test SDK übernommen - jetzt super flexibel und erweiterbar! 🚀

## 🏗️ Architektur

### Strategy Pattern Aufbau

```
Exception tritt auf
    ↓
AssertableHttpClient.AssertAsync (Global Catch)
    ↓
ITestErrorHandlingStrategy.HandleAsync()
    ↓
Findet passenden Handler:
    ├─ ProblemDetailsErrorHandler (für ProblemDetailsException)
    ├─ JsonSerializationErrorHandler (könnte man noch bauen)
    ├─ NetworkErrorHandler (könnte man noch bauen)
    └─ DefaultErrorHandler (Catch-All für alles andere)
    ↓
Handler formatiert Fehler
    ↓
Assert.Fail() mit formatierter Message
```

## 📁 Dateistruktur

```
ErrorHandling/
├── ITestErrorHandler.cs              # Interface + Base Class
├── TestErrorHandlingStrategy.cs      # Strategy Implementation
└── Handlers/
    ├── ProblemDetailsErrorHandler.cs # Für ProblemDetailsException
    └── DefaultErrorHandler.cs        # Catch-All Handler
```

## 🔧 Komponenten

### 1. ITestErrorHandler (Interface)

```csharp
public interface ITestErrorHandler
{
    Type GetExceptionType();
    bool CanHandle(Exception exception);
    Task<string> HandleAsync(IHttpAssertContext context, Exception exception);
}
```

### 2. TestErrorHandler<TException> (Base Class)

```csharp
public abstract class TestErrorHandler<TException> : ITestErrorHandler 
    where TException : Exception
{
    // Automatisches Type Checking
    public bool CanHandle(Exception exception) { ... }
    
    // Überschreibbar für custom Checks
    protected virtual bool CanHandle(TException exception) { return true; }
    
    // Muss implementiert werden
    protected abstract Task<string> HandleExceptionAsync(
        IHttpAssertContext context, 
        TException exception);
}
```

### 3. ITestErrorHandlingStrategy (Strategy Interface)

```csharp
public interface ITestErrorHandlingStrategy
{
    Task<string> HandleAsync(IHttpAssertContext context, Exception exception);
}
```

### 4. TestErrorHandlingStrategy (Strategy Implementation)

```csharp
internal sealed class TestErrorHandlingStrategy(
    IEnumerable<ITestErrorHandler> testErrorHandlers) 
    : ITestErrorHandlingStrategy
{
    public async Task<string> HandleAsync(IHttpAssertContext context, 
                                          Exception exception)
    {
        // 1. Finde alle Handler, die die Exception handlen können
        var compatibleHandlers = testErrorHandlers
            .Where(handler => handler.CanHandle(exception))
            .ToList();

        // 2. Nimm den ersten (order matters!)
        var selectedHandler = compatibleHandlers.FirstOrDefault();

        // 3. Handle die Exception
        return await selectedHandler.HandleAsync(context, exception);
    }
}
```

## 🎨 Spezifische Handler

### ProblemDetailsErrorHandler

**Handled:** `ProblemDetailsException`

**Output:**
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED API ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
🌍 HTTP
⚠️ Problem Details
📋 Extension Data
💡 What This Means
📝 Assert Call
🔁 Reproduce Locally
══════════════════════════════════════════════════════════════
```

**Implementation:**
```csharp
internal sealed class ProblemDetailsErrorHandler(
    IProblemDetailsOutputBuilder problemDetailsOutputBuilder)
    : TestErrorHandler<ProblemDetailsException>
{
    protected override Task<string> HandleExceptionAsync(
        IHttpAssertContext context,
        ProblemDetailsException exception)
    {
        var errorOutput = problemDetailsOutputBuilder
            .BuildUnexpectedError(context, exception);
        return Task.FromResult(errorOutput);
    }
}
```

### DefaultErrorHandler

**Handled:** `Exception` (Catch-All!)

**Output:**
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED TEST SDK ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
🌍 HTTP Request
⚠️ Exception Details (Type, Message, Stack Trace)
💡 What This Means
══════════════════════════════════════════════════════════════
```

**Handled Scenarios:**
- SDK Bugs
- Network Errors (Timeout, Connection Refused, etc.)
- Serialization Errors (JsonException, etc.)
- Alle anderen unerwarteten Exceptions

## 🚀 Verwendung

### Automatisch aktiv für ALLE HTTP Asserts

```csharp
await Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(
    "api/endpoint",
    "request.json", 
    "expected.json");
```

### Handler-Registrierung (DI)

```csharp
services.AddTestErrorHandlingStrategy();  // In AssertableHttpClient

// Expandiert zu:
services.AddProblemDetailsErrorHandler();  // Specific Handler
services.AddDefaultErrorHandler();         // Fallback (MUST BE LAST!)
services.AddSingletonIfNotExists<ITestErrorHandlingStrategy, 
                                  TestErrorHandlingStrategy>();
```

### Neue Handler hinzufügen

**Schritt 1:** Handler erstellen

```csharp
internal sealed class JsonSerializationErrorHandler 
    : TestErrorHandler<JsonException>
{
    protected override Task<string> HandleExceptionAsync(
        IHttpAssertContext context,
        JsonException exception)
    {
        // Custom formatting für JsonException
        var errorOutput = BuildJsonError(context, exception);
        return Task.FromResult(errorOutput);
    }

    // Optional: Extra Checks
    protected override bool CanHandle(JsonException exception)
    {
        // Nur bei bestimmten JSON Fehlern
        return exception.Message.Contains("deserialization");
    }
}
```

**Schritt 2:** Extension Method

```csharp
internal static class AddJsonSerializationErrorHandlerExtension
{
    public static void AddJsonSerializationErrorHandler(
        this IServiceCollection services)
    {
        services.AddSingletonIfNotExists<ITestErrorHandler, 
                                          JsonSerializationErrorHandler>();
    }
}
```

**Schritt 3:** Registrieren (vor DefaultErrorHandler!)

```csharp
public static void AddTestErrorHandlingStrategy(this IServiceCollection services)
{
    services.AddProblemDetailsErrorHandler();
    services.AddJsonSerializationErrorHandler();  // NEU!
    
    // Fallback - MUST BE LAST!
    services.AddDefaultErrorHandler();

    services.AddSingletonIfNotExists<ITestErrorHandlingStrategy, 
                                      TestErrorHandlingStrategy>();
}
```

## ✨ Vorteile des Strategy Pattern

### 1. Flexibilität
- ✅ Neue Handler einfach hinzufügen ohne bestehenden Code zu ändern
- ✅ Order matters - First match wins
- ✅ Fallback Handler garantiert, dass immer was zurückkommt

### 2. Erweiterbarkeit
Neue Handler für spezifische Szenarien:
```
- ProblemDetailsErrorHandler      ✅ Implemented
- DefaultErrorHandler              ✅ Implemented
- JsonSerializationErrorHandler    🎯 Future
- NetworkTimeoutErrorHandler       🎯 Future
- HttpClientErrorHandler           🎯 Future
- ValidationErrorHandler           🎯 Future
- AuthenticationErrorHandler       🎯 Future
```

### 3. Separation of Concerns
- ✅ Jeder Handler kümmert sich nur um EINE Exception-Art
- ✅ `TestErrorHandlingStrategy` kümmert sich nur um die Auswahl
- ✅ `AssertableHttpClient` kümmert sich nur um den Catch

### 4. Testbarkeit
```csharp
// Mock einen Handler
var mockHandler = new Mock<ITestErrorHandler>();
mockHandler.Setup(h => h.CanHandle(It.IsAny<Exception>()))
           .Returns(true);
mockHandler.Setup(h => h.HandleAsync(It.IsAny<IHttpAssertContext>(), 
                                     It.IsAny<Exception>()))
           .ReturnsAsync("Test Error");

// Test Strategy
var strategy = new TestErrorHandlingStrategy(new[] { mockHandler.Object });
var result = await strategy.HandleAsync(context, exception);

Assert.AreEqual("Test Error", result);
```

### 5. Consistency mit Web API SDK
- ✅ Gleiches Pattern wie `Siemens.AspNet.ErrorHandling`
- ✅ Team kennt das Pattern schon
- ✅ Best Practices übernommen

## 🔍 Vergleich: Vorher vs. Nachher

### ❌ Vorher (Hardcoded Try-Catch)

```csharp
try
{
    // ... logic ...
}
catch (ProblemDetailsException e)
{
    var output = problemDetailsOutputBuilder.BuildUnexpectedError(context, e);
    Assert.Fail(output);
}
catch (Exception e)
{
    var output = BuildUnexpectedSdkError(context, e);
    Assert.Fail(output);
}
```

**Probleme:**
- ❌ Hardcoded - neue Exception-Typen = Code ändern
- ❌ Schwer zu erweitern
- ❌ Duplizierter Code
- ❌ Keine Flexibilität

### ✅ Nachher (Strategy Pattern)

```csharp
try
{
    // ... logic ...
}
catch (Exception exception)
{
    var errorOutput = await testErrorHandlingStrategy
        .HandleAsync(context, exception);
    Assert.Fail(errorOutput);
}
```

**Vorteile:**
- ✅ Clean - EIN Catch-Block
- ✅ Flexibel - Handler via DI registrieren
- ✅ Erweiterbar - neue Handler ohne Code-Änderung
- ✅ Testbar - Handler mocken
- ✅ Maintainable - klare Struktur

## 🎓 Flow-Beispiel

### Beispiel 1: ProblemDetailsException

```
1. API gibt ProblemDetails zurück
   ↓
2. JsonSerializer wirft ProblemDetailsException
   ↓
3. AssertableHttpClient fängt Exception ab
   ↓
4. testErrorHandlingStrategy.HandleAsync(context, exception)
   ↓
5. Strategy sucht kompatible Handler:
   - ProblemDetailsErrorHandler.CanHandle(exception) ✅ TRUE
   - DefaultErrorHandler.CanHandle(exception) ✅ TRUE (aber zu spät)
   ↓
6. Erster Match: ProblemDetailsErrorHandler
   ↓
7. ProblemDetailsErrorHandler.HandleAsync(context, exception)
   ↓
8. problemDetailsOutputBuilder.BuildUnexpectedError(...)
   ↓
9. Formatierte Error Message zurück an Strategy
   ↓
10. Assert.Fail(errorMessage)
```

### Beispiel 2: Network Timeout

```
1. Network Timeout
   ↓
2. HttpClient wirft HttpRequestException
   ↓
3. AssertableHttpClient fängt Exception ab
   ↓
4. testErrorHandlingStrategy.HandleAsync(context, exception)
   ↓
5. Strategy sucht kompatible Handler:
   - ProblemDetailsErrorHandler.CanHandle(exception) ❌ FALSE
   - DefaultErrorHandler.CanHandle(exception) ✅ TRUE
   ↓
6. Match: DefaultErrorHandler (Catch-All)
   ↓
7. DefaultErrorHandler.HandleAsync(context, exception)
   ↓
8. BuildUnexpectedSdkError(...)
   ↓
9. Formatierte Error Message zurück an Strategy
   ↓
10. Assert.Fail(errorMessage)
```

## 📋 Handler Registrierungs-Reihenfolge

**WICHTIG:** Order matters! First match wins!

```csharp
// 1. Most Specific First
services.AddProblemDetailsErrorHandler();

// 2. More Specific Handlers
services.AddJsonSerializationErrorHandler();
services.AddNetworkTimeoutErrorHandler();
services.AddValidationErrorHandler();

// 3. Catch-All MUST BE LAST!
services.AddDefaultErrorHandler();
```

**Warum?**
- `DefaultErrorHandler` matched `Exception` (alles!)
- Wenn zuerst registriert → matched immer → spezifische Handler werden nie erreicht

## 🎯 Best Practices

### 1. Handler schreiben
```csharp
✅ DO: Specific Exception Type
internal sealed class MyHandler : TestErrorHandler<MySpecificException>

❌ DON'T: Generic Exception Type (außer Default Handler)
internal sealed class MyHandler : TestErrorHandler<Exception>
```

### 2. CanHandle Override
```csharp
✅ DO: Zusätzliche Checks wenn nötig
protected override bool CanHandle(MyException exception)
{
    return exception.Message.Contains("specific-case");
}

❌ DON'T: Komplexe Logik - keep it simple
protected override bool CanHandle(MyException exception)
{
    // 100 Zeilen Logic...
}
```

### 3. Error Message Building
```csharp
✅ DO: Hilfreich und strukturiert
- Test Info (Project, Class, Method, Line)
- HTTP Info (Method, URL)
- Exception Info (Type, Message, Stack Trace)
- Erklärung (Was bedeutet das?)
- Reproduktion (Curl command wenn möglich)

❌ DON'T: Nur exception.ToString()
return exception.ToString();  // Nicht hilfreich!
```

### 4. Dependencies
```csharp
✅ DO: Inject was du brauchst
internal sealed class MyHandler(
    ISomeService service,
    ISomeOtherService otherService) 
    : TestErrorHandler<MyException>

✅ DO: Register Dependencies im Extension
public static void AddMyHandler(this IServiceCollection services)
{
    services.AddSomeService();
    services.AddSomeOtherService();
    services.AddSingletonIfNotExists<ITestErrorHandler, MyHandler>();
}
```

## 🚀 Future Enhancements

### Mögliche neue Handler

1. **JsonSerializationErrorHandler**
   - `JsonException`, `JsonReaderException`
   - Zeigt JSON Path wo der Fehler ist
   - Zeigt expected vs. actual Type

2. **NetworkTimeoutErrorHandler**
   - `HttpRequestException` mit Timeout
   - Zeigt Timeout-Wert
   - Gibt Tipps für Configuration

3. **AuthenticationErrorHandler**
   - `UnauthorizedAccessException`, 401/403
   - Zeigt Auth-Header Info
   - Gibt Tipps für Token-Probleme

4. **ValidationErrorHandler**
   - Custom Validation Exceptions
   - Zeigt Validation Errors in strukturierter Form

## ✅ Fazit

### Das Strategy Pattern gibt uns:

1. ✅ **Flexibilität** - Neue Handler einfach hinzufügen
2. ✅ **Erweiterbarkeit** - Open/Closed Principle
3. ✅ **Wartbarkeit** - Klare Struktur, jeder Handler eine Datei
4. ✅ **Testbarkeit** - Handler einzeln testbar
5. ✅ **Consistency** - Gleich wie Web API SDK
6. ✅ **Professional** - Enterprise-Level Error Handling

**Von "aaaa" zu Production-Ready Error Handling!** 🎉
