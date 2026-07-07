using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.NativTypes
{
    [TestClass]
    [TestCategory("Controller")]
    public class NativeTypes : ApiTestBase
    {
        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_String_As_Well()
        {
            return Client.AssertGetAsync("api/v1/native-types/string", "String only");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_Int_As_Well()
        {
            return Client.AssertGetAsync<int>("api/v1/native-types/int", "42");
        }
    }
}