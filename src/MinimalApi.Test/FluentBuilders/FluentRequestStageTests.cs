using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for the guided request stages: a body verb keeps its order free - parameters and headers
    /// before or after <c>Accepts…</c> - and every allowed order still sends body, header and parameter.
    /// The orders that are NOT allowed (GET/DELETE with a body, a second <c>Accepts…</c>) are proven not to
    /// compile in <see cref="FluentTypeStateCompileTests"/>.
    ///
    /// <para>
    /// Every positive case is paired with a test that MUST fail: it expects a value the request did not send,
    /// so the green test is green because the value really arrived.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentRequestStageTests : ApiTestBase
    {
        private const string EchoRequestUrl = "api/v1/persons/echo-request";

        private const string CorrelationIdHeader = "X-Correlation-Id";

        private const string PatchUrl = "api/v1/persons";

        private const string BodylessPostUrl = "api/v1/errors/not-implemented";

        private const string OrderFix = "Check that HttpRequestBuilder carries body, headers and parameters across the stage switch of Accepts… (IHttpBodyRequestConfiguring → IHttpRequestConfiguring) - the order of the calls must not matter.";

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }

        // ============================================================
        // Header before / after the body.
        // ============================================================

        [TestMethod]
        public Task Header_Before_Body_Should_Send_Both()
        {
            return Client.AssertPost(EchoRequestUrl)
                         .WithHeader(CorrelationIdHeader, "before")
                         .Accepts(new EchoRequestBody("Vegeta"))
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoRequestResponse("before", "Vegeta"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Header_Before_Body_Expecting_Another_Body_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoRequestUrl)
                                                                                    .WithHeader(CorrelationIdHeader, "before")
                                                                                    .Accepts(new EchoRequestBody("Vegeta"))
                                                                                    .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoRequestResponse("before", "Son"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The body sent Vegeta - expecting Son has to fail, or the body never reached the endpoint.",
                                                                        fix: OrderFix).ConfigureAwait(false);
        }

        [TestMethod]
        public Task Header_After_Body_Should_Send_Both()
        {
            return Client.AssertPost(EchoRequestUrl)
                         .Accepts(new EchoRequestBody("Vegeta"))
                         .WithHeader(CorrelationIdHeader, "after")
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoRequestResponse("after", "Vegeta"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Header_After_Body_Expecting_Another_Header_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoRequestUrl)
                                                                                    .Accepts(new EchoRequestBody("Vegeta"))
                                                                                    .WithHeader(CorrelationIdHeader, "after")
                                                                                    .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoRequestResponse("(absent)", "Vegeta"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The header set after Accepts… was sent - expecting it absent has to fail.",
                                                                        fix: OrderFix).ConfigureAwait(false);
        }

        // ============================================================
        // Parameter before / after the body - it fills the body's placeholder either way.
        // ============================================================

        [TestMethod]
        public Task Parameter_Before_Body_Should_Fill_The_Body()
        {
            return Client.AssertPatch(PatchUrl)
                         .WithParameter("Name", "Vegeta")
                         .Accepts(Vegeta() with { Name = "$Name$" })
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponse(Vegeta())
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Parameter_After_Body_Should_Fill_The_Body()
        {
            return Client.AssertPatch(PatchUrl)
                         .Accepts(Vegeta() with { Name = "$Name$" })
                         .WithParameter("Name", "Vegeta")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponse(Vegeta())
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Parameter_After_Body_Expecting_The_Placeholder_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPatch(PatchUrl)
                                                                                    .Accepts(Vegeta() with { Name = "$Name$" })
                                                                                    .WithParameter("Name", "Goku")
                                                                                    .Produces<Person>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Vegeta())
                                                                                    .ExecuteAsync(),
                                                                        because: "The parameter set after Accepts… replaced $Name$ with Goku - expecting Vegeta has to fail.",
                                                                        fix: OrderFix).ConfigureAwait(false);
        }

        // ============================================================
        // A body verb without a body.
        // ============================================================

        [TestMethod]
        public async Task Body_Verb_Without_Body_Should_Reach_The_Endpoint()
        {
            // No Accepts…: the body stage goes straight to Produces…. The body-less sample POST always throws,
            // so its 500 proves the request was really sent.
            await Client.AssertPost(BodylessPostUrl)
                        .Produces(HttpStatusCode.InternalServerError)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Body_Verb_Without_Body_Expecting_Success_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(BodylessPostUrl)
                                                                                    .Produces(HttpStatusCode.OK)
                                                                                    .ExecuteAsync(),
                                                                        because: "The body-less sample POST answers 500 - expecting 200 has to fail.",
                                                                        fix: OrderFix).ConfigureAwait(false);
        }
    }
}
