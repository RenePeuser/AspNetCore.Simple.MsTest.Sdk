using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test.Controllers
{
    [TestClass]
    public class ErrorsController : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/tests/v1/errors/not-implemented", /*lang=json,strict*/ "{\"title\":\"NotImplementedException was thrown.\",\"status\":501,\"detail\":\"Implementation is missing\"}");
        }
    }
}
