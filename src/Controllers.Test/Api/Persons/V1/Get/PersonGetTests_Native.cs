using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Native API tests for GET /api/v1/persons endpoint.
    /// Uses the classic AssertGetAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
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

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        [DataRow("{dsdmsd")]
        [DataRow("[invalid")]
        [DataRow("{incomplete")]
        public async Task Native_Should_Throw_When_Json_Malformed(string invalidJson)
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", invalidJson),
                                                                                        because: $"Inline json that does not parse ({invalidJson}) has to stop the test. Treating it as an unresolvable snapshot reference and carrying on would compare against nothing.",
                                                                                        fix: "Check that the inline-json path reports a parse error instead of falling through to the snapshot lookup.")
                                        .ConfigureAwait(false);

            // Just verify an exception was thrown with invalid JSON
            Assert.That.IsNotNull(exception,
                                  because: "ThrowsExactlyAsync hands the caught exception back so the message can be inspected - a null here would mean the assert helper itself lost it.",
                                  fix: "Check the return value of Assert.That.ThrowsExactlyAsync.");
        }
    }
}