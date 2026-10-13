using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for the fluent OPTIONS chain against the positive sample <c>OPTIONS persons/echo-request</c>
    /// (204, <c>Allow</c> + <c>Access-Control-Allow-Methods: POST, OPTIONS</c>; <c>api-supported-versions: 1</c> comes
    /// from API versioning). Header names, content headers and the three header sources: <c>OptionsHeaderTests</c>.
    ///
    /// <para>
    /// <see cref="IHttpOptionsConfiguring"/>'s builder used to mutate itself and return <c>this</c> from
    /// <c>ExpectedHeaders…</c> - the one builder the copy-on-write change had missed. A stored
    /// <c>AssertOptions(url)</c> stage branched into two expectations therefore compared BOTH branches against
    /// whichever expectation was set last.
    /// </para>
    ///
    /// <para>Every positive case is paired with a test that MUST fail.</para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentOptionsTests : ApiTestBase
    {
        private const string OptionsUrl = "api/v1/persons/echo-request";

        private const string ImmutabilityFix = "Check that HttpOptionsBuilder.ExpectedHeaders… returns a NEW builder (copy-on-write) instead of mutating and returning this.";

        // All three ExpectedHeaders… overloads share the copy-on-write this class proves; raw JSON keeps it short.
        private const string CorrectHeaders = """{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""";

        private const string WrongHeaders = """{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "GET" ], "api-supported-versions": [ "1" ] }""";

        [TestMethod]
        public Task Options_Should_Advertise_The_Allowed_Verbs()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromJsonString(CorrectHeaders)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Options_Expecting_Other_Verbs_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString(WrongHeaders)
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint advertises 'POST, OPTIONS' - expecting 'GET' has to fail, or the headers are not compared at all.",
                                                                        fix: "Check that AssertOptions(...).ExecuteAsync() compares the response headers against ExpectedHeaders….").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Options_Branch_Should_Keep_Its_Own_Expectation()
        {
            var baseRequest = Client.AssertOptions(OptionsUrl);

            var correct = baseRequest.ExpectedHeadersFromJsonString(CorrectHeaders);

            _ = baseRequest.ExpectedHeadersFromJsonString(WrongHeaders);

            await correct.ExecuteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Options_Branch_Should_Compare_Its_Own_Wrong_Expectation()
        {
            var baseRequest = Client.AssertOptions(OptionsUrl);

            var wrong = baseRequest.ExpectedHeadersFromJsonString(WrongHeaders);

            _ = baseRequest.ExpectedHeadersFromJsonString(CorrectHeaders);

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => wrong.ExecuteAsync(),
                                                                        because: "This branch expects 'GET' - it has to fail, no matter what a sibling branch of the same AssertOptions(url) expects.",
                                                                        fix: ImmutabilityFix).ConfigureAwait(false);
        }
    }
}
