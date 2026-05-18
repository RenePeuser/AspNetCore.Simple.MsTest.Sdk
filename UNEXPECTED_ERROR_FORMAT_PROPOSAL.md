# Unexpected Error Output Format Proposal

## Overview
This document proposes a standardized output format for `ProblemDetailsException` errors that occur during JSON deserialization in the test SDK.

## Proposed Structure

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
Title      : An unexpected error occurred during deserialization
Detail     : The API returned a ProblemDetails response that could not be 
             properly deserialized into the expected type.

Type       : https://tools.ietf.org/html/rfc7807
Instance   : /api/core/v1/admin/health-metrics/collect

📋 Extension Data
──────────────────────────────────────────────────────────────

┌──────────────────┬───────────────────────────────────────────────┐
│ Key              │ Value                                         │
├──────────────────┼───────────────────────────────────────────────┤
│ traceId          │ 00-123abc456def789-0af7651916cd43dd-00       │
│ errorCode        │ VALIDATION_FAILED                             │
│ timestamp        │ 2026-05-18T10:30:45Z                         │
└──────────────────┴───────────────────────────────────────────────┘

💡 What This Means
──────────────────────────────────────────────────────────────

The API endpoint returned an error response (ProblemDetails), but the test 
expected a different response type. This typically indicates:

  • The endpoint encountered an unexpected error
  • The test's expected response type doesn't match what the API returned
  • There may be a validation or server-side processing issue

📝 Assert Call
──────────────────────────────────────────────────────────────

await Client.AssertPostAsErrorAsync<ValidationProblemDetailsExtended>(
    $"api/core/v1/admin/health-metrics/collect",
    $"{usecase}.json", 
    $"{usecase}.json").ConfigureAwait(false);

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl \
--location \
--request POST 'http://localhost/api/core/v1/admin/health-metrics/collect' \
--header 'Authorization: Bearer Sorry i am secret :)' \
--header 'Content-Type: application/json' \
--data-raw '{"domainNames":["not-ex1st1ng-d0ma1n-111"]}'

══════════════════════════════════════════════════════════════
```

## Key Features

1. **Consistent Structure**: Follows the same pattern as "HTTP RESPONSE TYPE MISMATCH" errors
2. **Clear Sections**: Organized with emoji icons for easy visual scanning
3. **Complete Information**: Shows all ProblemDetails properties and extensions
4. **Helpful Context**: "What This Means" section explains the error in plain language
5. **Actionable**: Includes the test code and curl command for reproduction

## Implementation Details

### Method Signature
```csharp
string BuildUnexpectedError(
    IHttpAssertContext context,
    ProblemDetailsException exception,
    EndpointInfo? endpoint = null);
```

### Key Components
- **Test Information**: Project, class, method, line number
- **HTTP Details**: Method, URL, status code
- **Problem Details**: All RFC 7807 fields (status, title, detail, type, instance)
- **Extension Data**: Any additional data in the Extensions dictionary
- **Explanatory Text**: User-friendly explanation of what happened
- **Assert Call**: The actual test code that triggered the error
- **Curl Command**: Reproduction instructions

### Color/Decoration Scheme
- Header: `textDecorator.Error()` (red)
- Section Titles: `textDecorator.SectionTitle()` (cyan/blue)
- Separators: `textDecorator.Dim()` (gray)
- Success/Code: `textDecorator.Success()` (green)
- Highlights: `textDecorator.Highlight()` (yellow)

## Usage Example

```csharp
catch (ProblemDetailsException problemDetailsException)
{
    var errorOutput = endpointValidationOutputBuilder.BuildUnexpectedError(
        context,
        problemDetailsException,
        endpoint);
    
    Assert.Fail(errorOutput);
}
```

## Benefits

1. **Consistency**: Matches existing error output format
2. **Clarity**: Clear, structured information that's easy to scan
3. **Debugging**: All necessary information for troubleshooting
4. **Professional**: Polished output that looks intentional, not like a raw exception
5. **Actionable**: Provides reproduction steps and context

## Alternative Titles Considered

- ❌ UNEXPECTED API ERROR (Recommended)
- ❌ API PROBLEM DETAILS ERROR
- ❌ DESERIALIZATION ERROR
- ❌ UNEXPECTED RESPONSE ERROR
- ⚠️ API ERROR RESPONSE

"UNEXPECTED API ERROR" was chosen because it:
- Clearly indicates something unexpected happened
- Doesn't assume deserialization is the root cause
- Matches the severity of the situation
- Is concise and clear

## Implementation Summary

### Files Created
1. **`ProblemDetails/ProblemDetailsOutputBuilder.cs`** - New isolated output builder for ProblemDetails exceptions
   - Interface: `IProblemDetailsOutputBuilder`
   - Method: `BuildUnexpectedError(IHttpAssertContext, ProblemDetailsException, EndpointInfo?)`
   - Handles formatting of all ProblemDetails fields and extensions
   - Includes text wrapping for long detail messages
   - Provides status code coloring based on HTTP status ranges

### Files Modified
1. **`Comparison/JsonComparisonStrategy.cs`** 
   - Changed from `Assert.Fail("aaaa")` to `throw;`
   - Allows exception to propagate to proper handler

2. **`AssertableHttpClient/Pipelines/Steps/JsonComparisonStep.cs`**
   - Added try-catch block around `assertService.ObjectsAreEqual()`
   - Catches `ProblemDetailsException` and formats error using `ProblemDetailsOutputBuilder`
   - This is where HTTP context is available, making it the ideal place to handle the exception

3. **`Assert/AssertService.cs`**
   - Added try-catch with documentation comment
   - Rethrows to allow handling at HTTP level where context is available

### Registration
Add to DI container setup where needed:
```csharp
services.AddProblemDetailsOutputBuilder();
```

### Error Flow
1. API returns ProblemDetails response
2. `JsonSerializer.Deserialize<T>()` throws `ProblemDetailsException` in `JsonComparisonStrategy`
3. Exception propagates through `AssertService.ObjectsAreEqual()`
4. Caught in `JsonComparisonStep.Execute()` where HTTP context is available
5. `ProblemDetailsOutputBuilder.BuildUnexpectedError()` creates formatted output
6. `Assert.That.Fail()` called with formatted message
7. Developer sees nice, structured error instead of raw exception
