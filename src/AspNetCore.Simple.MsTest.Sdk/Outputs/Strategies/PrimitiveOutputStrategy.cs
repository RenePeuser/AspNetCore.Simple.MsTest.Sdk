using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
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
    internal sealed class PrimitiveOutputStrategy(ITextDecorator textDecorator) : IAssertOutputStrategy
    {
        private readonly CharacterDiff _characterDiff = new CharacterDiff(textDecorator);
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

            // Section 2: Expected vs Current with character-level diff
            BuildComparison(stringBuilder, expectedJson, currentJson);

            return stringBuilder.ToString();
        }

        private void BuildComparison(StringBuilder stringBuilder, string expectedJson, string currentJson)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📊 Comparison"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();

            // Apply character-level diff for string comparisons
            var (decoratedExpected, decoratedCurrent) = _characterDiff.HighlightDifferences(expectedJson, currentJson);

            stringBuilder.AppendLine($"{textDecorator.Highlight("Expected")} : {decoratedExpected}");
            stringBuilder.AppendLine($"{textDecorator.Highlight("Current")}  : {decoratedCurrent}");
        }

        private void BuildHeader(StringBuilder stringBuilder,
                                 IObjectAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = ExtractClassName(context);
            var methodName = context.CallerMemberName;

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine(textDecorator.Error("❌ VALUE COMPARISON FAILED"));
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{"Project",-10} : {projectName}");
            stringBuilder.AppendLine($"{"Class",-10} : {className}");
            stringBuilder.AppendLine($"{"Method",-10} : {methodName}");
            stringBuilder.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
        }

        private static string ExtractClassName(IObjectAssertContext context)
        {
            var callerFilePath = context.CallerFilePath;
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
            return fileName.Replace(".cs", string.Empty);
        }
    }
}