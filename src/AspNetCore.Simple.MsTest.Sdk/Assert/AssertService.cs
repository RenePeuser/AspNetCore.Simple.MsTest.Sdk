using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAssertServiceExtension
    {
        /// <summary>
        /// Registers all assertion services and their dependencies in the DI container.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddAssertService(this IServiceCollection services)
        {
            // 1. Register all dependencies via their own extensions
            services.AddPrimitiveTypeConverter();
            services.AddJsonDiffer();
            services.AddOutputFormatter();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddJsonSerializer();

            // Note: IEmbeddedFileLocalizer registration requires IConfiguration and should be done at app startup

            // 2. Register the service itself
            services.AddSingletonIfNotExists<IAssertService, AssertService>();
        }
    }

    /// <summary>
    /// Represents an assertion service for comparing objects in tests.
    /// Core service that performs deep object comparisons with automatic diff generation.
    /// </summary>
    public interface IAssertService
    {
        /// <summary>
        /// Compares two objects and asserts they are equal using the provided context configuration.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">The assertion context containing all comparison parameters.</param>
        void ObjectsAreEqual<T>(ObjectAssertContext<T> context);
    }

    /// <summary>
    /// Implementation of <see cref="IAssertService"/> that contains the core assertion logic.
    /// This class encapsulates all dependencies needed for object assertions.
    /// All dependencies are injected via the primary constructor for testability and flexibility.
    /// </summary>
    internal sealed class AssertService(IPrimitiveTypeConverter primitiveTypeConverter,
                                        IJsonDiffer jsonDiffer,
                                        IOutputFormatter outputFormatter,
                                        IResponseWriter responseWriter,
                                        IWriteResponseService writeResponseService,
                                        JsonSerializer jsonSerializer,
                                        JsonSerializerOptions jsonSerializerOptions) : IAssertService
    {
        public void ObjectsAreEqual<T>(ObjectAssertContext<T> context)
        {
            // Assumption: Context is fully prepared with ResolvedExpectedJson
            var expectedJson = context.ResolvedExpectedJson ?? string.Empty;
            var currentObject = context.Current;

            // 1. Serialize current object
            var currentJson = currentObject.ToJson(jsonSerializerOptions);

            // 2. Write response if configured
            if (writeResponseService.ShouldWriteResponse(context))
            {
                responseWriter.Write(context, currentJson, context.ExpectedResultFile);
            }

            // 3. Handle primitive types vs. complex objects
            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
            {
                HandlePrimitiveComparison(context, expectedJson, currentJson);
            }
            else
            {
                HandleObjectComparison(context, expectedJson, currentJson);
            }
        }

        private void HandlePrimitiveComparison<T>(ObjectAssertContext<T> context,
                                                  string expectedJson,
                                                  string currentJson)
        {
            var expectedValue = primitiveTypeConverter.ConvertTo<T>(expectedJson);
            var title = context.Title ?? string.Empty;

            var output = outputFormatter.GetOutputString(title, expectedJson, currentJson);

            Assert.AreEqual(expectedValue, context.Current, output);
        }

        private void HandleObjectComparison<T>(ObjectAssertContext<T> context,
                                               string expectedJson,
                                               string currentJson)
        {
            var expectedResultParameterName = context.ExpectedResultParameterName;
            var currentResultParameterName = context.CurrentResultParameterName;
            var title = context.Title ?? string.Empty;

            // 1. Deserialize expected object
            T? expectedObject;

            try
            {
                expectedObject = jsonSerializer.Deserialize<T>(expectedJson);
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                var error = outputFormatter.GetOutputString(title,
                                                            $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}. Exception: {e.Message}",
                                                            expectedJson,
                                                            currentJson,
                                                            string.Empty);

                Assert.Fail(error);

                return; // Unreachable, but helps compiler
            }

            // 2. Validate deserialized object
            var nullError = outputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                            expectedJson,
                                                            null,
                                                            string.Empty);

            Assert.IsNotNull(expectedObject, nullError);

            // 3. Apply ordering function
            var orderedExpected = context.OrderFunc(expectedObject);
            var orderedCurrent = context.OrderFunc(context.Current);

            var expectedOrderedJson = orderedExpected.ToJson(jsonSerializerOptions);
            var currentOrderedJson = orderedCurrent.ToJson(jsonSerializerOptions);

            // 4. Find differences
            var differences = jsonDiffer.FindDifferences(expectedOrderedJson, currentOrderedJson);

            // 5. Check for schema mismatches
            var hasSchemaMismatch = differences.Any(item =>
                                                        item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                        item.MemberPath.EndsWith(']').IsFalse());

            // 6. Special case: Content.Value differences (HTTP-specific - TODO: move to strategy)
            var contentValueDifference = differences.FirstOrDefault(d =>
                                                                        d.MemberPath.Equals("Content.Value", StringComparison.OrdinalIgnoreCase));

            if (contentValueDifference.IsNotNull())
            {
                differences = jsonDiffer.FindDifferences(contentValueDifference.Value1 ?? string.Empty,
                                                         contentValueDifference.Value2 ?? string.Empty);

                hasSchemaMismatch = differences.Any(item =>
                                                        (item.MismatchType is MismatchType.MissingInFirst or MismatchType.MissingInSecond) &&
                                                        item.MemberPath.EndsWith(']').IsFalse());
            }

            // 7. Filter differences
            var commonDifferences = AssertObjectExtensions.DifferenceFunc(differences).ToImmutableList();
            var filteredDifferences = context.DifferenceFunc(commonDifferences).ToImmutableList();

            // 8. Write response again if needed (with filtered differences)
            if (writeResponseService.ShouldWriteResponse(context))
            {
                responseWriter.Write(context, currentOrderedJson, context.ExpectedResultFile);
            }

            // 9. Assert schema matches
            if (hasSchemaMismatch)
            {
                var differenceTable = filteredDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var schemaError = outputFormatter.GetOutputString(title,
                                                                  "Schema mismatch: Expected result and current result does not match",
                                                                  expectedOrderedJson,
                                                                  currentOrderedJson,
                                                                  differenceTable,
                                                                  string.Empty);

                Assert.Fail(schemaError);
            }

            // 10. Assert values match
            if (filteredDifferences.Any())
            {
                var resultTable = filteredDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var valueError = outputFormatter.GetOutputString(title,
                                                                 $"Detected differences: {filteredDifferences.Count}",
                                                                 expectedOrderedJson,
                                                                 currentOrderedJson,
                                                                 resultTable,
                                                                 string.Empty);

                Assert.Fail(valueError);
            }
        }
    }
}
