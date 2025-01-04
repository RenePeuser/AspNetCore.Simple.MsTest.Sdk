using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed record SimpleHttpResponseMessage
    {
        public string Version { get; init; } = "1.1";

        public SimpleHttpContent? Content { get; init; }

        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

        public string ReasonPhrase { get; init; } = "OK";

        public IImmutableList<KeyValuePair<string, IImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;

        public IImmutableList<KeyValuePair<string, IImmutableList<string>>> TrailingHeaders { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;

        public bool IsSuccessStatusCode { get; init; } = true;
    }
}
