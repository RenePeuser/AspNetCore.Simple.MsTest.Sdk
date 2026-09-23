using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Fluent API tests for DELETE /api/v1/persons in the CONTROLLER stack.
    /// <c>AssertDelete</c> had no fluent coverage in either stack, while README and EXAMPLES documented it.
    /// </summary>
    public partial class PersonDeleteTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("DELETE")]
        public Task Fluent_Should_Delete_Person_NoContent()
        {
            return Client.AssertDelete("api/v1/persons/1")
                         .Produces(HttpStatusCode.NoContent)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("DELETE")]
        public Task Fluent_Should_Delete_Person_With_Url_Placeholder()
        {
            // The body-less path used to send the url verbatim, so "$Id$" reached the server and 404'd.
            return Client.AssertDelete("api/v1/persons/$Id$")
                         .WithParameter("Id", 1)
                         .Produces(HttpStatusCode.NoContent)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("DELETE")]
        public Task Fluent_Should_Delete_Person_With_Typed_Response()
        {
            // deletedAt is a wall-clock timestamp — the snapshot keeps a placeholder for it.
            return Client.AssertDelete("api/v1/persons/1/with-response")
                         .Produces<DeletePersonResponse>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("DeletePersonWithResponse.json")
                         .IgnoreProperty<DeletePersonResponse>(r => r.DeletedAt)
                         .ExecuteAsync();
        }
    }
}