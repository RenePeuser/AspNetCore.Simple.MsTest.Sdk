using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static class AssertObjectExtensions
    {
        private static readonly JsonDiffer JsonDiffer = new();

        private static readonly ParameterReplacer ParameterReplacer = new();

        private static readonly ResponseWriter ResponseWriter = new ResponseWriter([
                                                                                       new DifferenceResponseWriter(JsonDiffer, new JsonPathWriter(), ParameterReplacer),
                                                                                       new OverwriteAllResponseWriter(ParameterReplacer)
                                                                                   ]);

        private static readonly WriteResponseService WriteResponseService = new WriteResponseService();

        // You have the possible to set and pass the api settings specific json options
        public static JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
                                                                                  {
                                                                                      PropertyNameCaseInsensitive = true,
                                                                                      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                                      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                                                                      NumberHandling = JsonNumberHandling.AllowReadingFromString,
                                                                                      Converters = { new JsonStringEnumConverter() }
                                                                                  };

        public static Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = item => item;

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new EmbeddedFileLocalizer(new TestCreatorSettings(), JsonSerializerOptions);

        private static readonly Serializer.Json.JsonSerializer JsonSerializer = new(JsonSerializerOptions);

        // Text decorator - conditional on build configuration
#if DEBUG
        private static readonly ITextDecorator TextDecorator = new PlainTextDecorator();
#else
        private static readonly ITextDecorator TextDecorator = new AnsiColorTextDecorator();
#endif

        // Builders for output strategies
        private static readonly DifferencesTableBuilder DifferencesTableBuilder = new(TextDecorator);

        private static readonly JsonSectionBuilder JsonSectionBuilder = new(TextDecorator);

        // Output strategies for AssertService
        private static readonly PrimitiveOutputStrategy PrimitiveOutputStrategy = new();

        private static readonly ObjectOutputStrategy ObjectOutputStrategy = new(DifferencesTableBuilder, JsonSectionBuilder);

        private static readonly IAssertOutputStrategy[] OutputStrategies = [PrimitiveOutputStrategy, ObjectOutputStrategy];

        private static readonly AssertOutputBuilder OutputBuilder = new(OutputStrategies);

        private static readonly AssertService AssertService = new(JsonDiffer,
                                                                  ResponseWriter,
                                                                  WriteResponseService,
                                                                  JsonSerializer,
                                                                  JsonSerializerOptions,
                                                                  OutputBuilder);

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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   [],
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   title: string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   input => input,
                                   title,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   input => input,
                                   title,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   orderFunc,
                                   title,
                                   difference => difference,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            // Convert expected object to JSON and delegate to string-based method
            // This ensures consistent data preprocessing through the main pipeline
            var expectedObjectAsJson = expectedObject.ToJson(JsonSerializerOptions);

            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   comparisonFunc,
                                   title,
                                   callingAssembly,
                                   differenceFunc,
                                   curl,
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath, callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentObject,
                                   item => item,
                                   Assembly.GetCallingAssembly(),
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            assert.ObjectsAreEqual(expectedObjectAsJson,
                                   currentResult,
                                   item => item,
                                   callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              Assembly callingAssembly,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              Assembly callingAssembly,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
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
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
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
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "")
        {
            // Resolve embedded files once here - this avoids duplicate resolution later in the pipeline
            var expectedFile = EmbeddedFileLocalizer.LocalizeResponseFile(expectedObjectAsJson, callerFilePath, callingAssembly);

            // Resolve parameters in expected JSON once here - ready-to-use for comparison
            var resolvedExpectedJson = ParameterReplacer.ResolveParameters(expectedFile.Content, parameters);

            var targetIsPrimitiveType = typeof(T).IsPrimitive || typeof(T).EqualsTo(typeof(string));

            // Create context with preprocessed data - no further logic needed in AssertService
            var context = new ObjectAssertContext<T>
                          {
                              CallerFilePath = callerFilePath,
                              CallerMemberName = callerMemberName,
                              CallingAssembly = callingAssembly,
                              Current = currentObject,
                              CurrentObject = currentObject,
                              CurrentResultParameterName = currentResultParameterName,
                              DifferenceFunc = differenceFunc,
                              ExpectedObjectAsJson = expectedObjectAsJson,
                              ExpectedResultFile = expectedFile,
                              ExpectedResultParameterName = expectedResultParameterName,
                              OrderFunc = orderFunc,
                              Parameters = parameters,
                              ResolvedExpectedJson = resolvedExpectedJson,
                              TypeIsPrimitiveType = targetIsPrimitiveType,
                              WriteResponse = writeResponse,
                          };

            ObjectsAreEqual(assert, context);
        }

        // ============================================================
        // Context-based implementation (internal)
        // ============================================================

        public static void ObjectsAreEqual<T>(this Assert _,
                                              ObjectAssertContext<T> context)
        {
            // Delegate to the DI-based AssertService for the actual implementation
            // This keeps the static extension method as a thin wrapper for backward compatibility
            AssertService.ObjectsAreEqual(context);
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
