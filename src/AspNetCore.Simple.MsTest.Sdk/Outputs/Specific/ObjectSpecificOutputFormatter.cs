using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddObjectSpecificOutputFormatterExtension
    {
        public static void AddObjectSpecificOutputFormatter(this IServiceCollection services)
        {
            // Register service itself
            services.AddSingletonIfNotExists<ISpecificOutputFormatter, ObjectSpecificOutputFormatter>();
        }
    }

    /// <summary>
    /// Formats output for pure object comparison assertions.
    /// Focuses on object differences without HTTP-specific information.
    /// </summary>
    internal sealed class ObjectSpecificOutputFormatter : ISpecificOutputFormatter
    {
        public bool CanFormat(OutputContext context)
        {
            // Fallback formatter - can always format if no HTTP-specific formatter matched
            // This is the default formatter for pure object comparisons
            return true;
        }

        public string Format(OutputContext context)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            // Title Section
            if (context.Title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine($"=== {context.Title} ===");
                stringBuilder.AppendLine();
            }

            // Error Information Section
            if (context.ErrorInfo.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine("Error:");
                stringBuilder.AppendLine(context.ErrorInfo);
                stringBuilder.AppendLine();
            }

            // Differences Section
            if (context.DifferenceTableFormatted.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine("Differences:");
                stringBuilder.AppendLine(context.DifferenceTableFormatted);
                stringBuilder.AppendLine();
            }
            else if (context.Differences.Count > 0)
            {
                stringBuilder.AppendLine($"Found {context.Differences.Count} difference(s)");
                stringBuilder.AppendLine();
            }

            // Expected Result Section
            var expectedLabel = context.ExpectedResultParameterName.IsNotNullOrWhiteSpace()
                                    ? context.ExpectedResultParameterName
                                    : "Expected";

            stringBuilder.AppendLine($"{expectedLabel}:");
            stringBuilder.AppendLine(context.ExpectedResultAsJson ?? "null");
            stringBuilder.AppendLine();

            // Current Result Section
            var currentLabel = context.CurrentResultParameterName.IsNotNullOrWhiteSpace()
                                   ? context.CurrentResultParameterName
                                   : "Actual";

            stringBuilder.AppendLine($"{currentLabel}:");
            stringBuilder.AppendLine(context.CurrentResultAsJson ?? "null");
            stringBuilder.AppendLine();

            // Parameters Section (if any)
            if (context.Parameters?.Length > 0)
            {
                stringBuilder.AppendLine("Parameters:");

                foreach (var (key, value) in context.Parameters)
                {
                    stringBuilder.AppendLine($"  {key} = {value}");
                }

                stringBuilder.AppendLine();
            }

            // Caller Info Section
            if (context.CallerFilePath.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine($"Test: {context.CallerFilePath}");
            }

            return stringBuilder.ToString();
        }
    }
}
