using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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
        internal static readonly JsonDiffer JsonDiffer = new();
        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, string.Empty);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              string title) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, title);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              string title,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, input => input, title, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<T?, T?> orderFunc) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<T?, T?> orderFunc,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, item => item, string.Empty, differenceFunc, string.Empty);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, item => item, string.Empty, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<T?, T?> orderFunc,
                                              string title) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty, difference => difference);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, title, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty, differenceFunc, string.Empty);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, string.Empty, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, title, differenceFunc, string.Empty);
        }
        
        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(object1, object2, orderFunc, title, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<T?>> object1,
                                              Expression<Func<T?>> object2,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl) where T : class
        {
            var obj1 = orderFunc(object1.Compile()());
            var obj2 = orderFunc(object2.Compile()());

            var json1 = obj1.ToJson();
            var json2 = obj2.ToJson();

            var differences = JsonDiffer.FindDifferences(json1, json2);
            var optimizedDifferences = differenceFunc(differences).ToImmutableList();

            var resultTable = optimizedDifferences.ToResultTable(object1.NameOf(), object2.NameOf());

            Assert.IsTrue(optimizedDifferences.IsEmpty(), GetOutputString(resultTable, obj1!, obj2!, title, string.Empty));
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? object1,
                                              T? object2,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              [CallerArgumentExpression(nameof(object1))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            var json1 = object1.ToJson();
            var json2 = object2.ToJson();

            var differences = JsonDiffer.FindDifferences(json1, json2);
            var optimizedDifferences = differenceFunc(differences).ToImmutableList();

            var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

            Assert.IsTrue(optimizedDifferences.IsEmpty(), GetOutputString(resultTable, object1!, object2!, title, string.Empty));
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> objectExpression) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, Assembly.GetCallingAssembly());
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T objectExpression,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(objectExpression))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, Assembly.GetCallingAssembly(), expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> objectExpression,
                                              Assembly callingAssembly) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, callingAssembly);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, currentResult, item => item, callingAssembly, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> objectExpression,
                                              string title) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, title, Assembly.GetCallingAssembly(), difference => difference);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T currentResult,
                                              string title,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, currentResult, item => item, title, Assembly.GetCallingAssembly(), difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> objectExpression,
                                              string title,
                                              Assembly callingAssembly) where T : class
        {
            assert.ObjectsAreEqual(json, objectExpression, item => item, title, callingAssembly, difference => difference);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, currentResult, item => item, title, callingAssembly, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> object2,
                                              Func<T, T> orderFunc) where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, string.Empty, Assembly.GetCallingAssembly(), difference => difference);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T object2,
                                              Func<T, T> orderFunc,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, string.Empty, Assembly.GetCallingAssembly(), difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> object2,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where T : class
        {
            assert.ObjectsAreEqual(json, object2, item => item, string.Empty, Assembly.GetCallingAssembly(), differenceFunc);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T object2,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, object2, item => item, string.Empty, Assembly.GetCallingAssembly(), differenceFunc, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> object2,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly) where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, string.Empty, callingAssembly, difference => difference);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T object2,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, string.Empty, callingAssembly, difference => difference, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> object2,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, title, callingAssembly, differenceFunc, string.Empty);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T object2,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            assert.ObjectsAreEqual(json, object2, orderFunc, title, callingAssembly, differenceFunc, string.Empty, expectedResultParameterName, currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              Expression<Func<string>> json,
                                              Expression<Func<T>> object2,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl) where T : class
        {
            T? object1;
            var jsonSource = json.Compile()();
            var jsonObject = jsonSource.GetJsonString(callingAssembly);

            try
            {
                object1 = JsonSerializer.Deserialize<T>(jsonObject, JsonSerializerOptions);
            }
            catch (Exception)
            {
                throw new DeserializeException($"The given json for: '{json.NameOf()}' was not possible to convert into type: {typeof(T).Name}. Json was:{Environment.NewLine}{Environment.NewLine}{jsonSource}");
            }

            if (object1 is null)
            {
                throw new InvalidOperationException("Expected object is null. This is not allowed for comparison as source object");
            }


            var obj2 = object2.Compile()();
            var orderedObject1 = orderFunc(object1);
            var orderedObject2 = orderFunc(obj2);

            var jsonDiffer = new JsonDiffer();
            var object1AsJson = orderedObject1.ToJson();
            var object2AsJson = orderedObject2.ToJson();

            var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

            // 1. Check if we are comparing the sam schema
            var schemaNotMatching = differences.Any() && differences.All(item => item.Value1.IsNull() || item.Value2.IsNull());

            Assert.IsFalse(schemaNotMatching, GetSchemeNotMatching(title, object1AsJson, object2AsJson));

            var optimizedDifferences = differenceFunc(differences).ToImmutableList();

            var expectedValueName = jsonSource.EndsWith(".json", StringComparison.InvariantCulture) ? jsonSource : json.NameOf();

            var resultTable = optimizedDifferences.ToResultTable(expectedValueName, object2.NameOf());

            PrintCurl(callingAssembly, curl);

            Assert.IsTrue(optimizedDifferences.IsEmpty(), GetOutputString(resultTable, orderedObject1, orderedObject2, title, curl));
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string json,
                                              T object2,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              [CallerArgumentExpression(nameof(json))] string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(object2))] string currentResultParameterName = "") where T : class
        {
            T? object1;

            var jsonObject = json.GetJsonString(callingAssembly);

            try
            {
                object1 = JsonSerializer.Deserialize<T>(jsonObject, JsonSerializerOptions);
            }
            catch (Exception)
            {
                throw new DeserializeException($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}. Json was:{Environment.NewLine}{Environment.NewLine}{jsonObject}");
            }

            if (object1 is null)
            {
                throw new InvalidOperationException("Expected object is null. This is not allowed for comparison as source object");
            }


            var orderedObject1 = orderFunc(object1);
            var orderedObject2 = orderFunc(object2);

            var jsonDiffer = new JsonDiffer();
            var object1AsJson = orderedObject1.ToJson();
            var object2AsJson = orderedObject2.ToJson();

            var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

            // 1. Check if we are comparing the sam schema
            var schemaNotMatching = differences.Any() && differences.All(item => item.Value1.IsNull() || item.Value2.IsNull());

            Assert.IsFalse(schemaNotMatching, GetSchemeNotMatching(title, object1AsJson, object2AsJson));

            var optimizedDifferences = differenceFunc(differences).ToImmutableList();

            var expectedValueName = json.EndsWith(".json", StringComparison.InvariantCulture) ? json : expectedResultParameterName;

            var resultTable = optimizedDifferences.ToResultTable(expectedValueName, currentResultParameterName);

            PrintCurl(callingAssembly, curl);

            Assert.IsTrue(optimizedDifferences.IsEmpty(), GetOutputString(resultTable, orderedObject1, orderedObject2, title, curl));
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

        private static string GetOutputString(string resultTable,
                                              object expectedResult,
                                              object current,
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

            stringBuilder.AppendLine(resultTable);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Current result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(currentResultAsJson);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Expected result:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(expectedResultAsJson);
            stringBuilder.AppendLine();

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
