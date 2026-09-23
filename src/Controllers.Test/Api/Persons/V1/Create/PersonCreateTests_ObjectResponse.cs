using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Tests for POST /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertPostAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
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
                                          expectedHttpStatusCode: HttpStatusCode.OK);
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
    }
}