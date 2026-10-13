using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Echo
{
    /// <summary>
    /// Controller twin of <c>MinimalApi.Test.FluentBuilders.CancellationTests</c>: <c>ExecuteAsync(CancellationToken)</c>
    /// on every fluent terminal against controller actions. A cancellation must surface as
    /// <see cref="OperationCanceledException"/>, not as an assertion failure about an API that did nothing wrong.
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Fluent")]
    [TestCategory("Cancellation")]
    public sealed class CancellationTests : ApiTestBase
    {
        private const string PersonUrl = "api/v1/persons/2";

        private const string ForwardFix = "Check that ExecuteAsync(cancellationToken) reaches HttpAssertContext.CancellationToken and from there HttpClient.SendAsync, and that AssertableHttpClient rethrows an OperationCanceledException instead of turning it into Assert.Fail.";

        private static CancellationToken Cancelled()
        {
            return new CancellationToken(canceled: true);
        }

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }

        [TestMethod]
        public async Task Typed_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertGet(PersonUrl)
                                                                                  .Produces<Person>(HttpStatusCode.OK)
                                                                                  .ExpectedResponse(Vegeta())
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "A cancelled token must stop the call - the action answering correctly does not make it a green test.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Body_Verb_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertPost("api/v1/persons/echo-request")
                                                                                  .Accepts(new EchoRequestBody("Goku"))
                                                                                  .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "The body verb runs through the same engine and must honor the token as well.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Status_Only_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertGet(PersonUrl)
                                                                                  .Produces(HttpStatusCode.OK)
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "Produces(code).ExecuteAsync(token) must honor the token.",
                                                                      fix: ForwardFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Options_Terminal_Should_Honor_Cancelled_Token()
        {
            await Assert.That.ThrowsAsync<OperationCanceledException>(() => Client.AssertOptions("api/v1/persons/echo-request")
                                                                                  .ExpectedHeaders(ImmutableDictionary<string, string>.Empty.Add("Allow", "POST, OPTIONS"))
                                                                                  .ExecuteAsync(Cancelled()),
                                                                      because: "The OPTIONS terminal sends its own request - it has to forward the token too, not fail on the headers.",
                                                                      fix: "Check that HttpOptionsBuilder.ExecuteAsync passes the token to AssertOptionsAsync and on to SendAsync.").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Live_Token_Should_Run_The_Assertion()
        {
            using var source = new CancellationTokenSource();

            await Client.AssertGet(PersonUrl)
                        .Produces<Person>(HttpStatusCode.OK)
                        .ExpectedResponse(Vegeta())
                        .ExecuteAsync(source.Token)
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Live_Token_Should_Not_Swallow_A_Wrong_Expectation()
        {
            using var source = new CancellationTokenSource();

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(PersonUrl)
                                                                                    .Produces<Person>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Vegeta() with { Name = "Wrong" })
                                                                                    .ExecuteAsync(source.Token),
                                                                        because: "A token that was never cancelled must not change the outcome - the wrong name has to fail.",
                                                                        fix: "Check that only an OperationCanceledException with a CANCELLED token bypasses the assertion error handling.").ConfigureAwait(false);
        }
    }
}
