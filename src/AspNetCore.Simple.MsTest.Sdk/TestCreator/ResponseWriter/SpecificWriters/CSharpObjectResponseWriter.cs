using System;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddCSharpObjectResponseWriterExtension
    {
        public static void AddCSharpObjectResponseWriter(this IServiceCollection services)
        {
            SdkTrace.WriteLine("[AddCSharpObjectResponseWriter] Called!");
            services.AddParameterReplacer();
            services.AddSourceCodeExtractor();
            services.AddCSharpCodeGenerator();
            services.AddRoslynCodeManipulator();
            services.AddObjectTypeDetector();
            services.AddSingletonIfNotExists<ISpecificResponseWriter, CSharpObjectResponseWriter>();
            SdkTrace.WriteLine("[AddCSharpObjectResponseWriter] Registered!");
        }
    }

    internal sealed class CSharpObjectResponseWriter(IParameterReplacer parameterReplacementService,
                                                     ISourceCodeExtractor sourceCodeExtractor,
                                                     ICSharpCodeGenerator codeGenerator,
                                                     IRoslynCodeManipulator codeManipulator) : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            // Only handle when:
            // 1. Mode is GenerateCSharpObject
            // 2. AND there is no json snapshot file - those belong to the json writers
            // 3. AND assembly is compiled in DEBUG mode

            var canHandle = context.Mode == ResponseWriteMode.GenerateCSharpObject &&
                            context.ExpectedResult.EmbeddedFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse() &&
                            context.CallingAssembly.IsCompiledInDebug();

            SdkTrace.WriteLine($"[CSharpObjectResponseWriter.CanHandle] Mode={context.Mode}, IsDebug={context.CallingAssembly.IsCompiledInDebug()}, CanHandle={canHandle}");

            return canHandle;
        }

        public void Write(WriteResponseRequest context)
        {
            SdkTrace.WriteLine("[CSharpObjectResponseWriter.Write] Called!");

            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                SdkTrace.WriteLine("[CSharpObjectResponseWriter.Write] Not in DEBUG mode, skipping");

                return;
            }

            try
            {
                SdkTrace.WriteLine("[CSharpObjectResponseWriter.Write] Starting C# code generation...");

                // Extract context from WriteResponseRequest fields
                var callerFilePath = context.CallerFilePath;
                var callerLineNumber = context.CallerLineNumber;
                var expectedParameterName = context.ExpectedResultParameterName;

                if (string.IsNullOrWhiteSpace(callerFilePath) || callerLineNumber == 0)
                {
                    SdkTrace.WriteLine("[CSharpObjectResponseWriter.Write] Missing caller context - cannot generate code");

                    return;
                }

                // Apply parameter replacement to response JSON
                var responseJson = parameterReplacementService.ReplaceWithPlaceholders(context.CurrentResponseAsString,
                                                                                       context.Parameters.Where(p => !p.key.StartsWith("__", StringComparison.Ordinal)).ToArray());

                // Extract source code to determine indentation
                var sourceCode = sourceCodeExtractor.ExtractCallCode(callerFilePath, callerLineNumber);
                var baseIndentation = GetBaseIndentation(sourceCode);

                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] expectedParameterName='{expectedParameterName}', baseIndentation={baseIndentation}");
                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] ExpectedType={context.ExpectedType.Name}");

                // Extract the inner value JSON from HTTP response wrapper (content.value)
                var actualJson = ExtractInnerValueFromJson(responseJson);
                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] Extracted inner JSON: {actualJson}");

                // Extract variable name first
                var variableName = ExtractVariableName(expectedParameterName);
                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] variableName='{variableName}'");

                // Extract type info from the variable declaration using Roslyn
                var (typeName, constructionType) = codeManipulator.ExtractTypeInfo(callerFilePath, callerLineNumber, variableName);
                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] Extracted type name: {typeName}, construction type: {constructionType}");

                // Generate C# code based on detected type
                var csharpCode = constructionType switch
                {
                    ObjectConstructionType.AnonymousObject => codeGenerator.GenerateAnonymousObjectInitializer(actualJson, baseIndentation),
                    ObjectConstructionType.RecordPositional => codeGenerator.GenerateRecordPositionalConstructor(typeName, actualJson, baseIndentation),
                    ObjectConstructionType.RecordNominal => codeGenerator.GenerateRecordNominalInitializer(typeName, actualJson, baseIndentation),
                    ObjectConstructionType.ClassNominal => codeGenerator.GenerateClassNominalInitializer(typeName, actualJson, baseIndentation),
                    _ => codeGenerator.GenerateAnonymousObjectInitializer(actualJson, baseIndentation)
                };

                SdkTrace.WriteLine($"[CSharpObjectResponseWriter.Write] Generated code: {csharpCode}");

                // Use Roslyn to replace the object initializer in test file
                codeManipulator.ReplaceEmptyAnonymousObject(callerFilePath,
                                                            callerLineNumber,
                                                            variableName,
                                                            csharpCode);
            }
#pragma warning disable CA1031
            catch (Exception ex)
#pragma warning restore CA1031
            {
                // Log error but don't fail the test
                SdkTrace.WriteLine($"[CSharpObjectResponseWriter] Failed to generate C# code: {ex.Message}");
            }
        }

        private static int GetBaseIndentation(string? sourceCode)
        {
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                return 12; // Default indentation
            }

            // Count leading spaces in first line
            var firstLine = sourceCode.Split('\n')[0];
            var count = 0;

            foreach (var ch in firstLine)
            {
                if (ch is ' ' or '\t')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        private static string ExtractInnerValueFromJson(string jsonContent)
        {
            try
            {
                // Try to extract content.value from HTTP response wrapper
                using var doc = System.Text.Json.JsonDocument.Parse(jsonContent);

                if (doc.RootElement.TryGetProperty("content", out var content))
                {
                    if (content.TryGetProperty("value", out var value))
                    {
                        return value.GetRawText();
                    }
                }

                // Fallback: return original JSON
                return jsonContent;
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                return jsonContent;
            }
        }

        private static string ExtractVariableName(string? parameterExpression)
        {
            if (string.IsNullOrWhiteSpace(parameterExpression))
            {
                return "expectedResponse";
            }

            // Handle "new { }" -> default name
            if (parameterExpression.Trim().StartsWith("new", StringComparison.Ordinal))
            {
                return "expectedResponse";
            }

            // Extract variable name before assignment or usage
            // Examples:
            //   "expectedPerson" -> "expectedPerson"
            //   "var expectedPerson = new { }" -> "expectedPerson"
            var parts = parameterExpression.Split('=');

            if (parts.Length > 0)
            {
                var varPart = parts[0].Trim();
                var tokens = varPart.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

                return tokens.LastOrDefault() ?? "expectedResponse";
            }

            return parameterExpression.Trim();
        }
    }
}