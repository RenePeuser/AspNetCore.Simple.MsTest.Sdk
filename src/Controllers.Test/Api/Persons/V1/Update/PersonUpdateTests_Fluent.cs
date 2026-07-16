using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Update
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
            return Client.AssertPatch("api/v1/persons")
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
            return Client.AssertPatch("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
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
            return Client.AssertPut("api/v1/persons")
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
            return Client.AssertPut("api/v1/persons")
                         .WithEmbeddedJson("UpdatePerson.json")
                         .ReturnsEmbeddedJson<Person>("UpdatePersonNew.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }
    }
}
