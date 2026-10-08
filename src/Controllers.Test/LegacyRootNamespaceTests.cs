using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
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
    /// Stripping the assembly name from that name yields nothing, which used to leave the snapshot
    /// without a physical file - and that silently disabled write response.
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

            Assert.That.Contains(resources,
                                 ExpectedResourceName,
                                 because: "Every other test in this class depends on a resource whose name does not start with the assembly name - without that fixture they would all pass for the wrong reason.",
                                 fix: $"Restore the LogicalName override for Legacy\\V1\\Results\\LegacyRootNamespace.json in Controllers.Test.csproj. Resources matching 'LegacyRootNamespace' right now: {found}");
        }

        [TestMethod]
        public void AReferenceUnderAForeignRootNamespaceShouldResolveToItsContent()
        {
            // Passes only if the reference reached the resource embedded under 'Pulse.Legacy.Root' and
            // its json was read - an unresolved reference fails with "snapshot not found".
            Assert.That.ObjectsAreEqual(SnapshotReference,
                                        JsonNode.Parse( /*lang=json,strict*/ """{"id":1,"name":"LegacySnapshot"}"""));
        }

        [TestMethod]
        public void WriteResponseShouldUpdateASnapshotUnderAForeignRootNamespace()
        {
            // Every writer bails out for non DEBUG assemblies - write response is a developer feature.
            Assert.That.IsTrue(typeof(LegacyRootNamespaceTests).Assembly.IsCompiledInDebug(),
                               because: "Every response writer bails out for non DEBUG assemblies - write response is a developer feature, so in a RELEASE build this test would pass without exercising anything.",
                               fix: "Run this test from a DEBUG build, or exclude it from RELEASE runs.");

            var snapshot = Path.Combine(ProjectFolder(), "Legacy", "V1", "Results", "LegacyRootNamespace.json");
            var originalContent = File.ReadAllText(snapshot);

            try
            {
                // name differs and is compared, so the assert fails - after the writer ran.
                Assert.That.ThrowsExactly<AssertFailedException>(() => Assert.That.ObjectsAreEqual(SnapshotReference,
                                                                                                   JsonNode.Parse( /*lang=json,strict*/ """{"id":1,"name":"Rewritten"}"""),
                                                                                                   writeResponse: true),
                                                                 because: "The fixture holds 'LegacySnapshot', so the comparison has to fail. If it passes, the snapshot was never compared.",
                                                                 fix: "Check that the reference resolves to the resource under the foreign root namespace.");

                var written = JToken.Parse(File.ReadAllText(snapshot));

                Assert.That.AreEqual("Rewritten",
                                     written["name"]?.ToString(),
                                     because: "Write response has to reach the snapshot even when it lives under a foreign root namespace - still reading the old value means the resource name was never mapped back to its physical file and the writer silently did nothing.",
                                     fix: "Stripping the assembly name off a foreign resource name yields nothing; EmbeddedFileLocalizer has to fall back to the resource's own root namespace when mapping back to a path (Legacy\\V1\\Results\\LegacyRootNamespace.json).");
            }
            finally
            {
                File.WriteAllText(snapshot, originalContent);
            }
        }

        [TestMethod]
        public void OneForeignResourceMustNotHijackTheProjectWideRootNamespace()
        {
            // The LogicalName fixture votes for "Pulse.Legacy.Root", every other resource of this
            // project votes for "Controllers.Test". The majority decides, otherwise a single legacy
            // file would break the resource names built for snapshots that do not exist yet.
            var failure = Assert.That.ThrowsExactly<AssertFailedException>(() => Assert.That.ObjectsAreEqual("Responses.NotRecordedUnderAnyNamespace.json",
                                                                                                             JsonNode.Parse("{}")),
                                                                           because: "The reference does not exist, so the assert has to fail and name where the snapshot was expected.",
                                                                           fix: "Check SnapshotReferenceGuard - an unresolved reference must raise SnapshotNotFoundException.");

            Assert.That.Contains(failure.Message,
                                 "Controllers.Test.Responses.NotRecordedUnderAnyNamespace.json",
                                 because: "The project-wide root namespace is decided by majority vote. One legacy file voting for 'Pulse.Legacy.Root' must not win, otherwise every snapshot that does not exist yet would get its resource name built from the wrong namespace.",
                                 fix: "Check the tallying in ResourceRootNamespaceResolver.ResolveForAssembly - it has to pick the most frequent candidate, not the first or the odd one out.");

            Assert.That.DoesNotContain(failure.Message,
                                       "Pulse.Legacy.Root.Responses",
                                       because: "The legacy namespace only owns the one resource it was given - it must not leak into the name of an unrelated snapshot.",
                                       fix: "ResolveForResource is per resource; the project-wide answer must come from ResolveForAssembly.");
        }

        private static string ProjectFolder([CallerFilePath] string callerFilePath = "")
        {
            return new FileInfo(callerFilePath).Directory!.FullName;
        }
    }
}