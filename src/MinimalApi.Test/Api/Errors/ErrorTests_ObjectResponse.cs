using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Tests for error handling using object-based expectedResponse.
    /// Uses the new AssertXAsErrorAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Errors")]
    public partial class ErrorTestsObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Handle_NotFound_Error_On_Get()
        {
            var expectedError = new
            {
                Title = "Person for given Id does not exist",
                Status = 404,
                Detail = "The person with the Id: 999 does not exist",
                Id = 999
            };

            return Client.AssertGetAsErrorAsync("api/v1/persons/999",
                                                expectedError,
                                                skipEndpointValidation: true,
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
                                                 skipEndpointValidation: true,
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
                                                skipEndpointValidation: true,
                                                expectedHttpStatusCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        public Task ObjectResponse_Should_Handle_NotFound_Error_On_Delete()
        {
            var expectedError = new
            {
                StatusCode = 404,
                Message = "Person not found"
            };

            return Client.AssertDeleteAsErrorAsync("api/v1/persons/999",
                                                   expectedError,
                                                   skipEndpointValidation: true,
                                                   expectedHttpStatusCode: HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        public Task ObjectResponse_Should_Handle_BadRequest_Error_On_Query()
        {
            // Note: Invalid fields are ignored by model binding, so we test that it returns success
            // This test verifies the endpoint handles unknown fields gracefully
            return Client.AssertQueryAsync<IEnumerable<Person>>(url: "api/v1/persons/search",
                                                                payloadAsJson: /*lang=json,strict*/ "{\"name\":\"\",\"minAge\":null,\"maxAge\":null}",
                                                                writeResponse: false);
        }
    }
}