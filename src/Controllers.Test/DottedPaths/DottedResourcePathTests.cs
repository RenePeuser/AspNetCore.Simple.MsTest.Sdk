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

            Assert.AreEqual("Controllers.Test.DottedPaths.V3._1.Responses.DottedFolder.json",
                            fileInfo.EmbeddedFileName,
                            "The fixture folder DottedPaths\\V3.1 is gone or no longer embedded.");

            Assert.IsNotNull(fileInfo.EmbeddedFile);

            Assert.AreEqual(Path.Combine("DottedPaths", "V3.1", "Responses", "DottedFolder.json"),
                            RelativeToProject(fileInfo.EmbeddedFile));

            Assert.IsTrue(fileInfo.EmbeddedFile.Exists, $"Not found on disk: {fileInfo.EmbeddedFile.FullName}");
            Assert.AreEqual("DottedFolder", JToken.Parse(fileInfo.Content)["name"]?.ToString());
        }

        [TestMethod]
        public void AFileNameWithExtraDotsMustStayOneFileName()
        {
            var fileInfo = Localize("Responses.my.dotted.file.json");

            Assert.AreEqual("Controllers.Test.DottedPaths.Responses.my.dotted.file.json",
                            fileInfo.EmbeddedFileName);

            Assert.IsNotNull(fileInfo.EmbeddedFile);

            Assert.AreEqual(Path.Combine("DottedPaths", "Responses", "my.dotted.file.json"),
                            RelativeToProject(fileInfo.EmbeddedFile));

            Assert.IsTrue(fileInfo.EmbeddedFile.Exists, $"Not found on disk: {fileInfo.EmbeddedFile.FullName}");
            Assert.AreEqual("DottedFile", JToken.Parse(fileInfo.Content)["name"]?.ToString());
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

            Assert.IsNotNull(fileInfo.EmbeddedFile);

            Assert.AreEqual(Path.Combine("DottedPaths", "Responses", "ZzDoesNotExistYet.json"),
                            RelativeToProject(fileInfo.EmbeddedFile));

            Assert.IsFalse(fileInfo.Resolved, "Nothing is embedded under that name - it must not count as resolved.");
        }

        private static EmbeddedFileInfo Localize(string reference,
                                                 [CallerFilePath] string callerFilePath = "")
        {
            var localizer = new EmbeddedFileLocalizer(new TestCreatorSettings(),
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
