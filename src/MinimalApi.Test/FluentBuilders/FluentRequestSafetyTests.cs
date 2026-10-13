using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for request details that used to be sent differently than the test line reads:
    ///
    /// <list type="bullet">
    /// <item><c>WithParameter("Amount", 1.5m)</c> was formatted with the CURRENT culture - a German machine sent
    /// <c>amount=1,5</c> in the url and the invalid json <c>{ "name": 1,5 }</c> in a body.</item>
    /// <item><c>WithHeader("Content-Type", …)</c> was dropped silently: HttpClient keeps content headers on the
    /// content and refuses them on the request. An invalid header name was dropped the same way.</item>
    /// <item>An empty parameter or header name was accepted and then skipped - its value was never sent.</item>
    /// </list>
    ///
    /// Every positive case is paired with a test that MUST fail.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentRequestSafetyTests : ApiTestBase
    {
        private const string EchoQueryUrl = "api/v1/persons/echo-query";

        private const string EchoRequestUrl = "api/v1/persons/echo-request";

        private const string EchoHeaderUrl = "api/v1/persons/echo-header";

        private const string CultureFix = "Check ParameterReplacer.Format: placeholder values are formatted with CultureInfo.InvariantCulture in url, body and expected json.";

        private const string ContentHeaderFix = "Check HttpRequestMessageBuilder.AddRequestHeaders: a header the request headers refuse goes onto the content (replacing its value), and one neither side accepts throws.";

        private const string EmptyNameFix = "Check HttpRequestBuilder.AddParameters / AddHeader: a null or blank name throws ArgumentException at the line that gives it.";

        // The test host runs in globalization-invariant mode (no "de-DE"), so a German-style culture is built by hand.
        private static async Task InGermanCulture(Func<Task> test)
        {
            var german = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            german.NumberFormat.NumberDecimalSeparator = ",";
            german.NumberFormat.NumberGroupSeparator = ".";

            var culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = german;

            try
            {
                await test().ConfigureAwait(false);
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }

        private static Task<EchoQueryResponse> QueryWithAmountAndRatio(EchoQueryResponse expected)
        {
            return Client.AssertGet(EchoQueryUrl + "?amount=$Amount$&ratio=$Ratio$")
                         .WithParameter("Amount", 1.5m)
                         .WithParameter("Ratio", 0.25d)
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(expected)
                         .ExecuteAsync();
        }

        private static Task<EchoRequestResponse> PostWithAmount(EchoRequestResponse expected)
        {
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromJsonString("""{ "name": "$Amount$" }""")
                         .WithParameter("Amount", 1.5m)
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(expected)
                         .ExecuteAsync();
        }

        private static EchoQueryResponse AmountAndRatio(string amount,
                                                        string ratio)
        {
            return new EchoQueryResponse($"?amount={amount}&ratio={ratio}",
                                         new Dictionary<string, string[]> { ["amount"] = [amount], ["ratio"] = [ratio] });
        }

        // ============================================================
        // Placeholder values — culture-invariant.
        // ============================================================

        [TestMethod]
        public Task Url_Placeholder_Should_Be_Formatted_Culture_Invariant()
        {
            return InGermanCulture(() => QueryWithAmountAndRatio(AmountAndRatio("1.5", "0.25")));
        }

        [TestMethod]
        public async Task Url_Placeholder_Expecting_The_Decimal_Comma_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => InGermanCulture(() => QueryWithAmountAndRatio(AmountAndRatio("1,5", "0,25"))),
                                                                        because: "The url carries 1.5 whatever the machine's culture - expecting the German 1,5 has to fail, or the culture still leaks into the url.",
                                                                        fix: CultureFix).ConfigureAwait(false);
        }

        [TestMethod]
        public Task Body_Placeholder_Should_Be_Formatted_Culture_Invariant()
        {
            return InGermanCulture(() => PostWithAmount(new EchoRequestResponse("(absent)", "1.5")));
        }

        [TestMethod]
        public async Task Body_Placeholder_Expecting_The_Decimal_Comma_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => InGermanCulture(() => PostWithAmount(new EchoRequestResponse("(absent)", "1,5"))),
                                                                        because: "The body carries 1.5 whatever the machine's culture - expecting the German 1,5 has to fail.",
                                                                        fix: CultureFix).ConfigureAwait(false);
        }

        // ============================================================
        // Content headers — sent on the content, never dropped.
        // ============================================================

        [TestMethod]
        public Task Content_Type_Header_Should_Reach_The_Endpoint()
        {
            // text/plain is not json - the endpoint answering 415 is the proof the header was sent.
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromJsonString("""{ "name": "Goku" }""")
                         .WithHeader("Content-Type", "text/plain")
                         .Produces(HttpStatusCode.UnsupportedMediaType)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Content_Type_Header_Expecting_Success_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoRequestUrl)
                                                                                    .AcceptsFromJsonString("""{ "name": "Goku" }""")
                                                                                    .WithHeader("Content-Type", "text/plain")
                                                                                    .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoRequestResponse("(absent)", "Goku"))
                                                                                    .ExecuteAsync(),
                                                                        because: "A 200 means the text/plain Content-Type was dropped and the default json one was sent instead.",
                                                                        fix: ContentHeaderFix).ConfigureAwait(false);
        }

        [TestMethod]
        public Task Content_Type_Header_Should_Replace_The_Default_Not_Add_To_It()
        {
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromJsonString("""{ "name": "Goku" }""")
                         .WithHeader("content-type", "application/json")
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoRequestResponse("(absent)", "Goku"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Header_HttpClient_Refuses_Should_Fail_Instead_Of_Being_Dropped()
        {
            // The engine reports it like every other failure of the call - as the sdk's assertion output.
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoHeaderUrl)
                                                                                                         .WithHeader("X Correlation Id", "abc")
                                                                                                         .Produces(HttpStatusCode.OK)
                                                                                                         .ExecuteAsync(),
                                                                                             because: "A name with spaces is no valid header - the request went out without it while the test read as if it was sent.",
                                                                                             fix: ContentHeaderFix).ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "X Correlation Id",
                                 because: "The output has to name the header that cannot be sent.",
                                 fix: ContentHeaderFix);
        }

        // ============================================================
        // Empty names — rejected at the line that gives them.
        // ============================================================

        [TestMethod]
        [DataRow("", DisplayName = "empty")]
        [DataRow("  ", DisplayName = "blank")]
        public void Blank_Parameter_Name_Should_Throw(string name)
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoQueryUrl).WithParameter(name, "Vegeta"),
                                                         because: "A blank name is no placeholder - the engine skipped it and the value was never sent.",
                                                         fix: EmptyNameFix);

            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertPost(EchoRequestUrl).WithParameters((name, "Vegeta")),
                                                         because: "The body stage and the tuple overload share the same rule.",
                                                         fix: EmptyNameFix);
        }

        [TestMethod]
        public void Null_Parameter_Name_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentNullException>(() => Client.AssertGet(EchoQueryUrl).WithParameters((null!, "Vegeta")),
                                                             because: "A null name is no placeholder either.",
                                                             fix: EmptyNameFix);
        }

        [TestMethod]
        [DataRow("", DisplayName = "empty")]
        [DataRow("  ", DisplayName = "blank")]
        public void Blank_Header_Name_Should_Throw(string name)
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoHeaderUrl).WithHeader(name, "value"),
                                                         because: "A blank header name cannot be sent - HttpClient would refuse it while the test reads as if it went out.",
                                                         fix: EmptyNameFix);
        }

        [TestMethod]
        public Task Valid_Names_Should_Still_Be_Sent()
        {
            return Client.AssertGet(EchoHeaderUrl)
                         .WithHeader("X-Correlation-Id", "abc")
                         .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoHeaderResponse("abc"))
                         .ExecuteAsync();
        }
    }
}
