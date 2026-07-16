using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Fluent API tests for GET /api/v1/persons endpoint (Model B: chain ends in ExecuteAsync).
    /// </summary>
    public partial class PersonGetTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                         .ReturnsEmbeddedJson<IEnumerable<Person>>("GetAllPersons.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_With_Filtering()
        {
            return Client.AssertGet("api/v1/persons")
                         .ReturnsEmbeddedJson<IEnumerable<Person>>("GetAllPersons.json")
                         .FilterResponse(TestHelpers.OrderByIdFilter)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_Ignore_Id()
        {
            return Client.AssertGet("api/v1/persons")
                         .ReturnsEmbeddedJson<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertGet("api/v1/persons")
                         .ReturnsEmbeddedJson<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_Persons_By_Query_Parameter()
        {
            return Client.AssertGet("api/v1/persons?name=Son")
                         .ReturnsEmbeddedJson<IEnumerable<Person>>("GetPersonByQuery.json")
                         .ExpectingSuccess()
                         .ExecuteAsync();
        }
    }
}
