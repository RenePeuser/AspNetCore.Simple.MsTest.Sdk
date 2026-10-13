using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Argument.Check;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Builds a fluent OPTIONS assertion. All three header sources funnel into a single JSON string and
    /// run through the same engine as the classic <c>AssertOptionsAsync</c>, so both report alike.
    ///
    /// <para>
    /// Immutable, like the other builders: <c>ExpectedHeaders…</c> returns a NEW builder. A stored
    /// <c>AssertOptions(url)</c> stage can be branched into several expectations without the last one
    /// overwriting the others.
    /// </para>
    /// </summary>
    internal sealed class HttpOptionsBuilder : IHttpOptionsConfiguring, IHttpOptionsExpectationConfiguring
    {
        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

        private readonly HttpClient _client;

        private readonly string _url;

        private string _expectedHeadersJson = string.Empty;

        internal HttpOptionsBuilder(HttpClient client,
                                    string url,
                                    Assembly callingAssembly,
                                    string callerFilePath,
                                    string callerMemberName,
                                    int callerLineNumber)
        {
            _client = client;
            _url = url;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeaders(IImmutableDictionary<string, string> expectedHeaders)
        {
            Throw.IfNull(expectedHeaders);

            return ExpectedHeaders(expectedHeaders.ToImmutableDictionary(item => item.Key, item => item.Value.AsImmutableList()));
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeaders(IImmutableDictionary<string, ImmutableList<string>> expectedHeaders)
        {
            Throw.IfNull(expectedHeaders);

            return WithExpectedHeaders(OptionsHeaders.Serialize(expectedHeaders));
        }

        // A blank expectation is no expectation - like an empty ExpectedResponseFromJsonString it must not reach the engine.
        public IHttpOptionsExpectationConfiguring ExpectedHeadersFromJsonString(string expectedHeadersJson)
        {
            Throw.IfNullOrWhiteSpace(expectedHeadersJson);

            return WithExpectedHeaders(expectedHeadersJson);
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeadersFromEmbeddedJson(string embeddedFileName)
        {
            Throw.IfNullOrWhiteSpace(embeddedFileName);

            return WithExpectedHeaders(embeddedFileName);
        }

        // Copy-on-write - see HttpRequestBuilder.With. Mutating and returning this let a second
        // ExpectedHeaders… on a stored AssertOptions(url) stage overwrite the first branch's expectation.
        private HttpOptionsBuilder WithExpectedHeaders(string expectedHeadersJson)
        {
            var copy = (HttpOptionsBuilder)MemberwiseClone();
            copy._expectedHeadersJson = expectedHeadersJson;

            return copy;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return _client.AssertOptionsAsync(_url,
                                              _expectedHeadersJson,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              cancellationToken);
        }
    }
}
