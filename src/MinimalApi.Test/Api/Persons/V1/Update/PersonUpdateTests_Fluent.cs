using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Fluent API tests for PUT/PATCH /api/v1/persons endpoint.
    /// Contains both Neutral and Endpoint Style variations.
    /// </summary>
    public partial class PersonUpdateTests
    {
        // ============================================================
        // FLUENT API TESTS - NEUTRAL STYLE - PATCH
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person()
        {
            return Client.AssertPatch("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePerson.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_Ignore_Id()
        {
            return Client.AssertPatch("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPatch("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePerson.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - NEUTRAL STYLE - PUT
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person()
        {
            return Client.AssertPut("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePersonNew.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_Ignore_Id()
        {
            return Client.AssertPut("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePersonNew.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPut("api/v1/persons")
                         .WithBody("UpdatePerson.json")
                         .WithResponse<Person>("UpdatePersonNew.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - ENDPOINT STYLE - PATCH
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PATCH")]
        public Task FluentEndpoint_Should_Patch_Person()
        {
            return Client.AssertPatch("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePerson.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PATCH")]
        public Task FluentEndpoint_Should_Patch_Person_Ignore_Id()
        {
            return Client.AssertPatch("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PATCH")]
        public Task FluentEndpoint_Should_Patch_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPatch("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePerson.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - ENDPOINT STYLE - PUT
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PUT")]
        public Task FluentEndpoint_Should_Put_Person()
        {
            return Client.AssertPut("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePersonNew.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PUT")]
        public Task FluentEndpoint_Should_Put_Person_Ignore_Id()
        {
            return Client.AssertPut("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePersonNew.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("PUT")]
        public Task FluentEndpoint_Should_Put_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPut("api/v1/persons")
                         .Accepts("UpdatePerson.json")
                         .Produces<Person>("UpdatePersonNew.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }
    }
}