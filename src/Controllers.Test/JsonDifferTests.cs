using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
