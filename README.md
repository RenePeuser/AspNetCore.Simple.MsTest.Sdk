# AspNetCore.Simple.MsTest.Sdk

Target of this package is to write more efficient clean test against your ASP.Net Core API's.
You will save tons of Assert and will be able to write even more faster and better readable test as before.
Main reason was to be more focused on the Test-First approach.

## Getting started

### Prerequisites
* [.Net 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

### Install the package

```dotnetcli
dotnet add package AspNetCore.Simple.MsTest.Sdk
```

## Samples

### Basic concept
```csharp
// Those assert helper are smart they do following things:
// - Assser the expected call for Sucess -> AssertPostAsync -> Ok AssertPostAsErrorAsync -> NOK
// - It compares the complete reponse structure for equality -> Not only IsSuccessStatusCode, Also deep object 
// - You can provide json as string, or like here in the sample a json file which is embedded in the test assembly
// - Why json -> it is that fomat which is used for communication, and you can directly use your payloads in curl, 
//   postman or anywhere -> if you have c# code -> transform first into json, not nice to handle comparison
// - With this syntax you see directly the route which will be called
// - It is all made for maximize productivity

await Client.AssertPostAsync<AddUserReponse>($"api/v1/users/",                                                                        
                                             "Users.V1.Payloads.NewUser.json,
                                             "Users.V1.Results.NewUser.json);
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
