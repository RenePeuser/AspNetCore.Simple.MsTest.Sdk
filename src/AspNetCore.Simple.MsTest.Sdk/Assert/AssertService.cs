using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
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
            services.AddCurlFormatter();
            services.AddCurlPrinter();
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
                                        ICurlFormatter curlFormatter,
                                        ICurlPrinter curlPrinter,
                                        IOutputFormatter outputFormatter,
                                        IResponseWriter responseWriter,
                                        IWriteResponseService writeResponseService,
                                        IEmbeddedFileLocalizer embeddedFileLocalizer,
                                        JsonSerializer jsonSerializer,
                                        JsonSerializerOptions jsonSerializerOptions,
                                        IParameterReplacer parameterReplacementService) : IAssertService
    {
        public void ObjectsAreEqual<T>(ObjectAssertContext<T> context)
        {
            // Extract values from context
            var expectedObjectAsJson = context.ExpectedObjectAsJson;
            var currentObject = context.CurrentObject;

            var expectedResultParameterName = context.ExpectedResultParameterName;
            var currentResultParameterName = context.CurrentResultParameterName;
            var callerFilePath = context.CallerFilePath;
            var callingAssembly = context.CallingAssembly;
            var orderFunc = context.OrderFunc;
            var differenceFunc = context.DifferenceFunc;
            var title = context.Title ?? string.Empty;
            var curl = context.Curl ?? string.Empty;
            var parameters = context.Parameters;
            var writeResponse = context.WriteResponse;

            if (expectedObjectAsJson.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                expectedResultParameterName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                expectedResultParameterName = expectedObjectAsJson;
            }

            // localize expected response and payload
            // So the caller does not have to pass the unique file name of the embedded resource
            // - Api.V1.Users.GetAllUsersTest.Responses.GetAllUsersResponse.json
            // - GetAllUsersResponse.json
            var localizedExpectedResponseFile = embeddedFileLocalizer.LocalizeResponseFile(expectedResultParameterName, callerFilePath, callingAssembly);

            if (localizedExpectedResponseFile.EmbeddedFile.IsNull() ||
                localizedExpectedResponseFile.EmbeddedFile.Exists.IsFalse())
            {
                localizedExpectedResponseFile = localizedExpectedResponseFile with { Content = expectedObjectAsJson };
            }

            // This is most the use case when calling an API and want to know what comes back
            var currentObjectAsJson = currentObject.ToJson(jsonSerializerOptions);

            var jsonObject = localizedExpectedResponseFile.Content.GetJsonStringFrom<T>(currentObjectAsJson,
                                                                                        callingAssembly,
                                                                                        curl,
                                                                                        currentResultParameterName);

            jsonObject = parameterReplacementService.ResolveParameters(jsonObject, parameters);

            // Brand new crazy function
            // We write the current result to the expected file
            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(writeResponse, callingAssembly);

            if (shouldWriteResponse)
            {
                var writeResponseRequest = new WriteResponseRequest()
                                           {
                                               CallingAssembly = callingAssembly,
                                               DifferenceFunc = differenceFunc,
                                               CurrentResponseAsString = currentObjectAsJson,
                                               ExpectedResult = localizedExpectedResponseFile,
                                               Parameters = parameters,
                                               Mode = ResponseWriteMode.DifferencesOnly
                                           };

                responseWriter.Write(writeResponseRequest);
            }

            var type = typeof(T);

            if (type.IsPrimitive || type.EqualsTo(typeof(string)))
            {
                var expectedResult = primitiveTypeConverter.ConvertTo<T>(jsonObject);
                var output = outputFormatter.GetOutputString(title, jsonObject, currentObjectAsJson);

                Assert.AreEqual(expectedResult, currentObject, output);

                curlPrinter.PrintCurl(callingAssembly, curl);
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
                                                                                       curlFormatter.GetCurlAsFormattedString(curl));

                    Assert.Fail(cantSerializeJsonErrorOutput);
                }

                var serializeResultIsNullOutput = outputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                  jsonObject.ToJson(jsonSerializerOptions),
                                                                                  null,
                                                                                  curlFormatter.GetCurlAsFormattedString(curl));

                Assert.IsNotNull(expectedObject, serializeResultIsNullOutput);

                var orderedObject1 = orderFunc(expectedObject);
                var orderedObject2 = orderFunc(currentObject);

                var object1AsJson = parameterReplacementService.ResolveParameters(orderedObject1.ToJson(jsonSerializerOptions),
                                                                                  parameters);

                var object2AsJson = parameterReplacementService.ResolveParameters(orderedObject2.ToJson(jsonSerializerOptions),
                                                                                  parameters);

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
                                                                             object2AsJson, differenceOutputTable, curl);

                if (shouldWriteResponse)
                {
                    var writeResponseRequest = new WriteResponseRequest()
                                               {
                                                   CallingAssembly = callingAssembly,
                                                   DifferenceFunc = differenceFunc,
                                                   CurrentResponseAsString = object2AsJson,
                                                   ExpectedResult = localizedExpectedResponseFile,
                                                   Parameters = parameters,
                                                   Mode = ResponseWriteMode.DifferencesOnly
                                               };

                    responseWriter.Write(writeResponseRequest);
                }

                Assert.IsFalse(hasSchemaMismatch, schemaNotMatchingError);

                var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var output = outputFormatter.GetOutputString(title, 
                                                             $"Detected differences: {optimizedDifferences.Count}", 
                                                             object1AsJson,
                                                             object2AsJson,
                                                             resultTable, 
                                                             curl);

                if (optimizedDifferences.Any())
                {
                    Assert.Fail(output);
                }

                curlPrinter.PrintCurl(callingAssembly, curl);
            }
        }
    }
}
