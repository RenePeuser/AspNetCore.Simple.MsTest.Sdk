using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Proves the type-state of the fluent request stages by COMPILING chains against the real sdk assembly
    /// (not a stub): the wrong chains must not compile, every allowed chain must.
    ///
    /// <para>
    /// The guided shape: GET/DELETE start on <see cref="IHttpRequestConfiguring"/>, which has no <c>Accepts…</c>;
    /// POST/PUT/PATCH/QUERY start on <see cref="IHttpBodyRequestConfiguring"/>, whose every <c>Accepts…</c> leads
    /// back to <see cref="IHttpRequestConfiguring"/> - so a body is set at most once and never on GET/DELETE.
    /// Before, both chains compiled: GET sent a body the classic API never sends, and a second <c>Accepts…</c>
    /// silently replaced the first.
    /// </para>
    ///
    /// <para>
    /// Each "does not compile" test is paired with one that MUST fail: asserting that the wrong chain compiles
    /// has to throw - so the green test is green because the chain is rejected, not because the harness never
    /// compiles anything.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentTypeStateCompileTests
    {
        private const string TypeStateFix = "Check the return types of the fluent entry points and of Accepts…: GET/DELETE return IHttpRequestConfiguring (no Accepts…), POST/PUT/PATCH/QUERY return IHttpBodyRequestConfiguring, and every Accepts… returns IHttpRequestConfiguring.";

        private const string GetWithBody = """Client.AssertGet("api/v1/persons").Accepts(new { Name = "Son" }).Produces(HttpStatusCode.OK).ExecuteAsync()""";

        private const string DeleteWithBody = """Client.AssertDelete("api/v1/persons/1").AcceptsFromJsonString("{}").Produces(HttpStatusCode.NoContent).ExecuteAsync()""";

        private const string SecondAccepts = """Client.AssertPost("api/v1/persons").Accepts(new { Name = "Son" }).Accepts(new { Name = "Vegeta" }).Produces(HttpStatusCode.OK).ExecuteAsync()""";

        private const string SecondAcceptsAfterWith = """Client.AssertPut("api/v1/persons").AcceptsFromEmbeddedJson("Person.json").WithHeader("X-Id", "1").AcceptsFromJsonString("{}").Produces(HttpStatusCode.OK).ExecuteAsync()""";

        private static readonly Lazy<ImmutableArray<MetadataReference>> References = new(LoadReferences);

        // ============================================================
        // MUST NOT compile.
        // ============================================================

        [TestMethod]
        public void Get_With_Body_Should_Not_Compile()
        {
            AssertDoesNotCompile(GetWithBody, missingMember: "Accepts", onType: nameof(IHttpRequestConfiguring));
        }

        [TestMethod]
        public void Get_With_Body_Asserted_As_Compiling_Should_Fail()
        {
            Assert.That.ThrowsExactly<AssertFailedException>(() => AssertCompiles(GetWithBody),
                                                             because: "GET carries no request body - the chain has to be rejected by the compiler, so claiming it compiles must fail.",
                                                             fix: TypeStateFix);
        }

        [TestMethod]
        public void Delete_With_Body_Should_Not_Compile()
        {
            AssertDoesNotCompile(DeleteWithBody, missingMember: "AcceptsFromJsonString", onType: nameof(IHttpRequestConfiguring));
        }

        [TestMethod]
        public void Delete_With_Body_Asserted_As_Compiling_Should_Fail()
        {
            Assert.That.ThrowsExactly<AssertFailedException>(() => AssertCompiles(DeleteWithBody),
                                                             because: "DELETE carries no request body - the chain has to be rejected by the compiler, so claiming it compiles must fail.",
                                                             fix: TypeStateFix);
        }

        [TestMethod]
        public void Second_Accepts_Should_Not_Compile()
        {
            AssertDoesNotCompile(SecondAccepts, missingMember: "Accepts", onType: nameof(IHttpRequestConfiguring));
        }

        [TestMethod]
        public void Second_Accepts_Asserted_As_Compiling_Should_Fail()
        {
            Assert.That.ThrowsExactly<AssertFailedException>(() => AssertCompiles(SecondAccepts),
                                                             because: "A second Accepts… used to silently replace the first body - it has to be a compile error now.",
                                                             fix: TypeStateFix);
        }

        [TestMethod]
        public void Second_Accepts_After_A_Header_Should_Not_Compile()
        {
            AssertDoesNotCompile(SecondAcceptsAfterWith, missingMember: "AcceptsFromJsonString", onType: nameof(IHttpRequestConfiguring));
        }

        [TestMethod]
        public void Second_Accepts_After_A_Header_Asserted_As_Compiling_Should_Fail()
        {
            Assert.That.ThrowsExactly<AssertFailedException>(() => AssertCompiles(SecondAcceptsAfterWith),
                                                             because: "WithHeader after Accepts… must not reopen the body - the state after the body has to survive With… calls.",
                                                             fix: TypeStateFix);
        }

        // ============================================================
        // MUST compile — every allowed order.
        // ============================================================

        [TestMethod]
        [DataRow("""Client.AssertGet("api/v1/persons/$Id$").WithParameter("Id", 1).WithHeader("X-Id", "1").SkipEndpointValidation().Produces(HttpStatusCode.OK).ExecuteAsync()""", DisplayName = "GET with parameter, header, skip")]
        [DataRow("""Client.AssertDelete("api/v1/persons/$Id$").WithParameters(new { Id = 1 }).Produces(HttpStatusCode.NoContent).ExecuteAsync()""", DisplayName = "DELETE with parameters")]
        [DataRow("""Client.AssertPost("api/v1/persons").Accepts(new { Name = "Son" }).WithHeader("X-Id", "1").Produces(HttpStatusCode.OK).ExecuteAsync()""", DisplayName = "POST body then header")]
        [DataRow("""Client.AssertPost("api/v1/persons").WithHeader("X-Id", "1").Accepts(new { Name = "Son" }).Produces(HttpStatusCode.OK).ExecuteAsync()""", DisplayName = "POST header then body")]
        [DataRow("""Client.AssertPut("api/v1/persons/$Id$").WithParameter("Id", 1).SkipEndpointValidation().AcceptsFromJsonString("{}").WithHeader("X-Id", "1").Produces(HttpStatusCode.OK).ExecuteAsync()""", DisplayName = "PUT parameter, skip, body, header")]
        [DataRow("""Client.AssertPatch("api/v1/persons").AcceptsFromEmbeddedJson("Person.json").Produces<object>(HttpStatusCode.OK).ExpectedResponse(new object()).ExecuteAsync()""", DisplayName = "PATCH body then typed expectation")]
        [DataRow("""Client.AssertQuery("api/v1/persons").Produces(HttpStatusCode.OK).ExecuteAsync()""", DisplayName = "QUERY without body")]
        [DataRow("""Client.AssertPost("api/v1/persons/1/activate").WithHeader("X-Id", "1").Produces(HttpStatusCode.NoContent).ExecuteAsync()""", DisplayName = "POST without body")]
        public void Allowed_Chain_Should_Compile(string chain)
        {
            AssertCompiles(chain);
        }

        // ============================================================
        // Harness.
        // ============================================================

        private static void AssertDoesNotCompile(string chain,
                                                 string missingMember,
                                                 string onType)
        {
            var errors = Compile(chain);

            Assert.That.IsTrue(errors.Any(error => error.Id == "CS1061" &&
                                                   error.GetMessage().Contains($"'{onType}'", StringComparison.Ordinal) &&
                                                   error.GetMessage().Contains($"'{missingMember}'", StringComparison.Ordinal)),
                               because: $"'{onType}' must not offer '{missingMember}' - the chain has to be a compile error (CS1061), not a runtime surprise.{Environment.NewLine}Chain : {chain}{Environment.NewLine}Errors: {Describe(errors)}",
                               fix: TypeStateFix);
        }

        private static void AssertCompiles(string chain)
        {
            var errors = Compile(chain);

            Assert.That.IsEmpty(errors,
                                because: $"This chain is an allowed order and has to compile.{Environment.NewLine}Chain : {chain}{Environment.NewLine}Errors: {Describe(errors)}",
                                fix: TypeStateFix);
        }

        private static ImmutableArray<Diagnostic> Compile(string chain)
        {
            var source = $$"""
                           using System.Net;
                           using System.Net.Http;
                           using System.Threading.Tasks;
                           using AspNetCore.Simple.MsTest.Sdk;

                           public static class Probe
                           {
                               public static Task Run(HttpClient Client)
                               {
                                   return {{chain}};
                               }
                           }
                           """;

            var compilation = CSharpCompilation.Create("FluentTypeStateProbe",
                                                       [CSharpSyntaxTree.ParseText(source)],
                                                       References.Value,
                                                       new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

            return compilation.GetDiagnostics()
                              .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                              .ToImmutableArray();
        }

        private static string Describe(ImmutableArray<Diagnostic> errors)
        {
            return errors.IsEmpty
                       ? "(none)"
                       : string.Join(Environment.NewLine + "        ", errors.Select(error => $"{error.Id}: {error.GetMessage()}"));
        }

        // The runtime's own assemblies plus the sdk under test - exactly what a consumer compiles against.
        private static ImmutableArray<MetadataReference> LoadReferences()
        {
            var platform = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            return platform.Append(typeof(IHttpRequestConfiguring).Assembly.Location)
                           .DistinctBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                           .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
                           .ToImmutableArray();
        }
    }
}
