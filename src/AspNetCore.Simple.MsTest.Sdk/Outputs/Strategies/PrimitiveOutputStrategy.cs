using System.Collections.Immutable;
using System.Text;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    public static class AddPrimitiveOutputStrategyExtension
    {
        public static void AddPrimitiveOutputStrategy(this IServiceCollection services)
        {
            // No dependencies needed for primitive output
            services.AddSingletonIfNotExists<IAssertOutputStrategy, PrimitiveOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for primitive type comparisons (int, string, bool, etc.).
    /// Builds simple assertion failure output showing expected vs current value.
    /// </summary>
    internal sealed class PrimitiveOutputStrategy : IAssertOutputStrategy
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

            var typeIsPrimitive = type.IsPrimitive || type == typeof(string);

            return typeIsPrimitive;
        }

        public string BuildOutput(IObjectAssertContext context,
                                  ImmutableList<Difference> differences,
                                  string expectedJson,
                                  string currentJson)
        {
            var stringBuilder = new StringBuilder();

            // Section 1: Header + Test Info
            BuildHeader(stringBuilder, context);
            stringBuilder.AppendLine();

            // Section 2: Expected vs Current
            stringBuilder.AppendLine($"Expected: {expectedJson}");
            stringBuilder.AppendLine($"Current : {currentJson}");

            return stringBuilder.ToString();
        }

        private static void BuildHeader(StringBuilder stringBuilder,
                                        IObjectAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var classPath = BuildClassPath(context);
            var expectedName = GetExpectedName(context);

            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine("PRIMITIVE COMPARISON FAILED");
            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"Project    : {projectName}");
            stringBuilder.AppendLine($"Class      : {classPath}");
            stringBuilder.AppendLine($"LineNumber : {context.CallerLineNumber}");
            stringBuilder.AppendLine($"Expected   : {expectedName}");
            stringBuilder.AppendLine($"Current    : N/A");
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
