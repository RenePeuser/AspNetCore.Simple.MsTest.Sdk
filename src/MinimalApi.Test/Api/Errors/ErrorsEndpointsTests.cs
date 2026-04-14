using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace MinimalApi.Test.Api.Errors
{
    [TestClass]
    public class ErrorsEndpointsTests : ApiTestBase
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
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 DifferenceFunc);

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
