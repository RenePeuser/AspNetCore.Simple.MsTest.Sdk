using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Classic
{
    /// <summary>
    /// Consumer tests for the classic <c>Assert…AsUnauthorizedAsync</c> methods.
    ///
    /// <para>
    /// They used to swap <c>HttpClient.DefaultRequestHeaders.Authorization</c> to an invalid token and restore it
    /// after the call. On a client shared by the session that leaked the invalid token into every test running in
    /// parallel, and a call that threw never restored it. The invalid token now rides on the request message only.
    /// </para>
    ///
    /// <para>
    /// <c>persons/requires-token</c> answers 204 for <c>Bearer valid-token</c> and 401 otherwise, for all six verbs.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Unauthorized")]
    public sealed class AsUnauthorizedTests : ApiTestBase
    {
        private const string RequiresTokenUrl = "api/v1/persons/requires-token";

        private const string EchoAuthorizationUrl = "api/v1/persons/echo-authorization";

        private const string PerRequestFix = "Check HttpClientAssertExtensions.SendWithInvalidTokenAsync: the invalid token goes on the HttpRequestMessage, HttpClient.DefaultRequestHeaders are never written.";

        private static HttpClient SignedInClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            return client;
        }

        // ============================================================
        // Every verb sends the invalid token - even from a signed-in client.
        // ============================================================

        [TestMethod]
        public async Task Every_Verb_Should_Get_401_From_A_Signed_In_Client_And_Leave_It_Signed_In()
        {
            using var client = SignedInClient();

            await client.AssertGetAsUnauthorizedAsync(RequiresTokenUrl).ConfigureAwait(false);
            await client.AssertDeleteAsUnauthorizedAsync(RequiresTokenUrl).ConfigureAwait(false);
            await client.AssertPostAsUnauthorizedAsync(RequiresTokenUrl, new { Name = "Goku" }).ConfigureAwait(false);
            await client.AssertPutAsUnauthorizedAsync(RequiresTokenUrl, new { Name = "Goku" }, []).ConfigureAwait(false);
            await client.AssertPatchAsUnauthorizedAsync(RequiresTokenUrl, new { Name = "Goku" }).ConfigureAwait(false);
            await client.AssertQueryAsUnauthorizedAsync(RequiresTokenUrl, new { Name = "Goku" }).ConfigureAwait(false);

            Assert.That.AreEqual("valid-token", client.DefaultRequestHeaders.Authorization?.Parameter,
                                 because: "The unauthorized calls must not change the client's own credentials - the next test of the session uses them.",
                                 fix: PerRequestFix);

            await client.AssertGet(RequiresTokenUrl)
                        .Produces(HttpStatusCode.NoContent)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Open_Endpoint_Should_Fail_The_Unauthorized_Assertion()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsUnauthorizedAsync(EchoAuthorizationUrl),
                                                                        because: "echo-authorization answers 200 to anybody - the 401 assertion has to fail, or the green tests above would prove nothing.",
                                                                        fix: "Check that AssertGetAsUnauthorizedAsync asserts HttpStatusCode.Unauthorized on the response.").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Failed_Unauthorized_Assertion_Should_Leave_The_Client_Signed_In()
        {
            // The assertion fails AFTER the call - the old swap restored the token before it, so this one is about
            // the client never being written at all.
            using var client = SignedInClient();

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => client.AssertGetAsUnauthorizedAsync(EchoAuthorizationUrl),
                                                                        because: "echo-authorization answers 200 - the assertion has to fail.",
                                                                        fix: PerRequestFix).ConfigureAwait(false);

            Assert.That.AreEqual("valid-token", client.DefaultRequestHeaders.Authorization?.Parameter,
                                 because: "A failing unauthorized assertion must leave the client's credentials as they were.",
                                 fix: PerRequestFix);
        }

        [TestMethod]
        public async Task Parallel_Calls_On_One_Client_Should_Keep_Their_Own_Credentials()
        {
            // With the old swap, a signed-in call running while an unauthorized one held the invalid default token got
            // 401 instead of 204. 50 rounds make that interleaving all but certain.
            using var client = SignedInClient();

            // CA2025 cannot see that Task.WhenAll below completes every call before the client is disposed.
#pragma warning disable CA2025
            var calls = Enumerable.Range(0, 50)
                                  .SelectMany(_ => new[]
                                  {
                                      client.AssertGetAsUnauthorizedAsync(RequiresTokenUrl),
                                      client.AssertGet(RequiresTokenUrl)
                                            .Produces(HttpStatusCode.NoContent)
                                            .ExecuteAsync()
                                  })
                                  .ToArray();
#pragma warning restore CA2025

            await Task.WhenAll(calls).ConfigureAwait(false);
        }
    }
}
