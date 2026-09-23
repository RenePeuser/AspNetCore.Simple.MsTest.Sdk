using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ParameterizedSnapshot
{
    /// <summary>
    /// Pins down that a snapshot carrying parameters can still be re-recorded.
    ///
    /// A parameter standing in for a number has to be written bare - <c>"id": $personId$</c> - because
    /// quoting it would make the resolved value a string. That is a template, not json, and the whole
    /// write response machinery reads the snapshot with Newtonsoft: the difference writer parses it to
    /// merge the new values in, and SnapshotShape parses it to learn which shape the file uses.
    ///
    /// Neither could read a bare placeholder. Recording such a snapshot threw
    /// <c>JsonReaderException: Unexpected character encountered while parsing value: $</c>, and the shape
    /// lookup - which swallows parse errors by design - silently reported "no shape" and would have
    /// replaced the bare body with a full envelope. So every test using a numeric parameter was excluded
    /// from write response, and the failure only appeared once somebody switched recording on.
    ///
    /// See PlaceholderJson for how both sides move into a parseable form and back.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("ParameterizedSnapshot")]
    public sealed class ParameterizedSnapshotTests : ApiTestBase
    {
        private const string Snapshot = "Responses.ParameterizedPerson.json";

        private const string PersonUrl = "api/v1/persons/1";

        [TestMethod]
        [TestCategory("GET")]
        public async Task ASnapshotWithABareNumericParameterMustBeRecordable()
        {
            var snapshotPath = SnapshotPath();
            var before = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

            try
            {
                // writeResponse drives the difference writer, which is the path that could not read the
                // bare placeholder. Without it the assert alone would never touch Newtonsoft.
                await Client.AssertGetAsync<Person>(PersonUrl,
                                                    Snapshot,
                                                    parameters: [("$name$", "Son"), ("$age$", 99)],
                                                    writeResponse: true)
                            .ConfigureAwait(false);

                var after = await File.ReadAllTextAsync(snapshotPath).ConfigureAwait(false);

                Assert.That.AreEqual(before,
                                     after,
                                     because: "Nothing about this response differs from the snapshot, so a recording run has to leave the file exactly as it was. If it changed, the placeholders did not survive the round trip - most likely they came back quoted, which turns the resolved value into a string and breaks the very next run.",
                                     fix: "Check PlaceholderJson.Restore and the bare token set the writers hand it: a name that stood bare in the file has to be written back bare.");
            }
            finally
            {
                await File.WriteAllTextAsync(snapshotPath, before).ConfigureAwait(false);
            }
        }

        private static string SnapshotPath([CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(new FileInfo(callerFilePath).Directory!.FullName, "Responses", "ParameterizedPerson.json");
        }
    }
}