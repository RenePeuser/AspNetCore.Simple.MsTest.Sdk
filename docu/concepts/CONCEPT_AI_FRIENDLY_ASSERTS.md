# AI-Friendly Test SDK - Assert Concept

## 🎯 Vision
Create a modern testing SDK where every failed assertion provides **structured, AI-optimized output** that includes:
- **What went wrong** (the problem)
- **Why it's wrong** (the context and expectation)
- **How to fix it** (actionable guidance)

## 📐 Core Principles

### 1. Consistent Output Format
Every assert failure follows the same beautiful, structured format:

```
══════════════════════════════════════════════════════════════
❌ [ASSERTION TYPE] - [SPECIFIC FAILURE]
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : PersonEndpointsTests
Method     : Should_Create_Person
Line       : 88
File       : file:///D:/path/to/test.cs:88

⚠️ Problem
──────────────────────────────────────────────────────────────

[Clear description of what went wrong]

📊 Details
──────────────────────────────────────────────────────────────

[Structured comparison data, differences, context]

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: [Most likely solution]
  - Specific steps or code changes

Option 2: [Alternative solution]
  - Specific steps or code changes

══════════════════════════════════════════════════════════════
```

### 2. Developer-Guided Context
Developers MUST provide context when calling asserts:

```csharp
Assert.That.IsTrue(
    condition: user.IsActive,
    because: "Active users must have verified email addresses",
    fix: "Verify the email before activating the user account"
);

Assert.That.AreEqual(
    expected: 5,
    actual: count,
    because: "We expect exactly 5 premium features for this tier",
    fix: "Check the feature flag configuration for premium tier"
);
```

### 3. AI-Optimized Output
The final phase will generate machine-readable JSON alongside human output for AI analysis:

```json
{
  "assertionType": "equality",
  "severity": "error",
  "test": {
    "project": "MinimalApi.Test",
    "class": "PersonEndpointsTests",
    "method": "Should_Create_Person",
    "line": 88,
    "file": "file:///D:/path/to/test.cs:88"
  },
  "problem": {
    "summary": "Expected value does not match actual value",
    "category": "value_mismatch",
    "expected": "5",
    "actual": "3",
    "difference": "-2"
  },
  "context": {
    "because": "We expect exactly 5 premium features for this tier",
    "fix": "Check the feature flag configuration for premium tier"
  },
  "suggestions": [
    {
      "priority": 1,
      "action": "Check the feature flag configuration",
      "rationale": "Feature count is controlled by feature flags"
    }
  ]
}
```

## 🔧 Assert Categories

### 1. Basic Assertions

#### 1.1 Equality Assertions
```csharp
// Primitive equality
Assert.That.AreEqual<T>(
    expected: T,
    actual: T,
    because: string,    // Why these should be equal
    fix: string         // How to make them equal
);

// Object equality (existing ObjectsAreEqual - enhance)
Assert.That.ObjectsAreEqual<T>(
    expected: T,
    actual: T,
    because: string,    // Why the objects should match
    fix: string         // How to resolve differences
);

// Collection equality
Assert.That.CollectionsAreEqual<T>(
    expected: IEnumerable<T>,
    actual: IEnumerable<T>,
    because: string,
    fix: string,
    ignoreOrder: bool = false
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ VALUE MISMATCH - INTEGER COMPARISON
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : FeatureTests
Method     : Should_Have_Correct_Feature_Count
Line       : 42

⚠️ Problem
──────────────────────────────────────────────────────────────

Expected and actual values do not match.

📊 Details
──────────────────────────────────────────────────────────────

Expected   : 5
Actual     : 3
Difference : -2 (40% less than expected)
Type       : System.Int32

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

We expect exactly 5 premium features for this tier according
to the product specification.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Check feature flag configuration
  - Navigate to: appsettings.json > FeatureFlags > PremiumTier
  - Verify all 5 features are enabled: [AdvancedAnalytics, 
    ExportData, Priority Support, CustomBranding, ApiAccess]

Option 2: Check the feature activation logic
  - Review: FeatureService.GetActiveFeaturesForTier(TierType.Premium)
  - Ensure no features are filtered by user state

══════════════════════════════════════════════════════════════
```

#### 1.2 Boolean Assertions
```csharp
Assert.That.IsTrue(
    condition: bool,
    because: string,
    fix: string
);

Assert.That.IsFalse(
    condition: bool,
    because: string,
    fix: string
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ CONDITION FAILED - EXPECTED TRUE
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : UserApi.Test
Class      : UserValidationTests
Method     : Should_Validate_Active_User
Line       : 67

⚠️ Problem
──────────────────────────────────────────────────────────────

Expected condition to be TRUE but it was FALSE.

📊 Details
──────────────────────────────────────────────────────────────

Condition  : user.IsActive
Result     : False
Expected   : True

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

Active users must have verified email addresses before they can
access premium features.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Verify the email before activation
  - Call: user.VerifyEmail(verificationToken)
  - Then: user.Activate()

Option 2: Check user creation flow
  - Ensure verification email is sent during registration
  - Verify the VerifyEmail method is called before Activate

══════════════════════════════════════════════════════════════
```

#### 1.3 Null Assertions
```csharp
Assert.That.IsNull(
    obj: object?,
    because: string,
    fix: string
);

Assert.That.IsNotNull(
    obj: object?,
    because: string,
    fix: string
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ NULL REFERENCE - EXPECTED NON-NULL
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : PersonServiceTests
Method     : Should_Return_Person_By_Id
Line       : 35

⚠️ Problem
──────────────────────────────────────────────────────────────

Expected object to be non-null but received null.

📊 Details
──────────────────────────────────────────────────────────────

Variable   : personResult
Type       : Person
Value      : null
Expected   : Non-null Person object

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

The GetById method should return a person object for existing IDs.
Null indicates the person was not found in the database.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Verify test data setup
  - Ensure the person with Id=1 exists in the test database
  - Check: Database seeding in TestBase.InitializeAsync()

Option 2: Check the repository query
  - Review: PersonRepository.GetByIdAsync(id)
  - Verify the WHERE clause is correct
  - Check database connection string in test configuration

══════════════════════════════════════════════════════════════
```

### 2. String Assertions

```csharp
Assert.That.StringContains(
    text: string,
    substring: string,
    because: string,
    fix: string,
    ignoreCase: bool = false
);

Assert.That.StringStartsWith(
    text: string,
    prefix: string,
    because: string,
    fix: string,
    ignoreCase: bool = false
);

Assert.That.StringEndsWith(
    text: string,
    suffix: string,
    because: string,
    fix: string,
    ignoreCase: bool = false
);

Assert.That.StringMatches(
    text: string,
    pattern: string,    // Regex pattern
    because: string,
    fix: string
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ STRING MISMATCH - MISSING SUBSTRING
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : EmailService.Test
Class      : WelcomeEmailTests
Method     : Should_Include_Activation_Link
Line       : 28

⚠️ Problem
──────────────────────────────────────────────────────────────

Expected substring not found in the text.

📊 Details
──────────────────────────────────────────────────────────────

Text Length   : 456 characters
Expected      : "https://app.example.com/activate"
Found         : No
Case Sensitive: Yes

Text Preview  :
  "Welcome to our platform! Please click the link below..."
  [... 400 more characters ...]

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

Welcome emails must include an activation link so users can
verify their email and access the platform.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Check email template
  - File: Templates/WelcomeEmail.html
  - Ensure {{ActivationUrl}} placeholder is present
  - Verify the template is not using an outdated version

Option 2: Verify link generation
  - Review: ActivationLinkGenerator.Generate(userId, email)
  - Check base URL configuration in EmailSettings
  - Ensure HTTPS is used (not HTTP)

══════════════════════════════════════════════════════════════
```

### 3. Collection Assertions

```csharp
Assert.That.CollectionHasCount<T>(
    collection: IEnumerable<T>,
    expectedCount: int,
    because: string,
    fix: string
);

Assert.That.CollectionContains<T>(
    collection: IEnumerable<T>,
    item: T,
    because: string,
    fix: string,
    comparer: IEqualityComparer<T>? = null
);

Assert.That.CollectionIsEmpty<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
);

Assert.That.CollectionIsNotEmpty<T>(
    collection: IEnumerable<T>,
    because: string,
    fix: string
);

Assert.That.CollectionAll<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,    // Human-readable description
    because: string,
    fix: string
);

Assert.That.CollectionAny<T>(
    collection: IEnumerable<T>,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ COLLECTION COUNT MISMATCH
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : OrderService.Test
Class      : OrderProcessingTests
Method     : Should_Process_All_Line_Items
Line       : 91

⚠️ Problem
──────────────────────────────────────────────────────────────

Collection count does not match expected value.

📊 Details
──────────────────────────────────────────────────────────────

Expected Count : 5
Actual Count   : 3
Difference     : -2 items (40% less than expected)
Type           : List<OrderLineItem>

Missing Items  :
  [3] OrderLineItem { ProductId: 42, Quantity: 2 }
  [4] OrderLineItem { ProductId: 99, Quantity: 1 }

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

All line items in the order must be processed and included in
the invoice. Missing items indicate incomplete processing.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Check filtering logic
  - Review: OrderProcessor.GetProcessableItems(order)
  - Verify no items are excluded by validation rules
  - Check: Items with discontinued products or zero stock

Option 2: Verify order loading
  - Ensure: OrderRepository.GetByIdAsync includes all line items
  - Check: Entity Framework navigation property configuration
  - Review: .Include(o => o.LineItems) in the query

══════════════════════════════════════════════════════════════
```

### 4. Numeric Assertions

```csharp
Assert.That.IsInRange<T>(
    value: T,
    min: T,
    max: T,
    because: string,
    fix: string
) where T : IComparable<T>;

Assert.That.IsGreaterThan<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>;

Assert.That.IsLessThan<T>(
    value: T,
    threshold: T,
    because: string,
    fix: string
) where T : IComparable<T>;

Assert.That.IsCloseTo(
    actual: double,
    expected: double,
    tolerance: double,
    because: string,
    fix: string
);
```

### 5. Type Assertions

```csharp
Assert.That.IsOfType<TExpected>(
    obj: object,
    because: string,
    fix: string
);

Assert.That.IsAssignableTo<TExpected>(
    obj: object,
    because: string,
    fix: string
);

Assert.That.IsNotOfType<TNotExpected>(
    obj: object,
    because: string,
    fix: string
);
```

### 6. Exception Assertions

```csharp
Assert.That.Throws<TException>(
    action: Action,
    because: string,
    fix: string,
    expectedMessage: string? = null
) where TException : Exception;

Assert.That.ThrowsAsync<TException>(
    action: Func<Task>,
    because: string,
    fix: string,
    expectedMessage: string? = null
) where TException : Exception;

Assert.That.DoesNotThrow(
    action: Action,
    because: string,
    fix: string
);
```

**Output Example:**
```
══════════════════════════════════════════════════════════════
❌ EXCEPTION MISMATCH - EXPECTED EXCEPTION NOT THROWN
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : ValidationService.Test
Class      : InputValidationTests
Method     : Should_Throw_On_Invalid_Email
Line       : 55

⚠️ Problem
──────────────────────────────────────────────────────────────

Expected an exception to be thrown but none was thrown.

📊 Details
──────────────────────────────────────────────────────────────

Expected Exception : ValidationException
Actual Result      : No exception
Method Called      : ValidateEmail("invalid-email")

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

Invalid email formats must throw ValidationException to prevent
bad data from entering the system.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Add validation logic
  - File: EmailValidator.cs
  - Method: ValidateEmail(string email)
  - Add: if (!IsValidFormat(email)) throw new ValidationException(...)

Option 2: Check validation is enabled
  - Review: ValidationService configuration
  - Ensure: ValidationEnabled = true in settings
  - Check: Email validation rules are registered

══════════════════════════════════════════════════════════════
```

### 7. HTTP Assertions (Enhanced Existing)

These already exist but should be enhanced with the new pattern:

```csharp
Client.AssertPostAsync<T>(
    url: string,
    payload: object,
    expectedResponse: string,
    because: string,          // NEW
    fix: string,              // NEW
    expectedStatusCode: HttpStatusCode = HttpStatusCode.Created
);

Client.AssertGetAsync<T>(
    url: string,
    expectedResponse: string,
    because: string,          // NEW
    fix: string,              // NEW
    expectedStatusCode: HttpStatusCode = HttpStatusCode.OK
);
```

### 8. Custom/Generic Assertions

```csharp
Assert.That.Satisfies<T>(
    value: T,
    predicate: Func<T, bool>,
    predicateDescription: string,
    because: string,
    fix: string
);

Assert.That.Fail(
    message: string,
    because: string,          // Enhanced existing
    fix: string               // Enhanced existing
);
```

## 🏗️ Implementation Strategy

### Phase 1: Foundation (Current - Establish Pattern)
- [x] Create standardized output helpers (OutputContext, HttpFailureOutputHelper)
- [x] Implement consistent formatting across HTTP assertions
- [ ] Extract and generalize the output building pattern
- [ ] Create base assert infrastructure with `because` and `fix` parameters

**Deliverable:** Core assert extension methods with consistent beautiful output.

### Phase 2: Complete Basic Asserts (Next)
Implement all basic assertion types:
- [ ] Equality asserts (primitives, objects, collections)
- [ ] Boolean asserts (IsTrue, IsFalse)
- [ ] Null asserts (IsNull, IsNotNull)
- [ ] String asserts (Contains, StartsWith, EndsWith, Matches)
- [ ] Collection asserts (Count, Contains, Empty, All, Any)
- [ ] Numeric asserts (InRange, GreaterThan, LessThan, CloseTo)
- [ ] Type asserts (IsOfType, IsAssignableTo)
- [ ] Exception asserts (Throws, DoesNotThrow)

**Deliverable:** Complete assert library with developer-guided context.

### Phase 3: Enhanced HTTP Asserts
- [ ] Add `because` and `fix` parameters to all HTTP assertion methods
- [ ] Enhance existing output strategies with more context
- [ ] Add smart suggestions based on HTTP status code patterns
- [ ] Implement auto-detection of common issues (auth, validation, etc.)

**Deliverable:** HTTP asserts with rich contextual guidance.

### Phase 4: AI-Optimized Output
- [ ] Design machine-readable JSON schema for assertions
- [ ] Implement dual output (human + machine readable)
- [ ] Add structured metadata for AI parsing
- [ ] Include git context, test history, and code references
- [ ] Generate suggested fixes based on patterns

**Deliverable:** AI-friendly test output with structured data.

## 📋 Output Format Specification

### Required Sections (All Asserts)

#### 1. Header
```
══════════════════════════════════════════════════════════════
❌ [ASSERTION CATEGORY] - [SPECIFIC FAILURE]
══════════════════════════════════════════════════════════════
```

Categories: VALUE MISMATCH, CONDITION FAILED, NULL REFERENCE, STRING MISMATCH, 
COLLECTION MISMATCH, TYPE MISMATCH, EXCEPTION MISMATCH, HTTP FAILURE, etc.

#### 2. Test Information (Always Present)
```
📦 Test Information
──────────────────────────────────────────────────────────────

Project    : [Project Name]
Class      : [Fully Qualified Class Name]
Method     : [Test Method Name]
Line       : [Line Number]
File       : file:///[Full Path]:[Line] (clickable URI)
```

#### 3. Problem (Always Present)
```
⚠️ Problem
──────────────────────────────────────────────────────────────

[Clear, single-sentence description of what went wrong]
```

#### 4. Details (Always Present)
```
📊 Details
──────────────────────────────────────────────────────────────

[Structured data specific to the assertion type]
Expected   : [value]
Actual     : [value]
Difference : [calculated difference if applicable]
...
```

#### 5. Context (NEW - Developer Provided)
```
💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

[Developer's "because" explanation - why this assertion exists
and what it's protecting against]
```

#### 6. Suggested Fix (NEW - Developer + Auto Generated)
```
✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: [Primary suggestion from developer's "fix" parameter]
  - [Specific actionable steps]
  - [File/method references]
  
Option 2: [SDK-generated alternative based on failure pattern]
  - [Smart suggestions based on assertion type]
  - [Common patterns and fixes]

[Optional] See Also:
  - [Related documentation]
  - [Stack Overflow links for common issues]
```

#### 7. Footer
```
══════════════════════════════════════════════════════════════
```

### Optional Sections (Context Dependent)

#### Difference Table (for object/collection comparisons)
```
📋 Differences Found
──────────────────────────────────────────────────────────────

| Path            | Expected | Actual | Change |
|-----------------|----------|--------|--------|
| Name            | "Goku"   | "Soku" | -1 G   |
| Age             | 99       | 42     | -57    |
| Emails[0].Type  | "GMX"    | null   | Missing|
```

#### HTTP Section (for HTTP assertions)
```
🌍 HTTP Request/Response
──────────────────────────────────────────────────────────────

Method     : POST
URL        : http://localhost/api/v1/persons
Status     : 400 (Bad Request)
Content    : application/json
```

#### curl Reproduction (for HTTP assertions)
```
🔧 Reproduce with curl
──────────────────────────────────────────────────────────────

curl -X POST http://localhost/api/v1/persons \
  -H "Content-Type: application/json" \
  -d '{"name":"Goku","age":99}'
```

## 🎨 Visual Guidelines

### Icons
- ❌ Header (failure indicator)
- 📦 Test Information
- ⚠️ Problem
- 📊 Details
- 💭 Context (Why)
- ✅ Suggested Fix
- 🌍 HTTP Information
- 🔧 Reproducibility (curl, etc.)
- 📋 Differences/Tables
- 💡 Tips/Notes
- 🚫 Blocking Issues

### Colors (ANSI for Terminal)
- Red: Error indicators, header
- Yellow: Warnings, missing data
- Green: Suggested fixes, success indicators
- Cyan: Section titles
- Gray/Dim: Separators, less important info
- White: Primary content

## 🔮 Future Enhancements

### AI Integration Features
- **Pattern Recognition**: Detect common failure patterns and suggest fixes automatically
- **Historical Analysis**: Reference similar test failures in git history
- **Code Context**: Include relevant code snippets from the codebase
- **Documentation Links**: Auto-link to relevant internal/external docs
- **Smart Diffs**: AI-powered explanation of what changed and why it matters
- **Fix Confidence**: Rate fix suggestions by likelihood of success
- **Batch Analysis**: Analyze multiple test failures together for root cause

### Advanced Features
- **Visual Diffs**: For objects, generate visual comparison tables
- **Timeline**: Show test execution timeline for async/timing issues
- **Dependencies**: Show related test failures that might be caused by the same issue
- **Metrics**: Track assertion patterns and common failures over time
- **Custom Formatters**: Allow teams to extend output format
- **Interactive Mode**: Generate fix suggestions interactively in IDE

## 📚 Usage Examples

### Before (Traditional Assert)
```csharp
[TestMethod]
public void Should_Validate_User_Age()
{
    var user = new User { Age = 15 };
    Assert.IsTrue(user.Age >= 18);  // Fails with: "Assert.IsTrue failed."
}
```

### After (AI-Friendly Assert)
```csharp
[TestMethod]
public void Should_Validate_User_Age()
{
    var user = new User { Age = 15 };
    
    Assert.That.IsGreaterThan(
        value: user.Age,
        threshold: 18,
        because: "Users must be 18+ to create an account per legal requirements",
        fix: "Add age validation in the registration form and UserService.CreateAsync method"
    );
}
```

**Output:**
```
══════════════════════════════════════════════════════════════
❌ NUMERIC COMPARISON - VALUE TOO SMALL
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : UserService.Test
Class      : UserValidationTests
Method     : Should_Validate_User_Age
Line       : 42

⚠️ Problem
──────────────────────────────────────────────────────────────

Value is less than the required minimum threshold.

📊 Details
──────────────────────────────────────────────────────────────

Actual Value   : 15
Minimum        : 18
Difference     : -3 (83% of threshold, needs 17% more)
Type           : System.Int32

💭 Context (Why this matters)
──────────────────────────────────────────────────────────────

Users must be 18+ to create an account per legal requirements.
This protects against COPPA violations and ensures compliance.

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Add age validation in registration
  - File: UserService.cs
  - Method: CreateAsync(User user)
  - Add: if (user.Age < 18) throw new ValidationException("Must be 18+")

Option 2: Add client-side validation
  - File: RegistrationForm.tsx
  - Add age check before form submission
  - Display clear error message to user

Option 3: Update test data
  - If this test should use a valid user, change Age to 18 or higher
  - File: UserTestData.cs

══════════════════════════════════════════════════════════════
```

## 🎯 Success Criteria

A successful AI-friendly assert should:

1. ✅ **Be immediately understandable** - Any developer can read the output and know exactly what failed
2. ✅ **Provide actionable guidance** - Clear steps to fix the issue
3. ✅ **Include context** - Why the assertion exists and what it protects
4. ✅ **Be consistent** - Same format across all assertion types
5. ✅ **Be beautiful** - Well-formatted, easy to scan, professional
6. ✅ **Be AI-parseable** - Structured output that AI tools can consume
7. ✅ **Be locatable** - Include file URIs that IDEs can click
8. ✅ **Be reproducible** - Include curl commands or code to reproduce
9. ✅ **Be contextual** - Adapt output based on failure type
10. ✅ **Be helpful** - Multiple fix suggestions with rationale

---

## 📝 Notes

- Keep output **concise but complete** - no unnecessary verbosity
- Use **consistent terminology** across all assertions
- Make **file paths clickable** (file:/// URIs)
- Include **smart defaults** for common scenarios
- Allow **customization** without losing consistency
- Support **both debug and release** modes (plain text vs ANSI colors)
- Follow **existing patterns** in the codebase (ITextDecorator, OutputContext, etc.)
