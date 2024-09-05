using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static class AssertObjectExtensions
    {
        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();
        private static readonly JsonDiffer JsonDiffer = new();
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, input => input, string.Empty, expectedResultParameterName, currentResultParameterName);
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
                                              Func<T?, T?> orderFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, item => item, string.Empty, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc, title, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc, string.Empty, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObject))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObject, currentObject, orderFunc, title, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
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
            
            var orderedExpectedObject = comparisonFunc(expectedObject);
            var orderedCurrentObject = comparisonFunc(currentObject);
            
            var json1 = orderedExpectedObject.ToJson();
            var json2 = orderedCurrentObject.ToJson();

            var differences = JsonDiffer.FindDifferences(json1, json2);
            var optimizedDifferences = differenceFunc(differences).ToImmutableList();

            var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);
            if (optimizedDifferences.Any())
            {
                Assert.Fail(GetOutputString(resultTable, expectedObject!, currentObject!, title, string.Empty));
            }
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item, Assembly.GetCallingAssembly(), expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item, callingAssembly, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item, title, Assembly.GetCallingAssembly(), difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentResult, item => item, title, callingAssembly, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc, string.Empty, Assembly.GetCallingAssembly(), difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, item => item, string.Empty, Assembly.GetCallingAssembly(), differenceFunc, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc, string.Empty, callingAssembly, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson, currentObject, orderFunc, title, callingAssembly, differenceFunc, string.Empty, [], expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, string Value)[] parameters,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))] string currentResultParameterName = "")
        {
            var jsonObject = expectedObjectAsJson.GetJsonString<T>(callingAssembly);
            var type = typeof(T);
            if (type.IsPrimitive || type == typeof(string))
            {
                var expectedResult = PrimitiveTypeConverter.ConvertTo<T>(jsonObject);

                Assert.AreEqual(expectedResult, currentObject, GetOutputString(title, curl, currentObject, expectedResult));
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
                    Assert.Fail(GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}", curl, currentObject.ToJson(), jsonObject));
                }

                Assert.IsNotNull(expectedObject, GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}", curl, default, jsonObject));


                var orderedObject1 = orderFunc(expectedObject);
                var orderedObject2 = orderFunc(currentObject);

                var jsonDiffer = new JsonDiffer();
                
                
                var object1AsJson = orderedObject1.ToJson();
                var object2AsJson = orderedObject2.ToJson();

                parameters.ForEach(p =>
                                   {
                                       if (object1AsJson.IsNotNullOrWhiteSpace())
                                       {
                                           object1AsJson = object1AsJson.Replace(p.Key, p.Value);
                                       }

                                       if (object2AsJson.IsNotNullOrWhiteSpace())
                                       {
                                           object2AsJson = object2AsJson.Replace(p.Key, p.Value);
                                       }
                                   });
                
                
                var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

                // 1. Check if we are comparing the sam schema
                var schemaNotMatching = differences.Any() && differences.All(item => item.Value1.IsNull() || item.Value2.IsNull());

                Assert.IsFalse(schemaNotMatching, GetSchemeNotMatching(title, object1AsJson, object2AsJson));

                var optimizedDifferences = differenceFunc(differences).ToImmutableList();

                var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                PrintCurl(callingAssembly, curl);

                Assert.IsTrue(optimizedDifferences.IsEmpty(), GetOutputString(resultTable, orderedObject1, orderedObject2, title, curl));
            }
        }

        internal static void PrintCurl(Assembly callingAssembly, string curl)
        {
            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            if (curl.IsNotNullOrWhiteSpace())
            {
                var maxLength = curl.Split(Environment.NewLine).Max(line => line.Length);
                var separator = maxLength.Times(() => "-").Flatten();

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(separator);
                stringBuilder.AppendLine("Http call as curl");
                stringBuilder.AppendLine(separator);
                stringBuilder.AppendLine(curl);
                stringBuilder.AppendLine(separator);
                var curlOutput = stringBuilder.ToString();

                HttpClientAssertExtensions.LogAction(curlOutput);
            }
        }

        private static string GetOutputString<T>(string title,
                                                 string curl,
                                                 T? currentResult,
                                                 T? expectedResult)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();

            if (title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(currentResult.ToJson());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(expectedResult.ToJson());
            stringBuilder.AppendLine();

            if (curl.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(curl);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private static string GetOutputString(string resultTable,
                                              object? expectedResult,
                                              object? current,
                                              string title,
                                              string curl)
        {
            var expectedResultAsJson = expectedResult.ToJson();
            var currentResultAsJson = current.ToJson();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();

            if (title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(resultTable);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(expectedResultAsJson);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(currentResultAsJson);
            stringBuilder.AppendLine();

            if (curl.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(curl);
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        private static string GetSchemeNotMatching(string title,
                                                   string json1,
                                                   string json2)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();

            if (title.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(title);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("--------------------------------------------------------");
            stringBuilder.AppendLine("! The schemas of the objects to compare does not match !");
            stringBuilder.AppendLine("--------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(json2);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(json1);
            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
