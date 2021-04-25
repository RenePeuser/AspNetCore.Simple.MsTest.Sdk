using System.Threading.Tasks;
using AspNetCore.Simple.Sdk.ErrorHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class Errors : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostError<ProblemDetails>("api/tests/v1/errors/not-implemented", "{\"Title\":\"NotImplementedException was thrown.\",\"Details\":\"Implementation is missing\",\"StatusCode\":501,\"ErrorDetails\":{}}");
        }
    }
}
