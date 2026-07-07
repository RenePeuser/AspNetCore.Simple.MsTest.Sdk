using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Tests for PUT/PATCH /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertPutAsync/AssertPatchAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonUpdateTests_ObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PUT")]
        public Task ObjectResponse_Should_Update_Person_With_Put()
        {
            var personToUpdate = new Person(Id: 1,
                                            Name: "Son",
                                            FirstName: "Goku Updated",
                                            Age: 100,
                                            Emails: ImmutableList<Email>.Empty);

            var expectedPerson = new Person(Id: 1,
                                            Name: "Son",
                                            FirstName: "Goku Updated",
                                            Age: 100,
                                            Emails: ImmutableList<Email>.Empty);

            return Client.AssertPutAsync("api/v1/persons", personToUpdate, expectedPerson);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PUT")]
        public Task ObjectResponse_Should_Update_Person_With_Status_Code()
        {
            var personToUpdate = new Person(Id: 2,
                                            Name: "Vegeta",
                                            FirstName: "Prince",
                                            Age: 80,
                                            Emails: ImmutableList<Email>.Empty);

            var expectedPerson = new Person(Id: 2,
                                            Name: "Vegeta",
                                            FirstName: "Prince",
                                            Age: 80,
                                            Emails: ImmutableList<Email>.Empty);

            return Client.AssertPutAsync("api/v1/persons",
                                         personToUpdate,
                                         expectedPerson,
                                         expectedHttpStatusCode: System.Net.HttpStatusCode.OK);
        }
    }
}