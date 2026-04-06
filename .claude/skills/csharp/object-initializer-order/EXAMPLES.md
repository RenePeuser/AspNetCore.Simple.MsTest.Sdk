# Object Initializer Order Examples

## Prefer: Alphabetical Property Order

```csharp
var responseContext = new HttpResponseContext<TResult>
{
    AbsoluteUrl = absoluteUrl,
    CallerFilePath = context.CallerFilePath,
    CallingAssembly = context.CallingAssembly,
    Client = context.Client,
    ContentAsString = contentAsString,
    ContentAsStringParameterized = resolvedParametersJsonString,
    Current = context.Current,
    CurrentObject = currentResult,
    CurrentResult = currentResult,
    CurrentResultParameterName = context.CurrentResultParameterName,
    DifferenceFunc = context.DifferenceFunc,
    ExpectedObjectAsJson = context.ExpectedObjectAsJson,
    ExpectedResultFile = context.ExpectedResultFile,
    ExpectedResultParameterName = context.ExpectedResultParameterName,
    HttpMethod = context.HttpMethod,
    HttpResponseMessage = httpResponseMessage,
    HttpStatusCode = httpResponseMessage.StatusCode,
    IsExpectedStatusCode = isExpectedStatusCode,
    IsSuccessStatusCode = context.IsSuccessStatusCode,
    OrderFunc = context.OrderFunc,
    Parameters = context.Parameters,
    PayloadAsJson = context.PayloadAsJson,
    PayloadFile = context.PayloadFile,
    PayloadParameterName = context.PayloadParameterName,
    ResolvedExpectedJson = context.ResolvedExpectedJson,
    ResolvedPayload = context.ResolvedPayload,
    ShowTokenInCurl = context.ShowTokenInCurl,
    TypeIsPrimitiveType = targetIsPrimitiveType,
    Url = context.Url,
    WriteResponse = context.WriteResponse,
};
```

## Avoid: Random or Insertion Order

```csharp
var responseContext = new HttpResponseContext<TResult>
{
    TypeIsPrimitiveType = targetIsPrimitiveType,
    HttpResponseMessage = httpResponseMessage,
    HttpStatusCode = httpResponseMessage.StatusCode,
    ContentAsString = contentAsString,
    CurrentResult = currentResult,
    AbsoluteUrl = absoluteUrl,
    Client = context.Client,
    Url = context.Url,
    PayloadFile = context.PayloadFile,
    IsSuccessStatusCode = context.IsSuccessStatusCode,
    Parameters = context.Parameters,
    ExpectedResultFile = context.ExpectedResultFile,
    CallerFilePath = context.CallerFilePath,
};
```

## Prefer: Small Initializer (Alphabetical)

```csharp
var config = new ServiceConfiguration
{
    EnableLogging = true,
    RetryCount = 3,
    Timeout = TimeSpan.FromSeconds(30),
};
```

## Avoid: Small Initializer (Random)

```csharp
var config = new ServiceConfiguration
{
    RetryCount = 3,
    Timeout = TimeSpan.FromSeconds(30),
    EnableLogging = true,
};
```

## Acceptable: Domain-Grouped with Alphabetical Within Groups

When domain grouping is clearly beneficial and obvious:

```csharp
var audit = new AuditRecord
{
    // Identity properties
    Id = recordId,
    TenantId = tenantId,
    UserId = userId,
    
    // Audit properties
    Action = auditAction,
    Timestamp = timestamp,
    
    // Metadata properties
    IpAddress = ipAddress,
    UserAgent = userAgent,
};
```

Note: Each group is sorted alphabetically. Use this pattern only when grouping is self-evident.

## Key Difference

**Prefer**: Properties sorted alphabetically - easy to find, easy to diff, consistent placement
**Avoid**: Properties in random order - requires full scan to find, unpredictable diffs, merge conflicts
