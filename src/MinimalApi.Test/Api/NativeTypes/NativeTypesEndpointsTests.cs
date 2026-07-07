using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.NativeTypes
{
    [TestClass]
    [TestCategory("Minimal Api")]
    public class NativeTypesEndpointsTests : ApiTestBase
    {
        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_String_As_Well()
        {
            return Client.AssertGetAsync("api/v1/native-types/string", "String only");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Native_Int_As_Well()
        {
            return Client.AssertGetAsync<int>("api/v1/native-types/int", expectedResult: "42");
        }

        [TestMethod]
        public async Task Should_Be_Able_To_Fetch_Native_Int_As_Well_Invalid()
        {
            var result = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<int>("api/v1/native-types/int", expectedResult: "24")).ConfigureAwait(false);

            Assert.Contains("24", result.Message);
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Another_Native_Int()
        {
            // Tests different int value to verify comparison works correctly
            return Client.AssertGetAsync<int>("api/v1/native-types/another-int", expectedResult: "999");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Fetch_Another_Native_String()
        {
            // Tests different string value to verify comparison works correctly
            return Client.AssertGetAsync("api/v1/native-types/another-string", "Different text");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Use_Int_With_Skip_Endpoint_Validation()
        {
            // Test with skipEndpointValidation flag - no actual endpoint call needed
            return Client.AssertGetAsync<int>("api/v1/native-types/int", expectedResult: "42", skipEndpointValidation: true);
        }

        [TestMethod]
        public Task Should_Be_Able_To_Use_String_With_Skip_Endpoint_Validation()
        {
            // Test with skipEndpointValidation flag - no actual endpoint call needed
            return Client.AssertGetAsync("api/v1/native-types/string", "String only", skipEndpointValidation: true);
        }
    }
}