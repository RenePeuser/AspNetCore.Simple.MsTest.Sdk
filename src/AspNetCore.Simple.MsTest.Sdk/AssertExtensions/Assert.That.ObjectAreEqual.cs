using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static class AssertObjectExtensions
    {
        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();
        private static readonly JsonDiffer JsonDiffer = new();
        private static readonly CurlFormatter CurlFormatter = new();
        private static readonly CurlPrinter CurlPrinter = new(CurlFormatter);
        private static readonly OutputFormatter OutputFormatter = new(CurlFormatter);


        public static Func<IImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = item => item;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, input => input,
                                   string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, input => input,
                                   string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              string title,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, input => input, title, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, input => input, title, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, item => item,
                                   string.Empty, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              (string Key, object? Value)[] parameters,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, item => item,
                                   string.Empty, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   title, difference => difference, expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   title, difference => difference, expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   string.Empty, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   string.Empty, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   title, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc,
                                   title, differenceFunc, string.Empty,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, comparisonFunc,
                                   title, differenceFunc, curl,
                                   [], expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            var orderedExpectedObject = comparisonFunc(expectedObject);
            var orderedCurrentObject = comparisonFunc(currentObject);

            var json1 = orderedExpectedObject.ToJson();
            var json2 = orderedCurrentObject.ToJson();

            foreach (var valueTuple in parameters)
            {
                json1 = json1.Replace(valueTuple.Key, valueTuple.Value?.ToString());
                json2 = json2.Replace(valueTuple.Key, valueTuple.Value?.ToString());
            }

            var differences = JsonDiffer.FindDifferences(json1, json2);
            
            
            var commonDifferences = DifferenceFunc(differences).ToImmutableList();
            var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

            var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

            if (optimizedDifferences.Any())
            {
                var output = OutputFormatter.GetOutputString(resultTable, json1, json2, title, curl);
                Assert.Fail(output);
            }
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item,
                                   Assembly.GetCallingAssembly(), expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item,
                                   Assembly.GetCallingAssembly(), parameters, expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   callingAssembly, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   callingAssembly, parameters, expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   title, Assembly.GetCallingAssembly(), difference => difference,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   title, Assembly.GetCallingAssembly(), difference => difference,
                                   parameters, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   title, callingAssembly, difference => difference,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item,
                                   title, callingAssembly, difference => difference,
                                   parameters, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   string.Empty, Assembly.GetCallingAssembly(), difference => difference,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   string.Empty, Assembly.GetCallingAssembly(), difference => difference,
                                   parameters, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item,
                                   string.Empty, Assembly.GetCallingAssembly(), differenceFunc,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item,
                                   string.Empty, Assembly.GetCallingAssembly(), differenceFunc,
                                   parameters, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   string.Empty, callingAssembly, difference => difference,
                                   expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   string.Empty, callingAssembly, difference => difference,
                                   parameters, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   title, callingAssembly, differenceFunc,
                                   string.Empty, [], expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc,
                                   title, callingAssembly, differenceFunc,
                                   string.Empty, parameters, expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            var jsonObject = expectedObjectAsJson.GetJsonString<T>(callingAssembly);
            jsonObject = jsonObject.ResolveParameters(parameters);

            // This is most the use case when calling an API and want to know what comes back
            if (expectedObjectAsJson is "{}" or "[]")
            {
                var output = OutputFormatter.GetOutputString(title,
                                                             $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                             jsonObject,
                                                             currentObject.ToJson(),
                                                             curl);
                Assert.Fail(output);
            }

            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
            {
                var expectedResult = PrimitiveTypeConverter.ConvertTo<T>(jsonObject);
                var output = OutputFormatter.GetOutputString(title, jsonObject, currentObject.ToJson());

                Assert.AreEqual(expectedResult, currentObject, output);

                CurlPrinter.PrintCurl(callingAssembly, curl);
            }
            else
            {
                T? expectedObject = default;

                try
                {
                    expectedObject = JsonSerializer.Deserialize<T>(jsonObject, JsonSerializerOptions);
                }
                catch (Exception)
                {
                    var cantSerializeJsonErrorOutput = OutputFormatter.GetOutputString(title,
                                                                                       $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                       jsonObject,
                                                                                       currentObject.ToJson(),
                                                                                       CurlFormatter.GetCurlAsFormattedString(curl));

                    Assert.Fail(cantSerializeJsonErrorOutput);
                }

                var serializeResultIsNullOutput = OutputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                  jsonObject.ToJson(),
                                                                                  null,
                                                                                  CurlFormatter.GetCurlAsFormattedString(curl));

                Assert.IsNotNull(expectedObject, serializeResultIsNullOutput);

                var orderedObject1 = orderFunc(expectedObject);
                var orderedObject2 = orderFunc(currentObject);

                var jsonDiffer = new JsonDiffer();

                var object1AsJson = orderedObject1.ToJson().ResolveParameters(parameters);
                var object2AsJson = orderedObject2.ToJson().ResolveParameters(parameters);

                var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

                // 1. Check if we are comparing the sam schema
                var schemaNotMatching = differences.Any() && differences.All(item => item.Value1.IsNull() || item.Value2.IsNull());

                var schemaNotMatchingError = OutputFormatter.GetOutputString(object1AsJson, object2AsJson, title, curl);
                Assert.IsFalse(schemaNotMatching, schemaNotMatchingError);

                var commonDifferences = DifferenceFunc(differences).ToImmutableList();
                var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

                var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var output = OutputFormatter.GetOutputString(title, $"Detected differences: {optimizedDifferences.Count}", object1AsJson, object2AsJson, resultTable, curl);
                Assert.IsTrue(optimizedDifferences.IsEmpty(), output);

                CurlPrinter.PrintCurl(callingAssembly, curl);
            }
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
