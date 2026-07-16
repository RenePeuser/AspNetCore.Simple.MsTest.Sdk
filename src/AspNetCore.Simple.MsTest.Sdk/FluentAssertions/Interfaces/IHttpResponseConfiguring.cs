using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent state after <c>Produces&lt;T&gt;(code)</c> (Endpoint-Stil, §15.6). The status + return type
    /// are already fixed by <c>Produces</c>. Here you optionally attach an expected body to compare
    /// against — via <c>ExpectedResponse…</c> (Schema A: object / raw JSON / embedded file).
    ///
    /// <para>
    /// <b>Type-state:</b> comparison config (<c>IgnoreProperty</c>, …) lives on
    /// <see cref="IHttpComparisonConfiguring{TResult}"/>, reachable ONLY after an <c>ExpectedResponse…</c>.
    /// Without an expected body there is nothing to configure — so you cannot even write it (§4).
    /// </para>
    ///
    /// <para>
    /// Omitting <c>ExpectedResponse…</c> and going straight to <see cref="ExecuteAsync"/> is the
    /// body-less path: the real response is still deserialized and returned as <typeparamref name="TResult"/>,
    /// but no body comparison happens.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The response type (also the return type of <see cref="ExecuteAsync"/>).</typeparam>
    [FluentBuilder]
    public interface IHttpResponseConfiguring<TResult>
    {
        /// <summary>Sets the expected response body from a C# object (serialized to JSON).</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected);

        /// <summary>Sets the expected response body from a raw JSON string (used verbatim).</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponseFromJsonString(string expectedJson);

        /// <summary>Sets the expected response body from an embedded-resource JSON file name.</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponseFromEmbeddedJson(string embeddedFileName);

        /// <summary>
        /// Executes the request WITHOUT body comparison — asserts only the status code, deserializes
        /// and returns the real response. The single terminal.
        /// </summary>
        Task<TResult> ExecuteAsync();
    }
}
