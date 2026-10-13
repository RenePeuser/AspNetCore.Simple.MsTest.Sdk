using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for <c>ExecuteAsync(CancellationToken)</c> on every fluent terminal.
    ///
    /// <para>
    /// The engine used to send every request with <c>CancellationToken.None</c>: a test timeout or a cancelled
    /// test run could not stop a hanging call. A cancellation must surface as
    /// <see cref="OperationCanceledException"/> - reporting it as an assertion failure would print an error
    /// analysis of an API that did nothing wrong.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Cancellation")]
    public sealed class CancellationTests : ApiTestBase
    {
        private const string ProfileUrl = "api/v1/persons/1/profile";

        private const string ForwardFix = "Check that ExecuteAsync(cancellationToken) reaches HttpAssertContext.CancellationToken and from there HttpClient.SendAsync, and that AssertableHttpClient rethrows an OperationCanceledException instead of turning it into Assert.Fail.";

        private static CancellationToken Cancelled()
        {
            return new CancellationToken(canceled: true);
        }

        private static PersonProfile Expected()
        {
            return new PersonProfile(Id: 1,
                                     Name: "Son Goku",
                                     Address: new ProfileAddress("Home", "Kame House 1", "12345"),
                                     Contacts: ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                    new ProfileContact("Private", "goku@kame.house")));
        }

        // ============================================================
        // A cancelled token stops the call - on every terminal.
        // ============================================================

        [TestMethod]
        public async Task Typed_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertGet(ProfileUrl)
                                                                                  .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                  .ExpectedResponse(Expected())
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "A cancelled token must stop the call - the endpoint answering correctly does not make it a green test.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Bodyless_Typed_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertGet(ProfileUrl)
                                                                                  .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "The body-less path runs through the same engine and must honor the token as well.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Status_Only_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertGet(ProfileUrl)
                                                                                  .Produces(HttpStatusCode.OK)
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "Produces(code).ExecuteAsync(token) must honor the token.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Options_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertOptions("api/v1/persons")
                                                                                  .ExpectedHeaders(ImmutableDictionary<string, string>.Empty.Add("Allow", "GET"))
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "The OPTIONS terminal sends its own request - it has to forward the token too, not fail on the status code.",
                                                                      fix: "Check that HttpOptionsBuilder.ExecuteAsync passes the token to AssertOptionsAsync and on to SendAsync.").ConfigureAwait(false);
        }

        // ============================================================
        // A live token changes nothing about the assertion.
        // ============================================================

        [TestMethod]
        public async Task Live_Token_Should_Run_The_Assertion()
        {
            using var source = new CancellationTokenSource();

            await Client.AssertGet(ProfileUrl)
                        .Produces<PersonProfile>(HttpStatusCode.OK)
                        .ExpectedResponse(Expected())
                        .ExecuteAsync(source.Token)
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Live_Token_Should_Not_Swallow_A_Wrong_Expectation()
        {
            using var source = new CancellationTokenSource();

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                    .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Expected() with { Name = "Wrong" })
                                                                                    .ExecuteAsync(source.Token),
                                                                        because: "A token that was never cancelled must not change the outcome - the wrong name has to fail.",
                                                                        fix: "Check that only an OperationCanceledException with a CANCELLED token bypasses the assertion error handling.").ConfigureAwait(false);
        }
    }
}
