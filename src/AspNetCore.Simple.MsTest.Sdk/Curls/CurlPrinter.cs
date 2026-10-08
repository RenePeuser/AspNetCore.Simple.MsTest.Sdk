using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddCurlPrinterExtension
    {
        public static void AddCurlPrinter(this IServiceCollection services)
        {
            services.AddTestSdkSettings();
            services.AddSingletonIfNotExists<ICurlPrinter, CurlPrinter>();
        }
    }

    internal interface ICurlPrinter
    {
        /// <summary>
        /// Prints the curl command from HTTP response context.
        /// Only prints in DEBUG mode.
        /// </summary>
        void PrintCurl<TResult>(AssertableHttpClient.HttpResponseContext<TResult> context);
    }

    internal sealed class CurlPrinter(ICurlFormatter curlFormatter,
                                      ICurlBuilder curlBuilder,
                                      TestSdkSettings testSdkSettings) : ICurlPrinter
    {
        public void PrintCurl<TResult>(AssertableHttpClient.HttpResponseContext<TResult> context)
        {
            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            var curl = curlBuilder.BuildFrom(context);
            var curlAsString = curlFormatter.GetCurlAsFormattedString(curl);

            if (curlAsString.IsNotNullOrWhiteSpace())
            {
                testSdkSettings.LogAction(curlAsString);
            }
        }
    }
}