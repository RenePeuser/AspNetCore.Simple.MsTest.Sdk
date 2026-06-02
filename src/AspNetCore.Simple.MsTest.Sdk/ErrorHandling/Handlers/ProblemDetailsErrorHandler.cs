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
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             TestSdkProblemDetailsException exception)
        {
            // Delegate to the specialized ProblemDetailsOutputBuilder
            var errorOutput = problemDetailsOutputBuilder.BuildUnexpectedError(context, exception);

            return Task.FromResult(errorOutput);
        }
    }
}