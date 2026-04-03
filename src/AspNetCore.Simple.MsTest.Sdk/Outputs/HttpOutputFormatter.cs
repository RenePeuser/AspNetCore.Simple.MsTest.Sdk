using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs
{
    public static class AddHttpOutputFormatterExtension
    {
        public static void AddHttpOutputFormatter(this IServiceCollection services)
        {
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

    internal sealed class HttpOutputFormatter : IHttpOutputFormatter
    {
        public string GetOutputString(string errorInfo,
                                      HttpMethod httpMethod,
                                      string url,
                                      HttpStatusCode httpStatusCode)
        {
            var consoleTable = new ConsoleTable { Options = { EnableCount = false } };

            var enumerable = new List<string>
                             {
                                 "HttpMethod",
                                 "Url",
                                 "HttpStatusCode"
                             };

            consoleTable.AddColumn(enumerable);

            consoleTable.AddRow(httpMethod.Method, url, httpStatusCode);

            var stringBuilder = new StringBuilder();
            var consoleTableResult = consoleTable.ToString();

            stringBuilder.AppendLine(errorInfo);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(consoleTableResult);

            var output = stringBuilder.ToString();

            return output;
        }
    }
}
