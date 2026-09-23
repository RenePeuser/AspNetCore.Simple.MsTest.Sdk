using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Query
{
    /// <summary>
    /// Tests for QUERY /api/v1/persons endpoint using object-based expectedResponse.
    /// Uses the new AssertQueryAsync overloads that accept TResult expectedResponse instead of JSON strings.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonQueryTestsObjectResponse : ApiTestBase
    {
        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        public Task ObjectResponse_Should_Query_Persons()
        {
            var queryRequest = new
                               {
                                   Name = "Son",
                                   MinAge = 50
                               };

            var expectedPersons = new[]
                                  {
                                      new Person(Id: 1,
                                                 Name: "Son",
                                                 FirstName: "Goku",
                                                 Age: 99,
                                                 Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                              new Email("abc@hotmail.de", "Microsoft")))
                                  };

            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search", queryRequest, expectedPersons);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        public Task ObjectResponse_Should_Query_Persons_With_Status_Code()
        {
            var queryRequest = new { Name = "Vegeta" };

            var expectedPersons = new[]
                                  {
                                      new Person(Id: 2,
                                                 Name: "Vegeta",
                                                 FirstName: "Unknown",
                                                 Age: 77,
                                                 Emails: ImmutableList.Create(new Email("abc@gmx.de", "GMX"),
                                                                              new Email("maxmustermann@hotmail.de", "Microsoft")))
                                  };

            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                                queryRequest,
                                                                expectedPersons,
                                                                expectedHttpStatusCode: HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        public Task ObjectResponse_Should_Query_Multiple_Persons()
        {
            var queryRequest = new { MinAge = 50 };

            var expectedPersons = new[]
                                  {
                                      new Person(Id: 1,
                                                 Name: "Son",
                                                 FirstName: "Goku",
                                                 Age: 99,
                                                 Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                              new Email("abc@hotmail.de", "Microsoft"))),
                                      new Person(Id: 2,
                                                 Name: "Vegeta",
                                                 FirstName: "Unknown",
                                                 Age: 77,
                                                 Emails: ImmutableList.Create(new Email("abc@gmx.de", "GMX"),
                                                                              new Email("maxmustermann@hotmail.de", "Microsoft")))
                                  };

            return Client.AssertQueryAsync<IEnumerable<Person>>("api/v1/persons/search",
                                                                queryRequest,
                                                                expectedPersons);
        }
    }
}