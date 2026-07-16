using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent state for configuring the body comparison. Reached ONLY after an <c>ExpectedResponse…</c>
    /// on <see cref="IHttpResponseConfiguring{TResult}"/> — the type-state guarantee that you cannot
    /// configure a comparison that does not exist (§4/§15.6).
    /// </summary>
    /// <typeparam name="TResult">The expected response type.</typeparam>
    [FluentBuilder]
    public interface IHttpComparisonConfiguring<TResult>
    {
        /// <summary>Transforms the deserialized response before comparison (sort/normalize).</summary>
        IHttpComparisonConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter);

        /// <summary>Filters which differences count as failures (e.g. ignore timestamps).</summary>
        IHttpComparisonConfiguring<TResult> IgnoreDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter);

        /// <summary>
        /// Per-difference predicate: a difference counts as a failure ONLY when <paramref name="filter"/>
        /// returns <see langword="true"/>. Complements <see cref="IgnoreDifferences"/> (which rewrites the
        /// whole list at once) with a simple per-item keep/drop test, and mirrors the native
        /// <c>differenceFilter</c> parameter so migration stays mechanical.
        /// </summary>
        IHttpComparisonConfiguring<TResult> DifferenceFilter(Predicate<Difference> filter);

        /// <summary>Type-safe way to ignore a property in the comparison (hard skip).</summary>
        IHttpComparisonConfiguring<TResult> IgnoreProperty<T>(Expression<Func<T, object?>> propertySelector);

        /// <summary>Enables writing the actual response to disk as a snapshot (update expectations).</summary>
        IHttpComparisonConfiguring<TResult> WriteSnapshot(bool write = true);

        /// <summary>Executes the request, runs the body comparison, and returns the real response. The single terminal.</summary>
        Task<TResult> ExecuteAsync();
    }
}
