using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Controllers.Api.SdkScenarios;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.VolatileHeaders
{
    /// <summary>
    /// Headers like traceparent or X-Amzn-Trace-Id carry a new value on every single call, and the
    /// snapshot envelope recorded them. They are never compared - neither the envelope headers nor the
    /// content headers take part in the diff - so all they ever produced was a changed file on every
    /// re-record and a noisy review.
    ///
    /// The names come from <see cref="TestSdkSettings.VolatileHeaderNames" /> so a project can
    /// swap in its own list.
    /// </summary>
    [TestClass]
    [TestCategory("VolatileHeaders")]
    public sealed class VolatileHeaderTests : ApiTestBase
    {
        private const string Url = "api/v1/sdk-scenarios/volatile-headers";

        [TestMethod]
        public async Task ARecordedEnvelopeMustNotCarryVolatileHeaders()
        {
            Assert.That.IsTrue(typeof(VolatileHeaderTests).Assembly.IsCompiledInDebug(),
                               because: "Every response writer bails out for non DEBUG assemblies - in a RELEASE build nothing would be written.",
                               fix: "Run this test from a DEBUG build, or exclude it from RELEASE runs.");

            var snapshot = SnapshotPath("VolatileHeadersWrite.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            try
            {
                // title is stale and compared, so the assert fails - after the writer ran.
                await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<BlogPost>(Url,
                                                                                                                  "Responses.VolatileHeadersWrite.json",
                                                                                                                  writeResponse: true),
                                                                            because: "The fixture holds the title 'Stale', so the comparison has to fail. If it passes, the snapshot was never compared.",
                                                                            fix: "Restore VolatileHeaders\\Responses\\VolatileHeadersWrite.json - its title has to be 'Stale'.")
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.AreEqual("Fresh",
                                     written["content"]?["value"]?["title"]?.ToString(),
                                     because: "The compared property has to be re-recorded - otherwise the writer never ran and the header checks below prove nothing.",
                                     fix: "Check that write response runs for an envelope snapshot over http.");

                var names = HeaderNames(written);

                Assert.That.DoesNotContain(names,
                                           "traceparent",
                                           because: "The endpoint sends a new traceparent on every call. Recording it means a changed file on every re-record - noise for a header that is never compared.",
                                           fix: "Check that the written envelope runs through WithoutVolatileHeaders with TestSdkSettings.VolatileHeaderNames.");

                Assert.That.IsFalse(names.Any(name => name.Equals("X-Amzn-Trace-Id", StringComparison.OrdinalIgnoreCase)),
                                    because: "The endpoint sends the header as X-AMZN-TRACE-ID. Header names are case-insensitive, so the filter has to drop it regardless of casing.",
                                    fix: "Check that WithoutVolatileHeaders compares header names case-insensitively.");

                Assert.That.Contains(names,
                                     "X-Business-Relevant",
                                     because: "Only the configured volatile names may be dropped. Losing a business-relevant header would remove information the author recorded on purpose.",
                                     fix: "Check that WithoutVolatileHeaders only considers TestSdkSettings.VolatileHeaderNames.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Snapshots recorded before the filter existed still carry the headers. Filtering must not
        /// make a single one of them fail.
        /// </summary>
        [TestMethod]
        public Task ASnapshotThatStillCarriesVolatileHeadersMustStayGreen()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "Responses.StaleVolatileHeaders.json");
        }

        [TestMethod]
        public void TheFixtureMustReallyContainVolatileHeaders()
        {
            var names = HeaderNames(JToken.Parse(File.ReadAllText(SnapshotPath("StaleVolatileHeaders.json"))));

            const string because = "ASnapshotThatStillCarriesVolatileHeadersMustStayGreen only proves something if the fixture really carries these headers. Once they are gone from the file that test passes for the wrong reason.";
            const string fix = "Restore the volatile headers in VolatileHeaders\\Responses\\StaleVolatileHeaders.json - the fixture deliberately represents a snapshot recorded before the filter existed and must not be re-recorded.";

            Assert.That.Contains(names, "traceparent", because: because,
                                 fix: fix);

            Assert.That.Contains(names, "X-Amzn-Trace-Id", because: because,
                                 fix: fix);

            Assert.That.Contains(names, "Date", because: because,
                                 fix: fix);
        }

        private static List<string> HeaderNames(JToken envelope)
        {
            return envelope["headers"]!.Select(header => header["key"]!.ToString())
                                       .ToList();
        }

        private static string SnapshotPath(string fileName,
                                           [CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }
    }
}
