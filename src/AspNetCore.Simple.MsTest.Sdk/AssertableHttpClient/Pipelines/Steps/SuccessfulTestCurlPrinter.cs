using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    internal static class AddSuccessfulTestCurlPrinterExtension
    {
        /// <summary>
        /// Registers the curl printer step and its dependencies.
        /// </summary>
        public static void AddSuccessfulTestCurlPrinter(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddCurlPrinter(); // ICurlPrinter handles debug mode check

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, SuccessfulTestCurlPrinter>();
        }
    }

    /// <summary>
    /// Prints the curl command for successful tests to the test output.
    /// This step runs last in the pipeline after all validations pass.
    /// Prints curl when the test expectations are met (IsExpectedStatusCode is true).
    /// Works for both success tests (2xx) and error tests (4xx/5xx) - if the test passes, curl appears.
    /// Only prints in DEBUG mode to avoid polluting CI pipeline logs.
    /// Provides curl commands for easy reproduction of HTTP calls during development.
    /// </summary>
    internal sealed class SuccessfulTestCurlPrinter(ICurlPrinter curlPrinter) : IHttpAssertionStep
    {
        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // Only prints in DEBUG mode (checks CallingAssembly.IsCompiledInDebug())
            curlPrinter.PrintCurl(context);
        }
    }
}