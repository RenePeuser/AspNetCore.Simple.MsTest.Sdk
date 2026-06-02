using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
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

            // Note: ITextDecorator is registered separately based on build configuration

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, ObjectOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for pure object comparisons (non-HTTP).
    /// Builds comprehensive object assertion failure output including test info,
    /// differences, and JSON comparison with Expected/Current labels.
    /// </summary>
#pragma warning disable IDE0060 // Remove unused parameter - jsonSectionBuilder kept for future use
    internal sealed class ObjectOutputStrategy(IDifferencesTableBuilder differencesTableBuilder,
                                               IJsonSectionBuilder jsonSectionBuilder,
                                               ITextDecorator textDecorator) : IAssertOutputStrategy
#pragma warning restore IDE0060
    {
        public bool CanHandle(IObjectAssertContext context)
        {
            if (context is IHttpAssertContext)
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
            if (differences.Any())
            {
                BuildDifferencesSection(stringBuilder, context, differences);
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

        private void BuildHeader(StringBuilder stringBuilder,
                                 IObjectAssertContext context,
                                 ImmutableList<Difference> differences)
        {
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var className = TestContextHelper.ExtractFullyQualifiedClassName(context.CallerFilePath, context.CallingAssembly);
            var methodName = context.CallerMemberName;

            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            stringBuilder.AppendLine(textDecorator.Error("❌ OBJECT COMPARISON FAILED"));
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

        private void BuildDifferencesSection(StringBuilder stringBuilder,
                                             IObjectAssertContext context,
                                             ImmutableList<Difference> differences)
        {
            var expectedName = GetExpectedName(context);

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle($"🔍 Differences (Count {differences.Count})"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();

            // Use the proper table builder with character-level diff
            var table = differencesTableBuilder.BuildObjectDifferencesTable(expectedName, differences);
            stringBuilder.Append(table);
        }

        private string BuildExpectedSection(IObjectAssertContext context,
                                            string expectedJson)
        {
            // Suppress unused parameter warning
            _ = jsonSectionBuilder;

            var expectedName = GetExpectedName(context);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle($"📄 Expected ({expectedName})"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(NormalizeJsonToSingleLine(expectedJson));

            return stringBuilder.ToString();
        }

        private string BuildCurrentSection(string currentJson)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📄 Current"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(NormalizeJsonToSingleLine(currentJson));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Normalizes JSON to single-line format (removes indentation and newlines).
        /// </summary>
        private static string NormalizeJsonToSingleLine(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json;
            }

            try
            {
                return Newtonsoft.Json.Linq.JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.None);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return json;
            }
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
    }
}