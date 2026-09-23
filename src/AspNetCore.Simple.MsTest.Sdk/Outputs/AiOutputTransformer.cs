using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAiOutputTransformerExtension
    {
        public static void AddAiOutputTransformer(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IAiOutputTransformer, AiOutputTransformer>();
        }
    }

    /// <summary>
    /// Transforms assertion contexts into structured JSON output optimized for AI agents.
    /// </summary>
    public interface IAiOutputTransformer
    {
        /// <summary>
        /// Transforms the assertion context and comparison results into structured JSON.
        /// </summary>
        /// <param name="context">The assertion context containing test location and configuration</param>
        /// <param name="differences">List of differences found during comparison</param>
        /// <param name="expectedJson">Expected result as JSON string</param>
        /// <param name="currentJson">Current/actual result as JSON string</param>
        /// <returns>Structured JSON string with error code, severity, differences, and suggested fixes</returns>
        string TransformToJson(IObjectAssertContext context,
                              ImmutableList<Difference> differences,
                              string expectedJson,
                              string currentJson);
    }

    /// <summary>
    /// Implementation that generates AI-friendly JSON output with error codes, severity,
    /// location information, differences, and actionable suggested fixes.
    /// </summary>
    internal sealed class AiOutputTransformer : IAiOutputTransformer
    {
        public string TransformToJson(IObjectAssertContext context,
                                     ImmutableList<Difference> differences,
                                     string expectedJson,
                                     string currentJson)
        {
            // Build HTTP-specific context if available
            IHttpResponseContext? httpContext = context as IHttpResponseContext;

            var output = new
            {
                errorCode = DetermineErrorCode(context, httpContext),
                severity = "error",
                project = context.CallingAssembly.GetName().Name,
                @class = ExtractClassName(context),
                method = context.CallerMemberName,
                file = context.CallerFilePath,
                line = context.CallerLineNumber,
                expectedType = context.ExpectedType.FullName,
                actualType = context.CurrentObject?.GetType().FullName,
                differences = differences.Select(d => new
                {
                    path = d.MemberPath,
                    expected = d.Value1,
                    actual = d.Value2,
                    mismatchType = d.MismatchType.ToString()
                }),
                expectedJson,
                actualJson = currentJson,
                suggestedFix = GenerateSuggestedFix(context, differences),

                // HTTP-specific fields (null for non-HTTP contexts)
                httpMethod = httpContext?.HttpResponseMessage.RequestMessage?.Method.ToString(),
                url = httpContext?.AbsoluteUrl,
                statusCode = httpContext?.HttpStatusCode,
                httpFailureType = httpContext?.FailureType,
                expectedStatusCode = httpContext?.ExpectedStatusCode,
                actualStatusCode = httpContext?.ActualStatusCode
            };

            return JsonConvert.SerializeObject(output, Formatting.Indented);
        }

        private static string DetermineErrorCode(IObjectAssertContext context, IHttpResponseContext? httpContext)
        {
            // Primitive value comparison - check if the expected type is a primitive
            var expectedType = context.ExpectedType;

            if (expectedType.IsPrimitive || expectedType == typeof(string) || expectedType == typeof(decimal))
            {
                return "VALUE_COMPARISON_FAILED";
            }

            // HTTP-specific error codes
            if (httpContext is not null)
            {
                return httpContext.FailureType switch
                {
                    HttpAssertionFailureType.StatusCodeMismatch => "HTTP_STATUS_CODE_MISMATCH",
                    HttpAssertionFailureType.SchemaMismatch => "HTTP_SCHEMA_MISMATCH",
                    HttpAssertionFailureType.SnapshotMismatch => "HTTP_SNAPSHOT_MISMATCH",
                    HttpAssertionFailureType.ContentTypeMismatch => "HTTP_CONTENT_TYPE_MISMATCH",
                    HttpAssertionFailureType.None => "HTTP_ASSERTION_FAILED",
                    _ => "HTTP_ASSERTION_FAILED"
                };
            }

            // Object comparison
            return "OBJECT_COMPARISON_FAILED";
        }

        private static string ExtractClassName(IObjectAssertContext context)
        {
            // Try to extract class name from file path
            // Format: D:\...\ProjectName\FolderName\ClassName.cs
            if (context.CallerFilePath.IsNotNullOrWhiteSpace())
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(context.CallerFilePath);

                // If we have the assembly name, prepend it as namespace
                var assemblyName = context.CallingAssembly.GetName().Name;

                if (assemblyName.IsNotNullOrWhiteSpace())
                {
                    return $"{assemblyName}.{fileName}";
                }

                return fileName;
            }

            return "UnknownClass";
        }

        private static string GenerateSuggestedFix(IObjectAssertContext context, ImmutableList<Difference> differences)
        {
            // No differences - likely schema mismatch
            if (differences.Count == 0)
            {
                return "Check schema compatibility between expected and actual types.";
            }

            // Single difference - provide specific fix
            if (differences.Count == 1)
            {
                var diff = differences[0];

                return diff.MismatchType switch
                {
                    MismatchType.ValueDifference => $"Update the expected value at path '{diff.MemberPath}' from '{diff.Value1}' to '{diff.Value2}'.",
                    MismatchType.MissingInFirst => $"Add the missing property at path '{diff.MemberPath}' with value '{diff.Value2}' to the expected result.",
                    MismatchType.MissingInSecond => $"Remove the unexpected property at path '{diff.MemberPath}' from the expected result, or add it to the actual result.",
                    _ => $"Update the expected value at path '{diff.MemberPath}'."
                };
            }

            // Multiple differences - general guidance
            var valueChanges = differences.Count(d => d.MismatchType == MismatchType.ValueDifference);
            var missing = differences.Count(d => d.MismatchType == MismatchType.MissingInFirst);
            var extra = differences.Count(d => d.MismatchType == MismatchType.MissingInSecond);

            var parts = new System.Collections.Generic.List<string>();

            if (valueChanges > 0)
            {
                parts.Add($"{valueChanges} value change(s)");
            }

            if (missing > 0)
            {
                parts.Add($"{missing} missing propert{(missing == 1 ? "y" : "ies")}");
            }

            if (extra > 0)
            {
                parts.Add($"{extra} unexpected propert{(extra == 1 ? "y" : "ies")}");
            }

            var summary = string.Join(", ", parts);

            return $"Found {differences.Count} difference(s): {summary}. Consider regenerating the snapshot or updating the expected result.";
        }
    }
}
