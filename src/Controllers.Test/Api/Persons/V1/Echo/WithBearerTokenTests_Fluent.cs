using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Echo
{
    /// <summary>
    /// Controller twin of <c>MinimalApi.Test.FluentBuilders.WithBearerTokenTests</c>: per-request credentials
    /// (<c>WithBearerToken</c> / <c>WithoutAuthorization</c>) against controller actions. Both put the
    /// <c>Authorization</c> header on the request MESSAGE - the client's <c>DefaultRequestHeaders</c> stay untouched.
    ///
    /// <para>
    /// <c>persons/echo-authorization</c> shows which credentials arrived; <c>persons/requires-token</c> answers 401
    /// unless it gets <c>Bearer valid-token</c>. Every positive case is paired with a test that MUST fail.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Fluent")]
    public sealed class WithBearerTokenTests : ApiTestBase
    {
        private const string EchoAuthorizationUrl = "api/v1/persons/echo-authorization";

        private const string RequiresTokenUrl = "api/v1/persons/requires-token";

        private const string PerRequestFix = "Check that WithBearerToken / WithoutAuthorization add the Authorization header to the builder's request headers and that HttpRequestMessageBuilder puts them on the HttpRequestMessage - never on HttpClient.DefaultRequestHeaders.";

        // A client signed in for "the whole session" - its own instance, so no other test sees its default token.
        private static HttpClient SignedInClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            return client;
        }

        [TestMethod]
        public Task Token_Should_Reach_The_Action()
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
                                                                        because: "The shared client sends no credentials - if this echoed a token, the action would not measure the header and the test above would prove nothing.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);
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

        [TestMethod]
        public Task Valid_Token_Should_Open_The_Protected_Action()
        {
            return Client.AssertGet(RequiresTokenUrl)
                         .WithBearerToken("valid-token")
                         .Produces(HttpStatusCode.NoContent)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Wrong_Token_Expecting_204_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(RequiresTokenUrl)
                                                                                    .WithBearerToken("wrong-token")
                                                                                    .Produces(HttpStatusCode.NoContent)
                                                                                    .ExecuteAsync(),
                                                                        because: "The action answers 401 for any other token - a 204 would mean it does not look at the header and the positive case above proves nothing.",
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
        public async Task Parallel_Calls_On_One_Client_Should_Each_Send_Their_Own_Credentials()
        {
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
        public async Task Without_Authorization_Should_Drop_The_Client_Default_For_This_Call()
        {
            using var client = SignedInClient();

            await client.AssertGet(RequiresTokenUrl)
                        .WithoutAuthorization()
                        .Produces(HttpStatusCode.Unauthorized)
                        .ExecuteAsync()
                        .ConfigureAwait(false);

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
                                                                        because: "The signed-in client gets 204 - the 401 in the test above comes from WithoutAuthorization, not from the action rejecting everybody.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);
        }
    }
}
