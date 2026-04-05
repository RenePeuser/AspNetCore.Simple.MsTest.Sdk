using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddObjectOutputStrategyExtension
    {
        public static void AddObjectOutputStrategy(this IServiceCollection services)
        {
            // Register dependencies
            services.AddOutputFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, ObjectOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for pure object comparisons (non-HTTP).
    /// Uses the legacy OutputFormatter for simple object comparison output.
    /// </summary>
    internal sealed class ObjectOutputStrategy(IOutputFormatter outputFormatter) : IAssertOutputStrategy
    {
        public bool CanHandle(IObjectAssertContext context)
        {
            // Handle only pure object contexts, not HTTP-derived contexts
            return context is not IHttpResponseContext;
        }

        public string BuildOutput(IObjectAssertContext context,
                                  ImmutableList<Difference> differences,
                                  string expectedJson,
                                  string currentJson)
        {
            var title = context.Title ?? string.Empty;
            var expectedResultParameterName = context.ExpectedResultParameterName;
            var currentResultParameterName = context.CurrentResultParameterName;

            // Determine if schema mismatch or value differences
            var hasSchemaMismatch = differences.Any(item =>
                                                        item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                        item.MemberPath.EndsWith(']').IsFalse());

            if (hasSchemaMismatch)
            {
                // Schema mismatch output
                var differenceTable = differences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                return outputFormatter.GetOutputString(title,
                                                       "Schema mismatch: Expected result and current result does not match",
                                                       expectedJson,
                                                       currentJson,
                                                       differenceTable,
                                                       string.Empty);
            }

            // Value differences output
            if (differences.Any())
            {
                var resultTable = differences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                return outputFormatter.GetOutputString(title,
                                                       $"Detected differences: {differences.Count}",
                                                       expectedJson,
                                                       currentJson,
                                                       resultTable,
                                                       string.Empty);
            }

            // Fallback: no differences (shouldn't happen)
            return outputFormatter.GetOutputString(title, expectedJson, currentJson);
        }
    }
}
