# Invalid JSON Error Handler

## 🎯 Überblick

Ein **spezifischer Error Handler** für `InvalidJsonException` - zeigt was schief ging wenn das JSON nicht mal das richtige Format hat!

## 💡 Was ist InvalidJsonException?

`InvalidJsonException` wird geworfen wenn:
- Der Response **nicht** mit `{` (Object) startet
- Der Response **nicht** mit `[` (Array) startet
- Der Response ist **kein gültiges JSON** überhaupt

## ✨ Features

### 1. Format Validation Details
- ✅ Zeigt was für ein Format erwartet wird
- ✅ Zeigt was tatsächlich empfangen wurde
- ✅ Analysiert den Content Type automatisch

### 2. Content Analysis
- ✅ **Raw Content** - Zeigt first 50 characters
- ✅ **Detected Type** - HTML? XML? Plain Text?
- ✅ **Content Preview** - Zeigt bis zu 200 chars

### 3. Intelligente Type Detection
```csharp
"<!DOCTYPE html>" → HTML (probably an error page)
"<?xml version"   → XML
"<html>"         → HTML (probably an error page)
"plain text"     → Plain Text / Unknown format
"123"            → Numeric value
"true"           → Boolean value
"null"           → Null value
```

## 📋 Output Format

```
══════════════════════════════════════════════════════════════
❌ INVALID JSON FORMAT ERROR
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
Project         : MyTest.Project
Class           : SomeApiTest
Method          : Should_Return_Valid_Json
Line            : 42

🌍 HTTP Request/Response
──────────────────────────────────────────────────────────────
Method          : GET
URL             : http://localhost/api/data
Expected Type   : MyResponseDto

⚠️ Format Validation Error
──────────────────────────────────────────────────────────────

The provided content is not valid JSON format.

Valid JSON must:
  • Start with '{' for objects  OR
  • Start with '[' for arrays

Error           : Your given json string does not contain valid JSON...

📄 Content Analysis
──────────────────────────────────────────────────────────────

Response Content (Raw):
  <!DOCTYPE html><html><head><title>404 Not Found</title>...

Detected Type   : HTML (probably an error page)

First 50 characters:
  "<!DOCTYPE html><html><head><title>404 Not Foun..."

Expected JSON structure (from file):
  {"data": {"items": [...]}}

💡 What This Means
──────────────────────────────────────────────────────────────

The response is not in valid JSON format. Common causes:

  • The API returned an HTML error page (404, 500, etc.)
  • The response is plain text instead of JSON
  • The endpoint returned XML instead of JSON
  • Empty or whitespace-only response
  • The response starts with a BOM (Byte Order Mark)

Suggestions:
  1. Check the actual HTTP status code (might be an error)
  2. Verify the endpoint URL is correct
  3. Check if the API expects specific headers (Accept: application/json)
  4. Look at the 'Response Content' above - is it HTML/XML/plain text?
  5. Use the curl command below to test the endpoint manually

📝 Assert Call
──────────────────────────────────────────────────────────────

await Client.AssertGetAsync<MyResponseDto>("api/data", "expected.json");

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl --request GET 'http://localhost/api/data' \
--header 'Accept: application/json'

══════════════════════════════════════════════════════════════
```

## 🎯 Use Cases

### Use Case 1: HTML Error Page (404)
```
Request:  GET /api/wrong-endpoint
Response: <!DOCTYPE html><html>404 Not Found</html>

Handler Output:
├─ Detected Type: HTML (probably an error page)
├─ Hint: The API returned an HTML error page (404, 500, etc.)
└─ Suggestion: Check the actual HTTP status code
```

### Use Case 2: Plain Text Response
```
Request:  GET /api/status
Response: Service is running

Handler Output:
├─ Detected Type: Plain Text / Unknown format
├─ Hint: The response is plain text instead of JSON
└─ Suggestion: Check if the API expects specific headers
```

### Use Case 3: XML Instead of JSON
```
Request:  GET /api/data
Response: <?xml version="1.0"?><data>...</data>

Handler Output:
├─ Detected Type: XML
├─ Hint: The endpoint returned XML instead of JSON
└─ Suggestion: Verify the endpoint URL is correct
```

### Use Case 4: Empty Response
```
Request:  GET /api/data
Response: [empty]

Handler Output:
├─ Detected Type: Empty/Whitespace
├─ Hint: Empty or whitespace-only response
└─ Suggestion: Check if the API expects specific headers
```

## 🔄 Handler Priority

```
Exception: InvalidJsonException
    ↓
1. ProblemDetailsErrorHandler
   CanHandle? No (not TestSdkProblemDetailsException)
    ↓
2. InvalidJsonErrorHandler
   CanHandle? Yes! ✅
    ↓
   BuildInvalidJsonError(...)
    ↓
   (JsonSerializationErrorHandler never reached)
```

**Why before JsonSerializationErrorHandler?**
- `InvalidJsonException` is more specific (format validation)
- `JsonException` would match any JSON error
- We want to catch format errors first

## 📊 Content Type Detection

Der Handler analysiert den Content automatisch:

| Content Starts With | Detected As |
|--------------------|-------------|
| `<!DOCTYPE` or `<html` | HTML (probably an error page) |
| `<?xml` or `<` | XML |
| `{` | JSON Object (but parsing failed) |
| `[` | JSON Array (but parsing failed) |
| Only digits | Numeric value |
| `"..."` | Quoted string |
| `true` / `false` | Boolean value |
| `null` | Null value |
| Anything else | Plain Text / Unknown format |

## 🔧 Implementation Details

### Smart Content Analysis
```csharp
private static string AnalyzeContentType(string content)
{
    var trimmed = content.TrimStart();

    if (trimmed.StartsWith("<!DOCTYPE") || 
        trimmed.StartsWith("<html"))
        return "HTML (probably an error page)";

    if (trimmed.StartsWith("<?xml") || 
        trimmed.StartsWith("<"))
        return "XML";

    // ... more checks
}
```

### Content Preview
```csharp
// Shows first 200 chars by default
GetContentPreview(content, maxLength: 200)

// Shows first N chars for detail view
GetFirstCharacters(content, count: 50)
```

### Extract from Exception Message
```csharp
// Exception message format:
// "...Your invalid string is:\n{actual content}"

ExtractInvalidStringFromMessage(exception.Message)
```

## 📋 Registration

```csharp
services.AddTestErrorHandlingStrategy();
    ↓
    ├─ ProblemDetailsErrorHandler       (1st)
    ├─ InvalidJsonErrorHandler          (2nd) ← NEW! 🎉
    ├─ JsonSerializationErrorHandler    (3rd)
    └─ DefaultErrorHandler              (Fallback)
```

## ✅ Status

- ✅ Implementiert
- ✅ Registriert in Strategy (vor JsonSerializationErrorHandler)
- ✅ Registriert in Static Initialization
- ✅ Zeigt Raw Content + Preview
- ✅ Detected Type Analysis
- ✅ Intelligente Hints basierend auf Content Type
- ✅ Curl Command für Reproduction

## 🎯 Handler-Übersicht (Updated)

Jetzt haben wir **4 spezifische Handler**:

| # | Handler | Exception Type | Use Case |
|---|---------|---------------|----------|
| 1 | ProblemDetailsErrorHandler | `TestSdkProblemDetailsException` | API returned ProblemDetails |
| 2 | InvalidJsonErrorHandler | `InvalidJsonException` | Content is not JSON format ← **NEW!** |
| 3 | JsonSerializationErrorHandler | `JsonException` | JSON parsing/deserialization failed |
| 4 | DefaultErrorHandler | `Exception` | All other errors (catch-all) |

**Order matters!** More specific handlers first.

## 💡 Unterschied zu JsonSerializationErrorHandler

### InvalidJsonErrorHandler
- **When:** Content ist **kein JSON** (startet nicht mit `{` oder `[`)
- **Example:** HTML Error Page, XML, Plain Text
- **Focus:** Format validation
- **Shows:** Raw content, detected type, first characters

### JsonSerializationErrorHandler
- **When:** Content **ist JSON**, aber **Parsing failed**
- **Example:** Type mismatch, missing property, invalid value
- **Focus:** Deserialization errors
- **Shows:** JSON path, line/position, formatted JSON

**Both are important!** Sie decken verschiedene Error-Szenarien ab.

## 🚀 Real-World Examples

### Example 1: Wrong Endpoint (404)
```bash
curl http://localhost/api/wrong-url

Response:
<!DOCTYPE html>
<html>
  <head><title>404 Not Found</title></head>
  <body><h1>Not Found</h1></body>
</html>

→ InvalidJsonErrorHandler triggers
→ Detected Type: HTML (probably an error page)
→ Hint: Check the endpoint URL
```

### Example 2: Server Error (500)
```bash
curl http://localhost/api/broken

Response:
Internal Server Error

→ InvalidJsonErrorHandler triggers
→ Detected Type: Plain Text / Unknown format
→ Hint: The response is plain text instead of JSON
```

### Example 3: XML API
```bash
curl http://localhost/api/legacy

Response:
<?xml version="1.0"?>
<data><item>value</item></data>

→ InvalidJsonErrorHandler triggers
→ Detected Type: XML
→ Hint: The endpoint returned XML instead of JSON
```

## 🎊 Result

**Alle JSON-bezogenen Fehler sind jetzt covered:**
- ✅ **Format Error** → InvalidJsonErrorHandler
- ✅ **Parsing Error** → JsonSerializationErrorHandler  
- ✅ **ProblemDetails** → ProblemDetailsErrorHandler
- ✅ **Anything else** → DefaultErrorHandler

**Kein unformatierter JSON-Error mehr!** 🚀
