using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// Challenge tests for JSON diff logic:
    /// - Detecting properties in response/current that are missing in expected
    /// - Detecting properties in expected that are missing in response/current
    /// </summary>
    [TestClass]
    [TestCategory("JsonDiffer")]
    [TestCategory("MissingProperties")]
    public sealed class JsonDifferMissingPropertiesTests
    {
        private const string ExtraBecause = "A property only the current response carries means the api returned something the snapshot does not describe. Missing it means a growing contract goes unnoticed until it breaks somewhere else.";

        private const string MissingBecause = "A property only the expected snapshot carries means the api stopped returning it. Missing it means a shrinking contract goes unnoticed - the most damaging kind of silent break.";

        private const string DirectionFix = "Check the argument order in JsonDiffer.FindDifferences: json1 is expected, json2 is current. MissingInFirst = absent from expected, MissingInSecond = absent from current. Swapping the two inverts every message the sdk prints.";

        private const string CountFix = "Compare the reported paths in the Details above with the two documents: too many findings means JsonDiffer also reports properties that are identical, too few means it stops descending too early.";

        private static IJsonDiffer _jsonDiffer = null!;

        [TestInitialize]
        public void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddJsonDiffer();
            _jsonDiffer = serviceCollection.BuildServiceProvider().GetRequiredService<IJsonDiffer>();
        }

        #region Properties in Current/Response missing in Expected (MissingInFirst)

        [TestMethod]
        public void ShouldDetect_ExtraPropertyInCurrent_Simple()
        {
            // Expected: only "id" and "name"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "name": "Test"
                                                }
                                                """;

            // Current: has additional "email" property
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "name": "Test",
                                                   "email": "test@example.com"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only 'email' is extra; every other property matches. {ExtraBecause}",
                                 fix: CountFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "email" &&
                                 d.MismatchType == MismatchType.MissingInFirst &&
                                 d.Value1 == null &&
                                 d.Value2 != null,
                            predicateDescription: "'email' as MissingInFirst with Value1 null and Value2 set",
                            because: $"{ExtraBecause} The two values also have to say which side the property came from: nothing on the expected side, the actual content on the current side.",
                            fix: $"{DirectionFix} And check that JsonDiffer leaves Value1 null for a property that only exists on the current side instead of filling both slots.");
        }

        [TestMethod]
        public void ShouldDetect_MultipleExtraPropertiesInCurrent()
        {
            // Expected: minimal set
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1
                                                }
                                                """;

            // Current: has many additional properties
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "name": "Test",
                                                   "email": "test@example.com",
                                                   "age": 25,
                                                   "isActive": true
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(4,
                                 diffs,
                                 because: $"All four extra properties have to be reported in one run. {ExtraBecause} Reporting only the first would make the author fix a growing contract one property at a time.",
                                 fix: CountFix);

            Assert.That.All(diffs,
                            d => d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "every difference classified as MissingInFirst",
                            because: "The expected side is empty here, so nothing can be a value difference - every single finding has to be a property the current side added.",
                            fix: DirectionFix);

            var extraProps = new[]
                             {
                                 "name", "email", "age",
                                 "isActive"
                             };

            foreach (var prop in extraProps)
            {
                Assert.That.Any(diffs,
                                d => d.MemberPath == prop,
                                predicateDescription: $"a difference for the property '{prop}'",
                                because: $"'{prop}' exists only in the current response. {ExtraBecause}",
                                fix: $"Check that JsonDiffer iterates the current document's properties as well - a missing '{prop}' here means it stops at the expected side.");
            }
        }

        [TestMethod]
        public void ShouldDetect_ExtraNestedPropertyInCurrent()
        {
            // Expected: nested object without "city"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "user": {
                                                        "name": "Alice"
                                                    }
                                                }
                                                """;

            // Current: has additional "city" in nested object
            var current = /*lang=json,strict*/ """
                                               {
                                                   "user": {
                                                       "name": "Alice",
                                                       "city": "Berlin"
                                                   }
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only the nested 'city' is extra - the surrounding 'user' object is identical and must not be reported as a difference of its own. {ExtraBecause}",
                                 fix: $"{CountFix} A finding on 'user' instead of 'user.city' means JsonDiffer compares objects wholesale rather than descending into them.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "user.city" &&
                                 d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'user.city' as MissingInFirst",
                            because: $"{ExtraBecause} The dotted path is what lets the author find the property inside a nested object instead of hunting through the payload.",
                            fix: $"Check that JsonDiffer joins parent and child with '.' while descending. {DirectionFix}");
        }

        [TestMethod]
        public void ShouldDetect_ExtraPropertyInArrayElement()
        {
            // Expected: array elements without "status"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "items": [
                                                        {
                                                            "id": 1,
                                                            "name": "Item1"
                                                        }
                                                    ]
                                                }
                                                """;

            // Current: array element has additional "status"
            var current = /*lang=json,strict*/ """
                                               {
                                                   "items": [
                                                       {
                                                           "id": 1,
                                                           "name": "Item1",
                                                           "status": "active"
                                                       }
                                                   ]
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only the element's extra 'status' differs - the array itself has the same length and must not be reported on top. {ExtraBecause}",
                                 fix: $"{CountFix} A second finding on 'items' means JsonDiffer compares array elements wholesale instead of descending into them.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "items[0].status" &&
                                 d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'items[0].status' as MissingInFirst",
                            because: $"{ExtraBecause} The path has to carry both the index and the property name - and because it does not end with ']' it correctly counts as a schema mismatch, unlike a pure array length change.",
                            fix: $"Check that JsonDiffer appends '[index]' and then keeps appending the property name. {DirectionFix}");
        }

        #endregion

        #region Properties in Expected missing in Current/Response (MissingInSecond)

        [TestMethod]
        public void ShouldDetect_MissingPropertyInCurrent_Simple()
        {
            // Expected: requires "id", "name", and "email"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "name": "Test",
                                                    "email": "test@example.com"
                                                }
                                                """;

            // Current: missing "email"
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "name": "Test"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only 'email' is missing; every other property matches. {MissingBecause}",
                                 fix: CountFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "email" &&
                                 d.MismatchType == MismatchType.MissingInSecond &&
                                 d.Value1 != null &&
                                 d.Value2 == null,
                            predicateDescription: "'email' as MissingInSecond with Value1 set and Value2 null",
                            because: $"{MissingBecause} The two values also have to say which side the property came from: the expected content on the first side, nothing on the current side.",
                            fix: $"{DirectionFix} And check that JsonDiffer leaves Value2 null for a property that only exists on the expected side instead of filling both slots.");
        }

        [TestMethod]
        public void ShouldDetect_MultipleMissingPropertiesInCurrent()
        {
            // Expected: full set of properties
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "name": "Test",
                                                    "email": "test@example.com",
                                                    "age": 25,
                                                    "isActive": true
                                                }
                                                """;

            // Current: only has "id"
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(4,
                                 diffs,
                                 because: $"All four missing properties have to be reported in one run. {MissingBecause} Reporting only the first would make the author fix a shrinking contract one property at a time.",
                                 fix: CountFix);

            Assert.That.All(diffs,
                            d => d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "every difference classified as MissingInSecond",
                            because: "The current side is empty here, so nothing can be a value difference - every single finding has to be a property the api stopped returning.",
                            fix: DirectionFix);

            var missingProps = new[]
                               {
                                   "name", "email", "age",
                                   "isActive"
                               };

            foreach (var prop in missingProps)
            {
                Assert.That.Any(diffs,
                                d => d.MemberPath == prop,
                                predicateDescription: $"a difference for the property '{prop}'",
                                because: $"'{prop}' is expected but absent from the current response. {MissingBecause}",
                                fix: $"Check that JsonDiffer iterates the expected document's properties - a missing '{prop}' here means it never looked for it on the current side.");
            }
        }

        [TestMethod]
        public void ShouldDetect_MissingNestedPropertyInCurrent()
        {
            // Expected: nested object with "city"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "user": {
                                                        "name": "Alice",
                                                        "city": "Berlin"
                                                    }
                                                }
                                                """;

            // Current: missing "city" in nested object
            var current = /*lang=json,strict*/ """
                                               {
                                                   "user": {
                                                       "name": "Alice"
                                                   }
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only the nested 'city' is missing - the surrounding 'user' object still matches and must not be reported as a difference of its own. {MissingBecause}",
                                 fix: $"{CountFix} A finding on 'user' instead of 'user.city' means JsonDiffer compares objects wholesale rather than descending into them.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "user.city" &&
                                 d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'user.city' as MissingInSecond",
                            because: $"{MissingBecause} The dotted path is what lets the author find the property inside a nested object instead of hunting through the payload.",
                            fix: $"Check that JsonDiffer joins parent and child with '.' while descending. {DirectionFix}");
        }

        [TestMethod]
        public void ShouldDetect_MissingPropertyInArrayElement()
        {
            // Expected: array element with "status"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "items": [
                                                        {
                                                            "id": 1,
                                                            "name": "Item1",
                                                            "status": "active"
                                                        }
                                                    ]
                                                }
                                                """;

            // Current: array element missing "status"
            var current = /*lang=json,strict*/ """
                                               {
                                                   "items": [
                                                       {
                                                           "id": 1,
                                                           "name": "Item1"
                                                       }
                                                   ]
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: $"Only the element's missing 'status' differs - the array itself has the same length and must not be reported on top. {MissingBecause}",
                                 fix: $"{CountFix} A second finding on 'items' means JsonDiffer compares array elements wholesale instead of descending into them.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "items[0].status" &&
                                 d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'items[0].status' as MissingInSecond",
                            because: $"{MissingBecause} The path has to carry both the index and the property name - and because it does not end with ']' it correctly counts as a schema mismatch, unlike a pure array length change.",
                            fix: $"Check that JsonDiffer appends '[index]' and then keeps appending the property name. {DirectionFix}");
        }

        #endregion

        #region Combined Scenarios (Both MissingInFirst and MissingInSecond)

        [TestMethod]
        public void ShouldDetect_BothExtraAndMissingProperties()
        {
            // Expected: has "id" and "name"
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "name": "Test"
                                                }
                                                """;

            // Current: missing "name", has extra "email"
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "email": "test@example.com"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: "One property was dropped and another added at the same time, so both directions have to show up - finding only one would hide half the contract change.",
                                 fix: CountFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "name" &&
                                 d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'name' as MissingInSecond",
                            because: MissingBecause,
                            fix: DirectionFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "email" &&
                                 d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'email' as MissingInFirst",
                            because: ExtraBecause,
                            fix: DirectionFix);
        }

        [TestMethod]
        public void ShouldDetect_ComplexMixedMismatches()
        {
            // Expected: specific structure
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "user": {
                                                        "name": "Alice",
                                                        "role": "admin"
                                                    },
                                                    "tags": ["test"]
                                                }
                                                """;

            // Current: different structure
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "user": {
                                                       "name": "Bob",
                                                       "email": "bob@example.com"
                                                   },
                                                   "status": "active"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            // Expected differences:
            // 1. user.name: ValueDifference (Alice vs Bob)
            // 2. user.role: MissingInSecond (in expected, not in current)
            // 3. user.email: MissingInFirst (in current, not in expected)
            // 4. tags: MissingInSecond (in expected, not in current)
            // 5. status: MissingInFirst (in current, not in expected)

            Assert.That.IsGreaterThanOrEqual(diffs.Count,
                                             5,
                                             because: "All three kinds of mismatch occur here at once, nested and at the root. The differ has to keep going after each one - stopping early is what makes an author chase the same payload five times.",
                                             fix: CountFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "user.name" && d.MismatchType == MismatchType.ValueDifference,
                            predicateDescription: "'user.name' as ValueDifference",
                            because: "'name' exists on both sides with different content, so it is a plain value difference even though the document around it changed shape.",
                            fix: "A value change nested inside an otherwise reshaped object must not be swallowed by the surrounding structural findings.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "user.role" && d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'user.role' as MissingInSecond",
                            because: MissingBecause,
                            fix: DirectionFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "user.email" && d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'user.email' as MissingInFirst",
                            because: $"{ExtraBecause} Here it sits in the same nested object that also lost a property - both have to be reported side by side.",
                            fix: DirectionFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "tags" && d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'tags' as MissingInSecond",
                            because: $"{MissingBecause} An array-valued property that disappeared entirely has to be reported at the property, not at its elements.",
                            fix: DirectionFix);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "status" && d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'status' as MissingInFirst",
                            because: $"{ExtraBecause} This one sits at the root, so the differ has to report root-level and nested changes in the same run.",
                            fix: DirectionFix);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void ShouldDetect_EmptyExpectedVsFullCurrent()
        {
            // Expected: empty object
            var expected = /*lang=json,strict*/ """{}""";

            // Current: has properties
            var current = /*lang=json,strict*/ """
                                               {
                                                   "id": 1,
                                                   "name": "Test"
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: $"An empty expected object against a populated response has to yield one finding per property - collapsing them into a single 'objects differ' finding would tell the author nothing. {ExtraBecause}",
                                 fix: $"{CountFix} An empty object on one side must not short-circuit the property walk.");

            Assert.That.All(diffs,
                            d => d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "every difference classified as MissingInFirst",
                            because: "With nothing on the expected side there is nothing to compare values against - every finding can only be a property the current side added.",
                            fix: DirectionFix);
        }

        [TestMethod]
        public void ShouldDetect_FullExpectedVsEmptyCurrent()
        {
            // Expected: has properties
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "id": 1,
                                                    "name": "Test"
                                                }
                                                """;

            // Current: empty object
            var current = /*lang=json,strict*/ """{}""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: $"A populated expected object against an empty response has to yield one finding per property - this is the shape an endpoint returning nothing produces, and the author needs to see exactly what is gone. {MissingBecause}",
                                 fix: $"{CountFix} An empty object on one side must not short-circuit the property walk.");

            Assert.That.All(diffs,
                            d => d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "every difference classified as MissingInSecond",
                            because: "With nothing on the current side there is nothing to compare values against - every finding can only be a property the api stopped returning.",
                            fix: DirectionFix);
        }

        [TestMethod]
        public void ShouldDetect_DeepNestedMismatches()
        {
            // Expected: deeply nested
            var expected = /*lang=json,strict*/ """
                                                {
                                                    "level1": {
                                                        "level2": {
                                                            "level3": {
                                                                "expectedProp": "value"
                                                            }
                                                        }
                                                    }
                                                }
                                                """;

            // Current: has different nested structure
            var current = /*lang=json,strict*/ """
                                               {
                                                   "level1": {
                                                       "level2": {
                                                           "level3": {
                                                               "currentProp": "value"
                                                           }
                                                       }
                                                   }
                                               }
                                               """;

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: "Three levels down, one property was dropped and another added. The intermediate objects are identical and must not be reported as differences of their own.",
                                 fix: $"{CountFix} Findings on 'level1' or 'level1.level2' mean JsonDiffer stops descending as soon as two objects are not reference-equal.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "level1.level2.level3.expectedProp" &&
                                 d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'level1.level2.level3.expectedProp' as MissingInSecond",
                            because: $"{MissingBecause} At this depth the full dotted path is the only thing that makes the finding actionable - 'expectedProp' alone would not say where to look.",
                            fix: $"Check that JsonDiffer keeps appending to the path at every level instead of resetting it. {DirectionFix}");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "level1.level2.level3.currentProp" &&
                                 d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'level1.level2.level3.currentProp' as MissingInFirst",
                            because: $"{ExtraBecause} Both directions have to be reported at the same depth in one pass.",
                            fix: $"Check that JsonDiffer keeps appending to the path at every level instead of resetting it. {DirectionFix}");
        }

        #endregion
    }
}