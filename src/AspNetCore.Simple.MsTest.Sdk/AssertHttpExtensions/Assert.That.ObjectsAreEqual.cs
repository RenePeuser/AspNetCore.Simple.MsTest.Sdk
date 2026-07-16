using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
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

        /// <summary>
        /// Global per-difference predicate. Return <c>true</c> to keep a difference,
        /// <c>false</c> to ignore it. The SDK iterates internally, so you only describe
        /// the condition (e.g. <c>d => d.MemberPath != "id"</c>) instead of writing a loop.
        /// Applied in addition to (and after) <see cref="DifferenceFunc"/> and any
        /// per-assert filter. Defaults to keeping every difference.
        /// </summary>
        public static Predicate<Difference> DifferenceFilter { get; set; } = _ => true;

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

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new EmbeddedFileLocalizer(new TestCreatorSettings(), JsonSerializerOptions, new PlainTextDecorator(),
                                                                                                        new SourceCodeExtractor());

        private static readonly Serializer.Json.JsonSerializer JsonSerializer = new(JsonSerializerOptions);

        // Text decorator - conditional on build configuration (default fallback)
#if DEBUG
        private static readonly ITextDecorator TextDecorator = new PlainTextDecorator();
#else
        private static readonly ITextDecorator TextDecorator = new AnsiColorTextDecorator();
#endif

        // Builders for output strategies
        private static readonly TableBuilder StaticTableBuilder = new();

        private static readonly DifferencesTableBuilder DifferencesTableBuilder = new(StaticTableBuilder, TextDecorator);

        private static readonly JsonSectionBuilder JsonSectionBuilder = new(TextDecorator);

        // Output strategies for AssertService
        private static readonly PrimitiveOutputStrategy PrimitiveOutputStrategy = new(TextDecorator);

        private static readonly ObjectOutputStrategy ObjectOutputStrategy = new(DifferencesTableBuilder, JsonSectionBuilder, TextDecorator);

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
            assert.ObjectsAreEqual(expectedObject: expectedObject,
                                   currentObject: currentObject,
                                   comparisonFunc: comparisonFunc,
                                   title: title,
                                   differenceFunc: differenceFunc,
                                   differenceFilter: differenceFilter,
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
                ObjectsAreEqualInternal(context);
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
                // ToDo: Error handler as well
                // GLOBAL EXCEPTION HANDLER FOR OBJECT ASSERTIONS
                // Build a simple error message since we don't have HTTP context here
                var errorOutput = BuildObjectAssertionError(context, exception);
                Assert.That.Fail(errorOutput);

                throw; // Never reached, but required for compiler
            }
        }

        private static void ObjectsAreEqualInternal<T>(ObjectAssertContext<T> context)
        {
            // Check if calling assembly is in debug mode - if so, use PlainTextDecorator
            var useDebugDecorator = context.CallingAssembly.IsCompiledInDebug();

            if (useDebugDecorator)
            {
                // Create debug-specific builders and service
                var plainTextDecorator = new PlainTextDecorator();
                var tableBuilder = new TableBuilder();
                var debugDifferencesTableBuilder = new DifferencesTableBuilder(tableBuilder, plainTextDecorator);
                var debugJsonSectionBuilder = new JsonSectionBuilder(plainTextDecorator);
                var debugPrimitiveOutputStrategy = new PrimitiveOutputStrategy(plainTextDecorator);
                var debugObjectOutputStrategy = new ObjectOutputStrategy(debugDifferencesTableBuilder, debugJsonSectionBuilder, plainTextDecorator);

                var debugOutputStrategies = new IAssertOutputStrategy[] { debugPrimitiveOutputStrategy, debugObjectOutputStrategy };

                var debugOutputBuilder = new AssertOutputBuilder(debugOutputStrategies);

                var debugAssertService = new AssertService(ComparisonStrategy, ResponseWriter, WriteResponseService,
                                                           debugOutputBuilder);

                // Use debug service
                debugAssertService.ObjectsAreEqual(context);
            }
            else
            {
                // Use the default (release) service
                AssertService.ObjectsAreEqual(context);
            }
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
            var fullClassName = GetFullClassName(context.CallerFilePath, projectName);
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

        private static string GetFullClassName(string callerFilePath,
                                               string projectName)
        {
            try
            {
                // Get the file name without extension
                var fileName = Path.GetFileNameWithoutExtension(callerFilePath);

                // Find the project root by looking for the project name in the path
                var pathSegments = callerFilePath.Replace("\\", "/").Split('/');
                var projectIndex = Array.FindIndex(pathSegments, s => s.Equals(projectName, StringComparison.OrdinalIgnoreCase));

                if (projectIndex >= 0 && projectIndex < pathSegments.Length - 1)
                {
                    // Take segments after the project name up to (but not including) the file name
                    var namespaceParts = pathSegments.Skip(projectIndex + 1).Take(pathSegments.Length - projectIndex - 2).ToList();

                    if (namespaceParts.Count > 0)
                    {
                        // Build namespace.ClassName
                        var namespaceStr = string.Join(".", namespaceParts.Select(s => s.Replace(" ", "")));

                        return $"{projectName}.{namespaceStr}.{fileName}";
                    }

                    // File is directly in project root
                    return $"{projectName}.{fileName}";
                }

                // Fallback to just the file name
                return fileName;
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                // Fallback to full caller file path on any error
                return callerFilePath;
            }
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}