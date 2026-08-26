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

            Assert.IsFalse(SnapshotShape.IsEnvelope(JToken.Parse(original)),
                           "The fixture has to be a bare body, otherwise this test proves nothing.");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                  "Responses.BareBody.json",
                                                                  writeResponse: true)
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.IsFalse(SnapshotShape.IsEnvelope(written),
                               "The snapshot was migrated to the response envelope.");

                Assert.IsInstanceOfType<JArray>(written, "A bare body of this endpoint is an array of persons.");
                Assert.AreEqual(2, ((JArray)written).Count);
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

            Assert.IsNotNull(JToken.Parse(original)["content"], "The fixture has to be an envelope.");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                  "Responses.Envelope.json",
                                                                  writeResponse: true)
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.IsNotNull(written["content"], "The envelope was flattened to a bare body.");
                Assert.IsNotNull(written["content"]!["value"]);
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

            Assert.IsFalse(SnapshotShape.IsEnvelope(body));

            var envelope = JToken.Parse("""{ "content": { "value": {} }, "statusCode": "OK" }""");

            Assert.IsTrue(SnapshotShape.IsEnvelope(envelope));
        }

        [TestMethod]
        public void MatchExistingMustLeaveANewSnapshotAsAnEnvelope()
        {
            const string Envelope = """{ "content": { "value": [1,2] }, "statusCode": "OK" }""";

            // No existing content - nothing to preserve, the envelope stays.
            Assert.AreEqual(Envelope, SnapshotShape.MatchExisting(Envelope, existingContent: null));

            var unwrapped = SnapshotShape.MatchExisting(Envelope, existingContent: "[1,2]");

            CollectionAssert.AreEqual(new[] { 1, 2 }, JArray.Parse(unwrapped).Select(item => (int)item).ToList());
        }

        private static string SnapshotPath(string fileName,
                                           [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }
    }
}
