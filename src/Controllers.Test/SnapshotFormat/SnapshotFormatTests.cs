using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.SnapshotFormat
{
    /// <summary>
    /// Snapshots exist in two shapes - the full response envelope and the bare body. Writing always
    /// emitted the envelope, so the first write to an old bare-body snapshot silently migrated it.
    /// With write response enabled globally that rewrites whole folders in one run.
    ///
    /// An existing snapshot now keeps the shape it has; only a new one gets the envelope.
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotFormat")]
    public sealed class SnapshotFormatTests : ApiTestBase
    {
        [TestMethod]
        public async Task RewritingABareBodySnapshotMustNotMigrateItToTheEnvelope()
        {
            var snapshot = SnapshotPath("BareBody.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            Assert.That.IsFalse(SnapshotShape.IsEnvelope(JToken.Parse(original)),
                                because: "This test is about NOT migrating a bare body to the envelope. If the fixture already is an envelope there is nothing left to migrate and the test would pass for the wrong reason.",
                                fix: "Restore Responses\\BareBody.json to a bare body (a plain array of persons, no 'content'/'statusCode' wrapper). A previous run may have rewritten it - that is the very bug under test.");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                 "Responses.BareBody.json",
                                                                 writeResponse: true)
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.IsFalse(SnapshotShape.IsEnvelope(written),
                                    because: "An existing snapshot has to keep the shape it has. Writing always emitted the envelope, so with write response enabled globally one run rewrote whole folders of bare-body snapshots.",
                                    fix: "Check SnapshotShape.MatchExisting: when the existing content is a bare body the envelope has to be unwrapped again before writing.");

                Assert.That.IsAssignableTo<JArray>(written,
                                                   because: "A bare body of this endpoint is the array of persons itself - an object here means the envelope wrapper survived.",
                                                   fix: "See SnapshotShape.MatchExisting - it has to hand back the 'content.value' payload, not the whole envelope.");

                Assert.That.AreEqual(2,
                                     ((JArray)written).Count,
                                     because: "Keeping the shape must not cost content: the endpoint returns two persons and both have to end up in the rewritten snapshot.",
                                     fix: "Check that unwrapping in SnapshotShape.MatchExisting takes the complete 'content.value' array instead of a single element.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task RewritingAnEnvelopeSnapshotMustKeepTheEnvelope()
        {
            var snapshot = SnapshotPath("Envelope.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            Assert.That.IsNotNull(JToken.Parse(original)["content"],
                                  because: "This test is the mirror image of the bare-body one: it proves an envelope stays an envelope. Without a 'content' wrapper in the fixture there is nothing to preserve.",
                                  fix: "Restore Responses\\Envelope.json to the full response envelope ('content' with 'value', plus 'statusCode').");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                 "Responses.Envelope.json",
                                                                 writeResponse: true)
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.IsNotNull(written["content"],
                                      because: "Keeping the existing shape has to work in both directions - an envelope must not be flattened to a bare body either, or the recorded status code and headers are lost.",
                                      fix: "Check SnapshotShape.MatchExisting: when the existing content is an envelope it has to write the envelope through unchanged.");

                Assert.That.IsNotNull(written["content"]!["value"],
                                      because: "An envelope without its payload is worse than a flattened one: the file still looks right but the actual response body is gone.",
                                      fix: "Check that the envelope written by the response writer carries content.value and that MatchExisting does not strip it.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public void ShapeDetectionMustNotMistakeABodyForAnEnvelope()
        {
            // A body may well carry a property called "content" - only the envelope pairs it with a
            // "value" AND a status code.
            var body = JToken.Parse("""{ "content": { "value": "a blog post" } }""");

            Assert.That.IsFalse(SnapshotShape.IsEnvelope(body),
                                because: "A perfectly ordinary body may carry a property called 'content' - a blog post is the obvious case. Treating it as an envelope would unwrap the real payload away.",
                                fix: "SnapshotShape.IsEnvelope must require content, content.value AND a statusCode together - any single one of them is not enough.");

            var envelope = JToken.Parse("""{ "content": { "value": {} }, "statusCode": "OK" }""");

            Assert.That.IsTrue(SnapshotShape.IsEnvelope(envelope),
                               because: "The full triple content + content.value + statusCode only occurs in the envelope, so this shape has to be recognised - otherwise every envelope would be rewritten as a bare body.",
                               fix: "Check the property lookups in SnapshotShape.IsEnvelope - an empty 'value' object still counts as present.");
        }

        [TestMethod]
        public void MatchExistingMustLeaveANewSnapshotAsAnEnvelope()
        {
            const string envelope = /*lang=json,strict*/ """{ "content": { "value": [1,2] }, "statusCode": "OK" }""";

            // No existing content - nothing to preserve, the envelope stays.
            Assert.That.AreEqual(envelope,
                                 SnapshotShape.MatchExisting(envelope, existingContent: null),
                                 because: "With no existing content there is no shape to preserve, so the new snapshot keeps the envelope - that is the richer format and the intended default for anything created from now on.",
                                 fix: "Check the null branch in SnapshotShape.MatchExisting: it has to return the envelope unchanged instead of unwrapping by default.");

            var unwrapped = SnapshotShape.MatchExisting(envelope, existingContent: "[1,2]");

            Assert.That.AreEqual(new[] { 1, 2 },
                                 JArray.Parse(unwrapped).Select(item => (int)item).ToList(),
                                 because: "When the existing snapshot is a bare body the envelope has to be unwrapped down to exactly its payload - same elements, same order, nothing of the wrapper left over.",
                                 fix: "Check that SnapshotShape.MatchExisting returns content.value verbatim when the existing content is not an envelope.");
        }

        private static string SnapshotPath(string fileName,
                                           [System.Runtime.CompilerServices.CallerFilePath]
                                           string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }
    }
}