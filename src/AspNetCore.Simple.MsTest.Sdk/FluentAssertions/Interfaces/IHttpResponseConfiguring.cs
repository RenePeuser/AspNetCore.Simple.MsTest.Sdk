using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent state for configuring response validation. Reached after <c>Returns…&lt;T&gt;()</c>.
    ///
    /// <para>
    /// MODEL B: every <c>Expecting…</c> method is COMPOSABLE CONFIG that returns the builder — you can
    /// stack several expectations. The chain is executed by exactly one terminal, <see cref="ExecuteAsync"/>.
    /// The builder is deliberately NOT awaitable (no GetAwaiter): a chain that forgets
    /// <c>ExecuteAsync()</c> is a dangling <see cref="FluentBuilderAttribute"/> expression → MSTESTSDK001.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The expected response type.</typeparam>
    [FluentBuilder]
    public interface IHttpResponseConfiguring<TResult>
    {
        // ============================================================
        // Response transformation / difference configuration.
        // ============================================================

        /// <summary>Transforms the deserialized response before comparison (sort/normalize).</summary>
        IHttpResponseConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter);

        /// <summary>Filters which differences count as failures (e.g. ignore timestamps).</summary>
        IHttpResponseConfiguring<TResult> IgnoreDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter);

        /// <summary>Type-safe way to ignore a property in the comparison (hard skip).</summary>
        IHttpResponseConfiguring<TResult> IgnoreProperty<T>(Expression<Func<T, object?>> propertySelector);

        /// <summary>Configures placeholder parameters ($Token$) substituted in the expected JSON.</summary>
        IHttpResponseConfiguring<TResult> WithParameters(params (string Key, object? Value)[] parameters);

        /// <summary>Enables writing the actual response to disk as a snapshot (update expectations).</summary>
        IHttpResponseConfiguring<TResult> WriteSnapshot(bool write = true);

        // ============================================================
        // Expectations — COMPOSABLE CONFIG (return the builder, NOT a Task).
        // ============================================================

        /// <summary>Expects any 2xx success status.</summary>
        IHttpResponseConfiguring<TResult> ExpectingSuccess();

        /// <summary>Expects exactly this status code.</summary>
        IHttpResponseConfiguring<TResult> ExpectingStatus(HttpStatusCode code);

        /// <summary>Expects one of the given status codes.</summary>
        IHttpResponseConfiguring<TResult> ExpectingOneOf(params HttpStatusCode[] codes);

        /// <summary>Expects an error status code (4xx/5xx).</summary>
        IHttpResponseConfiguring<TResult> ExpectingError(HttpStatusCode code);

        // ============================================================
        // THE one terminal — the only awaitable, the only Task-returning member.
        // ============================================================

        /// <summary>Executes the request and runs all configured expectations. The single terminal.</summary>
        Task<TResult> ExecuteAsync();
    }
}
