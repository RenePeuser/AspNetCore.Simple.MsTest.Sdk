using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;
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
    /// succeeding for the cases it used to give up on. Seen from the outside the mapping decides two
    /// things: which snapshot an assert compares against, and which file write response rewrites.
    /// </summary>
    [TestClass]
    [TestCategory("DottedResourcePaths")]
    public sealed class DottedResourcePathTests
    {
        [TestMethod]
        public void AFolderWithADotMustResolveToItsSnapshot()
        {
            // Passes only if the reference found the resource embedded as 'V3._1' and read its json.
            Assert.That.ObjectsAreEqual("Responses.DottedFolder.json",
                                        JsonNode.Parse( /*lang=json,strict*/ """{"name":"DottedFolder"}"""));
        }

        [TestMethod]
        public void AFolderWithADotMustMapToThatOneFolderWhenWritten()
        {
            var written = Rewrite("Responses.DottedFolder.json",
                                  Path.Combine(ProjectFolder(), "DottedPaths", "V3.1", "Responses", "DottedFolder.json"));

            Assert.That.AreEqual("Rewritten",
                                 written,
                                 because: "'V3._1' has to fold back into the single folder 'V3.1'. Splitting blindly on '.' turns it into V3\\_1 - write response then writes somewhere nothing reads and the fixture stays untouched.",
                                 fix: "Check the resource-name-to-path logic in EmbeddedFileLocalizer: it has to match candidate segments against folders that really exist instead of splitting on every dot.");
        }

        [TestMethod]
        public void AFileNameWithExtraDotsMustResolveToItsSnapshot()
        {
            Assert.That.ObjectsAreEqual("Responses.my.dotted.file.json",
                                        JsonNode.Parse( /*lang=json,strict*/ """{"name":"DottedFile"}"""));
        }

        [TestMethod]
        public void AFileNameWithExtraDotsMustStayOneFileNameWhenWritten()
        {
            var written = Rewrite("Responses.my.dotted.file.json",
                                  Path.Combine(ProjectFolder(), "DottedPaths", "Responses", "my.dotted.file.json"));

            Assert.That.AreEqual("Rewritten",
                                 written,
                                 because: "Taking the last two segments as the file name turns 'my.dotted.file.json' into my\\dotted\\file.json - the second of the two shapes this class exists for. Write response would then miss the real file.",
                                 fix: "Check the resource-name-to-path logic in EmbeddedFileLocalizer: it has to keep every segment after the folder part as one file name instead of assuming exactly two.");
        }

        /// <summary>
        /// A snapshot that does not exist yet has no folder on disk to match against - the plain
        /// reading (last two segments are the file) still has to produce a usable target path,
        /// otherwise write response could not create it.
        /// </summary>
        [TestMethod]
        public void ASnapshotThatDoesNotExistYetMustBeCreatedNextToItsSiblings()
        {
            var target = Path.Combine(ProjectFolder(), "DottedPaths", "Responses", "ZzDoesNotExistYet.json");

            Assert.That.ThrowsExactly<AssertFailedException>(() => Assert.That.ObjectsAreEqual("Responses.ZzDoesNotExistYet.json",
                                                                                               JsonNode.Parse("{}")),
                                                             because: "Nothing is embedded under that name. Treating it as resolved would compare against an empty snapshot instead of reporting the missing one.",
                                                             fix: "EmbeddedFileLocalizer has to set Resolved only when a manifest resource was really found - a usable target path alone is not enough.");

            try
            {
                Assert.That.ObjectsAreEqual("Responses.ZzDoesNotExistYet.json",
                                            JsonNode.Parse( /*lang=json,strict*/ """{"name":"Created"}"""),
                                            writeResponse: true);

                Assert.That.IsTrue(File.Exists(target),
                                   because: "The fallback target path is where write response creates the file, so it has to land next to its siblings in Responses.",
                                   fix: "When nothing matches, EmbeddedFileLocalizer has to fall back to the plain reading (last segment is the file name) - keep the folder part of the reference and treat only the trailing name.extension as the file.");
            }
            finally
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
            }
        }

        /// <summary>
        /// Rewrites the snapshot behind <paramref name="reference" /> through write response and hands
        /// back the 'name' found at <paramref name="expectedFile" /> - then restores the fixture.
        /// </summary>
        private static string? Rewrite(string reference,
                                       string expectedFile)
        {
            Assert.That.IsTrue(typeof(DottedResourcePathTests).Assembly.IsCompiledInDebug(),
                               because: "Every response writer bails out for non DEBUG assemblies - in a RELEASE build nothing would be written.",
                               fix: "Run this test from a DEBUG build, or exclude it from RELEASE runs.");

            var original = File.ReadAllText(expectedFile);

            try
            {
                // name differs and is compared, so the assert fails - after the writer ran.
                Assert.That.ThrowsExactly<AssertFailedException>(() => Assert.That.ObjectsAreEqual(reference,
                                                                                                   JsonNode.Parse( /*lang=json,strict*/ """{"name":"Rewritten"}"""),
                                                                                                   writeResponse: true),
                                                                 because: "The fixture holds a different name, so the comparison has to fail. If it passes, the snapshot was never compared.",
                                                                 fix: $"Check that {reference} still resolves.");

                return JToken.Parse(File.ReadAllText(expectedFile))["name"]?.ToString();
            }
            finally
            {
                File.WriteAllText(expectedFile, original);
            }
        }

        private static string ProjectFolder([CallerFilePath] string callerFilePath = "")
        {
            return new FileInfo(callerFilePath).Directory!.Parent!.FullName;
        }
    }
}
