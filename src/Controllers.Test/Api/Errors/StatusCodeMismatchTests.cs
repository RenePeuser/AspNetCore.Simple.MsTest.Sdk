using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Errors
{
    /// <summary>
    /// Tests that verify we get STATUS CODE MISMATCH errors (not JSON Serialization errors)
    /// when the API returns an error response but the test expects success.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
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
            var exception = await Assert.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                       Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                              writeResponse: false,
                                                                                                                              skipEndpointValidation: true)).ConfigureAwait(false);

            // VERIFY: Error message should contain "STATUS CODE" (not "SERIALIZATION")
            Assert.IsTrue(exception.Message.Contains("STATUS CODE"),
                          "Should show STATUS CODE MISMATCH, not JSON SERIALIZATION ERROR");

            Assert.IsTrue(exception.Message.Contains("500") || exception.Message.Contains("InternalServerError"),
                          "Should show actual status code 500");

            // VERIFY: Response content (ProblemDetails) should be visible
            Assert.IsTrue(exception.Message.Contains("Implementation is missing") ||
                          exception.Message.Contains("ProblemDetails"),
                          "Should show ProblemDetails response content");
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
            var exception = await Assert.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                       Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                              writeResponse: false,
                                                                                                                              expectedHttpStatusCode: System.Net.HttpStatusCode.OK, // Explicitly expect 200
                                                                                                                              skipEndpointValidation: true)).ConfigureAwait(false);

            // VERIFY: Should show status code mismatch
            Assert.IsTrue(exception.Message.Contains("STATUS CODE") ||
                          exception.Message.Contains("200") ||
                          exception.Message.Contains("500"),
                          "Should show STATUS CODE MISMATCH error");
        }
    }
}