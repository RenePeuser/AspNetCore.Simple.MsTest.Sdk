using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Query
{
    [TestClass]
    [TestCategory("MinimalApi")]
    public partial class PersonQueryTests : ApiTestBase
    {
        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_All_Persons()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>(url: "api/v1/persons",
                                                                expectedResult: "GetAllPersons.json",
                                                                parameters: []);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_All_Persons_With_Filtering()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons",
                                                                "GetAllPersons.json",
                                                                TestHelpers.OrderByIdFilter);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_All_Persons_Ignore_Id()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons",
                                                                "GetAllPersons.json",
                                                                TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("QUERY")]
        public Task Native_Should_Query_All_Persons_Ignore_Response()
        {
            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons");
        }
    }
}