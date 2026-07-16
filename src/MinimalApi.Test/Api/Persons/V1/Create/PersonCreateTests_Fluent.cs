using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Fluent API tests for POST /api/v1/persons endpoint (Endpoint-Stil: chain ends in ExecuteAsync).
    /// </summary>
    public partial class PersonCreateTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            var result = await Client.AssertPost("api/v1/persons")
                                     .Accepts(person)
                                     .Produces<Person>(HttpStatusCode.OK)
                                     .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                                     .ExecuteAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Emails()
        {
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Json()
        {
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonFull.json")
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_Ignore_Id()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                            .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Type_Safe_Property_Ignore()
        {
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                             .IgnoreProperty<Person>(p => p.Id)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Should_Create_Person_Without_Body_Comparison()
        {
            var person = TestHelpers.CreateValidPerson();

            // Body-less path: only status + typed result, no golden-file comparison (§15.6).
            var result = await Client.AssertPost("api/v1/persons")
                                     .Accepts(person)
                                     .Produces<Person>(HttpStatusCode.OK)
                                     .ExecuteAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
        }
    }
}
