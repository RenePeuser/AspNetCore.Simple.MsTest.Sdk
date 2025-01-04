using System.Collections.Generic;
using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed record SimpleHttpContent
    {
        public IImmutableList<KeyValuePair<string, IImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;

        public object? Value { get; init; } = string.Empty;
    }
}
