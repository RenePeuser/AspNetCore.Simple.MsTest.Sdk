using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Fluent twins of the classic features the fluent API was missing: an anonymous (partial)
    /// expected object as in <c>AssertGetAsErrorAsync(url, new { … })</c>, <c>skipEndpointValidation</c>
    /// and OPTIONS.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Errors")]
    public sealed class ErrorTestsFluent : ApiTestBase
    {
        // ============================================================
        // Anonymous expected object — Produces(code).ExpectedResponse(new { … }).
        // ============================================================

        [TestMethod]
        [TestCategory("GET")]
        public async Task Fluent_Should_Compare_Anonymous_Expected_Object()
        {
            var expectedError = new
            {
                Title = "Person for given Id does not exist",
                Status = 404,
                Detail = "The person with the Id: 999 does not exist",
                Id = 999
            };

            var error = await Client.AssertGet("api/v1/persons/999")
                                    .Produces(HttpStatusCode.NotFound)
                                    .ExpectedResponse(expectedError)
                                    .ExecuteAsync()
                                    .ConfigureAwait(false);

            Assert.That.AreEqual(999, error.Id,
                                 because: "ExecuteAsync returns the real response, deserialized into the type of the expected object.",
                                 fix: "Check that ExpectedResponse<TExpected> hands TExpected through as the result type.");
        }

        [TestMethod]
        [TestCategory("GET")]
        public async Task Fluent_Should_Fail_When_Anonymous_Expected_Object_Differs()
        {
            var expectedError = new
            {
                Title = "Something else entirely",
                Status = 404
            };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                            Client.AssertGet("api/v1/persons/999")
                                                                                  .Produces(HttpStatusCode.NotFound)
                                                                                  .ExpectedResponse(expectedError)
                                                                                  .ExecuteAsync(),
                                                                        because: "The title differs from what the api reports, so the comparison has to fail - otherwise the anonymous path compares nothing.",
                                                                        fix: "Check that ExpectedResponse<TExpected> sets the expected json and does not fall back to the body-less path.")
                        .ConfigureAwait(false);
        }

        // ============================================================
        // SkipEndpointValidation — the classic skipEndpointValidation flag.
        // ============================================================

        [TestMethod]
        [TestCategory("GET")]
        public async Task Fluent_Should_Fail_Endpoint_Validation_For_Mismatching_Type()
        {
            // The endpoint declares Produces<Person>(200); <object> is a mismatch. This is the baseline
            // that makes the next test meaningful - without it a green SkipEndpointValidation test
            // would prove nothing.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                            Client.AssertGet("api/v1/persons/1")
                                                                                  .Produces<object>(HttpStatusCode.OK)
                                                                                  .ExecuteAsync(),
                                                                        because: "The endpoint declares Person, the test names object - endpoint validation has to reject that.",
                                                                        fix: "Check that the typed Produces<T>(code) path still runs endpoint validation by default.")
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("GET")]
        public Task Fluent_Should_Skip_Endpoint_Validation()
        {
            return Client.AssertGet("api/v1/persons/1")
                         .SkipEndpointValidation()
                         .Produces<object>(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        // ============================================================
        // OPTIONS — the fluent twin of AssertOptionsAsync.
        // ============================================================

        [TestMethod]
        [TestCategory("OPTIONS")]
        public async Task Fluent_Options_Should_Fail_When_Endpoint_Does_Not_Answer_NoContent()
        {
            // The sample api maps no OPTIONS handler, so the call cannot answer 204. A failing assert is
            // the proof that the chain really sends the request and checks the answer.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                            Client.AssertOptions("api/v1/persons")
                                                                                  .ExpectedHeaders(ImmutableDictionary<string, string>.Empty.Add("Allow", "GET, POST"))
                                                                                  .ExecuteAsync(),
                                                                        because: "Without an OPTIONS handler the endpoint cannot answer 204 NoContent, so the assertion has to fail.",
                                                                        fix: "Check that AssertOptions(...).ExecuteAsync() delegates to the OPTIONS engine instead of returning without a call.")
                        .ConfigureAwait(false);
        }
    }
}
