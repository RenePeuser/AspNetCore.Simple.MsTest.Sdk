using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Api.SdkScenarios;
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

            Assert.That.IsFalse(IsEnvelope(JToken.Parse(original)),
                                because: "This test is about NOT migrating a bare body to the envelope. If the fixture already is an envelope there is nothing left to migrate and the test would pass for the wrong reason.",
                                fix: "Restore Responses\\BareBody.json to a bare body (a plain array of persons, no 'content'/'statusCode' wrapper). A previous run may have rewritten it - that is the very bug under test.");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                 "Responses.BareBody.json",
                                                                 writeResponse: true)
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.IsFalse(IsEnvelope(written),
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

        /// <summary>
        /// A body may well carry a property called "content" with a "value" inside - a blog post is the
        /// obvious case. Only the envelope pairs that with a status code. Mistaking such a bare body for
        /// an envelope would keep writing the envelope over it - the migration this class exists to stop.
        /// </summary>
        [TestMethod]
        public async Task ABareBodyThatCarriesAContentPropertyMustStayABareBody()
        {
            var snapshot = SnapshotPath("BlogPostBareBody.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            try
            {
                // title is stale and compared, so the assert fails - after the writer ran.
                await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<BlogPost>("api/v1/sdk-scenarios/blog-post",
                                                                                                                  "Responses.BlogPostBareBody.json",
                                                                                                                  writeResponse: true),
                                                                            because: "The fixture holds the title 'Stale', so the comparison has to fail. If it passes, the snapshot was never compared.",
                                                                            fix: "Restore SnapshotFormat\\Responses\\BlogPostBareBody.json - its title has to be 'Stale'.")
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.IsFalse(IsEnvelope(written),
                                    because: "A perfectly ordinary body may carry 'content.value'. Treating it as an envelope would rewrite the bare body as an envelope - exactly the silent migration this class exists for.",
                                    fix: "SnapshotShape.IsEnvelope must require content, content.value AND a statusCode together - any single one of them is not enough.");

                Assert.That.AreEqual("a blog post",
                                     written["content"]?["value"]?.ToString(),
                                     because: "The body's own 'content.value' is payload, not a wrapper - it must not be unwrapped away.",
                                     fix: "SnapshotShape.MatchExisting must only unwrap the sdk's envelope, never a body's own properties.");

                Assert.That.AreEqual("Fresh",
                                     written["title"]?.ToString(),
                                     because: "The compared property has to be re-recorded - otherwise the writer never ran and the checks above prove nothing.",
                                     fix: "Check that write response runs before the assert throws.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task ANewSnapshotMustBeWrittenAsAnEnvelope()
        {
            var snapshot = SnapshotPath("ZzNewSnapshotShape.json");

            try
            {
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                 "Responses.ZzNewSnapshotShape.json",
                                                                 writeResponse: true)
                            .ConfigureAwait(false);

                Assert.That.IsTrue(File.Exists(snapshot),
                                   because: "writeResponse on a reference that does not exist yet has to create the snapshot.",
                                   fix: $"Expected the snapshot at {snapshot}.");

                Assert.That.IsTrue(IsEnvelope(JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false))),
                                   because: "With no existing content there is no shape to preserve, so the new snapshot keeps the envelope - that is the richer format and the intended default for anything created from now on.",
                                   fix: "Check the null branch in SnapshotShape.MatchExisting: it has to return the envelope unchanged instead of unwrapping by default.");
            }
            finally
            {
                if (File.Exists(snapshot))
                {
                    File.Delete(snapshot);
                }
            }
        }

        // The envelope is content + content.value + statusCode together.
        private static bool IsEnvelope(JToken token)
        {
            return token is JObject envelope && envelope["content"]?["value"] is not null && envelope["statusCode"] is not null;
        }

        private static string SnapshotPath(string fileName,
                                           [System.Runtime.CompilerServices.CallerFilePath]
                                           string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }
    }
}