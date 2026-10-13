using System;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    ///     Pins the package's public namespace surface. Every public type lives in the ONE root namespace,
    ///     so the folder structure stays an implementation detail: moving a file to another folder must
    ///     never remove a namespace a consumer has a <c>using</c> for - that would turn a pure refactoring
    ///     into a breaking change and every consumer build red.
    /// </summary>
    [TestClass]
    [TestCategory("Contract")]
    public sealed class PublicNamespaceContractTests
    {
        private const string RootNamespace = "AspNetCore.Simple.MsTest.Sdk";

        [TestMethod]
        public void Public_Types_Should_Live_In_The_Root_Namespace()
        {
            var leaked = typeof(HttpClientAssertExtensions).Assembly
                                                           .GetExportedTypes()
                                                           .Where(type => type.Namespace != RootNamespace)
                                                           .Select(type => type.FullName!)
                                                           .Order(StringComparer.Ordinal)
                                                           .ToList();

            Assert.That.IsEmpty(leaked,
                                because: $"Every public type belongs to '{RootNamespace}'. A sub-namespace mirrors the folder structure, and the next restructuring would delete a namespace consumers import. Leaked: {string.Join(", ", leaked)}",
                                fix: $"Change the type's namespace to '{RootNamespace}' and add its folder to NamespaceFoldersToSkip in AspNetCore.Simple.MsTest.Sdk.csproj.DotSettings.");
        }

        [TestMethod]
        public void FluentBuilder_Attribute_Should_Keep_The_Name_The_Analyzers_Match()
        {
            // MSTESTSDK001/002 find the fluent builders by this exact name (FluentBuilderMarker), not by a
            // symbol reference. If the attribute moves, the analyzers silently stop reporting forgotten
            // terminals - and a dangling chain is a test that never sends its request and stays green.
            const string analyzerMarkerName = "AspNetCore.Simple.MsTest.Sdk.FluentBuilderAttribute";

            var attributeType = typeof(HttpClientAssertExtensions).Assembly.GetType(analyzerMarkerName);

            Assert.That.IsNotNull(attributeType,
                                  because: $"The analyzers match '{analyzerMarkerName}' by name; without a type of that name they detect nothing.",
                                  fix: "Move FluentBuilderAttribute back, or change FluentBuilderMarker.FluentBuilderAttributeName (analyzer) and FluentApiStub (analyzer tests) in the same commit.");
        }
    }
}
