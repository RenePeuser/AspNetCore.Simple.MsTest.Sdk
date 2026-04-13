using System.Collections.Immutable;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    public static class AddObjectOutputStrategyExtension
    {
        public static void AddObjectOutputStrategy(this IServiceCollection services)
        {
            // Register dependencies - builders needed for Object report
            services.AddDifferencesTableBuilder();
            services.AddJsonSectionBuilder();

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, ObjectOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for pure object comparisons (non-HTTP).
    /// Builds comprehensive object assertion failure output including test info,
    /// differences, and JSON comparison with Expected/Current labels.
    /// </summary>
#pragma warning disable IDE0060 // Remove unused parameter
    internal sealed class ObjectOutputStrategy(IDifferencesTableBuilder differencesTableBuilder,
                                               IJsonSectionBuilder jsonSectionBuilder) : IAssertOutputStrategy
#pragma warning restore IDE0060
    {
        public bool CanHandle(IObjectAssertContext context)
        {
            // Handle contexts where CurrentObject is a primitive type or string
            var currentObject = context.CurrentObject;

            if (currentObject == null)
            {
                return false;
            }

            var type = currentObject.GetType();

            var typeIsNotPrimitive = type.IsPrimitive.IsFalse() &&
                                     type.NotEqualsTo(typeof(string));

            return typeIsNotPrimitive;
        }

        public string BuildOutput(IObjectAssertContext context,
                                  ImmutableList<Difference> differences,
                                  string expectedJson,
                                  string currentJson)
        {
            var stringBuilder = new StringBuilder();

            // Section 1: Header + Test Info
            BuildHeader(stringBuilder, context, differences);
            stringBuilder.AppendLine();

            // Section 2: Differences Table (if any)
            // For object context, we need to build a simple differences table
            // We can't use the HTTP-based differencesTableBuilder as it expects IHttpResponseContext
            // So we build a simple table here
            if (differences.Any())
            {
                BuildDifferencesTable(stringBuilder, context, differences);
                stringBuilder.AppendLine();
            }

            // Section 3: Expected Result
            var expectedSection = BuildExpectedSection(context, expectedJson);
            stringBuilder.AppendLine(expectedSection);
            stringBuilder.AppendLine();

            // Section 4: Current Result
            var currentSection = BuildCurrentSection(currentJson);
            stringBuilder.AppendLine(currentSection);

            return stringBuilder.ToString();
        }

        private static void BuildHeader(StringBuilder stringBuilder,
                                        IObjectAssertContext context,
                                        ImmutableList<Difference> differences)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var classPath = BuildClassPath(context);
            var expectedName = GetExpectedName(context);
            var errorCount = differences.Count;
            var errorTypes = differences.Select(d => d.MismatchType).Distinct().ToList();

            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine("OBJECT COMPARISON FAILED");
            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Project    : {projectName}");
            stringBuilder.AppendLine($"Class      : {classPath}");
            stringBuilder.AppendLine($"LineNumber : {context.CallerLineNumber}");
            stringBuilder.AppendLine($"Expected   : {expectedName}");
            stringBuilder.AppendLine($"Current    : N/A");
            stringBuilder.AppendLine($"Errors     : {errorCount}");

            if (errorTypes.Any())
            {
                var errorTypesStr = string.Join(", ", errorTypes);
                stringBuilder.AppendLine($"ErrorTypes: {errorTypesStr}");
            }
        }

        private void BuildDifferencesTable(StringBuilder stringBuilder,
                                           IObjectAssertContext context,
                                           ImmutableList<Difference> differences)
        {
            var expectedName = GetExpectedName(context);

            // Suppress unused parameter warning by referencing it
            _ = differencesTableBuilder;

            stringBuilder.AppendLine("DIFFERENCES");
            stringBuilder.AppendLine();

            // Build simple table format
            foreach (var difference in differences)
            {
                stringBuilder.AppendLine($"  Path       : {difference.MemberPath ?? "N/A"}");
                stringBuilder.AppendLine($"  {expectedName,-10} : {difference.Value1 ?? "null"}");
                stringBuilder.AppendLine($"  Current    : {difference.Value2 ?? "null"}");
                stringBuilder.AppendLine($"  Type       : {difference.MismatchType}");
                stringBuilder.AppendLine();
            }
        }

        private string BuildExpectedSection(IObjectAssertContext context,
                                            string expectedJson)
        {
            // For object context, we can't use jsonSectionBuilder.BuildExpected because it expects IHttpResponseContext
            // So we build it manually here
            var expectedName = GetExpectedName(context);
            var label = $"EXPECTED RESULT ({expectedName})";

            // Suppress unused parameter warning by referencing it
            _ = jsonSectionBuilder;

            return $"{label}:\n\n{expectedJson}";
        }

        private string BuildCurrentSection(string currentJson)
        {
            // For object context, we use simple formatting
            // Suppress unused parameter warning by referencing it
            _ = jsonSectionBuilder;

            return $"CURRENT RESULT:\n\n{currentJson}";
        }

        private static string GetExpectedName(IObjectAssertContext context)
        {
            var expectedFileName = context.ExpectedResultFile.EmbeddedFile?.Name;

            if (expectedFileName.IsNotNullOrWhiteSpace())
            {
                return expectedFileName;
            }

            return "Expected";
        }

        private static string BuildClassPath(IObjectAssertContext context)
        {
            var assemblyName = context.CallingAssembly.GetName().Name;
            var callerFilePath = context.CallerFilePath;

            if (assemblyName.IsNullOrWhiteSpace())
            {
                return Path.GetFileName(callerFilePath);
            }

            // Try to find assembly name in path
            var assemblyIndex = callerFilePath.IndexOf(assemblyName, StringComparison.OrdinalIgnoreCase);

            if (assemblyIndex >= 0)
            {
                // Found! Build namespace-style path
                var relativePath = callerFilePath.Substring(assemblyIndex + assemblyName.Length)
                                                 .TrimStart('\\', '/');

                return $"{assemblyName}.{relativePath.Replace('\\', '.').Replace('/', '.')}";
            }

            // Fallback: Original path
            return callerFilePath;
        }
    }
}
