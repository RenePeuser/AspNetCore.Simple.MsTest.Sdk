using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling
{
    /// <summary>
    /// Provides extension methods for registering the test error handling strategy.
    /// </summary>
    internal static class AddTestErrorHandlingStrategyExtension
    {
        /// <summary>
        /// Registers all error handlers and the error handling strategy in the DI container.
        /// </summary>
        public static void AddTestErrorHandlingStrategy(this IServiceCollection services)
        {
            // Register specific error handlers (order matters - first match wins!)
            services.AddProblemDetailsErrorHandler();
            services.AddSnapshotNotFoundErrorHandler();
            services.AddInvalidSnapshotJsonErrorHandler();
            services.AddSnapshotPlaceholderLostErrorHandler();
            services.AddNonSeekableBodyErrorHandler();
            services.AddInvalidJsonErrorHandler();
            services.AddJsonSerializationErrorHandler();

            // Fallback handler - must be last!
            services.AddDefaultErrorHandler();

            // Register the strategy itself
            services.AddSingletonIfNotExists<ITestErrorHandlingStrategy, TestErrorHandlingStrategy>();
        }
    }

    /// <summary>
    /// Defines a contract for handling exceptions in test assertions.
    /// </summary>
    internal interface ITestErrorHandlingStrategy
    {
        /// <summary>
        /// Asynchronously handles the specified exception and returns a formatted error message.
        /// </summary>
        /// <param name="context">
        /// The assertion context - an <see cref="IHttpAssertContext"/> for http asserts, a plain
        /// <see cref="IObjectAssertContext"/> for the direct object route.
        /// </param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A formatted error message suitable for display in test output.</returns>
        Task<string> HandleAsync(IObjectAssertContext context,
                                 Exception exception);
    }

    /// <summary>
    /// Default implementation of <see cref="ITestErrorHandlingStrategy"/> that routes exceptions
    /// through one or more specialized error handlers.
    ///
    /// The strategy follows this flow:
    /// 1. Try to find a specific handler that can handle the exception
    /// 2. If found, use that handler to format the error
    /// 3. If no specific handler is found, use the default handler as fallback
    /// 4. Return the formatted error message for display in test output
    /// </summary>
    internal sealed class TestErrorHandlingStrategy(IEnumerable<ITestErrorHandler> testErrorHandlers) : ITestErrorHandlingStrategy
    {
        /// <summary>
        /// Handles the exception by finding the most suitable handler and returning a formatted error message.
        /// </summary>
        public async Task<string> HandleAsync(IObjectAssertContext context,
                                              Exception exception)
        {
            // 1. Find all handlers that can handle this exception type, in registration order.
            //    The default handler is registered last so it acts as a catch-all.
            var compatibleHandlers = testErrorHandlers
                                     .Where(handler => handler.CanHandle(exception))
                                     .ToList();

            // 2. Ask them in turn. An empty answer means "I have nothing useful to say about THIS
            //    context" - a handler that needs http data cannot serve a plain object assert - so the
            //    next compatible handler gets its turn instead of dropping straight to the fallback.
            foreach (var handler in compatibleHandlers)
            {
                var errorMessage = await handler.HandleAsync(context, exception).ConfigureAwait(false);

                if (errorMessage.IsNotNullOrWhiteSpace())
                {
                    return errorMessage;
                }
            }

            // 3. Final fallback - this should never happen if DefaultErrorHandler is registered correctly
            //    But we provide a simple error message just in case
            return BuildFallbackError(context, exception);
        }

        /// <summary>
        /// Builds a minimal error message when no handler can process the exception.
        /// This should never be called if the handlers are registered correctly.
        /// </summary>
        private static string BuildFallbackError(IObjectAssertContext context,
                                                 Exception exception)
        {
            var origin = context is IHttpAssertContext httpContext
                             ? $"""
                                HTTP Method    : {httpContext.HttpMethod.Method}
                                URL            : {httpContext.Url}
                                """
                             : $"""
                                Method         : {context.CallerMemberName}
                                Line           : {context.CallerLineNumber}
                                """;

            return $"""

                    ══════════════════════════════════════════════════════════════
                    ❌ UNHANDLED EXCEPTION
                    ══════════════════════════════════════════════════════════════

                    Exception Type : {exception.GetType().Name}
                    Message        : {exception.Message}

                    {origin}

                    This error should not occur - please check error handler registration.
                    ══════════════════════════════════════════════════════════════

                    """;
        }
    }
}