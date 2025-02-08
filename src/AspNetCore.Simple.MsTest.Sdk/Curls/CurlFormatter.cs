using System;
using System.Linq;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurlFormatterExtension
    {
        public static void AddCurlFormatter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ICurlFormatter, CurlFormatter>();
        }
    }

    public interface ICurlFormatter
    {
        string GetCurlAsFormattedString(string curl);
    }

    internal sealed class CurlFormatter : ICurlFormatter
    {
        public string GetCurlAsFormattedString(string curl)
        {
            if (!curl.IsNotNullOrWhiteSpace())
            {
                return string.Empty;
            }

            var maxLength = curl.Split(Environment.NewLine).Max(line => line.Length);
            var separator = maxLength.Times(() => "-").Flatten();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(separator);
            stringBuilder.AppendLine("Http call as curl");
            stringBuilder.AppendLine(separator);
            stringBuilder.AppendLine(curl);
            stringBuilder.AppendLine(separator);
            var curlOutput = stringBuilder.ToString();

            return curlOutput;
        }
    }
}
