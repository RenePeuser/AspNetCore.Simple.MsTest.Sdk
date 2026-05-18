using AspNetCore.Simple.MsTest.Sdk.Validation;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling
{
    /// <summary>
    /// Defines a contract for handling specific exception types in test assertions.
    /// </summary>
    public interface ITestErrorHandler
    {
        /// <summary>
        /// Determines whether this handler can process the specified exception.
        /// </summary>
        /// <param name="exception">The exception to check.</param>
        /// <returns>True if this handler can process the exception; otherwise, false.</returns>
        bool CanHandle(Exception exception);

        /// <summary>
        /// Asynchronously handles the specified exception and returns a formatted error message.
        /// </summary>
        /// <param name="context">The HTTP assertion context containing request/response information.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A formatted error message suitable for display in test output.</returns>
        Task<string> HandleAsync(IHttpAssertContext context,
                                 Exception exception);
    }

    /// <summary>
    /// Base class for implementing type-safe exception handlers.
    /// Provides automatic type checking and delegation to derived classes.
    /// </summary>
    /// <typeparam name="TException">The specific exception type this handler processes.</typeparam>
    public abstract class TestErrorHandler<TException> : ITestErrorHandler where TException : Exception
    {
        /// <summary>
        /// Determines whether this handler can process the specified exception.
        /// </summary>
        public bool CanHandle(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            // Base type check
            var baseCheck = typeof(TException).IsAssignableFrom(exception.GetType());
            if (!baseCheck)
            {
                return baseCheck;
            }

            // Allow derived classes to add additional checks
            var specificCheck = CanHandle((TException)exception);
            return specificCheck;
        }

        /// <summary>
        /// Override this method to add additional checks beyond type matching.
        /// Default implementation accepts all exceptions of the specified type.
        /// </summary>
        /// <param name="exception">The exception to check.</param>
        /// <returns>True if this handler should process the exception; otherwise, false.</returns>
        protected virtual bool CanHandle(TException exception)
        {
            return true;
        }

        /// <summary>
        /// Handles the exception by delegating to the type-safe implementation.
        /// </summary>
        public async Task<string> HandleAsync(IHttpAssertContext context,
                                              Exception exception)
        {
            // Safety first - only handle if we can
            if (CanHandle(exception))
            {
                return await HandleExceptionAsync(context, (TException)exception).ConfigureAwait(false);
            }

            // Return empty string if we can't handle - let another handler try
            return string.Empty;
        }

        /// <summary>
        /// Override this method to implement the actual exception handling logic.
        /// </summary>
        /// <param name="context">The HTTP assertion context containing request/response information.</param>
        /// <param name="exception">The exception to handle (already cast to the correct type).</param>
        /// <returns>A formatted error message for display in test output.</returns>
        protected abstract Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             TException exception);
    }
}
