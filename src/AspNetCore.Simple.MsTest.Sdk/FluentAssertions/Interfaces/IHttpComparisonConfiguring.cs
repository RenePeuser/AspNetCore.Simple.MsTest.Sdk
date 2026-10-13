using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
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
        /// <summary>
        /// Transforms the deserialized response before comparison (sort/normalize). Several calls combine in
        /// call order: the second filter receives the output of the first.
        /// </summary>
        IHttpComparisonConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter);

        /// <summary>
        /// Ignores every difference for which <paramref name="ignore"/> returns <see langword="true"/>
        /// (e.g. <c>d => d.MemberPath.EndsWith("timestamp")</c>). Several calls combine: a difference is
        /// ignored as soon as ONE predicate ignores it.
        /// </summary>
        IHttpComparisonConfiguring<TResult> IgnoreDifferencesWhere(Predicate<Difference> ignore);

        /// <summary>
        /// Ignores the given member paths and everything below them. Paths use the difference format —
        /// <c>id</c>, <c>[0].id</c>, <c>emails[0].emailAddress</c> — with <c>[*]</c> for any index
        /// (<c>"[*].id"</c>, <c>"emails[*].emailAddress"</c>). Property names compare case-insensitively.
        /// </summary>
        IHttpComparisonConfiguring<TResult> IgnorePaths(params string[] paths);

        /// <summary>
        /// Ignores exactly the property <paramref name="propertySelector"/> addresses — at that position,
        /// not every property with the same name. Nested access, indices and <c>Select</c> for "every
        /// element" are supported: <c>p => p.Address.Street</c>, <c>p => p.Emails[0].EmailAddress</c>,
        /// <c>p => p.Emails.Select(e => e.EmailAddress)</c>, <c>list => list.Select(p => p.Id)</c>.
        /// JSON names honor <c>[JsonPropertyName]</c> and the configured naming policy.
        /// </summary>
        IHttpComparisonConfiguring<TResult> IgnoreProperty(Expression<Func<TResult, object?>> propertySelector);

        /// <summary>
        /// Low-level escape hatch: rewrites the whole difference list. The function returns the differences
        /// that still COUNT — what it drops is ignored. Mirrors the classic <c>differenceFunc</c> parameter.
        /// Prefer <see cref="IgnoreDifferencesWhere"/>, <see cref="IgnorePaths"/> or <see cref="IgnoreProperty"/>.
        /// </summary>
        IHttpComparisonConfiguring<TResult> FilterDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter);

        /// <summary>Enables writing the actual response to disk as a snapshot (update expectations).</summary>
        IHttpComparisonConfiguring<TResult> WriteSnapshot(bool write = true);

        /// <summary>Executes the request, runs the body comparison, and returns the real response. The single terminal.</summary>
        /// <param name="cancellationToken">Cancels the HTTP call; a cancellation surfaces as <see cref="System.OperationCanceledException"/>, not as an assertion failure.</param>
        Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}