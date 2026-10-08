using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.SnapshotResolution
{
    /// <summary>
    /// Where a snapshot that does not exist YET is going to be written.
    ///
    /// While a snapshot is embedded it is found by its name, wherever it sits - the caller context
    /// only breaks ties. The first recording has nothing to find, so the target path is derived from
    /// the caller context plus the reference, and that derivation has to land on the same folder the
    /// reference would have been read from. Otherwise write response creates a file in a folder
    /// nothing reads, the assert keeps comparing against the old content, and the test stays red with
    /// a freshly written snapshot sitting right next to it.
    ///
    /// The trap is a reference that repeats the folders the test itself lives in - the normal way to
    /// write it when snapshots are addressed from the project root.
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotResolution")]
    public sealed class DottedReferenceOverlapTests
    {
        [TestMethod]
        public void ANewSnapshotMustNotBeWrittenIntoARepeatedFolderChain()
        {
            // This test file lives in SnapshotResolution - and says so again in the reference.
            var created = Record("SnapshotResolution.Responses.NotRecordedYet.json",
                                 "NotRecordedYet.json",
                                 Path.Combine(ThisFolder(), "SnapshotResolution"));

            Assert.That.AreEqual(Path.Combine(ThisFolder(), "Responses", "NotRecordedYet.json"),
                                 created,
                                 because: "A reference that repeats the caller's own folder means that folder, not a second one inside it. Writing to SnapshotResolution\\SnapshotResolution\\Responses puts the recording where no assert will ever read it - the test then fails against the old snapshot while a brand new one sits unused on disk.",
                                 fix: "Check EmbeddedFileLocalizer.JoinWithoutOverlap: the overlap between the end of the caller context and the start of the dotted reference has to be written once.");
        }

        [TestMethod]
        public void AReferenceThatDoesNotRepeatTheCallerFolderStaysRelativeToIt()
        {
            // The overlap rule must not touch the ordinary case.
            var created = Record("Responses.AlsoNotRecordedYet.json",
                                 "AlsoNotRecordedYet.json");

            Assert.That.AreEqual(Path.Combine(ThisFolder(), "Responses", "AlsoNotRecordedYet.json"),
                                 created,
                                 because: "Without an overlap there is nothing to strip - a plain reference has always been resolved relative to the test that names it, and that must not change.",
                                 fix: "Check that JoinWithoutOverlap only strips segments that really are on both sides, and never the file name itself.");
        }

        /// <summary>
        /// Records <paramref name="reference" /> through write response and hands back where the file
        /// appeared below this folder - then deletes it again, wherever it landed.
        /// </summary>
        private static string? Record(string reference,
                                      string fileName,
                                      string? strayFolder = null)
        {
            var candidates = new[]
                             {
                                 Path.Combine(ThisFolder(), "Responses", fileName),
                                 Path.Combine(strayFolder ?? ThisFolder(), "Responses", fileName)
                             };

            try
            {
                Assert.That.ObjectsAreEqual(reference,
                                            JsonNode.Parse( /*lang=json,strict*/ """{"name":"Created"}"""),
                                            writeResponse: true);

                return Directory.EnumerateFiles(ThisFolder(), fileName, SearchOption.AllDirectories)
                                .FirstOrDefault();
            }
            finally
            {
                foreach (var candidate in Directory.EnumerateFiles(ThisFolder(), fileName, SearchOption.AllDirectories).Concat(candidates))
                {
                    if (File.Exists(candidate))
                    {
                        File.Delete(candidate);
                    }
                }

                var stray = Path.Combine(ThisFolder(), "SnapshotResolution");

                if (Directory.Exists(stray) && Directory.EnumerateFileSystemEntries(stray, "*", SearchOption.AllDirectories).All(Directory.Exists))
                {
                    Directory.Delete(stray, recursive: true);
                }
            }
        }

        private static string ThisFolder([CallerFilePath] string callerFilePath = "")
        {
            return new FileInfo(callerFilePath).Directory!.FullName;
        }
    }
}
