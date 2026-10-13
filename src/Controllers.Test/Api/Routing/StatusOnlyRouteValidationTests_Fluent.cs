using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Routing
{
    /// <summary>
    /// Controller twin of <c>MinimalApi.Test.FluentBuilders.StatusOnlyRouteValidationTests</c>: the untyped
    /// <c>Produces(code)</c> path validates the ROUTE (verb, url, api version) of a controller action, not its type.
    /// It used to skip the endpoint validation entirely, so a misspelled url (404) or a wrong verb (405) went green.
    ///
    /// <para>Every positive case is paired with a test that MUST fail.</para>
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Fluent")]
    public sealed class StatusOnlyRouteValidationTests : ApiTestBase
    {
        private const string RouteFix = "Check that Produces(code) validates the ROUTE (validateRouteOnly) instead of skipping the endpoint validation.";

        [TestMethod]
        public Task Get_Unknown_Id_On_Existing_Action_Should_Pass()
        {
            return Client.AssertGet("api/v1/persons/999")
                         .Produces(HttpStatusCode.NotFound)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Action_With_Body_Should_Pass_Without_A_Type()
        {
            return Client.AssertGet("api/v1/persons/1")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Delete_Unknown_Id_On_Existing_Action_Should_Pass()
        {
            return Client.AssertDelete("api/v1/persons/999")
                         .Produces(HttpStatusCode.NotFound)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Misspelled_Route_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/persnos/1")
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExecuteAsync(),
                                          "'persnos' is a typo - the 404 comes from the router, not from an action.");
        }

        [TestMethod]
        public Task Get_Wrong_Verb_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/errors/not-implemented")
                                                      .Produces(HttpStatusCode.MethodNotAllowed)
                                                      .ExecuteAsync(),
                                          "errors/not-implemented is POST only - the 405 comes from the router, not from an action.");
        }

        [TestMethod]
        public Task Delete_Wrong_Verb_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertDelete("api/v1/native-types/int")
                                                      .Produces(HttpStatusCode.MethodNotAllowed)
                                                      .ExecuteAsync(),
                                          "native-types/int has no DELETE action.");
        }

        [TestMethod]
        public Task Expected_Response_On_Misspelled_Route_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/persnos/1")
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExpectedResponse(new { Title = "Not Found" })
                                                      .ExecuteAsync(),
                                          "Produces(code).ExpectedResponse(obj) takes its type from the object - the route still has to exist.");
        }

        [TestMethod]
        public Task Get_Misspelled_Route_With_Skip_Should_Pass()
        {
            return Client.AssertGet("api/v1/persnos/1")
                         .Produces(HttpStatusCode.NotFound)
                         .SkipEndpointValidation()
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Wrong_Verb_With_Skip_Should_Pass()
        {
            return Client.AssertGet("api/v1/errors/not-implemented")
                         .Produces(HttpStatusCode.MethodNotAllowed)
                         .SkipEndpointValidation()
                         .ExecuteAsync();
        }

        private static async Task AssertEndpointNotFound(Func<Task> action,
                                                         string because)
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(action,
                                                                                        because: because,
                                                                                        fix: RouteFix).ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "ENDPOINT NOT FOUND",
                                 because + " The report has to say that the endpoint does not exist - not show a status or body difference.",
                                 RouteFix);
        }
    }
}
