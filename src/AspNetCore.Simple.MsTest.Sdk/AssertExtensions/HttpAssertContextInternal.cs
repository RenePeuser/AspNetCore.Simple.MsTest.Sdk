using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Internal context object used by the master AssertHttpCallAsync method.
    /// Contains all parameters needed for HTTP assertion logic.
    /// This is a pure data object - no logic or factory methods.
    /// </summary>
    /// <typeparam name="TResult">The expected result type</typeparam>
    internal sealed class HttpAssertContextInternal<TResult>
    {
        public required HttpClient Client { get; init; }

        public required string Url { get; init; }

        public required string PayloadAsJson { get; init; }

        public required string ExpectedResult { get; init; }

        public required HttpMethod HttpMethod { get; init; }

        public Func<TResult, TResult> FilterFunc { get; init; } = item => item;

        public Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; } = difference => difference;

        public (string Key, object? Value)[] Parameters { get; init; } = [];

        public required Assembly CallingAssembly { get; init; }

        public bool WriteResponse { get; init; }

        public required bool IsSuccessStatusCode { get; init; } = true;

        public required string CallerFilePath { get; init; } = string.Empty;

        public required string PayloadParameterName { get; init; } = string.Empty;

        public required string ExpectedResultParameterName { get; init; } = string.Empty;
    }

    /// <summary>
    /// Internal context object for non-generic HTTP assertions.
    /// This is a pure data object - no logic or factory methods.
    /// </summary>
    internal sealed class HttpAssertContextInternal
    {
        public required HttpClient Client { get; init; }

        public required string Url { get; init; }

        public required string? PayloadAsJson { get; init; }

        public required HttpMethod HttpMethod { get; init; }

        public (string Key, object? Value)[] Parameters { get; init; } = [];

        public required Assembly CallingAssembly { get; init; }

        public bool WriteResponse { get; init; }

        public required bool IsSuccessStatusCode { get; init; } = true;

        public required string CallerFilePath { get; init; } = string.Empty;

        public required string PayloadParameterName { get; init; } = string.Empty;
    }
}
