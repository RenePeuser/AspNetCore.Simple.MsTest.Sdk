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

        private static readonly CurrentResponseWriter CurrentResponseWriter = new(new TestCreatorSettings());

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
                                                                              {
                                                                                  PropertyNameCaseInsensitive = true,
                                                                                  Converters = { new JsonStringEnumConverter() }
                                                                              };

        public static Func<IImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = item => item;

        public static bool WriteResponse { get; set; }

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
                                   string.Empty,
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName,
                                   callerFilePath);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              T? expectedObject,
                                              T? currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            var json1 = orderedExpectedObject.ToJson();
            var json2 = orderedCurrentObject.ToJson();

            foreach (var valueTuple in parameters)
            {
                json1 = json1.Replace(valueTuple.Key, valueTuple.Value?.ToString());
                json2 = json2.Replace(valueTuple.Key, valueTuple.Value?.ToString());
            }

            // Brand new crazy function
            // We write the current result to the expected file
            if (writeResponse || WriteResponse)
            {
                CurrentResponseWriter.Write(json2, expectedResultParameterName, callerFilePath,
                                            callingAssembly);
            }

            var differences = JsonDiffer.FindDifferences(json1, json2);

            var commonDifferences = DifferenceFunc(differences).ToImmutableList();
            var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

            var resultTable = optimizedDifferences.ToResultTable(expectedResultParameterName, currentResultParameterName);

            if (optimizedDifferences.Any())
            {
                var output = OutputFormatter.GetOutputString(resultTable, json1, json2,
                                                             title, curl);

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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
                                   currentResultParameterName);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T, T> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
                                   writeResponse,
                                   expectedResultParameterName,
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
                                   parameters,
                                   writeResponse,
                                   expectedResultParameterName,
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
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            // This is most the use case when calling an API and want to know what comes back
            var currentObjectAsJson = currentObject.ToJson();

            // Brand new crazy function
            // We write the current result to the expected file
            if (writeResponse || WriteResponse)
            {
                CurrentResponseWriter.Write(currentObjectAsJson, expectedResultParameterName, callerFilePath,
                                            callingAssembly);
            }

            var jsonObject = expectedObjectAsJson.GetJsonStringFrom<T>(currentObjectAsJson, callingAssembly, curl,
                                                                       currentResultParameterName);

            jsonObject = jsonObject.ResolveParameters(parameters);

            var type = typeof(T);

            if (type.IsPrimitive || type == typeof(string))
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
                catch (Exception)
                {
                    var cantSerializeJsonErrorOutput = OutputFormatter.GetOutputString(title,
                                                                                       $"The given json for: '{expectedResultParameterName}' was not possible to convert into type: {typeof(T).Name}",
                                                                                       jsonObject,
                                                                                       currentObjectAsJson,
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

                var object1AsJson = orderedObject1.ToJson()
                                                  .ResolveParameters(parameters);

                var object2AsJson = orderedObject2.ToJson()
                                                  .ResolveParameters(parameters);

                
                var differences = jsonDiffer.FindDifferences(object1AsJson, object2AsJson);

                // 1. Check if we are comparing the same schema
                var schemaNotMatching = differences.Any() && differences.All(item => item.Value1.IsNull() || item.Value2.IsNull());
                var schemaMismatchTable = differences.ToResultTable(expectedResultParameterName, currentResultParameterName);

                var schemaNotMatchingError = OutputFormatter.GetOutputString(title, "Schema mismatch: Expected result and current result does not match", object1AsJson,
                                                                             object2AsJson, schemaMismatchTable, curl);

                Assert.IsFalse(schemaNotMatching, schemaNotMatchingError);

                var commonDifferences = DifferenceFunc(differences).ToImmutableList();
                var optimizedDifferences = differenceFunc(commonDifferences).ToImmutableList();

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
