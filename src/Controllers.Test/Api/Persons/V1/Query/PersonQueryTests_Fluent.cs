using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Query
{
    /// <summary>
    /// Fluent API tests for QUERY /api/v1/persons — the fluent twin of the native QUERY tests.
    /// </summary>
    public partial class PersonQueryTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_All_Persons()
        {
            return Client.AssertQuery("api/v1/persons")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetAllPersons.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_All_Persons_With_Filtering()
        {
            return Client.AssertQuery("api/v1/persons")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetAllPersons.json")
                         .FilterResponse(TestHelpers.OrderByIdFilter)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_All_Persons_Ignore_Id()
        {
            return Client.AssertQuery("api/v1/persons")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetAllPersons.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_All_Persons_Without_Body_Comparison()
        {
            return Client.AssertQuery("api/v1/persons")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExecuteAsync();
        }
    }
}
