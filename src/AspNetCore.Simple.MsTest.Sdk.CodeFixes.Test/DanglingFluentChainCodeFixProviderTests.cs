using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Analyzers;
using AspNetCore.Simple.MsTest.Sdk.Analyzers.Test;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AspNetCore.Simple.MsTest.Sdk.CodeFixes.Test
{
    /// <summary>
    /// Verifies the MSTESTSDK001 code fix: it appends the terminal <c>.ExecuteAsync()</c>, wraps the
    /// chain in <c>await</c>, and makes the enclosing scope async (void → Task) so the result compiles.
    /// </summary>
    [TestClass]
    public sealed class DanglingFluentChainCodeFixProviderTests
    {
        private static Task VerifyFixAsync(string testCode,
                                           string fixedCode)
        {
            var test = new CSharpCodeFixTest<DanglingFluentChainAnalyzer,
                           DanglingFluentChainCodeFixProvider,
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
        // Already async: only append '.ExecuteAsync()' + 'await'.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Appends_Terminal_And_Await_When_Method_Already_Async()
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
                                              .Produces<Person>(HttpStatusCode.Created);|]
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
                                                   .Produces<Person>(HttpStatusCode.Created).ExecuteAsync();
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // Sync void method: becomes 'async Task', await added.
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
                                          [|Client.AssertGet("api/persons")
                                              .WithHeader("X-Trace", "1")
                                              .Produces(HttpStatusCode.NoContent);|]
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
                                               await Client.AssertGet("api/persons")
                                                   .WithHeader("X-Trace", "1")
                                                   .Produces(HttpStatusCode.NoContent).ExecuteAsync();
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // Sync void local function: becomes 'async Task', await added.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task Converts_Void_LocalFunction_To_Async_Task()
        {
            var test = Stub + """

                              namespace TestApp
                              {
                                  using System.Net;

                                  public class Tests
                                  {
                                      public void Outer()
                                      {
                                          void Inner()
                                          {
                                              [|Client.AssertGet("api/persons")
                                                  .WithHeader("X-Trace", "1")
                                                  .Produces(HttpStatusCode.NoContent);|]
                                          }
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
                                           public void Outer()
                                           {
                                               async Task Inner()
                                               {
                                                   await Client.AssertGet("api/persons")
                                                       .WithHeader("X-Trace", "1")
                                                       .Produces(HttpStatusCode.NoContent).ExecuteAsync();
                                               }
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }

        // ---------------------------------------------------------------
        // FixAll: several dangling chains in one method fixed in a single pass.
        // ---------------------------------------------------------------

        [TestMethod]
        public Task FixAll_Repairs_Multiple_Dangling_Chains()
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
                                          [|Client.AssertPost("api/persons").Accepts(new Person()).Produces<Person>(HttpStatusCode.Created);|]
                                          [|Client.AssertGet("api/persons").Produces(HttpStatusCode.NoContent);|]
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
                                               await Client.AssertPost("api/persons").Accepts(new Person()).Produces<Person>(HttpStatusCode.Created).ExecuteAsync();
                                               await Client.AssertGet("api/persons").Produces(HttpStatusCode.NoContent).ExecuteAsync();
                                           }
                                       }
                                   }
                                   """;

            return VerifyFixAsync(test, fixedCode);
        }
    }
}