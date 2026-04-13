using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Test.Api.Errors
{
    [TestClass]
    public class ErrorsController : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/tests/v1/errors/not-implemented",
                                                                 "ErrorResponse.json");
        }

        [TestMethod]
        public Task Should_Handle_Error_Response_With_Filter_Func()
        {
            // 1. Call endpoint which will return an error response
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/tests/v1/errors/not-implemented",
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
    }
}
