using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers.Test
{
    using Verify = CSharpAnalyzerVerifier<DanglingFluentChainAnalyzer, DefaultVerifier>;

    /// <summary>
    /// Verifies MSTESTSDK001: it MUST fire on a dangling (never-terminated) fluent chain, and MUST NOT
    /// fire when the chain is consumed (awaited / returned / assigned) or already terminated.
    /// </summary>
    [TestClass]
    public sealed class DanglingFluentChainAnalyzerTests
    {
        // Wraps a test body inside an async method + the fluent-API stub so the snippet compiles.
        private static string InMethod(string body)
        {
            return FluentApiStub.Source
                   + """

                     namespace TestApp
                     {
                         using System.Threading.Tasks;
                         using System.Net;

                         public class Tests
                         {
                             public async Task Run()
                             {
                     """
                   + body
                   + """
                             }
                         }
                     }
                     """;
        }

        private static Task VerifyAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<DanglingFluentChainAnalyzer, DefaultVerifier>
            {
                TestCode = source
            };

            test.ExpectedDiagnostics.AddRange(expected);

            return test.RunAsync();
        }

        // ---------------------------------------------------------------
        // MUST fire — dangling chains (request is never sent → silent green).
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Fires_When_ResponseChain_Not_Terminated()
        {
            var source = InMethod("""
                        {|#0:Client.AssertPost("api/persons")
                            .WithBody(new Person())
                            .Returns<Person>(new Person())
                            .ExpectingStatus(HttpStatusCode.Created);|}
            """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_When_StatusOnlyChain_Not_Terminated()
        {
            var source = InMethod("""
                        {|#0:Client.AssertGet("api/persons")
                            .ExpectingResponse()
                            .ExpectingSuccess();|}
            """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_When_RequestBuilder_Dangles_Without_Expectation()
        {
            // Even a bare request builder carries [FluentBuilder] → still a dangling, unsent chain.
            var source = InMethod("""
                        {|#0:Client.AssertPost("api/persons").WithBody(new Person());|}
            """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        // ---------------------------------------------------------------
        // MUST NOT fire — chain is consumed or terminated.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task DoesNotFire_When_Awaited_And_Terminated()
        {
            var source = InMethod("""
                        await Client.AssertPost("api/persons")
                            .Returns<Person>(new Person())
                            .ExpectingStatus(HttpStatusCode.Created)
                            .ExecuteAsync();
            """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_When_Assigned_To_Variable()
        {
            // Stored-then-terminated-later pattern: deliberately NOT flagged (avoids false positives).
            var source = InMethod("""
                        var chain = Client.AssertPost("api/persons").Returns<Person>(new Person());
                        await chain.ExecuteAsync();
            """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_On_Unrelated_Fluent_Chain()
        {
            var source = FluentApiStub.Source
                + """

                namespace TestApp
                {
                    public class Other
                    {
                        public Other Configure() => this;
                        public void Run()
                        {
                            // Not a [FluentBuilder] type → must be ignored.
                            new Other().Configure().Configure();
                        }
                    }
                }
                """;

            return VerifyAsync(source);
        }
    }
}
