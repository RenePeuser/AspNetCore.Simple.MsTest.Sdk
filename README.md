# AspNetCore.Simple.MsTest.Sdk

> 🚀 Snapshot-based API testing for ASP.NET Core\
> Clean. Deterministic. Productive.

This package enables efficient and structured testing of your ASP.NET
Core APIs using full-response snapshot comparison.

It dramatically reduces required asserts and promotes:

-   ✅ Test-First development
-   ✅ Full response validation (headers, status, body)
-   ✅ JSON snapshot comparison
-   ✅ Clean, readable tests
-   ✅ Powerful difference analysis

------------------------------------------------------------------------

# 📦 Installation

## Prerequisites

-   .NET 9

## Install

``` bash
dotnet add package AspNetCore.Simple.MsTest.Sdk
```

------------------------------------------------------------------------

# 🧠 Core Concept

Instead of manually asserting:

-   Status codes
-   Headers
-   Content
-   Nested objects
-   Collections

You assert the **entire HTTP response snapshot**.

``` csharp
await Client.AssertPostAsync<AddUserResponse>("api/v1/users",
                                              "NewUser.json",
                                              "NewUser.json");
```
✔ Full response comparison\
✔ Automatic difference table\
✔ Curl output on failure\
✔ Snapshot-based testing\


```   
    Http call infos:
    
     ----------------------------------------------------------------------------- 
     | HttpMethod | Url                                         | HttpStatusCode |
     ----------------------------------------------------------------------------- 
     | POST       | https://localhost:5001/api/tests/v1/users   | OK             |
     ----------------------------------------------------------------------------- 
    
    
    Detected differences: 3
    
    
     --------------------------------------------------------------------------------------------------------------------- 
     | MemberPath                                  | "NewUser.json"                    | CurrentResult                   |
     --------------------------------------------------------------------------------------------------------------------- 
     | Content.Headers["Content-Type"].Value[0]    | application/octet; charset=utf-8  | application/json; charset=utf-8 |
     --------------------------------------------------------------------------------------------------------------------- 
     | Content.Value.FirstName                     | Goku Failed                       | Goku                            |
     --------------------------------------------------------------------------------------------------------------------- 
     | StatusCode                                  | NotFound                          | OK                              |
     --------------------------------------------------------------------------------------------------------------------- 
    
    Expected result:
    
    {"Version":"1.1","Content":{"Headers":[{"Key":"Content-Type","Value":["application/octet; charset=utf-8"]}],"Value":{"Id":1,"Name":"Son","FirstName":"Goku Failed","Age":42,"Emails":[]}},"StatusCode":"NotFound","ReasonPhrase":"OK","Headers":[],"TrailingHeaders":[],"IsSuccessStatusCode":true}
    
    Current result:
    
    {"Version":"1.1","Content":{"Headers":[{"Key":"Content-Type","Value":["application/json; charset=utf-8"]}],"Value":{"Id":1,"Name":"Son","FirstName":"Goku","Age":42,"Emails":[]}},"StatusCode":"OK","ReasonPhrase":"OK","Headers":[],"TrailingHeaders":[],"IsSuccessStatusCode":true}
    
    
    --------------------------------------------------------------
    Http call as curl
    --------------------------------------------------------------
    curl \
    --location \
    --request POST 'https://localhost:5001/api/tests/v1/users' \
    --header 'Content-Type: application/json' \
    --header 'Authorization: Bearer Sorry i am secret :)'
    --data-raw '{
      "Id": 1,
      "Name": "Son",
      "FirstName": "Goku Failed",
      "Age": 99,
      "Emails": []
    }'
    --------------------------------------------------------------
    
```


------------------------------------------------------------------------

# 🎯 Why This SDK?
```

  Traditional Testing      This SDK
  ------------------------ -----------------------
  Many asserts             One snapshot
  Manual header checks     Automatic
  Manual JSON comparison   Deep diff engine
  Hard to debug            Structured diff table
  No reproduction          Auto-generated curl
```

------------------------------------------------------------------------

# 🧠 Design Philosophy

-   Snapshot-first testing
-   Deterministic results
-   Minimal boilerplate
-   Developer productivity focus

------------------------------------------------------------------------

# 📁 JSON File Convention (IMPORTANT)

From now on:

> ✅ Only use the file name\
> ❌ Never use full paths like `Users.V1.Payloads.NewUser.json`

### ✔ Correct

``` csharp
"NewUser.json"
```

------------------------------------------------------------------------

# 📊 Simple object comparison
```csharp
[TestMethod]
public void Simple_Object_Comparison()
{
    var person1 = new Person("Son", "Goku", 29);
    var person2 = new Person("Muten", "Roshi", 63);

    Assert.That.ObjectsAreEqual(person1, person2, title: "Persons are not equal");
}
```

```bash
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

 Count: 3

Current result:

{"Name":"Muten","FamilyName":"Roshi","Age":63}

Expected result:

{"Name":"Son","FamilyName":"Goku","Age":29}
```

------------------------------------------------------------------------

# 📦 Recommended Test Folder Structure

    📦 Api
     ┣ 📂 Users
     ┃ ┗ 📂 V1
     ┃ ┃ ┣ 📂 Create
     ┃ ┃ ┃ ┣ 📂 Status_200_Ok
     ┃ ┃ ┃ ┃ ┣ 📂 Requests
     ┃ ┃ ┃ ┃ ┃ ┗ 📜 CreateUser.json
     ┃ ┃ ┃ ┃ ┣ 📂 Responses
     ┃ ┃ ┃ ┃ ┃ ┗ 📜 CreateUser.json
     ┃ ┃ ┃ ┃ ┗ 📜 CreateUser_Status_200_OK_Test.cs

-   `Requests` folder → input payload\
-   `Responses` folder → expected snapshot\
-   Test class sits in same logical folder\
-   Only file name is required in your test

------------------------------------------------------------------------

# 🧪 Setup Test Base

Just a sample, you can also use your own custom setup. This is just out of the box provided with this sdk.

``` csharp
namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public abstract class ApiTestBase
    {       
        private static ApiTestBase<Startup> _apiTestBase = null!;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // 1. Super simple just use the provided API test base class and you are ready to go
            _apiTestBase = new ApiTestBase<Startup>("Development", // The environment name
                                                    (_, _) => { }, // The register services action
                                                    []);           // Configure environment variables  

            // 2. We need once the http client to communicate with the started api
            Client = _apiTestBase.CreateClient();
        }

        protected static HttpClient Client { get; private set; } = null!;

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase.Dispose();
            Client.Dispose();
        }
    }
}
```

------------------------------------------------------------------------

# 🧪 Example Test

``` csharp
[TestClass]
public class Persons : ApiTestBase
{
    [TestMethod]
    public Task Should_Be_Able_To_Post_A_Person_By_Json()
    {
        return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                              "SonGoku.json",  // This json file must be an embedded file in your solution or native json string
                                              "SonGoku.json"); // This json file must be an embedded file in your solution or native json string
    }
}

// Possible but not recommended:
// You can use also raw json strings instead of files, but this is not recommended for large payloads
[TestMethod]
public Task Should_Return_No_Users_If_No_One_Was_Added()
{
    return Client.AssertGetAsync<GetAllUsersResponse>("v1/users", """{ "Users": [] }""");
}
```

------------------------------------------------------------------------

# 🧩 Ignore Generated Values

``` csharp
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

------------------------------------------------------------------------

# 🔁 Replacements

``` csharp
await Client.AssertGetAsync<GetUserByIdResponse>($"api/v1/users/{user.Id}",
                                                 "GetUser.json",
                                                 [
                                                     ("$Id$", user.Id)
                                                 ]);
```

------------------------------------------------------------------------

### Mismatch Types

-   `ValueDifference`
-   `MissingInFirst`
-   `MissingInSecond`

------------------------------------------------------------------------

# 🌍 Global Ignore

``` csharp
AssertObjectExtensions.DifferenceFunc = differences =>
{
    foreach (var difference in differences)
    {
        if (difference.MemberPath.Contains("x-amzn-trace-id"))
        {
            continue;
        }

        yield return difference;
    }
};
```

------------------------------------------------------------------------

# 🧪 Enum Test Cases

Instead of this:

```csharp
[DataTestMethod]
[DataRow(MyEnum.Feature)]
[DataRow(MyEnum.Component)]
[DataRow(MyEnum.System)]
[DataRow(MyEnum.Feature)]
[DataRow(MyEnum.Component)]
[DataRow(MyEnum.System)]
public async Task Should_Be_Able_To_Create_A_CapabilityType_If_Status_Is_Correct(MyEnum useCase)
{
    // Your test logic here
}
```

Just use:

```csharp
[DataTestMethod]
// Generates one test case for each enum value in CapabilityTypeEnum with status "Active"
// You can even add multiple sets
[EnumTestCase<CapabilityTypeEnum>()]
public async Task Should_Be_Able_To_Create_A_CapabilityType_If_Status_Is_Correct(MyEnum useCase)
{
    // Your test logic here
}
```


------------------------------------------------------------------------

# 🛠 Test Response Writer

``` csharp
await Client.AssertPostAsync<CreateUserResponse>("api/v1/users",
                                                 "CreateUser.json",
                                                 "CreateUser.json",
                                                 writeResponse: true);
```

Global flag:

``` csharp
AssertObjectExtensions.WriteResponse = true;
```

Environment variable:

    AspNetCoreSimpleMsTestSdk__WriteResponse=true

⚠ Use carefully --- this overwrites snapshots.

------------------------------------------------------------------------

# 🔥 Curl Output

Each test used by the provided exetensions tracks the request and response.

``` bash
curl \
--request POST 'https://localhost:5001/api/v1/users' \
--header 'Content-Type: application/json' \
--data-raw '{ ... }'
```

------------------------------------------------------------------------
