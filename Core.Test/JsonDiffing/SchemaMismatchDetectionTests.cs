using System;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using Core.Test.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.JsonDiffing
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
        /// <summary>
        /// The rule under test, in one place:
        /// hasSchemaMismatch = differences.Any(item =&gt;
        ///     item.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &amp;&amp;
        ///     item.MemberPath.EndsWith(']').IsFalse());
        /// </summary>
        private static readonly Func<Difference, bool> SchemaMismatchRule =
            item => item.MismatchType != MismatchType.ValueDifference && !item.MemberPath.EndsWith(']');

        private const string RuleDescription = "a difference that is not a ValueDifference and whose path does not end with ']'";

        private const string RuleFix = "The rule lives wherever hasSchemaMismatch is computed: a difference counts as a schema mismatch only when its MismatchType is not ValueDifference AND its MemberPath does not end with ']'. Check both halves - and check what JsonDiffer reported as MismatchType and MemberPath in the Details above.";


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

            var diffs = JsonDifferences.Of(expected, current);

            var emailDiff = diffs.FirstOrDefault(d => d.MemberPath == "email");

            Assert.That.IsNotNull(emailDiff,
                                  because: "'email' exists only in current, so the differ has to report it at all - a property appearing out of nowhere is exactly what a schema mismatch is.",
                                  fix: "Check that JsonDiffer.FindDifferences walks the properties of the second document too, not only those of the first.");

            Assert.That.AreEqual(MismatchType.MissingInFirst,
                                 emailDiff.MismatchType,
                                 because: "The direction is what tells the author whether the api grew a property or the snapshot lost one. 'In current but not in expected' is MissingInFirst - expected is the first argument.",
                                 fix: "Check the argument order in JsonDiffer.FindDifferences: MissingInFirst means absent from json1, MissingInSecond means absent from json2. Swapping them inverts every message the sdk prints.");
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

            var diffs = JsonDifferences.Of(expected, current);

            var emailDiff = diffs.FirstOrDefault(d => d.MemberPath == "email");

            Assert.That.IsNotNull(emailDiff,
                                  because: "'email' exists only in expected, so the differ has to report it - a property the api stopped returning must never pass unnoticed.",
                                  fix: "Check that JsonDiffer.FindDifferences walks the properties of the first document and reports the ones missing on the other side.");

            Assert.That.AreEqual(MismatchType.MissingInSecond,
                                 emailDiff.MismatchType,
                                 because: "The direction is what tells the author whether the api grew a property or dropped one. 'In expected but not in current' is MissingInSecond - current is the second argument.",
                                 fix: "Check the argument order in JsonDiffer.FindDifferences: MissingInSecond means absent from json2. Swapping the two inverts every message the sdk prints.");
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

            var diffs = JsonDifferences.Of(expected, current);

            var nameDiff = diffs.FirstOrDefault(d => d.MemberPath == "name");

            Assert.That.IsNotNull(nameDiff,
                                  because: "Both sides carry 'name' with different content, so the differ has to report the value difference.",
                                  fix: "Check the leaf comparison in JsonDiffer.FindDifferences - two present properties with unequal values have to produce a Difference.");

            Assert.That.AreEqual(MismatchType.ValueDifference,
                                 nameDiff.MismatchType,
                                 because: "The property exists on both sides, so the schema is intact and only the data differs. Classifying this as missing would make every ordinary value change look like a contract break.",
                                 fix: "JsonDiffer must only use MissingInFirst/MissingInSecond when a property is really absent on one side, never for two present-but-unequal values.");
        }

        #endregion

        #region Schema Mismatch Logic Application

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_WhenAnyDifferenceIs_MissingInFirst_AndNotArrayIndex()
        {
            var expected = /*lang=json,strict*/ """{ "id": 1 }""";
            var current = /*lang=json,strict*/ """{ "id": 1, "extra": "value" }""";

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.Any(diffs,
                            SchemaMismatchRule,
                            predicateDescription: RuleDescription,
                            because: "'extra' appears only in current and its path carries no array index, so it is a genuine schema mismatch - the api returns something the snapshot does not know about.",
                            fix: RuleFix);
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeTrue_WhenAnyDifferenceIs_MissingInSecond_AndNotArrayIndex()
        {
            var expected = /*lang=json,strict*/ """{ "id": 1, "required": "value" }""";
            var current = /*lang=json,strict*/ """{ "id": 1 }""";

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.Any(diffs,
                            SchemaMismatchRule,
                            predicateDescription: RuleDescription,
                            because: "'required' is expected but absent from current and its path carries no array index, so it is a genuine schema mismatch - the api stopped returning a property the snapshot relies on.",
                            fix: RuleFix);
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_WhenOnlyValueDifferences()
        {
            var expected = /*lang=json,strict*/ """{ "name": "Expected", "value": 1 }""";
            var current = /*lang=json,strict*/ """{ "name": "Current", "value": 2 }""";

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.None(diffs,
                             SchemaMismatchRule,
                             predicateDescription: RuleDescription,
                             because: "Both documents carry exactly the same properties; only the data differs. Reporting a schema mismatch here would send the author looking for a contract change that never happened.",
                             fix: RuleFix);
        }

        [TestMethod]
        public void SchemaMismatch_ShouldBeFalse_WhenIdentical()
        {
            var expected = /*lang=json,strict*/ """{ "id": 1, "name": "Test" }""";
            var current = /*lang=json,strict*/ """{ "id": 1, "name": "Test" }""";

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.IsEmpty(diffs,
                                because: "The two documents are identical, so the differ must report nothing at all - a phantom difference here would make every unchanged snapshot fail.",
                                fix: "Check the equality comparison in JsonDiffer.FindDifferences; property order and formatting must not count as a difference.");

            Assert.That.None(diffs,
                             SchemaMismatchRule,
                             predicateDescription: RuleDescription,
                             because: "No differences at all can never amount to a schema mismatch.",
                             fix: RuleFix);
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

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.Any(diffs,
                            d => d.MemberPath.EndsWith(']'),
                            predicateDescription: "a difference whose path ends with ']' (an array element)",
                            because: "The exclusion rule can only be proven if the differ really produced an array-index path here - without one this test would pass no matter what the rule does.",
                            fix: "Check how JsonDiffer builds MemberPath for array elements: an extra element has to be reported as 'items[2]', not as 'items'.");

            Assert.That.None(diffs,
                             SchemaMismatchRule,
                             predicateDescription: RuleDescription,
                             because: "A longer or shorter array is a data difference, not a contract change - the property 'items' exists on both sides. That is why paths ending with ']' are excluded from the rule.",
                             fix: RuleFix);
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

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.Any(diffs,
                            d => d.MemberPath.Contains('[') && !d.MemberPath.EndsWith(']'),
                            predicateDescription: "a difference inside an array element, e.g. 'items[0].status'",
                            because: "The exclusion rule keys on the path ending with ']'. A property inside an element ends with the property name, so it has to be reported with the full path 'items[0].status'.",
                            fix: "Check that JsonDiffer appends the property name after the index when descending into array elements, instead of stopping the path at 'items[0]'.");

            Assert.That.Any(diffs,
                            SchemaMismatchRule,
                            predicateDescription: RuleDescription,
                            because: "An element that grew a property is a real contract change, even though it sits inside an array - only the array length itself is exempt from the rule.",
                            fix: RuleFix);
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

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.Any(diffs,
                            d => d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "a MissingInFirst difference (here: 'currentProp')",
                            because: "Both sides lost and gained a property at once. The differ has to report both directions - reporting only one would hide half the contract change.",
                            fix: "Check that JsonDiffer walks both documents' property sets and does not stop after the first side it finds a mismatch on.");

            Assert.That.Any(diffs,
                            d => d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "a MissingInSecond difference (here: 'expectedProp')",
                            because: "Both sides lost and gained a property at once. The differ has to report both directions - reporting only one would hide half the contract change.",
                            fix: "Check that JsonDiffer walks both documents' property sets and does not stop after the first side it finds a mismatch on.");

            Assert.That.Any(diffs,
                            SchemaMismatchRule,
                            predicateDescription: RuleDescription,
                            because: "Properties missing on both sides is the clearest possible schema mismatch.",
                            fix: RuleFix);
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

            var diffs = JsonDifferences.Of(expected, current);

            Assert.That.None(diffs,
                             SchemaMismatchRule,
                             predicateDescription: RuleDescription,
                             because: "Both documents carry the same properties and the same array length - every difference here is pure data. The two exclusions of the rule have to hold at the same time.",
                             fix: RuleFix);
        }

        [TestMethod]
        public void SchemaMismatch_CompleteRuleVerification()
        {
            // Test cases that should trigger schema mismatch
            var testCases = new[] { (Expected: /*lang=json,strict*/ """{"a":1}""", Current: /*lang=json,strict*/ """{"a":1,"b":2}""", Reason: "Extra property in current"), (Expected: /*lang=json,strict*/ """{"a":1,"b":2}""", Current: /*lang=json,strict*/ """{"a":1}""", Reason: "Missing property in current"), (Expected: /*lang=json,strict*/ """{"x":{"y":1}}""", Current: /*lang=json,strict*/ """{"x":{"y":1,"z":2}}""", Reason: "Extra nested property") };

            foreach (var testCase in testCases)
            {
                var diffs = JsonDifferences.Of(testCase.Expected, testCase.Current);

                Assert.That.Any(diffs,
                                SchemaMismatchRule,
                                predicateDescription: RuleDescription,
                                because: $"{testCase.Reason} - expected {testCase.Expected} against current {testCase.Current}. Every shape in this table changes the set of properties, so all of them have to be flagged.",
                                fix: RuleFix);
            }

            // Test cases that should NOT trigger schema mismatch
            var noSchemaMismatchCases = new[] { (Expected: /*lang=json,strict*/ """{"a":1}""", Current: /*lang=json,strict*/ """{"a":2}""", Reason: "Only value differs"), (Expected: /*lang=json,strict*/ """{"a":1,"b":2}""", Current: /*lang=json,strict*/ """{"a":1,"b":2}""", Reason: "Identical"), (Expected: /*lang=json,strict*/ """{"items":[1,2]}""", Current: /*lang=json,strict*/ """{"items":[1,2,3]}""", Reason: "Array length differs") };

            foreach (var testCase in noSchemaMismatchCases)
            {
                var diffs = JsonDifferences.Of(testCase.Expected, testCase.Current);

                Assert.That.None(diffs,
                                 SchemaMismatchRule,
                                 predicateDescription: RuleDescription,
                                 because: $"{testCase.Reason} - expected {testCase.Expected} against current {testCase.Current}. The set of properties is unchanged, so this must not be flagged as a contract break.",
                                 fix: RuleFix);
            }
        }

        #endregion
    }
}