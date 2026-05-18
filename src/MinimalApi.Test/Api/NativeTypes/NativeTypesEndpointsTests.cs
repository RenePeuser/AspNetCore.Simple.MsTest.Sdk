using AspNetCore.Simple.MsTest.Sdk;

namespace MinimalApi.Test.Api.NativeTypes
{
    [TestClass]
    [TestCategory("Minimal Api")]
    public class NativeTypesEndpointsTests : ApiTestBase
    {
        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_String_As_Well()
        {
            return Client.AssertGetAsync<string>("api/v1/native-types/string", "String only");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_Int_As_Well()
        {
            return Client.AssertGetAsync<int>("api/v1/native-types/int", "42");
        }
    }
}