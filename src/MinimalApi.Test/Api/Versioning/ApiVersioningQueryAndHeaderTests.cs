using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Versioning
{
    /// <summary>
    ///     V1 and V2 of "number" side by side, but the route carries no version: the client sends it as
    ///     ?api-version=… or as x-api-version header. Endpoint validation has to read the version from the
    ///     same place the api does - otherwise both endpoints match the url and the test reports
    ///     AMBIGUOUS ENDPOINT MATCH, or the version is ignored and the wrong response type is accepted.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Api Versioning")]
    public class ApiVersioningQueryAndHeaderTests : ApiTestBase
    {
        private const string QueryUrl = "api/versioning-query/number";
        private const string HeaderUrl = "api/versioning-header/number";

        // ── Query string ────────────────────────────────────────────────────────────────

        [TestMethod]
        public Task Query_Version_1_Resolves_The_V1_Endpoint()
        {
            return Client.AssertGetAsync<int>(QueryUrl + "?api-version=1", expectedResult: "1");
        }

        [TestMethod]
        public Task Query_Version_2_Resolves_The_V2_Endpoint()
        {
            return Client.AssertGetAsync(QueryUrl + "?api-version=2", "one");
        }

        [TestMethod]
        public Task Query_Version_With_Minor_Resolves_The_Same_Endpoint()
        {
            // "2.0" and "2" are the same version - a text comparison would report ENDPOINT NOT FOUND
            return Client.AssertGetAsync(QueryUrl + "?api-version=2.0", "one");
        }

        [TestMethod]
        public Task Fluent_Query_Version_Resolves_The_V2_Endpoint()
        {
            return Client.AssertGet(QueryUrl)
                         .WithQueryParameter("api-version", 2)
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Query_Version_2_Does_Not_Accept_The_V1_Response_Type()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<int>(QueryUrl + "?api-version=2", expectedResult: "1"),
                                                                                        because: "V2 declares string, only V1 declares int. Accepting <int> for ?api-version=2 means the query version was ignored.",
                                                                                        fix: "Check that ApiVersionResolver reads ?api-version=… and EndpointProvider filters the candidates by it.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V2 endpoint exists - only the expected type is wrong. AMBIGUOUS ENDPOINT MATCH would mean V1 and V2 were not told apart.",
                                 fix: "Check that every endpoint gets its own EndpointInfo per mapped version (MapToApiVersion).");
        }

        // ── Header ──────────────────────────────────────────────────────────────────────

        [TestMethod]
        public Task Fluent_Header_Version_1_Resolves_The_V1_Endpoint()
        {
            return Client.AssertGet(HeaderUrl)
                         .WithHeader("x-api-version", "1")
                         .Produces<int>(HttpStatusCode.OK)
                         .ExpectedResponse(1)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Fluent_Header_Version_2_Resolves_The_V2_Endpoint()
        {
            return Client.AssertGet(HeaderUrl)
                         .WithHeader("x-api-version", "2.0")
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_Header_Version_2_Does_Not_Accept_The_V1_Response_Type()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(HeaderUrl)
                                                                                                    .WithHeader("x-api-version", "2")
                                                                                                    .Produces<int>(HttpStatusCode.OK)
                                                                                                    .ExpectedResponse(1)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "V2 declares string, only V1 declares int. Accepting <int> with x-api-version: 2 means the header of the request was ignored.",
                                                                                        fix: "Check that the request headers (WithHeader) reach ApiVersionResolver, not only the client's DefaultRequestHeaders.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V2 endpoint exists - only the expected type is wrong. AMBIGUOUS ENDPOINT MATCH would mean the header version was not read at all.",
                                 fix: "Check that ApiVersionResolver reads the x-api-version request header.");
        }

        [TestMethod]
        public async Task Missing_Version_Is_Reported_As_Ambiguous()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(HeaderUrl)
                                                                                                    .Produces<int>(HttpStatusCode.OK)
                                                                                                    .ExpectedResponse(1)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "Without a version the url matches V1 and V2. Picking one silently would validate against a version the api never serves for this call.",
                                                                                        fix: "Check that EndpointProvider returns no single endpoint when several versions match and no version was requested.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "AMBIGUOUS ENDPOINT MATCH",
                                 because: "The author forgot the version - the report has to show both candidates so the missing header is obvious.",
                                 fix: "Check EndpointValidator: several matches have to end in BuildMultipleMatches.");
        }
    }
}
