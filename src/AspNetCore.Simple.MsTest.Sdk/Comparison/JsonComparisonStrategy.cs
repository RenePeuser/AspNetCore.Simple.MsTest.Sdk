using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    public static class AddJsonComparisonStrategyExtension
    {
        public static void AddJsonComparisonStrategy(this IServiceCollection services)
        {
            services.AddJsonSerializer();
            services.AddJsonDiffer();
            services.AddSingletonIfNotExists<ISpecificComparisonStrategy, JsonComparisonStrategy>();
        }
    }

    /// <summary>
    /// Comparison strategy for JSON-serializable objects.
    /// Performs deep object comparison using JSON serialization and diffing.
    /// Acts as fallback strategy - can handle all types except string.
    ///
    /// Note: Does NOT inherit from ComparisonStrategyBase because it handles multiple types (not type-specific).
    /// Should be registered LAST in DI so specific strategies are checked first.
    /// </summary>
    internal sealed class JsonComparisonStrategy(IJsonDiffer jsonDiffer,
                                                 Serializer.Json.JsonSerializer jsonSerializer,
                                                 JsonSerializerOptions jsonSerializerOptions) : ISpecificComparisonStrategy
    {
        public bool CanCompare<T>(ObjectAssertContext<T> context)
        {
            // Fallback strategy - can handle all types (except string which is handled by StringComparisonStrategy)
            // Returns true for everything - should be registered last in DI
            var canCompare = typeof(T).NotEqualsTo(typeof(string));

            return canCompare;
        }

        public ComparisonResult Compare<T>(ObjectAssertContext<T> context)
        {
            // Defensive check - should only be called if CanCompare returned true
            if (CanCompare(context).IsFalse())
            {
                throw new InvalidOperationException($"JsonComparisonStrategy can only compare objects, but was asked to compare {typeof(T).Name}");
            }

            var currentObject = context.CurrentObject;

            // 1. Serialize current object
            string currentJson;
            var serializationFailed = false;

            try
            {
                currentJson = currentObject.ToJson(jsonSerializerOptions);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // Serialization failed (e.g., object contains non-serializable properties like MethodBase)
                // Fall back to string-based comparison
                serializationFailed = true;
                currentJson = currentObject?.ToString() ?? "null";
            }

            // 2. Get or deserialize expected object
            T? expectedObject;
            string expectedJson;

            // Optimization: If Expected object is already available, skip deserialization
            if (context.Expected.IsNotNull())
            {
                expectedObject = context.Expected;
                expectedJson = string.Empty; // Will be serialized later after ordering
            }
            else
            {
                // Fallback: Deserialize from JSON
                expectedJson = context.ResolvedExpectedJson ?? string.Empty;

                try
                {
                    expectedObject = jsonSerializer.Deserialize<T>(expectedJson);
                }
                catch (TestSdkProblemDetailsException)
                {
                    // This is handled by the caller - rethrow to let proper error formatting happen
                    throw;
                }
#pragma warning disable CA1031
                catch (Exception)
#pragma warning restore CA1031
                {
                    // Deserialization failed - might be a text file rather than JSON
                    // If serialization also failed, do string comparison
                    if (serializationFailed)
                    {
                        return CompareAsStrings(expectedJson, currentJson);
                    }

                    // Return error result - caller will handle assertion failure
                    return new ComparisonResult
                    {
                        Differences = ImmutableList<Difference>.Empty,
                        FormattedExpected = expectedJson,
                        FormattedCurrent = currentJson,
                        HasSchemaMismatch = true
                    };
                }
            }

            // 3. Validate deserialized object
            if (expectedObject.IsNull())
            {
                return new ComparisonResult
                {
                    Differences = ImmutableList<Difference>.Empty,
                    FormattedExpected = expectedJson,
                    FormattedCurrent = "null",
                    HasSchemaMismatch = true
                };
            }

            // 4. Apply ordering function
            var orderedExpected = context.OrderFunc(expectedObject);
            var orderedCurrent = context.OrderFunc(context.Current);

            var expectedOrderedJson = orderedExpected.ToJson(jsonSerializerOptions);
            var currentOrderedJson = orderedCurrent.ToJson(jsonSerializerOptions);

            // 5. Find differences
            var differences = jsonDiffer.FindDifferences(expectedOrderedJson, currentOrderedJson);

            // 6. Check for schema mismatches
            var hasSchemaMismatch = differences.Any(item =>
                                                        item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                        item.MemberPath.EndsWith(']').IsFalse());

            // 7. Special case: Content.Value differences (HTTP-specific)
            var contentValueDifference = differences.FirstOrDefault(d =>
                                                                        d.MemberPath.Equals("Content.Value", StringComparison.OrdinalIgnoreCase));

            if (contentValueDifference.IsNotNull())
            {
                differences = jsonDiffer.FindDifferences(contentValueDifference.Value1 ?? string.Empty,
                                                         contentValueDifference.Value2 ?? string.Empty);

                hasSchemaMismatch = differences.Any(item =>
                                                        (item.MismatchType is MismatchType.MissingInFirst or MismatchType.MissingInSecond) &&
                                                        item.MemberPath.EndsWith(']').IsFalse());
            }

            // 8. Apply difference filtering (global func + per-assert func + global/per-assert predicate)
            var filteredDifferences = AssertObjectExtensions.ApplyDifferenceFiltering(differences,
                                                                                     context.DifferenceFunc,
                                                                                     context.DifferenceFilter);

            return new ComparisonResult
            {
                Differences = filteredDifferences,
                FormattedExpected = expectedOrderedJson,
                FormattedCurrent = currentOrderedJson,
                HasSchemaMismatch = hasSchemaMismatch
            };
        }

        /// <summary>
        /// Fallback method for string-based comparison when JSON serialization fails.
        /// </summary>
        private static ComparisonResult CompareAsStrings(string expectedString,
                                                         string currentString)
        {
            // Normalize line endings
            expectedString = expectedString.Replace("\r\n", "\n").Replace("\r", "\n");
            currentString = currentString.Replace("\r\n", "\n").Replace("\r", "\n");

            // Simple equality check
            if (expectedString == currentString)
            {
                return new ComparisonResult
                {
                    Differences = ImmutableList<Difference>.Empty,
                    FormattedExpected = expectedString,
                    FormattedCurrent = currentString,
                    HasSchemaMismatch = false
                };
            }

            // Strings differ - create a single difference
            var difference = new Difference
            {
                MemberPath = "Value",
                Value1 = expectedString,
                Value2 = currentString,
                MismatchType = MismatchType.ValueDifference
            };

            return new ComparisonResult
            {
                Differences = ImmutableList.Create(difference),
                FormattedExpected = expectedString,
                FormattedCurrent = currentString,
                HasSchemaMismatch = true
            };
        }
    }
}