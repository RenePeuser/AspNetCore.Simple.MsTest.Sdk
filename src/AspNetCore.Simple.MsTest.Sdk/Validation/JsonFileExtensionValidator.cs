using System;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    public static class AddJsonFileExtensionValidatorExtension
    {
        public static void AddJsonFileExtensionValidator(this IServiceCollection services)
        {
            services.AddSourceCodeExtractor();
            services.AddSingletonIfNotExists<IJsonFileExtensionValidator, JsonFileExtensionValidator>();
        }
    }

    public interface IJsonFileExtensionValidator
    {
        /// <summary>
        /// Validates that file references have the required .json extension.
        /// Throws an exception with a beautifully formatted error message and suggested fix if validation fails.
        /// </summary>
        void ValidatePayloadAndExpectedResult(string payloadAsJson,
                                              string expectedResult,
                                              string callerFilePath,
                                              int callerLineNumber);
    }

    internal sealed class JsonFileExtensionValidator(ISourceCodeExtractor sourceCodeExtractor,
                                                     ITextDecorator textDecorator) : IJsonFileExtensionValidator
    {
        public void ValidatePayloadAndExpectedResult(string payloadAsJson,
                                                     string expectedResult,
                                                     string callerFilePath,
                                                     int callerLineNumber)
        {
            // Validate payload
            ValidateSingleInput(payloadAsJson, "Payload", callerFilePath, callerLineNumber);

            // Validate expected result
            ValidateSingleInput(expectedResult, "ExpectedResult", callerFilePath, callerLineNumber);
        }

        private void ValidateSingleInput(string input,
                                         string parameterType,
                                         string callerFilePath,
                                         int callerLineNumber)
        {
            var trimmed = input?.Trim().Trim('"') ?? string.Empty;

            // Skip validation if empty
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return;
            }

            // Check if it's raw JSON - if yes, validation passes
            if (IsRawJson(trimmed))
            {
                return;
            }

            // Not raw JSON - must end with .json
            if (!trimmed.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                var suggestedFix = $"{trimmed}.json";
                var errorMessage = BuildMissingJsonExtensionError(trimmed,
                                                                  suggestedFix,
                                                                  parameterType,
                                                                  callerFilePath,
                                                                  callerLineNumber);

                throw new InvalidOperationException(errorMessage);
            }
        }

        private string BuildMissingJsonExtensionError(string invalidInput,
                                                      string suggestedFix,
                                                      string parameterType,
                                                      string callerFilePath,
                                                      int callerLineNumber)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ INVALID FILE REFERENCE: MISSING .json EXTENSION"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            // File Information
            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            var fileUri = $"file:///{callerFilePath.Replace('\\', '/')}:{callerLineNumber}";
            sb.AppendLine($"{"File",-10} : {fileUri}");
            sb.AppendLine($"{"Line",-10} : {callerLineNumber}");
            sb.AppendLine($"{"Parameter",-10} : {parameterType}");
            sb.AppendLine();

            // Failure Details
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("File references for payloads and expected responses must end with " + textDecorator.Success(".json"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Dim("Note: Validation failed before executing the HTTP request."));
            sb.AppendLine();
            sb.AppendLine($"You provided: {textDecorator.Error($"\"{invalidInput}\"")}");
            sb.AppendLine($"Expected:     {textDecorator.Success($"\"{suggestedFix}\"")}");
            sb.AppendLine();

            // What's Valid
            sb.AppendLine(textDecorator.SectionTitle("✅ Valid Inputs"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("  • File reference:  \"MyPayload.json\""));
            sb.AppendLine(textDecorator.Success("  • Dotted path:     \"Requests.MyPayload.json\""));
            sb.AppendLine(textDecorator.Success("  • Inline JSON:     \"{}\" or \"[]\""));
            sb.AppendLine();

            // What's Invalid
            sb.AppendLine(textDecorator.SectionTitle("❌ Invalid Inputs"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("  • Missing .json:   \"MyPayload\""));
            sb.AppendLine(textDecorator.Error("  • Missing .json:   \"Requests.MyPayload\""));
            sb.AppendLine();

            // Assert Call - Original source code
            var sourceCode = sourceCodeExtractor.ExtractCallCode(callerFilePath, callerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
                sb.AppendLine();

                // Suggested Fix - Generate corrected code
                var suggestedFixCode = sourceCode.Replace($"\"{invalidInput}\"", $"\"{suggestedFix}\"");

                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success(suggestedFixCode));
                sb.AppendLine();
            }
            else
            {
                // Fallback if no source code is available
                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine($"Change {textDecorator.Error($"\"{invalidInput}\"")} to {textDecorator.Success($"\"{suggestedFix}\"")}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static bool IsRawJson(string input)
        {
            var trimmed = input.TrimStart();
            return trimmed.StartsWith('{') || trimmed.StartsWith('[');
        }
    }
}
