using System;
using System.Text;
using System.Text.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal interface ICSharpCodeGenerator
    {
        string GenerateAnonymousObjectInitializer(string jsonContent,
                                                  int baseIndentation);

        string GenerateRecordPositionalConstructor(string typeName,
                                                   string jsonContent,
                                                   int baseIndentation);

        string GenerateRecordNominalInitializer(string typeName,
                                                string jsonContent,
                                                int baseIndentation);

        string GenerateClassNominalInitializer(string typeName,
                                               string jsonContent,
                                               int baseIndentation);
    }

    internal sealed class CSharpCodeGenerator : ICSharpCodeGenerator
    {
        public string GenerateAnonymousObjectInitializer(string jsonContent,
                                                         int baseIndentation)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return "new { }";
            }

            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                var sb = new StringBuilder();

                sb.Append("new");
                sb.AppendLine();
                sb.Append(new string(' ', baseIndentation));
                sb.Append('{');

                GenerateProperties(doc.RootElement, sb, baseIndentation + 4,
                                   isFirst: true);

                sb.AppendLine();
                sb.Append(new string(' ', baseIndentation));
                sb.Append('}');

                return sb.ToString();
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return "new { }";
            }
        }

        private void GenerateProperties(JsonElement element,
                                        StringBuilder sb,
                                        int indentation,
                                        bool isFirst)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (!isFirst)
                {
                    sb.Append(',');
                }

                sb.AppendLine();
                sb.Append(new string(' ', indentation));
                sb.Append(ToCamelCase(property.Name));
                sb.Append(" = ");

                GenerateValue(property.Value, sb, indentation);

                isFirst = false;
            }
        }

        private void GenerateValue(JsonElement value,
                                   StringBuilder sb,
                                   int indentation)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Null:
                    sb.Append("null");

                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    sb.Append(value.GetBoolean() ? "true" : "false");

                    break;

                case JsonValueKind.Number:
                    if (value.TryGetInt32(out var intValue))
                    {
                        sb.Append(intValue);
                    }
                    else if (value.TryGetInt64(out var longValue))
                    {
                        sb.Append(longValue).Append('L');
                    }
                    else if (value.TryGetDouble(out var doubleValue))
                    {
                        sb.Append(doubleValue);
                    }

                    break;

                case JsonValueKind.String:
                    sb.Append('"');
                    sb.Append(EscapeString(value.GetString()));
                    sb.Append('"');

                    break;

                case JsonValueKind.Array:
                    GenerateArray(value, sb, indentation);

                    break;

                case JsonValueKind.Object:
                    sb.Append("new");
                    sb.AppendLine();
                    sb.Append(new string(' ', indentation));
                    sb.Append('{');

                    GenerateProperties(value, sb, indentation + 4,
                                       isFirst: true);

                    sb.AppendLine();
                    sb.Append(new string(' ', indentation));
                    sb.Append('}');

                    break;
            }
        }

        private void GenerateArray(JsonElement array,
                                   StringBuilder sb,
                                   int indentation)
        {
            sb.Append("new[]");
            sb.AppendLine();
            sb.Append(new string(' ', indentation));
            sb.Append('{');

            var isFirst = true;

            foreach (var item in array.EnumerateArray())
            {
                if (!isFirst)
                {
                    sb.Append(',');
                }

                sb.AppendLine();
                sb.Append(new string(' ', indentation + 4));
                GenerateValue(item, sb, indentation + 4);

                isFirst = false;
            }

            sb.AppendLine();
            sb.Append(new string(' ', indentation));
            sb.Append('}');
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            {
                return name;
            }

            return char.ToLower(name[0]) + name.Substring(1);
        }

        private static string EscapeString(string? str)
        {
            if (str == null)
            {
                return string.Empty;
            }

            return str.Replace("\\", "\\\\", StringComparison.Ordinal)
                      .Replace("\"", "\\\"", StringComparison.Ordinal)
                      .Replace("\n", "\\n", StringComparison.Ordinal)
                      .Replace("\r", "\\r", StringComparison.Ordinal)
                      .Replace("\t", "\\t", StringComparison.Ordinal);
        }

        public string GenerateRecordPositionalConstructor(string typeName,
                                                          string jsonContent,
                                                          int baseIndentation)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return $"new {typeName}()";
            }

            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                var sb = new StringBuilder();

                sb.Append($"new {typeName}(");

                var isFirst = true;

                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    if (!isFirst)
                    {
                        sb.Append(", ");
                    }

                    GenerateValueInline(property.Value, sb);
                    isFirst = false;
                }

                sb.Append(')');

                return sb.ToString();
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return $"new {typeName}()";
            }
        }

        public string GenerateRecordNominalInitializer(string typeName,
                                                       string jsonContent,
                                                       int baseIndentation)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return $"new {typeName}()";
            }

            try
            {
                using var doc = JsonDocument.Parse(jsonContent);
                var sb = new StringBuilder();

                sb.Append($"new {typeName}");
                sb.AppendLine();
                sb.Append(new string(' ', baseIndentation));
                sb.Append('{');

                GeneratePropertiesTyped(doc.RootElement, sb, baseIndentation + 4,
                                        isFirst: true);

                sb.AppendLine();
                sb.Append(new string(' ', baseIndentation));
                sb.Append('}');

                return sb.ToString();
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return $"new {typeName}()";
            }
        }

        public string GenerateClassNominalInitializer(string typeName,
                                                      string jsonContent,
                                                      int baseIndentation)
        {
            // For classes, use the same format as record nominal
            return GenerateRecordNominalInitializer(typeName, jsonContent, baseIndentation);
        }

        private void GenerateValueInline(JsonElement value,
                                         StringBuilder sb)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Null:
                    sb.Append("null");

                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    sb.Append(value.GetBoolean() ? "true" : "false");

                    break;

                case JsonValueKind.Number:
                    if (value.TryGetInt32(out var intValue))
                    {
                        sb.Append(intValue);
                    }
                    else if (value.TryGetInt64(out var longValue))
                    {
                        sb.Append(longValue).Append('L');
                    }
                    else if (value.TryGetDouble(out var doubleValue))
                    {
                        sb.Append(doubleValue);
                    }

                    break;

                case JsonValueKind.String:
                    sb.Append('"');
                    sb.Append(EscapeString(value.GetString()));
                    sb.Append('"');

                    break;

                case JsonValueKind.Array:
                    sb.Append('[');
                    var isFirst = true;

                    foreach (var item in value.EnumerateArray())
                    {
                        if (!isFirst)
                        {
                            sb.Append(", ");
                        }

                        GenerateValueInline(item, sb);
                        isFirst = false;
                    }

                    sb.Append(']');

                    break;

                case JsonValueKind.Object:
                    sb.Append("new { ");
                    isFirst = true;

                    foreach (var property in value.EnumerateObject())
                    {
                        if (!isFirst)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(ToCamelCase(property.Name));
                        sb.Append(" = ");
                        GenerateValueInline(property.Value, sb);
                        isFirst = false;
                    }

                    sb.Append(" }");

                    break;
            }
        }

        private void GeneratePropertiesTyped(JsonElement element,
                                             StringBuilder sb,
                                             int indentation,
                                             bool isFirst)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (!isFirst)
                {
                    sb.Append(',');
                }

                sb.AppendLine();
                sb.Append(new string(' ', indentation));
                sb.Append(ToPascalCase(property.Name));
                sb.Append(" = ");

                GenerateValue(property.Value, sb, indentation);

                isFirst = false;
            }
        }

        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            if (char.IsUpper(name[0]))
            {
                return name;
            }

            return char.ToUpper(name[0]) + name[1..];
        }
    }

    internal static class AddCSharpCodeGeneratorExtension
    {
        public static void AddCSharpCodeGenerator(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ICSharpCodeGenerator, CSharpCodeGenerator>();
        }
    }
}