using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Probes.DynamicRequestLocator.Status_200_Ok
{
    /// <summary>
    /// Probe G4 (NEXT_STEPS "Test gaps"): does <c>[DynamicRequestLocator]</c> still work as the root README advertises?
    /// The README passes ONE file name as payload AND expected response - the engine has to read it from
    /// <c>Requests/</c> for the body and from <c>Responses/</c> for the snapshot.
    ///
    /// <para>
    /// Request and response files differ on purpose: only the response carries <c>correlationId</c>. A green run
    /// therefore proves the snapshot came from <c>Responses/</c>, not from the request file of the same name.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Probe")]
    public sealed class DynamicRequestLocatorProbeTests : ApiTestBase
    {
        private const string EchoRequestUrl = "api/v1/persons/echo-request";

        private const string LocatorFix = "Check DynamicRequestLocatorAttribute.GetData: it strips the class name from the namespace, appends '.Requests' and yields one case per embedded .json there.";

        [TestMethod]
        public void Locator_Should_Yield_One_Case_Per_Request_File()
        {
            var method = typeof(DynamicRequestLocatorProbeTests).GetMethod(nameof(Classic_Chain_Should_Pass_For_Every_Request_File))!;

            var cases = new DynamicRequestLocatorAttribute().GetData(method)
                                                            .Select(data => data.Single()?.ToString())
                                                            .Order()
                                                            .ToArray();

            Assert.That.AreEqual("Goku.json, Vegeta.json", string.Join(", ", cases),
                                 because: "Requests/ holds exactly Goku.json and Vegeta.json - the Responses/ files of the same name must not become cases.",
                                 fix: LocatorFix);
        }

        [TestMethod]
        [DynamicRequestLocator]
        public Task Classic_Chain_Should_Pass_For_Every_Request_File(string useCase)
        {
            return Client.AssertPostAsync<EchoRequestResponse>(EchoRequestUrl,
                                                               useCase,
                                                               useCase);
        }

        [TestMethod]
        [DynamicRequestLocator]
        public Task Fluent_Chain_Should_Pass_For_Every_Request_File(string useCase)
        {
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromEmbeddedJson(useCase)
                         .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson(useCase)
                         .ExecuteAsync();
        }

        [TestMethod]
        [DynamicRequestLocator(HttpStatusCode.OK)]
        public Task Extra_Parameters_Should_Follow_The_File_Name(string useCase,
                                                                 HttpStatusCode statusCode)
        {
            return Client.AssertPost(EchoRequestUrl)
                         .AcceptsFromEmbeddedJson(useCase)
                         .Produces<EchoRequestResponse>(statusCode)
                         .ExpectedResponseFromEmbeddedJson(useCase)
                         .ExecuteAsync();
        }

        [TestMethod]
        [DynamicRequestLocator]
        public async Task Response_Of_The_Other_Case_Should_Fail(string useCase)
        {
            var otherCase = useCase == "Goku.json" ? "Vegeta.json" : "Goku.json";

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoRequestUrl)
                                                                                    .AcceptsFromEmbeddedJson(useCase)
                                                                                    .Produces<EchoRequestResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponseFromEmbeddedJson(otherCase)
                                                                                    .ExecuteAsync(),
                                                                        because: "The request of one case against the snapshot of the other has to fail - otherwise the located files are not compared at all.",
                                                                        fix: LocatorFix).ConfigureAwait(false);
        }
    }
}
