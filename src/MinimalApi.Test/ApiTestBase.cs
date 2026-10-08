using System;
using System.Net.Http;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test
{
    [TestClass]
    public abstract class ApiTestBase
    {
        private static ApiTestBase<Program> _apiTestBase = null!;

        protected static IServiceCollection ServiceCollection { get; private set; } = null!;

        /// <summary>
        ///     The host's provider - the very container the static asserts resolve from.
        /// </summary>
        protected static IServiceProvider Services => _apiTestBase.Services;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // Use Program or Startup as entry point for proper WebApplicationFactory support
            // - Program: for minimal API / top-level statements (Program.cs)
            // - Startup: for traditional Startup.cs class
            _apiTestBase = new ApiTestBase<Program>("Development",
                                                    (services,
                                                     configuration) =>
                                                    {
                                                        ServiceCollection = services;

                                                        services.AddAssertableHttpClient(configuration,
                                                                                         settings =>
                                                                                         {
                                                                                             settings.DifferenceFunc = TestHelpers.IgnoreIdDifferences;
                                                                                         });
                                                    });

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