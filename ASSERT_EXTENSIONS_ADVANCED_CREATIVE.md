# Advanced & Creative Assert Extensions
**Going beyond the basics - Domain-specific, semantic, and ultra-specialized assertions**

---

## 🎨 **17. HTTP/WEB CHECKS**

```csharp
// URL is valid
Assert.That.IsValidUrl(
    url: string,
    because: string,
    fix: string
)

// URL is absolute
Assert.That.IsAbsoluteUrl(
    url: string,
    because: string,
    fix: string
)

// URL has scheme
Assert.That.UrlHasScheme(
    url: string,
    expectedScheme: string,  // "https"
    because: string,
    fix: string
)

// URL is reachable (makes actual HTTP call)
Assert.That.UrlIsReachable(
    url: string,
    timeout: TimeSpan,
    because: string,
    fix: string
)

// Email is valid format
Assert.That.IsValidEmail(
    email: string,
    because: string,
    fix: string
)

// Content-Type header is correct
Assert.That.HasContentType(
    response: HttpResponseMessage,
    expectedContentType: string,
    because: string,
    fix: string
)

// Status code is in success range
Assert.That.IsSuccessStatusCode(
    statusCode: HttpStatusCode,
    because: string,
    fix: string
)

// Status code is in error range
Assert.That.IsErrorStatusCode(
    statusCode: HttpStatusCode,
    because: string,
    fix: string
)
```

---

## 🎨 **18. SECURITY/VALIDATION CHECKS**

```csharp
// Password meets strength requirements
Assert.That.IsStrongPassword(
    password: string,
    minLength: int,
    requireUppercase: bool,
    requireLowercase: bool,
    requireDigit: bool,
    requireSpecialChar: bool,
    because: string,
    fix: string
)

// String is sanitized (no SQL injection patterns)
Assert.That.IsSanitized(
    input: string,
    because: string,
    fix: string
)

// String contains no XSS patterns
Assert.That.IsXssSafe(
    input: string,
    because: string,
    fix: string
)

// JWT token is valid
Assert.That.IsValidJwt(
    token: string,
    because: string,
    fix: string
)

// JWT token is not expired
Assert.That.JwtIsNotExpired(
    token: string,
    because: string,
    fix: string
)

// API key format is valid
Assert.That.IsValidApiKey(
    apiKey: string,
    expectedFormat: string,  // regex or pattern
    because: string,
    fix: string
)
```

---

## 🎨 **19. BUSINESS RULE CHECKS**

```csharp
// Value is within business hours
Assert.That.IsWithinBusinessHours(
    dateTime: DateTime,
    businessStart: TimeSpan,
    businessEnd: TimeSpan,
    because: string,
    fix: string
)

// Date is a business day (not weekend)
Assert.That.IsBusinessDay(
    date: DateTime,
    because: string,
    fix: string
)

// Age is valid for rule (18+, 21+, etc.)
Assert.That.MeetsAgeRequirement(
    birthDate: DateTime,
    minimumAge: int,
    because: string,
    fix: string
)

// Price is within budget
Assert.That.IsWithinBudget(
    price: decimal,
    budget: decimal,
    because: string,
    fix: string
)

// Discount is valid percentage
Assert.That.IsValidDiscountPercentage(
    discount: decimal,
    minPercent: decimal,
    maxPercent: decimal,
    because: string,
    fix: string
)

// SKU format is valid
Assert.That.IsValidSku(
    sku: string,
    pattern: string,
    because: string,
    fix: string
)

// Credit card number passes Luhn check
Assert.That.IsValidCreditCardNumber(
    cardNumber: string,
    because: string,
    fix: string
)

// Phone number is valid format
Assert.That.IsValidPhoneNumber(
    phoneNumber: string,
    countryCode: string,
    because: string,
    fix: string
)

// Postal code is valid
Assert.That.IsValidPostalCode(
    postalCode: string,
    countryCode: string,
    because: string,
    fix: string
)

// Currency code is valid (ISO 4217)
Assert.That.IsValidCurrencyCode(
    currencyCode: string,
    because: string,
    fix: string
)
```

---

## 🎨 **20. PERFORMANCE/TIMING CHECKS**

```csharp
// Operation completes quickly
Assert.That.CompletesQuickly(
    action: Action,
    maxDuration: TimeSpan,
    because: string,
    fix: string
)

// Async operation completes quickly
Assert.That.CompletesQuicklyAsync(
    action: Func<Task>,
    maxDuration: TimeSpan,
    because: string,
    fix: string
)

// Operation takes at least minimum time
Assert.That.TakesAtLeast(
    action: Action,
    minDuration: TimeSpan,
    because: string,
    fix: string
)

// Memory allocation is under limit
Assert.That.AllocatesLessThan(
    action: Action,
    maxBytes: long,
    because: string,
    fix: string
)

// Throughput meets requirement
Assert.That.MeetsThroughput(
    action: Action,
    iterations: int,
    maxDuration: TimeSpan,
    because: string,
    fix: string
)
```

---

## 🎨 **21. DATABASE/DATA CHECKS**

```csharp
// Connection string is valid
Assert.That.IsValidConnectionString(
    connectionString: string,
    because: string,
    fix: string
)

// Table exists in database
Assert.That.TableExists(
    tableName: string,
    connectionString: string,
    because: string,
    fix: string
)

// Column exists in table
Assert.That.ColumnExists(
    tableName: string,
    columnName: string,
    connectionString: string,
    because: string,
    fix: string
)

// Row count matches expectation
Assert.That.HasRowCount(
    tableName: string,
    expectedCount: int,
    connectionString: string,
    because: string,
    fix: string
)

// Query returns results
Assert.That.QueryReturnsResults(
    query: string,
    connectionString: string,
    because: string,
    fix: string
)

// Primary key is set
Assert.That.HasPrimaryKey<T>(
    entity: T,
    keySelector: Func<T, object>,
    because: string,
    fix: string
) where T : class
```

---

## 🎨 **22. XML/MARKUP CHECKS**

```csharp
// String is valid XML
Assert.That.IsValidXml(
    xml: string,
    because: string,
    fix: string
)

// XML contains XPath
Assert.That.XmlContainsPath(
    xml: string,
    xpath: string,
    because: string,
    fix: string
)

// XML XPath equals value
Assert.That.XmlPathEquals(
    xml: string,
    xpath: string,
    expectedValue: string,
    because: string,
    fix: string
)

// HTML is valid
Assert.That.IsValidHtml(
    html: string,
    because: string,
    fix: string
)

// HTML contains selector (CSS)
Assert.That.HtmlContainsSelector(
    html: string,
    cssSelector: string,
    because: string,
    fix: string
)
```

---

## 🎨 **23. CONFIGURATION/ENVIRONMENT CHECKS**

```csharp
// Environment variable exists
Assert.That.EnvironmentVariableExists(
    variableName: string,
    because: string,
    fix: string
)

// Environment variable equals value
Assert.That.EnvironmentVariableEquals(
    variableName: string,
    expectedValue: string,
    because: string,
    fix: string
)

// Configuration key exists
Assert.That.ConfigKeyExists(
    configuration: IConfiguration,
    key: string,
    because: string,
    fix: string
)

// Configuration value equals
Assert.That.ConfigValueEquals(
    configuration: IConfiguration,
    key: string,
    expectedValue: string,
    because: string,
    fix: string
)

// Feature flag is enabled
Assert.That.FeatureFlagIsEnabled(
    flagName: string,
    because: string,
    fix: string
)

// Running in specific environment
Assert.That.IsEnvironment(
    expectedEnvironment: string,  // "Development", "Production"
    because: string,
    fix: string
)
```

---

## 🎨 **24. REFLECTION/METADATA CHECKS**

```csharp
// Type has attribute
Assert.That.HasAttribute<TAttribute>(
    type: Type,
    because: string,
    fix: string
) where TAttribute : Attribute

// Member has attribute
Assert.That.MemberHasAttribute<TAttribute>(
    memberInfo: MemberInfo,
    because: string,
    fix: string
) where TAttribute : Attribute

// Property exists
Assert.That.HasProperty(
    type: Type,
    propertyName: string,
    because: string,
    fix: string
)

// Property is readable
Assert.That.PropertyIsReadable(
    type: Type,
    propertyName: string,
    because: string,
    fix: string
)

// Property is writable
Assert.That.PropertyIsWritable(
    type: Type,
    propertyName: string,
    because: string,
    fix: string
)

// Method exists
Assert.That.HasMethod(
    type: Type,
    methodName: string,
    because: string,
    fix: string
)

// Type is sealed
Assert.That.IsSealed(
    type: Type,
    because: string,
    fix: string
)

// Type is abstract
Assert.That.IsAbstract(
    type: Type,
    because: string,
    fix: string
)
```

---

## 🎨 **25. STREAM/BINARY CHECKS**

```csharp
// Stream is readable
Assert.That.IsReadable(
    stream: Stream,
    because: string,
    fix: string
)

// Stream is writable
Assert.That.IsWritable(
    stream: Stream,
    because: string,
    fix: string
)

// Stream is seekable
Assert.That.IsSeekable(
    stream: Stream,
    because: string,
    fix: string
)

// Stream has data
Assert.That.HasData(
    stream: Stream,
    because: string,
    fix: string
)

// Stream length equals
Assert.That.HasLength(
    stream: Stream,
    expectedLength: long,
    because: string,
    fix: string
)

// Byte array starts with signature
Assert.That.StartsWithSignature(
    data: byte[],
    signature: byte[],
    because: string,
    fix: string
)

// File has magic number (PNG, PDF, etc.)
Assert.That.HasMagicNumber(
    filePath: string,
    expectedMagicNumber: byte[],
    because: string,
    fix: string
)
```

---

## 🎨 **26. DEPENDENCY INJECTION CHECKS**

```csharp
// Service is registered
Assert.That.ServiceIsRegistered<TService>(
    services: IServiceCollection,
    because: string,
    fix: string
)

// Service has lifetime
Assert.That.ServiceHasLifetime<TService>(
    services: IServiceCollection,
    expectedLifetime: ServiceLifetime,
    because: string,
    fix: string
)

// Service can be resolved
Assert.That.ServiceCanBeResolved<TService>(
    serviceProvider: IServiceProvider,
    because: string,
    fix: string
)

// All dependencies can be resolved
Assert.That.AllDependenciesCanBeResolved(
    serviceProvider: IServiceProvider,
    because: string,
    fix: string
)
```

---

## 🎨 **27. SEMANTIC/DOMAIN CHECKS**

```csharp
// Order is valid
Assert.That.IsValidOrder<TOrder>(
    order: TOrder,
    validationRules: Func<TOrder, ValidationResult>,
    because: string,
    fix: string
)

// User has role
Assert.That.HasRole(
    user: ClaimsPrincipal,
    roleName: string,
    because: string,
    fix: string
)

// User has claim
Assert.That.HasClaim(
    user: ClaimsPrincipal,
    claimType: string,
    claimValue: string,
    because: string,
    fix: string
)

// User is authenticated
Assert.That.IsAuthenticated(
    user: ClaimsPrincipal,
    because: string,
    fix: string
)

// User is authorized for resource
Assert.That.IsAuthorized(
    user: ClaimsPrincipal,
    resource: string,
    action: string,
    because: string,
    fix: string
)
```

---

## 🎨 **28. GRAPH/TREE CHECKS**

```csharp
// Tree has depth
Assert.That.HasDepth<TNode>(
    root: TNode,
    expectedDepth: int,
    childSelector: Func<TNode, IEnumerable<TNode>>,
    because: string,
    fix: string
)

// Graph has cycle
Assert.That.HasCycle<TNode>(
    root: TNode,
    adjacencySelector: Func<TNode, IEnumerable<TNode>>,
    because: string,
    fix: string
)

// Graph is connected
Assert.That.IsConnected<TNode>(
    nodes: IEnumerable<TNode>,
    adjacencySelector: Func<TNode, IEnumerable<TNode>>,
    because: string,
    fix: string
)

// Tree is balanced
Assert.That.IsBalanced<TNode>(
    root: TNode,
    childSelector: Func<TNode, IEnumerable<TNode>>,
    because: string,
    fix: string
)
```

---

## 🎨 **29. CRYPTOGRAPHIC CHECKS**

```csharp
// String is Base64
Assert.That.IsBase64(
    value: string,
    because: string,
    fix: string
)

// Hash matches
Assert.That.HashMatches(
    data: byte[],
    expectedHash: byte[],
    algorithm: HashAlgorithm,
    because: string,
    fix: string
)

// Signature is valid
Assert.That.SignatureIsValid(
    data: byte[],
    signature: byte[],
    publicKey: byte[],
    because: string,
    fix: string
)

// Certificate is valid
Assert.That.CertificateIsValid(
    certificate: X509Certificate2,
    because: string,
    fix: string
)

// Certificate is not expired
Assert.That.CertificateIsNotExpired(
    certificate: X509Certificate2,
    because: string,
    fix: string
)
```

---

## 🎨 **30. EVENT/MESSAGE CHECKS**

```csharp
// Event was raised
Assert.That.EventWasRaised(
    eventName: string,
    action: Action,
    because: string,
    fix: string
)

// Event was raised N times
Assert.That.EventWasRaisedTimes(
    eventName: string,
    expectedCount: int,
    action: Action,
    because: string,
    fix: string
)

// Message was published
Assert.That.MessageWasPublished<TMessage>(
    action: Action,
    because: string,
    fix: string
)

// Message queue is empty
Assert.That.QueueIsEmpty(
    queueName: string,
    because: string,
    fix: string
)

// Message queue has count
Assert.That.QueueHasCount(
    queueName: string,
    expectedCount: int,
    because: string,
    fix: string
)
```

---

## 🎨 **31. CONCURRENCY/THREAD CHECKS**

```csharp
// Operation is thread-safe
Assert.That.IsThreadSafe(
    action: Action,
    threadCount: int,
    iterations: int,
    because: string,
    fix: string
)

// Lock is held
Assert.That.LockIsHeld(
    lockObject: object,
    because: string,
    fix: string
)

// Semaphore has available slots
Assert.That.HasAvailableSlots(
    semaphore: SemaphoreSlim,
    minimumSlots: int,
    because: string,
    fix: string
)

// Task is running on thread pool
Assert.That.IsRunningOnThreadPool(
    task: Task,
    because: string,
    fix: string
)
```

---

## 🎨 **32. COMPARISON/MATH CHECKS**

```csharp
// Value is even
Assert.That.IsEven(
    value: int,
    because: string,
    fix: string
)

// Value is odd
Assert.That.IsOdd(
    value: int,
    because: string,
    fix: string
)

// Value is prime
Assert.That.IsPrime(
    value: int,
    because: string,
    fix: string
)

// Value is power of 2
Assert.That.IsPowerOfTwo(
    value: int,
    because: string,
    fix: string
)

// Value is divisible by
Assert.That.IsDivisibleBy(
    value: int,
    divisor: int,
    because: string,
    fix: string
)

// Percentage is valid
Assert.That.IsValidPercentage(
    value: decimal,
    because: string,
    fix: string
)

// Ratio is within tolerance
Assert.That.RatioIsCloseTo(
    numerator: double,
    denominator: double,
    expectedRatio: double,
    tolerance: double,
    because: string,
    fix: string
)
```

---

## 🎨 **33. COLOR/MEDIA CHECKS**

```csharp
// Color is valid hex
Assert.That.IsValidHexColor(
    color: string,
    because: string,
    fix: string
)

// Image dimensions are correct
Assert.That.HasDimensions(
    imagePath: string,
    expectedWidth: int,
    expectedHeight: int,
    because: string,
    fix: string
)

// Image format is correct
Assert.That.HasImageFormat(
    imagePath: string,
    expectedFormat: string,  // "PNG", "JPEG"
    because: string,
    fix: string
)

// Video duration is within range
Assert.That.HasDuration(
    videoPath: string,
    minDuration: TimeSpan,
    maxDuration: TimeSpan,
    because: string,
    fix: string
)
```

---

## 🎨 **34. VERSIONING CHECKS**

```csharp
// Version is valid SemVer
Assert.That.IsValidSemVer(
    version: string,
    because: string,
    fix: string
)

// Version is greater than
Assert.That.VersionIsGreaterThan(
    actual: Version,
    minimum: Version,
    because: string,
    fix: string
)

// Version is compatible
Assert.That.IsCompatibleVersion(
    actual: Version,
    required: Version,
    because: string,
    fix: string
)

// Assembly version matches
Assert.That.AssemblyVersionMatches(
    assembly: Assembly,
    expectedVersion: Version,
    because: string,
    fix: string
)
```

---

## 🎨 **35. LOCALIZATION/CULTURE CHECKS**

```csharp
// Culture is supported
Assert.That.CultureIsSupported(
    cultureName: string,
    supportedCultures: IEnumerable<string>,
    because: string,
    fix: string
)

// Resource key exists
Assert.That.ResourceKeyExists(
    resourceManager: ResourceManager,
    key: string,
    culture: CultureInfo,
    because: string,
    fix: string
)

// Translation is not empty
Assert.That.TranslationExists(
    resourceManager: ResourceManager,
    key: string,
    culture: CultureInfo,
    because: string,
    fix: string
)
```

---

## 🎨 **36. RATE LIMITING/QUOTA CHECKS**

```csharp
// Rate limit is not exceeded
Assert.That.RateLimitNotExceeded(
    action: Action,
    maxCalls: int,
    timeWindow: TimeSpan,
    because: string,
    fix: string
)

// Quota is available
Assert.That.QuotaIsAvailable(
    used: long,
    limit: long,
    because: string,
    fix: string
)

// Throttle is respected
Assert.That.ThrottleIsRespected(
    action: Action,
    minimumInterval: TimeSpan,
    because: string,
    fix: string
)
```

---

## 🎨 **37. CACHING CHECKS**

```csharp
// Cache contains key
Assert.That.CacheContainsKey(
    cache: IMemoryCache,
    key: string,
    because: string,
    fix: string
)

// Cache entry is not expired
Assert.That.CacheEntryIsValid(
    cache: IMemoryCache,
    key: string,
    because: string,
    fix: string
)

// Cache hit rate is acceptable
Assert.That.CacheHitRateIsAcceptable(
    hits: int,
    misses: int,
    minimumHitRate: double,
    because: string,
    fix: string
)
```

---

## 🎨 **38. LOGGING/TELEMETRY CHECKS**

```csharp
// Log contains message
Assert.That.LogContains(
    logs: IEnumerable<string>,
    expectedMessage: string,
    because: string,
    fix: string
)

// Log level was used
Assert.That.LogLevelWasUsed(
    logs: IEnumerable<LogEntry>,
    expectedLevel: LogLevel,
    because: string,
    fix: string
)

// Metric was recorded
Assert.That.MetricWasRecorded(
    metricName: string,
    because: string,
    fix: string
)

// Trace has expected span count
Assert.That.TraceHasSpanCount(
    trace: Trace,
    expectedCount: int,
    because: string,
    fix: string
)
```

---

## 🎨 **39. API CONTRACT CHECKS**

```csharp
// Response matches OpenAPI schema
Assert.That.MatchesOpenApiSchema(
    response: HttpResponseMessage,
    schemaPath: string,
    operationId: string,
    because: string,
    fix: string
)

// Request headers are valid
Assert.That.HasRequiredHeaders(
    request: HttpRequestMessage,
    requiredHeaders: IEnumerable<string>,
    because: string,
    fix: string
)

// API version is supported
Assert.That.ApiVersionIsSupported(
    version: string,
    supportedVersions: IEnumerable<string>,
    because: string,
    fix: string
)
```

---

## 🎨 **40. PATTERN MATCHING CHECKS**

```csharp
// Object matches pattern
Assert.That.Matches<T>(
    obj: T,
    pattern: object,  // anonymous object pattern
    because: string,
    fix: string
)

// Collection matches pattern sequence
Assert.That.MatchesSequence<T>(
    collection: IEnumerable<T>,
    patterns: params Func<T, bool>[],
    because: string,
    fix: string
)

// State machine is in expected state
Assert.That.IsInState<TState>(
    stateMachine: IStateMachine<TState>,
    expectedState: TState,
    because: string,
    fix: string
)
```

---

## 🚀 **BONUS: INTELLIGENT ASSERTS**

```csharp
// Auto-detect issue and suggest fix
Assert.That.IsValid<T>(
    value: T,
    because: string
)  // Auto-generates fix suggestions based on validation failures

// Assert with retry logic
Assert.That.EventuallyBecomes<T>(
    valueFactory: Func<T>,
    expectedValue: T,
    timeout: TimeSpan,
    pollInterval: TimeSpan,
    because: string,
    fix: string
)

// Snapshot testing (auto-creates baseline)
Assert.That.MatchesSnapshot(
    value: object,
    snapshotName: string,
    because: string,
    fix: string
)

// Property-based testing integration
Assert.That.HoldsForAll<T>(
    generator: Arbitrary<T>,
    property: Func<T, bool>,
    propertyDescription: string,
    because: string,
    fix: string
)
```

---

## 📊 **TOTALS**

**Complete Library: 300+ Assert Methods!**

### Categories Summary:
- **Core (from main list)**: 150+
- **HTTP/Web**: 8
- **Security/Validation**: 6
- **Business Rules**: 10
- **Performance/Timing**: 5
- **Database/Data**: 6
- **XML/Markup**: 5
- **Configuration/Environment**: 6
- **Reflection/Metadata**: 9
- **Stream/Binary**: 7
- **Dependency Injection**: 4
- **Semantic/Domain**: 5
- **Graph/Tree**: 4
- **Cryptographic**: 5
- **Event/Message**: 5
- **Concurrency/Thread**: 4
- **Comparison/Math**: 7
- **Color/Media**: 4
- **Versioning**: 4
- **Localization/Culture**: 3
- **Rate Limiting/Quota**: 3
- **Caching**: 3
- **Logging/Telemetry**: 4
- **API Contract**: 3
- **Pattern Matching**: 3
- **Intelligent Asserts**: 4

---

## 💡 **Usage Philosophy**

1. **Semantic over Generic** - Use domain-specific asserts when possible
2. **Explicit over Implicit** - Clear intent through method names
3. **Helpful over Terse** - Rich error messages guide resolution
4. **Composable over Monolithic** - Build complex assertions from simple ones
5. **AI-Ready over Human-Only** - Structured output for tools and humans

---

## 🎯 **Next Steps**

1. Prioritize most commonly needed assertions
2. Group by namespace/feature area
3. Implement core set (30-40 methods)
4. Add specialized ones as needed
5. Enable community contributions for domain-specific asserts
