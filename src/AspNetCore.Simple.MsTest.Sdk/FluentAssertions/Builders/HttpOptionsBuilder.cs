using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Builds a fluent OPTIONS assertion. All three header sources funnel into a single JSON string and
    /// run through the same engine as the classic <c>AssertOptionsAsync</c>, so both report alike.
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
            return ExpectedHeaders(expectedHeaders.ToImmutableDictionary(item => item.Key, item => item.Value.AsImmutableList()));
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeaders(IImmutableDictionary<string, ImmutableList<string>> expectedHeaders)
        {
            _expectedHeadersJson = expectedHeaders.ToJson(HttpClientAssertExtensions.JsonSerializerOptionsFor(_callingAssembly));

            return this;
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeadersFromJsonString(string expectedHeadersJson)
        {
            _expectedHeadersJson = expectedHeadersJson;

            return this;
        }

        public IHttpOptionsExpectationConfiguring ExpectedHeadersFromEmbeddedJson(string embeddedFileName)
        {
            _expectedHeadersJson = embeddedFileName;

            return this;
        }

        public Task<HttpResponseMessage> ExecuteAsync()
        {
            return _client.AssertOptionsAsync(_url,
                                              _expectedHeadersJson,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber);
        }
    }
}
