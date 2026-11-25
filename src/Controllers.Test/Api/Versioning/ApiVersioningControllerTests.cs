using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Versioning
{
    /// <summary>
    ///     V1 and V2 run side by side on one controller: "versioning/number" exists once per version,
    ///     "versioning/shared" is one action mapped to both, "versioning/implicit" has no mapping and
    ///     serves every version of the controller. Endpoint validation has to tell them apart by the
    ///     version each action is mapped to, not by the first [ApiVersion] of the controller.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("Api Versioning")]
    public class ApiVersioningControllerTests : ApiTestBase
    {
        [TestMethod]
        public Task Same_Route_In_V1_Resolves_The_V1_Endpoint()
        {
            return Client.AssertGetAsync<int>("api/v1/versioning/number", expectedResult: "1");
        }

        [TestMethod]
        public Task Same_Route_In_V2_Resolves_The_V2_Endpoint()
        {
            return Client.AssertGetAsync("api/v2/versioning/number", "one");
        }

        [TestMethod]
        public Task Endpoint_Mapped_To_V1_And_V2_Is_Found_Under_V1()
        {
            return Client.AssertGetAsync<int>("api/v1/versioning/shared", expectedResult: "7");
        }

        [TestMethod]
        public Task Endpoint_Mapped_To_V1_And_V2_Is_Found_Under_V2()
        {
            return Client.AssertGetAsync<int>("api/v2/versioning/shared", expectedResult: "7");
        }

        [TestMethod]
        public Task Endpoint_Without_Mapping_Is_Found_Under_V1()
        {
            return Client.AssertGetAsync<int>("api/v1/versioning/implicit", expectedResult: "9");
        }

        [TestMethod]
        public Task Endpoint_Without_Mapping_Is_Found_Under_V2()
        {
            return Client.AssertGetAsync<int>("api/v2/versioning/implicit", expectedResult: "9");
        }

        [TestMethod]
        public async Task V1_Response_Type_Is_Not_Accepted_For_The_V2_Endpoint()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<int>("api/v2/versioning/number", expectedResult: "1"),
                                                                                        because: "V2 of 'versioning/number' declares string, only V1 declares int. Accepting <int> under V2 means validation looked at the V1 action - the version in the url was ignored.",
                                                                                        fix: "Check that EndpointInfoParser creates one EndpointInfo per version an action is mapped to ([MapToApiVersion]) instead of filing every action under the controller's first [ApiVersion].")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V2 endpoint exists - the only thing wrong is the expected type. ENDPOINT NOT FOUND or AMBIGUOUS ENDPOINT MATCH would mean the V1 and V2 actions were not told apart.",
                                 fix: "Check that the V2 action is registered with ApiVersion 2.0 and the V1 action with 1.0, so the version from the url picks exactly one of them.");
        }

        [TestMethod]
        public async Task V2_Response_Type_Is_Not_Accepted_For_The_V1_Endpoint()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync("api/v1/versioning/number", "one"),
                                                                                        because: "V1 of 'versioning/number' declares int, only V2 declares string. Accepting <string> under V1 means validation looked at the V2 action.",
                                                                                        fix: "Check that the version an action is mapped to ([MapToApiVersion]) decides which EndpointInfo the url resolves to.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V1 endpoint exists - the only thing wrong is the expected type.",
                                 fix: "Check that the V1 action is registered with ApiVersion 1.0 only.");
        }

        [TestMethod]
        public async Task Version_The_Controller_Does_Not_Declare_Is_Not_Found()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<int>("api/v3/versioning/shared", expectedResult: "7"),
                                                                                        because: "The controller declares V1 and V2 only. Finding a V3 endpoint means the versions are not taken from the declaration at all.",
                                                                                        fix: "Check that EndpointInfoParser only emits EndpointInfos for versions declared via [ApiVersion] / [MapToApiVersion].")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "ENDPOINT NOT FOUND",
                                 because: "There is no V3 at all - the report has to say the endpoint does not exist rather than complain about types.",
                                 fix: "Check that the endpoint lookup compares the requested version with the version of each EndpointInfo.");
        }

        // ── Fluent ──────────────────────────────────────────────────────────────────────

        [TestMethod]
        public Task Fluent_Same_Route_In_V1_Resolves_The_V1_Endpoint()
        {
            return Client.AssertGet("api/v1/versioning/number")
                         .Produces<int>(HttpStatusCode.OK)
                         .ExpectedResponse(1)
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Fluent_Same_Route_In_V2_Resolves_The_V2_Endpoint()
        {
            return Client.AssertGet("api/v2/versioning/number")
                         .Produces<string>(HttpStatusCode.OK)
                         .ExpectedResponse("one")
                         .ExecuteAsync();
        }

        [TestMethod]
        public Task Fluent_Endpoint_Mapped_To_V1_And_V2_Is_Found_Under_V2()
        {
            return Client.AssertGet("api/v2/versioning/shared")
                         .Produces<int>(HttpStatusCode.OK)
                         .ExpectedResponse(7)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_V1_Response_Type_Is_Not_Accepted_For_The_V2_Endpoint()
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v2/versioning/number")
                                                                                                    .Produces<int>(HttpStatusCode.OK)
                                                                                                    .ExpectedResponse(1)
                                                                                                    .ExecuteAsync(),
                                                                                        because: "The fluent chain must validate against the same per-version endpoint as the classic api: V2 declares string, so <int> has to be refused.",
                                                                                        fix: "Check that the fluent builder hands the url to the same ApiVersionResolver / EndpointProvider path as AssertGetAsync.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The V2 endpoint exists - only the expected type is wrong.",
                                 fix: "Check that V1 and V2 of 'versioning/number' are separate EndpointInfos.");
        }
    }
}
