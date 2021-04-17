# Assert helpers

WIP - Is in preparation for `POST` `PUT` `GET` `DELETE`. With this testhelper you will be able to write realy nice and simple integration test, which checks IN -> OUT

Test an API very ultra simple, with an emdedded `myResult.json` file in your test assembly.
```csharp
[TestMethod]
public Task Should_Return_Expected_Persons()
{    
    // This is now the ultimate simple version of, you can assert a complete API cal in one line
    // 1. Use your extension method for your http call GET/POST/PUT/DELETE
    // 2. Set as generic type the type of your response type of your endpoint
    // 3. Add your url => "/myAPi/persons"
    // 4. Add the name of the embeded json file from your expected results.
    return Client.AssertGetAsync<IEnumerable<Person>>("/myApi/persons", "myResult.json");
}
```

Test an API very ultra simple, with an emdedded `json string` file in your test assembly.
```csharp
[TestMethod]
public Task Should_Return_Expected_Persons()
{    
    // This is now the ultimate simple version of, you can assert a complete API cal in one line
    // 1. Use your extension method for your http call GET/POST/PUT/DELETE
    // 2. Set as generic type the type of your response type of your endpoint
    // 3. Add your url => "/myAPi/persons"
    // 4. Add the expected result as json string
    return Client.AssertGetAsync<IEnumerable<Person>>("/myApi/persons", ""[{""firstName"":""Son Goku""}, {""firstName"":""Vegeta""}]"");
}
```