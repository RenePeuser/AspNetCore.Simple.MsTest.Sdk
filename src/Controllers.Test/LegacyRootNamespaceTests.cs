using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
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
    public sealed class LegacyRootNamespaceTests
    {
        private const string ExpectedResourceName = "Pulse.Legacy.Root.Legacy.V1.Results.LegacyRootNamespace.json";

        private const string SnapshotReference = "Results.LegacyRootNamespace.json";

        [TestMethod]
        public void FixtureShouldBeEmbeddedUnderAForeignRootNamespace()
        {
            var resources = typeof(LegacyRootNamespaceTests).Assembly.GetManifestResourceNames();

            var found = string.Join(", ", resources.Where(r => r.Contains("LegacyRootNamespace", StringComparison.Ordinal)));

            Assert.IsTrue(resources.Contains(ExpectedResourceName),
                          $"The LogicalName override in Controllers.Test.csproj is gone. Found: {found}");
        }

        [TestMethod]
        public void LocalizeResponseFileShouldMapAForeignRootNamespaceToItsPhysicalFile()
        {
            var callerFilePath = ThisFile();
            var assembly = typeof(LegacyRootNamespaceTests).Assembly;

            var fileInfo = CreateLocalizer().LocalizeResponseFile(SnapshotReference, callerFilePath, assembly);

            Assert.AreEqual(ExpectedResourceName, fileInfo.EmbeddedFileName);

            Assert.IsNotNull(fileInfo.EmbeddedFile,
                             "Without a physical file no response writer can handle the request - write response would silently do nothing.");

            Assert.IsTrue(fileInfo.EmbeddedFile.Exists, $"Not found on disk: {fileInfo.EmbeddedFile.FullName}");

            var projectFolder = new FileInfo(callerFilePath).Directory!;

            Assert.AreEqual(Path.Combine("Legacy", "V1", "Results", "LegacyRootNamespace.json"),
                            Path.GetRelativePath(projectFolder.FullName, fileInfo.EmbeddedFile.FullName));

            // The content has to be the json, not the file name that was passed in.
            Assert.AreEqual("LegacySnapshot", JToken.Parse(fileInfo.Content)["name"]?.ToString());
        }

        [TestMethod]
        public void ResponseWriterShouldUpdateASnapshotUnderAForeignRootNamespace()
        {
            var callerFilePath = ThisFile();
            var assembly = typeof(LegacyRootNamespaceTests).Assembly;

            // Every writer bails out for non DEBUG assemblies - write response is a developer feature.
            Assert.IsTrue(assembly.IsCompiledInDebug(), "This test only makes sense for a DEBUG build.");

            var fileInfo = CreateLocalizer().LocalizeResponseFile(SnapshotReference, callerFilePath, assembly);

            Assert.IsNotNull(fileInfo.EmbeddedFile);

            var originalContent = File.ReadAllText(fileInfo.EmbeddedFile.FullName);

            try
            {
                var writer = new ResponseWriter([new OverwriteAllResponseWriter(new ParameterReplacer())]);

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

                Assert.AreEqual("Rewritten", written["name"]?.ToString());
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

            var resolver = new ResourceRootNamespaceResolver();

            // The LogicalName fixture votes for "Pulse.Legacy.Root", every other resource of this
            // project votes for "Controllers.Test". The majority decides, otherwise a single legacy
            // file would break the resource names built for snapshots that do not exist yet.
            Assert.AreEqual("Controllers.Test", resolver.ResolveForAssembly(assembly, projectFolder));

            Assert.AreEqual("Pulse.Legacy.Root",
                            resolver.ResolveForResource(ExpectedResourceName, assembly, projectFolder));
        }

        private static EmbeddedFileLocalizer CreateLocalizer()
        {
            return new EmbeddedFileLocalizer(new TestCreatorSettings(),
                                             new JsonSerializerOptions(),
                                             new PlainTextDecorator(),
                                             new SourceCodeExtractor(),
                                             new ResourceRootNamespaceResolver());
        }

        private static string ThisFile([CallerFilePath] string callerFilePath = "")
        {
            return callerFilePath;
        }
    }
}