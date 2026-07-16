using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Fluent API tests for POST /api/v1/persons endpoint (Model B: chain ends in ExecuteAsync).
    /// </summary>
    public partial class PersonCreateTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .WithBody(person)
                         .ReturnsEmbeddedJson<Person>("CreatePerson.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Json()
        {
            return Client.AssertPost("api/v1/persons")
                         .WithEmbeddedJson("CreatePersonFull.json")
                         .ReturnsEmbeddedJson<Person>("CreatePersonFull.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_Ignore_Id()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .WithBody(person)
                         .ReturnsEmbeddedJson<Person>("CreatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Explicit_Status()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .WithBody(person)
                         .ReturnsEmbeddedJson<Person>("CreatePerson.json")
                         .ExpectingStatus(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Status_And_Filtering()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .WithBody(person)
                         .ReturnsEmbeddedJson<Person>("CreatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingStatus(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.StatusCode")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_OneOf_Status()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .WithBody(person)
                         .ReturnsEmbeddedJson<Person>("CreatePerson.json")
                         .ExpectingOneOf(HttpStatusCode.OK, HttpStatusCode.Created)
                         .ExecuteAsync();
        }
    }
}
