# Assert helpers

## Assert API results

Wihout this test SDK
```csharp
[TestMethod]
public async Task Test_What_You_Expect_Without_The_Test_Sdk()
{
    // Call your Api, which returns a response object.
    var getPersonResponse = await Client.GetAsync("/myApi/persons");

    // To avoid bad errors in your test code you should check first that your call was successful.
    Assert.IsTrue(getPersonResponse.IsSuccessStatusCode, "Get persons was not successful")

    // If it was successfull, then you can read the content what you will expect.
    var persons = await getPersonResponse.Content.ReadAsAsync<IEnumerable<Person>>();

    // If you have your data you has to assert all your data....
    Assert.AreEqual(10, persons.Count())
    Assert.AreEqual(...)
    Assert.AreEqual(...)
    Assert.AreEqual(...)
    Assert.AreEqual(...)
    Assert.AreEqual(...)
    Assert.AreEqual(...)    
}
```

Test an API result
```csharp
[TestMethod]
public async Task Test_What_You_Expect_With_The_Test_Sdk()
{
    // first you can call your API and define direct getPersonResponse type, exception is handled automatically !!
    // much more nicer to read, and concrete data response
    var persons = await Client.GetAsAsync<IEnumerable<Person>>("/myApi/persons");

    // In this sample we have our expected result in an embeded file inside the assembly, we fetch the expected
    // result and compare it then with the current result. You can also use your own object, just a sample...
    var expectedResult = EmbededFile.GetFileContentFrom("get-expected-person-result.json");

    // This assert helper compares complete object hierachies and provides you all differences
    // Cool, saves you writing tons of test and assert, and if you extend your data or change it this test works
    // as well
    Assert.That.ObjectsAreEqual(() => expectedResult, () => persons);
}
```

Test an occuring exception of an API call
```csharp
[TestMethod]
public async Task Test_What_You_Expect_With_The_Test_Sdk()
{
    // This extensions will check the type what you request if it is an exception it will handle
    // correct in the background.
    var problemDetailsException = await Client.GetAsAsync<ProblemDetailsException>("/myApi/persons");

    // Your assert code....
}
```

WIP - Is in preparation for `POST` `PUT` `GET` `DELETE`

Test an API very ultra simple, with an emdedded `myResult.json` file in your test assembly.
```csharp
[TestMethod]
public Task Should_Return_Expected_Persons()
{    
    // This is now the ultimate simple version of, you can assert a complete API cal in one line
    // 1. Add your url => "/myAPi/persons"
    // 2. Add your expetced result from your API, you can give a json string or the name of an embedded file
    return Client.AssertGetAsync<IEnumerable<Person>>("/myApi/persons", "myResult.json");
}
```

Test an API very ultra simple, with an emdedded `json string` file in your test assembly.
```csharp
[TestMethod]
public Task Should_Return_Expected_Persons()
{    
    // This is now the ultimate simple version of, you can assert a complete API cal in one line
    // 1. Add your url => "/myAPi/persons"
    // 2. Add your expetced result from your API, you can give a json string or the name of an embedded file
    return Client.AssertGetAsync<IEnumerable<Person>>("/myApi/persons", ""[{""firstName"":""Son Goku""}, {""firstName"":""Vegeta""}]"");
}
```