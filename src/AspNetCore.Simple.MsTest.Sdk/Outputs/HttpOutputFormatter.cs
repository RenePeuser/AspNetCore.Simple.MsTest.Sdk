using System.Net;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpOutputFormatterExtension
    {
        public static void AddHttpOutputFormatter(this IServiceCollection services)
        {
            services.AddTableBuilder();
            services.AddSingletonIfNotExists<IHttpOutputFormatter, HttpOutputFormatter>();
        }
    }

    public interface IHttpOutputFormatter
    {
        string GetOutputString(string errorInfo,
                               HttpMethod httpMethod,
                               string url,
                               HttpStatusCode httpStatusCode);
    }

    internal sealed class HttpOutputFormatter(ITableBuilder tableBuilder) : IHttpOutputFormatter
    {
        public string GetOutputString(string errorInfo,
                                      HttpMethod httpMethod,
                                      string url,
                                      HttpStatusCode httpStatusCode)
        {
            var columns = new[] { "HttpMethod", "Url", "HttpStatusCode" };
            var rows = new List<object[]> { new object[] { httpMethod.Method, url, httpStatusCode } };

            var table = tableBuilder.BuildTable(columns, rows, enableCount: false);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(errorInfo);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(table);

            return stringBuilder.ToString();
        }
    }
}