using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.VolatileHeaders
{
    /// <summary>
    /// Headers like traceparent or X-Amzn-Trace-Id carry a new value on every single call. Recording
    /// them into a snapshot produced pure noise: every re-record showed a diff, and nothing about it
    /// said anything about the behaviour under test.
    ///
    /// The names come from <see cref="TestCreatorSettings.VolatileHeaderNames" /> so a project can add
    /// its own.
    /// </summary>
    [TestClass]
    [TestCategory("VolatileHeaders")]
    public sealed class VolatileHeaderTests : ApiTestBase
    {
        /// <summary>
        /// The fixture snapshot still carries three volatile headers, exactly as it would have been
        /// recorded before the filter existed. It has to stay green - filtering only the current side
        /// would trade recording noise for a test that can never pass again.
        /// </summary>
        [TestMethod]
        public Task ASnapshotThatStillCarriesVolatileHeadersMustNotFail()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "Responses.StaleVolatileHeaders.json");
        }

        [TestMethod]
        public void TheFixtureMustReallyContainVolatileHeaders()
        {
            var snapshot = Path.Combine(ProjectFolder(), "VolatileHeaders", "Responses", "StaleVolatileHeaders.json");
            var headers = JToken.Parse(File.ReadAllText(snapshot))["headers"]!;

            var names = headers.Select(header => header["key"]!.ToString()).ToList();

            CollectionAssert.Contains(names, "traceparent", "Without a volatile header the test above proves nothing.");
            CollectionAssert.Contains(names, "X-Amzn-Trace-Id");
            CollectionAssert.Contains(names, "Date");
        }

        [TestMethod]
        public void TheFilterMustDropTheConfiguredNamesAndKeepEverythingElse()
        {
            var headers = ImmutableList.Create(Header("traceparent", "00-abc-def-01"),
                                               Header("X-AMZN-TRACE-ID", "Root=1-2-3"), // casing must not matter
                                               Header("Content-Type", "application/json"),
                                               Header("X-Business-Relevant", "keep me"));

            var filtered = headers.WithoutVolatileHeaders(new TestCreatorSettings());

            var names = filtered.Select(header => header.Key).ToList();

            CollectionAssert.AreEquivalent(new[] { "Content-Type", "X-Business-Relevant" }, names);
        }

        [TestMethod]
        public void AnEmptyConfiguredListMustKeepEveryHeader()
        {
            var headers = ImmutableList.Create(Header("traceparent", "00-abc-def-01"));

            var filtered = headers.WithoutVolatileHeaders(new TestCreatorSettings { VolatileHeaderNames = [] });

            Assert.AreEqual(1, filtered.Count, "An empty list means the project opted out of filtering.");
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
