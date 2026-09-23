using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers.Test
{
    using Verify = CSharpAnalyzerVerifier<UnawaitedFluentTerminalAnalyzer, DefaultVerifier>;

    /// <summary>
    /// Verifies MSTESTSDK002: it MUST fire when a terminated fluent chain is never awaited, and MUST NOT
    /// fire when the task is consumed (awaited / returned / assigned / discarded).
    ///
    /// <para>
    /// The headline case is <see cref="Fires_In_Sync_Method_Where_Compiler_Is_Silent"/>: inside a
    /// synchronous test method there is no CS4014 either, so without this analyzer a wrong expectation
    /// runs detached and the test is GREEN.
    /// </para>
    /// </summary>
    [TestClass]
    public sealed class UnawaitedFluentTerminalAnalyzerTests
    {
        // Wraps a test body inside an async method + the fluent-API stub so the snippet compiles.
        private static string InAsyncMethod(string body)
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
                                 await Task.CompletedTask;
                             }
                         }
                     }
                     """;
        }

        private static Task VerifyAsync(string source,
                                        params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<UnawaitedFluentTerminalAnalyzer, DefaultVerifier> { TestCode = source };

            test.ExpectedDiagnostics.AddRange(expected);

            return test.RunAsync();
        }

        // ---------------------------------------------------------------
        // MUST fire — terminal present, task dropped → assertion runs detached.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Fires_When_Typed_Terminal_Not_Awaited()
        {
            var source = InAsyncMethod("""
                                                   {|#0:Client.AssertPost("api/persons")
                                                       .Accepts(new Person())
                                                       .Produces<Person>(HttpStatusCode.Created)
                                                       .ExecuteAsync();|}
                                       """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_When_StatusOnly_Terminal_Not_Awaited()
        {
            var source = InAsyncMethod("""
                                                   {|#0:Client.AssertPost("api/persons")
                                                       .Accepts(new Person())
                                                       .Produces(HttpStatusCode.Created)
                                                       .ExecuteAsync();|}
                                       """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_When_ConfigureAwait_Present_But_Await_Missing()
        {
            // The repo's own idiom is '…ExecuteAsync().ConfigureAwait(false)'. Dropping only the 'await'
            // leaves ConfigureAwait as the OUTERMOST call — the analyzer has to descend to find the terminal.
            var source = InAsyncMethod("""
                                                   {|#0:Client.AssertGet("api/persons")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExecuteAsync().ConfigureAwait(false);|}
                                       """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_After_Comparison_Stage()
        {
            var source = InAsyncMethod("""
                                                   {|#0:Client.AssertPost("api/persons")
                                                       .Accepts(new Person())
                                                       .Produces<Person>(HttpStatusCode.Created)
                                                       .ExpectedResponse(new Person())
                                                       .ExecuteAsync();|}
                                       """);

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_In_Sync_Method_Where_Compiler_Is_Silent()
        {
            // THE case this analyzer exists for: no async → no CS4014, and MSTESTSDK001 cannot see it
            // either because the statement's type is Task, not a [FluentBuilder]. Silently green.
            var source = FluentApiStub.Source
                         + """

                           namespace TestApp
                           {
                               using System.Net;

                               public class Tests
                               {
                                   public void Run()
                                   {
                                       {|#0:Client.AssertPost("api/persons")
                                           .Accepts(new Person())
                                           .Produces(HttpStatusCode.Conflict)
                                           .ExecuteAsync();|}
                                   }
                               }
                           }
                           """;

            return VerifyAsync(source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_Once_Per_Unawaited_Chain()
        {
            var source = InAsyncMethod("""
                                                   {|#0:Client.AssertGet("api/persons").Produces<Person>(HttpStatusCode.OK).ExecuteAsync();|}
                                                   {|#1:Client.AssertDelete("api/persons/1").Produces(HttpStatusCode.NoContent).ExecuteAsync();|}
                                       """);

            return VerifyAsync(source,
                               Verify.Diagnostic().WithLocation(0),
                               Verify.Diagnostic().WithLocation(1));
        }

        // ---------------------------------------------------------------
        // MUST NOT fire — the task is consumed.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task DoesNotFire_When_Awaited()
        {
            var source = InAsyncMethod("""
                                                   await Client.AssertPost("api/persons")
                                                       .Accepts(new Person())
                                                       .Produces<Person>(HttpStatusCode.Created)
                                                       .ExecuteAsync();
                                       """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_When_Awaited_With_ConfigureAwait()
        {
            var source = InAsyncMethod("""
                                                   await Client.AssertPost("api/persons")
                                                       .Accepts(new Person())
                                                       .Produces<Person>(HttpStatusCode.Created)
                                                       .ExecuteAsync().ConfigureAwait(false);
                                       """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_When_Returned()
        {
            // The dominant shape in this repo: 'return Client.Assert…ExecuteAsync();' in a Task method.
            var source = FluentApiStub.Source
                         + """

                           namespace TestApp
                           {
                               using System.Threading.Tasks;
                               using System.Net;

                               public class Tests
                               {
                                   public Task Run()
                                   {
                                       return Client.AssertPost("api/persons")
                                           .Accepts(new Person())
                                           .Produces<Person>(HttpStatusCode.Created)
                                           .ExecuteAsync();
                                   }
                               }
                           }
                           """;

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_When_Assigned_To_Variable()
        {
            var source = InAsyncMethod("""
                                                   var task = Client.AssertGet("api/persons")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExecuteAsync();
                                                   await task;
                                       """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_On_Explicit_Discard()
        {
            // '_ =' is a deliberate fire-and-forget. Conservative, mirroring MSTESTSDK001.
            var source = InAsyncMethod("""
                                                   _ = Client.AssertGet("api/persons")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExecuteAsync();
                                       """);

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_On_Unrelated_ExecuteAsync()
        {
            // Same method name, no [FluentBuilder] → must be ignored.
            var source = FluentApiStub.Source
                         + """

                           namespace TestApp
                           {
                               using System.Threading.Tasks;

                               public class Other
                               {
                                   public Task ExecuteAsync() => Task.CompletedTask;

                                   public void Run()
                                   {
                                       new Other().ExecuteAsync();
                                   }
                               }
                           }
                           """;

            return VerifyAsync(source);
        }

        [TestMethod]
        public Task DoesNotFire_On_Dangling_Chain_Without_Terminal()
        {
            // That shape belongs to MSTESTSDK001 — the two diagnostics must not double-report.
            var source = InAsyncMethod("""
                                                   Client.AssertPost("api/persons").Accepts(new Person());
                                       """);

            return VerifyAsync(source);
        }
    }
}