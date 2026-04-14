using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi.Test
{
    [TestClass]
    public abstract class ApiTestBase
    {
        private static ApiTestBase<Program> _apiTestBase = null!;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // 1. Super simple just use the provided API test base class and you are ready to go
            _apiTestBase = new ApiTestBase<Program>("Development", // The environment name
                                                    (services,
                                                     configuration) =>
                                                    {
                                                        services.AddAssertableHttpClient(configuration);
                                                    }, // The register services action
                                                    []); // Configure environment variables

            // 2. We need once the http client to communicate with the started api
            Client = _apiTestBase.CreateClient();

            // NEW: Initialize the HttpClientAssertExtensions with the service provider to enable assertion capabilities in your tests
            HttpClientAssertExtensions.Setup(_apiTestBase.Services);
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
