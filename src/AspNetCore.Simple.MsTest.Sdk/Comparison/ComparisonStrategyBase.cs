using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    /// <summary>
    /// Abstract base class for type-specific comparison strategies.
    /// Reduces duplication by handling type checks, casting, and guard clauses.
    /// Concrete strategies only need to implement CompareTyped with their specific logic.
    ///
    /// Use this base class for strategies that handle a single specific type (e.g., string, DateTime).
    /// Do NOT use this for fallback strategies that handle multiple types (e.g., JsonComparisonStrategy).
    /// </summary>
    /// <typeparam name="T">The specific type this strategy can compare</typeparam>
    internal abstract class ComparisonStrategyBase<T> : ISpecificComparisonStrategy
    {
        public bool CanCompare<TContext>(ObjectAssertContext<TContext> context)
        {
            return typeof(TContext) == typeof(T);
        }

        public ComparisonResult Compare<TContext>(ObjectAssertContext<TContext> context)
        {
            // Defensive check - should only be called if CanCompare returned true
            if (CanCompare(context).IsFalse())
            {
                throw new InvalidOperationException($"{GetType().Name} can only compare {typeof(T).Name}, but was asked to compare {typeof(TContext).Name}");
            }

            // Cast to strongly-typed context
            var typedContext = context as ObjectAssertContext<T>;

            if (typedContext.IsNull())
            {
                throw new InvalidOperationException($"Failed to cast context to ObjectAssertContext<{typeof(T).Name}>");
            }

            // Delegate to concrete strategy implementation
            return CompareTyped(typedContext);
        }

        /// <summary>
        /// Performs the actual comparison for the specific type.
        /// Concrete strategies implement this method with their comparison logic.
        /// </summary>
        /// <param name="context">The strongly-typed assertion context</param>
        /// <returns>Comparison result</returns>
        protected abstract ComparisonResult CompareTyped(ObjectAssertContext<T> context);
    }
}