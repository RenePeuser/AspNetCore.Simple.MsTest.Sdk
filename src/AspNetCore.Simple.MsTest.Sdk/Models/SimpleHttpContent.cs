using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public sealed record SimpleHttpContent
    {
        public ImmutableList<KeyValuePair<string, ImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty;

        public object? Value { get; init; } = string.Empty;
    }
}
