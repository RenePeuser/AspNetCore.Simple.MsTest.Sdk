using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Errors
{
    [TestClass]
    [TestCategory("Controller")]
    public class ErrorsController : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json");
        }

        [TestMethod]
        public Task Should_Handle_Error_Response_With_Filter_Func()
        {
            // 1. Call endpoint which will return an error response
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 DifferenceFunc);

            // 2. Intercept difference detection also for error response
            static IEnumerable<Difference> DifferenceFunc(ImmutableList<Difference> differences)
            {
                foreach (var difference in differences)
                {
                    yield return difference;
                }
            }
        }

        [TestMethod]
        public Task Should_Return_500_When_Expected_Status_Code_Is_Specified()
        {
            // Test with explicit expectedHttpStatusCode parameter (500 InternalServerError)
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Should_Fail_When_Expected_Status_Code_Does_Not_Match()
        {
            // This test should fail because we expect 404 but get 500
            // The endpoint returns 500 (InternalServerError)
            // We don't provide an expected JSON file, so only the status code is checked
            await Assert.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                       Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                              writeResponse: false,
                                                                                                              skipEndpointValidation: true,
                                                                                                              expectedHttpStatusCode: System.Net.HttpStatusCode.NotFound))
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public Task Should_Pass_When_Expected_Status_Code_Matches()
        {
            // This test should pass because we explicitly expect 500
            // and the endpoint returns 500 (InternalServerError)
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }
    }
}