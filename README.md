# AspNetCore.Simple.MsTest.Sdk

!! PRERELEASE STATE !!

Target of this package is to write more effecient clean test against your ASP.Net Core API's.
You will save tons of `Assert` and will be able to write even more faster and better readable
test as before. 

This packages provides following features:
* [Assert-Helpers](docu/assert-helper.md)
* [HttpClient Extensions](docu/http-client-extensions.md)

All the samples currently based on a base class:
!! This code is currently not inlcuded in this package !!
```csharp
[TestClass]
public class MyTestClass : MsTestBase
{
    // your tests....
}

[TestClass]
public abstract class MsTestBase
{
    // In this sample we currently use assembley intialize, to save performance, but you can do it also different.
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        // Create this with new, is not a fault, the reason is to keep the test class more cleaner.
        CustomWebApplicationFactory = new CustomWebApplicationFactory();
        ServiceProvider = CustomWebApplicationFactory.Services;
        Client = CustomWebApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });        
    }

    internal static CustomWebApplicationFactory CustomWebApplicationFactory { get; set; } = null!;

    internal static IServiceProvider ServiceProvider { get; private set; } = null!;    

    protected static HttpClient Client { get; private set; } = null!;

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        CustomWebApplicationFactory?.Dispose();
        Client?.Dispose();
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testAppsettingsJson = Path.Combine(Environment.CurrentDirectory, "appsettings.test.json");

        builder.ConfigureAppConfiguration((_, configurationBuilder) => configurationBuilder.AddJsonFile(testAppsettingsJson));

        builder.ConfigureServices(services =>
        {
            // if we need to switch between services we have to do it here
        });
    }
}
```
