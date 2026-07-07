# `AspNetCore.Simple.MsTest.Sdk`

[![NuGet](https://img.shields.io/badge/nuget-AspNetCore.Simple.MsTest.Sdk-blue)](https://www.nuget.org/packages/AspNetCore.Simple.MsTest.Sdk)
[![.NET 10](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![HTTP QUERY](https://img.shields.io/badge/RFC%2010008-HTTP%20QUERY-green)](https://datatracker.ietf.org/doc/html/rfc10008)
[![License](https://img.shields.io/badge/license-Proprietary-red)]()

> **API snapshot testing so productive it feels like cheating.**  
> Add a JSON file. A test appears. When it fails, you get the exact diff, full HTTP context, and a ready-to-run `curl`.
>
> **AI-friendly assertions that let your AI assistant fix your tests.**  
> Every failure includes context (`because`), guidance (`fix`), and structured output. Human-readable. AI-parseable.

---

```csharp
[TestMethod]
[DynamicRequestLocator]
public Task Should_Create_User(string useCase)
{
    return Client.AssertPostAsync<UserResponse>("api/v1/users", 
                                                useCase, 
                                                useCase);
}
```

---

## Quick Start

### Install

```sh
dotnet add package AspNetCore.Simple.MsTest.Sdk
```

### Minimal setup

```csharp
[TestClass]
public abstract class ApiTestBase
{
    private static ApiTestBase<Program> _apiTestBase = null!;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        // Use Program or Startup as entry point for proper WebApplicationFactory support
        // - Program: for minimal API / top-level statements (Program.cs)
        // - Startup: for traditional Startup.cs class
        _apiTestBase = new ApiTestBase<Program>("Development",
                                                (services,
                                                 configuration) =>
                                                {
                                                    // IMPORTANT: Required for endpoint validation and assertable HTTP client features
                                                    services.AddAssertableHttpClient(configuration);
                                                });

        Client = _apiTestBase.CreateClient();

        // IMPORTANT: Required to make all HttpClientAssertExtensions 100% functional
        HHttpClientAssertExtensions.Setup(_apiTestBase.Services);
    }

    protected static HttpClient Client { get; private set; } = null!;

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        _apiTestBase.Dispose();
        Client.Dispose();
    }
}
```

### First test

```csharp
[TestClass]
public class UserTests : ApiTestBase
{
    [TestMethod]
    public Task Should_Create_User()
    {
        return Client.AssertPostAsync<UserResponse>(
            "api/v1/users",
            "CreateUser.json",
            "CreateUser.json");
    }
}
```

### Add the JSON snapshot files

Use embedded JSON files for request and expected response.

`CreateUser.json` request:

```json
{
  "Id": 1,
  "Name": "Son",
  "FirstName": "Goku",
  "Age": 99,
  "Emails": [
    {
      "EmailAddress": "alf@gmx.de",
      "Type": "GMX"
    },
    {
      "EmailAddress": "abc@hotmail.de",
      "Type": "Microsoft"
    }
  ]
}
```

`CreateUser.json` response snapshot:

```json
{
  "Content": {
    "Headers": [
      {
        "Key": "Content-Type",
        "Value": [ "application/json; charset=utf-8" ]
      }
    ],
    "Value": {
      "Id": 1,
      "Name": "Son",
      "FirstName": "Goku",
      "Age": 99,
      "Emails": []
    }
  },
  "StatusCode": "OK",
  "Headers": [],
  "TrailingHeaders": [],
  "IsSuccessStatusCode": true
}
```

### Result

Run the test and you get:

- full-response validation
- structured diffs on mismatch
- HTTP context in the failure output
- generated `curl` for instant reproduction

---

## What you get

- **HTTP QUERY support** ([RFC 10008](https://datatracker.ietf.org/doc/html/rfc10008)) - Complex queries with request body, GET semantics
- Full HTTP response snapshots: status, headers, body, trailing headers
- Precise structured diffs with deep `MemberPath` paths
- Context-specific error headers (Snapshot Mismatch, Schema Mismatch, Status Code, etc.)
- **Clickable file links** in error output - jump directly to failing test line in your IDE
- **Fully qualified class names** - see complete namespace path in test information
- Ready-to-run `curl` output on failures
- Convention-based test discovery with `DynamicRequestLocator`
- Snapshot generation from live traffic
- Snapshot auto-update and ignore strategies
- Drastically less boilerplate than traditional API tests

---

## Supported HTTP Methods & Content Types

Complete feature matrix showing what's supported out of the box:

### HTTP Methods

| Method | With Body | Success Response | Error Response |
|--------|-----------|------------------|----------------|
| **GET** | ❌ | ✅ `AssertGetAsync<T>()` | ✅ `AssertGetAsErrorAsync<T>()` |
| **QUERY** [RFC 10008](https://datatracker.ietf.org/doc/html/rfc10008) | ✅ | ✅ `AssertQueryAsync<T>()` | ✅ `AssertQueryAsErrorAsync<T>()` |
| **POST** | ✅ | ✅ `AssertPostAsync<T>()` | ✅ `AssertPostAsErrorAsync<T>()` |
| **PUT** | ✅ | ✅ `AssertPutAsync<T>()` | ✅ `AssertPutAsErrorAsync<T>()` |
| **PATCH** | ✅ | ✅ `AssertPatchAsync<T>()` | ✅ `AssertPatchAsErrorAsync<T>()` |
| **DELETE** | ❌ | ✅ `AssertDeleteAsync<T>()` | ✅ `AssertDeleteAsErrorAsync<T>()` |
| **OPTIONS** | ❌ | ✅ `AssertOptionsAsync()` | ❌ |

**Note**: POST, PUT, PATCH, DELETE also support NoContent (204) variants without `<T>` generic parameter.

### Content Types

| Content Type | Request | Response | Snapshot Format | Status |
|--------------|---------|----------|-----------------|--------|
| **application/json** | ✅ | ✅ | `.json` files | ✅ Full support |
| **application/xml** | ❌ | ❌ | N/A | ⏳ Planned |
| **multipart/form-data** | ❌ | N/A | N/A | ⏳ Planned |
| **application/x-www-form-urlencoded** | ❌ | N/A | N/A | ⏳ Planned |
| **text/plain** | ✅ | ✅ | `.txt` files | ✅ String comparison |

### Features

| Feature | Support | Notes |
|---------|---------|-------|
| **Request body validation** | ✅ | JSON snapshots |
| **Response body validation** | ✅ | Deep object comparison |
| **Status code validation** | ✅ | Expected vs actual |
| **Header validation** | ✅ | Full HTTP response snapshots |
| **Query parameters** | ✅ | URL parameters + parameter replacement |
| **Dynamic parameters** | ✅ | `$placeholder$` replacement in JSON |
| **File upload** | ❌ | Multipart not yet supported |
| **Binary responses** | ❌ | Text/JSON only |
| **Streaming** | ❌ | Snapshot-based only |
| **WebSockets** | ❌ | HTTP only |

**Legend:**
- ✅ = Fully supported
- ⏳ = Planned for future releases  
- ❌ = Not supported

**Current focus**: JSON-based REST APIs with full snapshot testing support for all standard HTTP methods including the new QUERY method.

### Example: Complete CRUD workflow with QUERY

```csharp
[TestClass]
public class UserApiTests : ApiTestBase
{
    [TestMethod]
    public async Task Complete_User_Lifecycle()
    {
        // CREATE - POST with response
        var created = await Client.AssertPostAsync<User>(
            "api/v1/users",
            "CreateUser.json",
            "CreatedUser.json");
        
        // READ - GET single resource
        await Client.AssertGetAsync<User>(
            $"api/v1/users/{created.Id}",
            "UserDetails.json");
        
        // QUERY - Complex search with body (new RFC 10008 method!)
        await Client.AssertQueryAsync<SearchResults>(
            "api/v1/users/search",
            "SearchRequest.json",
            "SearchResults.json");
        
        // UPDATE - PUT with response
        await Client.AssertPutAsync<User>(
            $"api/v1/users/{created.Id}",
            "UpdateUser.json",
            "UpdatedUser.json");
        
        // PARTIAL UPDATE - PATCH with response
        await Client.AssertPatchAsync<User>(
            $"api/v1/users/{created.Id}",
            "PatchUser.json",
            "PatchedUser.json");
        
        // DELETE - with NoContent (204)
        await Client.AssertDeleteAsync(
            $"api/v1/users/{created.Id}");
    }
}
```

All methods support:
- ✅ Full response snapshots
- ✅ Error scenarios with `AsErrorAsync` variants
- ✅ Dynamic parameter replacement
- ✅ Ignore strategies for dynamic values
- ✅ Endpoint validation against `[ProducesResponseType]`

---

## Why this feels different

Most API testing tools make you choose between speed, coverage, and debuggability.

This SDK does not.

It is built around a simple idea:

- **One snapshot validates the whole HTTP response**, not just the body
- **One failure tells you exactly what changed**, down to `content.value.emails[1].type`
- **One pasted `curl` reproduces the problem immediately**
- **One added JSON file creates a new test case automatically**

That combination changes how API testing feels in practice. Less plumbing. More coverage. Faster debugging.

---

## File conventions and folder structure

### Use file names, not full resource paths

Prefer this:

```csharp
"NewUser.json"
```

Not this:

```csharp
"Users.V1.Payloads.NewUser.json"
```

### Context-aware disambiguation

If multiple files with the same name exist in different folders, the SDK prefers the file in the **same namespace** as your test.

Example structure:

```plaintext
Api/
├─ Persons/
│  └─ Requests/SonGoku.json      ← Test in Persons namespace uses this
├─ Errors/
│  └─ Requests/SonGoku.json
└─ NativeTypes/
   └─ Requests/SonGoku.json
```

When you reference `"Requests.SonGoku.json"` from a test in the `Api.Persons` namespace, the SDK automatically picks `Api.Persons.Requests.SonGoku.json`.

If needed, you can be more specific:

```csharp
"Api.Persons.Requests.SonGoku.json"  // Fully qualified
"Persons.Requests.SonGoku.json"       // Partial namespace
```

The SDK uses **segment-based matching** to avoid false positives. `"Requests.SonGoku.json"` will not match `"ErrorRequests.SonGoku.json"` because the dot boundary matters.

This means you get:

- short, readable file references in tests
- automatic disambiguation by context
- explicit paths when you need them
- predictable resolution behavior

### Recommended structure

```plaintext
Api
└─ Users
   └─ V1
      └─ Create
         └─ Status_200_Ok
            ├─ Requests
            │  ├─ ValidUser.json
            │  ├─ AdminUser.json
            │  └─ GuestUser.json
            ├─ Responses
            │  ├─ ValidUser.json
            │  ├─ AdminUser.json
            │  └─ GuestUser.json
            └─ CreateUser_Status_200_OK_Test.cs
```

### Why this structure works well

- `Requests` contains input payloads
- `Responses` contains expected snapshots
- namespace mirrors folder structure
- `DynamicRequestLocator` can discover request files automatically
- adding scenarios stays simple and predictable

Example:

```csharp
namespace Api.Users.V1.Create.Status_200_Ok;

[TestClass]
public class CreateUser_Status_200_OK_Test : ApiTestBase
{
    [DataTestMethod]
    [DynamicRequestLocator]
    public Task Should_Create_User(string requestFileName)
    {
        return Client.AssertPostAsync<UserResponse>(
            "api/v1/users",
            requestFileName,
            requestFileName);
    }
}
```

---

**Add a JSON file. A new test appears.**

---

## What a failure looks like

The SDK provides **context-specific error outputs** that make debugging fast and intuitive. Each failure type has a dedicated format with actionable information.

### All Failure Types at a Glance

| Icon | Failure Type | When It Occurs | What It Means |
|------|--------------|----------------|---------------|
| 📸 | **SNAPSHOT MISMATCH** | JSON values differ | Business logic produces different values |
| 📋 | **SCHEMA MISMATCH** | Structure differs | API contract changed (breaking change) |
| 🚫 | **UNEXPECTED STATUS CODE** | Wrong HTTP status | Status code doesn't match expectation |
| 📄 | **CONTENT TYPE MISMATCH** | Wrong Content-Type | Response is not JSON (HTML, XML, etc.) |
| ❌ | **ASSERT METHOD MISMATCH** | Wrong assertion type | Using success assert with error status (or vice versa) |
| ❌ | **HTTP RESPONSE TYPE MISMATCH** | Wrong response type | Test type doesn't match endpoint contract |

All errors follow the same structure: Header → Failure Details → Test Info → HTTP Context → Problem Details → Suggested Fix → Curl Command

**Note:** The `File` field in Test Information contains a clickable `file://` URI that works in most IDEs (Rider, VS Code, Visual Studio). Click it to jump directly to the failing test line.

### Snapshot Mismatch (Value Differences)

When JSON values differ from the expected snapshot:

```plaintext
══════════════════════════════════════════════════════════════
📸 SNAPSHOT MISMATCH
══════════════════════════════════════════════════════════════

⚠️ Failure Details
──────────────────────────────────────────────────────────────

JSON values differ from the expected snapshot.
All properties exist but have different values.

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Be_Able_To_Post_A_Person_Object
Line       : 65
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:65

🌍 HTTP
──────────────────────────────────────────────────────────────

Method   : POST
Url      : http://localhost/api/v1/persons
Status   : 201 Created
Body     : {"id":1,"name":"Son","firstName":"Goku","age":42,"emails":[]}
Response : NewPerson.json

🔍 Differences (Count 1)
──────────────────────────────────────────────────────────────

┌────────────────────┬────────────────┬───────────────┬─────────────────┐
│ MemberPath         │ NewPerson.json │ CurrentResult │ MismatchType    │
├────────────────────┼────────────────┼───────────────┼─────────────────┤
│ content.value.name │ Son Test       │ Son           │ ValueDifference │
└────────────────────┴────────────────┴───────────────┴─────────────────┘

📄 Expected Snapshot
──────────────────────────────────────────────────────────────

{"content":{"headers":[...],"value":{"id":1,"name":"Son Test","firstName":"Goku",...}}}

📄 Current Result
──────────────────────────────────────────────────────────────

{"content":{"headers":[...],"value":{"id":1,"name":"Son","firstName":"Goku",...}}}

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl \
--location \
--request POST 'http://localhost/api/v1/persons' \
--header 'Content-Type: application/json' \
--data-raw '{"id":1,"name":"Son","firstName":"Goku","age":42,"emails":[]}'

══════════════════════════════════════════════════════════════
```

### Schema Mismatch (Structural Differences)

When the response structure doesn't match (missing properties, type mismatches):

```plaintext
══════════════════════════════════════════════════════════════
📋 SCHEMA MISMATCH
══════════════════════════════════════════════════════════════

⚠️ Failure Details
──────────────────────────────────────────────────────────────

Structure doesn't match expected type schema.
Properties missing, extra properties, or type mismatches detected.

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Get_Person_By_Id
Line       : 42
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:42

🌍 HTTP
──────────────────────────────────────────────────────────────

Method   : GET
Url      : http://localhost/api/v1/persons/1
Status   : 200 OK

🔍 Differences (Count 2)
──────────────────────────────────────────────────────────────

┌──────────────────────┬──────────────────┬───────────────┬────────────────┐
│ MemberPath           │ Expected         │ Current       │ MismatchType   │
├──────────────────────┼──────────────────┼───────────────┼────────────────┤
│ content.value.emails │ [email array]    │ null          │ MissingInFirst │
│ content.value.age    │ 42               │ null          │ MissingInFirst │
└──────────────────────┴──────────────────┴───────────────┴────────────────┘

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl \
--location \
--request GET 'http://localhost/api/v1/persons/1'

══════════════════════════════════════════════════════════════
```

### Assert Method Mismatch

When using success assertion (`AssertPostAsync`) with error status code:

```plaintext
══════════════════════════════════════════════════════════════
❌ ASSERT METHOD MISMATCH - SUCCESS EXPECTED
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Create_Person
Line       : 88
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:88

🌍 HTTP
──────────────────────────────────────────────────────────────

Method     : POST
Url        : http://localhost/api/v1/persons
Status     : Test Type Mismatch

⚠️ Problem
──────────────────────────────────────────────────────────────

The test is declared as a SUCCESS test (AssertPostAsync, AssertGetAsync, etc.)
but the expected response has status code 500 (InternalServerError) which is an ERROR status.

📊 Details
──────────────────────────────────────────────────────────────

Test Type            : Success (expects 2xx)
Expected Status      : 500 (InternalServerError)
Status Range         : Error (4xx/5xx)

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Use error assertion method instead
  - Use AssertPostAsErrorAsync() or similar error assertion method

Option 2: Update expected response status code
  - Change the expected response to have a success status code (200, 201, etc.)

══════════════════════════════════════════════════════════════
```

### Unexpected Status Code

When the HTTP status code doesn't match expectations:

```plaintext
══════════════════════════════════════════════════════════════
🚫 UNEXPECTED STATUS CODE
══════════════════════════════════════════════════════════════

⚠️ Failure Details
──────────────────────────────────────────────────────────────

Expected   : 200 (Success)
Actual     : 400 (Bad Request)

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Create_Person
Line       : 65
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:65

🌍 HTTP
──────────────────────────────────────────────────────────────

Method   : POST
Url      : http://localhost/api/v1/persons
Status   : 400 Bad Request

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl \
--location \
--request POST 'http://localhost/api/v1/persons' \
--header 'Content-Type: application/json' \
--data-raw '{"name":"Invalid"}'

══════════════════════════════════════════════════════════════
```

### Why This Matters

You immediately see:

1. **Failure type**: Snapshot mismatch vs Schema mismatch vs Assert method issue
2. **Exact location**: `content.value.name` with deep path precision
3. **Expected vs actual**: Side-by-side comparison
4. **HTTP context**: Method, URL, status code, request body
5. **Reproduction command**: Ready-to-run `curl`
6. **Suggested fixes**: Actionable guidance

That is a completely different debugging experience from:

```csharp
Assert.AreEqual("Son", response.Name);  // ❌ No context, no curl, no path
```

This SDK does not just tell you that something failed. It tells you **what kind of failure**, **where**, **what changed**, **under which HTTP call**, and **how to replay it now**.


### Response Type Mismatch

When test's response type doesn't match endpoint contract:

```plaintext
══════════════════════════════════════════════════════════════
❌ HTTP RESPONSE TYPE MISMATCH
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Create_Person
Line       : 65
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:65

🌍 HTTP
──────────────────────────────────────────────────────────────

Method     : POST
Url        : http://localhost/api/v1/persons
Status     : Type Mismatch
Source     : MinimalApi.Api.Persons.V1.CreatePersonEndpoint

🔍 Type Validation
──────────────────────────────────────────────────────────────

┌─────────────┬────────────────────────┬────────────────────┬───────┐
│ Status Code │ Endpoint Response Type │ Declared Test Type │ Match │
├─────────────┼────────────────────────┼────────────────────┼───────┤
│ 201         │ Person                 │ UnknownResponse    │ ✗     │
└─────────────┴────────────────────────┴────────────────────┴───────┘

The test is a success (2xx) test and declares response type 'UnknownResponse',
but none of the endpoint's success (2xx) status codes return this type.

Endpoint defines: 201 → Person

📝 Assert Call
──────────────────────────────────────────────────────────────

return Client.AssertPostAsync<UnknownResponse>("api/v1/persons",
                                               new Person(1, "Son", "Goku",
                                                          42, ImmutableList<Email>.Empty),
                                               "NewPerson.json");

✅ Suggested Fix
──────────────────────────────────────────────────────────────

return Client.AssertPostAsync<Person>("api/v1/persons",
                                      new Person(1, "Son", "Goku",
                                                 42, ImmutableList<Email>.Empty),
                                      "NewPerson.json");

🔁 Reproduce Locally
──────────────────────────────────────────────────────────────

curl \
--location \
--request POST 'http://localhost/api/v1/persons' \
--header 'Content-Type: application/json' \
--data-raw '{"id":1,"name":"Son","firstName":"Goku","age":42,"emails":[]}'

══════════════════════════════════════════════════════════════
```

### Smart endpoint validation with fallback

The SDK validates that your test's response type matches the endpoint's contract using a three-tier strategy.

**Tier 1: `[ProducesResponseType]` attributes**

When your endpoint declares explicit response types:

```csharp
[HttpPost("errors/not-implemented")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public void ThrowNotImplementedException() { ... }
```

The SDK validates your test type against the declared status codes. Success tests (`AssertPostAsync`) are checked against 2xx responses. Error tests (`AssertPostAsErrorAsync`) are checked against 4xx/5xx responses.

**Tier 2: Expected response JSON fallback**

If no `[ProducesResponseType]` attributes exist, the SDK extracts the status code from your **expected response snapshot**:

```json
{
  "StatusCode": "InternalServerError",
  "IsSuccessStatusCode": false,
  "Content": {
    "Value": {
      "title": "Implementation is missing",
      "status": 500
    }
  }
}
```

This enables validation even when developers forget to add attributes. The SDK parses both numeric (`500`) and enum string (`"InternalServerError"`) formats.

**Tier 3: Assert method validation**

The SDK catches when the assertion method doesn't align with the expected status code. This uses the same standardized error format as other failures:

```plaintext
══════════════════════════════════════════════════════════════
❌ ASSERT METHOD MISMATCH - SUCCESS EXPECTED
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────

Project    : MinimalApi.Test
Class      : MinimalApi.Test.Api.Persons.PersonEndpointsTests
Method     : Should_Create_Person
Line       : 88
File       : file:///D:/AzureDevOps/AspNetCore.Simple.MsTest.Sdk/src/MinimalApi.Test/Api/Persons/PersonEndpointsTests.cs:88

🌍 HTTP
──────────────────────────────────────────────────────────────

Method     : POST
Url        : http://localhost/api/v1/persons
Status     : Test Type Mismatch

⚠️ Problem
──────────────────────────────────────────────────────────────

The test is declared as a SUCCESS test (AssertPostAsync, AssertGetAsync, etc.)
but the expected response has status code 500 (InternalServerError) which is an ERROR status.

📊 Details
──────────────────────────────────────────────────────────────

Test Type            : Success (expects 2xx)
Expected Status      : 500 (InternalServerError)
Status Range         : Error (4xx/5xx)

✅ Suggested Fix
──────────────────────────────────────────────────────────────

Option 1: Use error assertion method instead
  - Use AssertPostAsErrorAsync() or similar error assertion method

Option 2: Update expected response status code
  - Change the expected response to have a success status code (200, 201, etc.)

══════════════════════════════════════════════════════════════
```

This catches common mistakes like using `AssertPostAsync` when you meant `AssertPostAsErrorAsync`, or vice versa. The header clearly shows whether the test expected SUCCESS or ERROR.

**Why this matters:**

- catches `ProblemDetails` vs `ValidationProblemDetails` confusion
- works even without explicit attributes
- prevents wrong test type usage
- uses test data that already exists

---

### Endpoint-only validation mode

Sometimes you need to validate that an endpoint exists and returns the correct type, but don't care about the response content. Perfect for process chain tests or when the endpoint is already thoroughly tested elsewhere.

**Simple syntax - no response comparison:**

```csharp
// Validates endpoint exists and returns GetAllNodesResponse
// Skips response content comparison automatically
await Client.AssertGetAsync<GetAllNodesResponse>("api/v1/nodes");
```

**With explicit control:**

```csharp
// Same as above, but explicit
await Client.AssertGetAsync<GetAllNodesResponse>("api/v1/nodes", 
                                                  ignoreResponse: true);

// Full response comparison (default when expectedResult provided)
await Client.AssertGetAsync<GetAllNodesResponse>("api/v1/nodes", 
                                                  "ExpectedNodes.json");
```

**What gets validated:**

- ✅ Endpoint exists and is reachable
- ✅ Response type matches endpoint contract
- ✅ HTTP status code is success (2xx)
- ✅ Request executes without errors
- ⏭️ Response content comparison skipped

**Why this matters:**

In large systems with lots of backend services, you often have:
- **Deep tests** that validate full response snapshots (detailed unit/integration tests)
- **Process tests** that validate multi-step workflows where intermediate calls just need to succeed

This feature lets you write process tests that stay fast and focused:

```csharp
[TestMethod]
public async Task Complete_User_Registration_Flow()
{
    // Step 1: Create user (validate full response)
    var user = await Client.AssertPostAsync<CreateUserResponse>(
        "api/v1/users", 
        "NewUser.json", 
        "NewUser.json");
    
    // Step 2: Send verification email (just validate it succeeds)
    await Client.AssertPostAsync<EmailSentResponse>(
        $"api/v1/users/{user.Id}/send-verification");
    
    // Step 3: Verify email (just validate it succeeds)  
    await Client.AssertPostAsync<VerificationResponse>(
        $"api/v1/users/{user.Id}/verify");
    
    // Step 4: Get final user state (validate full response)
    await Client.AssertGetAsync<GetUserResponse>(
        $"api/v1/users/{user.Id}",
        "VerifiedUser.json");
}
```

---

### Response as C# Objects

Instead of JSON strings, you can use C# objects for both request and response. This gives you compile-time type safety, better IDE support, and eliminates string-based JSON files for simple test cases.

**Traditional JSON-based approach:**

```csharp
[TestMethod]
public Task Should_Create_Person()
{
    return Client.AssertPostAsync<Person>(
        "api/v1/persons",
        "CreatePerson.json",       // Request JSON file
        "ExpectedPerson.json");    // Expected response JSON file
}
```

**New object-based approach:**

```csharp
[TestMethod]
public Task Should_Create_Person()
{
    var personToCreate = new Person(
        Id: 0,
        Name: "Son",
        FirstName: "Goku",
        Age: 99,
        Emails: ImmutableList<Email>.Empty);

    var expectedPerson = new Person(
        Id: 1,
        Name: "Son",
        FirstName: "Goku",
        Age: 99,
        Emails: ImmutableList<Email>.Empty);

    return Client.AssertPostAsync("api/v1/persons", 
                                  personToCreate, 
                                  expectedPerson);
}
```

**Benefits:**

- ✅ **Type safety**: Compiler catches errors before runtime
- ✅ **Refactoring support**: Rename properties with IDE refactoring tools
- ✅ **IntelliSense**: Full autocomplete for object properties
- ✅ **Less boilerplate**: No need to create JSON files for simple cases
- ✅ **Same validation**: Full HTTP response snapshots, structured diffs, curl generation
- ✅ **Flexible**: Mix and match with JSON files as needed

**Supported methods:**

All assert methods support object-based responses:

```csharp
// GET with object response
await Client.AssertGetAsync("api/v1/persons/1", expectedPerson);

// POST with object request and response
await Client.AssertPostAsync("api/v1/persons", requestPerson, expectedPerson);

// PUT with object request and response
await Client.AssertPutAsync("api/v1/persons", requestPerson, expectedPerson);

// PATCH with object request and response
await Client.AssertPatchAsync("api/v1/persons", requestPerson, expectedPerson);

// QUERY with object request and response
await Client.AssertQueryAsync("api/v1/persons/search", searchRequest, expectedResults);

// DELETE with object response
await Client.AssertDeleteAsync<DeleteConfirmation>("api/v1/persons/1", expectedConfirmation);
```

**Error scenarios with objects:**

```csharp
[TestMethod]
public Task Should_Return_NotFound_Error()
{
    var expectedError = new
    {
        Title = "Person not found",
        Status = 404,
        Detail = "The person with the Id: 999 does not exist",
        Id = 999
    };

    return Client.AssertGetAsErrorAsync("api/v1/persons/999",
                                        expectedError,
                                        skipEndpointValidation: true,
                                        expectedHttpStatusCode: HttpStatusCode.NotFound);
}
```

**Ignoring dynamic fields:**

Use `differenceFunc` to ignore generated IDs or timestamps:

```csharp
[TestMethod]
public Task Should_Create_Person_Ignore_Id()
{
    var personToCreate = TestHelpers.CreateValidPerson();

    var expectedPerson = new Person(
        Id: 0,  // Will be ignored
        Name: personToCreate.Name,
        FirstName: personToCreate.FirstName,
        Age: personToCreate.Age,
        Emails: personToCreate.Emails);

    return Client.AssertPostAsync("api/v1/persons",
                                  personToCreate,
                                  expectedPerson,
                                  differenceFunc: diffs => 
                                      diffs.Where(d => !d.MemberPath.Contains("id")));
}
```

**When to use objects vs JSON files:**

| Scenario | Use |
|----------|-----|
| Simple, stable test data | **C# objects** - Type-safe, less overhead |
| Complex nested structures | **JSON files** - Easier to read and maintain |
| Dynamic test data generation | **C# objects** - Programmatic control |
| Snapshot-driven workflows | **JSON files** - File-based test discovery |
| Shared test data across tests | **JSON files** - Reusable snapshots |
| Type-checked domain models | **C# objects** - Compile-time safety |

**Mixing approaches:**

You can mix objects and JSON files based on your needs:

```csharp
// Request as object, expected response from JSON file
await Client.AssertPostAsync<Person>(
    "api/v1/persons",
    personToCreate,
    "ExpectedPerson.json");

// Request from JSON file, expected response as object
await Client.AssertPostAsync(
    "api/v1/persons",
    "CreatePerson.json",
    expectedPerson);
```

The SDK automatically serializes objects to JSON and performs the same deep comparison, structured diff, and HTTP context output as with JSON files.

---

### Skip endpoint validation

Sometimes you need to test external APIs or use different response types than what the endpoint declares. In these cases, endpoint validation becomes a blocker rather than a helper.

**When to skip endpoint validation:**

- Testing external APIs where endpoint metadata is not available
- Using a different response type than defined in the endpoint contract
- Working with OpenAPI specs not yet integrated into your codebase
- Testing legacy endpoints without proper `[ProducesResponseType]` attributes

**How to use it:**

```csharp
// Test external API without endpoint validation
await Client.AssertGetAsync<ExternalApiResponse>(
    "https://external-api.com/v1/data",
    "ExpectedResponse.json",
    skipEndpointValidation: true);

// Use custom response type for endpoint
await Client.AssertPostAsync<CustomResponse>(
    "api/v1/users",
    "Request.json",
    "Response.json",
    skipEndpointValidation: true);
```

**What gets validated when skipped:**

- ✅ HTTP status code matches expectation (success vs error)
- ✅ Response content comparison (if `expectedResult` provided)
- ✅ Request executes successfully
- ⏭️ Endpoint metadata validation skipped
- ⏭️ Response type contract checking skipped

**What gets skipped:**

- Type checking against `[ProducesResponseType]` attributes
- Endpoint existence validation
- Status code to response type mapping

**Difference from `ignoreResponse`:**

| Feature | `ignoreResponse: true` | `skipEndpointValidation: true` |
|---------|------------------------|--------------------------------|
| Validates endpoint exists | ✅ Yes | ❌ No |
| Validates response type matches endpoint | ✅ Yes | ❌ No |
| Compares response content | ❌ No | ✅ Yes (if expectedResult provided) |
| Use case | Process tests where call must succeed | External APIs or custom response types |

**Example: Testing external API**

```csharp
[TestMethod]
public async Task Should_Fetch_GitHub_User()
{
    // GitHub API is external - no endpoint metadata available
    await Client.AssertGetAsync<GitHubUser>(
        "https://api.github.com/users/octocat",
        "GitHubUser.json",
        skipEndpointValidation: true);
}
```

**Example: Custom response transformation**

```csharp
[TestMethod]
public async Task Should_Transform_Response()
{
    // Endpoint returns User, but we transform to UserViewModel in test
    await Client.AssertGetAsync<UserViewModel>(
        "api/v1/users/123",
        "UserViewModel.json",
        skipEndpointValidation: true);
}
```

**Future enhancement:**

Later versions may support OpenAPI spec integration for external APIs, allowing endpoint validation even for external services. This would involve downloading and parsing OpenAPI specs at runtime - a bigger round trip that's not currently implemented.

---

## Why this saves ridiculous amounts of time

### Traditional API testing

Typical API tests tend to look like this:

```csharp
Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
Assert.AreEqual("Son", body.Name);
Assert.AreEqual("Goku", body.FirstName);
Assert.AreEqual(99, body.Age);
// ...and so on
```

That approach costs time in three places:

- writing the assertions
- maintaining them when the contract changes
- figuring out what actually broke

### With this SDK

```csharp
await Client.AssertPostAsync<UserResponse>(
    "api/v1/users",
    "CreateUser.json",
    "CreateUser.json");
```

You get:

- full-response verification instead of cherry-picked assertions
- automatic regression detection when new fields appear
- structured diff output instead of vague failures
- replayable `curl` output instead of manual reproduction steps

### Boilerplate reduction that actually matters

| Task | Traditional approach | This SDK |
|---|---|---|
| Add a new edge case | Add `DataRow` + add JSON + keep them in sync | Add one JSON file |
| Validate headers + body + status | Multiple asserts | One snapshot |
| Reproduce a failed request | Rebuild it manually | Paste generated `curl` |
| See nested mismatch location | Manually inspect payloads | Read `MemberPath` |
| Update snapshots after intentional API changes | Rewrite asserts | Enable snapshot update mode |

### The real multiplier: JSON-driven scaling

With `DynamicRequestLocator`, test count scales with files, not attributes.

- 3 scenarios? Add 3 files
- 30 edge cases? Add 30 files
- new bug found in production? Add one JSON file and you have a permanent regression test

That is why this feels like a productivity tool, not just a test library.

---

## Unique features

### `DynamicRequestLocator`: add a JSON file, get a test

This is the killer idea.

```csharp
[DataTestMethod]
[DynamicRequestLocator]
public Task Should_Create_User(string requestFileName)
{
    return Client.AssertPostAsync<UserResponse>(
        "api/v1/users",
        requestFileName,
        requestFileName);
}
```

If the `Requests` folder contains:

- `ValidUser.json`
- `AdminUser.json`
- `MissingField.json`

then the test runner gets one case per file automatically.

No manual `[DataRow]`. No sync issues. No silent gaps.

**Why it matters:**

- adding a case is just adding a file
- deleting a case is just deleting a file
- file names become readable test names
- coverage naturally stays aligned with your snapshot set

If you have many input variations, this feature alone changes the economics of testing.

---

### Full HTTP response snapshots

This SDK validates the full HTTP response, not just the JSON body.

A single snapshot can include:

- response body
- status code
- headers
- trailing headers
- success state

```json
{
  "Content": {
    "Headers": [
      {
        "Key": "Content-Type",
        "Value": [ "application/json; charset=utf-8" ]
      }
    ],
    "Value": {
      "Id": 1,
      "Name": "Son"
    }
  },
  "StatusCode": "OK",
  "Headers": [],
  "TrailingHeaders": [],
  "IsSuccessStatusCode": true
}
```

That means changes in headers, status, or response shape are caught by the same test.

---

### Structured diffs with deep `MemberPath` precision

When a snapshot fails, you do not get a vague object mismatch. You get exact paths.

```plaintext
 ---------------------------------------------------------------------------------- 
 | MemberPath         | SonGokuNewResponse.json | CurrentResult | MismatchType    |
 ---------------------------------------------------------------------------------- 
 | content.value.name | Son 1                   | Son           | ValueDifference |
 ----------------------------------------------------------------------------------
```

This is especially valuable when:

- payloads are nested
- arrays are involved
- a response changed in only one deep property
- you need to distinguish missing vs changed values

Supported mismatch types include:

- `ValueDifference`
- `MissingInFirst`
- `MissingInSecond`

### Array length mismatches

When array lengths differ, the SDK consolidates element-level differences into a single array-level entry.

Instead of showing:

```plaintext
| content.value.emails[0] | null | {"emailAddress": "test@example.com"} | MissingInFirst |
| content.value.emails[1] | null | {"emailAddress": "user@example.com"} | MissingInFirst |
```

You get:

```plaintext
DIFFERENCES
 ----------------------------------------------------------------------------------- 
 | MemberPath           | NewPersonParameter.json | CurrentResult | MismatchType   |
 ----------------------------------------------------------------------------------- 
 | content.value.emails | [] (0 items)            | [2 item(s)]   | MissingInFirst |
 -----------------------------------------------------------------------------------
```

This makes it immediately clear that the issue is array length, not individual element values.

When arrays have mixed differences (some elements changed, some missing), the SDK shows element-level details. Consolidation only happens when all elements are uniformly missing or added.

---

### Built-in `curl` generation

Every failed test includes a ready-to-run `curl` command.

```sh
curl \
--request POST 'https://localhost:5001/api/v1/users' \
--header 'Content-Type: application/json' \
--data-raw '{ ... }'
```

That means:

- faster debugging
- easier collaboration
- simpler reproduction outside the test runner
- better handoff between test failures and API investigation

The generated `curl` output alone removes a surprising amount of wasted time.

---

### Automatic test generation from live traffic

You can generate tests from actual API usage.

```csharp
app.UseTestCreator();
```

The middleware captures requests and responses and turns them into test assets.

This is useful for:

- bootstrapping regression coverage quickly
- documenting legacy APIs
- converting exploratory testing into permanent test cases
- generating real examples from live behavior

---

### Enum test cases without `DataRow` boilerplate

If you need one test per enum value, use `EnumTestCase`.

```csharp
[DataTestMethod]
[EnumTestCase<Status>()]
public async Task Should_Handle_Status(Status status)
{
    // test logic
}
```

Instead of manually listing enum values with `[DataRow]`, test cases are generated automatically.

This is small, but on large suites it removes a lot of repetitive noise.

---

### Snapshot auto-update mode

When an API change is intentional, updating snapshots should be easy.

You can enable snapshot writing per test:

```csharp
await Client.AssertPostAsync<CreateUserResponse>(
    "api/v1/users",
    "CreateUser.json",
    "CreateUser.json",
    writeResponse: true);
```

Or globally:

```csharp
AssertObjectExtensions.WriteResponse = true;
```

Or via environment variable:

```plaintext
AspNetCoreSimpleMsTestSdk__WriteResponse=true
```

Use it when:

- refactoring response contracts
- updating baselines after intentional changes
- regenerating snapshots across a suite

---

### Global and scoped ignore strategies

Some values are dynamic and should not break the test: timestamps, GUIDs, trace IDs, database-generated IDs.

Global ignore example:

```csharp
AssertObjectExtensions.DifferenceFunc = differences =>
{
    foreach (var difference in differences)
    {
        if (difference.MemberPath.Contains("timestamp"))
        {
            continue;
        }

        yield return difference;
    }
};
```

Scoped ignore example:

```csharp
await Client.AssertPostAsync<AddUserReponse>(
    "api/v1/users",
    "NewUser.json",
    "NewUser.json",
    differenceFunc: differences =>
    {
        foreach (var difference in differences)
        {
            if (difference.MemberPath == "Content.Value.Id")
            {
                continue;
            }

            yield return difference;
        }
    });
```

This lets you keep snapshots strict where they should be strict and flexible where they must be flexible.

---

### Dynamic parameter replacement

If your snapshot needs a runtime value, use placeholders.

```csharp
var user = await CreateUserAsync();

await Client.AssertGetAsync<GetUserByIdResponse>(
    $"api/v1/users/{user.Id}",
    "GetUser.json",
    [
        ("$Id$", user.Id)
    ]);
```

Snapshot:

```json
{
  "content": {
    "value": {
      "id": "$Id$",
      "name": "Son",
      "age": 99
    }
  }
}
```

This keeps snapshots deterministic while still supporting dynamic test flows.

---

### Retry support for unstable scenarios

For eventual consistency or flaky integration points, use `SnapshotTestMethod`.

```csharp
[SnapshotTestMethod(maxRetries: 3)]
public async Task Should_Eventually_Be_Consistent()
{
    await Client.AssertGetAsync<Response>("api/eventual", "Response.json");
}
```

Useful for:

- eventual consistency
- async propagation delays
- snapshot creation flows that need retries before settling

---

## More examples / advanced usage

### Standard API snapshot test

```csharp
[TestClass]
public class Persons : ApiTestBase
{
    [TestMethod]
    public Task Should_Be_Able_To_Post_A_Person_By_Json()
    {
        return Client.AssertPostAsync<Person>(
            "api/tests/v1/persons",
            "SonGoku.json",
            "SonGoku.json");
    }
}
```

### Raw JSON input

Possible, but better for small payloads only.

```csharp
[TestMethod]
public Task Should_Return_No_Users_If_No_One_Was_Added()
{
    return Client.AssertGetAsync<GetAllUsersResponse>(
        "v1/users",
        """{ "Users": [] }""");
}
```

### Ignore generated IDs

```csharp
private static IEnumerable<Difference> IgnoreId(IImmutableList<Difference> differences)
{
    foreach (var difference in differences)
    {
        if (difference.MemberPath == "Content.Value.Id")
        {
            continue;
        }

        yield return difference;
    }
}
```

### Simple object comparison

This is available too, but the primary value of the SDK is the ASP.NET Core API testing workflow.

```csharp
[TestMethod]
public void Simple_Object_Comparison()
{
    var person1 = new Person("Son", "Goku", 29);
    var person2 = new Person("Muten", "Roshi", 63);

    Assert.That.ObjectsAreEqual(person1, person2, title: "Persons are not equal");
}
```

Example output:

```plaintext
Persons are not equal

 ----------------------------------
 | MemberPath | person1 | person2 |
 ----------------------------------
 | Name       | Son     | Muten   |
 ----------------------------------
 | FamilyName | Goku    | Roshi   |
 ----------------------------------
 | Age        | 29      | 63      |
 ----------------------------------
```

### Custom comparison strategies

The SDK uses a **Strategy Pattern** for comparisons, making it extensible for custom types.

**Built-in strategies:**

1. **`StringComparisonStrategy`** - Line-by-line comparison (like git diff) for string types
   - Perfect for comparing console outputs, log files, error messages
   - Use `.txt` files as snapshots for string comparisons
2. **`JsonComparisonStrategy`** - Deep object comparison via JSON serialization (fallback for all non-string types)
   - Use `.json` files as snapshots for object comparisons

**How it works:**

- Strategies are checked in registration order via `CanCompare()`
- First matching strategy handles the comparison
- String comparisons use line-by-line diff
- All other types use JSON deep comparison

**Example: String comparison from file**

```csharp
[TestMethod]
public void Compare_Error_Message()
{
    var error = GetErrorMessage();
    
    // Uses StringComparisonStrategy for line-by-line comparison
    Assert.That.ObjectsAreEqual<string>(
        expectedObjectAsJson: "ExpectedError.txt", 
        currentObject: error.Message);
}
```

**Example: Custom CSV comparison strategy**

```csharp
public class CsvComparisonStrategy : ComparisonStrategyBase<CsvData>
{
    protected override ComparisonResult CompareTyped(ObjectAssertContext<CsvData> context)
    {
        var expected = ParseCsv(context.ResolvedExpectedJson);
        var current = context.Current;
        
        var differences = CompareCsvRows(expected, current);
        
        return new ComparisonResult
        {
            Differences = differences,
            FormattedExpected = FormatCsv(expected),
            FormattedCurrent = FormatCsv(current),
            HasSchemaMismatch = differences.Any(d => d.MismatchType != MismatchType.ValueDifference)
        };
    }
}
```

**Registration:**

```csharp
// In your test setup (before running tests)
services.AddSingleton<ISpecificComparisonStrategy, CsvComparisonStrategy>();
services.AddComparisonStrategy(); // Adds built-in strategies (String + JSON)
```

**Why use `ComparisonStrategyBase<T>`?**

- Automatic type checking via `CanCompare()`
- Automatic casting to strongly-typed context
- Defensive validation built-in
- You only implement `CompareTyped()` with your comparison logic

**When NOT to use the base class:**

Don't use `ComparisonStrategyBase<T>` for fallback strategies that handle multiple types (like `JsonComparisonStrategy`). Implement `ISpecificComparisonStrategy` directly instead.

---

## Architecture / design philosophy

### Snapshot-first, but API-focused

This SDK is not generic snapshot tooling with HTTP support bolted on. It is designed around ASP.NET Core API testing.

That is why the core experience centers on:

- full HTTP response snapshots
- deep object diffs
- request reproduction
- file-based scaling of test coverage

### Fail fast, fail with context

The assertion flow is pipeline-based:

1. status code validation
2. content-type validation
3. JSON structure validation
4. deep comparison (extensible via custom comparison strategies)

That means failures stop early and come with relevant context instead of a long tail of noisy assertions.

The comparison system uses a Strategy Pattern, making it extensible for custom types beyond the built-in JSON and string comparisons.

### Contract drift should be obvious

Traditional tests often validate only the fields someone remembered to assert.

Snapshot testing flips that:

- if a field changes, you see it
- if a field disappears, you see it
- if a field is added, you see it
- if a header changes, you see it

That is exactly what you want for API regression safety.

### Type-safe endpoint validation

The SDK enforces type safety at the HTTP layer by distinguishing between compile-time types (`TResult`) and runtime validation types (`ExpectedType`).

**Non-generic methods = NoContent endpoints (204):**

```csharp
// Non-generic signatures expect void response (204 NoContent)
await Client.AssertDeleteAsync("api/v1/items/123");
await Client.AssertPostAsync("api/v1/items", "NewItem.json");
await Client.AssertPutAsync("api/v1/items/123", "UpdatedItem.json");
await Client.AssertPatchAsync("api/v1/items/123", "PatchItem.json");
// → ExpectedType = typeof(void)
// → Validates endpoint returns 204 NoContent
```

**Generic methods = Typed responses (200, 201, etc.):**

```csharp
// Generic signatures expect typed responses (200 OK, 201 Created with body)
await Client.AssertDeleteAsync<DeleteItemResponse>("api/v1/items/123", 
                                                     "Expected.json");
await Client.AssertPostAsync<CreateItemResponse>("api/v1/items",
                                                  "NewItem.json",
                                                  "Expected.json");
await Client.AssertPutAsync<UpdateItemResponse>("api/v1/items/123",
                                                 "UpdatedItem.json",
                                                 "Expected.json");
await Client.AssertPatchAsync<PatchItemResponse>("api/v1/items/123",
                                                  "PatchItem.json",
                                                  "Expected.json");
// → ExpectedType = typeof(ResponseType)
// → Validates endpoint returns 2xx with response body
```

**Why this distinction matters:**

HTTP semantics demand different handling:
- **200 OK / 201 Created** = success with response body
- **204 NoContent** = success without response body

Using the wrong method signature catches real bugs:

```csharp
// ❌ Bug: Test expects void but endpoint returns 200 with body
await Client.AssertPostAsync("api/v1/items", "NewItem.json");
// → Validator Error: "Expected void, got CreateItemResponse"

// ✅ Fix: Use correct generic signature
await Client.AssertPostAsync<CreateItemResponse>("api/v1/items", 
                                                  "NewItem.json",
                                                  "Expected.json");
```

**Supported methods with NoContent variants:**

| HTTP Method | NoContent (204) | With Response Body (200/201) |
|-------------|-----------------|------------------------------|
| GET | N/A (always has body) | `AssertGetAsync<T>()` |
| **QUERY** | N/A (always has body) | `AssertQueryAsync<T>()` |
| POST | `AssertPostAsync()` | `AssertPostAsync<T>()` |
| PUT | `AssertPutAsync()` | `AssertPutAsync<T>()` |
| PATCH | `AssertPatchAsync()` | `AssertPatchAsync<T>()` |
| DELETE | `AssertDeleteAsync()` | `AssertDeleteAsync<T>()` |

**Real-world examples:**

```csharp
// Command-style endpoint (no response needed)
await Client.AssertPostAsync("api/v1/notifications/send", "Notification.json");

// Update endpoint that returns updated entity
await Client.AssertPutAsync<User>("api/v1/users/123", 
                                   "UpdateUser.json", 
                                   "UpdatedUser.json");

// Partial update without response
await Client.AssertPatchAsync("api/v1/users/123/status", "StatusUpdate.json");

// Delete with confirmation response
await Client.AssertDeleteAsync<DeleteConfirmation>("api/v1/items/123", 
                                                     "DeletedItem.json");
```

This prevents:
- Tests passing with wrong expectations
- Refactoring breaking test contracts silently
- 200 vs 204 confusion
- Mismatch between `[ProducesResponseType]` and actual endpoint behavior

### Real behavior should be easy to turn into tests

The live traffic capture feature exists because good API tests often start with a real request. Recording that request and response into reusable test assets is part of the design, not an afterthought.

---

## When to use this SDK

### Great fit

- REST API testing for ASP.NET Core
- integration tests with real HTTP behavior
- contract and regression testing
- large sets of request/response scenarios
- teams that want fast failure diagnosis
- APIs where headers and status matter as much as body shape

### Less ideal

- pure unit tests
- performance benchmarks
- load testing

---

## HTTP QUERY Method Support ([RFC 10008](https://datatracker.ietf.org/doc/html/rfc10008))

The SDK supports the new **HTTP QUERY** method standardized in RFC 10008. QUERY is designed for safe, cacheable queries that can include a request body - bridging the gap between GET (no body) and POST (not safe/cacheable).

### Why HTTP QUERY?

The QUERY method addresses a long-standing limitation in HTTP:

- **GET** is perfect for simple queries but has no request body
- **POST** can send complex queries but isn't safe or cacheable
- **QUERY** gives you both: request body support with GET semantics

Perfect for complex search queries, GraphQL, database queries, or any scenario where query parameters are too limiting but POST semantics don't fit.

### Query without request body (GET-like)

```csharp
[TestMethod]
public Task Should_Query_All_Users()
{
    return Client.AssertQueryAsync<IEnumerable<User>>(
        "api/v1/users",
        "ExpectedUsers.json");
}
```

### Query with request body (the main use case)

```csharp
[TestMethod]
public Task Should_Query_Users_With_Complex_Search()
{
    return Client.AssertQueryAsync<SearchResults>(
        "api/v1/users/search",
        "ComplexSearchRequest.json",
        "ExpectedResults.json");
}
```

Example search request body:
```json
{
  "filters": {
    "ageRange": { "min": 25, "max": 65 },
    "roles": ["Admin", "PowerUser"],
    "active": true
  },
  "sort": [
    { "field": "lastName", "direction": "asc" },
    { "field": "created", "direction": "desc" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 50
  }
}
```

### Error handling for QUERY

```csharp
[TestMethod]
public Task Should_Return_Error_For_Invalid_Query()
{
    return Client.AssertQueryAsErrorAsync<ProblemDetails>(
        "api/v1/users/search",
        "InvalidSearchRequest.json",
        "ExpectedError.json");
}
```

### Implementing QUERY endpoints

**Controller-based:**
```csharp
[AcceptVerbs("QUERY")]
[Route("search")]
[ProducesResponseType(typeof(IEnumerable<User>), 200)]
public IEnumerable<User> QueryUsers([FromBody] SearchRequest request)
{
    return _users.Where(u => /* search logic */);
}
```

**Minimal API:**
```csharp
app.MapMethods("api/v1/users/search", new[] { "QUERY" }, 
    ([FromBody] SearchRequest request) =>
    {
        var results = /* search logic */;
        return Results.Ok(results);
    })
    .WithName("queryUsers")
    .Produces<IEnumerable<User>>(StatusCodes.Status200OK);
```

### Benefits of QUERY over POST

1. **Semantic clarity**: QUERY signals a read-only operation
2. **Cacheability**: Responses can be cached like GET
3. **Safety**: No side effects, idempotent like GET
4. **Body support**: Complex queries without URL length limits
5. **Better REST semantics**: Read operations shouldn't use POST

### When to use QUERY vs GET vs POST

| Method | Use When | Body | Safe | Cacheable |
|--------|----------|------|------|-----------|
| **GET** | Simple queries (query params) | ❌ No | ✅ Yes | ✅ Yes |
| **QUERY** | Complex queries (need body) | ✅ Yes | ✅ Yes | ✅ Yes |
| **POST** | Creating/modifying data | ✅ Yes | ❌ No | ❌ No |

The SDK makes QUERY a first-class citizen with the same full support as GET, POST, PUT, PATCH, and DELETE.

---

## Assert.That - AI-Friendly Assertions

The SDK includes a modern assertion library designed for both human readability and AI debuggability.

### Why `Assert.That.*`?

Traditional assertions give you a line number and a brief message. `Assert.That.*` gives you **structured failure context** that makes debugging instant for humans and enables AI tools to understand test failures without guessing.

### Key features

- **Mandatory context**: Every assertion requires `because` (why) and `fix` (how to resolve) parameters
- **Rich structured output**: Failures include test location, problem description, details, context, and suggested fixes
- **Automatic variable capture**: Uses `CallerArgumentExpression` to show actual code expressions
- **Organized by category**: Boolean, Numeric, String, Collection, DateTime, Exception assertions
- **Human-first, AI-ready**: Beautiful console output with optional JSON serialization for tooling

### Example

```csharp
Assert.That.IsTrue(user.IsActive && user.IsVerified,
    because: "Active and verified users should have full access",
    fix: "Activate the user account in the admin panel");
```

### Failure output

```plaintext
══════════════════════════════════════════════════════════════
⚠️  CONDITION FAILED - EXPECTED TRUE
══════════════════════════════════════════════════════════════

📦 Test Information
──────────────────────────────────────────────────────────────
File     : UserTests.cs:42
Method   : Should_Grant_Access_To_Active_Users
Condition: user.IsActive && user.IsVerified

⚠️ Problem
──────────────────────────────────────────────────────────────
Expected condition to be TRUE but it was FALSE.

📊 Details
──────────────────────────────────────────────────────────────
Condition  : user.IsActive && user.IsVerified
Result     : False
Expected   : True

💭 Context (Why)
──────────────────────────────────────────────────────────────
Active and verified users should have full access

✅ Suggested Fix (How)
──────────────────────────────────────────────────────────────
Activate the user account in the admin panel

Additional suggestions:
  • Review the logic in 'user.IsActive && user.IsVerified' to ensure it returns true
  • Check the values being compared in the condition
  • Verify that prerequisites for this condition are met

══════════════════════════════════════════════════════════════
```

### Available assertions

| Category | Methods | Use For |
|----------|---------|---------|
| **Boolean** | `IsTrue`, `IsFalse` | Condition checks |
| **Null** | `IsNull`, `IsNotNull` | Null reference validation |
| **Equality** | `AreEqual`, `AreNotEqual`, `AreSame`, `AreNotSame` | Value and reference comparison |
| **Type** | `IsInstanceOfType`, `IsNotInstanceOfType` | Type checking |
| **Numeric** | `IsGreaterThan`, `IsLessThan`, `IsInRange`, `IsPositive`, `IsNegative` | Number validation |
| **String** | `IsEmpty`, `IsNotEmpty`, `Contains`, `StartsWith`, `EndsWith`, `Matches` | String validation |
| **Collection** | `IsEmpty`, `IsNotEmpty`, `Contains`, `DoesNotContain`, `AllMatch` | Collection validation |
| **Exception** | `Throws`, `DoesNotThrow` | Exception behavior |
| **DateTime** | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `IsUtc`, `IsLocal`, `IsUnspecified` | Date/time validation |
| **DateTimeOffset** | `IsAfter`, `IsBefore`, `IsInRange`, `IsCloseTo`, `HasOffset`, `IsUtc`, `IsLocal` | Timezone-aware date/time validation |

### Why not `AiAssert.*`?

The `because` and `fix` parameters make assertions self-documenting. The structured output format is already AI-parseable. Creating a separate `AiAssert` namespace would fragment the API and create confusion about when to use which.

**Single API, dual benefit**: Write once, debug easily (human), parse reliably (AI).

---

## Contributing

This SDK is battle-tested in production environments.

Repository: `https://renepeuser.visualstudio.com/_git/AspNetCore.Simple.MsTest.Sdk`

---

## License

Copyright 2021-2026 (c) Rene Peuser. All rights reserved.
