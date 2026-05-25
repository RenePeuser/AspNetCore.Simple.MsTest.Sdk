using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;

namespace Controllers.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Native API tests for POST /api/tests/v1/persons endpoint.
    /// Uses the classic AssertPostAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
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
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  person,
                                                  "CreatePerson.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Json_Files()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  "CreatePersonFull.json",
                                                  "CreatePersonFull.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_With_Status_Code()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  "CreatePersonFull.json",
                                                  "CreatePersonFull.json",
                                                  expectedStatusCode: System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("POST")]
        public Task Native_Should_Create_Person_Parameterized()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
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
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  "CreatePersonParameterized.json",
                                                  "CreatePersonParameterized.json",
                                                  parameters: [("$Name$", "Son"), ("$Age$", 42)],
                                                  differenceFunc: TestHelpers.IgnoreIdDifferences);
        }
    }
}
