using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.SnapshotWriteFiltering
{
    /// <summary>
    /// Write response re-records a snapshot from the current response. A difference func or difference
    /// filter declares that a property is not compared - typically because it changes on every single
    /// run. If the writer put the new value into the file anyway, every re-record would produce a diff
    /// for exactly the properties the author declared uninteresting, and the review noise would make
    /// the real changes invisible.
    ///
    /// The contract these tests pin down:
    ///   1. snapshot does not exist                    -> write everything
    ///   2. snapshot exists, nothing is ignored        -> write every difference
    ///   3. snapshot exists, some properties ignored   -> keep the snapshot value for those,
    ///                                                    update every other property
    ///
    /// Case 3 is the anti-noise guarantee and lives in <see cref="DifferenceResponseWriter"/>: the
    /// ignored paths get their value copied back from the expected snapshot before the file is written.
    ///
    /// Most tests drive the writer directly against a temp file - that keeps expected and current
    /// under full control, which no endpoint can offer. <see cref="TheDifferenceFuncOfAnAssertMustReachTheWriter"/>
    /// closes the loop and proves the func an author passes to an assert really arrives at the writer.
    ///
    /// Note on the global func: <c>IDifferenceFiltering.Apply</c> also runs
    /// <see cref="TestSdkSettings.DifferenceFunc"/>, and this project sets it to
    /// <c>TestHelpers.IgnoreIdDifferences</c> in <see cref="ApiTestBase"/> - it drops every path
    /// containing "id" or "deletedAt". The fixtures below therefore avoid those names, so that what a
    /// test proves is caused by the mechanism the test is about.
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotWriteFiltering")]
    public sealed class SnapshotWriteFilteringTests : ApiTestBase
    {
        private const string SnapshotReference = "Responses.StaleFilteredWrite.json";

        /// <summary>
        /// The full set of json writers, exactly as <c>AddResponseWriter</c> registers them. Using the
        /// set instead of a single writer means every test also proves the selection: for an existing
        /// file in <see cref="ResponseWriteMode.DifferencesOnly"/> only the difference writer may claim
        /// the request - <see cref="ResponseWriter"/> throws when two writers do.
        /// </summary>
        private static IResponseWriter CreateWriter()
        {
            return Services.GetRequiredService<IResponseWriter>();
        }

        // ============================================================
        // Guard - the writers are a DEBUG only feature.
        // ============================================================

        [TestMethod]
        public void TheseTestsOnlyProveSomethingInADebugBuild()
        {
            Assert.That.IsTrue(typeof(SnapshotWriteFilteringTests).Assembly.IsCompiledInDebug(),
                               because: "Every response writer returns immediately for a non DEBUG assembly - write response is a developer feature. In a RELEASE build every test in this class would pass without a single byte being written.",
                               fix: "Run this class from a DEBUG build, or exclude it from RELEASE runs.");
        }

        // ============================================================
        // Case 1 - the snapshot does not exist yet: write everything.
        // ============================================================

        [TestMethod]
        public void ASnapshotThatDoesNotExistYetMustBeWrittenInFull()
        {
            var folder = Directory.CreateTempSubdirectory("snapshot-write-filtering");

            try
            {
                var file = new FileInfo(Path.Combine(folder.FullName, "Snapshot.json"));

                // Nothing to preserve and nothing to compare against - a filter must not cost content here.
                CreateWriter().Write(Request(file,
                                             expected: string.Empty,
                                             current: /*lang=json,strict*/ """{"name":"Son","age":99}""",
                                             mode: ResponseWriteMode.OverwriteAll,
                                             differenceFilter: static _ => false));

                var written = JToken.Parse(File.ReadAllText(file.FullName));

                Assert.That.AreEqual("Son",
                                     written["name"]?.ToString(),
                                     because: "A snapshot that does not exist yet has no value worth keeping, so recording it has to write the response in full - even with a filter that ignores everything.",
                                     fix: "Check OverwriteAllResponseWriter.CanHandle: for a file that does not exist it has to claim the request regardless of the filter, and its Write must not consult the difference filtering at all.");

                Assert.That.AreEqual("99",
                                     written["age"]?.ToString(),
                                     because: "The same for every other property: an ignored property has nothing to fall back to in a file that is being created, so leaving it out would record an incomplete snapshot.",
                                     fix: "See above - the OverwriteAll path writes the current response unfiltered.");
            }
            finally
            {
                folder.Delete(recursive: true);
            }
        }

        // ============================================================
        // Case 2 - the snapshot exists and nothing is ignored: write every difference.
        // ============================================================

        [TestMethod]
        public void WithoutAnyFilteringEveryDifferenceMustBeWritten()
        {
            var written = Write(expected: /*lang=json,strict*/ """{"name":"Son","age":99}""",
                                current: /*lang=json,strict*/ """{"name":"Vegeta","age":100}""");

            Assert.That.AreEqual("Vegeta",
                                 written["name"]?.ToString(),
                                 because: "Without a difference func or filter nothing is declared uninteresting, so re-recording has to take over every changed value. Keeping the old one would silently record a snapshot that never was a response.",
                                 fix: "Check DifferenceResponseWriter.Write: with no ignored paths the result has to be the current response, untouched.");

            Assert.That.AreEqual("100",
                                 written["age"]?.ToString(),
                                 because: "Same for the second property - the anti-noise logic must only ever trigger for paths that were really filtered out.",
                                 fix: "Check the 'ignoredPaths' set in DifferenceResponseWriter: with no filtering it has to stay empty.");
        }

        // ============================================================
        // Case 3 - the anti-noise guarantee.
        // ============================================================

        [TestMethod]
        public void AnIgnoredPropertyMustKeepItsSnapshotValue()
        {
            var written = Write(expected: /*lang=json,strict*/ """{"name":"Son","age":99}""",
                                current: /*lang=json,strict*/ """{"name":"Son","age":100}""",
                                differenceFilter: KeepUnlessAge);

            Assert.That.AreEqual("99",
                                 written["age"]?.ToString(),
                                 because: "'age' is filtered out, so it is not compared - writing the new value anyway would produce a diff on every single re-record for a property the author declared uninteresting. That is exactly the git noise the filtering exists to prevent.",
                                 fix: "Check DifferenceResponseWriter.Write: for every ignored path the value has to be copied back from the expected snapshot into the result before the file is written.");
        }

        [TestMethod]
        public void AnIgnoredPropertyMustNotStopItsNeighbourFromBeingUpdated()
        {
            // The real world case: one volatile property, one that genuinely changed.
            var written = Write(expected: /*lang=json,strict*/ """{"name":"Son","age":99}""",
                                current: /*lang=json,strict*/ """{"name":"Vegeta","age":100}""",
                                differenceFilter: KeepUnlessAge);

            Assert.That.AreEqual("99",
                                 written["age"]?.ToString(),
                                 because: "The ignored property has to keep its snapshot value even when the same write updates another property - otherwise re-recording a real change drags the volatile one along.",
                                 fix: "Check that DifferenceResponseWriter restores the ignored paths on the result clone and not on the expected root.");

            Assert.That.AreEqual("Vegeta",
                                 written["name"]?.ToString(),
                                 because: "Protecting an ignored property must not turn into a general write block: 'name' was compared, it really differs, and re-recording is the whole point of write response. Keeping the old value here would make write response look broken.",
                                 fix: "Check that only the paths in 'ignoredPaths' are restored - a too-broad match (for example a StartsWith or a Contains over the path) would take the neighbours with it.");
        }

        [TestMethod]
        public void AListTransformDifferenceFuncMustProtectTheSameWayAFilterDoes()
        {
            // Two ways to say the same thing - a func that drops from the list, a predicate per item.
            // The writer must not care which one the author used.
            var written = Write(expected: /*lang=json,strict*/ """{"name":"Son","age":99}""",
                                current: /*lang=json,strict*/ """{"name":"Vegeta","age":100}""",
                                differenceFunc: DropAge);

            Assert.That.AreEqual("99",
                                 written["age"]?.ToString(),
                                 because: "differenceFunc and differenceFilter are two spellings of the same intent. A snapshot protected by the filter but rewritten by the func would make the choice between them a trap.",
                                 fix: "Check that DifferenceResponseWriter derives the ignored paths from IDifferenceFiltering.Apply (which runs both mechanisms) instead of from the filter alone.");

            Assert.That.AreEqual("Vegeta",
                                 written["name"]?.ToString(),
                                 because: "Same as with the filter - the property the func kept in the list is compared, so its new value belongs in the file.",
                                 fix: "See AnIgnoredPropertyMustNotStopItsNeighbourFromBeingUpdated.");
        }

        [TestMethod]
        public void AnIgnoredNestedPropertyMustKeepItsSnapshotValue()
        {
            var written = Write(expected: /*lang=json,strict*/ """{"person":{"name":"Son","age":99}}""",
                                current: /*lang=json,strict*/ """{"person":{"name":"Vegeta","age":100}}""",
                                differenceFilter: KeepUnlessAge);

            Assert.That.AreEqual("99",
                                 written["person"]?["age"]?.ToString(),
                                 because: "Volatile properties are rarely at the root - a timestamp sits somewhere inside the payload. If protection only worked at the top level the feature would be useless for real responses.",
                                 fix: "Check JsonPathWriter.AddOrUpdate: it has to resolve the parent path ('person') and set the last segment on it.");

            Assert.That.AreEqual("Vegeta",
                                 written["person"]?["name"]?.ToString(),
                                 because: "The sibling inside the same nested object still has to be updated, otherwise protecting one property freezes the whole object.",
                                 fix: "Check that the parent object is taken from the result clone, so writing one property does not replace the whole object with the expected one.");
        }

        [TestMethod]
        public void AnIgnoredArrayElementMustKeepItsSnapshotValue()
        {
            var written = Write(expected: /*lang=json,strict*/ """{"values":["a","b"]}""",
                                current: /*lang=json,strict*/ """{"values":["a","CHANGED"]}""",
                                differenceFilter: static difference => difference.MemberPath.Contains("[1]", StringComparison.Ordinal).IsFalse());

            Assert.That.AreEqual("b",
                                 written["values"]?[1]?.ToString(),
                                 because: "An ignored difference inside an array has to be protected just like a property - an index path is only a different spelling of the same location.",
                                 fix: "Check the array branch of JsonPathWriter.AddOrUpdate and GetParentPath: for 'values[1]' the parent is 'values' and the last segment is the index 1.");

            Assert.That.AreEqual("a",
                                 written["values"]?[0]?.ToString(),
                                 because: "The untouched element must survive the restore - writing an index must not rebuild or truncate the array.",
                                 fix: "Check that AddOrUpdate only assigns the one index instead of replacing the JArray.");
        }

        [TestMethod]
        public void AWriteWhereEveryDifferenceIsIgnoredMustLeaveTheFileUnchanged()
        {
            // The strictest form of the promise, and the one a reviewer actually sees: not "the values
            // are equal again" but "git reports nothing at all" - byte for byte, formatting included.
            var expected = Indented( /*lang=json,strict*/ """{"name":"Son","age":99,"city":"West City"}""");

            var written = WriteRaw(expected: expected,
                                   current: /*lang=json,strict*/ """{"name":"Vegeta","age":100,"city":"East City"}""",
                                   differenceFilter: static _ => false);

            Assert.That.AreEqual(expected,
                                 written,
                                 because: "When every difference is ignored the re-record has nothing to record. The file has to come out byte identical - a reordered property or a changed indentation is git noise just like a changed value.",
                                 fix: "Check DifferenceResponseWriter.Write: the result is the current response with all ignored paths restored, serialized indented. If only the values match but the text does not, compare the serializer settings with the format the snapshot was written in.");
        }

        // ============================================================
        // Two shapes where the restore has nothing to copy back from.
        // ============================================================

        [TestMethod]
        public void AnIgnoredPropertyThatOnlyExistsInTheResponseMustNotBeAdded()
        {
            var written = Write(expected: /*lang=json,strict*/ """{"name":"Son"}""",
                                current: /*lang=json,strict*/ """{"name":"Son","trace":"7f3a-91"}""",
                                differenceFilter: static difference => difference.MemberPath.Contains("trace", StringComparison.OrdinalIgnoreCase).IsFalse());

            Assert.That.IsNull(written["trace"],
                               because: "A property that appears in the response but is ignored must not enter the snapshot. 'trace' is the textbook case - a value that is new on every call. Recording it once means a diff on every re-record from then on, which is the noise the filtering was supposed to prevent.",
                               fix: "Check DifferenceResponseWriter.Write: for an ignored path the expected snapshot has no token to copy back, so the branch is skipped and the new property survives in the result. That case has to remove the path from the result instead - see JsonPathWriter.Remove.");
        }

        [TestMethod]
        public void AnIgnoredEntryOfAKeyValueArrayMustKeepItsSnapshotValue()
        {
            // JsonDiffer addresses key-value arrays by key instead of by index, so the ignored path
            // reads settings["theme"].Value - a shape JsonPathWriter has to understand as well.
            var written = Write(expected: /*lang=json,strict*/ """{"settings":[{"Key":"theme","Value":"dark"}]}""",
                                current: /*lang=json,strict*/ """{"settings":[{"Key":"theme","Value":"light"}]}""",
                                differenceFilter: static _ => false);

            Assert.That.AreEqual("dark",
                                 written["settings"]?[0]?["Value"]?.ToString(),
                                 because: "Key-value arrays are a normal payload shape, and an ignored difference in one has to be protected like any other - otherwise the guarantee silently depends on how the diff happened to spell the path.",
                                 fix: "JsonDiffer builds 'settings[\"theme\"].Value' for these arrays. Check that both the SelectToken lookup in DifferenceResponseWriter and JsonPathWriter.GetParentPath/GetLastSegment can resolve a quoted key segment, not only a numeric index.");
        }

        // ============================================================
        // End to end - the func an author writes has to reach the writer.
        // ============================================================

        [TestMethod]
        public async Task TheDifferenceFuncOfAnAssertMustReachTheWriter()
        {
            var snapshot = SnapshotPath("StaleFilteredWrite.json");
            var original = await File.ReadAllTextAsync(snapshot).ConfigureAwait(false);

            var stale = JToken.Parse(original);

            Assert.That.AreEqual("1",
                                 stale[0]?["age"]?.ToString(),
                                 because: "The fixture is deliberately stale in two places: an ignored one (age) and a compared one (firstName). Once age matches the endpoint again this test proves nothing - it would pass whether the protection works or not.",
                                 fix: "Restore SnapshotWriteFiltering\\Responses\\StaleFilteredWrite.json - the first person has to carry age 1 and firstName 'Stale'. A re-record with write response is what usually destroys it.");

            try
            {
                // firstName differs and is compared, so the assert fails - after the writer ran.
                await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                                                                             SnapshotReference,
                                                                                                                             differenceFunc: DropAge,
                                                                                                                             writeResponse: true),
                                                                            because: "'firstName' is not filtered out and the fixture holds 'Stale', so the assert has to fail. If it passes, the comparison never saw the snapshot and everything below would be measuring nothing.",
                                                                            fix: "Check that the snapshot reference resolves and that differenceFunc only drops the age differences.")
                            .ConfigureAwait(false);

                var written = JToken.Parse(await File.ReadAllTextAsync(snapshot).ConfigureAwait(false));

                Assert.That.AreEqual("1",
                                     written[0]?["age"]?.ToString(),
                                     because: "This is the whole feature seen from the outside: the func an author hands to the assert has to reach the writer, so a re-record keeps the value of the property that is not compared.",
                                     fix: "Check that IObjectAssertContext.DifferenceFunc is copied into the WriteResponseRequest in ResponseWriter.Write(context, ...) - if the request gets the default identity func the writer sees no ignored paths at all.");

                Assert.That.AreEqual("Goku",
                                     written[0]?["firstName"]?.ToString(),
                                     because: "The compared property has to be re-recorded in the same write - that is what the author asked for by turning write response on.",
                                     fix: "Check that write response runs before the assert throws in AssertService.ObjectsAreEqual, and that only the ignored paths are restored.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshot, original).ConfigureAwait(false);
            }
        }

        // ============================================================
        // Helpers
        // ============================================================

        private static IEnumerable<Difference> DropAge(ImmutableList<Difference> differences)
        {
            return differences.Where(difference => difference.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase).IsFalse());
        }

        private static bool KeepUnlessAge(Difference difference)
        {
            return difference.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase).IsFalse();
        }

        private static string Indented(string json)
        {
            return JToken.Parse(json).ToString(Formatting.Indented);
        }

        private static JToken Write(string expected,
                                    string current,
                                    Func<ImmutableList<Difference>, IEnumerable<Difference>>? differenceFunc = null,
                                    Predicate<Difference>? differenceFilter = null)
        {
            return JToken.Parse(WriteRaw(expected, current, differenceFunc,
                                         differenceFilter));
        }

        /// <summary>
        /// Runs the writers against a temp snapshot and hands back the file as it was written - as text,
        /// so a test can also look at the formatting.
        /// </summary>
        private static string WriteRaw(string expected,
                                       string current,
                                       Func<ImmutableList<Difference>, IEnumerable<Difference>>? differenceFunc = null,
                                       Predicate<Difference>? differenceFilter = null)
        {
            var folder = Directory.CreateTempSubdirectory("snapshot-write-filtering");

            try
            {
                var file = new FileInfo(Path.Combine(folder.FullName, "Snapshot.json"));
                File.WriteAllText(file.FullName, expected);

                CreateWriter().Write(Request(file, expected, current,
                                             ResponseWriteMode.DifferencesOnly, differenceFunc, differenceFilter));

                return File.ReadAllText(file.FullName);
            }
            finally
            {
                folder.Delete(recursive: true);
            }
        }

        private static WriteResponseRequest Request(FileInfo file,
                                                    string expected,
                                                    string current,
                                                    ResponseWriteMode mode,
                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>>? differenceFunc = null,
                                                    Predicate<Difference>? differenceFilter = null)
        {
            return new WriteResponseRequest
            {
                CallingAssembly = typeof(SnapshotWriteFilteringTests).Assembly,
                CurrentResponseAsString = current,
                ExpectedResult = new EmbeddedFileInfo("Responses.Snapshot.json", expected, file),
                Parameters = [],
                DifferenceFunc = differenceFunc ?? (differences => differences),
                DifferenceFilter = differenceFilter ?? (static _ => true),
                Mode = mode,
                CallerFilePath = ThisFile(),
                CallerLineNumber = 0,
                ExpectedResultParameterName = nameof(SnapshotReference),
                ExpectedType = typeof(object),
                ExpectedObject = null
            };
        }

        private static string SnapshotPath(string fileName,
                                           [CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", fileName);
        }

        private static string ThisFile([CallerFilePath] string callerFilePath = "")
        {
            return callerFilePath;
        }
    }
}