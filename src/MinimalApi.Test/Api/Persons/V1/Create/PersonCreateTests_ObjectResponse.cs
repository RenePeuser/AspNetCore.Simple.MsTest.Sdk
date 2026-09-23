using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Tests for POST /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertPostAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonCreateTestsObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        public Task ObjectResponse_Should_Create_Person()
        {
            var personToCreate = TestHelpers.CreateValidPerson();

            var expectedPerson = new Person(Id: 1,
                                            Name: personToCreate.Name,
                                            FirstName: personToCreate.FirstName,
                                            Age: personToCreate.Age,
                                            Emails: personToCreate.Emails);

            return Client.AssertPostAsync("api/v1/persons", personToCreate, expectedPerson);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        public Task ObjectResponse_Should_Create_Person_With_Status_Code()
        {
            var personToCreate = TestHelpers.CreateValidPerson();

            var expectedPerson = new Person(Id: 1,
                                            Name: personToCreate.Name,
                                            FirstName: personToCreate.FirstName,
                                            Age: personToCreate.Age,
                                            Emails: personToCreate.Emails);

            return Client.AssertPostAsync("api/v1/persons",
                                          personToCreate,
                                          expectedPerson,
                                          expectedHttpStatusCode: HttpStatusCode.Created);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        public Task ObjectResponse_Should_Create_Person_Ignore_Id()
        {
            var personToCreate = TestHelpers.CreateValidPerson();

            var expectedPerson = new Person(Id: 0,
                                            Name: personToCreate.Name,
                                            FirstName: personToCreate.FirstName,
                                            Age: personToCreate.Age,
                                            Emails: personToCreate.Emails);

            return Client.AssertPostAsync("api/v1/persons",
                                          personToCreate,
                                          expectedPerson,
                                          TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        public Task Test_CSharp_Response_Creation()
        {
            var personToCreate = TestHelpers.CreateValidPerson();

            var expectedPerson = new Person(1, "Son", "Goku",
                                            42, []);

            return Client.AssertPostAsync("api/v1/persons",
                                          personToCreate,
                                          expectedPerson,
                                          writeResponse: true);
        }
    }
}