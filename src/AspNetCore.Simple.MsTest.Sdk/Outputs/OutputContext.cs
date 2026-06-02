using System.Collections.Immutable;
using System.Net;
using System.Net.Http;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Contains all data needed for formatting assertion failure output.
    /// This is a pure data container - rendering/formatting decisions are separate.
    /// </summary>
    public sealed record OutputContext
    {
        // ============================================================
        // Title & Error Information
        // ============================================================

        /// <summary>
        /// Main title for the output (e.g., "Http call infos:", "Assertion failed").
        /// Optional - can be null if no title needed.
        /// </summary>
        public string? Title { get; init; }

        /// <summary>
        /// Error description explaining what went wrong.
        /// (e.g., "Schema mismatch: Expected result and current result does not match")
        /// Optional - can be null if no specific error info needed.
        /// </summary>
        public string? ErrorInfo { get; init; }

        // ============================================================
        // HTTP Call Information (if applicable)
        // ============================================================

        /// <summary>
        /// HTTP method used in the call (GET, POST, etc.).
        /// Null if this is not an HTTP assertion.
        /// </summary>
        public HttpMethod? HttpMethod { get; init; }

        /// <summary>
        /// Full URL that was called.
        /// Null if this is not an HTTP assertion.
        /// </summary>
#pragma warning disable CA1056 // URI properties should not be strings - keeping as string for consistency with rest of codebase
        public string? Url { get; init; }
#pragma warning restore CA1056

        /// <summary>
        /// HTTP status code received.
        /// Null if this is not an HTTP assertion.
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; init; }

        // ============================================================
        // Comparison Data
        // ============================================================

        /// <summary>
        /// Expected result as JSON string.
        /// This is what the test expected to receive.
        /// </summary>
        public string? ExpectedResultAsJson { get; init; }

        /// <summary>
        /// Current/actual result as JSON string.
        /// This is what was actually received.
        /// </summary>
        public string? CurrentResultAsJson { get; init; }

        /// <summary>
        /// Name/label for the expected result (e.g., parameter name or file name).
        /// Used in difference tables and output headers.
        /// </summary>
        public string? ExpectedResultParameterName { get; init; }

        /// <summary>
        /// Name/label for the current result (e.g., "Current response", "Actual value").
        /// Used in difference tables and output headers.
        /// </summary>
        public string? CurrentResultParameterName { get; init; }

        // ============================================================
        // Differences
        // ============================================================

        /// <summary>
        /// List of differences found between expected and current.
        /// Raw difference data - can be formatted into tables or other representations.
        /// Empty list if no differences.
        /// </summary>
        public ImmutableList<Difference> Differences { get; init; } = ImmutableList<Difference>.Empty;

        /// <summary>
        /// Pre-formatted difference table as string (for backward compatibility).
        /// If provided, this can be used directly. Otherwise, format from Differences list.
        /// Optional - can be null.
        /// </summary>
        public string? DifferenceTableFormatted { get; init; }

        // ============================================================
        // Reproducibility Information
        // ============================================================

        /// <summary>
        /// Curl command to reproduce the HTTP call.
        /// Null if this is not an HTTP assertion or curl generation disabled.
        /// </summary>
        public string? Curl { get; init; }

        /// <summary>
        /// File path of the test that failed (from CallerFilePath).
        /// Useful for navigation and context.
        /// Optional - can be null.
        /// </summary>
        public string? CallerFilePath { get; init; }

        // ============================================================
        // Additional Context
        // ============================================================

        /// <summary>
        /// Parameters used in the test (for parameter replacement, etc.).
        /// Can be shown in output for debugging parameterized tests.
        /// Empty array if no parameters.
        /// </summary>
#pragma warning disable CA1819
        public (string Key, object? Value)[] Parameters { get; init; } = [];
#pragma warning restore CA1819

        /// <summary>
        /// Any additional context-specific data that might be useful.
        /// This is an extensibility point for custom output formatters.
        /// Optional - can be null.
        /// </summary>
        public object? AdditionalData { get; init; }
    }
}