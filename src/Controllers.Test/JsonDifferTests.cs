using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Controllers.Test
{
    [TestClass]
    [TestCategory("JsonDiffer")]
    public sealed class JsonDifferTests
    {
        private const string NormalizationFix = "Check the normalization in JsonDiffer.FindDifferences: it has to compare parsed JTokens instead of raw strings, and object property order must not count as a difference. Whatever shows up in the Details above is a formatting artefact that leaked into the comparison.";

        private static IJsonDiffer _jsonDiffer = null!;

        [TestInitialize]
        public void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddJsonDiffer();
            _jsonDiffer = serviceCollection.BuildServiceProvider().GetRequiredService<IJsonDiffer>();
        }

        [TestMethod]
        public void FindDifferencesShouldDetectValueDifferences()
        {
            var left = JToken.Parse("""
                                    {
                                        "name": "old",
                                        "age": 1
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                         "name": "new",
                                         "age": 1
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.That.HasCount(1,
                                 diffs,
                                 because: "Only 'name' differs; 'age' is identical on both sides. A second entry would mean the differ reports untouched properties as well and would drown every real finding.",
                                 fix: "Check the leaf comparison in JsonDiffer.FindDifferences - equal values must produce no Difference at all.");

            Assert.That.AreEqual("name",
                                 diffs[0].MemberPath,
                                 because: "The member path is what the author uses to find the spot in the payload, so it has to name the property that actually changed.",
                                 fix: "Check how JsonDiffer builds MemberPath while descending - at the root level it is the plain property name, without a leading dot or '$'.");

            Assert.That.AreEqual(MismatchType.ValueDifference,
                                 diffs[0].MismatchType,
                                 because: "'name' exists on both sides with different content, so the schema is intact and only the value changed. Any other classification would make an ordinary data change look like a contract break.",
                                 fix: "JsonDiffer must reserve MissingInFirst/MissingInSecond for properties that are really absent on one side.");
        }

        [TestMethod]
        public void FindDifferencesShouldDetectMissingPropertiesOnEachSide()
        {
            var left = JToken.Parse("""
                                    {
                                        "name": "value",
                                        "onlyLeft": true
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                         "name": "value",
                                         "onlyRight": 1 
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: "Each side carries one property the other does not, so exactly two findings are expected - one per direction. 'name' is identical and must not add a third.",
                                 fix: "Check that JsonDiffer walks both property sets and reports each missing property exactly once.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "onlyLeft" && d.MismatchType == MismatchType.MissingInSecond,
                            predicateDescription: "'onlyLeft' reported as MissingInSecond",
                            because: "'onlyLeft' exists in the first document only, so it is missing in the second. Getting the direction wrong inverts every message the sdk prints about it.",
                            fix: "Check the argument order in JsonDiffer.FindDifferences: MissingInSecond means absent from json2, which is the right-hand document here.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "onlyRight" && d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'onlyRight' reported as MissingInFirst",
                            because: "'onlyRight' exists in the second document only, so it is missing in the first. Both directions have to be reported, otherwise half of a contract change stays invisible.",
                            fix: "Check that JsonDiffer also iterates the second document's properties - reporting only one direction is the classic omission here.");
        }

        [TestMethod]
        public void FindDifferencesShouldReportArrayDifferencesWithIndexPaths()
        {
            var left = JToken.Parse("""
                                    {
                                        "items": [
                                            {
                                                "value": 1
                                            }
                                        ]
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                         "items": [
                                             {
                                                 "value": 2
                                             },
                                             {
                                                 "value": 3
                                             }
                                         ]
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.That.Any(diffs,
                            d => d.MemberPath == "items[0].value" && d.MismatchType == MismatchType.ValueDifference,
                            predicateDescription: "'items[0].value' reported as ValueDifference",
                            because: "Inside an array the path has to carry the index and continue into the element, otherwise the author cannot tell which of many elements changed.",
                            fix: "Check that JsonDiffer appends '[index]' when descending into an array and then keeps appending the property name.");

            Assert.That.Any(diffs,
                            d => d.MemberPath == "items[1]" && d.MismatchType == MismatchType.MissingInFirst,
                            predicateDescription: "'items[1]' reported as MissingInFirst",
                            because: "The right-hand array has a second element the left one does not. A surplus element is reported at the index itself - that trailing ']' is what later excludes it from the schema-mismatch rule.",
                            fix: "Check the array length handling in JsonDiffer: a surplus element has to be reported as MissingInFirst at path 'items[1]', not as a difference on 'items'.");
        }

        [TestMethod]
        public void FindDifferencesShouldDetectPrimitiveIntegerDifferences()
        {
            // Test for bug fix: Primitive values should be detected as differences
            var diffs = _jsonDiffer.FindDifferences("69", "42");

            Assert.HasCount(1, diffs);
            Assert.AreEqual("", diffs[0].MemberPath); // Root level primitive
            Assert.AreEqual("69", diffs[0].Value1);
            Assert.AreEqual("42", diffs[0].Value2);
            Assert.AreEqual(MismatchType.ValueDifference, diffs[0].MismatchType);
        }

        [TestMethod]
        public void FindDifferencesShouldHandleComplexNestedStructures()
        {
            var left = JToken.Parse("""
                                    {
                                       "content":{
                                          "headers":[
                                             {
                                                "key":"Content-Type",
                                                "value":[
                                                   "application/json; charset=utf-8"
                                                ]
                                             }
                                          ],
                                          "value":{
                                             "capability":{
                                                "properties":{
                                                   "lifecycleRules":[
                                                      
                                                   ],
                                                   "name":"rps-lenovo-p16-8610-sdc",
                                                   "region":"eu-west-1",
                                                   "tags":[
                                                      
                                                   ],
                                                   "versioningEnabled":false
                                                },
                                                "id":"ceda6a6d-99bb-4763-a162-28a68d172411",
                                                "providerType":"Aws",
                                                "lastUpdatedByUser":"2ftebv7pq5qgh2aj5b0chilbis",
                                                "lastModifiedAt":"2026-02-16T14:35:29.6755705+00:00",
                                                "createdAt":"2026-02-16T14:35:29.6755705+00:00",
                                                "createdBy":"2ftebv7pq5qgh2aj5b0chilbis",
                                                "deploymentId":"00000000-0000-0000-0000-000000000000",
                                                "type":"AWS_S3_BUCKET",
                                                "stage":"Workbench",
                                                "name":"RPS-LENOVO-P16",
                                                "isDeployed":false
                                             }
                                          }
                                       },
                                       "statusCode":"OK",
                                       "headers":[
                                          {
                                             "key":"api-supported-versions",
                                             "value":[
                                                "1"
                                             ]
                                          }
                                       ],
                                       "trailingHeaders":[
                                          
                                       ],
                                       "isSuccessStatusCode":true
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                        "content":{
                                           "headers":[
                                              {
                                                 "key":"Content-Type",
                                                 "value":[
                                                    "application/json; charset=utf-8"
                                                 ]
                                              }
                                           ],
                                           "value":{
                                              "capability":{
                                                 "properties":{
                                                    "lifecycleRules":[
                                                       
                                                    ],
                                                    "name":"rps-lenovo-p16-1234-sdc",
                                                    "region":"eu-west-1",
                                                    "tags":[
                                                       
                                                    ],
                                                    "versioningEnabled":false
                                                 },
                                                 "id":"fb84b84c-0016-44cd-b05b-27a04e10e035",
                                                 "providerType":"Aws",
                                                 "lastUpdatedByUser":"2ftebv7pq5qgh2aj5b0chilbis",
                                                 "lastModifiedAt":"2026-01-24T20:09:11.8010206\u002B01:00",
                                                 "createdAt":"2026-01-24T20:09:11.8010206\u002B01:00",
                                                 "createdBy":"2ftebv7pq5qgh2aj5b0chilbis",
                                                 "deploymentId":"00000000-0000-0000-0000-000000000000",
                                                 "type":"AWS_S3_BUCKET",
                                                 "stage":"Workbench",
                                                 "name":"RPS-LENOVO-P16-BAD",
                                                 "isDeployed":false
                                              }
                                           }
                                        },
                                        "statusCode":"OK",
                                        "headers":[
                                           {
                                              "key":"api-supported-versions",
                                              "value":[
                                                 "1"
                                              ]
                                           }
                                        ],
                                        "trailingHeaders":[
                                           
                                        ],
                                        "isSuccessStatusCode":true
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.HasCount(5, diffs);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreDifferentPropertyOrder()
        {
            // Same properties, different order - should report no differences
            var left = JToken.Parse("""
                                    {
                                        "name": "test",
                                        "age": 25,
                                        "city": "Berlin"
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                         "city": "Berlin",
                                         "name": "test",
                                         "age": 25
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Property order should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreDifferentWhitespaceFormatting()
        {
            // Same content, different formatting - should report no differences
            var leftCompact = /*lang=json,strict*/ """{"name":"test","nested":{"value":42}}""";

            var rightFormatted = /*lang=json,strict*/ """
                                                      {
                                                        "name": "test",
                                                        "nested": {
                                                          "value": 42
                                                        }
                                                      }
                                                      """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Whitespace formatting should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnorePropertyOrderInNestedObjects()
        {
            // Nested objects with different property orders
            var left = JToken.Parse("""
                                    {
                                        "user": {
                                            "name": "Alice",
                                            "address": {
                                                "city": "Berlin",
                                                "street": "Main St",
                                                "zip": "10115"
                                            },
                                            "age": 30
                                        }
                                    }
                                    """);

            var right = JToken.Parse("""
                                     {
                                         "user": {
                                             "age": 30,
                                             "address": {
                                                 "zip": "10115",
                                                 "city": "Berlin",
                                                 "street": "Main St"
                                             },
                                             "name": "Alice"
                                         }
                                     }
                                     """);

            var diffs = _jsonDiffer.FindDifferences(left, right);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Property order in nested objects should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldDetectActualDifferencesRegardlessOfFormatting()
        {
            // Different values with different formatting - should detect the value difference
            var leftCompact = /*lang=json,strict*/ """{"name":"Alice","age":25}""";

            var rightFormatted = /*lang=json,strict*/ """
                                                      {
                                                        "name": "Bob",
                                                        "age": 25
                                                      }
                                                      """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.HasCount(1, diffs);
            Assert.AreEqual("name", diffs[0].MemberPath);
            Assert.AreEqual("Alice", diffs[0].Value1);
            Assert.AreEqual("Bob", diffs[0].Value2);
            Assert.AreEqual(MismatchType.ValueDifference, diffs[0].MismatchType);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreArrayFormattingWhenContentIsIdentical()
        {
            // Note: Arrays are order-sensitive by design, so this test verifies
            // that identical arrays (same order) are treated as equal regardless of formatting
            var leftCompact = /*lang=json,strict*/ """{"items":[1,2,3]}""";

            var rightFormatted = /*lang=json,strict*/ """
                                                      {
                                                        "items": [
                                                          1,
                                                          2,
                                                          3
                                                        ]
                                                      }
                                                      """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Array formatting should not affect comparison when order is same. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldDetectArrayOrderDifferences()
        {
            // Arrays with different order should be detected as different
            var left = JToken.Parse("""{"items":[1,2,3]}""");
            var right = JToken.Parse("""{"items":[3,2,1]}""");

            var diffs = _jsonDiffer.FindDifferences(left, right);

            // Should detect differences at index 0 and 2
            Assert.IsTrue(diffs.Count >= 2, "Array order differences should be detected");
            Assert.IsTrue(diffs.Any(d => d.MemberPath.Contains("[0]")));
            Assert.IsTrue(diffs.Any(d => d.MemberPath.Contains("[2]")));
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreArrayWhitespaceVariations()
        {
            // Compact array vs. array with newlines and spaces
            var leftCompact = """[1]""";

            var rightFormatted = """
                                 [
                                   1
                                 ]
                                 """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Array whitespace variations should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreMultipleArrayElementWhitespace()
        {
            // Multiple elements with different whitespace
            var leftCompact = """[1,2,3]""";

            var rightFormatted = """
                                 [
                                   1,
                                   2,
                                   3
                                 ]
                                 """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Array element whitespace should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreArrayOfObjectsWhitespace()
        {
            // Array of objects with different whitespace
            var leftCompact = /*lang=json,strict*/ """[{"id":1,"name":"test"},{"id":2,"name":"demo"}]""";

            var rightFormatted = /*lang=json,strict*/ """
                                                      [
                                                        {
                                                          "id": 1,
                                                          "name": "test"
                                                        },
                                                        {
                                                          "id": 2,
                                                          "name": "demo"
                                                        }
                                                      ]
                                                      """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Array of objects whitespace should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreEmptyArrayWhitespace()
        {
            // Empty arrays with different whitespace
            var leftCompact = /*lang=json,strict*/ """{"items":[]}""";
            var rightWithSpace = /*lang=json,strict*/ """{"items":[ ]}""";

            var rightWithNewline = /*lang=json,strict*/ """
                                                        {
                                                          "items": [

                                                          ]
                                                        }
                                                        """;

            var diffs1 = _jsonDiffer.FindDifferences(leftCompact, rightWithSpace);
            var diffs2 = _jsonDiffer.FindDifferences(leftCompact, rightWithNewline);

            Assert.That.HasCount(0,
                                 diffs1,
                                 because: "Empty array with space should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
            Assert.That.HasCount(0,
                                 diffs2,
                                 because: "Empty array with newline should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreNestedArrayWhitespace()
        {
            // Nested arrays with different whitespace
            var leftCompact = /*lang=json,strict*/ """{"matrix":[[1,2],[3,4]]}""";

            var rightFormatted = /*lang=json,strict*/ """
                                                      {
                                                        "matrix": [
                                                          [
                                                            1,
                                                            2
                                                          ],
                                                          [
                                                            3,
                                                            4
                                                          ]
                                                        ]
                                                      }
                                                      """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Nested array whitespace should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }

        [TestMethod]
        public void FindDifferencesShouldDetectActualArrayValueDifferencesRegardlessOfWhitespace()
        {
            // Different values with different whitespace - should detect the difference
            var leftCompact = """[1,2,3]""";

            var rightFormatted = """
                                 [
                                   1,
                                   99,
                                   3
                                 ]
                                 """;

            var diffs = _jsonDiffer.FindDifferences(leftCompact, rightFormatted);

            Assert.HasCount(1, diffs);
            Assert.AreEqual("[1]", diffs[0].MemberPath);
            Assert.AreEqual("2", diffs[0].Value1);
            Assert.AreEqual("99", diffs[0].Value2);
            Assert.AreEqual(MismatchType.ValueDifference, diffs[0].MismatchType);
        }

        [TestMethod]
        public void FindDifferencesShouldIgnoreMixedWhitespaceInComplexStructure()
        {
            // Complex structure with mixed whitespace styles
            var leftMixed = /*lang=json,strict*/ """
                                                 {
                                                   "user": {"name":"Alice","age":30},
                                                   "items":[1,2,3],
                                                   "nested": {
                                                     "values": [
                                                       {"id":1},{"id":2}
                                                     ]
                                                   }
                                                 }
                                                 """;

            var rightMixed = /*lang=json,strict*/ """
                                                  {"user":{"name":"Alice","age":30},"items":[
                                                    1,
                                                    2,
                                                    3
                                                  ],"nested":{"values":[{"id":1},{"id":2}]}}
                                                  """;

            var diffs = _jsonDiffer.FindDifferences(leftMixed, rightMixed);

            Assert.That.HasCount(0,
                                 diffs,
                                 because: "Mixed whitespace styles should not affect comparison. Formatting is not data - a snapshot written with different indentation, line breaks or property order still describes the same response, so reporting a difference here would make every reformatted snapshot fail.",
                                 fix: NormalizationFix);
        }
    }
}