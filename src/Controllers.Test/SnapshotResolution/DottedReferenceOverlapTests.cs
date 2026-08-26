using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Validation;
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
        private static EmbeddedFileLocalizer CreateLocalizer()
        {
            return new EmbeddedFileLocalizer(new TestSdkSettings(),
                                             new JsonSerializerOptions(),
                                             new PlainTextDecorator(),
                                             new SourceCodeExtractor(),
                                             new ResourceRootNamespaceResolver());
        }

        [TestMethod]
        public void ANewSnapshotMustNotBeWrittenIntoARepeatedFolderChain()
        {
            // This test file lives in SnapshotResolution - and says so again in the reference.
            var file = CreateLocalizer().LocalizeResponseFile("SnapshotResolution.Responses.NotRecordedYet.json",
                                                              ThisFile(),
                                                              Assembly.GetExecutingAssembly());

            var folder = file.EmbeddedFile?.Directory?.FullName;

            Assert.That.AreEqual(Path.Combine(new FileInfo(ThisFile()).Directory!.FullName, "Responses"),
                                 folder,
                                 because: "A reference that repeats the caller's own folder means that folder, not a second one inside it. Writing to SnapshotResolution\\SnapshotResolution\\Responses puts the recording where no assert will ever read it - the test then fails against the old snapshot while a brand new one sits unused on disk.",
                                 fix: "Check EmbeddedFileLocalizer.JoinWithoutOverlap: the overlap between the end of the caller context and the start of the dotted reference has to be written once.");
        }

        [TestMethod]
        public void AReferenceThatDoesNotRepeatTheCallerFolderStaysRelativeToIt()
        {
            // The overlap rule must not touch the ordinary case.
            var file = CreateLocalizer().LocalizeResponseFile("Responses.AlsoNotRecordedYet.json",
                                                              ThisFile(),
                                                              Assembly.GetExecutingAssembly());

            var folder = file.EmbeddedFile?.Directory?.FullName;

            Assert.That.AreEqual(Path.Combine(new FileInfo(ThisFile()).Directory!.FullName, "Responses"),
                                 folder,
                                 because: "Without an overlap there is nothing to strip - a plain reference has always been resolved relative to the test that names it, and that must not change.",
                                 fix: "Check that JoinWithoutOverlap only strips segments that really are on both sides, and never the file name itself.");
        }

        private static string ThisFile([CallerFilePath] string callerFilePath = "")
        {
            return callerFilePath;
        }
    }
}
