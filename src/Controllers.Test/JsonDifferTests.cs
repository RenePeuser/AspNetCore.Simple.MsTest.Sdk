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

            Assert.HasCount(1, diffs);
            Assert.AreEqual("name", diffs[0].MemberPath);
            Assert.AreEqual(MismatchType.ValueDifference, diffs[0].MismatchType);
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

            Assert.HasCount(2, diffs);
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "onlyLeft" && d.MismatchType == MismatchType.MissingInSecond));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "onlyRight" && d.MismatchType == MismatchType.MissingInFirst));
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

            Assert.IsTrue(diffs.Any(d => d.MemberPath == "items[0].value" && d.MismatchType == MismatchType.ValueDifference));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "items[1]" && d.MismatchType == MismatchType.MissingInFirst));
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

            Assert.HasCount(0, diffs, "Property order should not affect comparison");
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

            Assert.HasCount(0, diffs, "Whitespace formatting should not affect comparison");
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

            Assert.HasCount(0, diffs, "Property order in nested objects should not affect comparison");
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

            Assert.HasCount(0, diffs, "Array formatting should not affect comparison when order is same");
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

            Assert.HasCount(0, diffs, "Array whitespace variations should not affect comparison");
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

            Assert.HasCount(0, diffs, "Array element whitespace should not affect comparison");
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

            Assert.HasCount(0, diffs, "Array of objects whitespace should not affect comparison");
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

            Assert.HasCount(0, diffs1, "Empty array with space should not affect comparison");
            Assert.HasCount(0, diffs2, "Empty array with newline should not affect comparison");
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

            Assert.HasCount(0, diffs, "Nested array whitespace should not affect comparison");
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

            Assert.HasCount(0, diffs, "Mixed whitespace styles should not affect comparison");
        }
    }
}