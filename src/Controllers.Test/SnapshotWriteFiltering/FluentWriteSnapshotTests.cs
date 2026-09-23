using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.SnapshotWriteFiltering
{
    /// <summary>
    /// <c>WriteSnapshot()</c> on the fluent chain — the re-record path.
    ///
    /// <para>
    /// Only <c>WriteSnapshot(false)</c> was covered so far, which exercises the builder method but never
    /// touches disk: a chain that silently failed to enable writing would have passed that test just as
    /// happily. These tests drive the real thing and restore the fixture afterwards.
    /// </para>
    ///
    /// <para>
    /// Uses its OWN fixture (FluentStaleWrite.json), not the one
    /// <see cref="SnapshotWriteFilteringTests"/> re-records — two tests rewriting the same file would
    /// race as soon as the classes run in parallel.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotWriteFiltering")]
    [TestCategory("Fluent")]
    public sealed class FluentWriteSnapshotTests : ApiTestBase
    {
        private const string SnapshotReference = "Responses.FluentStaleWrite.json";

        [TestMethod]
        public void TheseTestsOnlyProveSomethingInADebugBuild()
        {
            Assert.That.IsTrue(typeof(FluentWriteSnapshotTests).Assembly.IsCompiledInDebug(),
                               because: "Every response writer returns immediately for a non DEBUG assembly - write response is a developer feature. In a RELEASE build the write assertions below would pass without a single byte being written.",
                               fix: "Run this class from a DEBUG build, or exclude it from RELEASE runs.");
        }

        [TestMethod]
        public async Task WriteSnapshot_Must_Re_Record_The_Compared_Property()
        {
            var snapshot = SnapshotPath("FluentStaleWrite.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            var stale = JToken.Parse(original);

            Assert.That.AreEqual("Stale",
                                 stale[0]?["firstName"]?.ToString(),
                                 because: "The fixture has to be stale for this test to prove anything. Once firstName matches the endpoint again the assert below would pass whether WriteSnapshot did its job or not.",
                                 fix: "Restore SnapshotWriteFiltering\\Responses\\FluentStaleWrite.json - the first person has to carry firstName 'Stale' and age 1.");

            try
            {
                // firstName is compared and stale, so the chain fails - but only AFTER the writer ran.
                await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persons")
                                                                                        .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                                                                        .ExpectedResponseFromEmbeddedJson(SnapshotReference)
                                                                                        .IgnoreDifferences(DropAge)
                                                                                        .WriteSnapshot()
                                                                                        .ExecuteAsync(),
                                                                            because: "'firstName' is not filtered out and the fixture holds 'Stale', so the comparison has to fail. If it passes, the snapshot was never compared and everything below would be measuring nothing.",
                                                                            fix: "Check that the embedded snapshot reference resolves and that IgnoreDifferences only drops the age differences.")
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.AreEqual("Goku",
                                     written[0]?["firstName"]?.ToString(),
                                     because: "This is WriteSnapshot seen from the outside: turning it on has to re-record the compared property from the live response. If it still reads 'Stale', the flag never reached the engine and the whole re-record workflow is unavailable on the fluent API.",
                                     fix: "Check that HttpResponseBuilder passes _writeSnapshot as writeResponse into AssertHttpCallAsync.");

                Assert.That.AreEqual("1",
                                     written[0]?["age"]?.ToString(),
                                     because: "age is declared uncompared via IgnoreDifferences, so a re-record has to keep the snapshot value. Writing the live value back would produce a diff for exactly the property the author called uninteresting.",
                                     fix: "Check that the fluent IgnoreDifferences func reaches the writer through IObjectAssertContext.DifferenceFunc, the same way the native differenceFunc does.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task WriteSnapshot_False_Must_Not_Touch_The_File()
        {
            var snapshot = SnapshotPath("FluentStaleWrite.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            try
            {
                await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persons")
                                                                                        .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                                                                        .ExpectedResponseFromEmbeddedJson(SnapshotReference)
                                                                                        .IgnoreDifferences(DropAge)
                                                                                        .WriteSnapshot(false)
                                                                                        .ExecuteAsync(),
                                                                            because: "The stale firstName still has to fail the comparison - WriteSnapshot(false) only turns off re-recording, not the assertion.",
                                                                            fix: "Check that writeResponse false leaves the comparison untouched.")
                            .ConfigureAwait(false);

                var after = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

                Assert.That.AreEqual(original, after,
                                     because: "WriteSnapshot(false) is the default and must leave the file byte-identical. A test run that silently re-records turns every later run green and destroys the fixture for everyone else.",
                                     fix: "Check that the writeResponse flag is honoured and defaults to false when WriteSnapshot is not called or called with false.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        private static IEnumerable<Difference> DropAge(ImmutableList<Difference> differences)
        {
            return differences.Where(difference => difference.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase).IsFalse());
        }

        private static string SnapshotPath(string fileName,
                                           [CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }
    }
}