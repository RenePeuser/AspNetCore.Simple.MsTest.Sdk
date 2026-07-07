using System.Net.Http;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            // Use Program or Startup as entry point for proper WebApplicationFactory support
            // - Program: for minimal API / top-level statements (Program.cs)
            // - Startup: for traditional Startup.cs class
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

            AssertObjectExtensions.DifferenceFunc = TestHelpers.IgnoreIdDifferences;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase.Dispose();
            Client.Dispose();
        }
    }
}