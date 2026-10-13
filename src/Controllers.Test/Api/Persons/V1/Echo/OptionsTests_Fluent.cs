using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Echo
{
    /// <summary>
    /// Controller twin of <c>MinimalApi.Test.FluentBuilders.FluentOptionsTests</c>: the fluent OPTIONS chain against an
    /// <c>[HttpOptions]</c> action (<c>OPTIONS persons/echo-request</c>: 204, <c>Allow</c> as a CONTENT header and
    /// <c>Access-Control-Allow-Methods</c> as a RESPONSE header, both <c>POST, OPTIONS</c>). Unlike the MinimalApi
    /// sample, the controllers host does not report <c>api-supported-versions</c>.
    ///
    /// <para>Every positive case is paired with a test that MUST fail.</para>
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Fluent")]
    public sealed class OptionsTests : ApiTestBase
    {
        private const string OptionsUrl = "api/v1/persons/echo-request";

        private const string CorrectHeaders = """{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ] }""";

        private const string WrongMethods = """{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "GET" ] }""";

        private const string WithoutAllow = """{ "Access-Control-Allow-Methods": [ "POST, OPTIONS" ] }""";

        private const string CompareFix = "Check that AssertOptions(...).ExecuteAsync() compares the response AND content headers (OptionsHeaders.cs) against ExpectedHeaders….";

        [TestMethod]
        public Task Options_Should_Advertise_The_Allowed_Verbs()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromJsonString(CorrectHeaders)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Options_Should_Accept_Header_Names_In_Any_Casing()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
                                                                             .Add("allow", "POST, OPTIONS")
                                                                             .Add("ACCESS-CONTROL-ALLOW-METHODS", "POST, OPTIONS"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Options_Expecting_Other_Verbs_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString(WrongMethods)
                                                                                    .ExecuteAsync(),
                                                                        because: "The action advertises 'POST, OPTIONS' - expecting 'GET' has to fail, or the response headers are not compared at all.",
                                                                        fix: CompareFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Options_Without_The_Allow_Content_Header_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString(WithoutAllow)
                                                                                    .ExecuteAsync(),
                                                                        because: "Allow is a CONTENT header in HttpClient - the comparison is exact, so leaving it out has to fail, or content headers are not read.",
                                                                        fix: CompareFix).ConfigureAwait(false);
        }
    }
}
