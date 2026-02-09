using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;

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

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new EmbeddedFileLocalizer(new TestCreatorSettings());

        private static readonly CurrentResponseWriter CurrentResponseWriter = new CurrentResponseWriter(JsonDiffer);

        private static readonly WriteResponseService WriteResponseService = new WriteResponseService();

        // You have the possible to set and pass the api settings specific json options
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public static Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = item => item;

        // GlobalWriteResponse
        // NEW Env variable WriteResponse = true -> For Ai Usage
        public static bool WriteResponse { get; set; }

        public static bool ResponseFileFullPath { get; set; }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   [],
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   title: string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              string title,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   input => input,
                                   title,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   input => input,
                                   title,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   item => item,
                                   string.Empty,
                                   differenceFunc,
                                   string.Empty,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              (string Key, object? Value)[] parameters,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   item => item,
                                   string.Empty,
                                   differenceFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   difference => difference,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   difference => difference,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   differenceFunc,
                                   string.Empty,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   differenceFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   differenceFunc,
                                   string.Empty,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   differenceFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc,
                                   title,
                                   differenceFunc,
                                   curl,
                                   [],
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc,
                                   title,
                                   differenceFunc,
                                   curl,
                                   parameters,
                                   Assembly.GetCallingAssembly(),
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              Assembly callingAssembly,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObject))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            var orderedExpectedObject = comparisonFunc(expectedObject);
            var orderedCurrentObject = comparisonFunc(currentObject);

            var json1 = orderedExpectedObject.ToJson(JsonSerializerOptions);
            var json2 = orderedCurrentObject.ToJson(JsonSerializerOptions);

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
                var output = OutputFormatter.GetOutputString($"Differences detected between your current:{currentResultParameterName} and expected result: {expectedResultParameterName}", resultTable, json1, json2,
                                                             title ?? $"Differences detected between your current:{currentResultParameterName} and expected result: {expectedResultParameterName}", curl);

                Assert.Fail(output);
            }
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFile = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   item => item,
                                   Assembly.GetCallingAssembly(),
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   item => item,
                                   Assembly.GetCallingAssembly(),
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   title,
                                   Assembly.GetCallingAssembly(),
                                   difference => difference,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   title,
                                   Assembly.GetCallingAssembly(),
                                   difference => difference,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   title,
                                   callingAssembly,
                                   difference => difference,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentResult,
                                              string title,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentResult))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   title,
                                   callingAssembly,
                                   difference => difference,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   Assembly.GetCallingAssembly(),
                                   difference => difference,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   Assembly.GetCallingAssembly(),
                                   difference => difference,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   item => item,
                                   string.Empty,
                                   Assembly.GetCallingAssembly(),
                                   differenceFunc,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   Assembly.GetCallingAssembly(),
                                   differenceFunc,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   item => item,
                                   string.Empty,
                                   Assembly.GetCallingAssembly(),
                                   differenceFunc,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   callingAssembly,
                                   difference => difference,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   callingAssembly,
                                   difference => difference,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   callingAssembly,
                                   differenceFunc,
                                   string.Empty,
                                   [],
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   callingAssembly,
                                   differenceFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            // Special case if expected and current jsons are parameters passed by we need to set the
            // correct parameter names
            var expectedResponseFile = EmbeddedFileLocalizer.LocalizeResponseFile(expectedObjectAsJson, callerFilePath, callingAssembly);

            if (expectedObjectAsJson.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                expectedResultParameterName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                expectedResultParameterName = expectedObjectAsJson;
            }

            // localize expected response and payload
            // So the caller does not have to pass the unique file name of the embedded resource
            // - Api.V1.Users.GetAllUsersTest.Responses.GetAllUsersResponse.json
            // - GetAllUsersResponse.json
            var localizedExpectedResponseFile = EmbeddedFileLocalizer.LocalizeResponseFile(expectedResultParameterName, callerFilePath, callingAssembly);
            if (localizedExpectedResponseFile.EmbeddedFile.IsNull())
            {
                localizedExpectedResponseFile = localizedExpectedResponseFile with
                {
                    Content = expectedObjectAsJson
                };
            }

            // This is most the use case when calling an API and want to know what comes back
            var currentObjectAsJson = currentObject.ToJson(JsonSerializerOptions);

            var jsonObject = localizedExpectedResponseFile.Content.GetJsonStringFrom<T>(currentObjectAsJson,
                                                                                        callingAssembly,
                                                                                        curl,
                                                                                        currentResultParameterName);

            jsonObject = jsonObject.ResolveParameters(parameters);

            // Brand new crazy function
            // We write the current result to the expected file
            if (WriteResponseService.ShouldWriteResponse(writeResponse, callingAssembly))
            {
                CurrentResponseWriter.Write(currentObjectAsJson,
                                            expectedResponseFile,
                                            parameters,
                                            differenceFunc,
                                            callingAssembly);
            }

            var type = typeof(T);

            if (type.IsPrimitive || type.EqualsTo(typeof(string)))
            {
                var expectedResult = PrimitiveTypeConverter.ConvertTo<T>(jsonObject);
                var output = OutputFormatter.GetOutputString(title, jsonObject, currentObjectAsJson);

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
#pragma warning disable CA1031
                catch (Exception e)
#pragma warning restore CA1031
                {
                    var cantSerializeJsonErrorOutput = OutputFormatter.GetOutputString(title,
                                                                                       $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}. Exception: {e.Message}",
                                                                                       jsonObject,
                                                                                       currentObjectAsJson,
                                                                                       CurlFormatter.GetCurlAsFormattedString(curl));

                    Assert.Fail(cantSerializeJsonErrorOutput);
                }

                var serializeResultIsNullOutput = OutputFormatter.GetOutputString($"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                  jsonObject.ToJson(JsonSerializerOptions),
                                                                                  null,
                                                                                  CurlFormatter.GetCurlAsFormattedString(curl));

                Assert.IsNotNull(expectedObject, serializeResultIsNullOutput);

                var orderedObject1 = orderFunc(expectedObject);
                var orderedObject2 = orderFunc(currentObject);

                var object1AsJson = orderedObject1.ToJson(JsonSerializerOptions)
                                                  .ResolveParameters(parameters);

                var object2AsJson = orderedObject2.ToJson(JsonSerializerOptions)
                                                  .ResolveParameters(parameters);

                var differences = JsonDiffer.FindDifferences(object1AsJson, object2AsJson);

                // 1. Check if we are comparing the same schema
                var hasSchemaMismatch = differences.Any(item => item.MismatchType.NotEqualsTo(MismatchType.ValueDifference));

                var contentValueDifferences = differences.FirstOrDefault(d => d.MemberPath.Equals("Content.Value", StringComparison.OrdinalIgnoreCase));
                if (contentValueDifferences.IsNotNull())
                {
                    differences = JsonDiffer.FindDifferences(contentValueDifferences.Value1 ?? string.Empty, contentValueDifferences.Value2 ?? string.Empty);
                    hasSchemaMismatch = differences.Any(item => item.MismatchType is MismatchType.MissingInFirst or MismatchType.MissingInSecond);
                }


                var commonDifferences = DifferenceFunc(differences).ToImmutableList();
                var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

                var differenceOutputTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var schemaNotMatchingError = OutputFormatter.GetOutputString(title, "Schema mismatch: Expected result and current result does not match", object1AsJson,
                                                                             object2AsJson, differenceOutputTable, curl);

                if (WriteResponseService.ShouldWriteResponse(writeResponse, callingAssembly))
                {
                    CurrentResponseWriter.Write(object2AsJson ?? "{}",
                                                localizedExpectedResponseFile,
                                                parameters,
                                                differenceFunc,
                                                callingAssembly);
                }

                Assert.IsFalse(hasSchemaMismatch, schemaNotMatchingError);

                var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var output = OutputFormatter.GetOutputString(title, $"Detected differences: {optimizedDifferences.Count}", object1AsJson,
                                                             object2AsJson, resultTable, curl);

                Assert.IsTrue(optimizedDifferences.IsEmpty(), output);

                CurlPrinter.PrintCurl(callingAssembly, curl);
            }
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
