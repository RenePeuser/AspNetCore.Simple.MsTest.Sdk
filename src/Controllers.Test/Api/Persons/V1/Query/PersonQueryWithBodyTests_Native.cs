using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Query
{
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonQueryWithBodyTests : ApiTestBase
    {
        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_Person_With_Body_Full_Search()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                               "QuerySearchRequest.json",
                                                               "GetAllPersons.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_Person_With_Body_Name_Filter()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                               "QuerySearchRequestNameOnly.json",
                                                               "GetPersonByQuery.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_Person_With_Body_Age_Filter()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                               "QuerySearchRequestAgeFilter.json",
                                                               "GetPersonByQuery.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_Person_With_Body_Parameterized()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                               "QuerySearchRequestParameterized.json",
                                                               "GetPersonByQuery.json",
                                                               [
                                                                   ("$Name$", "Son"),
                                                                   ("$MinAge$", 50)
                                                               ]);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_Person_With_Body_Ignore_Response()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>(url: "api/v1/persons/search",
                                                               payloadAsJson: "QuerySearchRequest.json",
                                                               writeResponse: false);
        }
    }
}
