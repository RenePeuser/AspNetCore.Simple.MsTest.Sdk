# JSON Serialization Error Handler

## 🎯 Überblick

Ein **spezifischer Error Handler** für `JsonException` - zeigt alle wichtigen Informationen wenn JSON Deserialization fehlschlägt!

## ✨ Features

### 1. Detaillierte JSON Error Information
- ✅ Error Message
- ✅ JSON Path (z.B. `$.data.items[0].name`)
- ✅ Line Number & Position
- ✅ Inner Exception Details

### 2. Vollständiger JSON Context
- ✅ **Request Payload** - Was wurde gesendet
- ✅ **Response Content** - Was kam zurück (das fehlerhafte JSON)
- ✅ **Expected JSON** - Was erwartet wurde (aus File)
- ✅ Automatisches Pretty-Printing

### 3. Intelligente Erklärungen
- ✅ Analysiert Exception Message
- ✅ Gibt spezifische Hinweise basierend auf Fehlertyp
- ✅ Konkrete Lösungsvorschläge

## 📋 Output Format

```
══════════════════════════════════════════════════════════════
❌ JSON SERIALIZATION ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
Project         : MyTest.Project
Class           : SomeApiTest
Method          : Should_Deserialize_Response
Line            : 42

🌍 HTTP Request/Response
──────────────────────────────────────────────────────────────
Method          : POST
URL             : http://localhost/api/data
Expected Type   : MyResponseDto

⚠️ JSON Error Details
──────────────────────────────────────────────────────────────
Error Message   : The JSON value could not be converted to System.Int32.
JSON Path       : $.data.items[0].id
Position        : Line 5, Position 22

📄 JSON Content
──────────────────────────────────────────────────────────────

Request Payload:
  {
    "filter": "active",
    "limit": 10
  }

Response Content:
  {
    "data": {
      "items": [
        {
          "id": "abc",    ← ERROR HERE!
          "name": "Test"
        }
      ]
    }
  }

Expected JSON (from file):
  {
    "data": {
      "items": [
        {
          "id": 123,
          "name": "Example"
        }
      ]
    }
  }

💡 What This Means
──────────────────────────────────────────────────────────────

The JSON response could not be deserialized. Common causes:

  • Type mismatch - the JSON value doesn't match the expected C# type
  • Example: trying to parse "abc" as an integer

Suggestions:
  1. Compare the Response Content with the Expected Type
  2. Check if property names match (case-sensitive!)
  3. Verify the response is actually JSON (not HTML error page)
  4. Use a JSON validator to check the response format

📝 Assert Call
──────────────────────────────────────────────────────────────

await Client.AssertPostAsync<MyResponseDto>(
    "api/data",
    "request.json",
    "expected.json");

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl --request POST 'http://localhost/api/data' \
--header 'Content-Type: application/json' \
--data-raw '{"filter":"active","limit":10}'

══════════════════════════════════════════════════════════════
```

## 🔍 Intelligente Error-Analyse

Der Handler analysiert die Exception Message und gibt spezifische Hinweise:

### Type Mismatch
```
Exception: "Could not convert string to integer"

Hints:
  • Type mismatch - the JSON value doesn't match the expected C# type
  • Example: trying to parse "abc" as an integer
```

### Invalid JSON Format
```
Exception: "Unexpected character encountered"

Hints:
  • Invalid JSON format - check for missing quotes, commas, or brackets
  • The response might not be valid JSON at all
```

### Missing Property
```
Exception: "Required property 'name' not found"

Hints:
  • Missing required property in the JSON
  • The expected type has [Required] properties that aren't in the response
```

### Duplicate Property
```
Exception: "Duplicate property 'id' found"

Hints:
  • Duplicate property names in the JSON
```

## 🎯 Registration

Der Handler ist automatisch registriert:

```csharp
services.AddTestErrorHandlingStrategy();
    ↓
    ├─ services.AddProblemDetailsErrorHandler();      // 1st priority
    ├─ services.AddJsonSerializationErrorHandler();   // 2nd priority ← NEW!
    └─ services.AddDefaultErrorHandler();             // Fallback
```

**Order matters!** JsonSerializationErrorHandler wird vor DefaultErrorHandler aufgerufen.

## 🔧 Implementation Details

### Handler Klasse
```csharp
internal sealed class JsonSerializationErrorHandler(
    ITableBuilder tableBuilder,
    ICurlBuilder curlBuilder,
    ICurlFormatter curlFormatter,
    ISourceCodeExtractor sourceCodeExtractor)
    : TestErrorHandler<JsonException>
{
    protected override Task<string> HandleExceptionAsync(
        IHttpAssertContext context,
        JsonException exception)
    {
        var errorOutput = BuildJsonSerializationError(context, exception);
        return Task.FromResult(errorOutput);
    }
}
```

### Key Features

#### 1. JSON Path Extraction
```csharp
private static string ExtractJsonPath(string message)
{
    // Patterns:
    // - "Path: $.data.items[0]"
    // - "at path '$.data.items[0]'"
    // - "JSON path $.data.items[0]"
    // - Just: $.data.items[0]
}
```

#### 2. Line/Position Info
```csharp
private static string ExtractLineInfo(JsonException exception)
{
    if (exception.LineNumber.HasValue)
    {
        var line = exception.LineNumber.Value;
        var pos = exception.BytePositionInLine ?? 0;
        return $"Line {line}, Position {pos}";
    }
}
```

#### 3. JSON Pretty-Printing
```csharp
private static string TryFormatJson(string json)
{
    try
    {
        using var doc = JsonDocument.Parse(json);
        // Format with indentation
        return formatted;
    }
    catch
    {
        // If invalid, return original
        return json;
    }
}
```

#### 4. JSON Indentation
```csharp
private static string IndentJson(string json, int spaces)
{
    var indent = new string(' ', spaces);
    return string.Join(Environment.NewLine, 
        lines.Select(line => indent + line));
}
```

## 📊 Comparison: Before vs After

### ❌ Before (Generic Error)
```
System.Text.Json.JsonException: The JSON value could not be converted to System.Int32.
   at System.Text.Json.ThrowHelper.ThrowJsonException()
   at System.Text.Json.Serialization.JsonConverter...
```

**Problems:**
- ❌ No context about which test failed
- ❌ No information about what JSON caused the error
- ❌ No hint about what was expected
- ❌ Hard to debug

### ✅ After (Specific Handler)
```
══════════════════════════════════════════════════════════════
❌ JSON SERIALIZATION ERROR
══════════════════════════════════════════════════════════════

📦 Test Information      ← Know which test failed
🌍 HTTP Request/Response ← Know which endpoint
⚠️ JSON Error Details    ← Know exact error + path
📄 JSON Content          ← See actual vs expected JSON
💡 What This Means       ← Get specific hints
📝 Assert Call           ← See the test code
🔁 Reproduce Locally     ← Get curl command
```

**Benefits:**
- ✅ Vollständiger Context
- ✅ Sieht Request + Response + Expected
- ✅ JSON Path zeigt genau wo der Fehler ist
- ✅ Spezifische Hinweise für Fix
- ✅ Einfach zu reproducieren

## 🚀 Use Cases

### Use Case 1: Type Mismatch
```csharp
// Expected Type
public class UserDto
{
    public int Id { get; set; }      // ← Expects integer
    public string Name { get; set; }
}

// Actual Response
{
    "id": "abc",    // ← String instead of integer!
    "name": "John"
}

// Handler Output:
JSON Path: $.id
Error: Could not convert string to System.Int32
Hint: Type mismatch - trying to parse "abc" as an integer
```

### Use Case 2: Missing Property
```csharp
// Expected Type
public class UserDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }  // ← Required!
}

// Actual Response
{
    "id": 123
    // Missing "name" property!
}

// Handler Output:
Error: Required property 'Name' not found
Hint: Missing required property in the JSON
```

### Use Case 3: Invalid JSON
```csharp
// Actual Response (malformed)
{
    "id": 123,
    "name": "Test"   // ← Missing comma!
    "email": "test@example.com"
}

// Handler Output:
Error: Unexpected character encountered
Hint: Invalid JSON format - check for missing quotes, commas, or brackets
Position: Line 3, Position 4
```

## 🎓 Best Practices

### 1. Use Typed DTOs
```csharp
✅ GOOD:
await Client.AssertPostAsync<UserDto>(...);

❌ BAD:
await Client.AssertPostAsync<dynamic>(...);  // No type checking!
```

### 2. Match Property Names
```csharp
✅ GOOD:
public class UserDto
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }  // Matches API response
}

❌ BAD:
public class UserDto
{
    public int UserId { get; set; }  // Won't match "user_id" in JSON
}
```

### 3. Handle Nullable Types
```csharp
✅ GOOD:
public class UserDto
{
    public int? Age { get; set; }  // Allows null
}

❌ BAD:
public class UserDto
{
    public int Age { get; set; }  // Will fail if null in JSON
}
```

## 🔄 Handler Priority

```
Exception: JsonException
    ↓
1. ProblemDetailsErrorHandler
   CanHandle? No (not ProblemDetailsException)
    ↓
2. JsonSerializationErrorHandler
   CanHandle? Yes! ✅
    ↓
   BuildJsonSerializationError(...)
    ↓
   Return formatted error
    ↓
   Assert.Fail(errorOutput)
```

**If it was ProblemDetailsException:**
```
Exception: TestSdkProblemDetailsException
    ↓
1. ProblemDetailsErrorHandler
   CanHandle? Yes! ✅
    ↓
   (JsonSerializationErrorHandler never reached)
```

## ✅ Status

- ✅ Implementiert
- ✅ Registriert in Strategy
- ✅ Registriert in Static Initialization
- ✅ Zeigt Request + Response + Expected
- ✅ Extrahiert JSON Path
- ✅ Extrahiert Line/Position
- ✅ Pretty-prints JSON
- ✅ Gibt spezifische Hinweise
- ✅ Zeigt Curl Command

**Ready to use!** 🎉

## 🎯 Future Enhancements

### Mögliche Erweiterungen:

1. **JSON Schema Validation**
   - Zeige Schema Violations
   - Generiere JSON Schema aus C# Type

2. **Visual Diff**
   - Side-by-side Vergleich
   - Highlight Differences

3. **Auto-Fix Suggestions**
   - "Did you mean: UserId instead of user_id?"
   - Generate correct JSON structure

4. **JSON Path Highlighting**
   - Highlight error location in JSON
   - Show context around error

**Für jetzt: Sehr gut und ausreichend!** 👍
