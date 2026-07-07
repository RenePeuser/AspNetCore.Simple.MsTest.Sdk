using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Tests for DELETE /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertDeleteAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonDeleteTests_ObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Delete_Person()
        {
            // DeletedAt timestamp is dynamic, so we don't include it in the assertion
            var expectedResponse = /*lang=json,strict*/ """
                                   {
                                       "id": 1,
                                       "name": "Son",
                                       "firstName": "Goku",
                                       "deleted": true
                                   }
                                   """;

            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                   expectedResponse);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Delete_Person_With_Status_Code()
        {
            // DeletedAt timestamp is dynamic, so we don't include it in the assertion
            var expectedResponse = /*lang=json,strict*/ """
                                   {
                                       "id": 1,
                                       "name": "Son",
                                       "firstName": "Goku",
                                       "deleted": true
                                   }
                                   """;

            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                   expectedResponse,
                                                                   skipEndpointValidation: false);
        }
    }
}