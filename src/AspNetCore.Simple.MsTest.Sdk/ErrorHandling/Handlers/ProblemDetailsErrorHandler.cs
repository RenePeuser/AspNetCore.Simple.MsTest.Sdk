using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    /// <summary>
    ///     Extension method for registering the ProblemDetails error handler.
    /// </summary>
    internal static class AddProblemDetailsErrorHandlerExtension
    {
        public static void AddProblemDetailsErrorHandler(this IServiceCollection services)
        {
            services.AddProblemDetailsOutputBuilder();
            services.AddSingletonIfNotExists<ITestErrorHandler, ProblemDetailsErrorHandler>();
        }
    }

    /// <summary>
    ///     Handles TestSdkProblemDetailsException by formatting it using the ProblemDetailsOutputBuilder.
    ///     This exception occurs when the API returns a ProblemDetails response that doesn't match
    ///     the expected response type in the test.
    /// </summary>
    internal sealed class ProblemDetailsErrorHandler(IProblemDetailsOutputBuilder problemDetailsOutputBuilder)
        : TestErrorHandler<TestSdkProblemDetailsException>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             TestSdkProblemDetailsException exception)
        {
            // Problem details are an http response shape - a plain object assert cannot produce one,
            // so an empty answer hands the exception to the next compatible handler.
            if (context is not IHttpAssertContext httpContext)
            {
                return Task.FromResult(string.Empty);
            }

            // Delegate to the specialized ProblemDetailsOutputBuilder
            var errorOutput = problemDetailsOutputBuilder.BuildUnexpectedError(httpContext, exception);

            return Task.FromResult(errorOutput);
        }
    }
}