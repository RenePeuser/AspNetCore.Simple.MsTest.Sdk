using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs
{
    public static class AddOutputFormatterExtension
    {
        public static void AddOutputFormatter(this IServiceCollection services)
        {
            // Register dependency
            services.AddCurlFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<IOutputFormatter, OutputFormatter>();
        }
    }

    public interface IOutputFormatter
    {
        string GetOutputString(string title,
                               string? expectedResultAsJson,
                               string? currentResultAsJson);

        string GetOutputString(string title,
                               string errorInfo,
                               string? expectedResultAsJson,
                               string? currentResultAsJson);

        string GetOutputString(string title,
                               string errorInfo,
                               string? expectedResultAsJson,
                               string? currentResultAsJson,
                               string curl);

        string GetOutputString(string title,
                               string errorInfo,
                               string? expectedResultAsJson,
                               string? currentResultAsJson,
                               string objectDifferences,
                               string curl);
    }

    internal sealed class OutputFormatter(ICurlFormatter curlFormatter) : IOutputFormatter
    {
        public string GetOutputString(string title,
                                      string? expectedResultAsJson,
                                      string? currentResultAsJson)
        {
            return GetOutputString(title, string.Empty, expectedResultAsJson,
                                   currentResultAsJson);
        }

        public string GetOutputString(string title,
                                      string errorInfo,
                                      string? expectedResultAsJson,
                                      string? currentResultAsJson)
        {
            return GetOutputString(title, errorInfo, expectedResultAsJson,
                                   currentResultAsJson, string.Empty);
        }

        public string GetOutputString(string title,
                                      string errorInfo,
                                      string? expectedResultAsJson,
                                      string? currentResultAsJson,
                                      string curl)
        {
            return GetOutputString(title, errorInfo, expectedResultAsJson,
                                   currentResultAsJson, string.Empty, curl);
        }

        public string GetOutputString(string title,
                                      string errorInfo,
                                      string? expectedResultAsJson,
                                      string? currentResultAsJson,
                                      string objectDifferences,
                                      string curl)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            if (title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            if (errorInfo.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(errorInfo);
                stringBuilder.AppendLine();
            }

            // Result table is optional, if we can compare the results
            if (objectDifferences.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(objectDifferences);
            }

            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(expectedResultAsJson);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(currentResultAsJson);
            stringBuilder.AppendLine();

            if (curl.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(curlFormatter.GetCurlAsFormattedString(curl));
                stringBuilder.AppendLine();
            }

            var output = stringBuilder.ToString();

            return output;
        }
    }
}
