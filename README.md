# AspNetCore.Simple.MsTest.Sdk

This package is designed to enable efficient and clean testing against your ASP.NET Core APIs. It dramatically reduces the amount of required asserts, allowing for faster creation of more readable tests. It supports a Test-First approach, helping developers focus on testing earlier in the development cycle.

## Getting started

### Prerequisites
* [.Net 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Install the package

```dotnetcli
dotnet add package AspNetCore.Simple.MsTest.Sdk
```

### Basic concept
Our assert helpers are designed to streamline testing by doing the following:
- Asserting expected call outcomes (e.g., `AssertPostAsync` for success and `AssertPostAsErrorAsync` for errors).
- Comparing the entire response structure for equality, not just the status code.
- Allowing for direct usage of JSON strings or files in tests.
- Directly indicating the route being tested.
- Enhancing productivity by comparing content headers, status codes, and more.

```csharp
await Client.AssertPostAsync<AddUserReponse>($"api/v1/users/",                                                                        
                                             "Users.V1.Payloads.NewUser.json,
                                             "Users.V1.Results.NewUser.json);
```

## Setup your test environment
We provide you a simple `ApiTestBase<Startup>` you can use it directly in your test
class. But we recommend that you setup a central base class for startup and tear down.
Sample:

```csharp
using System;
using System.Net.Http;
using AspNetCore.Simple.MsTest.Sdk.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

## Setup a test class
```csharp
[TestClass]
public class Persons : ApiTestBase
{
    [TestMethod]
    public Task Should_Be_Able_To_Post_A_Person_By_Json()
    {
        return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                              "Payloads.SonGoku.json", // This json file must be an embedded file in your solution or native json string
                                              "Results.SonGoku.json"); // This json file must be an embedded file in your solution or native json string
    }
}
```

Payload: "Payloads.SonGoku.json"
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

Response: "Results.SonGoku.json"
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



## Samples

### Simple object comparisons
```csharp
[TestMethod]
public void Simple_Object_Comparison()
{
    var person1 = new Person("Son", "Goku", 29);
    var person2 = new Person("Muten", "Roshi", 63);

    Assert.That.ObjectsAreEqual(person1, person2, title: "Persons are not equal");
}

[TestMethod]
public void Simple_Object_Comparison()
{
    var firstNumber = 1;
    var secondNumber = 2;

    Assert.That.ObjectsAreEqual(firstNumber,secondNumber, title: "Persons are not equal");
}
```

```bash
Assert.IsTrue failed. 

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

### Embedded json file or native json string
```csharp
[TestMethod]
public Task Should_Return_No_Users_If_No_One_Was_Added()
{
    return Client.AssertGetAsync<GetAllUsersResponse>("v1/users", "EmptyUserResponse.json");
}
```

```csharp
[TestMethod]
public Task Should_Return_No_Users_If_No_One_Was_Added()
{
    return Client.AssertGetAsync<GetAllUsersResponse>("v1/users", """{ "Users": [] }""");
}
```

### Assert that GET all Users will returned 401 Unauthorized
```csharp
[TestMethod]
public Task Should_Not_Return_All_Users_Without_Authentication()
{
    return Client.AssertGetAsUnauthorizedAsync("v1/users");
}
```

### Assert that GET a user which not exists returns ProblemDetails

```csharp
[TestMethod]
public Task Should_Return_Not_Found_Error_If_User_Does_Not_Exits()
{
    return Client.AssertGetAsErrorAsync<ProblemDetails>($"v1/users/{1234}", "UserGetByIdErrorResponse.json");
}
```

### Assert that GET all Users is successful and checks that response is empty
```csharp
[TestMethod]
public Task Should_Return_No_Users_If_No_One_Was_Added()
{
    return Client.AssertGetAsync<GetAllUsersResponse>("v1/users", "EmptyUserResponse.json");
}
```

### Create a User and Ignore an Id which maybe is generated by the backend or database
```csharp
[TestMethod]
public Task Should_Return_Expected_Result_For_Given_Payload_But_Ignore_Id()
{
    await Client.AssertPostAsync<AddUserReponse>($"api/v1/users/",                                                                        
                                                 "Users.V1.Payloads.NewUser.json",
                                                 "Users.V1.Results.NewUser.json",
                                                 differenceFunc: DifferenceFunc);
}

// Difference func can be used to intercept the object comparison in the background
private IEnumerable<Difference> DifferenceFunc(IImmutableList<Difference> differences)
{
    foreach (var difference in differences)
    {
        // Here we ignore the Id property. Real world scenario generated id by database as an example
        if (difference.MemberPath == nameof(User.Id))
        {
            continue;
        }

        yield return difference;
    }
}
```

### Ignore functionality on error response
```csharp
[TestMethod]
public Task Should_Handle_Error_Response_With_Filter_Func()
{
    // 1. Call endpoint which will return an error response
    return Client.AssertPostAsErrorAsync<ProblemDetails>("api/tests/v1/errors/not-implemented",
                                                         "ErrorResponse.json",
                                                         DifferenceFunc);

    // 2. Intercept difference detection also for error response
    static IEnumerable<Difference> DifferenceFunc(IImmutableList<Difference> differences)
    {
        foreach (var difference in differences)
        {
            yield return difference;
        }
    }
}
```


### Fetch data from an API and do a post order to bring the items in the right order
```csharp
[TestMethod]
public Task Should_Return_Expected_Result_For_Given_Payload_And_Sorted()
{
    return Client.AssertGetAsync<IEnumerable<Person>>($"api/v1/users/",                                                                                                                             
                                                      "Users.V1.Results.AllUsers.json",
                                                      filterFunc: FilterFunc);
}

// The filter func can be used to sort or do some custom post filtering
// Sample: You get unsorted results from API so each call will provide
//         the users in different order. Why a something like a DB query
//         without sort action will not guarantee the order of the results.
//         If results does not match the expected results (order as well), 
//         the test will fail
private IEnumerable<Person> FilterFunc(IEnumerable<Person> arg)
{
    return arg.OrderBy(x => x.Id).ToImmutableList();
}
```


### Whole create, get and delete scenario. Looks nice and clean
```csharp
[TestMethod]    
public Task Should_Return_The_User_Which_Was_Added()
{
    // 1. Add an user
    var addedUserResponse = await Client.AssertPostAsync<AddUserReponse>($"api/v1/users/",
                                                                         "Users.V1.Payloads.NewUser.json",
                                                                         "Users.V1.Results.NewUser.json"); 

    // 2. Get the currently added user
    await Client.AssertGetAsync<GetAllUserResponse>($"api/v1/users/{addedUserResponse.User.Id}"
                                                     "Users.V1.Results.AddedUser.json");

    // 3. Delete the alrady added user -> Dependent on your test setup a test-tear down can also contain a cleanup step to remove all the created sources
    await Client.AssertDeleteAsync($"api/v1/users/{addedUserResponse.User.Id}"
                                   "Users.V1.Results.Deleteduser.json");
}
```

### Replacements
```csharp
[TestMethod]    
public Task Should_Return_The_User_Which_Was_Added()
{
    // 1. Add an user
    var addedUserResponse = await Client.AssertPostAsync<AddUserReponse>($"api/v1/users/",
                                                                         "Users.V1.Payloads.NewUser.json",
                                                                         "Users.V1.Results.NewUser.json"); 

    // 2. Get the currently added user
    await Client.AssertGetAsync<GetUserByIdResponse>($"api/v1/users/{addedUserResponse.User.Id}"
                                                     "Users.V1.Results.AddedUser.json",
                                                     [("{Id}", addedUserResponse.User.Id)]); // New, will replace in the Users.V1.Results.AddedUser.json the {Id} with the value of addedUserResponse.User.Id

    // 3. Delete the alrady added user -> Dependent on your test setup a test-tear down can also contain a cleanup step to remove all the created sources
    await Client.AssertDeleteAsync($"api/v1/users/{addedUserResponse.User.Id}"
                                   "Users.V1.Results.Deleteduser.json");
}
```


### Header, Status codes and many more
For each test we are evaluating the whole response which is based on a "Snapshot" from your api response.
```
 Assert.IsTrue failed. 
    
    Http call infos:
    
     ----------------------------------------------------------------------------- 
     | HttpMethod | Url                                         | HttpStatusCode |
     ----------------------------------------------------------------------------- 
     | POST       | https://localhost:5001/api/tests/v1/persons | OK             |
     ----------------------------------------------------------------------------- 
    
    
    Detected differences: 3
    
    
     --------------------------------------------------------------------------------------------------------------------- 
     | MemberPath                                  | "Results.NewPersonParameter.json" | CurrentResult                   |
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
    --request POST 'https://localhost:5001/api/tests/v1/persons' \
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

### Curl for each `Asserted` call
How pratical can it be so share call scenarios with your consumers.
For that reason you see in the test output the curl command for each 
asserted call.

This is like an aggreate function combined with possible context specific
ignore functions. First this GlobalIgnore func will be executed pre filter
the differences after this passed local ignore functions will be executed
with the pre filtered differences.
```csharp

```curl
-----------------------------------------------------------
Http call as curl
-----------------------------------------------------------
curl \
--location \
--request GET 'https://localhost:5001/api/tests/v1/persons'
--header 'Authorization: Bearer Sorry i am secret :)'
-----------------------------------------------------------
```

### Global ignore func
We provide you a global ignore possibilty to ignore common values which contains random values
In this sample here, we ignore the `x-amzn-trace-id` header if it is different for any assert in
your test assembly.

```csharp
AssertObjectExtensions.DifferenceFunc = DifferenceFunc;

static IEnumerable<Difference> DifferenceFunc(IImmutableList<Difference> differences)
{
    foreach (var difference in differences)
    {
        // 1. Response headers for x-amzn-trace-id are different any call
        if (difference.MemberPath.Contains("x-amzn-trace-id"))
        {
            continue;
        }

        yield return difference;
    }
}
```

### Request locator
To simplify multiple use cases you can simplify your tests with this little trick :)

```csharp
[TestMethod]
// NEW dynamic request locator -> dynamic location. A "Requests" folder will be searched in your scope
[RequestLocator]
// NEW static request locator -> static location
[RequestLocator("Api.Users.V1.Create.Status_200_Ok.Requests")]
public async Task Should_Be_Able_To_Create_A_User(string useCase)
{
    // 1. Create a new user
    var createUserResponse = await Client.AssertPostAsync<CreateUserResponse>($"api/console/v1/aws-s3-buckets/",
																			  useCase,
																			  useCase,
																			  IgnoreIdAndDate,
																			  [
																			   ("$UniqueUserName$", UniqueUserName),
																			   ("$Id$", Guid.Empty)
																			  ]).ConfigureAwait(false);

    // 2. Get the currently created user
    await Client.AssertGetAsync<GetUserByIdResponse>($"api/console/v1/aws-s3-buckets/{createUserResponse.User.Id}",
													 useCase,
													 IgnoreLastUpdatedDate,
													 [
													  ("$UniqueUserName$", UniqueUserName),
													  ("$Id$", createUserResponse.User.Id)
													 ]).ConfigureAwait(false);
}
```


### Test-Writer (POC-State)
We provide you now a small dev tool to help you create your test response files.
This helps you speed up writing and getting your test green.
```csharp
[TestClass]
public class Persons : ApiTestBase
{
    [TestMethod]
    public Task Should_Be_Able_To_Post_A_Person_By_Json()
    {
        return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                              "Payloads.SonGoku.json",
                                              "Results.SonGoku.json",
                                              writeResponse: false); // NEW: With this flag your json / string repsonse will be created with the given file name.
    }
}

```

#### Test-Writer (POC-State) - Global Response flag
Writes for all running tests -> be careful when using it !
```csharp
AssertObjectExtensions.WriteResponse = true;
```