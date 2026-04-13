using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    /// <summary>
    /// Orchestrator that delegates to specific comparison strategies.
    /// Uses first-match pattern: finds the first strategy that can handle the comparison.
    /// </summary>
    public interface IComparisonStrategy
    {
        /// <summary>
        /// Compares expected and current objects using the appropriate specific strategy.
        /// </summary>
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="context">The assertion context</param>
        /// <returns>Comparison result</returns>
        ComparisonResult Compare<T>(ObjectAssertContext<T> context);
    }

    /// <summary>
    /// Defines a specific comparison strategy for certain types.
    /// Strategies are checked in registration order until one matches via CanCompare.
    /// </summary>
    public interface ISpecificComparisonStrategy
    {
        /// <summary>
        /// Determines if this strategy can handle the comparison for the given context.
        /// </summary>
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="context">The assertion context</param>
        /// <returns>True if this strategy can handle the comparison, false otherwise</returns>
        bool CanCompare<T>(ObjectAssertContext<T> context);

        /// <summary>
        /// Compares expected and current objects and returns differences.
        /// Only called if CanCompare returned true.
        /// </summary>
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="context">The assertion context containing all comparison parameters</param>
        /// <returns>Comparison result with differences and formatted output</returns>
        ComparisonResult Compare<T>(ObjectAssertContext<T> context);
    }

    /// <summary>
    /// Result of a comparison operation.
    /// </summary>
    public sealed record ComparisonResult
    {
        /// <summary>
        /// List of differences found during comparison.
        /// Empty if objects are equal.
        /// </summary>
        public required ImmutableList<Difference> Differences { get; init; }

        /// <summary>
        /// Formatted expected value for output (may be transformed/ordered).
        /// </summary>
        public required string FormattedExpected { get; init; }

        /// <summary>
        /// Formatted current value for output (may be transformed/ordered).
        /// </summary>
        public required string FormattedCurrent { get; init; }

        /// <summary>
        /// Indicates if there is a schema mismatch (structural difference).
        /// </summary>
        public required bool HasSchemaMismatch { get; init; }
    }
}
