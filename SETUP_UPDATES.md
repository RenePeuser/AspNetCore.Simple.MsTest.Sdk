# Setup Updates - Error Handling Strategy Integration

## 🔧 Setup Method Updates

### Client.Assert.HttpCall.cs Changes

**File:** `AssertExtensions/Client.Assert.HttpCall.cs`

#### 1. Added Using Statement
```csharp
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
```

#### 2. Updated Setup Method

**Before:**
```csharp
// 9. Most important: Rebuild AssertableHttpClient with all updated components
_assertableHttpClientDefault = new AssertableHttpClient.AssertableHttpClient(
    _httpCallHandler,
    _parameterReplacer,
    _httpAssertionPipeline,
    _primitiveTypeConverter,
    _jsonSerializerOptions,
    _endpointValidator);
```

**After:**
```csharp
// 9. Resolve error handling strategy
var testErrorHandlingStrategy = serviceProvider.GetRequiredService<ITestErrorHandlingStrategy>();

// 10. Most important: Rebuild AssertableHttpClient with all updated components
_assertableHttpClientDefault = new AssertableHttpClient.AssertableHttpClient(
    _httpCallHandler,
    _parameterReplacer,
    _httpAssertionPipeline,
    _primitiveTypeConverter,
    _jsonSerializerOptions,
    _endpointValidator,
    testErrorHandlingStrategy);  // ← NEW PARAMETER!
```

## 📋 Complete Setup Flow

```
Setup(IServiceProvider serviceProvider)
    ↓
1. Resolve core services (primitiveTypeConverter, jsonDiffer, etc.)
    ↓
2. Resolve builders (tableBuilder, curlBuilder, etc.)
    ↓
3. Create output strategies
    ↓
4. Create assert service
    ↓
5. Resolve HTTP handler
    ↓
6. Create HTTP assertion pipeline
    ↓
7. Resolve validation services
    ↓
8. Create endpoint validator
    ↓
9. Resolve ITestErrorHandlingStrategy ← NEW!
    ↓
10. Create AssertableHttpClient with ALL dependencies (including error handling strategy)
    ↓
Done! ✅
```

## ✅ Integration Complete

Die Error Handling Strategy ist jetzt vollständig in das Setup integriert:

1. ✅ Using Statement hinzugefügt
2. ✅ Strategy aus DI Container aufgelöst
3. ✅ Strategy an AssertableHttpClient übergeben
4. ✅ AssertableHttpClient kann jetzt Exceptions mit Strategy handlen

## 🚀 Test

```csharp
// In deinem Test-Setup
services.AddAssertableHttpClient(configuration);

var serviceProvider = services.BuildServiceProvider();
HttpClientAssertExtensions.Setup(serviceProvider);

// Jetzt funktioniert Error Handling automatisch!
await Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(
    "api/endpoint",
    "request.json", 
    "expected.json");

// Falls ProblemDetailsException auftritt:
// → ProblemDetailsErrorHandler formatiert es schön
// Falls andere Exception auftritt:
// → DefaultErrorHandler formatiert es schön
```

## 🎯 Service Registration

Die Strategy wird automatisch registriert via:
```csharp
services.AddAssertableHttpClient(configuration);
    ↓
services.AddTestErrorHandlingStrategy();
    ↓
    ├─ services.AddProblemDetailsErrorHandler();
    ├─ services.AddDefaultErrorHandler();
    └─ services.AddSingletonIfNotExists<ITestErrorHandlingStrategy, TestErrorHandlingStrategy>();
```

Alles ist DI-basiert und wird automatisch aufgelöst! 🎉
