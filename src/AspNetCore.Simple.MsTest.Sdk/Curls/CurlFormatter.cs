using System;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurlFormatterExtension
    {
        public static void AddCurlFormatter(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately
            services.AddSingletonIfNotExists<ICurlFormatter, CurlFormatter>();
        }
    }

    public interface ICurlFormatter
    {
        string GetCurlAsFormattedString(string curl);
    }

    internal sealed class CurlFormatter(ITextDecorator textDecorator) : ICurlFormatter
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
            stringBuilder.AppendLine(textDecorator.Dim(separator));
            stringBuilder.AppendLine(textDecorator.SectionTitle("Http call as curl"));
            stringBuilder.AppendLine(textDecorator.Dim(separator));
            stringBuilder.AppendLine(textDecorator.Success(curl));
            stringBuilder.AppendLine(textDecorator.Dim(separator));
            var curlOutput = stringBuilder.ToString();

            return curlOutput;
        }
    }
}
