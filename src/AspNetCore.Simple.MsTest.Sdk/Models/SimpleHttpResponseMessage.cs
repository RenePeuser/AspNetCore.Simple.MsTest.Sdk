using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed record SimpleHttpResponseMessage
    {
        public SimpleHttpContent? Content { get; init; }

        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

        public ImmutableList<KeyValuePair<string, ImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty;

        public ImmutableList<KeyValuePair<string, ImmutableList<string>>> TrailingHeaders { get; init; } = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty;

        public bool IsSuccessStatusCode { get; init; } = true;
    }
}