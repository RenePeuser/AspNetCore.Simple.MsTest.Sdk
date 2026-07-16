using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class AssertObjectExtensions
    {
        // ============================================================
        // File-based comparison overloads (string expectedFile, T current)
        // ============================================================

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   item => item,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentResult: currentObject,
                                   callingAssembly: callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentResult: currentResult,
                                   callingAssembly: callingAssembly,
                                   parameters: [],
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentResult: currentResult,
                                   title: string.Empty,
                                   callingAssembly: callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentResult: currentResult,
                                   title: title,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentResult: currentResult,
                                   title: title,
                                   callingAssembly: callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
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
                                   callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
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
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
                                   callingAssembly: callingAssembly,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   differenceFilter: differenceFilter,
                                   callingAssembly: callingAssembly,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   differenceFilter: differenceFilter,
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
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: item => item,
                                   title: string.Empty,
                                   differenceFunc: differenceFunc,
                                   differenceFilter: differenceFilter,
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
                                   callingAssembly: callingAssembly,
                                   parameters: [],
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
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
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
                                   title: string.Empty,
                                   callingAssembly: callingAssembly,
                                   differenceFunc: difference => difference,
                                   curl: string.Empty,
                                   parameters: parameters,
                                   writeResponse: writeResponse,
                                   expectedResultParameterName: expectedResultParameterName,
                                   currentResultParameterName: currentResultParameterName,
                                   callerFilePath: callerFilePath,
                                   callerMemberName: callerMemberName,
                                   callerLineNumber: callerLineNumber);
        }

        public static void ObjectsAreEqual<T>(this Assert assert,
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
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
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              (string Key, object? Value)[] parameters,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            assert.ObjectsAreEqual(expectedObjectAsJson: expectedObjectAsJson,
                                   currentObject: currentObject,
                                   orderFunc: orderFunc,
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
                                              string expectedObjectAsJson,
                                              T currentObject,
                                              Func<T?, T?> orderFunc,
                                              string title,
                                              Assembly callingAssembly,
                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                              string curl,
                                              (string Key, object? Value)[] parameters,
                                              Predicate<Difference>? differenceFilter = null,
                                              bool writeResponse = false,
                                              [CallerArgumentExpression(nameof(expectedObjectAsJson))]
                                              string expectedResultParameterName = "",
                                              [CallerArgumentExpression(nameof(currentObject))]
                                              string currentResultParameterName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
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
                CallerLineNumber = callerLineNumber,
                CallerMemberName = callerMemberName,
                CallingAssembly = callingAssembly,
                Current = currentObject,
                CurrentObject = currentObject,
                CurrentResultParameterName = currentResultParameterName,
                DifferenceFunc = differenceFunc,
                DifferenceFilter = differenceFilter ?? (static _ => true),
                ExpectedType = typeof(T),
                ExpectedObjectAsJson = expectedObjectAsJson,
                ExpectedResultFile = expectedFile,
                ExpectedResultParameterName = expectedResultParameterName,
                OrderFunc = orderFunc,
                Parameters = parameters,
                ResolvedExpectedJson = resolvedExpectedJson,
                TypeIsPrimitiveType = targetIsPrimitiveType,
                WriteResponse = writeResponse,
                Expected = default,
            };

            ObjectsAreEqual(assert, context);
        }

        // ============================================================
    }
#pragma warning restore IDE0060 // Remove unused parameter
}