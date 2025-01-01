using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ConsoleTables;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs
{
    internal sealed class HttpOutputFormatter()
    {
        public string GetOutputString(string errorInfo,
                                      HttpMethod httpMethod,
                                      string url)
        {
            var consoleTable = new ConsoleTable() { Options = { EnableCount = false } };

            var enumerable = new List<string>()
                             {
                                 "HttpMethod",
                                 "Url"
                             };
            
            consoleTable.AddColumn(enumerable);

            consoleTable.AddRow(httpMethod.Method, url);

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
