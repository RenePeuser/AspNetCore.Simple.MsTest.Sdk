using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Formatters
{
    /// <summary>
    /// Formatter for JSON type mismatch errors without DI dependencies.
    /// Can be used from static contexts like StringExtensions.
    /// </summary>
    public static class JsonTypeMismatchFormatter
    {
        private const int MaxJsonDisplayLength = 500;

        private const string BorderLine = "══════════════════════════════════════════════════════════════";

        private const string SeparatorLine = "──────────────────────────────────────────────────────────────";

        /// <summary>
        /// Formats object-to-array type mismatch error with beautiful output.
        /// </summary>
        public static string FormatObjectToArrayMismatch(string expectedResultParameterName,
                                                         string targetTypeFullName,
                                                         string expectedJson,
                                                         string currentJson,
                                                         [CallerFilePath] string sourceFilePath = "",
                                                         [CallerLineNumber] int sourceLineNumber = 0,
                                                         [CallerMemberName] string memberName = "")
        {
            return FormatMismatch("JSON TYPE MISMATCH: ARRAY [] TO OBJECT {} CONVERSION",
                                  "Invalid JSON structure conversion",
                                  "The JSON starts with '[' (array), but the target type is not an array/collection",
                                  "• Wrap your JSON in '{}' to make it an object, OR\n  • Change your target type to IEnumerable<T> or List<T>",
                                  expectedResultParameterName,
                                  targetTypeFullName,
                                  expectedJson,
                                  currentJson,
                                  sourceFilePath,
                                  sourceLineNumber,
                                  memberName);
        }

        /// <summary>
        /// Formats array-to-object type mismatch error with beautiful output.
        /// </summary>
        public static string FormatArrayToObjectMismatch(string expectedResultParameterName,
                                                         string targetTypeFullName,
                                                         string expectedJson,
                                                         string currentJson,
                                                         [CallerFilePath] string sourceFilePath = "",
                                                         [CallerLineNumber] int sourceLineNumber = 0,
                                                         [CallerMemberName] string memberName = "")
        {
            return FormatMismatch("JSON TYPE MISMATCH: OBJECT {} TO ARRAY [] CONVERSION",
                                  "Invalid JSON structure conversion",
                                  "The JSON starts with '{' (object), but the target type expects an array/collection",
                                  "• Wrap your JSON in '[]' to make it an array, OR\n  • Change your target type to match the object structure",
                                  expectedResultParameterName,
                                  targetTypeFullName,
                                  expectedJson,
                                  currentJson,
                                  sourceFilePath,
                                  sourceLineNumber,
                                  memberName);
        }

        private static string FormatMismatch(string title,
                                             string failureType,
                                             string errorDescription,
                                             string howToFix,
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
            sb.AppendLine(BorderLine);
            sb.AppendLine($"🚫 {title}");
            sb.AppendLine(BorderLine);
            sb.AppendLine();

            // Test Information Section
            AppendTestInfoSection(sb, sourceFilePath, sourceLineNumber,
                                  memberName, failureType);

            // Error Details Section
            sb.AppendLine("⚠️ Error Details");
            sb.AppendLine(SeparatorLine);
            sb.AppendLine();
            sb.AppendLine($"Parameter   : {parameterName}");
            sb.AppendLine($"Target Type : {targetType}");
            sb.AppendLine($"Issue       : {errorDescription}");
            sb.AppendLine();

            // Expected JSON Section
            sb.AppendLine("📄 Expected JSON (Source)");
            sb.AppendLine(SeparatorLine);
            sb.AppendLine();
            AppendFormattedJson(sb, expectedJson, "expected");
            sb.AppendLine();

            // Current JSON Section
            sb.AppendLine("📄 Current JSON (Actual)");
            sb.AppendLine(SeparatorLine);
            sb.AppendLine();
            AppendFormattedJson(sb, currentJson, "actual");
            sb.AppendLine();

            // Help Section
            sb.AppendLine("💡 How to Fix");
            sb.AppendLine(SeparatorLine);
            sb.AppendLine();
            sb.AppendLine($"  {howToFix}");
            sb.AppendLine();

            sb.AppendLine(BorderLine);

            return sb.ToString();
        }

        private static void AppendTestInfoSection(StringBuilder sb,
                                                  string sourceFilePath,
                                                  int sourceLineNumber,
                                                  string memberName,
                                                  string failureType)
        {
            sb.AppendLine("📦 Test Information");
            sb.AppendLine(SeparatorLine);
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(sourceFilePath))
            {
                var className = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath);

                sb.AppendLine($"Class     : {className}");
                sb.AppendLine($"Method    : {memberName}");
                sb.AppendLine($"Line      : {sourceLineNumber}");
                sb.AppendLine($"File      : file:///{sourceFilePath.Replace("\\", "/")}:{sourceLineNumber}");
                sb.AppendLine($"Failure   : {failureType}");
            }

            sb.AppendLine();
        }

        private static void AppendFormattedJson(StringBuilder sb,
                                                string json,
                                                string label)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                sb.AppendLine($"  ({label} JSON is empty or null)");

                return;
            }

            var displayJson = json.Length > MaxJsonDisplayLength
                                  ? string.Concat(json.AsSpan(0, MaxJsonDisplayLength), $"... (truncated {json.Length - MaxJsonDisplayLength} chars)")
                                  : json;

            // Try to pretty-print if it looks like valid JSON
            if (displayJson.StartsWith('{') || displayJson.StartsWith('['))
            {
#pragma warning disable CA1031 // Do not catch general exception types - formatting is best-effort
                try
                {
                    var formatted = TryFormatJson(displayJson);
                    sb.AppendLine($"  {formatted}");
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

        private static string TryFormatJson(string json)
        {
#pragma warning disable CA1031 // Do not catch general exception types - formatting is best-effort
            try
            {
                // Basic indentation for readability (without full JSON parsing)
                var indented = new StringBuilder();
                var indent = 0;
                var inString = false;
                var prevChar = '\0';

                for (var i = 0; i < Math.Min(json.Length, MaxJsonDisplayLength); i++)
                {
                    var c = json[i];

                    if (c == '"' && prevChar != '\\')
                    {
                        inString = !inString;
                    }

                    if (!inString)
                    {
                        if (c == '{' || c == '[')
                        {
                            indented.Append(c);
                            indent++;

                            if (i < json.Length - 1 && json[i + 1] != '}' && json[i + 1] != ']')
                            {
                                indented.AppendLine();
                                indented.Append(' ', indent * 2);
                            }
                        }
                        else if (c == '}' || c == ']')
                        {
                            indent--;

                            if (prevChar != '{' && prevChar != '[')
                            {
                                indented.AppendLine();
                                indented.Append(' ', indent * 2);
                            }

                            indented.Append(c);
                        }
                        else if (c == ',')
                        {
                            indented.Append(c);
                            indented.AppendLine();
                            indented.Append(' ', indent * 2);
                        }
                        else if (c != '\r' && c != '\n')
                        {
                            indented.Append(c);
                        }
                    }
                    else
                    {
                        indented.Append(c);
                    }

                    prevChar = c;
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