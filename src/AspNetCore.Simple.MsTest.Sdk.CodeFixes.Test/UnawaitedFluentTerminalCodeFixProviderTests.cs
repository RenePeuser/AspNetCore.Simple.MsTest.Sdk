using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Analyzers;
using AspNetCore.Simple.MsTest.Sdk.Analyzers.Test;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AspNetCore.Simple.MsTest.Sdk.CodeFixes.Test
{
    /// <summary>
    /// Verifies the MSTESTSDK002 code fix: it adds the missing <c>await</c> and makes the enclosing scope
    /// async (void → Task). Unlike the MSTESTSDK001 fix it must NOT append a second
    /// <c>.ExecuteAsync()</c> — the terminal is already there.
    /// </summary>
    [TestClass]
    public sealed class UnawaitedFluentTerminalCodeFixProviderTests
    {
        private static Task VerifyFixAsync(string testCode,
                                           string fixedCode)
        {
            var test = new CSharpCodeFixTest<UnawaitedFluentTerminalAnalyzer,
                           UnawaitedFluentTerminalCodeFixProvider,
                           DefaultVerifier>
            {
                TestCode = testCode,
                FixedCode = fixedCode
            };

            return test.RunAsync();
        }

        // The fluent-API stub (marker attribute + builder interfaces + Client entry) prefixed to each case.
        private const string Stub = FluentApiStub.Source;

        // ---------------------------------------------------------------
        // Already async: only 'await' is added, the terminal stays as it is.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Adds_Await_Without_Duplicating_Terminal()
        {
            var test = Stub + """

                              namespace TestApp
                              {
                                  using System.Threading.Tasks;
                                  using System.Net;

                                  public class Tests
                                  {
                                      public async Task Run()
                                      {
                                          [|Client.AssertPost("api/persons")
                                              .Accepts(new Person())
                                              .Produces<Person>(HttpStatusCode.Created)
                                              .ExecuteAsync();|]
                                          await Task.CompletedTask;
                                      }
                                  }
                              }
                              """;

            var fixedCode = Stub + """

                                   namespace TestApp
                                   {
                                       using System.Threading.Tasks;
                                       using System.Net;

                                       public class Tests
                                       {
                                           public async Task Run()
                                           {
                                               await Client.AssertPost("api/persons")
                                                   .Accepts(new Person())
                                                   .Produces<Person>(HttpStatusCode.Created)
                                                   .ExecuteAsync();
                                               await Task.CompletedTask;
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // Sync void method (the silent-green case): becomes 'async Task'.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Converts_Void_Method_To_Async_Task()
        {
            var test = Stub + """

                              namespace TestApp
                              {
                                  using System.Net;

                                  public class Tests
                                  {
                                      public void Run()
                                      {
                                          [|Client.AssertPost("api/persons")
                                              .Accepts(new Person())
                                              .Produces(HttpStatusCode.Conflict)
                                              .ExecuteAsync();|]
                                      }
                                  }
                              }
                              """;

            var fixedCode = Stub + """

                                   namespace TestApp
                                   {
                                       using System.Net;

                                       public class Tests
                                       {
                                           public async Task Run()
                                           {
                                               await Client.AssertPost("api/persons")
                                                   .Accepts(new Person())
                                                   .Produces(HttpStatusCode.Conflict)
                                                   .ExecuteAsync();
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // ConfigureAwait chain: 'await' goes in front of the whole expression.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Adds_Await_In_Front_Of_ConfigureAwait_Chain()
        {
            var test = Stub + """

                              namespace TestApp
                              {
                                  using System.Threading.Tasks;
                                  using System.Net;

                                  public class Tests
                                  {
                                      public async Task Run()
                                      {
                                          [|Client.AssertGet("api/persons")
                                              .Produces<Person>(HttpStatusCode.OK)
                                              .ExecuteAsync().ConfigureAwait(false);|]
                                          await Task.CompletedTask;
                                      }
                                  }
                              }
                              """;

            var fixedCode = Stub + """

                                   namespace TestApp
                                   {
                                       using System.Threading.Tasks;
                                       using System.Net;

                                       public class Tests
                                       {
                                           public async Task Run()
                                           {
                                               await Client.AssertGet("api/persons")
                                                   .Produces<Person>(HttpStatusCode.OK)
                                                   .ExecuteAsync().ConfigureAwait(false);
                                               await Task.CompletedTask;
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // FixAll: several unawaited terminals repaired in one pass.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task FixAll_Repairs_Multiple_Unawaited_Terminals()
        {
            var test = Stub + """

                              namespace TestApp
                              {
                                  using System.Threading.Tasks;
                                  using System.Net;

                                  public class Tests
                                  {
                                      public async Task Run()
                                      {
                                          [|Client.AssertGet("api/persons").Produces<Person>(HttpStatusCode.OK).ExecuteAsync();|]
                                          [|Client.AssertDelete("api/persons/1").Produces(HttpStatusCode.NoContent).ExecuteAsync();|]
                                          await Task.CompletedTask;
                                      }
                                  }
                              }
                              """;

            var fixedCode = Stub + """

                                   namespace TestApp
                                   {
                                       using System.Threading.Tasks;
                                       using System.Net;

                                       public class Tests
                                       {
                                           public async Task Run()
                                           {
                                               await Client.AssertGet("api/persons").Produces<Person>(HttpStatusCode.OK).ExecuteAsync();
                                               await Client.AssertDelete("api/persons/1").Produces(HttpStatusCode.NoContent).ExecuteAsync();
                                               await Task.CompletedTask;
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }
    }
}
