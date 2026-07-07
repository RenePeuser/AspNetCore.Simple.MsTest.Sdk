using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Errors
{
    /// <summary>
    /// Tests for error handling using object-based expectedResponse.
    /// Uses the new AssertXAsErrorAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("Errors")]
    public partial class ErrorTests_ObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Handle_NotFound_Error_On_Get()
        {
            var expectedError = new ProblemDetails()
            {
                Status = 404,
                Title = "Person not found"
            };

            return Client.AssertGetAsErrorAsync("api/v1/persons/999",
                                                expectedError,
                                                expectedHttpStatusCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        public Task ObjectResponse_Should_Handle_BadRequest_Error_On_Post()
        {
            var invalidPerson = new { Name = "" };

            var expectedError = new
            {
                StatusCode = 400,
                Message = "Invalid request"
            };

            return Client.AssertPostAsErrorAsync("api/v1/persons",
                                                 invalidPerson,
                                                 expectedError,
                                                 expectedHttpStatusCode: HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PUT")]
        public Task ObjectResponse_Should_Handle_NotFound_Error_On_Put()
        {
            var personToUpdate = new
            {
                Name = "Test",
                Age = 30
            };

            var expectedError = new
            {
                StatusCode = 404,
                Message = "Person not found"
            };

            return Client.AssertPutAsErrorAsync("api/v1/persons/999",
                                                personToUpdate,
                                                expectedError,
                                                expectedHttpStatusCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Handle_NotFound_Error_On_Delete()
        {
            var problemDetails = new ProblemDetails() { Status = 404 };

            return Client.AssertDeleteAsErrorAsync("api/v1/persons/999",
                                                                   expectedResponse: problemDetails,
                                                                   expectedHttpStatusCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        public Task ObjectResponse_Should_Handle_BadRequest_Error_On_Query()
        {
            var invalidQuery = new { InvalidField = "test" };

            var expectedError = new
            {
                StatusCode = 400,
                Message = "Invalid query"
            };

            return Client.AssertQueryAsErrorAsync("api/v1/persons/search",
                                                  invalidQuery,
                                                  expectedError,
                                                  expectedHttpStatusCode: HttpStatusCode.BadRequest);
        }
    }
}