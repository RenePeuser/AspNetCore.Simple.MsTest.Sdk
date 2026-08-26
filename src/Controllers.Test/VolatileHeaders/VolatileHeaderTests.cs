using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Controllers.Api.Persons;
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
        [TestMethod]
        public void TheFilterMustDropTheConfiguredNamesAndKeepEverythingElse()
        {
            var headers = ImmutableList.Create(Header("traceparent", "00-abc-def-01"),
                                               Header("X-AMZN-TRACE-ID", "Root=1-2-3"), // casing must not matter
                                               Header("Content-Type", "application/json"),
                                               Header("X-Business-Relevant", "keep me"));

            var filtered = headers.WithoutVolatileHeaders(new TestSdkSettings());

            Assert.That.AreEquivalent(new[] { "Content-Type", "X-Business-Relevant" },
                                      filtered.Select(header => header.Key).ToList(),
                                      because: "The filter has to drop exactly the configured names - case-insensitively, hence X-AMZN-TRACE-ID in upper case - and leave every other header untouched. Dropping too much would lose business-relevant headers, dropping too little keeps the re-record noise.",
                                      fix: "Check WithoutVolatileHeaders in VolatileHeaderFilter: the name comparison has to be case-insensitive and must only consider TestSdkSettings.VolatileHeaderNames.");
        }

        /// <summary>
        /// The envelope is what gets written, so every header bearing part of it has to be covered -
        /// missing one would leave the noise in the file it was supposed to keep clean.
        /// </summary>
        [TestMethod]
        public void EveryHeaderCollectionOfTheWrittenEnvelopeMustBeFiltered()
        {
            var content = new SimpleHttpContent
            {
                Headers = ImmutableList.Create(Header("Date", "Tue, 26 Aug 2025 09:14:07 GMT"),
                                               Header("Content-Type", "application/json")),
                Value = "{}"
            };

            var response = new SimpleHttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Headers = ImmutableList.Create(Header("traceparent", "00-abc-01"), Header("X-Keep", "a")),
                TrailingHeaders = ImmutableList.Create(Header("Server-Timing", "app;dur=12"), Header("X-Keep", "b")),
                Content = content
            };

            var filtered = response.WithoutVolatileHeaders(new TestSdkSettings());

            Assert.That.AreEquivalent(new[] { "X-Keep" },
                                      filtered.Headers.Select(header => header.Key).ToList(),
                                      because: "The response headers are part of the written envelope, so 'traceparent' has to be gone from them - one unfiltered collection is enough to keep the noise in the file.",
                                      fix: "Check that WithoutVolatileHeaders(SimpleHttpResponseMessage) filters the Headers collection, not only the content headers.");

            Assert.That.AreEquivalent(new[] { "X-Keep" },
                                      filtered.TrailingHeaders.Select(header => header.Key).ToList(),
                                      because: "TrailingHeaders end up in the envelope just like the normal ones, so 'Server-Timing' has to be dropped there too.",
                                      fix: "Check that WithoutVolatileHeaders(SimpleHttpResponseMessage) also runs over TrailingHeaders - it is the collection most easily forgotten.");

            Assert.That.AreEquivalent(new[] { "Content-Type" },
                                      filtered.Content!.Headers.Select(header => header.Key).ToList(),
                                      because: "The content headers are written as well, so 'Date' has to be dropped there - and Content-Type has to survive, it is not volatile.",
                                      fix: "Check that WithoutVolatileHeaders(SimpleHttpResponseMessage) rebuilds Content.Headers through the same filter.");
        }

        [TestMethod]
        public void AnEmptyConfiguredListMustKeepEveryHeader()
        {
            var headers = ImmutableList.Create(Header("traceparent", "00-abc-def-01"));

            var filtered = headers.WithoutVolatileHeaders(new TestSdkSettings { VolatileHeaderNames = [] });

            Assert.That.HasCount(1,
                                 filtered,
                                 because: "An empty VolatileHeaderNames list means the project opted out of filtering, so even 'traceparent' has to survive. A hard-coded default list would silently ignore that opt-out.",
                                 fix: "Check that WithoutVolatileHeaders reads the names from the passed TestSdkSettings only and never falls back to a built-in list when that collection is empty.");
        }

        [TestMethod]
        public void AProjectMustBeAbleToAddItsOwnName()
        {
            var headers = ImmutableList.Create(Header("X-My-Correlation-Id", "42"), Header("X-Keep", "a"));

            var settings = new TestSdkSettings { VolatileHeaderNames = ["X-My-Correlation-Id"] };

            var filtered = headers.WithoutVolatileHeaders(settings);

            Assert.That.AreEquivalent(new[] { "X-Keep" },
                                      filtered.Select(header => header.Key).ToList(),
                                      because: "A project has to be able to name its own volatile header. Only 'X-My-Correlation-Id' was configured here, so the built-in names must not be added on top and X-Keep has to survive.",
                                      fix: "Check that WithoutVolatileHeaders uses exactly the configured TestSdkSettings.VolatileHeaderNames instead of merging them with a default set.");
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
            var snapshot = Path.Combine(ProjectFolder(), "VolatileHeaders", "Responses", "StaleVolatileHeaders.json");
            var names = JToken.Parse(File.ReadAllText(snapshot))["headers"]!
                              .Select(header => header["key"]!.ToString())
                              .ToList();

            const string Because = "ASnapshotThatStillCarriesVolatileHeadersMustStayGreen only proves something if the fixture really carries these headers. Once they are gone from the file that test passes for the wrong reason.";
            const string Fix = "Restore the volatile headers in VolatileHeaders\\Responses\\StaleVolatileHeaders.json - the fixture deliberately represents a snapshot recorded before the filter existed and must not be re-recorded.";

            Assert.That.Contains(names, "traceparent", because: Because, fix: Fix);
            Assert.That.Contains(names, "X-Amzn-Trace-Id", because: Because, fix: Fix);
            Assert.That.Contains(names, "Date", because: Because, fix: Fix);
        }

        private static KeyValuePair<string, ImmutableList<string>> Header(string name,
                                                                          string value)
        {
            return new KeyValuePair<string, ImmutableList<string>>(name, ImmutableList.Create(value));
        }

        private static string ProjectFolder([System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            return new FileInfo(callerFilePath).Directory!.Parent!.FullName;
        }
    }
}
