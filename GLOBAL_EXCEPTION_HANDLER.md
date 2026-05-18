# Global Exception Handler - Implementation Summary

## 🎯 Ziel
Ein zentraler Exception Handler im Test SDK, der ALLE Fehler abfängt und schön formatiert - egal ob es ProblemDetails vom API, Bugs im SDK selbst, Network-Fehler oder andere unerwartete Exceptions sind.

## 📍 Implementierungsort

**`AssertableHttpClient.AssertAsync`** - Der perfekte Ort! 🎯

### Warum hier?
1. ✅ **Zentraler Entry Point** - Jeder HTTP Assert durchläuft diese Methode
2. ✅ **Voller Context** - Zugriff auf `HttpAssertContext`, `HttpResponseMessage`, etc.
3. ✅ **Fängt ALLES** - Endpoint validation, HTTP call, deserialization, pipeline steps
4. ✅ **Saubere Architektur** - Kein try-catch in einzelnen Steps mehr nötig

### Flow
```
AssertHttpCallAsync (God-Method)
    ↓
AssertableHttpClient.AssertAsync  ← GLOBAL EXCEPTION HANDLER HIER! 🛡️
    ├─ endpointValidator.Validate()
    ├─ httpCallHandler.CallAsync()
    ├─ Deserialization
    └─ httpAssertionPipeline.Execute()
        └─ JsonComparisonStep
            └─ AssertService
                └─ ComparisonStrategy
```

## 🔧 Implementierung

### 1. Exception Typen

#### ProblemDetailsException
```csharp
catch (ProblemDetailsException problemDetailsException)
{
    // API hat ProblemDetails zurückgegeben
    var errorOutput = problemDetailsOutputBuilder.BuildUnexpectedError(context, problemDetailsException);
    Assert.That.Fail(errorOutput);
}
```

**Output:**
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED API ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
🌍 HTTP
⚠️ Problem Details (Status, Title, Detail, Type, Instance)
📋 Extension Data (falls vorhanden)
💡 What This Means
📝 Assert Call
🔁 Reproduce Locally
```

#### Alle anderen Exceptions (Catch-All)
```csharp
catch (Exception exception)
{
    // SDK Bugs, Network-Fehler, Serialization-Probleme, etc.
    var errorOutput = BuildUnexpectedSdkError(context, exception);
    Assert.That.Fail(errorOutput);
}
```

**Output:**
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED TEST SDK ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
🌍 HTTP Request
⚠️ Exception Details (Type, Message, StackTrace)
💡 What This Means
```

## 📝 Geänderte Dateien

### 1. `AssertableHttpClient/AssertableHttpClient.cs`
**Änderungen:**
- ✅ Import: `using AspNetCore.Simple.MsTest.Sdk.ProblemDetails;`
- ✅ DI Registration: `services.AddProblemDetailsOutputBuilder();`
- ✅ Constructor: `IProblemDetailsOutputBuilder problemDetailsOutputBuilder` Parameter hinzugefügt
- ✅ `AssertAsync()`: Kompletter try-catch-catch Block implementiert
- ✅ Neue Methode: `BuildUnexpectedSdkError()` für den Catch-All Handler

### 2. `AssertableHttpClient/Pipelines/Steps/JsonComparisonStep.cs`
**Änderungen:**
- ✅ try-catch Block **ENTFERNT** (nicht mehr nötig!)
- ✅ `problemDetailsOutputBuilder` Parameter **ENTFERNT** aus Constructor
- ✅ Kommentar hinzugefügt: "Exception handling is now done globally"

### 3. `Assert/AssertService.cs`
**Änderungen:**
- ✅ Bleibt clean ohne try-catch
- ✅ Kommentar hinzugefügt: "Exception handling is done globally in AssertableHttpClient.AssertAsync"

### 4. `Comparison/JsonComparisonStrategy.cs`
**Änderungen:**
- ✅ `Assert.Fail("aaaa")` ersetzt durch `throw;`
- ✅ Exception propagiert korrekt zum globalen Handler

### 5. `ProblemDetails/ProblemDetailsOutputBuilder.cs`
**NEU erstellt:**
- ✅ Isolierter Output Builder für ProblemDetails
- ✅ Interface: `IProblemDetailsOutputBuilder`
- ✅ Methode: `BuildUnexpectedError()`
- ✅ Formatierung aller ProblemDetails-Felder + Extensions

## ✨ Vorteile

### 1. Saubere Architektur
- ❌ **Vorher**: try-catch in `JsonComparisonStep`, `AssertService`, etc.
- ✅ **Jetzt**: EIN globaler Handler an einem zentralen Punkt

### 2. Vollständige Abdeckung
- ✅ Endpoint validation errors
- ✅ HTTP call errors (Network, Timeout, etc.)
- ✅ Deserialization errors
- ✅ ProblemDetails vom API
- ✅ Pipeline step errors
- ✅ SDK bugs
- ✅ Alle anderen unerwarteten Exceptions

### 3. Konsistente Fehlerausgabe
- ✅ Strukturiert und lesbar
- ✅ Vollständiger Context (Test Info, HTTP Details)
- ✅ Hilfreich (Was bedeutet der Fehler?)
- ✅ Actionable (Curl command zum Reproduzieren)

### 4. Einfache Wartung
- ✅ Nur eine Stelle für Exception Handling
- ✅ Neue Exception-Typen können einfach hinzugefügt werden
- ✅ Kein duplizierter Code mehr

## 🚀 Verwendung

Der Handler ist automatisch aktiv für **ALLE** HTTP Asserts:
```csharp
await Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(
    "api/core/v1/admin/health-metrics/collect",
    "request.json", 
    "expected.json");
```

Wenn ein Fehler auftritt, wird automatisch einer der beiden Handler aktiviert:
- **ProblemDetailsException** → Schöne ProblemDetails-Formatierung
- **Andere Exception** → SDK Error mit vollständigem Context

## 🎓 Design Entscheidungen

### Warum in AssertableHttpClient?
**Alternative 1**: In `AssertHttpCallAsync` (God-Method)
- ❌ Noch kein `HttpResponseMessage` verfügbar
- ❌ Weniger Context für Error Formatting

**Alternative 2**: In einzelnen Steps (z.B. `JsonComparisonStep`)
- ❌ Nicht alle Fehler werden abgefangen
- ❌ Duplizierter Code in mehreren Steps
- ❌ Fehler in anderen Steps werden nicht erwischt

**✅ Gewählt**: `AssertableHttpClient.AssertAsync`
- ✅ Perfekter Entry Point mit vollem Context
- ✅ Fängt wirklich ALLE Fehler
- ✅ Ein globaler Handler für alles

### Catch-All Handler
Der `catch (Exception exception)` Block ist bewusst implementiert:
- ✅ Fängt SDK-Bugs ab, bevor sie den Test crashen
- ✅ Gibt dem Developer hilfreiche Informationen
- ✅ Macht Debugging einfacher
- ✅ Professional - kein ungecatchtes Exception mehr

## 🎨 Output-Beispiele

### ProblemDetailsException
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED API ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
Project    : Sdc.Core.Test
Class      : Collect_Health_Metrics_Status_400_BadRequest_Test
Method     : Should_Not_Be_Able_Collect_Health_Metrics_Request_Is_Invalid
Line       : 28

🌍 HTTP
──────────────────────────────────────────────────────────────
Method     : POST
Url        : http://localhost/api/core/v1/admin/health-metrics/collect
Status     : 500 Internal Server Error

⚠️ Problem Details
──────────────────────────────────────────────────────────────
Status     : 500
Title      : Internal Server Error
Detail     : An error occurred while processing your request.

📋 Extension Data
──────────────────────────────────────────────────────────────
┌──────────┬────────────────────────────────────────┐
│ Key      │ Value                                  │
├──────────┼────────────────────────────────────────┤
│ traceId  │ 00-123abc456def789-0af7651916cd43dd-00│
└──────────┴────────────────────────────────────────┘

💡 What This Means
──────────────────────────────────────────────────────────────
The API endpoint returned an error response (ProblemDetails), but the test
expected a different response type. This typically indicates:

  • The endpoint encountered an unexpected error
  • The test's expected response type doesn't match what the API returned
  • There may be a validation or server-side processing issue

📝 Assert Call
──────────────────────────────────────────────────────────────
await Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(...)

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────
curl --request POST 'http://localhost/api/...' --header '...'
══════════════════════════════════════════════════════════════
```

### SDK Error (Catch-All)
```
══════════════════════════════════════════════════════════════
❌ UNEXPECTED TEST SDK ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
Project    : Sdc.Core.Test
Class      : Some_Test_Class
Method     : Some_Test_Method
Line       : 42

🌍 HTTP Request
──────────────────────────────────────────────────────────────
Method     : POST
Url        : http://localhost/api/some/endpoint

⚠️ Exception Details
──────────────────────────────────────────────────────────────
Type       : JsonException
Message    : The JSON value could not be converted to System.String

Stack Trace:
   at System.Text.Json.ThrowHelper.ThrowJsonException()
   at AspNetCore.Simple.MsTest.Sdk...

💡 What This Means
──────────────────────────────────────────────────────────────
An unexpected error occurred in the Test SDK. This could indicate:

  • A bug in the Test SDK itself
  • Network connectivity issues
  • Invalid test configuration
  • Serialization/deserialization problems

Please report this issue if it appears to be a Test SDK bug.
══════════════════════════════════════════════════════════════
```

## ✅ Fazit

Der Global Exception Handler ist jetzt an der **perfekten Stelle** implementiert:
- ✅ Ein zentraler Handler statt viele try-catch Blöcke
- ✅ Fängt wirklich ALLE Fehler ab
- ✅ Schöne, strukturierte Error-Ausgabe
- ✅ Hilfreich für Debugging
- ✅ Professional und wartbar

**Kein "aaaa" mehr!** 🎉
