namespace AspNetCore.Simple.MsTest.Sdk.Analyzers.Test
{
    /// <summary>
    /// A minimal, self-contained source stub of the fluent HTTP-assertion API surface.
    ///
    /// <para>
    /// The analyzer keys off the <c>[FluentBuilder]</c> attribute by its full display name
    /// (<c>AspNetCore.Simple.MsTest.Sdk.FluentAssertions.FluentBuilderAttribute</c>), NOT off the real
    /// SDK assembly. So the tests declare that exact type here and build only against this stub —
    /// isolated, fast, and free of the ASP.NET dependency graph. Keep the namespaces/shapes in sync
    /// with the real interfaces.
    /// </para>
    /// </summary>
    internal static class FluentApiStub
    {
        /// <summary>
        /// Source prepended to every analyzer/code-fix test. Provides the marker attribute, the three
        /// builder interfaces (request / response / expectation), and a <c>Client</c> entry point so
        /// test chains read exactly like real usage.
        /// </summary>
        public const string Source = """
                                     using System;
                                     using System.Net;
                                     using System.Threading.Tasks;

                                     namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions
                                     {
                                         [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true)]
                                         public sealed class FluentBuilderAttribute : Attribute { }
                                     }

                                     namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
                                     {
                                         using AspNetCore.Simple.MsTest.Sdk.FluentAssertions;

                                         [FluentBuilder]
                                         public interface IHttpRequestConfiguring
                                         {
                                             IHttpRequestConfiguring WithBody<T>(T body);
                                             IHttpRequestConfiguring WithJsonString(string bodyJson);
                                             IHttpResponseConfiguring<TResult> Returns<TResult>(TResult expected);
                                             IHttpExpectationConfiguring ExpectingResponse();

                                             IHttpRequestConfiguring Accepts<T>(T body);
                                             IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode);
                                             IHttpExpectationConfiguring Produces(HttpStatusCode statusCode);
                                         }

                                         [FluentBuilder]
                                         public interface IHttpResponseConfiguring<TResult>
                                         {
                                             IHttpResponseConfiguring<TResult> ExpectingSuccess();
                                             IHttpResponseConfiguring<TResult> ExpectingStatus(HttpStatusCode code);
                                             IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected);
                                             Task<TResult> ExecuteAsync();
                                         }

                                         [FluentBuilder]
                                         public interface IHttpComparisonConfiguring<TResult>
                                         {
                                             Task<TResult> ExecuteAsync();
                                         }

                                         [FluentBuilder]
                                         public interface IHttpExpectationConfiguring
                                         {
                                             IHttpExpectationConfiguring ExpectingSuccess();
                                             IHttpExpectationConfiguring ExpectingStatus(HttpStatusCode code);
                                             Task ExecuteAsync();
                                         }
                                     }

                                     namespace TestApp
                                     {
                                         using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

                                         public class Person { public int Id { get; set; } public string? Name { get; set; } }

                                         public static class Client
                                         {
                                             public static IHttpRequestConfiguring AssertPost(string url) => null!;
                                             public static IHttpRequestConfiguring AssertGet(string url) => null!;
                                             public static IHttpRequestConfiguring AssertDelete(string url) => null!;
                                         }
                                     }
                                     """;
    }
}