using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Probes
{
    /// <summary>
    /// Probe G5 (NEXT_STEPS "Test gaps"): does <c>services.AddTestCreator(…)</c> + <c>app.UseTestCreator()</c> still
    /// generate a test from live traffic, as the root README advertises?
    ///
    /// <para>
    /// Runs its own host with the test creator wired in - and WITHOUT <c>AddAssertableHttpClient</c>, so it never
    /// takes over the static assert provider the other tests of this assembly resolve from.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Probe")]
    public sealed class TestCreatorProbeTests
    {
        private static readonly ConcurrentQueue<string> Generated = new();

        private static WebApplicationFactory<Startup> _baseFactory = null!;

        private static WebApplicationFactory<Startup> _factory = null!;

        private static HttpClient _client = null!;

        private const string CreatorFix = "Check TestCreatorMiddleware (request/response capture) and PostWithBodyTestCreator (template) - the generated text goes to TestSdkSettings.LogAction.";

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            _baseFactory = new WebApplicationFactory<Startup>();
            _factory = _baseFactory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")
                                                                                             .ConfigureServices((context,
                                                                                                                 services) =>
                                                                                                                {
                                                                                                                    services.AddTestCreator(context.Configuration, Generated.Enqueue);
                                                                                                                    services.AddTransient<IStartupFilter, UseTestCreatorStartupFilter>();
                                                                                                                }));
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
            _baseFactory.Dispose();
        }

        [TestMethod]
        public async Task Get_Should_Generate_A_Classic_Test()
        {
            using var response = await _client.GetAsync(new Uri("api/v1/persons/1", UriKind.Relative)).ConfigureAwait(false);

            var generated = Find("/api/v1/persons/1");

            Assert.That.Contains(generated,
                                 "Client.AssertGetAsync<Person>(\"/api/v1/persons/1\"",
                                 because: "A GET with a typed 200 has to produce a ready-to-paste classic test for exactly this url and type.",
                                 fix: CreatorFix);
        }

        [TestMethod]
        public async Task Post_Should_Generate_A_Test_With_Payload()
        {
            using var response = await _client.PostAsJsonAsync(new Uri("api/v1/persons/echo-request", UriKind.Relative),
                                                               new EchoRequestBody("Goku")).ConfigureAwait(false);

            var generated = Find("/api/v1/persons/echo-request");

            foreach (var part in new[] { "Client.AssertPostAsync<EchoRequestResponse>(\"/api/v1/persons/echo-request\"", "\"Payload\"", "\"Goku\"" })
            {
                Assert.That.Contains(generated,
                                     part,
                                     because: "A POST with a body has to produce a test with payload and response and show the captured body.",
                                     fix: CreatorFix);
            }
        }

        [TestMethod]
        public async Task Not_Found_Should_Generate_An_Error_Test()
        {
            using var response = await _client.GetAsync(new Uri("api/v1/persons/999", UriKind.Relative)).ConfigureAwait(false);

            var generated = Find("/api/v1/persons/999");

            Assert.That.Contains(generated,
                                 "Client.AssertGetAsErrorAsync<ProblemDetails>(\"/api/v1/persons/999\"",
                                 because: "A 404 with a declared ProblemDetails has to produce an error test, not a success test.",
                                 fix: CreatorFix);
        }

        private static string Find(string path)
        {
            return Generated.LastOrDefault(text => text.Contains($" {path}", StringComparison.Ordinal) ||
                                                   text.Contains($"/{path.TrimStart('/')}{Environment.NewLine}", StringComparison.Ordinal)) ??
                   $"(nothing generated for {path}; {Generated.Count} outputs in total)";
        }

        private sealed class UseTestCreatorStartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return app =>
                {
                    app.UseTestCreator();
                    next(app);
                };
            }
        }
    }
}
