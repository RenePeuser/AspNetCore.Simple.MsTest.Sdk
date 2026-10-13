using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for per-request credentials: <c>WithBearerToken(token)</c> and <c>WithoutAuthorization()</c>.
    ///
    /// <para>
    /// Both put the <c>Authorization</c> header on the request MESSAGE. <c>HttpClient</c> only merges default headers
    /// the request does not carry, so the per-request value wins for this call - and the client's
    /// <c>DefaultRequestHeaders</c> are never touched. A client shared by the whole session (one
    /// <c>[AssemblyInitialize]</c>, tests in parallel) keeps its own token before, during and after the call.
    /// </para>
    ///
    /// <para>
    /// <c>persons/echo-authorization</c> shows which credentials arrived; <c>persons/requires-token</c> answers 401
    /// unless it gets <c>Bearer valid-token</c>. Every positive case is paired with a test that MUST fail.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class WithBearerTokenTests : ApiTestBase
    {
        private const string EchoAuthorizationUrl = "api/v1/persons/echo-authorization";

        private const string RequiresTokenUrl = "api/v1/persons/requires-token";

        private const string PerRequestFix = "Check that WithBearerToken / WithoutAuthorization add the Authorization header to the builder's request headers and that HttpRequestMessageBuilder puts them on the HttpRequestMessage - never on HttpClient.DefaultRequestHeaders.";

        private const string DuplicateFix = "Check that HttpRequestBuilder.AddHeader routes a repeated Authorization header to FluentUsageError.DuplicateAuthorization.";

        // A client signed in for "the whole session" - its own instance, so no other test sees its default token.
        private static HttpClient SignedInClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            return client;
        }

        // ============================================================
        // WithBearerToken.
        // ============================================================

        [TestMethod]
        public Task Token_Should_Reach_The_Endpoint()
        {
            return Client.AssertGet(EchoAuthorizationUrl)
                         .WithBearerToken("abc")
                         .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoAuthorizationResponse("Bearer abc"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Without_Token_Expecting_One_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoAuthorizationUrl)
                                                                                    .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoAuthorizationResponse("Bearer abc"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The shared client sends no credentials - if this echoed a token, the endpoint would not measure the header and the test above would prove nothing.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Token_Should_Win_Over_The_Client_Default_And_Leave_It_Untouched()
        {
            using var client = SignedInClient();

            await client.AssertGet(EchoAuthorizationUrl)
                        .WithBearerToken("other-token")
                        .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                        .ExpectedResponse(new EchoAuthorizationResponse("Bearer other-token"))
                        .ExecuteAsync()
                        .ConfigureAwait(false);

            // The next call without WithBearerToken is back on the session token - nothing to restore.
            await client.AssertGet(EchoAuthorizationUrl)
                        .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                        .ExpectedResponse(new EchoAuthorizationResponse("Bearer valid-token"))
                        .ExecuteAsync()
                        .ConfigureAwait(false);

            Assert.That.AreEqual("valid-token", client.DefaultRequestHeaders.Authorization?.Parameter,
                                 because: "WithBearerToken is per request - the client's default credentials must be exactly what the session set.",
                                 fix: PerRequestFix);
        }

        [TestMethod]
        public async Task Token_Should_Leave_The_Client_Default_Untouched_When_The_Call_Fails()
        {
            using var client = SignedInClient();

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => client.AssertGet(EchoAuthorizationUrl)
                                                                                    .WithBearerToken("other-token")
                                                                                    .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoAuthorizationResponse("wrong"))
                                                                                    .ExecuteAsync(),
                                                                        because: "The expected echo is deliberately wrong - the call has to fail.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);

            await client.AssertGet(RequiresTokenUrl)
                        .Produces(HttpStatusCode.NoContent)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Parallel_Calls_On_One_Client_Should_Each_Send_Their_Own_Credentials()
        {
            // A swap of DefaultRequestHeaders would leak "Bearer other-token" into the default calls running at the
            // same time - with 50 rounds that shows up as a 401 on requires-token.
            using var client = SignedInClient();

            var calls = Enumerable.Range(0, 50)
                                  .SelectMany(round => new[]
                                  {
                                      client.AssertGet(EchoAuthorizationUrl)
                                            .WithBearerToken($"other-token-{round}")
                                            .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                                            .ExpectedResponse(new EchoAuthorizationResponse($"Bearer other-token-{round}"))
                                            .ExecuteAsync(),
                                      client.AssertGet(RequiresTokenUrl)
                                            .Produces(HttpStatusCode.NoContent)
                                            .ExecuteAsync()
                                  })
                                  .ToArray();

            await Task.WhenAll(calls).ConfigureAwait(false);
        }

        [TestMethod]
        public Task Body_Verb_Should_Send_Token_Before_Accepts()
        {
            return Client.AssertPost(EchoAuthorizationUrl)
                         .WithBearerToken("abc")
                         .Accepts(new { Name = "Goku" })
                         .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoAuthorizationResponse("Bearer abc"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Body_Verb_Should_Send_Token_After_Accepts()
        {
            return Client.AssertPost(EchoAuthorizationUrl)
                         .Accepts(new { Name = "Goku" })
                         .WithBearerToken("abc")
                         .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoAuthorizationResponse("Bearer abc"))
                         .ExecuteAsync();
        }

        // ============================================================
        // WithoutAuthorization.
        // ============================================================

        [TestMethod]
        public async Task Without_Authorization_Should_Drop_The_Client_Default_For_This_Call()
        {
            using var client = SignedInClient();

            await client.AssertGet(RequiresTokenUrl)
                        .WithoutAuthorization()
                        .Produces(HttpStatusCode.Unauthorized)
                        .ExecuteAsync()
                        .ConfigureAwait(false);

            // The empty header on the request message keeps HttpClient from merging its default one in - and it does not
            // reach the endpoint at all: the server sees no Authorization header.
            await client.AssertGet(EchoAuthorizationUrl)
                        .WithoutAuthorization()
                        .Produces<EchoAuthorizationResponse>(HttpStatusCode.OK)
                        .ExpectedResponse(new EchoAuthorizationResponse("(absent)"))
                        .ExecuteAsync()
                        .ConfigureAwait(false);

            await client.AssertGet(RequiresTokenUrl)
                        .Produces(HttpStatusCode.NoContent)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Signed_In_Client_Without_WithoutAuthorization_Should_Not_Get_401()
        {
            using var client = SignedInClient();

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => client.AssertGet(RequiresTokenUrl)
                                                                                    .Produces(HttpStatusCode.Unauthorized)
                                                                                    .ExecuteAsync(),
                                                                        because: "The signed-in client gets 204 - the 401 in the test above comes from WithoutAuthorization, not from the endpoint rejecting everybody.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);
        }

        // ============================================================
        // Credentials once per chain — runtime values, so they share the header rule.
        // ============================================================

        [TestMethod]
        public void Token_Set_Twice_Should_Throw_Without_Showing_The_Tokens()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoAuthorizationUrl)
                                                                                     .WithBearerToken("secret-one")
                                                                                     .WithBearerToken("secret-two"),
                                                                         because: "A request carries one Authorization header - the second token would replace the first silently.",
                                                                         fix: DuplicateFix);

            foreach (var part in new[] { "CREDENTIALS SET TWICE", "Bearer *** (hidden)", "What Happened", "Why This Fails", "How To Fix" })
            {
                Assert.That.Contains(exception.Message,
                                     part,
                                     because: "The usage error names both credential calls in the sdk's usual layout.",
                                     fix: "Check FluentUsageError.DuplicateAuthorization.");
            }

            Assert.That.DoesNotContain(exception.Message,
                                       "secret-",
                                       because: "Test tokens are often real ones and this message lands in CI logs - it must never print them.",
                                       fix: "Check FluentUsageError.DescribeAuthorization: it shows the scheme only.");
        }

        [TestMethod]
        public void Token_And_Without_Authorization_Should_Throw()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoAuthorizationUrl)
                                                                                     .WithoutAuthorization()
                                                                                     .WithBearerToken("abc"),
                                                                         because: "No credentials and a token contradict each other - one of them would win silently.",
                                                                         fix: DuplicateFix);

            Assert.That.Contains(exception.Message,
                                 "WithoutAuthorization()",
                                 because: "The usage error has to name the call that set the credentials first.",
                                 fix: "Check FluentUsageError.DescribeAuthorization for the empty value.");
        }

        [TestMethod]
        public void Token_And_Authorization_Header_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoAuthorizationUrl)
                                                                     .WithHeader("authorization", "Basic abc")
                                                                     .WithBearerToken("abc"),
                                                         because: "WithHeader(\"authorization\", …) sets the same header (case-insensitive) - the token would replace it silently.",
                                                         fix: DuplicateFix);
        }

        [TestMethod]
        public void Empty_Token_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoAuthorizationUrl)
                                                                     .WithBearerToken(" "),
                                                         because: "'Bearer ' without a token is no credential - WithoutAuthorization() is the explicit way to send none.",
                                                         fix: "Check that HttpRequestBuilder.AddBearerToken calls ArgumentException.ThrowIfNullOrWhiteSpace.");
        }
    }
}
