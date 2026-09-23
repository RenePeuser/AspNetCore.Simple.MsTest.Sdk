using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ParameterizedSnapshot
{
    /// <summary>
    ///     Pins down that recording refuses to write a snapshot that would lose a placeholder.
    ///     Placeholders are put back per json path, read off the file being updated - see
    ///     SnapshotPlaceholderRestorer. That covers everything as long as the path is still there. What it
    ///     cannot cover is a path that is gone: the api stopped returning the field, an array got shorter,
    ///     or the sentence carrying the placeholder was reworded. The recorded text then simply has no
    ///     place to put it back, and writing would turn the template into a hard coded snapshot.
    ///     This fixture takes the clearest of those - a property the response does not have at all. Nothing
    ///     is written and the developer is told which placeholder is at stake, instead of finding a
    ///     plausible looking value change in the git diff a week later.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("ParameterizedSnapshot")]
    public sealed class PlaceholderLostOnRecordTests : ApiTestBase
    {
        private const string Snapshot = "Responses.PlaceholderAtRisk.json";

        private const string PersonUrl = "api/v1/persons/1";

        [TestMethod]
        [TestCategory("GET")]
        public async Task ARecordingThatWouldDropAPlaceholderMustBeRefused()
        {
            var snapshotPath = SnapshotPath();
            var before = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

            try
            {
                // $legacyRef$ sits on a property the person response does not have, so the recorded tree
                // has no path to put it back at. $name$ is fine - it is there to show only the doomed one
                // is reported.
                var exception = await Assert.That
                                            .ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<Person>(PersonUrl,
                                                                                                                           Snapshot,
                                                                                                                           [("$name$", "Son"), ("$legacyRef$", "anything")],
                                                                                                                           true),
                                                                                       "Recording has to refuse when a placeholder has nowhere left to go. Writing it turns a template into a hard coded snapshot, and the failure that follows on the next run points at the api instead of at the recording.",
                                                                                       "Check SnapshotPlaceholderGuard.EnsureNoPlaceholderIsLost and that both response writers call it before File.WriteAllText.")
                                            .ConfigureAwait(false);

                Assert.That.Contains(exception.Message,
                                     "$legacyRef$",
                                     "The refusal has to name the placeholder at risk - which one it is IS the fix.",
                                     "Check SnapshotPlaceholderLostException.LostPlaceholders and how SnapshotPlaceholderLostErrorHandler renders it.");

                var after = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

                Assert.That.AreEqual(before,
                                     after,
                                     "A refused recording must leave the file untouched. Refusing after the write would report the damage and do it anyway.",
                                     "Check that the guard runs BEFORE File.WriteAllText in DifferenceResponseWriter and OverwriteAllResponseWriter.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshotPath, before).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Not a test - a way to LOOK at the refusal report.
        ///     The test above swallows the exception to assert on it, so the report never reaches the output.
        ///     Run this one on purpose (it is ignored, so it costs nothing in ci) and the full message is the
        ///     failure text. It restores the fixture on the way out, so running it leaves nothing behind.
        /// </summary>
        [TestMethod]
        [TestCategory("GET")]
        [Ignore("Demo - comment this Ignore out to read the refusal report, then put it back. MsTest skips an ignored test even when it is started on its own, so the attribute really has to go for the run. It fails by design and restores its fixture.")]
        public async Task Demo_ShowTheRefusalReport()
        {
            var snapshotPath = SnapshotPath();
            var before = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

            try
            {
                await Client.AssertGetAsync<Person>(PersonUrl,
                                                    Snapshot,
                                                    [("$name$", "Son"), ("$legacyRef$", "anything")],
                                                    true)
                            .ConfigureAwait(false);
            }
            finally
            {
                await File.WriteAllTextAsync(snapshotPath, before).ConfigureAwait(false);
            }
        }

        private static string SnapshotPath([CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", "PlaceholderAtRisk.json");
        }
    }
}