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

        /// <summary>
        /// Used to compare the whole rendered error block against a text snapshot, which made it fail
        /// in CI for reasons that had nothing to do with the endpoint: the block embeds the assert
        /// source code, its line number and box drawing, so reformatting this file was enough to break
        /// it. What the test is actually about is that a wrong response type is reported as a type
        /// mismatch and names the type the endpoint really returns - so that is what it asserts now.
        /// </summary>
        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public async Task Native_Should_Throw_When_Response_Type_Invalid()
        {
            var error = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<UnknownResponse>("api/v1/persons",
                                                                                                                            "GetAllPersons.json")).ConfigureAwait(false);

            StringAssert.Contains(error.Message, "HTTP RESPONSE TYPE MISMATCH");

            // The declared type and the one the endpoint returns - without both the message is useless.
            StringAssert.Contains(error.Message, nameof(UnknownResponse));
            StringAssert.Contains(error.Message, "IEnumerable<Person>");

            // And it has to say what to do about it.
            StringAssert.Contains(error.Message, "Suggested Fix");

            // No request may go out - the mismatch is caught by endpoint validation beforehand.
            StringAssert.Contains(error.Message, "No HTTP call was made");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        [DataRow("{dsdmsd")]
        [DataRow("[invalid")]
        [DataRow("{incomplete")]
        public async Task Native_Should_Throw_When_Json_Malformed(string invalidJson)
        {
            var exception = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", invalidJson))
                                        .ConfigureAwait(false);

            // Just verify an exception was thrown with invalid JSON
            Assert.IsNotNull(exception);
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