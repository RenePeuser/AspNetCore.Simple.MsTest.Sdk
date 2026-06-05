using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Native API tests for GET /api/v1/persons endpoint.
    /// Uses the classic AssertGetAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonGetTests : ApiTestBase
    {
        // ============================================================
        // NATIVE API TESTS
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Filtering()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              TestHelpers.OrderByIdFilter);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_Ignore_Id()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Status_Code()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              expectedHttpStatusCode: System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_Persons_By_Query_Parameter()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons?name=Son",
                                                              "GetPersonByQuery.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Inline_Json()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              /*lang=json,strict*/
                                                              "{\"content\":{\"headers\":[{\"key\":\"Content-Type\",\"value\":[\"application/json; charset=utf-8\"]}],\"value\":[{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]},{\"id\":2,\"name\":\"Vegeta\",\"firstName\":\"Unknown\",\"age\":77,\"emails\":[{\"emailAddress\":\"abc@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"maxmustermann@hotmail.de\",\"type\":\"Microsoft\"}]}]},\"statusCode\":\"OK\",\"headers\":[],\"trailingHeaders\":[],\"isSuccessStatusCode\":true}");
        }

        [Ignore("Fails in CI because of formatting")]
        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public async Task Native_Should_Throw_When_Response_Type_Invalid()
        {
            var error = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<UnknownResponse>("api/v1/persons",
                                                                                                                            "GetAllPersons.json")).ConfigureAwait(false);

            Assert.That.ObjectsAreEqual(expectedObjectAsJson: "InvalidResponseType.txt",
                                        currentObject: error.Message);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        [DataRow("I am not a valid json")]
        [DataRow("1234")]
        [DataRow("@abc jnd")]
        [DataRow("{dsdmsd")]
        [DataRow("I am not a valid json}")]
        public async Task Native_Should_Throw_When_Json_Invalid(string invalidJson)
        {
            var exception = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", invalidJson))
                                        .ConfigureAwait(false);

            Assert.Contains(invalidJson, exception.Message);
        }
    }

    /// <summary>
    /// Test helper class for invalid response type testing.
    /// </summary>
    internal sealed record UnknownResponse
    {
        public required string Property { get; init; }
    }
}