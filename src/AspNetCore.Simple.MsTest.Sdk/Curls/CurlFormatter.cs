using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddCurlFormatterExtension
    {
        public static void AddCurlFormatter(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately

            services.AddSingletonIfNotExists<ICurlFormatter, CurlFormatter>();
        }
    }

    internal interface ICurlFormatter
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

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("🔁 Reproduce Locally"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.Success(curl));
            stringBuilder.AppendLine();
            stringBuilder.Append(textDecorator.Dim("══════════════════════════════════════════════════════════════"));
            var curlOutput = stringBuilder.ToString();

            return curlOutput;
        }
    }
}