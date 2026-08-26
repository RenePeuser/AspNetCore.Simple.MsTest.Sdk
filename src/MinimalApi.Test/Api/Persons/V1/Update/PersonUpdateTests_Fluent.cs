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

        // ============================================================
        // Body-less Produces(code) — same engine as the typed path.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PATCH")]
        public Task Fluent_BodyLess_Patch_Should_Resolve_Embedded_Body()
        {
            // Two things at once, both of which the body-less path used to get wrong because it sent the
            // request itself instead of going through the engine: the embedded file was posted as its
            // NAME (a 500 from the api), and PATCH went out as application/json instead of
            // merge-patch+json.
            return Client.AssertPatch("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("PUT")]
        public Task Fluent_BodyLess_Put_Should_Resolve_Placeholders_In_Body()
        {
            return Client.AssertPut("api/v1/persons")
                         .AcceptsFromEmbeddedJson("UpdatePerson.json")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }
    }
}