using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    /// <summary>
    /// Strategy for building assertion failure output based on context type.
    /// Different context types (Object, HTTP, HttpResponse) produce different output formats.
    /// </summary>
    public interface IAssertOutputStrategy
    {
        /// <summary>
        /// Determines whether this strategy can handle the given context type.
        /// </summary>
        bool CanHandle(IObjectAssertContext context);

        /// <summary>
        /// Builds formatted assertion failure output.
        /// The strategy internally decides whether to output schema mismatch or value differences
        /// based on the differences list.
        /// </summary>
        /// <param name="context">The assertion context with all metadata</param>
        /// <param name="differences">List of differences found (empty if no structural comparison)</param>
        /// <param name="expectedJson">Expected JSON string</param>
        /// <param name="currentJson">Current/actual JSON string</param>
        /// <returns>Formatted error message for Assert.That.Fail()</returns>
        string BuildOutput(IObjectAssertContext context,
                           ImmutableList<Difference> differences,
                           string expectedJson,
                           string currentJson);
    }

    /// <summary>
    /// Generic base class for assertion output strategies.
    /// Provides type-safe context access by handling the casting from IObjectAssertContext
    /// to the specific context type TContext.
    /// Follows the Strategy Pattern with generic type constraint for compile-time safety.
    /// </summary>
    /// <typeparam name="TContext">The specific context type this strategy handles</typeparam>
    internal abstract class AssertOutputStrategyBase<TContext> : IAssertOutputStrategy
        where TContext : IObjectAssertContext
    {
        /// <summary>
        /// Type-checks if the context matches TContext.
        /// </summary>
        public bool CanHandle(IObjectAssertContext context)
        {
            return context is TContext;
        }

        /// <summary>
        /// Casts context to TContext and delegates to the type-safe abstract method.
        /// </summary>
        public string BuildOutput(IObjectAssertContext context,
                                  ImmutableList<Difference> differences,
                                  string expectedJson,
                                  string currentJson)
        {
            var typedContext = (TContext)context;

            return BuildOutput(typedContext, differences, expectedJson,
                               currentJson);
        }

        /// <summary>
        /// Abstract method that subclasses implement with type-safe context access.
        /// </summary>
        protected abstract string BuildOutput(TContext context,
                                              ImmutableList<Difference> differences,
                                              string expectedJson,
                                              string currentJson);
    }
}