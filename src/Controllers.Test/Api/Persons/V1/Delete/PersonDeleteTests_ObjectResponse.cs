using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Tests for DELETE /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertDeleteAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonDeleteTests_ObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Delete_Person()
        {
            // DeletedAt timestamp is dynamic, so we don't include it in the assertion
            var expectedResponse = /*lang=csharp*//*lang=json,strict*/ """
                                                  {
                                                      "id": 1,
                                                      "name": "Son",
                                                      "firstName": "Goku",
                                                      "deleted": true
                                                  }
                                                  """;

            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                  expectedResponse,
                                                                  TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Delete_Person_With_Status_Code()
        {
            // DeletedAt timestamp is dynamic, so we don't include it in the assertion
            var expectedResponse = /*lang=csharp*//*lang=json,strict*/ """
                                                  {
                                                      "id": 1,
                                                      "name": "Son",
                                                      "firstName": "Goku",
                                                      "deleted": true
                                                  }
                                                  """;

            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                  expectedResponse,
                                                                  skipEndpointValidation: false,
                                                                  differenceFunc: TestHelpers.IgnoreIdDifferences);
        }
    }
}