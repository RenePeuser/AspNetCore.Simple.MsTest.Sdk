using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Errors
{
    /// <summary>
    /// Fluent twins of the classic features the fluent API was missing: an anonymous (partial)
    /// expected object as in <c>AssertGetAsErrorAsync(url, new { … })</c>, <c>skipEndpointValidation</c>
    /// and OPTIONS.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
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
                Title = "Person not found",
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
    }
}
