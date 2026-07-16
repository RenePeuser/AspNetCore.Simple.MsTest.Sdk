using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Fluent API tests for PUT/PATCH /api/v1/persons endpoint (Endpoint-Stil: chain ends in ExecuteAsync).
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
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePerson.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_Ignore_Id()
        {
            return Client.AssertPatch("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_Should_Patch_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPatch("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePerson.json")
                         .IgnoreProperty<Person>(p => p.Id)
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
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePersonNew.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_Ignore_Id()
        {
            return Client.AssertPut("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePersonNew.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_Should_Put_Person_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertPut("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("UpdatePersonNew.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExecuteAsync();
        }
    }
}