using System.Linq;
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

            Assert.HasCount(1, diffs, "Should detect one extra property in current");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "email" &&
                                        d.MismatchType == MismatchType.MissingInFirst &&
                                        d.Value1 == null &&
                                        d.Value2 != null),
                          "Should detect 'email' as MissingInFirst (present in current, missing in expected)");
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

            Assert.HasCount(4, diffs, "Should detect 4 extra properties in current");

            Assert.IsTrue(diffs.All(d => d.MismatchType == MismatchType.MissingInFirst),
                          "All should be MissingInFirst");

            var extraProps = new[]
                             {
                                 "name", "email", "age",
                                 "isActive"
                             };

            foreach (var prop in extraProps)
            {
                Assert.IsTrue(diffs.Any(d => d.MemberPath == prop),
                              $"Should detect '{prop}' as extra property");
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

            Assert.HasCount(1, diffs, "Should detect one extra nested property");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "user.city" &&
                                        d.MismatchType == MismatchType.MissingInFirst),
                          "Should detect 'user.city' as MissingInFirst");
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

            Assert.HasCount(1, diffs, "Should detect one extra property in array element");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "items[0].status" &&
                                        d.MismatchType == MismatchType.MissingInFirst),
                          "Should detect 'items[0].status' as MissingInFirst");
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

            Assert.HasCount(1, diffs, "Should detect one missing property in current");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "email" &&
                                        d.MismatchType == MismatchType.MissingInSecond &&
                                        d.Value1 != null &&
                                        d.Value2 == null),
                          "Should detect 'email' as MissingInSecond (present in expected, missing in current)");
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

            Assert.HasCount(4, diffs, "Should detect 4 missing properties in current");

            Assert.IsTrue(diffs.All(d => d.MismatchType == MismatchType.MissingInSecond),
                          "All should be MissingInSecond");

            var missingProps = new[]
                               {
                                   "name", "email", "age",
                                   "isActive"
                               };

            foreach (var prop in missingProps)
            {
                Assert.IsTrue(diffs.Any(d => d.MemberPath == prop),
                              $"Should detect '{prop}' as missing property");
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

            Assert.HasCount(1, diffs, "Should detect one missing nested property");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "user.city" &&
                                        d.MismatchType == MismatchType.MissingInSecond),
                          "Should detect 'user.city' as MissingInSecond");
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

            Assert.HasCount(1, diffs, "Should detect one missing property in array element");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "items[0].status" &&
                                        d.MismatchType == MismatchType.MissingInSecond),
                          "Should detect 'items[0].status' as MissingInSecond");
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

            Assert.HasCount(2, diffs, "Should detect both missing and extra properties");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "name" &&
                                        d.MismatchType == MismatchType.MissingInSecond),
                          "Should detect 'name' as MissingInSecond");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "email" &&
                                        d.MismatchType == MismatchType.MissingInFirst),
                          "Should detect 'email' as MissingInFirst");
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

            Assert.IsTrue(diffs.Count >= 5, $"Should detect at least 5 differences, found {diffs.Count}");

            Assert.IsTrue(diffs.Any(d => d.MemberPath == "user.name" && d.MismatchType == MismatchType.ValueDifference));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "user.role" && d.MismatchType == MismatchType.MissingInSecond));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "user.email" && d.MismatchType == MismatchType.MissingInFirst));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "tags" && d.MismatchType == MismatchType.MissingInSecond));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "status" && d.MismatchType == MismatchType.MissingInFirst));
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

            Assert.HasCount(2, diffs, "Should detect 2 extra properties");
            Assert.IsTrue(diffs.All(d => d.MismatchType == MismatchType.MissingInFirst));
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

            Assert.HasCount(2, diffs, "Should detect 2 missing properties");
            Assert.IsTrue(diffs.All(d => d.MismatchType == MismatchType.MissingInSecond));
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

            Assert.HasCount(2, diffs, "Should detect 2 nested differences");

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "level1.level2.level3.expectedProp" &&
                                        d.MismatchType == MismatchType.MissingInSecond));

            Assert.IsTrue(diffs.Any(d =>
                                        d.MemberPath == "level1.level2.level3.currentProp" &&
                                        d.MismatchType == MismatchType.MissingInFirst));
        }

        #endregion
    }
}