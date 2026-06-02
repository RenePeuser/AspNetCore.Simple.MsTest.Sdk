using System.Net;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.AspNetCore.Http;

namespace Controllers.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Fluent API tests for POST /api/tests/v1/persons endpoint.
    /// Contains both Neutral and Endpoint Style variations.
    /// </summary>
    public partial class PersonCreateTests
    {
        // ============================================================
        // FLUENT API TESTS - NEUTRAL STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody(person)
                         .WithResponse<Person>("CreatePerson.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Json()
        {
            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody("CreatePersonFull.json")
                         .WithResponse<Person>("CreatePersonFull.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_Ignore_Id()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody(person)
                         .WithResponse<Person>("CreatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - ENDPOINT STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts(person)
                         .Produces<Person>("CreatePerson.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_With_Json()
        {
            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts("CreatePersonFull.json")
                         .Produces<Person>("CreatePersonFull.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_Ignore_Id()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts(person)
                         .Produces<Person>("CreatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - NEW ENDPOINT STYLE WITH STATUS CODE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_With_StatusCode_Terminal()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts(person)
                         .WithResponseType<Person>()
                         .Produces(StatusCodes.Status200OK, "CreatePerson.json");
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_With_StatusCode_And_Filtering()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts(person)
                         .WithResponseType<Person>()
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .Produces(StatusCodes.Status200OK, "CreatePerson.json");
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task FluentEndpoint_Should_Create_Person_With_HttpStatusCode_Enum()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .Accepts(person)
                         .WithResponseType<Person>()
                         .Produces((int)HttpStatusCode.OK, "CreatePerson.json");
        }

        // ============================================================
        // FLUENT API TESTS - TERMINAL WithResponse OVERLOADS
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("Fluent.Terminal")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_WithResponse_ExpectSuccess_Terminal()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody(person)
                         .WithResponse<Person>("CreatePerson.json", expectSuccess: true);
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("Fluent.Terminal")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_WithResponse_StatusCode_Terminal()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody(person)
                         .WithResponse<Person>("CreatePerson.json", HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("Fluent.Terminal")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_WithResponse_MultiStatus_Terminal()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/tests/v1/persons")
                         .WithBody(person)
                         .WithResponse<Person>("CreatePerson.json",
                                               HttpStatusCode.OK,
                                               HttpStatusCode.Created);
        }
    }
}