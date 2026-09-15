using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable IDE0060 // Remove unused parameter
    public static partial class AssertObjectExtensions
    {
        private static readonly JsonDiffer JsonDiffer = new();

        private static readonly ParameterReplacer ParameterReplacer = new();

        private static readonly ResponseWriter ResponseWriter = new ResponseWriter([
                                                                                       new DifferenceResponseWriter(JsonDiffer, new JsonPathWriter(), ParameterReplacer, new SnapshotPlaceholderGuard()),
                                                                                       new OverwriteAllResponseWriter(ParameterReplacer, new SnapshotPlaceholderGuard())
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

        /// <summary>
        /// Global per-difference predicate. Return <c>true</c> to keep a difference,
        /// <c>false</c> to ignore it. The SDK iterates internally, so you only describe
        /// the condition (e.g. <c>d => d.MemberPath != "id"</c>) instead of writing a loop.
        /// Applied in addition to (and after) <see cref="DifferenceFunc"/> and any
        /// per-assert filter. Defaults to keeping every difference.
        /// </summary>
        public static Predicate<Difference> DifferenceFilter { get; set; } = _ => true;

        /// <summary>
        /// Global predicate marking arrays whose element ORDER carries no meaning - an OpenAPI
        /// <c>anyOf</c>, a set of tags, anything a producer emits in a different order per run.
        /// Their elements are MATCHED against each other instead of compared index by index, so a
        /// pure reordering is no longer a difference while a missing or changed element still is.
        /// Neither document is reordered, which keeps every reported path pointing at the element
        /// it names.
        /// <para>
        /// Decide per array, not per bare name: <c>array => array.PropertyName is "anyOf"</c> makes
        /// EVERY anyOf order blind, <c>array => array.Path == "components.schemas.Pet.anyOf"</c>
        /// only that one. <see cref="JsonArrayContext.Path"/> is index free.
        /// </para>
        /// <para>
        /// This is the escape hatch for payloads you do not control. When the type is yours, the
        /// per-assert <c>orderFunc</c> is the better tool: it is type safe and it also normalizes
        /// what gets WRITTEN into the snapshot. And when the producer's order is nondeterministic
        /// at all, every client sees that - fixing it at the source beats hiding it in the test.
        /// </para>
        /// Defaults to <c>null</c>, which compares every array by index.
        /// </summary>
        public static Predicate<JsonArrayContext>? OrderIndependentArrayFilter { get; set; }

        /// <summary>
        /// Applies the configured difference filtering to a set of raw differences:
        /// the global <see cref="DifferenceFunc"/>, the per-assert difference func,
        /// and finally the global + per-assert <see cref="DifferenceFilter"/> predicates
        /// (a difference is kept only when both predicates return <c>true</c>).
        /// </summary>
        internal static ImmutableList<Difference> ApplyDifferenceFiltering(ImmutableList<Difference> differences,
                                                                           Func<ImmutableList<Difference>, IEnumerable<Difference>> perAssertFunc,
                                                                           Predicate<Difference>? perAssertFilter)
        {
            var afterGlobalFunc = DifferenceFunc(differences).ToImmutableList();
            var afterFunc = perAssertFunc(afterGlobalFunc).ToImmutableList();

            return afterFunc.Where(difference => DifferenceFilter(difference) &&
                                                 (perAssertFilter?.Invoke(difference) ?? true))
                            .ToImmutableList();
        }

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new EmbeddedFileLocalizer(new TestSdkSettings(), JsonSerializerOptions, new PlainTextDecorator(),
                                                                                                        new SourceCodeExtractor(),
                                                                                                        new ResourceRootNamespaceResolver());

        private static readonly Serializer.Json.JsonSerializer JsonSerializer = new(JsonSerializerOptions);

        // The object route has neither a container nor a Setup call, so the consumer assembly is only
        // known per assert. The output stack is therefore built per assert in BuildAssertService -
        // cheap objects on a failure path, and no shared decorator state anywhere.
        private static readonly TextDecoratorProvider TextDecoratorProvider = new TextDecoratorProvider();

        private static readonly TableBuilder StaticTableBuilder = new();

        // Comparison strategies (order matters - first match wins)
        private static readonly ISpecificComparisonStrategy StringComparisonStrategy = new StringComparisonStrategy();

        // Reads JsonSerializerOptions per comparison, not once here: this field initializer runs long
        // before a test hands the SDK the api's options, and both sides of the diff have to be written
        // with the very options the api writes with.
        private static readonly ISpecificComparisonStrategy JsonComparisonStrategy = new JsonComparisonStrategy(JsonDiffer, JsonSerializer, () => JsonSerializerOptions);

        private static readonly ISpecificComparisonStrategy[] SpecificComparisonStrategies =
        [
            StringComparisonStrategy,
            JsonComparisonStrategy
        ];

        private static readonly IComparisonStrategy ComparisonStrategy = new ComparisonStrategy(SpecificComparisonStrategies);

        /// <summary>
        /// Builds the output stack for one assert, bound to the decorator the consumer assembly asks
        /// for. Constructing it here instead of in a static field is what keeps the plain-vs-ANSI
        /// choice a function of the caller rather than of how the sdk itself was compiled.
        /// </summary>
        private static AssertService BuildAssertService(ITextDecorator textDecorator)
        {
            var differencesTableBuilder = new DifferencesTableBuilder(StaticTableBuilder, textDecorator);
            var jsonSectionBuilder = new JsonSectionBuilder(textDecorator);

            var outputBuilder = new AssertOutputBuilder([
                                                            new PrimitiveOutputStrategy(textDecorator),
                                                            new ObjectOutputStrategy(differencesTableBuilder, jsonSectionBuilder, textDecorator)
                                                        ]);

            return new AssertService(ComparisonStrategy, ResponseWriter, WriteResponseService, outputBuilder);
        }

        /// <summary>
        /// The object route has no DI container, so the handlers that can serve a context without a
        /// request are wired up by hand - in the same order the container registers them, catch-all
        /// last. The http-only handlers are left out: they answer with an empty string for a plain
        /// object context anyway.
        /// </summary>
        private static readonly TestErrorHandlingStrategy ErrorHandlingStrategy =
            new TestErrorHandlingStrategy([
                                              new SnapshotNotFoundErrorHandler(TextDecoratorProvider, new SourceCodeExtractor()),
                                              new InvalidSnapshotJsonErrorHandler(TextDecoratorProvider),
                                              new DefaultErrorHandler()
                                          ]);

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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
                                   curl: curl,
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
                                              Func<T?, T?> comparisonFunc,
                                              string title,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   differenceFilter: differenceFilter,
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

        // ============================================================
        // Explicit calling assembly overloads
        //
        // Every overload above reads the caller through Assembly.GetCallingAssembly().
        // That breaks down as soon as the assert is not written straight into the test:
        // a shared helper, a base class or a wrapper in another assembly hands the SDK
        // its OWN assembly, and the embedded snapshot is then looked up in the wrong
        // manifest. These twins let that caller pass the test assembly along instead.
        // ============================================================

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: item => item,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: item => item,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: item => item,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: [],
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   comparisonFunc: orderFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Assembly callingAssembly,
                                              Predicate<Difference>? differenceFilter = null,
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
                                   callingAssembly: callingAssembly,
                                   differenceFilter: differenceFilter,
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
                                              Predicate<Difference>? differenceFilter = null,
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
                DifferenceFilter = differenceFilter ?? (static _ => true),
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
            try
            {
                // A snapshot reference that resolved to nothing must never travel further:
                // EmbeddedFileInfo.Content then still holds the file NAME, and the downstream lookup
                // matches manifest names by substring - "Persons.json" binds to "GetAllPersons.json"
                // and the test goes green against a foreign snapshot. The http route has guarded this
                // for a while; this route did not, which left the protection half armed.
                EnsureSnapshotReferenceIsUsable(context);

                ObjectsAreEqualInternal(PrepareForRecording(context));
            }
            catch (AssertFailedException)
            {
                // If assert failed, that's expected - rethrow
                throw;
            }
#pragma warning disable CA1031
            catch (Exception exception)
#pragma warning restore CA1031
            {
                // GLOBAL EXCEPTION HANDLER FOR OBJECT ASSERTIONS
                // The handlers are shared with the http route - a missing or broken snapshot reads the
                // same no matter which assert found it. Only the generic fallback differs, because
                // there is no request and no response to print here.
                var errorOutput = ErrorHandlingStrategy.HandleAsync(context, exception)
                                                       .GetAwaiter()
                                                       .GetResult();

                Assert.That.Fail(errorOutput.IsNullOrWhiteSpace()
                                     ? BuildObjectAssertionError(context, exception)
                                     : errorOutput);

                throw; // Never reached, but required for compiler
            }
        }

        private static void EnsureSnapshotReferenceIsUsable<T>(ObjectAssertContext<T> context)
        {
            SnapshotReferenceGuard.EnsureSnapshotExists(context.ExpectedResultFile,
                                                        context.ExpectedObjectAsJson,
                                                        context.ExpectedResultParameterName,
                                                        context.CallingAssembly,
                                                        WriteResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly));

            // A file that exists but is not parseable json must say so. Otherwise the shape checks look
            // at the first character only and report a structure mismatch for a plain syntax error.
            SnapshotReferenceGuard.EnsureParseable(context.ExpectedResultFile,
                                                   context.ResolvedExpectedJson,
                                                   isPayload: false);
        }

        /// <summary>
        /// Turns an assert against a snapshot that does not exist yet into a recording.
        ///
        /// The writer sits behind the comparison, and for an unresolved reference
        /// <see cref="IObjectAssertContext.ResolvedExpectedJson"/> still holds the file NAME - so the
        /// comparison threw ("could not deserialize your json string into expected type") long before
        /// the writer could create anything. Recording the current object is what write response means
        /// for a snapshot that is about to be created; the http route does the same thing one layer up.
        /// </summary>
        private static ObjectAssertContext<T> PrepareForRecording<T>(ObjectAssertContext<T> context)
        {
            if (context.ExpectedResultFile.Resolved ||
                WriteResponseService.ShouldWriteResponse(context).IsFalse())
            {
                return context;
            }

            return context with
            {
                Expected = context.OrderFunc.IsNull() ? context.Current : context.OrderFunc(context.Current),
                ResolvedExpectedJson = null
            };
        }

        private static void ObjectsAreEqualInternal<T>(ObjectAssertContext<T> context)
        {
            var textDecorator = TextDecoratorProvider.For(context.CallingAssembly);

            BuildAssertService(textDecorator).ObjectsAreEqual(context);
        }

        /// <summary>
        /// Builds a formatted error message for unexpected exceptions in object assertions.
        /// Simpler than HTTP assertions since we don't have HTTP context.
        /// </summary>
        private static string BuildObjectAssertionError<T>(ObjectAssertContext<T> context,
                                                           Exception exception)
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine("❌ UNEXPECTED ASSERTION ERROR");
            sb.AppendLine("══════════════════════════════════════════════════════════════");
            sb.AppendLine();

            // Test Information
            sb.AppendLine("📦 Test Information");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
            var fullClassName = TestClassNameResolver.Resolve(context.CallerFilePath, projectName);
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {fullClassName}");
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine();

            // Assert Information
            sb.AppendLine("🔍 Assert Details");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Type",-10} : {typeof(T).Name}");
            sb.AppendLine($"{"Expected",-10} : {context.ExpectedResultParameterName}");
            sb.AppendLine($"{"Current",-10} : {context.CurrentResultParameterName}");

            // If this is a TestSdkProblemDetailsException, extract additional details
            if (exception is TestSdkProblemDetailsException sdkException && sdkException.ProblemDetails?.Extensions != null)
            {
                // Extract target type if available
                if (sdkException.ProblemDetails.Extensions.TryGetValue("type", out var targetType) && targetType != null)
                {
                    sb.AppendLine($"{"TargetType",-10} : {targetType}");
                }

                if (sdkException.ProblemDetails.Extensions.TryGetValue("typeFullName", out var targetTypeFullName) && targetTypeFullName != null)
                {
                    sb.AppendLine($"{"FullName",-10} : {targetTypeFullName}");
                }

                // Extract JSON string if available
                if (sdkException.ProblemDetails.Extensions.TryGetValue("jsonString", out var jsonString) && jsonString != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"{"JSON",-10} :");
                    sb.AppendLine(jsonString.ToString());
                }
            }

            sb.AppendLine();

            // Exception Details
            sb.AppendLine("⚠️ Exception Details");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine($"{"Type",-10} : {exception.GetType().FullName}");
            sb.AppendLine($"{"Message",-10} : {exception.Message}");

            if (exception.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exception:");
                sb.AppendLine($"{"Type",-10} : {exception.InnerException.GetType().FullName}");
                sb.AppendLine($"{"Message",-10} : {exception.InnerException.Message}");
            }

            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
            {
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);
            }

            sb.AppendLine();

            // Explanation
            sb.AppendLine("💡 What This Means");
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("An unexpected error occurred during object comparison. This could indicate:");
            sb.AppendLine();
            sb.AppendLine("  • A bug in the Test SDK assertion logic");
            sb.AppendLine("  • Serialization/deserialization issues");
            sb.AppendLine("  • Invalid object structure");
            sb.AppendLine("  • Type mismatch between expected and actual objects");
            sb.AppendLine();
            sb.AppendLine("If this appears to be a Test SDK bug, please report it with the");
            sb.AppendLine("exception details and stack trace shown above.");
            sb.AppendLine();
            sb.AppendLine("══════════════════════════════════════════════════════════════");

            return sb.ToString();
        }

    }
#pragma warning restore IDE0060 // Remove unused parameter
}