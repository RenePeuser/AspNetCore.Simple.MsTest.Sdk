using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddSuccessfulTestCurlPrinterExtension
    {
        /// <summary>
        /// Registers the curl printer step and its dependencies.
        /// </summary>
        public static void AddSuccessfulTestCurlPrinter(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddCurlBuilder();
            services.AddCurlFormatter();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, SuccessfulTestCurlPrinter>();
        }
    }

    /// <summary>
    /// Prints the curl command for successful tests to the test output.
    /// This step runs last in the pipeline after all validations pass.
    /// Prints curl when the test expectations are met (IsExpectedStatusCode is true).
    /// Works for both success tests (2xx) and error tests (4xx/5xx) - if the test passes, curl appears.
    /// Provides curl commands for easy reproduction of HTTP calls.
    /// </summary>
    internal sealed class SuccessfulTestCurlPrinter(ICurlBuilder curlBuilder,
                                                    ICurlFormatter curlFormatter) : IHttpAssertionStep
    {
        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // Build curl command from response context (implements IHttpResponseContext)
            var curl = curlBuilder.BuildFrom(context);
            var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);

            // Write curl command to test output
            // Use both Console.Out (for captured output) and Trace (for test frameworks)
            if (curlFormatted.IsNotNullOrWhiteSpace())
            {
                //// Write to Console.Out explicitly (captured by test runners)
                //Console.Out.WriteLine(curlFormatted);
                //Console.Out.Flush();

                //// Also write via Trace for test frameworks that capture trace output
                //System.Diagnostics.Trace.WriteLine(curlFormatted);

                // Custom log action
                HttpClientAssertExtensions.LogAction.Invoke(curlFormatted);
            }
        }
    }
}
