# Assert Extension Folder Structure & Organization

## 📁 Recommended Folder Structure

```
src/AspNetCore.Simple.MsTest.Sdk/
└── AssertExtensions/
    ├── Assert.That.cs                              # Main partial class definition
    │
    ├── Core/                                        # Basic assertions
    │   ├── Assert.That.Null.cs                     # IsNull, IsNotNull
    │   ├── Assert.That.Boolean.cs                  # IsTrue, IsFalse, AllTrue, AnyTrue
    │   ├── Assert.That.Equality.cs                 # AreEqual, AreNotEqual, AreSame
    │   ├── Assert.That.Type.cs                     # IsOfType, IsAssignableTo, Implements
    │   └── Assert.That.Fail.cs                     # ✅ EXISTING - Fail methods
    │
    ├── Numeric/                                     # Numeric comparisons
    │   ├── Assert.That.Comparison.cs               # IsGreaterThan, IsLessThan, etc.
    │   ├── Assert.That.Range.cs                    # IsInRange, IsOutOfRange
    │   ├── Assert.That.Tolerance.cs                # IsCloseTo for double/decimal/float
    │   ├── Assert.That.Sign.cs                     # IsPositive, IsNegative, IsZero
    │   └── Assert.That.Math.cs                     # IsEven, IsOdd, IsPrime, IsDivisibleBy
    │
    ├── String/                                      # String assertions
    │   ├── Assert.That.String.Null.cs              # IsNullOrEmpty, IsNotNullOrWhiteSpace
    │   ├── Assert.That.String.Content.cs           # Contains, StartsWith, EndsWith
    │   ├── Assert.That.String.Pattern.cs           # Matches, DoesNotMatch (regex)
    │   ├── Assert.That.String.Length.cs            # HasLength, HasLengthInRange
    │   └── Assert.That.String.Case.cs              # IsUpperCase, IsLowerCase
    │
    ├── Collection/                                  # Collection assertions
    │   ├── Assert.That.Collection.Null.cs          # IsNull, IsEmpty, IsNullOrEmpty
    │   ├── Assert.That.Collection.Count.cs         # HasCount, HasCountInRange
    │   ├── Assert.That.Collection.Contains.cs      # Contains, ContainsAll, ContainsAny
    │   ├── Assert.That.Collection.Predicate.cs     # All, Any, None, Single, Exactly
    │   ├── Assert.That.Collection.Unique.cs        # AllUnique, HasDuplicates
    │   ├── Assert.That.Collection.Ordering.cs      # IsOrdered, IsOrderedDescending
    │   └── Assert.That.Collection.Equality.cs      # AreEqual, AreEquivalent
    │
    ├── Object/                                      # Object/complex type assertions
    │   ├── Assert.That.ObjectsAreEqual.cs          # ✅ EXISTING - Enhanced with because/fix
    │   └── Assert.That.ObjectsAreEqual.FromFile.cs # ✅ EXISTING
    │
    ├── Exception/                                   # Exception assertions
    │   ├── Assert.That.Throws.cs                   # Throws<T>, ThrowsWithMessage
    │   ├── Assert.That.ThrowsAsync.cs              # ThrowsAsync<T>
    │   └── Assert.That.DoesNotThrow.cs             # DoesNotThrow, DoesNotThrowAsync
    │
    ├── DateTime/                                    # Date/Time assertions
    │   ├── Assert.That.DateTime.cs                 # IsAfter, IsBefore, IsInRange
    │   ├── Assert.That.DateTime.Relative.cs        # IsToday, IsInPast, IsInFuture
    │   ├── Assert.That.DateTime.Tolerance.cs       # IsCloseTo (with TimeSpan)
    │   └── Assert.That.TimeSpan.cs                 # IsLongerThan, IsShorterThan
    │
    ├── Guid/                                        # GUID assertions
    │   └── Assert.That.Guid.cs                     # IsEmpty, IsNotEmpty, AreEqual
    │
    ├── Http/                                        # HTTP-specific assertions (enhance existing)
    │   ├── Client.Assert.Get.cs                    # ✅ EXISTING - Enhance
    │   ├── Client.Assert.Post.cs                   # ✅ EXISTING - Enhance
    │   ├── Client.Assert.Put.cs                    # ✅ EXISTING - Enhance
    │   ├── Client.Assert.Delete.cs                 # ✅ EXISTING - Enhance
    │   ├── Client.Assert.Patch.cs                  # ✅ EXISTING - Enhance
    │   ├── Assert.That.Http.Url.cs                 # NEW - IsValidUrl, IsAbsoluteUrl
    │   ├── Assert.That.Http.Email.cs               # NEW - IsValidEmail
    │   └── Assert.That.Http.StatusCode.cs          # NEW - IsSuccessStatusCode
    │
    ├── FileSystem/                                  # File/Path assertions
    │   ├── Assert.That.File.cs                     # FileExists, FileDoesNotExist
    │   ├── Assert.That.Directory.cs                # DirectoryExists
    │   ├── Assert.That.Path.cs                     # IsAbsolutePath, IsRelativePath
    │   └── Assert.That.FileSystem.Extended.cs      # HasExtension, FileSizeInRange
    │
    ├── Json/                                        # JSON assertions
    │   ├── Assert.That.Json.cs                     # IsValidJson, JsonContainsPath
    │   └── Assert.That.Json.Schema.cs              # MatchesJsonSchema, JsonPathEquals
    │
    ├── Dictionary/                                  # Dictionary/Key-Value assertions
    │   └── Assert.That.Dictionary.cs               # ContainsKey, KeyHasValue, HasCount
    │
    ├── Async/                                       # Async/Task assertions
    │   ├── Assert.That.Task.cs                     # IsCompleted, IsFaulted, IsCanceled
    │   └── Assert.That.Task.Timing.cs              # CompletesWithin
    │
    ├── Custom/                                      # Custom/Predicate assertions
    │   └── Assert.That.Satisfies.cs                # Satisfies, DoesNotSatisfy, Custom
    │
    ├── Advanced/                                    # Advanced/Specialized (optional)
    │   ├── Security/
    │   │   ├── Assert.That.Security.Password.cs    # IsStrongPassword
    │   │   ├── Assert.That.Security.Sanitize.cs    # IsSanitized, IsXssSafe
    │   │   └── Assert.That.Security.Crypto.cs      # IsValidJwt, IsBase64, HashMatches
    │   │
    │   ├── Business/
    │   │   ├── Assert.That.Business.Time.cs        # IsWithinBusinessHours, IsBusinessDay
    │   │   ├── Assert.That.Business.Age.cs         # MeetsAgeRequirement
    │   │   ├── Assert.That.Business.Money.cs       # IsWithinBudget, IsValidDiscountPercentage
    │   │   └── Assert.That.Business.Validation.cs  # IsValidSku, IsValidPhoneNumber
    │   │
    │   ├── Performance/
    │   │   ├── Assert.That.Performance.Timing.cs   # CompletesQuickly, TakesAtLeast
    │   │   └── Assert.That.Performance.Memory.cs   # AllocatesLessThan, MeetsThroughput
    │   │
    │   ├── Database/
    │   │   └── Assert.That.Database.cs             # TableExists, ColumnExists, HasRowCount
    │   │
    │   ├── Xml/
    │   │   ├── Assert.That.Xml.cs                  # IsValidXml, XmlContainsPath
    │   │   └── Assert.That.Html.cs                 # IsValidHtml, HtmlContainsSelector
    │   │
    │   ├── Configuration/
    │   │   └── Assert.That.Configuration.cs        # EnvironmentVariableExists, ConfigKeyExists
    │   │
    │   ├── Reflection/
    │   │   └── Assert.That.Reflection.cs           # HasAttribute, HasProperty, HasMethod
    │   │
    │   ├── Stream/
    │   │   └── Assert.That.Stream.cs               # IsReadable, IsWritable, HasData
    │   │
    │   ├── DependencyInjection/
    │   │   └── Assert.That.DI.cs                   # ServiceIsRegistered, ServiceCanBeResolved
    │   │
    │   ├── Graph/
    │   │   └── Assert.That.Graph.cs                # HasDepth, HasCycle, IsConnected
    │   │
    │   ├── Events/
    │   │   └── Assert.That.Events.cs               # EventWasRaised, MessageWasPublished
    │   │
    │   ├── Concurrency/
    │   │   └── Assert.That.Concurrency.cs          # IsThreadSafe, LockIsHeld
    │   │
    │   ├── Media/
    │   │   └── Assert.That.Media.cs                # IsValidHexColor, HasDimensions
    │   │
    │   ├── Versioning/
    │   │   └── Assert.That.Version.cs              # IsValidSemVer, VersionIsGreaterThan
    │   │
    │   ├── Localization/
    │   │   └── Assert.That.Localization.cs         # CultureIsSupported, ResourceKeyExists
    │   │
    │   ├── RateLimiting/
    │   │   └── Assert.That.RateLimit.cs            # RateLimitNotExceeded, QuotaIsAvailable
    │   │
    │   ├── Caching/
    │   │   └── Assert.That.Cache.cs                # CacheContainsKey, CacheHitRateIsAcceptable
    │   │
    │   ├── Telemetry/
    │   │   └── Assert.That.Telemetry.cs            # LogContains, MetricWasRecorded
    │   │
    │   └── ApiContract/
    │       └── Assert.That.ApiContract.cs          # MatchesOpenApiSchema, ApiVersionIsSupported
    │
    └── Context/                                     # Context objects (existing)
        ├── HttpAssertContext.cs                    # ✅ EXISTING
        ├── ObjectAssertContext.cs                  # ✅ EXISTING
        └── IHttpResponseContext.cs                 # ✅ EXISTING
```

---

## 🎯 Partial Class Pattern

### Main Definition File: `Assert.That.cs`
```csharp
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Provides modern, AI-friendly assertion methods with rich context and fix guidance.
    /// All assertions require 'because' (why) and 'fix' (how to resolve) parameters.
    /// </summary>
    public static partial class AssertThat
    {
        // This file contains no implementations - only the partial class definition
        // All methods are implemented in category-specific files
        
        // Example of shared helpers (if needed)
        private static void ThrowAssertionFailure(
            string message,
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            throw new AssertFailedException(message);
        }
    }
}
```

### Category File Pattern: `Assert.That.{Category}.cs`

#### Example: `Core/Assert.That.Null.cs`
```csharp
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    // NULL CHECKS
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the value is null.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be null (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        public static void IsNull<T>(
            this Assert _,
            T? value,
            string because,
            string fix,
            [CallerArgumentExpression(nameof(value))] string valueName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = 0
        ) where T : class
        {
            if (value is null)
                return;

            var output = BuildNullAssertionOutput(
                isNull: true,
                valueName: valueName,
                actualValue: value,
                because: because,
                fix: fix,
                callerFilePath: callerFilePath,
                callerMemberName: callerMemberName,
                callerLineNumber: callerLineNumber
            );

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is NOT null.
        /// </summary>
        public static void IsNotNull<T>(
            this Assert _,
            T? value,
            string because,
            string fix,
            [CallerArgumentExpression(nameof(value))] string valueName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = 0
        ) where T : class
        {
            if (value is not null)
                return;

            var output = BuildNullAssertionOutput(
                isNull: false,
                valueName: valueName,
                actualValue: null,
                because: because,
                fix: fix,
                callerFilePath: callerFilePath,
                callerMemberName: callerMemberName,
                callerLineNumber: callerLineNumber
            );

            throw new AssertFailedException(output);
        }

        // Helper method (private to this partial)
        private static string BuildNullAssertionOutput<T>(
            bool isNull,
            string valueName,
            T? actualValue,
            string because,
            string fix,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber
        )
        {
            // Use existing output infrastructure
            var builder = new StringBuilder();
            
            builder.AppendLine("══════════════════════════════════════════════════════════════");
            builder.AppendLine(isNull 
                ? "❌ NULL REFERENCE - EXPECTED NULL" 
                : "❌ NULL REFERENCE - EXPECTED NON-NULL");
            builder.AppendLine("══════════════════════════════════════════════════════════════");
            builder.AppendLine();
            
            // Test Information section...
            // Problem section...
            // Context section...
            // Fix section...
            
            return builder.ToString();
        }
    }
}
```

---

## 📊 Naming Conventions

### Files
- **Pattern**: `Assert.That.{Category}.{SubCategory?}.cs`
- **Examples**:
  - `Assert.That.Null.cs`
  - `Assert.That.String.Content.cs`
  - `Assert.That.Collection.Predicate.cs`
  - `Assert.That.Security.Password.cs`

### Methods
- **Pattern**: `{ActionVerb}{Condition}`
- **Examples**:
  - `IsNull`, `IsNotNull`
  - `IsGreaterThan`, `IsLessThan`
  - `Contains`, `DoesNotContain`
  - `StartsWith`, `EndsWith`
  - `HasCount`, `HasLength`
  - `Throws`, `DoesNotThrow`

### Parameters (Always Include)
1. `this Assert _` - Extension point
2. Main value(s) to assert
3. `string because` - Context (why)
4. `string fix` - Resolution guidance (how)
5. `[CallerArgumentExpression]` for variable name
6. `[CallerFilePath]` for file path
7. `[CallerMemberName]` for method name
8. `[CallerLineNumber]` for line number

---

## 🎨 Implementation Phases

### Phase 1: Core Foundation (Priority)
```
✅ AssertExtensions/
   ├── Assert.That.cs (main definition)
   ├── Core/
   │   ├── Assert.That.Null.cs
   │   ├── Assert.That.Boolean.cs
   │   ├── Assert.That.Equality.cs
   │   └── Assert.That.Fail.cs (enhance existing)
   ├── Numeric/
   │   ├── Assert.That.Comparison.cs
   │   └── Assert.That.Range.cs
   ├── String/
   │   ├── Assert.That.String.Null.cs
   │   └── Assert.That.String.Content.cs
   └── Collection/
       ├── Assert.That.Collection.Null.cs
       ├── Assert.That.Collection.Count.cs
       └── Assert.That.Collection.Contains.cs
```

### Phase 2: Extended Core
```
✅ Continue with:
   - Exception/
   - DateTime/
   - Guid/
   - Object/ (enhance existing)
   - Custom/
```

### Phase 3: HTTP & Web
```
✅ Http/ folder
   - Enhance existing Client.Assert.* files
   - Add new Assert.That.Http.* files
```

### Phase 4: Advanced/Specialized
```
✅ Advanced/ folder
   - Add domain-specific categories as needed
   - Start with most commonly used (Security, Performance)
```

---

## 🔧 Usage in Code

### Import
```csharp
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
```

### Usage
```csharp
[TestMethod]
public void Should_Validate_User_Age()
{
    var user = GetUser();
    
    // Core null check
    Assert.That.IsNotNull(
        user,
        because: "User must exist in the database after registration",
        fix: "Check the user registration flow and database seeding"
    );
    
    // Numeric comparison
    Assert.That.IsGreaterThan(
        user.Age,
        threshold: 18,
        because: "Users must be adults to create an account (legal requirement)",
        fix: "Add age validation in UserService.CreateAsync method"
    );
    
    // String check
    Assert.That.IsNotNullOrWhiteSpace(
        user.Email,
        because: "Email is required for account verification",
        fix: "Ensure email is collected during registration"
    );
    
    // Collection check
    Assert.That.HasCount(
        user.Roles,
        expectedCount: 1,
        because: "New users should have exactly one default role",
        fix: "Check RoleAssignmentService.AssignDefaultRole method"
    );
}
```

---

## 🚀 Migration Strategy

### Step 1: Create Structure
```bash
# Create all folders
mkdir -p src/AspNetCore.Simple.MsTest.Sdk/AssertExtensions/{Core,Numeric,String,Collection,Object,Exception,DateTime,Guid,Http,FileSystem,Json,Dictionary,Async,Custom,Advanced}
```

### Step 2: Create Main Definition
- Create `Assert.That.cs` with partial class

### Step 3: Implement Core (Priority Order)
1. `Core/Assert.That.Null.cs`
2. `Core/Assert.That.Boolean.cs`
3. `Core/Assert.That.Equality.cs`
4. `Numeric/Assert.That.Comparison.cs`
5. `String/Assert.That.String.Null.cs`

### Step 4: Enhance Existing
- Keep existing files in place
- Add `because` and `fix` parameters via overloads
- Maintain backward compatibility

### Step 5: Add New Categories
- Implement based on user needs
- Follow the established pattern

---

## 📝 File Template

```csharp
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// {Category} assertion methods.
    /// Provides: {list of methods}
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// {Method description}
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="{param}">The value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="{param}Name">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        public static void {MethodName}(
            this Assert _,
            // ... parameters
            string because,
            string fix,
            [CallerArgumentExpression(nameof({param}))] string {param}Name = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            // 1. Check condition (return early if passes)
            if (condition)
                return;
            
            // 2. Build beautiful output
            var output = Build{Category}Output(...);
            
            // 3. Throw with formatted message
            throw new AssertFailedException(output);
        }
        
        // Private helper methods for this category
        private static string Build{Category}Output(...)
        {
            // Use existing output infrastructure (ITextDecorator, etc.)
            // Follow established format from CONCEPT document
            return formattedOutput;
        }
    }
}
```

---

## ✅ Benefits of This Structure

1. **Clear Organization** - Easy to find specific assertion types
2. **Partial Classes** - Each file is focused and manageable (~100-200 lines)
3. **Extensible** - Add new categories without touching existing code
4. **Discoverable** - IntelliSense shows `Assert.That.{Method}`
5. **Consistent** - All files follow same pattern
6. **Maintainable** - Related methods grouped together
7. **Testable** - Each category can have dedicated tests
8. **Scalable** - Can grow to 300+ methods without chaos

---

## 📌 Notes

- **Backward Compatibility**: Keep existing `Client.Assert.*` files unchanged initially
- **Gradual Migration**: Add `because`/`fix` via overloads, not breaking changes
- **Shared Helpers**: Put common output builders in a separate `Helpers/` folder if needed
- **Advanced = Optional**: Start with Core, add Advanced only when needed
- **File Size**: Keep files under 500 lines (split into sub-categories if needed)
