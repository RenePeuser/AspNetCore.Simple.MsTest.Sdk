using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using ConsoleTables;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs
{
    internal sealed class HttpOutputFormatter()
    {
        public string GetOutputString(string errorInfo,
                                      HttpMethod httpMethod,
                                      string url,
                                      HttpStatusCode httpStatusCode)
        {
            var consoleTable = new ConsoleTable() { Options = { EnableCount = false } };

            var enumerable = new List<string>()
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
            
            var output =  stringBuilder.ToString();

            return output;
        }
    }
}
