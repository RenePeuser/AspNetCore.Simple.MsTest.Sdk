using System.Net;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;

namespace Controllers.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Native API tests for DELETE /api/v1/persons endpoints.
    /// Tests both 204 NoContent and 200 OK with response body variants.
    /// </summary>
    [Ignore("Temp")]
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("DELETE")]
    public class PersonDeleteTests : ApiTestBase
    {
        // ============================================================
        // DELETE - 204 No Content (without response body)
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        public Task Native_Should_Delete_Person_NoContent()
        {
            // Non-generic AssertDeleteAsync expects 204 NoContent by default
            return Client.AssertDeleteAsync("api/v1/persons/1");
        }

        [TestMethod]
        [TestCategory("Native")]
        public Task Native_Should_Delete_Person_NoContent_With_Explicit_Status_Code()
        {
            // Explicit 204 validation
            return Client.AssertDeleteAsync("api/v1/persons/1",
                                            expectedHttpStatusCode: HttpStatusCode.NoContent);
        }

        // ============================================================
        // DELETE - 200 OK with Response Body
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        public Task Native_Should_Delete_Person_With_Response()
        {
            // Generic AssertDeleteAsync<T> expects typed response with 200 OK
            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                  "DeletePersonWithResponse.json",
                                                                  parameters: [("$DeletedAt$", DateTime.UtcNow)]);
        }

        [TestMethod]
        [TestCategory("Native")]
        public Task Native_Should_Delete_Person_With_Response_Ignore_Response_Content()
        {
            // Just validate endpoint exists and returns correct type, ignore response content
            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response");
        }

        [TestMethod]
        [TestCategory("Native")]
        public Task Native_Should_Delete_Person_With_Response_Ignore_With_Explicit_Status()
        {
            // Validate endpoint + explicit status code, ignore response content
            return Client.AssertDeleteAsync<DeletePersonResponse>("api/v1/persons/1/with-response",
                                                                  expectedHttpStatusCode: HttpStatusCode.OK);
        }
    }
}