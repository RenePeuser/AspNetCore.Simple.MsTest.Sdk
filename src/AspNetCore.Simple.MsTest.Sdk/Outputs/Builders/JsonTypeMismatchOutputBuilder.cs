using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Builders
{
    internal static class AddJsonTypeMismatchOutputBuilderExtension
    {
        public static void AddJsonTypeMismatchOutputBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IJsonTypeMismatchOutputBuilder, JsonTypeMismatchOutputBuilder>();
        }
    }

    internal interface IJsonTypeMismatchOutputBuilder
    {
        /// <summary>
        /// Builds a formatted error message for JSON type mismatches.
        /// </summary>
        string BuildObjectToArrayMismatch(string expectedResultParameterName,
                                          string targetTypeFullName,
                                          string expectedJson,
                                          string currentJson,
                                          [CallerFilePath] string sourceFilePath = "",
                                          [CallerLineNumber] int sourceLineNumber = 0,
                                          [CallerMemberName] string memberName = "");

        /// <summary>
        /// Builds a formatted error message for array-to-object type mismatches.
        /// </summary>
        string BuildArrayToObjectMismatch(string expectedResultParameterName,
                                          string targetTypeFullName,
                                          string expectedJson,
                                          string currentJson,
                                          [CallerFilePath] string sourceFilePath = "",
                                          [CallerLineNumber] int sourceLineNumber = 0,
                                          [CallerMemberName] string memberName = "");
    }

    internal sealed class JsonTypeMismatchOutputBuilder(ITextDecorator textDecorator) : IJsonTypeMismatchOutputBuilder
    {
        private const int MaxJsonDisplayLength = 500;

        public string BuildObjectToArrayMismatch(string expectedResultParameterName,
                                                 string targetTypeFullName,
                                                 string expectedJson,
                                                 string currentJson,
                                                 [CallerFilePath] string sourceFilePath = "",
                                                 [CallerLineNumber] int sourceLineNumber = 0,
                                                 [CallerMemberName] string memberName = "")
        {
            return BuildMismatch("OBJECT {} TO ARRAY [] MISMATCH",
                                 "Invalid source type: object {} cannot be converted to target array type []",
                                 expectedResultParameterName,
                                 targetTypeFullName,
                                 expectedJson,
                                 currentJson,
                                 sourceFilePath,
                                 sourceLineNumber,
                                 memberName);
        }

        public string BuildArrayToObjectMismatch(string expectedResultParameterName,
                                                 string targetTypeFullName,
                                                 string expectedJson,
                                                 string currentJson,
                                                 [CallerFilePath] string sourceFilePath = "",
                                                 [CallerLineNumber] int sourceLineNumber = 0,
                                                 [CallerMemberName] string memberName = "")
        {
            return BuildMismatch("ARRAY [] TO OBJECT {} MISMATCH",
                                 "Invalid source type: array [] cannot be converted to target object type {}",
                                 expectedResultParameterName,
                                 targetTypeFullName,
                                 expectedJson,
                                 currentJson,
                                 sourceFilePath,
                                 sourceLineNumber,
                                 memberName);
        }

        private string BuildMismatch(string title,
                                     string errorDescription,
                                     string parameterName,
                                     string targetType,
                                     string expectedJson,
                                     string currentJson,
                                     string sourceFilePath,
                                     int sourceLineNumber,
                                     string memberName)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error($"🚫 JSON TYPE MISMATCH: {title}"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            // Test Information Section
            BuildTestInfoSection(sb, sourceFilePath, sourceLineNumber,
                                 memberName);

            // Error Details Section
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Error Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{textDecorator.Highlight("Parameter")}  : {parameterName}");
            sb.AppendLine($"{textDecorator.Highlight("Target Type")}: {targetType}");
            sb.AppendLine($"{textDecorator.Highlight("Issue")}      : {errorDescription}");
            sb.AppendLine();

            // Expected JSON Section
            sb.AppendLine(textDecorator.SectionTitle("📄 Expected JSON (Source)"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            AppendFormattedJson(sb, expectedJson, "expected");
            sb.AppendLine();

            // Current JSON Section
            sb.AppendLine(textDecorator.SectionTitle("📄 Current JSON (Actual)"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            AppendFormattedJson(sb, currentJson, "actual");
            sb.AppendLine();

            // Help Section
            sb.AppendLine(textDecorator.SectionTitle("💡 How to Fix"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            if (title.Contains("OBJECT {} TO ARRAY []"))
            {
                sb.AppendLine("• The JSON starts with '[' but your target type is not an array/collection");
                sb.AppendLine("• Either wrap your JSON in '{}' to make it an object");
                sb.AppendLine($"• Or change your target type to IEnumerable<T> or List<T>");
            }
            else
            {
                sb.AppendLine("• The JSON starts with '{' but your target type expects an array/collection");
                sb.AppendLine("• Either wrap your JSON in '[]' to make it an array");
                sb.AppendLine("• Or change your target type to match the object structure");
            }

            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("══════════════════════════════════════════════════════════════"));

            return sb.ToString();
        }

        private void BuildTestInfoSection(StringBuilder sb,
                                          string sourceFilePath,
                                          int sourceLineNumber,
                                          string memberName)
        {
            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            if (sourceFilePath.IsNotNullOrWhiteSpace())
            {
                var fileName = System.IO.Path.GetFileName(sourceFilePath);
                var className = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath);

                sb.AppendLine($"{textDecorator.Highlight("Class")}     : {className}");
                sb.AppendLine($"{textDecorator.Highlight("Method")}    : {memberName}");
                sb.AppendLine($"{textDecorator.Highlight("Line")}      : {sourceLineNumber}");
                sb.AppendLine($"{textDecorator.Highlight("File")}      : file:///{sourceFilePath.Replace("\\", "/")}:{sourceLineNumber}");
            }

            sb.AppendLine();
        }

        private void AppendFormattedJson(StringBuilder sb,
                                         string json,
                                         string label)
        {
            if (json.IsNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.Dim($"  ({label} JSON is empty or null)"));

                return;
            }

            var displayJson = json.Length > MaxJsonDisplayLength
                                  ? string.Concat(json.AsSpan(0, MaxJsonDisplayLength), textDecorator.Dim($"... (truncated {json.Length - MaxJsonDisplayLength} chars)"))
                                  : json;

            // Try to pretty-print if it looks like valid JSON
            if (displayJson.StartsWith('{') || displayJson.StartsWith('['))
            {
#pragma warning disable CA1031 // Do not catch general exception types - formatting is best-effort
                try
                {
                    var formatted = TryFormatJson(displayJson);
                    sb.AppendLine(formatted);
                }
                catch (Exception)
                {
                    // If formatting fails, just display as-is
                    sb.AppendLine($"  {displayJson}");
                }
#pragma warning restore CA1031
            }
            else
            {
                sb.AppendLine($"  {displayJson}");
            }
        }

        private string TryFormatJson(string json)
        {
#pragma warning disable CA1031 // Do not catch general exception types - formatting is best-effort
            try
            {
                // Basic indentation for readability (without full JSON parsing)
                var indented = new StringBuilder();
                var indent = 0;
                var inString = false;

                for (var i = 0; i < Math.Min(json.Length, MaxJsonDisplayLength); i++)
                {
                    var c = json[i];

                    if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                    {
                        inString = !inString;
                    }

                    if (!inString)
                    {
                        if (c is '{' or '[')
                        {
                            indented.Append(c);
                            indent++;
                            indented.AppendLine();
                            indented.Append(new string(' ', indent * 2));
                        }
                        else if (c is '}' or ']')
                        {
                            indent--;
                            indented.AppendLine();
                            indented.Append(new string(' ', indent * 2));
                            indented.Append(c);
                        }
                        else if (c == ',')
                        {
                            indented.Append(c);
                            indented.AppendLine();
                            indented.Append(new string(' ', indent * 2));
                        }
                        else if (c is not '\r' and not '\n')
                        {
                            indented.Append(c);
                        }
                    }
                    else
                    {
                        indented.Append(c);
                    }
                }

                return indented.ToString();
            }
            catch (Exception)
            {
                return json;
            }
#pragma warning restore CA1031
        }
    }
}