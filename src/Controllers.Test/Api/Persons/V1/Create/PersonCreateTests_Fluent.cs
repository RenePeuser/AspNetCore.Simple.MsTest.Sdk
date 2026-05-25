using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;

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
    }
}
