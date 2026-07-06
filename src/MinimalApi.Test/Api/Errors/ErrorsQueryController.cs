using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    [TestClass]
    [TestCategory("MinimalApi")]
    public class ErrorsQueryController : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Query_Error()
        {
            return Client.AssertQueryAsErrorAsync<ProblemDetails>("api/v1/errors/query-not-implemented",
                                                                  "ErrorQueryResponse.json");
        }

        [TestMethod]
        public Task Should_Handle_Query_Error_Response_With_Filter_Func()
        {
            return Client.AssertQueryAsErrorAsync<ProblemDetails>("api/v1/errors/query-not-implemented",
                                                                  "ErrorQueryResponse.json",
                                                                  DifferenceFunc);

            static IEnumerable<Difference> DifferenceFunc(ImmutableList<Difference> differences)
            {
                foreach (var difference in differences)
                {
                    yield return difference;
                }
            }
        }

        [TestMethod]
        public Task Should_Return_500_When_Expected_Status_Code_Is_Specified_For_Query()
        {
            return Client.AssertQueryAsErrorAsync<ProblemDetails>("api/v1/errors/query-not-implemented",
                                                                  "ErrorQueryResponse.json",
                                                                  expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public Task Should_Pass_When_Query_Expected_Status_Code_Matches()
        {
            return Client.AssertQueryAsErrorAsync<ProblemDetails>("api/v1/errors/query-not-implemented",
                                                                  "ErrorQueryResponse.json",
                                                                  expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
