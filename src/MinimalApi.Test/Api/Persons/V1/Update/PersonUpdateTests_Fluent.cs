using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Fluent API tests for PUT/PATCH /api/v1/persons endpoint (Model B: chain ends in ExecuteAsync).
    /// </summary>
    public partial class PersonUpdateTests
    {
        // ============================================================
        // PATCH
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person()
        {
            return Client.Patch("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePerson.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_Ignore_Id()
        {
            return Client.Patch("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.Patch("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePerson.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        // ============================================================
        // PUT
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person()
        {
            return Client.Put("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePersonNew.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_Ignore_Id()
        {
            return Client.Put("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePersonNew.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.Put("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePersonNew.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }
    }
}
