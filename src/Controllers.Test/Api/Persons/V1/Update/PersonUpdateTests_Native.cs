using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;

namespace Controllers.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Native API tests for PUT/PATCH /api/tests/v1/persons endpoint.
    /// Uses the classic AssertPutAsync and AssertPatchAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonUpdateTests : ApiTestBase
    {
        // ============================================================
        // NATIVE API TESTS - PATCH
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("PATCH")]
        public Task Native_Should_Patch_Person()
        {
            return Client.AssertPatchAsync<Person>("api/tests/v1/persons",
                                                   "UpdatePerson.json",
                                                   "UpdatePerson.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("PATCH")]
        public Task Native_Should_Patch_Person_With_Status_Code()
        {
            return Client.AssertPatchAsync<Person>("api/tests/v1/persons",
                                                   "UpdatePerson.json",
                                                   "UpdatePerson.json",
                                                   expectedStatusCode: System.Net.HttpStatusCode.OK);
        }

        // ============================================================
        // NATIVE API TESTS - PUT
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("PUT")]
        public Task Native_Should_Put_Person()
        {
            return Client.AssertPutAsync<Person>("api/tests/v1/persons",
                                                 "UpdatePerson.json",
                                                 "UpdatePersonNew.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("PUT")]
        public Task Native_Should_Put_Person_With_Status_Code()
        {
            return Client.AssertPutAsync<Person>("api/tests/v1/persons",
                                                 "UpdatePerson.json",
                                                 "UpdatePersonNew.json",
                                                 expectedStatusCode: System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("PUT")]
        public Task Native_Should_Put_Person_With_Inline_Json()
        {
            return Client.AssertPutAsync<Person?>("api/tests/v1/persons",
                                                  /*lang=json,strict*/
                                                  "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99,\"Emails\":[{\"EmailAddress\":\"alf@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"abc@hotmail.de\",\"Type\":\"Microsoft\"}]}",
                                                  /*lang=json,strict*/
                                                  "{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]}");
        }
    }
}