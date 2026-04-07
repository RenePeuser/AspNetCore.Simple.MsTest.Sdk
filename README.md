# `AspNetCore.Simple.MsTest.Sdk`

[![NuGet](https://img.shields.io/badge/nuget-AspNetCore.Simple.MsTest.Sdk-blue)](https://www.nuget.org/packages/AspNetCore.Simple.MsTest.Sdk)
[![.NET 10](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Proprietary-red)]()

> **API snapshot testing so productive it feels like cheating.**  
> Add a JSON file. A test appears. When it fails, you get the exact diff, full HTTP context, and a ready-to-run `curl`.

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

**Add a JSON file. A new test appears.**

## What you get

- Full HTTP response snapshots: status, headers, body, trailing headers
- Precise structured diffs with deep `MemberPath` paths
- Ready-to-run `curl` output on failures
- Convention-based test discovery with `DynamicRequestLocator`
- Snapshot generation from live traffic
- Snapshot auto-update and ignore strategies
- Drastically less boilerplate than traditional API tests

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
    private static ApiTestBase<Startup> _testBase = null!;
    protected static HttpClient Client { get; private set; } = null!;

    [AssemblyInitialize]
    public static void Init(TestContext _)
    {
        _testBase = new ApiTestBase<Startup>("Development");
        Client = _testBase.CreateClient();
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        _testBase.Dispose();
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

## What a failure looks like

This is where the SDK earns its place.

```plaintext
══════════════════════════════════════════════════════════════════════════════
SNAPSHOT TEST FAILED
══════════════════════════════════════════════════════════════════════════════

Project   : AspNetCore.Simple.MsTest.Sdk.Test
Class     : Person_Test.cs
Method    : Should_Be_Able_To_Put_A_Patch_By_Json

Request   : AspNetCore.Simple.MsTest.Sdk.Test.Controllers.Requests.SonGoku.json
Response  : AspNetCore.Simple.MsTest.Sdk.Test.Controllers.Responses.SonGokuNewResponse.json

Errors    : 1
ErrorTypes: ValueDifference

HTTP CALL
 ----------------------------------------------------------------------- 
 | HttpMethod | Url                                   | HttpStatusCode |
 ----------------------------------------------------------------------- 
 | PUT        | http://localhost/api/tests/v1/persons | 200 OK         |
 -----------------------------------------------------------------------

DIFFERENCES
 ---------------------------------------------------------------------------------- 
 | MemberPath         | SonGokuNewResponse.json | CurrentResult | MismatchType    |
 ---------------------------------------------------------------------------------- 
 | content.value.name | Son 1                   | Son           | ValueDifference |
 ----------------------------------------------------------------------------------

EXPECTED RESULT (SonGokuNewResponse.json):

{"content":{"headers":[{"key":"Content-Type","value":["application/json; charset=utf-8"]}],"value":{"id":1,"name":"Son 1","firstName":"Goku","age":99,"emails":[{"emailAddress":"alf@gmx.de","type":"GMX"},{"emailAddress":"abc@hotmail.de","type":"Microsoft"}]}},"statusCode":"OK","headers":[],"trailingHeaders":[],"isSuccessStatusCode":true}

CURRENT RESULT:

{"content":{"headers":[{"key":"Content-Type","value":["application/json; charset=utf-8"]}],"value":{"id":1,"name":"Son","firstName":"Goku","age":99,"emails":[{"emailAddress":"alf@gmx.de","type":"GMX"},{"emailAddress":"abc@hotmail.de","type":"Microsoft"}]}},"statusCode":"OK","headers":[],"trailingHeaders":[],"isSuccessStatusCode":true}

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Http call as curl
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
curl \
--location \
--request PUT 'http://localhost/api/tests/v1/persons' \
--header 'Content-Type: application/json' \
--data-raw '{"Id":1,"Name":"Son","FirstName":"Goku","Age":99,"Emails":[{"EmailAddress":"alf@gmx.de","Type":"GMX"},{"EmailAddress":"abc@hotmail.de","Type":"Microsoft"}]}'
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
```

You immediately see:

1. **Exact failure location**: `content.value.name`
2. **Expected vs actual**: `Son 1` vs `Son`
3. **HTTP context**: method, URL, status code
4. **Reproduction command**: generated `curl`

```sh
curl \
--location \
--request PUT 'http://localhost/api/tests/v1/persons' \
--header 'Content-Type: application/json' \
--data-raw '{"Id":1,"Name":"Son","FirstName":"Goku","Age":99,"Emails":[{"EmailAddress":"alf@gmx.de","Type":"GMX"},{"EmailAddress":"abc@hotmail.de","Type":"Microsoft"}]}'
```

That is a completely different debugging experience from:

```csharp
Assert.AreEqual("Son 1", response.Name);
```

This SDK does not just tell you that something failed. It tells you **where**, **what**, **under which HTTP call**, and **how to replay it now**.

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

## Setup test base

The SDK provides a very simple out-of-the-box setup pattern.

```csharp
namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public abstract class ApiTestBase
    {
        private static ApiTestBase<Startup> _apiTestBase = null!;
        protected static HttpClient Client { get; private set; } = null!;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            _apiTestBase = new ApiTestBase<Startup>(
                "Development",
                (_, _) => { },
                []);

            Client = _apiTestBase.CreateClient();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase.Dispose();
            Client.Dispose();
        }
    }
}
```

You can also use your own custom setup. The point is that the default path is intentionally small.

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
4. deep comparison

That means failures stop early and come with relevant context instead of a long tail of noisy assertions.

### Contract drift should be obvious

Traditional tests often validate only the fields someone remembered to assert.

Snapshot testing flips that:

- if a field changes, you see it
- if a field disappears, you see it
- if a field is added, you see it
- if a header changes, you see it

That is exactly what you want for API regression safety.

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

## Contributing

This SDK is battle-tested in production environments.

Repository: `https://renepeuser.visualstudio.com/_git/AspNetCore.Simple.MsTest.Sdk`

---

## License

Copyright 2021-2026 (c) Rene Peuser. All rights reserved.

---

## What changed and why

- **Rebuilt the README around impact-first flow.** The new structure leads with the value proposition, tiny example, and immediate “why this is different” message instead of starting with a long feature dump.
- **Moved the failure output near the top.** The debugging experience is one of the strongest differentiators, so the README now shows exact diffs, HTTP context, and generated `curl` before deeper explanations.
- **Made `DynamicRequestLocator` a centerpiece instead of a buried detail.** The “add a JSON file → a test appears” story is now clearly positioned as a major productivity breakthrough.
- **Cut repetition aggressively.** Duplicate explanations of `DynamicRequestLocator`, `curl`, ignore strategies, and snapshot update mode were consolidated into single stronger sections.
- **Shifted lower-value material later.** Folder conventions, test base setup, general object comparison, and architecture details now support the story instead of slowing down the opening.
- **Framed the package as a productivity multiplier, not just a test library.** The revised README emphasizes speed, scale, debuggability, and boilerplate reduction throughout.
