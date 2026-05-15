using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static partial class AssertObjectExtensions
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
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public static Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = item => item;

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new EmbeddedFileLocalizer(new TestCreatorSettings(), JsonSerializerOptions);

        private static readonly Serializer.Json.JsonSerializer JsonSerializer = new(JsonSerializerOptions);

        // Text decorator - conditional on build configuration (default fallback)
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

        private static readonly IAssertOutputStrategy[] OutputStrategies =
        [
            PrimitiveOutputStrategy,
            ObjectOutputStrategy
        ];

        private static readonly AssertOutputBuilder OutputBuilder = new(OutputStrategies);

        // Comparison strategies (order matters - first match wins)
        private static readonly ISpecificComparisonStrategy StringComparisonStrategy = new StringComparisonStrategy();

        private static readonly ISpecificComparisonStrategy JsonComparisonStrategy = new JsonComparisonStrategy(JsonDiffer, JsonSerializer, JsonSerializerOptions);

        private static readonly ISpecificComparisonStrategy[] SpecificComparisonStrategies =
        [
            StringComparisonStrategy,
            JsonComparisonStrategy
        ];

        private static readonly IComparisonStrategy ComparisonStrategy = new ComparisonStrategy(SpecificComparisonStrategies);

        private static readonly AssertService AssertService = new(ComparisonStrategy,
                                                                  ResponseWriter,
                                                                  WriteResponseService,
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: input => input,
                                   title: string.Empty,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: input => input,
                                   title: title,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: input => input,
                                   title: title,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject,
                                   currentObject,
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: comparisonFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: curl,
                                   parameters: [],
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: comparisonFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: curl,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            // Optimization: Pass the expected object directly to avoid unnecessary serialize → deserialize → serialize cycles
            // Only serialize to JSON for string types (which need string representation)
            var expectedObjectAsJson = typeof(T) == typeof(string)
                                           ? expectedObject?.ToString() ?? string.Empty
                                           : string.Empty;

            // For embedded file resolution (used in FromFile overloads)
            var expectedFile = EmbeddedFileLocalizer.LocalizeResponseFile(expectedObjectAsJson, callerFilePath, callingAssembly);

            // Resolve parameters (no-op if expectedObjectAsJson is empty)
            var resolvedExpectedJson = ParameterReplacer.ResolveParameters(expectedFile.Content, parameters);

            var targetIsPrimitiveType = typeof(T).IsPrimitive || typeof(T).EqualsTo(typeof(string));

            // Create context with the expected object directly - avoids serialization roundtrip
            var context = new ObjectAssertContext<T>
            {
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber,
                CallerMemberName = callerMemberName,
                CallingAssembly = callingAssembly,
                Current = currentObject,
                CurrentObject = currentObject,
                CurrentResultParameterName = currentResultParameterName,
                DifferenceFunc = differenceFunc,
                Expected = expectedObject, // Direct object reference - no serialization needed
                ExpectedType = typeof(T),
                ExpectedObjectAsJson = expectedObjectAsJson,
                ExpectedResultFile = expectedFile,
                ExpectedResultParameterName = expectedResultParameterName,
                OrderFunc = comparisonFunc,
                Parameters = parameters,
                ResolvedExpectedJson = resolvedExpectedJson,
                TypeIsPrimitiveType = targetIsPrimitiveType,
                WriteResponse = writeResponse,
            };

            ObjectsAreEqual(assert, context);
        }

        // Context-based implementation (internal)
        // ============================================================

        public static void ObjectsAreEqual<T>(this Assert _,
                                              ObjectAssertContext<T> context)
        {
            // Check if calling assembly is in debug mode - if so, use PlainTextDecorator
            var useDebugDecorator = context.CallingAssembly.IsCompiledInDebug();

            if (useDebugDecorator)
            {
                // Create debug-specific builders and service
                var plainTextDecorator = new PlainTextDecorator();
                var debugDifferencesTableBuilder = new DifferencesTableBuilder(plainTextDecorator);
                var debugJsonSectionBuilder = new JsonSectionBuilder(plainTextDecorator);
                var debugObjectOutputStrategy = new ObjectOutputStrategy(debugDifferencesTableBuilder, debugJsonSectionBuilder);
                var debugOutputStrategies = new IAssertOutputStrategy[]
                {
                    PrimitiveOutputStrategy,
                    debugObjectOutputStrategy
                };
                var debugOutputBuilder = new AssertOutputBuilder(debugOutputStrategies);
                var debugAssertService = new AssertService(ComparisonStrategy, ResponseWriter, WriteResponseService, debugOutputBuilder);

                // Use debug service
                debugAssertService.ObjectsAreEqual(context);
            }
            else
            {
                // Use the default (release) service
                AssertService.ObjectsAreEqual(context);
            }
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
