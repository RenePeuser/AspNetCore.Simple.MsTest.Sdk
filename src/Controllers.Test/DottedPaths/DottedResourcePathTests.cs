using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test.DottedPaths
{
    /// <summary>
    /// A manifest resource name is one flat dotted string, so it cannot say on its own where a folder
    /// ends and the file begins. Mapping it back to a path used to split blindly on '.' and took the
    /// last two segments as the file name, which broke both shapes covered here:
    ///
    /// • a folder carrying a dot - "V3.1" embeds as "V3._1" and mapped to V3\_1\
    /// • a file carrying extra dots - "my.dotted.file.json" mapped to my\dotted\file.json
    ///
    /// Both are pre-existing, and both became reachable far more often once resolution started
    /// succeeding for the cases it used to give up on.
    /// </summary>
    [TestClass]
    [TestCategory("DottedResourcePaths")]
    public sealed class DottedResourcePathTests
    {
        [TestMethod]
        public void AFolderWithADotMustMapToThatOneFolder()
        {
            var fileInfo = Localize("Responses.DottedFolder.json");

            Assert.That.AreEqual("Controllers.Test.DottedPaths.V3._1.Responses.DottedFolder.json",
                                 fileInfo.EmbeddedFileName,
                                 because: "MSBuild embeds the folder 'V3.1' as the two segments 'V3._1'. If the resource is not found under that name the fixture this test needs is not embedded at all.",
                                 fix: "Check that the folder DottedPaths\\V3.1 with Responses\\DottedFolder.json still exists and is still picked up as an EmbeddedResource in Controllers.Test.csproj.");

            Assert.That.IsNotNull(fileInfo.EmbeddedFile,
                                  because: "A flat dotted resource name has to map back to a real file on disk - without it write response has nothing to write to.",
                                  fix: "EmbeddedFileLocalizer must not give up when a segment carries a dot; check the path reconstruction for the '_1' segment.");

            Assert.That.AreEqual(Path.Combine("DottedPaths", "V3.1", "Responses", "DottedFolder.json"),
                                 RelativeToProject(fileInfo.EmbeddedFile),
                                 because: "'V3._1' has to fold back into the single folder 'V3.1'. Splitting blindly on '.' turns it into V3\\_1 - one of the two shapes this class exists for.",
                                 fix: "Check the resource-name-to-path logic in EmbeddedFileLocalizer: it has to match candidate segments against folders that really exist instead of splitting on every dot.");

            Assert.That.IsTrue(fileInfo.EmbeddedFile.Exists,
                               because: "The reconstructed path is only correct if a file really sits there - a plausible but wrong path would otherwise pass the comparison above.",
                               fix: $"Expected the fixture at {fileInfo.EmbeddedFile.FullName}. Either it was moved or the segment folding produced the wrong folder.");

            Assert.That.AreEqual("DottedFolder",
                                 JToken.Parse(fileInfo.Content)["name"]?.ToString(),
                                 because: "Resolving the right path is only half the job - the content has to come from that very file, otherwise a wrong-but-existing sibling would go unnoticed.",
                                 fix: "Check which resource stream EmbeddedFileLocalizer actually reads; the marker property 'name' identifies the fixture.");
        }

        [TestMethod]
        public void AFileNameWithExtraDotsMustStayOneFileName()
        {
            var fileInfo = Localize("Responses.my.dotted.file.json");

            Assert.That.AreEqual("Controllers.Test.DottedPaths.Responses.my.dotted.file.json",
                                 fileInfo.EmbeddedFileName,
                                 because: "A file name carrying extra dots embeds verbatim, so the reference has to resolve to exactly this resource - anything else means the fixture is missing.",
                                 fix: "Check that DottedPaths\\Responses\\my.dotted.file.json still exists and is still embedded by Controllers.Test.csproj.");

            Assert.That.IsNotNull(fileInfo.EmbeddedFile,
                                  because: "A flat dotted resource name has to map back to a real file on disk - without it write response has nothing to write to.",
                                  fix: "EmbeddedFileLocalizer must not give up when the file name itself carries dots.");

            Assert.That.AreEqual(Path.Combine("DottedPaths", "Responses", "my.dotted.file.json"),
                                 RelativeToProject(fileInfo.EmbeddedFile),
                                 because: "Taking the last two segments as the file name turns 'my.dotted.file.json' into my\\dotted\\file.json - the second of the two shapes this class exists for.",
                                 fix: "Check the resource-name-to-path logic in EmbeddedFileLocalizer: it has to keep every segment after the folder part as one file name instead of assuming exactly two.");

            Assert.That.IsTrue(fileInfo.EmbeddedFile.Exists,
                               because: "The reconstructed path is only correct if a file really sits there - a plausible but wrong path would otherwise pass the comparison above.",
                               fix: $"Expected the fixture at {fileInfo.EmbeddedFile.FullName}. Either it was moved or the file name was split into folders.");

            Assert.That.AreEqual("DottedFile",
                                 JToken.Parse(fileInfo.Content)["name"]?.ToString(),
                                 because: "Resolving the right path is only half the job - the content has to come from that very file, otherwise a wrong-but-existing sibling would go unnoticed.",
                                 fix: "Check which resource stream EmbeddedFileLocalizer actually reads; the marker property 'name' identifies the fixture.");
        }

        /// <summary>
        /// A snapshot that does not exist yet has no folder on disk to match against - the plain
        /// reading (last two segments are the file) still has to produce a usable target path,
        /// otherwise write response could not create it.
        /// </summary>
        [TestMethod]
        public void ASnapshotThatDoesNotExistYetMustStillGetATargetPath()
        {
            var fileInfo = Localize("Responses.ZzDoesNotExistYet.json");

            Assert.That.IsNotNull(fileInfo.EmbeddedFile,
                                  because: "A snapshot that does not exist yet has no folder on disk to match against, but write response still needs a target path to create it at.",
                                  fix: "When nothing matches, EmbeddedFileLocalizer has to fall back to the plain reading (last segment is the file name) instead of returning null.");

            Assert.That.AreEqual(Path.Combine("DottedPaths", "Responses", "ZzDoesNotExistYet.json"),
                                 RelativeToProject(fileInfo.EmbeddedFile),
                                 because: "The fallback target path is where write response would create the file, so it has to land next to its siblings in Responses.",
                                 fix: "Check the fallback branch in EmbeddedFileLocalizer - it has to keep the folder part of the reference and treat only the trailing name.extension as the file.");

            Assert.That.IsFalse(fileInfo.Resolved,
                                because: "Nothing is embedded under that name. Reporting it as resolved would make a comparison run against an empty snapshot instead of triggering the missing-snapshot path.",
                                fix: "EmbeddedFileLocalizer has to set Resolved only when a manifest resource was really found - a usable target path alone is not enough.");
        }

        private static EmbeddedFileInfo Localize(string reference,
                                                 [CallerFilePath] string callerFilePath = "")
        {
            var localizer = new EmbeddedFileLocalizer(new TestSdkSettings(),
                                                      new JsonSerializerOptions(),
                                                      new PlainTextDecorator(),
                                                      new SourceCodeExtractor(),
                                                      new ResourceRootNamespaceResolver());

            return localizer.LocalizeResponseFile(reference, callerFilePath, typeof(DottedResourcePathTests).Assembly);
        }

        private static string RelativeToProject(FileSystemInfo file,
                                                [CallerFilePath] string callerFilePath = "")
        {
            var projectFolder = new FileInfo(callerFilePath).Directory!.Parent!;

            return Path.GetRelativePath(projectFolder.FullName, file.FullName);
        }
    }
}