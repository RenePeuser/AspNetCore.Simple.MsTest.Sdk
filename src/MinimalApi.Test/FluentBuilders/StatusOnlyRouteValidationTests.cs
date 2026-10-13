using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for the endpoint validation of the UNTYPED fluent paths <c>Produces(code)</c> and
    /// <c>Produces(code).ExpectedResponse(obj)</c>.
    ///
    /// <para>
    /// Both skipped the whole endpoint validation, because they name no response type to compare. That also
    /// skipped the ROUTE check: a misspelled url answered 404, a wrong verb 405 - and a status-only test
    /// expecting exactly that went green against an endpoint that does not exist. Now the route (verb, url,
    /// api version) is validated, the response type is not; <c>.SkipEndpointValidation()</c> after <c>Produces…</c>
    /// turns both off.
    /// </para>
    ///
    /// <para>Every positive case is paired with a test that MUST fail.</para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class StatusOnlyRouteValidationTests : ApiTestBase
    {
        private const string RouteFix = "Check that Produces(code) validates the ROUTE (validateRouteOnly) instead of skipping the endpoint validation.";

        private const string TypeFix = "Check that Produces(code) validates the route ONLY - it names no response type, so the type must not be compared.";

        private const string SkipFix = "Check that HttpExpectationBuilder.SkipEndpointValidation() sets the flag on a COPY and that ExecuteAsync / ExpectedResponse hand it to the engine.";

        // ============================================================
        // Existing route → passes, whatever the endpoint returns
        // ============================================================

        [TestMethod]
        public Task Get_Unknown_Id_On_Existing_Route_Should_Pass()
        {
            return Client.AssertGet("api/v1/persons/999")
                         .Produces(HttpStatusCode.NotFound)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Route_With_Body_Should_Pass_Without_A_Type()
        {
            // persons/{id} declares Person - status-only names no type, so nothing may be compared against it.
            return Client.AssertGet("api/v1/persons/1")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Route_With_Placeholder_Should_Validate_The_Resolved_Url()
        {
            return Client.AssertGet("api/v1/persons/$Id$")
                         .WithParameter("Id", 2)
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Route_With_Query_Should_Pass()
        {
            return Client.AssertGet("api/v1/persons?name=$Name$")
                         .WithParameter("Name", "Goku")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Post_Existing_Route_Should_Pass()
        {
            return Client.AssertPost("api/v1/persons/echo-request")
                         .Accepts(new { Name = "Goku" })
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Delete_Unknown_Id_On_Existing_Route_Should_Pass()
        {
            return Client.AssertDelete("api/v1/persons/999")
                         .Produces(HttpStatusCode.NotFound)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Route_With_Body_Expecting_Wrong_Status_Should_Still_Compare_The_Status()
        {
            return Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persons/1")
                                                                                     .Produces(HttpStatusCode.NotFound)
                                                                                     .ExecuteAsync(),
                                                                         because: "persons/1 exists and answers 200 - validating the route must not replace the status comparison.",
                                                                         fix: TypeFix);
        }

        [TestMethod]
        public async Task Get_Route_With_Body_Expecting_Wrong_Status_Is_Not_Reported_As_Endpoint_Not_Found()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persons/1")
                                                                                                    .Produces(HttpStatusCode.NotFound)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "persons/1 answers 200, expecting 404 has to fail.",
                                                                                        fix: TypeFix).ConfigureAwait(false);

            Assert.That.DoesNotContain(exception.Message,
                                       "ENDPOINT NOT FOUND",
                                       "The route exists - only the status differs, the report must say so.",
                                       TypeFix);
        }

        // ============================================================
        // Route does not exist → ENDPOINT NOT FOUND, before the call
        // ============================================================

        [TestMethod]
        public Task Get_Misspelled_Route_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/persnos/1")
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExecuteAsync(),
                                          "'persnos' is a typo - the 404 comes from the router, not from an endpoint.");
        }

        [TestMethod]
        public Task Get_Misspelled_Route_With_Placeholder_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/persnos/$Id$")
                                                      .WithParameter("Id", 1)
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExecuteAsync(),
                                          "'persnos' is a typo - placeholders do not change that.");
        }

        // Wrong-verb samples deliberately use paths WITHOUT a parameterized sibling: the route matcher falls back to
        // ignoring constraints (so "persons/-1" still resolves to "persons/{id:guid}" in error tests), which lets
        // GET "persons/echo-request" resolve to GET "persons/{id:long}" - existing design, not part of this check.
        [TestMethod]
        public Task Get_Wrong_Verb_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/errors/not-implemented")
                                                      .Produces(HttpStatusCode.MethodNotAllowed)
                                                      .ExecuteAsync(),
                                          "errors/not-implemented has no GET - the 405 comes from the router, not from an endpoint.");
        }

        [TestMethod]
        public Task Delete_Wrong_Verb_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertDelete("api/v1/native-types/int")
                                                      .Produces(HttpStatusCode.MethodNotAllowed)
                                                      .ExecuteAsync(),
                                          "native-types/int has no DELETE.");
        }

        [TestMethod]
        public Task Post_Misspelled_Route_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertPost("api/v1/persnos")
                                                      .Accepts(new { Name = "Goku" })
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExecuteAsync(),
                                          "'persnos' is a typo - a body verb is validated the same way.");
        }

        [TestMethod]
        public Task Get_Unknown_Api_Version_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v2/persons/1")
                                                      .Produces(HttpStatusCode.NotFound)
                                                      .ExecuteAsync(),
                                          "persons/{id} exists in v1 only - v2 is no endpoint.");
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
        public Task Expected_Response_On_Wrong_Verb_Should_Fail()
        {
            return AssertEndpointNotFound(() => Client.AssertGet("api/v1/errors/not-implemented")
                                                      .Produces(HttpStatusCode.MethodNotAllowed)
                                                      .ExpectedResponse(new { Title = "Method Not Allowed" })
                                                      .ExecuteAsync(),
                                          "errors/not-implemented has no GET.");
        }

        [TestMethod]
        public async Task Endpoint_Not_Found_Should_Show_How_To_Test_A_Deliberate_404()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persnos/1")
                                                                                                    .Produces(HttpStatusCode.NotFound)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "'persnos' is a typo.",
                                                                                        fix: RouteFix).ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 ".SkipEndpointValidation()",
                                 "A test that hits an unknown route ON PURPOSE must learn from the output how to say so.",
                                 "Check EndpointValidationOutputBuilder.BuildEndpointRegistrationInfo - the not-found hint names SkipEndpointValidation.");
        }

        // ============================================================
        // Expected response on an existing route → only the route is validated
        // ============================================================

        [TestMethod]
        public Task Expected_Response_On_Existing_Route_Should_Pass()
        {
            return Client.AssertPost("api/v1/persons/echo-request")
                         .Accepts(new { Name = "Goku" })
                         .Produces(HttpStatusCode.OK)
                         .ExpectedResponse(new { CorrelationId = "(absent)", Name = "Goku" })
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Expected_Response_With_Other_Type_Than_Declared_Should_Pass()
        {
            // The endpoint declares Person; the anonymous object is another type - only the route is validated.
            return Client.AssertGet("api/v1/persons/1")
                         .Produces(HttpStatusCode.OK)
                         .ExpectedResponse(new { Id = 1, FirstName = "Son", LastName = "Goku" })
                         .IgnoreDifferencesWhere(_ => true)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Expected_Response_With_Wrong_Value_Should_Fail()
        {
            return Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons/echo-request")
                                                                                     .Accepts(new { Name = "Goku" })
                                                                                     .Produces(HttpStatusCode.OK)
                                                                                     .ExpectedResponse(new { CorrelationId = "(absent)", Name = "Vegeta" })
                                                                                     .ExecuteAsync(),
                                                                         because: "The endpoint echoes 'Goku' - validating the route must not replace the body comparison.",
                                                                         fix: TypeFix);
        }

        // ============================================================
        // .SkipEndpointValidation() → a deliberate 404 / 405 test
        // ============================================================

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

        [TestMethod]
        public Task Post_Misspelled_Route_With_Skip_Should_Pass()
        {
            return Client.AssertPost("api/v1/persnos")
                         .Accepts(new { Name = "Goku" })
                         .Produces(HttpStatusCode.NotFound)
                         .SkipEndpointValidation()
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Unknown_Api_Version_With_Skip_Should_Pass()
        {
            return Client.AssertGet("api/v2/persons/1")
                         .Produces(HttpStatusCode.NotFound)
                         .SkipEndpointValidation()
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Expected_Response_On_Misspelled_Route_With_Skip_Before_Expected_Should_Pass()
        {
            return Client.AssertGet("api/v1/persnos/1")
                         .Produces(HttpStatusCode.NotFound)
                         .SkipEndpointValidation()
                         .ExpectedResponse(new { Status = 404 })
                         .IgnoreDifferencesWhere(_ => true)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Expected_Response_On_Misspelled_Route_With_Skip_After_Expected_Should_Pass()
        {
            return Client.AssertGet("api/v1/persnos/1")
                         .Produces(HttpStatusCode.NotFound)
                         .ExpectedResponse(new { Status = 404 })
                         .IgnoreDifferencesWhere(_ => true)
                         .SkipEndpointValidation()
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Get_Misspelled_Route_With_Skip_Still_Compares_The_Status()
        {
            return Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persnos/1")
                                                                                     .Produces(HttpStatusCode.OK)
                                                                                     .SkipEndpointValidation()
                                                                                     .ExecuteAsync(),
                                                                         because: "Skipping the endpoint validation must not skip the status comparison - the router answers 404, not 200.",
                                                                         fix: SkipFix);
        }

        [TestMethod]
        public async Task Skip_On_A_Stored_Stage_Does_Not_Leak_Into_A_Sibling()
        {
            var baseExpectation = Client.AssertGet("api/v1/persnos/1")
                                        .Produces(HttpStatusCode.NotFound);

            await baseExpectation.SkipEndpointValidation()
                                 .ExecuteAsync()
                                 .ConfigureAwait(false);

            await AssertEndpointNotFound(() => baseExpectation.ExecuteAsync(),
                                         "The sibling branch never called SkipEndpointValidation() - it has to validate the route.").ConfigureAwait(false);
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
