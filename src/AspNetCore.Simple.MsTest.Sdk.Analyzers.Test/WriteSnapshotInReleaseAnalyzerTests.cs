using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers.Test
{
    using Verify = CSharpAnalyzerVerifier<WriteSnapshotInReleaseAnalyzer, DefaultVerifier>;

    /// <summary>
    /// Verifies MSTESTSDK003: it MUST fire for a recording <c>WriteSnapshot()</c> in a Release build, and MUST
    /// NOT fire in Debug â€” that is where the author records and debugs.
    ///
    /// <para>
    /// Every Release case is paired with the same source compiled in Debug that must stay silent, so a gate
    /// that got lost (warning in Debug too) or inverted turns a test red either way.
    /// </para>
    /// </summary>
    [TestClass]
    public sealed class WriteSnapshotInReleaseAnalyzerTests
    {
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
                             }
                         }
                     }
                     """;
        }

        private static Task VerifyAsync(OptimizationLevel optimizationLevel,
                                        string source,
                                        params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<WriteSnapshotInReleaseAnalyzer, DefaultVerifier> { TestCode = source };

            test.SolutionTransforms.Add((solution, projectId) =>
                                        {
                                            var options = solution.GetProject(projectId)!.CompilationOptions!;

                                            return solution.WithProjectCompilationOptions(projectId, options.WithOptimizationLevel(optimizationLevel));
                                        });

            test.ExpectedDiagnostics.AddRange(expected);

            return test.RunAsync();
        }

        private const string WriteSnapshotDefault = """
                                                                await Client.AssertPost("api/persons")
                                                                    .Accepts(new Person())
                                                                    .Produces<Person>(HttpStatusCode.Created)
                                                                    .ExpectedResponseFromEmbeddedJson("Person.json")
                                                                    .{|#0:WriteSnapshot()|}
                                                                    .ExecuteAsync();
                                                    """;

        private const string WriteSnapshotTrue = """
                                                             await Client.AssertGet("api/persons/1")
                                                                 .Produces<Person>(HttpStatusCode.OK)
                                                                 .ExpectedResponse(new Person())
                                                                 .{|#0:WriteSnapshot(true)|}
                                                                 .ExecuteAsync();
                                                 """;

        private const string WriteSnapshotNamedTrue = """
                                                                  await Client.AssertGet("api/persons/1")
                                                                      .Produces<Person>(HttpStatusCode.OK)
                                                                      .ExpectedResponse(new Person())
                                                                      .{|#0:WriteSnapshot(write: true)|}
                                                                      .ExecuteAsync();
                                                      """;

        // ---------------------------------------------------------------
        // Release â€” MUST fire for a call that records.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Fires_In_Release_For_WriteSnapshot_Without_Argument()
        {
            return VerifyAsync(OptimizationLevel.Release, InAsyncMethod(WriteSnapshotDefault), Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_In_Release_For_WriteSnapshot_True()
        {
            return VerifyAsync(OptimizationLevel.Release, InAsyncMethod(WriteSnapshotTrue), Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_In_Release_For_Named_True_Argument()
        {
            return VerifyAsync(OptimizationLevel.Release, InAsyncMethod(WriteSnapshotNamedTrue), Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_In_Release_For_Constant_True()
        {
            // A const is as fixed as a literal - hiding 'true' behind a name must not hide the recording.
            var source = InAsyncMethod("""
                                                   const bool record = true;
                                                   await Client.AssertGet("api/persons/1")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExpectedResponse(new Person())
                                                       .{|#0:WriteSnapshot(record)|}
                                                       .ExecuteAsync();
                                       """);

            return VerifyAsync(OptimizationLevel.Release, source, Verify.Diagnostic().WithLocation(0));
        }

        [TestMethod]
        public Task Fires_In_Release_On_A_Stored_Comparison_Stage()
        {
            // The call does not have to sit inside one long chain - the builder is immutable and can be branched.
            var source = InAsyncMethod("""
                                                   var stage = Client.AssertGet("api/persons/1")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExpectedResponse(new Person());
                                                   await stage.{|#0:WriteSnapshot()|}.ExecuteAsync();
                                       """);

            return VerifyAsync(OptimizationLevel.Release, source, Verify.Diagnostic().WithLocation(0));
        }

        // ---------------------------------------------------------------
        // Debug â€” the same sources MUST stay silent (recording / debugging workflow).
        // ---------------------------------------------------------------

        [TestMethod]
        public Task DoesNotFire_In_Debug_For_WriteSnapshot_Without_Argument()
        {
            return VerifyAsync(OptimizationLevel.Debug, InAsyncMethod(WriteSnapshotDefault.Replace("{|#0:", string.Empty).Replace("|}", string.Empty)));
        }

        [TestMethod]
        public Task DoesNotFire_In_Debug_For_WriteSnapshot_True()
        {
            return VerifyAsync(OptimizationLevel.Debug, InAsyncMethod(WriteSnapshotTrue.Replace("{|#0:", string.Empty).Replace("|}", string.Empty)));
        }

        [TestMethod]
        public async Task Asserting_It_Fires_In_Debug_Must_Fail()
        {
            // The pair to the Release cases: if the gate were lost, this expectation would be met.
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => VerifyAsync(OptimizationLevel.Debug,
                                                                                    InAsyncMethod(WriteSnapshotDefault),
                                                                                    Verify.Diagnostic().WithLocation(0)))
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Asserting_It_Is_Silent_In_Release_Must_Fail()
        {
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => VerifyAsync(OptimizationLevel.Release,
                                                                                    InAsyncMethod(WriteSnapshotDefault.Replace("{|#0:", string.Empty).Replace("|}", string.Empty))))
                        .ConfigureAwait(false);
        }

        // ---------------------------------------------------------------
        // Release â€” MUST NOT fire when nothing is recorded for sure.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task DoesNotFire_In_Release_For_WriteSnapshot_False()
        {
            var source = InAsyncMethod("""
                                                   await Client.AssertGet("api/persons/1")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExpectedResponse(new Person())
                                                       .WriteSnapshot(false)
                                                       .ExecuteAsync();
                                       """);

            return VerifyAsync(OptimizationLevel.Release, source);
        }

        [TestMethod]
        public Task DoesNotFire_In_Release_For_Non_Constant_Argument()
        {
            // A switch the author controls (e.g. read from configuration) - conservative, not reported.
            var source = InAsyncMethod("""
                                                   var record = System.Environment.GetEnvironmentVariable("RECORD") == "1";
                                                   await Client.AssertGet("api/persons/1")
                                                       .Produces<Person>(HttpStatusCode.OK)
                                                       .ExpectedResponse(new Person())
                                                       .WriteSnapshot(record)
                                                       .ExecuteAsync();
                                       """);

            return VerifyAsync(OptimizationLevel.Release, source);
        }

        [TestMethod]
        public Task DoesNotFire_In_Release_On_Unrelated_WriteSnapshot()
        {
            // Same method name, no [FluentBuilder] -> must be ignored.
            var source = FluentApiStub.Source
                         + """

                           namespace TestApp
                           {
                               public class Recorder
                               {
                                   public Recorder WriteSnapshot(bool write = true) => this;

                                   public void Run()
                                   {
                                       new Recorder().WriteSnapshot();
                                   }
                               }
                           }
                           """;

            return VerifyAsync(OptimizationLevel.Release, source);
        }
    }
}
