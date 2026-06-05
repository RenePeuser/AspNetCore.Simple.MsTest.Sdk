using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Fluent API tests for GET /api/v1/persons endpoint.
    /// Contains both Neutral and Endpoint Style variations.
    /// </summary>
    public partial class PersonGetTests
    {
        // ============================================================
        // FLUENT API TESTS - NEUTRAL STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithResponse<IEnumerable<Person>>("GetAllPersons.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_With_Filtering()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithResponse<IEnumerable<Person>>("GetAllPersons.json")
                         .FilterResponse(TestHelpers.OrderByIdFilter)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_Ignore_Id()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithResponse<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithResponse<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_All_Persons_With_Status_Code()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithResponse<IEnumerable<Person>>("GetAllPersons.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.Neutral")]
        [TestCategory("GET")]
        public Task Fluent_Should_Get_Persons_By_Query_Parameter()
        {
            return Client.AssertGet("api/v1/persons?name=Son")
                         .WithResponse<IEnumerable<Person>>("GetPersonByQuery.json")
                         .ExpectSuccess();
        }

        // ============================================================
        // FLUENT API TESTS - ENDPOINT STYLE
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("GET")]
        public Task FluentEndpoint_Should_Get_All_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                         .Produces<IEnumerable<Person>>("GetAllPersons.json")
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("GET")]
        public Task FluentEndpoint_Should_Get_All_Persons_With_Filtering()
        {
            return Client.AssertGet("api/v1/persons")
                         .Produces<IEnumerable<Person>>("GetAllPersons.json")
                         .FilterResponse(TestHelpers.OrderByIdFilter)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("GET")]
        public Task FluentEndpoint_Should_Get_All_Persons_Ignore_Id()
        {
            return Client.AssertGet("api/v1/persons")
                         .Produces<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("GET")]
        public Task FluentEndpoint_Should_Get_All_Persons_With_Type_Safe_Property_Ignore()
        {
            return Client.AssertGet("api/v1/persons")
                         .Produces<IEnumerable<Person>>("GetAllPersons.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExpectSuccess();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("Fluent.EndpointStyle")]
        [TestCategory("GET")]
        public Task FluentEndpoint_Should_Get_Persons_By_Query_Parameter()
        {
            return Client.AssertGet("api/v1/persons?name=Son")
                         .Produces<IEnumerable<Person>>("GetPersonByQuery.json")
                         .ExpectSuccess();
        }
    }
}