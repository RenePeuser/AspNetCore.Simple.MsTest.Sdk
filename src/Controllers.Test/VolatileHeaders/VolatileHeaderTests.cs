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

            CollectionAssert.AreEquivalent(new[] { "Content-Type", "X-Business-Relevant" },
                                           filtered.Select(header => header.Key).ToList());
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

            CollectionAssert.AreEquivalent(new[] { "X-Keep" }, filtered.Headers.Select(header => header.Key).ToList());
            CollectionAssert.AreEquivalent(new[] { "X-Keep" }, filtered.TrailingHeaders.Select(header => header.Key).ToList());
            CollectionAssert.AreEquivalent(new[] { "Content-Type" }, filtered.Content!.Headers.Select(header => header.Key).ToList());
        }

        [TestMethod]
        public void AnEmptyConfiguredListMustKeepEveryHeader()
        {
            var headers = ImmutableList.Create(Header("traceparent", "00-abc-def-01"));

            var filtered = headers.WithoutVolatileHeaders(new TestSdkSettings { VolatileHeaderNames = [] });

            Assert.AreEqual(1, filtered.Count, "An empty list means the project opted out of filtering.");
        }

        [TestMethod]
        public void AProjectMustBeAbleToAddItsOwnName()
        {
            var headers = ImmutableList.Create(Header("X-My-Correlation-Id", "42"), Header("X-Keep", "a"));

            var settings = new TestSdkSettings { VolatileHeaderNames = ["X-My-Correlation-Id"] };

            var filtered = headers.WithoutVolatileHeaders(settings);

            CollectionAssert.AreEquivalent(new[] { "X-Keep" }, filtered.Select(header => header.Key).ToList());
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

            CollectionAssert.Contains(names, "traceparent", "Without a volatile header the test above proves nothing.");
            CollectionAssert.Contains(names, "X-Amzn-Trace-Id");
            CollectionAssert.Contains(names, "Date");
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
