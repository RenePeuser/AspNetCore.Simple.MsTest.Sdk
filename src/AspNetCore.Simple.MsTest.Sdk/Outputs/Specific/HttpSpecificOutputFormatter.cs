using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpSpecificOutputFormatterExtension
    {
        public static void AddHttpSpecificOutputFormatter(this IServiceCollection services)
        {
            // Register dependency
            services.AddCurlFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<ISpecificOutputFormatter, HttpSpecificOutputFormatter>();
        }
    }

    /// <summary>
    /// Formats output for HTTP assertion contexts.
    /// Includes HTTP-specific information like Method, URL, StatusCode, and Curl.
    /// </summary>
    internal sealed class HttpSpecificOutputFormatter(ICurlFormatter curlFormatter) : ISpecificOutputFormatter
    {
        public bool CanFormat(OutputContext context)
        {
            // Can format if context contains HTTP-specific data
            return context.HttpMethod is not null || context.Url is not null || context.Curl is not null;
        }

        public string Format(OutputContext context)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            // HTTP Call Information Section
            if (context.HttpMethod is not null || context.Url is not null || context.HttpStatusCode is not null)
            {
                stringBuilder.AppendLine("=== HTTP Call Information ===");

                if (context.HttpMethod is not null)
                {
                    stringBuilder.AppendLine($"Method: {context.HttpMethod}");
                }

                if (context.Url.IsNotNullOrWhiteSpace())
                {
                    stringBuilder.AppendLine($"URL: {context.Url}");
                }

                if (context.HttpStatusCode is not null)
                {
                    stringBuilder.AppendLine($"Status Code: {(int)context.HttpStatusCode} {context.HttpStatusCode}");
                }

                stringBuilder.AppendLine();
            }

            // Title Section
            if (context.Title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(context.Title);
                stringBuilder.AppendLine();
            }

            // Error Information Section
            if (context.ErrorInfo.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine("=== Error ===");
                stringBuilder.AppendLine(context.ErrorInfo);
                stringBuilder.AppendLine();
            }

            // Differences Table Section
            if (context.DifferenceTableFormatted.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine("=== Differences ===");
                stringBuilder.AppendLine(context.DifferenceTableFormatted);
                stringBuilder.AppendLine();
            }
            else if (context.Differences.Count > 0)
            {
                stringBuilder.AppendLine($"=== {context.Differences.Count} Difference(s) Found ===");
                stringBuilder.AppendLine();
            }

            // Expected vs Current Section
            stringBuilder.AppendLine($"=== Expected ({context.ExpectedResultParameterName ?? "Expected"}) ===");
            stringBuilder.AppendLine(context.ExpectedResultAsJson ?? "null");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"=== Current ({context.CurrentResultParameterName ?? "Actual"}) ===");
            stringBuilder.AppendLine(context.CurrentResultAsJson ?? "null");
            stringBuilder.AppendLine();

            // Curl Reproduction Section
            if (context.Curl.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine("=== Reproduce with Curl ===");
                stringBuilder.AppendLine(curlFormatter.GetCurlAsFormattedString(context.Curl));
                stringBuilder.AppendLine();
            }

            // Parameters Section (if any)
            if (context.Parameters?.Length > 0)
            {
                stringBuilder.AppendLine("=== Parameters ===");

                foreach (var (key, value) in context.Parameters)
                {
                    stringBuilder.AppendLine($"{key} = {value}");
                }

                stringBuilder.AppendLine();
            }

            // Caller Info Section
            if (context.CallerFilePath.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine($"Test Location: {context.CallerFilePath}");
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
