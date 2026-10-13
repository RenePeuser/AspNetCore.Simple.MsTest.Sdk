using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Initial fluent state after <c>Client.AssertOptions(url)</c>. An OPTIONS call has no body to
    /// compare — the response HEADERS are its payload — so the only step here is naming the expected
    /// headers (Schema A: object / raw JSON / embedded file).
    ///
    /// <para>
    /// There is deliberately no <c>ExecuteAsync()</c> here (type-state): an OPTIONS assertion without
    /// expected headers would only check the status code and silently prove nothing about the
    /// advertised verbs.
    /// </para>
    /// </summary>
    [FluentBuilder]
    public interface IHttpOptionsConfiguring
    {
        /// <summary>Sets the expected response headers, one value per header.</summary>
        IHttpOptionsExpectationConfiguring ExpectedHeaders(IImmutableDictionary<string, string> expectedHeaders);

        /// <summary>Sets the expected response headers, several values per header.</summary>
        IHttpOptionsExpectationConfiguring ExpectedHeaders(IImmutableDictionary<string, ImmutableList<string>> expectedHeaders);

        /// <summary>Sets the expected response headers from a raw JSON string (used verbatim).</summary>
        IHttpOptionsExpectationConfiguring ExpectedHeadersFromJsonString(string expectedHeadersJson);

        /// <summary>Sets the expected response headers from an embedded-resource JSON file name.</summary>
        IHttpOptionsExpectationConfiguring ExpectedHeadersFromEmbeddedJson(string embeddedFileName);
    }
}
