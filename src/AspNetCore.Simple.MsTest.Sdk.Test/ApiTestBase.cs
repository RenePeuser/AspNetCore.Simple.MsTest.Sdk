using System.Net.Http;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    /// <summary>
    ///     Your base class for all API tests
    /// </summary>
    [TestClass]
    public abstract class ApiTestBase
    {
        private static ApiTestBase<Startup> _apiTestBase = null!;

        protected static HttpClient Client { get; private set; } = null!;

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
                                                    (_,
                                                     _) =>
                                                    {
                                                    }); // Configure environment variables  

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
