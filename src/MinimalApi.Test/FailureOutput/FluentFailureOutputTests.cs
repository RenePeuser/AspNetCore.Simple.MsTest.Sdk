using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FailureOutput
{
    /// <summary>
    /// Consumer tests for what a FAILING fluent chain prints about the caller's arguments.
    ///
    /// <para>
    /// The classic API captures the caller's argument via <c>[CallerArgumentExpression]</c>: a literal
    /// (<c>"Missing.json"</c>) is not repeated as "Parameter", a variable (<c>snapshotFile</c>) is named. The
    /// fluent builders used to forward their OWN field expressions to the engine instead, so the output read
    /// <c>Parameter : expectedResult</c> or <c>Parameter : _body ?? string.Empty</c> - internals that point the
    /// reader nowhere.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("FailureOutput")]
    public sealed class FluentFailureOutputTests : ApiTestBase
    {
        private const string ProfileUrl = "api/v1/persons/1/profile";

        private const string LeakFix = "Check that the fluent builders capture the caller's argument with [CallerArgumentExpression] and pass it explicitly as payloadAsJsonParameterName / expectedResultParameterName to AssertHttpCallAsync.";

        private static void AssertNoBuilderInternals(string message)
        {
            Assert.That.DoesNotContain(message,
                                       "expectedResult",
                                       "The failure output must name what the TEST wrote, not the builder's internal variable.",
                                       LeakFix);

            Assert.That.DoesNotContain(message,
                                       "_body",
                                       "The failure output must name what the TEST wrote, not the builder's internal field.",
                                       LeakFix);
        }

        [TestMethod]
        public async Task Missing_Snapshot_Given_As_Literal_Should_Not_Print_A_Parameter_Line()
        {
            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                                  .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                                  .ExpectedResponseFromEmbeddedJson("DoesNotExist.json")
                                                                                                  .ExecuteAsync(),
                                                                                      because: "A snapshot that does not exist has to fail the test.",
                                                                                      fix: "Check the strict snapshot resolution.").ConfigureAwait(false);

            AssertNoBuilderInternals(failure.Message);

            Assert.That.DoesNotContain(failure.Message,
                                       "Parameter    :",
                                       "The literal IS the reference - like the classic API, the output must not repeat it as a separate 'Parameter' line.",
                                       LeakFix);

            Assert.That.Contains(failure.Message,
                                 "DoesNotExist.json",
                                 "The reference the author wrote is what they can act on.",
                                 "Check that the snapshot handler prints the unresolved reference.");
        }

        [TestMethod]
        public async Task Missing_Snapshot_Given_As_Variable_Should_Name_The_Variable()
        {
            var snapshotFile = "DoesNotExist.json";

            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                                  .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                                  .ExpectedResponseFromEmbeddedJson(snapshotFile)
                                                                                                  .ExecuteAsync(),
                                                                                      because: "A snapshot that does not exist has to fail the test.",
                                                                                      fix: "Check the strict snapshot resolution.").ConfigureAwait(false);

            AssertNoBuilderInternals(failure.Message);

            Assert.That.Contains(failure.Message,
                                 "Parameter    : snapshotFile",
                                 "When the reference comes from a variable, the output names that variable - exactly like the classic API.",
                                 LeakFix);
        }

        [TestMethod]
        public async Task Missing_Payload_Given_As_Variable_Should_Name_The_Variable()
        {
            var payloadFile = "DoesNotExist.json";

            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                                  .AcceptsFromEmbeddedJson(payloadFile)
                                                                                                  .Produces<Person>(HttpStatusCode.Created)
                                                                                                  .ExecuteAsync(),
                                                                                      because: "A payload that does not exist has to stop the test.",
                                                                                      fix: "Check the strict payload resolution.").ConfigureAwait(false);

            AssertNoBuilderInternals(failure.Message);

            Assert.That.Contains(failure.Message,
                                 "Parameter    : payloadFile",
                                 "The payload variable the test passed has to be named.",
                                 LeakFix);
        }

        [TestMethod]
        public async Task Missing_Payload_On_Status_Only_Chain_Should_Name_The_Variable()
        {
            // Produces(code) without <T> runs through a different builder (HttpExpectationBuilder).
            var payloadFile = "DoesNotExist.json";

            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                                  .AcceptsFromEmbeddedJson(payloadFile)
                                                                                                  .Produces(HttpStatusCode.Created)
                                                                                                  .ExecuteAsync(),
                                                                                      because: "A payload that does not exist has to stop the test.",
                                                                                      fix: "Check the strict payload resolution.").ConfigureAwait(false);

            AssertNoBuilderInternals(failure.Message);

            Assert.That.Contains(failure.Message,
                                 "Parameter    : payloadFile",
                                 "The status-only chain must report the payload like the typed chain does.",
                                 LeakFix);
        }

        [TestMethod]
        public async Task Json_Type_Mismatch_Should_Name_The_Callers_Argument()
        {
            // The endpoint returns an object; the expected JSON is an array.
            var arrayJson = "[{\"id\":1}]";

            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                                  .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                                  .ExpectedResponseFromJsonString(arrayJson)
                                                                                                  .ExecuteAsync(),
                                                                                      because: "An array cannot describe an object response.",
                                                                                      fix: "Check the JSON type mismatch detection.").ConfigureAwait(false);

            AssertNoBuilderInternals(failure.Message);

            Assert.That.Contains(failure.Message,
                                 "arrayJson",
                                 "The mismatch report names the argument that holds the wrong JSON - it has to be the test's own variable.",
                                 LeakFix);
        }
    }
}
