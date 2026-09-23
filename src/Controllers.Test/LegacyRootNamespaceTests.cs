using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test
{
    /// <summary>
    /// Legacy test projects override the MSBuild RootNamespace, so their manifest resource names do
    /// not start with the assembly name. Real world example:
    /// AssemblyName "Pulse.DataManagement.Test.API.MySql", RootNamespace "Pulse.DataManagement.Test",
    /// resource "Pulse.DataManagement.Test.API.V1.Results.Foo.json" for "API\V1\Results\Foo.json".
    /// The fixture reproduces that with a LogicalName override in Controllers.Test.csproj:
    /// "Legacy\V1\Results\LegacyRootNamespace.json" is embedded as
    /// "Pulse.Legacy.Root.Legacy.V1.Results.LegacyRootNamespace.json".
    /// Stripping the assembly name from that name yields nothing, which used to leave
    /// EmbeddedFileInfo.EmbeddedFile null - and a null physical file silently disables write
    /// response, because every ISpecificResponseWriter requires it.
    /// </summary>
    [TestClass]
    [TestCategory("LegacyRootNamespace")]
    public sealed class LegacyRootNamespaceTests : SdkTestBase
    {
        private const string ExpectedResourceName = "Pulse.Legacy.Root.Legacy.V1.Results.LegacyRootNamespace.json";

        private const string SnapshotReference = "Results.LegacyRootNamespace.json";

        [TestMethod]
        public void FixtureShouldBeEmbeddedUnderAForeignRootNamespace()
        {
            var resources = typeof(LegacyRootNamespaceTests).Assembly.GetManifestResourceNames();

            var found = string.Join(", ", resources.Where(r => r.Contains("LegacyRootNamespace", StringComparison.Ordinal)));

            Assert.That.Contains(resources,
                                 ExpectedResourceName,
                                 because: "Every other test in this class depends on a resource whose name does not start with the assembly name - without that fixture they would all pass for the wrong reason.",
                                 fix: $"Restore the LogicalName override for Legacy\\V1\\Results\\LegacyRootNamespace.json in Controllers.Test.csproj. Resources matching 'LegacyRootNamespace' right now: {found}");
        }

        [TestMethod]
        public void LocalizeResponseFileShouldMapAForeignRootNamespaceToItsPhysicalFile()
        {
            var callerFilePath = ThisFile();
            var assembly = typeof(LegacyRootNamespaceTests).Assembly;

            var fileInfo = CreateLocalizer().LocalizeResponseFile(SnapshotReference, callerFilePath, assembly);

            Assert.That.AreEqual(ExpectedResourceName,
                                 fileInfo.EmbeddedFileName,
                                 because: "The reference 'Results.LegacyRootNamespace.json' has to resolve to the resource embedded under the foreign root namespace 'Pulse.Legacy.Root', not to one built from the assembly name.",
                                 fix: "Check EmbeddedFileLocalizer/ResourceRootNamespaceResolver: the resource name must be resolved against the root namespace that actually owns the resource, not against the assembly name.");

            Assert.That.IsNotNull(fileInfo.EmbeddedFile,
                                  because: "Without a physical file no response writer can handle the request - write response would silently do nothing, which is exactly the bug this class covers.",
                                  fix: "Stripping the assembly name off a foreign resource name yields nothing; EmbeddedFileLocalizer has to fall back to the resource's own root namespace when mapping back to a path.");

            Assert.That.IsTrue(fileInfo.EmbeddedFile.Exists,
                               because: "The mapped path only proves the mapping is right if the file it points at is really there.",
                               fix: $"Expected the snapshot at {fileInfo.EmbeddedFile.FullName}. Either the fixture was moved/renamed or the resource-name-to-path mapping produced the wrong folder.");

            var projectFolder = new FileInfo(callerFilePath).Directory!;

            Assert.That.AreEqual(Path.Combine("Legacy", "V1", "Results",
                                              "LegacyRootNamespace.json"),
                                 Path.GetRelativePath(projectFolder.FullName, fileInfo.EmbeddedFile.FullName),
                                 because: "The dotted resource name has to map back to exactly the folder structure it was embedded from - one segment too many or too few and write response would create a second, orphaned snapshot.",
                                 fix: "Check how the resource name is split into folders: the part belonging to the foreign root namespace must be dropped before the rest becomes the path.");

            // The content has to be the json, not the file name that was passed in.
            Assert.That.AreEqual("LegacySnapshot",
                                 JToken.Parse(fileInfo.Content)["name"]?.ToString(),
                                 because: "EmbeddedFileInfo.Content must carry the resolved json. Getting the reference string back instead would mean the resource was never actually read.",
                                 fix: "Check that EmbeddedFileLocalizer reads the manifest resource stream and does not fall through to returning the passed-in reference.");
        }

        [TestMethod]
        public void ResponseWriterShouldUpdateASnapshotUnderAForeignRootNamespace()
        {
            var callerFilePath = ThisFile();
            var assembly = typeof(LegacyRootNamespaceTests).Assembly;

            // Every writer bails out for non DEBUG assemblies - write response is a developer feature.
            Assert.That.IsTrue(assembly.IsCompiledInDebug(),
                               because: "Every response writer bails out for non DEBUG assemblies - write response is a developer feature, so in a RELEASE build this test would pass without exercising anything.",
                               fix: "Run this test from a DEBUG build, or exclude it from RELEASE runs.");

            var fileInfo = CreateLocalizer().LocalizeResponseFile(SnapshotReference, callerFilePath, assembly);

            Assert.That.IsNotNull(fileInfo.EmbeddedFile,
                                  because: "The writer needs the physical file to overwrite; a null one is how this bug used to manifest - write response did nothing and the test stayed green.",
                                  fix: "See LocalizeResponseFileShouldMapAForeignRootNamespaceToItsPhysicalFile - the resource name has to be mapped back to a path via its own root namespace.");

            var originalContent = File.ReadAllText(fileInfo.EmbeddedFile.FullName);

            try
            {
                var writer = Services.GetRequiredService<IResponseWriter>();

                var request = new WriteResponseRequest
                {
                    CallingAssembly = assembly,
                    CurrentResponseAsString = /*lang=json,strict*/ """{"id":2,"name":"Rewritten"}""",
                    ExpectedResult = fileInfo,
                    Parameters = [],
                    DifferenceFunc = differences => differences,
                    Mode = ResponseWriteMode.OverwriteAll,
                    CallerFilePath = callerFilePath,
                    CallerLineNumber = 0,
                    ExpectedResultParameterName = nameof(SnapshotReference),
                    ExpectedType = typeof(object),
                    ExpectedObject = null
                };

                writer.Write(request);

                var written = JToken.Parse(File.ReadAllText(fileInfo.EmbeddedFile.FullName));

                Assert.That.AreEqual("Rewritten",
                                     written["name"]?.ToString(),
                                     because: "Write response has to reach the snapshot even when it lives under a foreign root namespace - still reading the old value means the writer silently did nothing.",
                                     fix: "Check that OverwriteAllResponseWriter received a non-null ExpectedResult.EmbeddedFile and that ResponseWriter picked a writer at all for this request.");
            }
            finally
            {
                File.WriteAllText(fileInfo.EmbeddedFile.FullName, originalContent);
            }
        }

        [TestMethod]
        public void OneForeignResourceMustNotHijackTheProjectWideRootNamespace()
        {
            var callerFilePath = ThisFile();
            var assembly = typeof(LegacyRootNamespaceTests).Assembly;
            var projectFolder = new FileInfo(callerFilePath).Directory;

            var resolver = Services.GetRequiredService<IResourceRootNamespaceResolver>();

            // The LogicalName fixture votes for "Pulse.Legacy.Root", every other resource of this
            // project votes for "Controllers.Test". The majority decides, otherwise a single legacy
            // file would break the resource names built for snapshots that do not exist yet.
            Assert.That.AreEqual("Controllers.Test",
                                 resolver.ResolveForAssembly(assembly, projectFolder),
                                 because: "The project-wide root namespace is decided by majority vote. One legacy file voting for 'Pulse.Legacy.Root' must not win, otherwise every snapshot that does not exist yet would get its resource name built from the wrong namespace.",
                                 fix: "Check the tallying in ResourceRootNamespaceResolver.ResolveForAssembly - it has to pick the most frequent candidate, not the first or the odd one out.");

            Assert.That.AreEqual("Pulse.Legacy.Root",
                                 resolver.ResolveForResource(ExpectedResourceName, assembly, projectFolder),
                                 because: "Per resource the answer is the opposite of the project-wide one: this single file really does live under 'Pulse.Legacy.Root' and has to be resolved against it.",
                                 fix: "ResolveForResource must derive the namespace from the given resource name itself instead of returning the project-wide majority.");
        }

        private static IEmbeddedFileLocalizer CreateLocalizer()
        {
            return Services.GetRequiredService<IEmbeddedFileLocalizer>();
        }

        private static string ThisFile([CallerFilePath] string callerFilePath = "")
        {
            return callerFilePath;
        }
    }
}