using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurlPrinterExtension
    {
        public static void AddCurlPrinter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ICurlPrinter, CurlPrinter>();
        }
    }

    internal interface ICurlPrinter
    {
        void PrintCurl(Assembly callingAssembly,
                       string curl);

        /// <summary>
        /// Prints the curl command using the HTTP context's properties.
        /// This is the preferred method for context-based operations.
        /// Requires the context to have curl information available.
        /// </summary>
        void PrintCurl(IHttpAssertContext context);
    }

    internal sealed class CurlPrinter(ICurlFormatter curlFormatter, ICurlBuilder curlBuilder) : ICurlPrinter
    {
        public void PrintCurl(Assembly callingAssembly,
                              string curl)
        {
            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            var curlAsString = curlFormatter.GetCurlAsFormattedString(curl);

            if (curlAsString.IsNotNullOrWhiteSpace())
            {
                HttpClientAssertExtensions.LogAction(curlAsString);
            }
        }

        public void PrintCurl(IHttpAssertContext context)
        {
            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            var curl = curlBuilder.BuildFrom(context);
            var curlAsString = curlFormatter.GetCurlAsFormattedString(curl);

            if (curlAsString.IsNotNullOrWhiteSpace())
            {
                HttpClientAssertExtensions.LogAction(curlAsString);
            }
        }
    }
}
