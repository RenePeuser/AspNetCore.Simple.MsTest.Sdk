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
            services.AddParameterReplacer();

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
                                        JsonSerializerOptions jsonSerializerOptions,
                                        IParameterReplacer parameterReplacementService) : IAssertService
    {
        public void ObjectsAreEqual<T>(ObjectAssertContext<T> context)
        {
            // Extract values from context
            var expectedObjectAsJson = context.ExpectedObjectAsJson;
            var currentObject = context.Current;

            var expectedResultParameterName = context.ExpectedResultParameterName;
            var currentResultParameterName = context.CurrentResultParameterName;
            var callerFilePath = context.CallerFilePath;
            var callingAssembly = context.CallingAssembly;
            var orderFunc = context.OrderFunc;
            var differenceFunc = context.DifferenceFunc;
            var title = context.Title ?? string.Empty;
            var parameters = context.Parameters;
            var writeResponse = context.WriteResponse;

            if (expectedObjectAsJson.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                expectedResultParameterName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                expectedResultParameterName = expectedObjectAsJson;
            }

            // Use pre-resolved data from context if available, otherwise fall back to legacy resolution (backward compatibility)
            string jsonObject;
            EmbeddedFileInfo localizedExpectedResponseFile;

            if (context.ResolvedExpectedJson.IsNotNull())
            {
                // Modern path: all preprocessing done before context creation
                jsonObject = context.ResolvedExpectedJson;
                localizedExpectedResponseFile = context.ExpectedResultFile!;
            }
            else
            {
                // Legacy path: resolve and process data here (backward compatibility)
                localizedExpectedResponseFile = context.ExpectedResultFile;

                if (localizedExpectedResponseFile.EmbeddedFile.IsNull() ||
                    localizedExpectedResponseFile.EmbeddedFile.Exists.IsFalse())
                {
                    localizedExpectedResponseFile = localizedExpectedResponseFile with { Content = expectedObjectAsJson };
                }

                var currentObjectAsJsonScope = currentObject.ToJson(jsonSerializerOptions);

                jsonObject = localizedExpectedResponseFile.Content.GetJsonStringFrom<T>(currentObjectAsJsonScope,
                                                                                        callingAssembly,
                                                                                        string.Empty,
                                                                                        currentResultParameterName);

                jsonObject = parameterReplacementService.ResolveParameters(jsonObject, context);
            }

            // This is most the use case when calling an API and want to know what comes back
            var currentObjectAsJson = currentObject.ToJson(jsonSerializerOptions);

            // Brand new crazy function
            // We write the current result to the expected file
            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context);

            if (shouldWriteResponse)
            {
                responseWriter.Write(context, currentObjectAsJson, localizedExpectedResponseFile);
            }

            var type = typeof(T);

            if (type.IsPrimitive || type.EqualsTo(typeof(string)))
            {
                var expectedResult = primitiveTypeConverter.ConvertTo<T>(jsonObject);
                var output = outputFormatter.GetOutputString(title, jsonObject, currentObjectAsJson);

                Assert.AreEqual(expectedResult, currentObject, output);
            }
            else
            {
                T? expectedObject = default;

                try
                {
                    expectedObject = jsonSerializer.Deserialize<T>(jsonObject);
                }
#pragma warning disable CA1031
                catch (Exception e)
#pragma warning restore CA1031
                {
                    var cantSerializeJsonErrorOutput = outputFormatter.GetOutputString(title,
                                                                                       $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}. Exception: {e.Message}",
                                                                                       jsonObject,
                                                                                       currentObjectAsJson,
                                                                                       string.Empty);

                    Assert.Fail(cantSerializeJsonErrorOutput);
                }

                var serializeResultIsNullOutput = outputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                  jsonObject.ToJson(jsonSerializerOptions),
                                                                                  null,
                                                                                  string.Empty);

                Assert.IsNotNull(expectedObject, serializeResultIsNullOutput);

                var orderedObject1 = orderFunc(expectedObject);
                var orderedObject2 = orderFunc(currentObject);

                var object1AsJson = orderedObject1.ToJson(jsonSerializerOptions);
                var object2AsJson = orderedObject2.ToJson(jsonSerializerOptions);

                // Parameter replacement already done during context creation (modern path)
                // For legacy path, parameters were resolved earlier in jsonObject
                if (context.ResolvedExpectedJson.IsNull())
                {
                    // Legacy path: apply parameter replacement here
                    object1AsJson = parameterReplacementService.ResolveParameters(object1AsJson, context);
                    object2AsJson = parameterReplacementService.ResolveParameters(object2AsJson, context);
                }

                var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

                // 1. Check if we are comparing the same schema
                var hasSchemaMismatch = differences.Any(item => item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                                item.MemberPath.EndsWith(']').IsFalse());

                var contentValueDifferences = differences.FirstOrDefault(d => d.MemberPath.Equals("Content.Value", StringComparison.OrdinalIgnoreCase));

                if (contentValueDifferences.IsNotNull())
                {
                    differences = jsonDiffer.FindDifferences(contentValueDifferences.Value1 ?? string.Empty, contentValueDifferences.Value2 ?? string.Empty);

                    hasSchemaMismatch = differences.Any(item => (item.MismatchType is MismatchType.MissingInFirst or MismatchType.MissingInSecond) &&
                                                                item.MemberPath.EndsWith(']').IsFalse());
                }

                var commonDifferences = AssertObjectExtensions.DifferenceFunc(differences).ToImmutableList();
                var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

                var differenceOutputTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var schemaNotMatchingError = outputFormatter.GetOutputString(title, "Schema mismatch: Expected result and current result does not match", object1AsJson,
                                                                             object2AsJson, differenceOutputTable, string.Empty);

                if (shouldWriteResponse)
                {
                    responseWriter.Write(context, object2AsJson, localizedExpectedResponseFile);
                }

                Assert.IsFalse(hasSchemaMismatch, schemaNotMatchingError);

                var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var output = outputFormatter.GetOutputString(title,
                                                             $"Detected differences: {optimizedDifferences.Count}",
                                                             object1AsJson,
                                                             object2AsJson,
                                                             resultTable,
                                                             string.Empty);

                if (optimizedDifferences.Any())
                {
                    Assert.Fail(output);
                }
            }
        }
    }
}
