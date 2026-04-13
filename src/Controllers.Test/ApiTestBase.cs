using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.DependencyInjection;
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]

namespace Controllers.Test
{
    /// <summary>
    ///     Your base class for all API tests
    /// </summary>
    [TestClass]
    public abstract class ApiTestBase
    {
        private static ApiTestBase<Startup> _apiTestBase = null!;

        protected static HttpClient Client { get; private set; } = null!;

        protected static IAssertableHttpClient AssertableHttpClient { get; private set; } = null!;

        /// <summary>
        ///     Initializes the test assembly by setting up the API test environment.
        ///     Import this happens one time before all tests are running. This is
        ///     like your prod case. Because your API is running continuously.
        /// </summary>
        /// <param name="_">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // 1. Super simple just use the provided API test base class and you are ready to go
            _apiTestBase = new ApiTestBase<Startup>("Development", // The environment name
                                                    (services,
                                                     configuration) =>
                                                    {
                                                        services.AddAssertableHttpClient(configuration);
                                                    }); // Configure environment variables

            Client = _apiTestBase.CreateClient();
            AssertableHttpClient = _apiTestBase.Services.GetRequiredService<IAssertableHttpClient>();

            // NEW: Initialize the HttpClientAssertExtensions with the service provider to enable assertion capabilities in your tests
            HttpClientAssertExtensions.Setup(_apiTestBase.Services);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase.Dispose();
            Client.Dispose();
        }
    }
}
