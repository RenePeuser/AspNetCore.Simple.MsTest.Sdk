using System.Text;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs
{
    internal sealed class OutputFormatter(CurlFormatter curlFormatter)
    {
        internal string GetOutputString(string title,
                                        string? expectedResultAsJson,
                                        string? currentResultAsJson)
        {
            return GetOutputString(title, string.Empty, expectedResultAsJson,
                                   currentResultAsJson);
        }

        internal string GetOutputString(string title,
                                        string errorInfo,
                                        string? expectedResultAsJson,
                                        string? currentResultAsJson)
        {
            return GetOutputString(title, errorInfo, expectedResultAsJson,
                                   currentResultAsJson, string.Empty);
        }

        internal string GetOutputString(string title,
                                        string errorInfo,
                                        string? expectedResultAsJson,
                                        string? currentResultAsJson,
                                        string curl)
        {
            return GetOutputString(title, errorInfo, expectedResultAsJson,
                                   currentResultAsJson, string.Empty, curl);
        }

        internal string GetOutputString(string title,
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
