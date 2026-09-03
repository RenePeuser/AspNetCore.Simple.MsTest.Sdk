using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ParameterizedSnapshot
{
    /// <summary>
    /// Pins down what an assert says when a snapshot carries a placeholder nobody supplied a parameter for.
    ///
    /// Left alone that produces one of the two least helpful failures the sdk can produce. A quoted
    /// placeholder parses, so the comparison runs and reports <c>"$name$"</c> against <c>Son</c> - a value
    /// mismatch that says nothing about the missing parameter. A bare one, standing in for a number, does
    /// not parse at all and used to arrive as a syntax error pointing at a dollar sign. In both cases the
    /// one sentence that ends the search - "you never passed this parameter" - was missing.
    ///
    /// The third test is the reason none of this may be a pre-flight check: a placeholder can be DATA. A
    /// mail template travels through the api as <c>$SurveyLink$</c> and comes back unchanged, both sides
    /// carry the same text, and the assert passes. Anything that flagged unresolved placeholders up front
    /// would turn that legitimate test red, which is why the hint only ever appears on an assert that is
    /// already failing.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("ParameterizedSnapshot")]
    public sealed class UnresolvedParameterTests : ApiTestBase
    {
        private const string PersonUrl = "api/v1/persons/1";

        [TestMethod]
        [TestCategory("GET")]
        public async Task AQuotedPlaceholderWithoutAParameterMustSayWhichOneIsMissing()
        {
            var exception = await Assert.That
                                        .ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<Person>(PersonUrl, "Responses.ForgottenQuotedParameter.json"),
                                                                                   because: "A quoted placeholder parses, so the snapshot reaches the comparison and fails on the value. That failure has to name the parameter - without it the report shows \"$name$\" against Son and the reader has to work out on their own that a parameters entry is missing.",
                                                                                   fix: "Check UnresolvedParameterSectionBuilder and that HttpResponseOutputStrategy renders its section.")
                                        .ConfigureAwait(false);

            Assert.That.Contains(exception.Message,
                                 "$name$",
                                 because: "The hint has to name the placeholder. Reporting that some parameter is missing, without saying which, leaves exactly the search this section exists to prevent.",
                                 fix: "Check UnresolvedParameterSectionBuilder.Findings - it reads the placeholder off the expected side of each difference.");

            Assert.That.Contains(exception.Message,
                                 "Unresolved Parameter",
                                 because: "The hint has to be its own titled section. Buried in the diff table it reads as one more mismatched value instead of the cause of all of them.",
                                 fix: "Check that HttpResponseOutputStrategy appends the unresolved parameter section before the expected snapshot section.");
        }

        [TestMethod]
        [TestCategory("GET")]
        public async Task ABarePlaceholderWithoutAParameterMustSayWhichOneIsMissing()
        {
            var exception = await Assert.That
                                        .ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<Person>(PersonUrl, "Responses.ForgottenBareParameter.json"),
                                                                                   because: "A bare placeholder stands in for a number and is not json until the parameter is applied. Without one the snapshot cannot be read at all, and that has to be reported as the missing parameter it is - not as a stray dollar sign on line 5.",
                                                                                   fix: "Check SnapshotReferenceGuard.EnsureParseable and InvalidSnapshotJsonErrorHandler - the guard raises InvalidSnapshotJsonException and the handler renders it as a formatted assert failure.")
                                        .ConfigureAwait(false);

            // Deliberately the whole sentence, not just "$age$": the report prints the file content as
            // well, so the bare name alone would be found there and this test would pass without the hint
            // ever being rendered.
            Assert.That.Contains(exception.Message,
                                 "Still unreplaced after the parameters were applied: $age$",
                                 because: "The parse error has to name the placeholder that survived. The parser only ever reports the position of a dollar sign, which is the symptom rather than the cause.",
                                 fix: "Check InvalidSnapshotJsonErrorHandler.UnresolvedPlaceholders - it reads the names off the already resolved content via PlaceholderJson.AllTokens.");
        }

        [TestMethod]
        [TestCategory("POST")]
        public Task APlaceholderThatIsRealDataMustNotBeReported()
        {
            // The api echoes the person back, so $SurveyLink$ arrives in the response exactly as it was
            // sent - the same round trip a mail template makes. No parameters, and nothing to report.
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "Payloads.EchoedPlaceholder.json",
                                                  "Responses.EchoedPlaceholder.json",
                                                  expectedHttpStatusCode: HttpStatusCode.OK);
        }
    }
}
