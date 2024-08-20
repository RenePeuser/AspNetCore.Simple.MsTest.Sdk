using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test.Controllers
{
    [TestClass]
    public class NativeTypes : MsTestBase
    {
        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_String_As_Well()
        {
            return Client.AssertGetAsync<string>("api/tests/v1/native-types/string",
                                                 "String only");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_Int_As_Well()
        {
            return Client.AssertGetAsync<int>("api/tests/v1/native-types/int",
                                              "42");
        }
    }
}
