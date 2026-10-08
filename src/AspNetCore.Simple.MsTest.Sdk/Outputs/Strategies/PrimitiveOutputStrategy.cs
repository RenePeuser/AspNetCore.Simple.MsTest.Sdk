using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    internal static class AddPrimitiveOutputStrategyExtension
    {
        public static void AddPrimitiveOutputStrategy(this IServiceCollection services)
        {
            services.AddCharacterDiff();
            services.AddTestContextHelper();
            services.AddSingletonIfNotExists<IAssertOutputStrategy, PrimitiveOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for primitive type comparisons (int, string, bool, etc.).
    /// Builds simple assertion failure output showing expected vs current value.
    /// </summary>
    internal sealed class PrimitiveOutputStrategy(ITextDecorator textDecorator,
                                                  ICharacterDiff characterDiff,
                                                  ITestContextHelper testContextHelper) : IAssertOutputStrategy
    {
        public bool CanHandle(IObjectAssertContext context)
        {
            // Exclude HTTP response contexts - they have their own specialized strategy
            // This prevents strategy collision when HTTP responses return primitive types
            if (context is IHttpResponseContext)
            {
                return false;
            }

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

        private void BuildComparison(StringBuilder stringBuilder,
                                     string expectedJson,
                                     string currentJson)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📊 Comparison"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();

            // Apply character-level diff for string comparisons
            var (decoratedExpected, decoratedCurrent) = characterDiff.HighlightDifferences(expectedJson, currentJson);

            stringBuilder.AppendLine($"{textDecorator.Highlight("Expected")} : {decoratedExpected}");
            stringBuilder.AppendLine($"{textDecorator.Highlight("Current")}  : {decoratedCurrent}");
        }

        private void BuildHeader(StringBuilder stringBuilder,
                                 IObjectAssertContext context)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = testContextHelper.ExtractFullyQualifiedClassName(context.CallerFilePath, context.CallingAssembly);
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
            var fileUri = $"file:///{context.CallerFilePath.Replace('\\', '/')}:{context.CallerLineNumber}";
            stringBuilder.AppendLine($"{"Project",-10} : {projectName}");
            stringBuilder.AppendLine($"{"Class",-10} : {className}");
            stringBuilder.AppendLine($"{"Method",-10} : {methodName}");
            stringBuilder.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            stringBuilder.AppendLine($"{"File",-10} : {fileUri}");
        }
    }
}