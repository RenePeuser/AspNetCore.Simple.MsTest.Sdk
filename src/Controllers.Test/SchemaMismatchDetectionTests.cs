using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// Challenge tests for Schema Mismatch Detection Logic:
    /// - Schema mismatch = MissingInFirst OR MissingInSecond (NOT just ValueDifference)
    /// - Array index differences (paths ending with ']') should NOT trigger schema mismatch
    /// - Property differences should trigger schema mismatch
    /// </summary>
    [TestClass]
    [TestCategory("SchemaMismatch")]
    public sealed class SchemaMismatchDetectionTests
    {
        private static IJsonDiffer _jsonDiffer = null!;

        [TestInitialize]
        public void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddJsonDiffer();
            _jsonDiffer = serviceCollection.BuildServiceProvider().GetRequiredService<IJsonDiffer>();
        }

        #region Schema Mismatch Indicators (MissingInFirst or MissingInSecond)

        [TestMethod]
        public void ShouldProduceMismatchType_MissingInFirst_WhenPropertyExistsOnlyInCurrent()
        {
            // Scenario: Current/Response has extra property that Expected doesn't have
            // Expected: { "id": 1 }
            // Current:  { "id": 1, "email": "test@example.com" }
            // Result: "email" is MissingInFirst (missing in Expected/First parameter)

            var expected = /*lang=json,strict*/ """{ "id": 1 }""";
            var current = /*lang=json,strict*/ """{ "id": 1, "email": "test@example.com" }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            var emailDiff = diffs.FirstOrDefault(d => d.MemberPath == "email");
            Assert.IsNotNull(emailDiff, "Should detect 'email' difference");

            Assert.AreEqual(MismatchType.MissingInFirst, emailDiff.MismatchType,
                            "Property in current but not in expected should be MissingInFirst");
        }

        [TestMethod]
        public void ShouldProduceMismatchType_MissingInSecond_WhenPropertyExistsOnlyInExpected()
        {
            // Scenario: Expected requires property that Current/Response doesn't have
            // Expected: { "id": 1, "email": "test@example.com" }
            // Current:  { "id": 1 }
            // Result: "email" is MissingInSecond (missing in Current/Second parameter)

            var expected = /*lang=json,strict*/ """{ "id": 1, "email": "test@example.com" }""";
            var current = /*lang=json,strict*/ """{ "id": 1 }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            var emailDiff = diffs.FirstOrDefault(d => d.MemberPath == "email");
            Assert.IsNotNull(emailDiff, "Should detect 'email' difference");

            Assert.AreEqual(MismatchType.MissingInSecond, emailDiff.MismatchType,
                            "Property in expected but not in current should be MissingInSecond");
        }

        [TestMethod]
        public void ShouldProduceMismatchType_ValueDifference_WhenBothHavePropertyButDifferentValues()
        {
            // Scenario: Both have same property but different values
            // Expected: { "name": "Expected" }
            // Current:  { "name": "Current" }
            // Result: "name" is ValueDifference (NOT a schema mismatch)

            var expected = /*lang=json,strict*/ """{ "name": "Expected" }""";
            var current = /*lang=json,strict*/ """{ "name": "Current" }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            var nameDiff = diffs.FirstOrDefault(d => d.MemberPath == "name");
            Assert.IsNotNull(nameDiff, "Should detect 'name' difference");

            Assert.AreEqual(MismatchType.ValueDifference, nameDiff.MismatchType,
                            "Same property with different values should be ValueDifference");
        }

        #endregion

        #region Schema Mismatch Logic Application

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_WhenAnyDifferenceIs_MissingInFirst_AndNotArrayIndex()
        {
            // Schema mismatch detection logic:
            // hasSchemaMismatch = differences.Any(item =>
            //     item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
            //     item.MemberPath.EndsWith(']').IsFalse());

            var expected = /*lang=json,strict*/ """{ "id": 1 }""";
            var current = /*lang=json,strict*/ """{ "id": 1, "extra": "value" }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify the condition for schema mismatch
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsTrue(hasSchemaMismatch,
                          "MissingInFirst (not array index) should trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_WhenAnyDifferenceIs_MissingInSecond_AndNotArrayIndex()
        {
            var expected = /*lang=json,strict*/ """{ "id": 1, "required": "value" }""";
            var current = /*lang=json,strict*/ """{ "id": 1 }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify the condition for schema mismatch
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsTrue(hasSchemaMismatch,
                          "MissingInSecond (not array index) should trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_WhenOnlyValueDifferences()
        {
            var expected = /*lang=json,strict*/ """{ "name": "Expected", "value": 1 }""";
            var current = /*lang=json,strict*/ """{ "name": "Current", "value": 2 }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify the condition for schema mismatch
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsFalse(hasSchemaMismatch,
                           "Only ValueDifference should NOT trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_WhenIdentical()
        {
            var expected = /*lang=json,strict*/ """{ "id": 1, "name": "Test" }""";
            var current = /*lang=json,strict*/ """{ "id": 1, "name": "Test" }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify the condition for schema mismatch
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsFalse(hasSchemaMismatch, "No differences means no schema mismatch");
            Assert.AreEqual(0, diffs.Count, "Should have no differences");
        }

        #endregion

        #region Array Index Exclusion Rule

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_ForArrayLengthDifference()
        {
            // Arrays with different lengths produce differences with paths ending with ']'
            // These should NOT trigger schema mismatch due to EndsWith(']') check
            var expected = /*lang=json,strict*/ """{ "items": [1, 2] }""";
            var current = /*lang=json,strict*/ """{ "items": [1, 2, 3] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify that we have array index differences
            var arrayIndexDiff = diffs.FirstOrDefault(d => d.MemberPath.EndsWith(']'));
            Assert.IsNotNull(arrayIndexDiff, "Should have at least one array index difference");

            // Verify the schema mismatch logic excludes array indexes
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsFalse(hasSchemaMismatch,
                           "Array length differences (paths ending with ']') should NOT trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_ForArrayElementPropertyDifference()
        {
            // Array elements with different properties (path does NOT end with ']')
            // Example: "items[0].status" (ends with 'status', not ']')
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "items": [{ "id": 1, "name": "Item1" }]
                                                }
                                                """;

            var current = /*lang=json,strict*/ """
                                               {
                                                   "items": [{ "id": 1, "name": "Item1", "status": "active" }]
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Verify we have property difference in array element
            var propertyDiff = diffs.FirstOrDefault(d =>
                                                        d.MemberPath.Contains('[') && !d.MemberPath.EndsWith(']'));

            Assert.IsNotNull(propertyDiff, "Should have property difference in array element");

            // Verify the schema mismatch logic includes this
            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsTrue(hasSchemaMismatch,
                          "Property differences in array elements (not ending with ']') should trigger schema mismatch");
        }

        #endregion

        #region Complex Combined Scenarios

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_WhenBothMissingInFirstAndSecond()
        {
            // Both types of missing properties (schema completely different)
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "expectedProp": "value"
                                                }
                                                """;

            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "currentProp": "value"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.IsTrue(diffs.Any(d => d.MismatchType == MismatchType.MissingInFirst),
                          "Should have MissingInFirst");

            Assert.IsTrue(diffs.Any(d => d.MismatchType == MismatchType.MissingInSecond),
                          "Should have MissingInSecond");

            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsTrue(hasSchemaMismatch,
                          "Combined missing properties should trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_WhenOnlyArrayValuesAndPropertiesDiffer()
        {
            // Same schema, only values differ
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "name": "Expected",
                                                    "items": [1, 2, 3]
                                                }
                                                """;

            var current = /*lang=json,strict*/ """
                                               {
                                                   "name": "Current",
                                                   "items": [4, 5, 6]
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            var hasSchemaMismatch = diffs.Any(item =>
                                                  item.MismatchType != MismatchType.ValueDifference &&
                                                  !item.MemberPath.EndsWith(']'));

            Assert.IsFalse(hasSchemaMismatch,
                           "Only value differences should NOT trigger schema mismatch");
        }

        [TestMethod]
        public void SchemaMismatch_CompleteRuleVerification()
        {
            // Complete verification of the schema mismatch rule:
            // hasSchemaMismatch = differences.Any(item =>
            //     item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
            //     item.MemberPath.EndsWith(']').IsFalse());

            // Test cases that should trigger schema mismatch
            var testCases = new[] { (Expected: /*lang=json,strict*/ """{"a":1}""", Current: /*lang=json,strict*/ """{"a":1,"b":2}""", Reason: "Extra property in current"), (Expected: /*lang=json,strict*/ """{"a":1,"b":2}""", Current: /*lang=json,strict*/ """{"a":1}""", Reason: "Missing property in current"), (Expected: /*lang=json,strict*/ """{"x":{"y":1}}""", Current: /*lang=json,strict*/ """{"x":{"y":1,"z":2}}""", Reason: "Extra nested property") };

            foreach (var testCase in testCases)
            {
                var diffs = _jsonDiffer.FindDifferences(testCase.Expected, testCase.Current);

                var hasSchemaMismatch = diffs.Any(item =>
                                                      item.MismatchType != MismatchType.ValueDifference &&
                                                      !item.MemberPath.EndsWith(']'));

                Assert.IsTrue(hasSchemaMismatch, $"Should detect schema mismatch for: {testCase.Reason}");
            }

            // Test cases that should NOT trigger schema mismatch
            var noSchemaMismatchCases = new[] { (Expected: /*lang=json,strict*/ """{"a":1}""", Current: /*lang=json,strict*/ """{"a":2}""", Reason: "Only value differs"), (Expected: /*lang=json,strict*/ """{"a":1,"b":2}""", Current: /*lang=json,strict*/ """{"a":1,"b":2}""", Reason: "Identical"), (Expected: /*lang=json,strict*/ """{"items":[1,2]}""", Current: /*lang=json,strict*/ """{"items":[1,2,3]}""", Reason: "Array length differs") };

            foreach (var testCase in noSchemaMismatchCases)
            {
                var diffs = _jsonDiffer.FindDifferences(testCase.Expected, testCase.Current);

                var hasSchemaMismatch = diffs.Any(item =>
                                                      item.MismatchType != MismatchType.ValueDifference &&
                                                      !item.MemberPath.EndsWith(']'));

                Assert.IsFalse(hasSchemaMismatch, $"Should NOT detect schema mismatch for: {testCase.Reason}");
            }
        }

        #endregion
    }
}