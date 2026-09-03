using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ParameterizedSnapshot
{
    /// <summary>
    /// Pins down that re-recording keeps each placeholder spelled the way the snapshot spells it - per
    /// occurrence, not per name.
    ///
    /// The fixture uses ONE parameter in two incompatible ways, which is what makes it the interesting
    /// case: <c>$myId$</c> stands bare where the value is a number (<c>"id": $myId$</c>) and sits inside a
    /// sentence where it is text (<c>"The person with the Id: $myId$ does not exist"</c>). Bare is not a
    /// style choice - quoting it would make the resolved value the string "999" and the snapshot would
    /// stop matching a numeric field. A restore that decided per NAME would have to pick one of the two
    /// and would corrupt the other.
    ///
    /// The sentence is the second half of it. ParameterReplacer puts placeholders back by matching a
    /// PROPERTY name and an equal value, so it finds nothing here: no property is called <c>myId</c>, and
    /// the text fallback ignores values under three characters anyway. Only reading the old value at the
    /// same json path as a template - placeholder as wildcard, everything around it fixed - brings it back.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("ParameterizedSnapshot")]
    public sealed class PlaceholderSpellingTests : ApiTestBase
    {
        private const string Snapshot = "Responses.PersonNotFoundTemplate.json";

        [TestMethod]
        [TestCategory("GET")]
        public async Task RecordingMustKeepEachPlaceholderSpelledAsTheSnapshotSpellsIt()
        {
            var snapshotPath = SnapshotPath();
            var before = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

            try
            {
                await Client.AssertGetAsErrorAsync<PersonNotFound>("api/v1/persons/999",
                                                                   Snapshot,
                                                                   [("$myId$", 999)],
                                                                   writeResponse: true,
                                                                   skipEndpointValidation: true,
                                                                   expectedHttpStatusCode: HttpStatusCode.NotFound)
                            .ConfigureAwait(false);

                var after = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

                Assert.That.AreEqual(before,
                                     after,
                                     because: "Nothing in this response differs from the snapshot once the parameter is applied, so recording has to leave the file byte for byte as it was. A change here means a placeholder came back in the wrong spelling - quoted where it has to be bare, or replaced by the concrete 999.",
                                     fix: "Check SnapshotPlaceholderRestorer.ApplyOriginalSpelling: it walks the recorded tree against the old snapshot and takes the spelling from the old value at the same json path.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshotPath, before).ConfigureAwait(false);
            }
        }

        private static string SnapshotPath([CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", "PersonNotFoundTemplate.json");
        }

        /// <summary>The 404 body of the person controller: problem details plus the id it was given.</summary>
        private sealed record PersonNotFound(string Title,
                                             int Status,
                                             string Detail,
                                             long Id);
    }
}
