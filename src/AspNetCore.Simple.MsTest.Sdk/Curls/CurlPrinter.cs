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
    }

    internal sealed class CurlPrinter(ICurlFormatter curlFormatter) : ICurlPrinter
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
    }
}
