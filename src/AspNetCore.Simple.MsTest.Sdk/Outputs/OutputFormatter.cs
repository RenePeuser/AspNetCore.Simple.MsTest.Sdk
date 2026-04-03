using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddOutputFormatterExtension
    {
        public static void AddOutputFormatter(this IServiceCollection services)
        {
            // Register dependencies
            services.AddCurlFormatter();
            services.AddHttpSpecificOutputFormatter();
            services.AddObjectSpecificOutputFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<IOutputFormatter, OutputFormatter>();
        }
    }

    public interface IOutputFormatter
    {
        /// <summary>
        /// Formats output using the appropriate strategy based on the context.
        /// This is the new preferred method that uses the strategy pattern.
        /// </summary>
        string Format(OutputContext context);

        // Legacy overloads for backward compatibility
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

    /// <summary>
    /// Strategy orchestrator that selects and delegates to the appropriate specific formatter.
    /// Uses the Chain of Responsibility pattern to find the first formatter that can handle the context.
    /// </summary>
    internal sealed class OutputFormatter(IEnumerable<ISpecificOutputFormatter> specificFormatters,
                                          ICurlFormatter curlFormatter) : IOutputFormatter
    {
        public string Format(OutputContext context)
        {
            // Strategy pattern: Find the first formatter that can handle this context
            // HttpSpecificOutputFormatter will match HTTP contexts
            // ObjectSpecificOutputFormatter is the fallback for pure object comparisons
            var formatter = specificFormatters.FirstOrDefault(f => f.CanFormat(context));

            if (formatter is not null)
            {
                return formatter.Format(context);
            }

            // Fallback to legacy formatting if no strategy matched (should not happen)
            return GetOutputString(context.Title ?? string.Empty,
                                   context.ErrorInfo ?? string.Empty,
                                   context.ExpectedResultAsJson,
                                   context.CurrentResultAsJson,
                                   context.DifferenceTableFormatted ?? string.Empty,
                                   context.Curl ?? string.Empty);
        }

        // Legacy overloads for backward compatibility
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
