using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Query
{
    /// <summary>
    /// Fluent API tests for QUERY /api/v1/persons/search — QUERY carries a request body, which is the
    /// whole point of the verb, so the request side gets exercised here.
    /// </summary>
    public partial class PersonQueryWithBodyTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_Person_With_Body_Full_Search()
        {
            return Client.AssertQuery("api/v1/persons/search")
                         .AcceptsFromEmbeddedJson("QuerySearchRequest.json")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetAllPersons.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_Person_With_Body_Name_Filter()
        {
            return Client.AssertQuery("api/v1/persons/search")
                         .AcceptsFromEmbeddedJson("QuerySearchRequestNameOnly.json")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetPersonByQuery.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_Person_With_Body_Parameterized()
        {
            return Client.AssertQuery("api/v1/persons/search")
                         .AcceptsFromEmbeddedJson("QuerySearchRequestParameterized.json")
                         .WithParameter("Name", "Son")
                         .WithParameter("MinAge", 50)
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetPersonByQuery.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("QUERY")]
        public Task Fluent_Should_Query_Person_With_Object_Body()
        {
            return Client.AssertQuery("api/v1/persons/search")
                         .Accepts(new { Name = "Son", MinAge = 50 })
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("GetPersonByQuery.json")
                         .ExecuteAsync();
        }
    }
}
