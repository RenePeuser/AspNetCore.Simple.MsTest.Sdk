using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.FluentBuilders.OptionsHeaders
{
    /// <summary>
    /// Consumer tests for the header comparison of OPTIONS - fluent AND classic, they share one engine - against
    /// <c>OPTIONS persons/echo-request</c>: 204, <c>Allow: POST, OPTIONS</c> (a CONTENT header in HttpClient),
    /// <c>Access-Control-Allow-Methods: POST, OPTIONS</c> and <c>api-supported-versions: 1</c> (response headers).
    ///
    /// <para>Two engine fixes are proven here:</para>
    /// <list type="bullet">
    /// <item><c>Allow</c> was never compared: only <c>HttpResponseMessage.Headers</c> was read, never
    /// <c>Content.Headers</c>.</item>
    /// <item>Header names were compared case-sensitively, and the dictionary overloads ran them through the app's
    /// camelCase key policy (<c>Access-Control-Allow-Methods</c> → <c>access-Control-Allow-Methods</c>) - a
    /// dictionary expectation of a mixed-case header could never pass.</item>
    /// </list>
    ///
    /// <para>Every positive case is paired with a test that MUST fail.</para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("OPTIONS")]
    public sealed class OptionsHeaderTests : ApiTestBase
    {
        private const string OptionsUrl = "api/v1/persons/echo-request";

        private const string AllowedMethods = "POST, OPTIONS";

        private const string ContentHeaderFix = "Check that the OPTIONS assertion compares the CONTENT headers (Allow, Content-*) together with the response headers.";

        private const string CaseFix = "Check that the OPTIONS assertion compares header NAMES case-insensitively and serializes expected dictionaries without a key policy.";

        private const string ValueFix = "Check that aligning the header names does not touch the header VALUES - only the name spelling may be adjusted.";

        // ============================================================
        // Content headers: Allow
        // ============================================================

        [TestMethod]
        public Task Fluent_Should_Compare_Allow_Content_Header()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromJsonString("""{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Classic_Should_Compare_Allow_Content_Header()
        {
            return Client.AssertOptionsAsync(OptionsUrl,
                                             """{ "Allow": [ "POST, OPTIONS" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""");
        }

        [TestMethod]
        public async Task Fluent_Expectation_Without_Allow_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString("""{ "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""")
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint sends Allow - an expectation that leaves it out has to fail, or Allow is not read at all.",
                                                                        fix: ContentHeaderFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Classic_Expectation_Without_Allow_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptionsAsync(OptionsUrl,
                                                                                                        """{ "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }"""),
                                                                        because: "The endpoint sends Allow - an expectation that leaves it out has to fail, or Allow is not read at all.",
                                                                        fix: ContentHeaderFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Fluent_Wrong_Allow_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString("""{ "Allow": [ "GET, DELETE" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""")
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint allows 'POST, OPTIONS' - expecting 'GET, DELETE' has to fail.",
                                                                        fix: ContentHeaderFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Classic_Wrong_Allow_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptionsAsync(OptionsUrl,
                                                                                                        ImmutableDictionary<string, string>.Empty
                                                                                                                                           .Add("Allow", "GET")
                                                                                                                                           .Add("Access-Control-Allow-Methods", AllowedMethods)
                                                                                                                                           .Add("api-supported-versions", "1")),
                                                                        because: "The endpoint allows 'POST, OPTIONS' - expecting 'GET' has to fail.",
                                                                        fix: ContentHeaderFix).ConfigureAwait(false);
        }

        // ============================================================
        // Header names: dictionary overloads (key policy) - fluent
        // ============================================================

        [TestMethod]
        public Task Fluent_Dictionary_With_Mixed_Case_Names_Should_Pass()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
                                                                             .Add("Allow", AllowedMethods)
                                                                             .Add("Access-Control-Allow-Methods", AllowedMethods)
                                                                             .Add("api-supported-versions", "1"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Fluent_Dictionary_Of_Lists_With_Mixed_Case_Names_Should_Pass()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeaders(ImmutableDictionary<string, ImmutableList<string>>.Empty
                                                                                            .Add("Allow", [AllowedMethods])
                                                                                            .Add("Access-Control-Allow-Methods", [AllowedMethods])
                                                                                            .Add("api-supported-versions", ["1"]))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_Dictionary_With_Wrong_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
                                                                                                                                        .Add("Allow", AllowedMethods)
                                                                                                                                        .Add("Access-Control-Allow-Methods", "GET")
                                                                                                                                        .Add("api-supported-versions", "1"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint advertises 'POST, OPTIONS' - a dictionary expecting 'GET' has to fail, the names now match.",
                                                                        fix: ValueFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Fluent_Dictionary_Missing_A_Header_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
                                                                                                                                        .Add("Allow", AllowedMethods)
                                                                                                                                        .Add("Access-Control-Allow-Methods", AllowedMethods))
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint also sends api-supported-versions - the comparison is exact, so leaving it out has to fail.",
                                                                        fix: CaseFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Fluent_Dictionary_Expecting_A_Header_The_Server_Does_Not_Send_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeaders(ImmutableDictionary<string, string>.Empty
                                                                                                                                        .Add("Allow", AllowedMethods)
                                                                                                                                        .Add("Access-Control-Allow-Methods", AllowedMethods)
                                                                                                                                        .Add("api-supported-versions", "1")
                                                                                                                                        .Add("Access-Control-Max-Age", "600"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The endpoint sends no Access-Control-Max-Age - expecting it has to fail.",
                                                                        fix: CaseFix).ConfigureAwait(false);
        }

        // ============================================================
        // Header names: dictionary overloads (key policy) - classic
        // ============================================================

        [TestMethod]
        public Task Classic_Dictionary_With_Mixed_Case_Names_Should_Pass()
        {
            return Client.AssertOptionsAsync(OptionsUrl,
                                             ImmutableDictionary<string, string>.Empty
                                                                                .Add("Allow", AllowedMethods)
                                                                                .Add("Access-Control-Allow-Methods", AllowedMethods)
                                                                                .Add("api-supported-versions", "1"));
        }

        [TestMethod]
        public Task Classic_Dictionary_Of_Lists_With_Mixed_Case_Names_Should_Pass()
        {
            return Client.AssertOptionsAsync(OptionsUrl,
                                             ImmutableDictionary<string, ImmutableList<string>>.Empty
                                                                                               .Add("Allow", [AllowedMethods])
                                                                                               .Add("Access-Control-Allow-Methods", [AllowedMethods])
                                                                                               .Add("api-supported-versions", ["1"]));
        }

        [TestMethod]
        public async Task Classic_Dictionary_Of_Lists_With_Wrong_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptionsAsync(OptionsUrl,
                                                                                                        ImmutableDictionary<string, ImmutableList<string>>.Empty
                                                                                                                                                          .Add("Allow", [AllowedMethods])
                                                                                                                                                          .Add("Access-Control-Allow-Methods", ["POST"])
                                                                                                                                                          .Add("api-supported-versions", ["1"])),
                                                                        because: "The endpoint advertises 'POST, OPTIONS' - expecting 'POST' only has to fail.",
                                                                        fix: ValueFix).ConfigureAwait(false);
        }

        // ============================================================
        // Header names: any spelling (raw json / embedded file)
        // ============================================================

        [TestMethod]
        public Task Fluent_Json_With_Lower_Case_Names_Should_Pass()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromJsonString("""{ "allow": [ "POST, OPTIONS" ], "access-control-allow-methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Fluent_Json_With_Upper_Case_Names_Should_Pass()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromJsonString("""{ "ALLOW": [ "POST, OPTIONS" ], "ACCESS-CONTROL-ALLOW-METHODS": [ "POST, OPTIONS" ], "API-SUPPORTED-VERSIONS": [ "1" ] }""")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Classic_Json_With_Lower_Case_Names_Should_Pass()
        {
            return Client.AssertOptionsAsync(OptionsUrl,
                                             """{ "allow": [ "POST, OPTIONS" ], "access-control-allow-methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""");
        }

        [TestMethod]
        public Task Fluent_Embedded_File_With_Other_Spelling_Should_Pass()
        {
            return Client.AssertOptions(OptionsUrl)
                         .ExpectedHeadersFromEmbeddedJson("EchoRequestOptionsHeaders.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_Json_With_Lower_Case_Name_And_Wrong_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                    .ExpectedHeadersFromJsonString("""{ "allow": [ "POST, OPTIONS" ], "access-control-allow-methods": [ "PUT" ], "api-supported-versions": [ "1" ] }""")
                                                                                    .ExecuteAsync(),
                                                                        because: "Ignoring the case of a NAME must never hide a wrong VALUE - expecting 'PUT' has to fail.",
                                                                        fix: ValueFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Fluent_Json_With_Lower_Case_Name_Shows_Only_The_Value_Difference()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                                    .ExpectedHeadersFromJsonString("""{ "allow": [ "POST, OPTIONS" ], "access-control-allow-methods": [ "PUT" ], "api-supported-versions": [ "1" ] }""")
                                                                                                    .ExecuteAsync(),
                                                                                    because: "Expecting 'PUT' has to fail.",
                                                                                    fix: ValueFix).ConfigureAwait(false);

            Assert.That.DoesNotContain(exception.Message,
                                       "MissingInFirst",
                                       "With the names aligned to the server's spelling the report shows the VALUE difference, not the same header as 'missing' twice.",
                                       CaseFix);
        }

        // ============================================================
        // Same header listed twice - no type can see that
        // ============================================================

        [TestMethod]
        public async Task Fluent_Json_Listing_A_Header_Twice_Should_Fail_With_Guidance()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptions(OptionsUrl)
                                                                                                    .ExpectedHeadersFromJsonString("""{ "Allow": [ "POST, OPTIONS" ], "allow": [ "GET" ], "Access-Control-Allow-Methods": [ "POST, OPTIONS" ], "api-supported-versions": [ "1" ] }""")
                                                                                                    .ExecuteAsync(),
                                                                                    because: "\"Allow\" and \"allow\" are ONE header - the comparison cannot know which expectation is meant.",
                                                                                    fix: "Check that the OPTIONS assertion rejects a header name listed twice in another spelling.").ConfigureAwait(false);

            AssertDuplicateGuidance(exception.Message, "\"Allow\" / \"allow\"");
        }

        [TestMethod]
        public async Task Classic_Dictionary_Listing_A_Header_Twice_Should_Fail_With_Guidance()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertOptionsAsync(OptionsUrl,
                                                                                                                        ImmutableDictionary<string, string>.Empty
                                                                                                                                                           .Add("Allow", AllowedMethods)
                                                                                                                                                           .Add("ALLOW", AllowedMethods)
                                                                                                                                                           .Add("Access-Control-Allow-Methods", AllowedMethods)
                                                                                                                                                           .Add("api-supported-versions", "1")),
                                                                                    because: "A case-sensitive dictionary can hold \"Allow\" and \"ALLOW\" - HTTP sees ONE header, so this has to fail.",
                                                                                    fix: "Check that the OPTIONS assertion rejects a header name listed twice in another spelling.").ConfigureAwait(false);

            AssertDuplicateGuidance(exception.Message, "\"ALLOW\"");
        }

        private static void AssertDuplicateGuidance(string message,
                                                    string expectedNames)
        {
            const string fix = "Check OptionsHeaders.FailOnDuplicateNames - the output has to name the header and show the fix.";

            Assert.That.Contains(message, "EXPECTED HEADER LISTED TWICE", "The failure has to say what went wrong.", fix);
            Assert.That.Contains(message, expectedNames, "The failure has to name the header spellings that collide.", fix);
            Assert.That.Contains(message, "List each header once", "The failure has to say how to fix it.", fix);
        }
    }
}
