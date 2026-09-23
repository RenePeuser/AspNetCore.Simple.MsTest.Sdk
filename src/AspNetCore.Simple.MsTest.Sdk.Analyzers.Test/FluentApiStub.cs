namespace AspNetCore.Simple.MsTest.Sdk.Analyzers.Test
{
    /// <summary>
    /// A minimal, self-contained source stub of the fluent HTTP-assertion API surface.
    ///
    /// <para>
    /// The analyzers key off the <c>[FluentBuilder]</c> attribute by its full display name
    /// (<c>AspNetCore.Simple.MsTest.Sdk.FluentAssertions.FluentBuilderAttribute</c>), NOT off the real
    /// SDK assembly. So the tests declare that exact type here and build only against this stub —
    /// isolated, fast, and free of the ASP.NET dependency graph.
    /// </para>
    ///
    /// <para>
    /// Mirrors the Endpoint-Stil surface (<c>Accepts</c> / <c>Produces</c> / <c>ExpectedResponse…</c>),
    /// the one canonical style. It used to also carry the discarded Neutral style
    /// (<c>WithBody</c> / <c>Returns</c> / <c>Expecting…</c>) — which meant the analyzer tests were
    /// written against an API that no longer exists, exactly the doc-drift DESIGN_VISION §1 warns about.
    /// Keep the shapes in sync with the real interfaces.
    /// </para>
    /// </summary>
    internal static class FluentApiStub
    {
        /// <summary>
        /// Source prepended to every analyzer/code-fix test. Provides the marker attribute, the four
        /// builder interfaces (request / response / comparison / expectation), and a <c>Client</c> entry
        /// point so test chains read exactly like real usage.
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
                                             IHttpRequestConfiguring Accepts<T>(T body);
                                             IHttpRequestConfiguring AcceptsFromJsonString(string bodyJson);
                                             IHttpRequestConfiguring AcceptsFromEmbeddedJson(string embeddedFileName);
                                             IHttpRequestConfiguring WithParameter(string key, object? value);
                                             IHttpRequestConfiguring WithHeader(string key, string value);
                                             IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode);
                                             IHttpResponseConfiguring<T> Produces<T>(int statusCode);
                                             IHttpExpectationConfiguring Produces(HttpStatusCode statusCode);
                                             IHttpExpectationConfiguring Produces(int statusCode);
                                         }

                                         [FluentBuilder]
                                         public interface IHttpResponseConfiguring<TResult>
                                         {
                                             IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected);
                                             IHttpComparisonConfiguring<TResult> ExpectedResponseFromJsonString(string expectedJson);
                                             IHttpComparisonConfiguring<TResult> ExpectedResponseFromEmbeddedJson(string embeddedFileName);
                                             Task<TResult> ExecuteAsync();
                                         }

                                         [FluentBuilder]
                                         public interface IHttpComparisonConfiguring<TResult>
                                         {
                                             IHttpComparisonConfiguring<TResult> WriteSnapshot(bool write = true);
                                             Task<TResult> ExecuteAsync();
                                         }

                                         [FluentBuilder]
                                         public interface IHttpExpectationConfiguring
                                         {
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
                                             public static IHttpRequestConfiguring AssertPut(string url) => null!;
                                             public static IHttpRequestConfiguring AssertPatch(string url) => null!;
                                             public static IHttpRequestConfiguring AssertDelete(string url) => null!;
                                         }
                                     }
                                     """;
    }
}