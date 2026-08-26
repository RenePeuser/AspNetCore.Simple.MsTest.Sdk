using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Tests that verify we get STATUS CODE MISMATCH errors (not JSON Serialization errors)
    /// when the API returns an error response but the test expects success.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Status Code Mismatch")]
    public class StatusCodeMismatchTests : ApiTestBase
    {
        /// <summary>
        /// Regression test for: "API returns 500 ProblemDetails, test expects 200 OK with typed response"
        ///
        /// BEFORE FIX: Would throw "JSON SERIALIZATION ERROR" because it tried to deserialize
        ///             ProblemDetails as ProblemDetails, which failed before status code check.
        ///
        /// AFTER FIX:  Throws "UNEXPECTED STATUS CODE: Expected 200, Got 500" with ProblemDetails
        ///             shown in the response content.
        /// </summary>
        [TestMethod]
        public async Task Should_Show_Status_Code_Mismatch_Not_Json_Error_When_Api_Returns_ProblemDetails()
        {
            // ARRANGE: Test expects success (AssertPostAsync), but API throws exception → 500 ProblemDetails

            // ACT & ASSERT: Should fail with STATUS CODE MISMATCH (not JSON error)
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                                   writeResponse: false,
                                                                                                                                   skipEndpointValidation: true),
                                                                                        because: "The endpoint throws and answers 500 ProblemDetails while the test expects success. That mismatch has to fail the test - passing would mean an erroring endpoint looks healthy.",
                                                                                        fix: "Check that the status code validation step runs before the body is deserialized.")
                                        .ConfigureAwait(false);

            // VERIFY: Error message should contain "STATUS CODE" (not "SERIALIZATION")
            Assert.That.Contains(exception.Message,
                                 "STATUS CODE",
                                 because: "The status code is the real problem. Reporting a JSON SERIALIZATION ERROR instead - which is what happens when the body is deserialized first - sends the author to debug their model for what is a 500 from the api.",
                                 fix: "Check the step order in the assert pipeline: StatusCodeValidationStep has to run before the content is deserialized into the expected type.");

            Assert.That.IsTrue(exception.Message.Contains("500") || exception.Message.Contains("InternalServerError"),
                               because: "Knowing that the status code was wrong is not enough - the author needs the code that actually came back to tell a 500 from a 404 or a 401.",
                               fix: "Check that the status code output prints the actual code, numerically or by name.");

            // VERIFY: Response content (ProblemDetails) should be visible
            Assert.That.IsTrue(exception.Message.Contains("Implementation is missing") ||
                               exception.Message.Contains("ProblemDetails"),
                               because: "The ProblemDetails body carries why the api failed. Without it the author knows only that something went wrong on the server and has to reproduce the call by hand.",
                               fix: "Check that the response content is read and included in the status code mismatch output instead of being dropped once the code check fails.");
        }

        /// <summary>
        /// Similar test but expecting ProblemDetails but as success response (wrong test setup).
        /// API returns 500 ProblemDetails, test expects 200 OK with ProblemDetails.
        /// This is a common mistake when testing error endpoints.
        /// </summary>
        [TestMethod]
        public async Task Should_Show_Status_Code_Mismatch_When_Using_Wrong_Assert_Method()
        {
            // ARRANGE: API returns 500 ProblemDetails
            //          Test uses AssertPostAsync (expects 200) instead of AssertPostAsErrorAsync

            // ACT & ASSERT: Should fail with STATUS CODE MISMATCH
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                                   writeResponse: false,
                                                                                                                                   expectedHttpStatusCode: System.Net.HttpStatusCode.OK, // Explicitly expect 200
                                                                                                                                   skipEndpointValidation: true),
                                                                                        because: "Using AssertPostAsync instead of AssertPostAsErrorAsync while explicitly expecting 200 is a common mistake when testing error endpoints. Declaring the expected code explicitly must not make the check any weaker.",
                                                                                        fix: "Check that an explicitly passed expectedHttpStatusCode is really compared and does not overwrite the check with a blanket 'any status is fine'.")
                                        .ConfigureAwait(false);

            // VERIFY: Should show status code mismatch
            Assert.That.IsTrue(exception.Message.Contains("STATUS CODE") ||
                               exception.Message.Contains("200") ||
                               exception.Message.Contains("500"),
                               because: "The message has to name the mismatch or at least the two codes involved, so the author sees they picked the wrong assert method rather than that the api is broken.",
                               fix: "Check that the status code output prints expected and actual code; a bare 'assertion failed' leaves the author with nothing.");
        }
    }
}