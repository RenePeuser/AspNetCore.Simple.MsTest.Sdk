using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent interface for configuring response validation and transformation.
    /// This state is reached after calling WithResponse on IHttpRequestConfiguring.
    /// </summary>
    /// <typeparam name="TResult">The expected response type</typeparam>
    public interface IHttpResponseConfiguring<TResult>
    {
        /// <summary>
        /// Applies a transformation to the response before comparison.
        /// Useful for sorting, filtering, or normalizing data.
        /// </summary>
        /// <param name="filter">Function to transform the response</param>
        /// <returns>Configuration builder for further setup</returns>
        IHttpResponseConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter);

        /// <summary>
        /// Filters which differences should be considered as assertion failures.
        /// Useful for ignoring specific fields like timestamps or IDs.
        /// </summary>
        /// <param name="filter">Function to filter differences</param>
        /// <returns>Configuration builder for further setup</returns>
        IHttpResponseConfiguring<TResult> IgnoreDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter);

        /// <summary>
        /// Type-safe way to ignore a specific property in the response comparison.
        /// </summary>
        /// <typeparam name="T">Type containing the property</typeparam>
        /// <param name="propertySelector">Expression selecting the property to ignore</param>
        /// <returns>Configuration builder for further setup</returns>
        IHttpResponseConfiguring<TResult> IgnoreProperty<T>(Expression<Func<T, object?>> propertySelector);

        /// <summary>
        /// Configures parameters for placeholder substitution in the expected response JSON.
        /// </summary>
        /// <param name="parameters">Array of key-value pairs for parameter substitution</param>
        /// <returns>Configuration builder for further setup</returns>
        IHttpResponseConfiguring<TResult> WithParameters(params (string Key, object? Value)[] parameters);

        /// <summary>
        /// Enables writing the actual response to disk as a snapshot file.
        /// Useful for updating test expectations.
        /// </summary>
        /// <param name="write">Whether to write snapshot</param>
        /// <returns>Configuration builder for further setup</returns>
        IHttpResponseConfiguring<TResult> WriteSnapshot(bool write = true);

        /// <summary>
        /// Expects any successful HTTP status code (2xx range) and validates response body.
        /// This is a terminal operation.
        /// </summary>
        /// <returns>Task that resolves to the validated response</returns>
        Task<TResult> ExpectSuccess();

        /// <summary>
        /// Expects one of the specified HTTP status codes and validates response body.
        /// This is a terminal operation.
        /// </summary>
        /// <param name="codes">Accepted HTTP status codes</param>
        /// <returns>Task that resolves to the validated response</returns>
        Task<TResult> Expect(params HttpStatusCode[] codes);

        /// <summary>
        /// Expects a specific status code and validates response body.
        /// This is a terminal operation.
        /// </summary>
        /// <param name="code">Expected HTTP status code</param>
        /// <returns>Task that resolves to the validated response</returns>
        Task<TResult> ExpectStatus(HttpStatusCode code);

        /// <summary>
        /// Expects an error status code (4xx or 5xx) and validates response body.
        /// This is a terminal operation.
        /// </summary>
        /// <param name="code">Expected error status code</param>
        /// <returns>Task that resolves to the validated response</returns>
        Task<TResult> ExpectError(HttpStatusCode code);
    }
}