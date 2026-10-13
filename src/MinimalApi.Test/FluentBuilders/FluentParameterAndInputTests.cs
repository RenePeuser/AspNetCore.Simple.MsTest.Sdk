using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for inputs a fluent chain used to accept silently while sending or checking something else:
    ///
    /// <list type="bullet">
    /// <item>A placeholder value with reserved characters was put into the url unencoded - <c>Son Goku &amp; Co</c>
    /// split the query.</item>
    /// <item>A parameter no placeholder takes (<c>$NAME$</c> vs. <c>"Name"</c>, or a placeholder inside
    /// <c>WithQueryParameter</c>, which sends its value literally) was never sent.</item>
    /// <item>A name carrying the <c>$</c> delimiter (<c>"$"</c>, <c>"$Name"</c>) named a token nothing contains.</item>
    /// <item>A blank body, a blank expected response and an empty <c>IgnorePaths()</c> were accepted - the blank
    /// expectation went green without comparing anything.</item>
    /// </list>
    ///
    /// Plus the behavior that stays and is pinned here: <c>Accepts(null)</c> sends the json <c>null</c>, and a typed
    /// <c>"{}"</c> expectation still compares. Every positive case is paired with a test that MUST fail.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentParameterAndInputTests : ApiTestBase
    {
        private const string EchoQueryUrl = "api/v1/persons/echo-query";

        private const string EchoRequestUrl = "api/v1/persons/echo-request";

        private const string EchoHeaderUrl = "api/v1/persons/echo-header";

        private const string ProfileUrl = "api/v1/persons/1/profile";

        private const string Reserved = "Son Goku & Co+1/2?=#%";

        private const string ReservedEncoded = "Son%20Goku%20%26%20Co%2B1%2F2%3F%3D%23%25";

        private const string EncodingFix = "Check that the fluent builders pass fluentParameters: true and ParameterReplacer.ReplaceInUrl(url, encodeValues: true, …) URL-encodes every value.";

        private const string UnusedFix = "Check HttpClientAssertExtensions.EnsureEveryParameterIsUsed: a parameter whose placeholder is in neither url, body nor expected throws FluentUsageError.UnusedParameters (fluent chains only, not while recording).";

        private const string NameFix = "Check PlaceholderName.Wrap: a name carrying the delimiter anywhere but as a full wrap throws ArgumentException.";

        private const string BlankFix = "Check the Throw.IfNullOrWhiteSpace / Throw.IfNullOrEmpty guards in Accepts…, ExpectedResponse…, ExpectedHeaders… and IgnorePaths.";

        private static EchoQueryResponse Echo(string rawQuery,
                                              string key,
                                              params string[] values)
        {
            return new EchoQueryResponse(rawQuery, new Dictionary<string, string[]> { [key] = values });
        }

        private static PersonProfile Profile()
        {
            return new PersonProfile(Id: 1,
                                     Name: "Son Goku",
                                     Address: new ProfileAddress("Home", "Kame House 1", "12345"),
                                     Contacts: ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                    new ProfileContact("Private", "goku@kame.house")));
        }

        // ============================================================
        // Placeholder values in the url are URL-encoded.
        // ============================================================

        [TestMethod]
        public Task Url_Placeholder_Value_Should_Be_Encoded_And_Arrive_Unchanged()
        {
            return Client.AssertGet(EchoQueryUrl + "?name=$Name$")
                         .WithParameter("Name", Reserved)
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?name=" + ReservedEncoded, "name", Reserved))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Url_Placeholder_Value_Expecting_The_Split_Query_Should_Fail()
        {
            // What the unencoded value did: '&' ends the value, '#' cuts off the rest.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl + "?name=$Name$")
                                                                                    .WithParameter("Name", Reserved)
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?name=Son%20Goku%20&%20Co+1/2?=", "name", "Son Goku "))
                                                                                    .ExecuteAsync(),
                                                                        because: "The whole value arrives - expecting the split one has to fail, or the value still goes into the url unencoded.",
                                                                        fix: EncodingFix).ConfigureAwait(false);
        }

        // ============================================================
        // Every parameter has to be taken by a placeholder.
        // ============================================================

        [TestMethod]
        public Task Parameter_Taken_By_The_Url_Should_Pass()
        {
            return Client.AssertGet(EchoQueryUrl + "?name=$Name$")
                         .WithParameter("Name", "Goku")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?name=Goku", "name", "Goku"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Parameter_Taken_By_The_Body_Should_Pass()
        {
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromJsonString("""{ "name": "$Name$" }""")
                         .WithParameter("Name", "Goku")
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoRequestResponse("(absent)", "Goku"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Parameter_Taken_By_The_Expected_Response_Only_Should_Pass()
        {
            return Client.AssertGet(EchoHeaderUrl)
                         .WithHeader("X-Correlation-Id", "abc")
                         .WithParameter("CorrelationId", "abc")
                         .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                         .ExpectedResponseFromJsonString("""{ "correlationId": "$CorrelationId$" }""")
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Parameter_Taken_By_The_Expected_Response_With_Another_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoHeaderUrl)
                                                                                    .WithHeader("X-Correlation-Id", "abc")
                                                                                    .WithParameter("CorrelationId", "other")
                                                                                    .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponseFromJsonString("""{ "correlationId": "$CorrelationId$" }""")
                                                                                    .ExecuteAsync(),
                                                                        because: "The placeholder in the expected response IS used - its value has to be compared, not skipped.",
                                                                        fix: UnusedFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Parameter_With_Other_Casing_Than_The_Placeholder_Should_Throw_With_Guidance()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<ArgumentException>(() => Client.AssertGet(EchoQueryUrl + "?name=$NAME$")
                                                                                                 .WithParameter("Name", "Goku")
                                                                                                 .Produces(HttpStatusCode.OK)
                                                                                                 .ExecuteAsync(),
                                                                                     because: "Placeholders are case-sensitive - $Name$ is in no url, body or expected, so 'Goku' would never be sent.",
                                                                                     fix: UnusedFix).ConfigureAwait(false);

            AssertGuidance(exception.Message, "PLACEHOLDER PARAMETER NOT USED", "$Name$", "\"Goku\"", "case-sensitive");
        }

        [TestMethod]
        public async Task Placeholder_Inside_A_Query_Value_Should_Throw_With_Guidance()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<ArgumentException>(() => Client.AssertGet(EchoQueryUrl)
                                                                                                 .WithQueryParameter("name", "$Name$")
                                                                                                 .WithParameter("Name", "Goku")
                                                                                                 .Produces(HttpStatusCode.OK)
                                                                                                 .ExecuteAsync(),
                                                                                     because: "WithQueryParameter sends its value literally - the parameter fills nothing and 'Goku' would never be sent.",
                                                                                     fix: UnusedFix).ConfigureAwait(false);

            AssertGuidance(exception.Message, "PLACEHOLDER PARAMETER NOT USED", "WithQueryParameter", ".WithQueryParameter(\"name\", \"Goku\")");
        }

        [TestMethod]
        public Task Query_Value_Should_Be_Sent_Literally()
        {
            return Client.AssertGet(EchoQueryUrl)
                         .WithQueryParameter("price", "$5")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?price=%245", "price", "$5"))
                         .ExecuteAsync();
        }

        // ============================================================
        // Names are naked - the sdk owns the delimiter.
        // ============================================================

        [TestMethod]
        [DataRow("$", DisplayName = "delimiter only")]
        [DataRow("$$", DisplayName = "empty wrap")]
        [DataRow("$Name", DisplayName = "half wrapped")]
        [DataRow("Na$me", DisplayName = "delimiter inside")]
        [DataRow("$Na$me$", DisplayName = "delimiter inside a wrap")]
        public void Name_With_The_Delimiter_Should_Throw(string name)
        {
            var exception = Assert.That.Throws<ArgumentException>(() => Client.AssertGet(EchoQueryUrl + "?name=$Name$").WithParameter(name, "Goku"),
                                                                  because: "A name carrying '$' wraps to a token no url, body or snapshot contains.",
                                                                  fix: NameFix);

            Assert.That.Contains(exception.Message,
                                 "WithParameter(\"Id\", 1)",
                                 because: "The message has to show the naked spelling.",
                                 fix: NameFix);
        }

        [TestMethod]
        public Task Legacy_Wrapped_Name_Should_Still_Work()
        {
            return Client.AssertGet(EchoQueryUrl + "?name=$Name$")
                         .WithParameter("$Name$", "Goku")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?name=Goku", "name", "Goku"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Legacy_Wrapped_Name_Expecting_Another_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl + "?name=$Name$")
                                                                                    .WithParameter("$Name$", "Goku")
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?name=Vegeta", "name", "Vegeta"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The fully wrapped migration spelling fills $Name$ - expecting another value has to fail.",
                                                                        fix: NameFix).ConfigureAwait(false);
        }

        // ============================================================
        // Blank inputs are rejected.
        // ============================================================

        [TestMethod]
        [DataRow("", DisplayName = "empty")]
        [DataRow("   ", DisplayName = "blank")]
        public void Blank_Body_Should_Throw(string body)
        {
            Assert.That.Throws<ArgumentException>(() => Client.AssertPost(EchoRequestUrl).AcceptsFromJsonString(body),
                                                  because: "A blank body is no body - the engine sent none while the line reads as if it set one.",
                                                  fix: BlankFix);

            Assert.That.Throws<ArgumentException>(() => Client.AssertPost(EchoRequestUrl).AcceptsFromEmbeddedJson(body),
                                                  because: "A blank file name names no file.",
                                                  fix: BlankFix);
        }

        [TestMethod]
        [DataRow("", DisplayName = "empty")]
        [DataRow("   ", DisplayName = "blank")]
        public void Blank_Expected_Response_Should_Throw(string expected)
        {
            Assert.That.Throws<ArgumentException>(() => Client.AssertGet(ProfileUrl).Produces<PersonProfile>(HttpStatusCode.OK).ExpectedResponseFromJsonString(expected),
                                                  because: "A blank expectation compared nothing - the chain went green without looking at the response.",
                                                  fix: BlankFix);

            Assert.That.Throws<ArgumentException>(() => Client.AssertGet(ProfileUrl).Produces<PersonProfile>(HttpStatusCode.OK).ExpectedResponseFromEmbeddedJson(expected),
                                                  because: "A blank file name names no snapshot.",
                                                  fix: BlankFix);

            Assert.That.Throws<ArgumentException>(() => Client.AssertOptions(EchoRequestUrl).ExpectedHeadersFromJsonString(expected),
                                                  because: "A blank OPTIONS expectation is no expectation either.",
                                                  fix: BlankFix);

            Assert.That.Throws<ArgumentException>(() => Client.AssertOptions(EchoRequestUrl).ExpectedHeadersFromEmbeddedJson(expected),
                                                  because: "A blank file name names no file.",
                                                  fix: BlankFix);
        }

        [TestMethod]
        public void Empty_Ignore_Paths_Should_Throw()
        {
            Assert.That.Throws<ArgumentException>(() => Client.AssertGet(ProfileUrl).Produces<PersonProfile>(HttpStatusCode.OK).ExpectedResponse(Profile()).IgnorePaths(),
                                                  because: "IgnorePaths() without a path ignores nothing - a leftover would read as if it ignored something.",
                                                  fix: BlankFix);

            Assert.That.Throws<ArgumentException>(() => Client.AssertGet(ProfileUrl).Produces<PersonProfile>(HttpStatusCode.OK).ExpectedResponse(Profile()).IgnorePaths(null!),
                                                  because: "null is no path list.",
                                                  fix: BlankFix);
        }

        [TestMethod]
        public Task Real_Expected_Response_Should_Still_Pass()
        {
            return Client.AssertGet(ProfileUrl)
                         .Produces<PersonProfile>(HttpStatusCode.OK)
                         .ExpectedResponse(Profile())
                         .IgnorePaths("contacts[*].type")
                         .ExecuteAsync();
        }

        // ============================================================
        // Pinned behavior that stays.
        // ============================================================

        [TestMethod]
        public Task Accepts_Null_Should_Send_The_Json_Null()
        {
            // The sample api maps the missing body (BadHttpRequestException) to 500 - what matters is that "null" went out.
            return Client.AssertPost(EchoRequestUrl)
                         .Accepts<EchoRequestBody?>(null)
                         .Produces(HttpStatusCode.InternalServerError)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Accepts_Null_Expecting_Success_Should_Fail_And_Show_The_Null_Body()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoRequestUrl)
                                                                                                    .Accepts<EchoRequestBody?>(null)
                                                                                                    .Produces(HttpStatusCode.OK)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "A null body is no person - the endpoint cannot answer 200.",
                                                                                        fix: "Check HttpRequestBuilder.Accepts<T>: null is serialized to the json null and sent as the body.").ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "--data-raw 'null'",
                                 because: "The curl in the failure output proves the json null was sent.",
                                 fix: "Check HttpRequestBuilder.Accepts<T>.");
        }

        [TestMethod]
        public async Task Typed_Empty_Object_Expectation_Should_Still_Compare()
        {
            // "{}" switches the engine to its C# generator mode for recording - the comparison must run anyway.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                    .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                    .ExpectedResponseFromJsonString("{}")
                                                                                    .ExecuteAsync(),
                                                                        because: "The profile has properties - an empty expectation has to fail, not go green.",
                                                                        fix: "Check AssertService: IsEmptyAnonymousObjectForCodeGeneration only chooses the write mode, the differences still fail the test.").ConfigureAwait(false);
        }

        private static void AssertGuidance(string message,
                                           params string[] expectedParts)
        {
            foreach (var part in expectedParts)
            {
                Assert.That.Contains(message,
                                     part,
                                     because: "The usage error has to say which parameter is unused, with its value, why and how to fix it.",
                                     fix: "Check FluentUsageError.UnusedParameters.");
            }

            foreach (var section in new[] { "What Happened", "Why This Fails", "How To Fix", "Test Information" })
            {
                Assert.That.Contains(message,
                                     section,
                                     because: "Every usage error carries the same sections as the sdk's assertion output.",
                                     fix: "Check FluentUsageError.Build.");
            }
        }
    }
}
