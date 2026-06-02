using AspNetCore.Simple.MsTest.Sdk;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Native API tests for POST /api/v1/persons endpoint.
    /// Uses the classic AssertPostAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonCreateTests : ApiTestBase
    {
        // ============================================================
        // NATIVE API TESTS
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  person,
                                                  "CreatePerson.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Emails()
        {
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  person,
                                                  "CreatePersonFull.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Json_Files()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "CreatePersonFull.json",
                                                  "CreatePersonFull.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Status_Code()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "CreatePersonFull.json",
                                                  "CreatePersonFull.json",
                                                  expectedStatusCode: System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_Parameterized()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "CreatePersonParameterized.json",
                                                  "CreatePersonParameterized.json",
                                                  [
                                                      ("$Name$", "Son"),
                                                      ("$Age$", 42)
                                                  ]);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_Parameterized_Ignore_Id()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "CreatePersonParameterized.json",
                                                  "CreatePersonParameterized.json",
                                                  parameters: [("$Name$", "Son"), ("$Age$", 42)],
                                                  differenceFunc: TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Inline_Json()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  /*lang=json,strict*/
                                                  "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99,\"Emails\":[{\"EmailAddress\":\"alf@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"abc@hotmail.de\",\"Type\":\"Microsoft\"}]}",
                                                  /*lang=json,strict*/
                                                  "{\"content\":{\"headers\":[{\"key\":\"Content-Type\",\"value\":[\"application/json; charset=utf-8\"]}],\"value\":{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]}},\"statusCode\":\"OK\",\"headers\":[],\"trailingHeaders\":[],\"isSuccessStatusCode\":true}");
        }
    }
}