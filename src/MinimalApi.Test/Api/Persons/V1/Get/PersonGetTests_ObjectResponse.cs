using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Tests for GET /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertGetAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonGetTests_ObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Get_Single_Person()
        {
            var expectedPerson = new Person(Id: 1,
                                            Name: "Son",
                                            FirstName: "Goku",
                                            Age: 99,
                                            Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                         new Email("abc@hotmail.de", "Microsoft")));

            return Client.AssertGetAsync("api/v1/persons/1", expectedPerson);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Get_Person_With_Status_Code()
        {
            var expectedPerson = new Person(Id: 1,
                                            Name: "Son",
                                            FirstName: "Goku",
                                            Age: 99,
                                            Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                         new Email("abc@hotmail.de", "Microsoft")));

            return Client.AssertGetAsync("api/v1/persons/1",
                                         expectedPerson,
                                         expectedHttpStatusCode: HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Get_Person_With_Filter()
        {
            var expectedPerson = new Person(Id: 999,
                                            Name: "Son",
                                            FirstName: "Goku",
                                            Age: 99,
                                            Emails: ImmutableList<Email>.Empty);

            return Client.AssertGetAsync("api/v1/persons/1",
                                         expectedPerson,
                                         filterFunc: person => person == null
                                                                   ? null
                                                                   : new Person(Id: 999,
                                                                                Name: person.Name,
                                                                                FirstName: person.FirstName,
                                                                                Age: person.Age,
                                                                                Emails: ImmutableList<Email>.Empty));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        public Task ObjectResponse_Should_Get_Person_Ignore_Id()
        {
            var expectedPerson = new Person(Id: 0,
                                            Name: "Son",
                                            FirstName: "Goku",
                                            Age: 99,
                                            Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                         new Email("abc@hotmail.de", "Microsoft")));

            return Client.AssertGetAsync("api/v1/persons/1",
                                         expectedPerson,
                                         differenceFunc: TestHelpers.IgnoreIdDifferences);
        }
    }
}