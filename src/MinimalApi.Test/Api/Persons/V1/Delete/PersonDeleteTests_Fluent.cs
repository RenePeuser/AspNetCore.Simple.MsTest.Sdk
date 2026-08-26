using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Fluent API tests for DELETE /api/v1/persons (Endpoint-Stil: chain ends in ExecuteAsync).
    ///
    /// <para>
    /// <c>AssertDelete</c> was the one entry point with no fluent coverage at all, while both README and
    /// EXAMPLES documented it — so the body-less path's url-placeholder bug could sit here unnoticed.
    /// </para>
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
            // The body-less path used to send the url verbatim, so "$Id$" reached the server and the call
            // 404'd. Only the typed path ran the replacement.
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
            // deletedAt is a wall-clock timestamp — the snapshot keeps a $DeletedAt$ placeholder for it,
            // so it has to be ignored exactly like the native twin does.
            return Client.AssertDelete("api/v1/persons/1/with-response")
                         .Produces<DeletePersonResponse>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("DeletePersonWithResponse.json")
                         .IgnoreProperty<DeletePersonResponse>(r => r.DeletedAt)
                         .ExecuteAsync();
        }
    }
}
