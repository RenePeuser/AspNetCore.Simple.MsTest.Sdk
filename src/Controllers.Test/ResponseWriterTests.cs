//using System;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json.Linq;

//namespace AspNetCore.Simple.MsTest.Sdk.Test
//{
//    [TestClass]
//    [TestCategory("ResponseWriter")]
//    public sealed class ResponseWriterTests
//    {
//        private static IJsonPathWriter _jsonPathWriter;
//        private static ISpecificResponseWriter specificResponseWriter;

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext context)
//        {
//            var serviceCollection = new ServiceCollection();
//            serviceCollection.AddJsonPathWriter();

//            _jsonPathWriter = serviceCollection.BuildServiceProvider().GetRequiredService<IJsonPathWriter>();
//        }

//        [TestMethod]
//        public void JsonPathWriterAddOrUpdateShouldUpdatePropertiesAndArrayItems()
//        {
//            var root = JToken.Parse("""
//                                    {
//                                        "name": "old",
//                                        "items": [
//                                            {
//                                                "value": 1
//                                            }
//                                        ],
//                                        "content": {
//                                            "headers": []
//                                        }
//                                    }
//                                    """);

//            _jsonPathWriter.AddOrUpdate(root, "name", JToken.FromObject("new"));
//            _jsonPathWriter.AddOrUpdate(root, "items[1]", JToken.Parse("{ \"value\": 2 }"));
//            _jsonPathWriter.AddOrUpdate(root, "content.headers[0]", JToken.Parse("{ \"key\": \"Content-Type\" }"));

//            Assert.AreEqual("new", root["name"]?.ToString());
//            Assert.AreEqual(2, root["items"]?.Count());
//            Assert.AreEqual("2", root["items"]?[1]?["value"]?.ToString());
//            Assert.AreEqual("Content-Type", root["content"]?["headers"]?[0]?["key"]?.ToString());
//        }

//        [TestMethod]
//        public void OverwriteAllResponseWriterShouldNotCorruptExistingPlaceholders()
//        {
//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "are": "$Are$",
//                                                       "status": "$Status$"
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();
//            try
//            {
//                File.WriteAllText(tempFile, "{}");

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
//                var overwriteWriter = new OverwriteAllResponseWriter();

//                overwriteWriter.Write(new WriteResponseRequest
//                {
//                    CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                    CurrentResponseAsString = currentJson,
//                    ExpectedResult = expectedInfo,
//                    Parameters = [("$Are$", "X")],
//                    DifferenceFunc = diffs => diffs,
//                    Mode = ResponseWriteMode.OverwriteAll
//                });

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                Assert.AreEqual("$Are$", updated["are"]?.ToString());
//                Assert.AreEqual("$Status$", updated["status"]?.ToString());
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void JsonPathWriterRemoveShouldRemovePropertiesAndArrayItems()
//        {
//            var writer = new JsonPathWriter();

//            var root = JToken.Parse("""
//                                    {
//                                        "name": "value",
//                                        "items": [
//                                            {
//                                                "value": 1
//                                            },
//                                            {
//                                                "value": 2
//                                            }
//                                        ],
//                                        "content": {
//                                            "headers[0]": {
//                                                "key": "Content-Type"
//                                            }
//                                        }
//                                    }
//                                    """);

//            writer.Remove(root, "name");
//            writer.Remove(root, "items[0]");
//            writer.Remove(root, "content.headers[0]");

//            Assert.IsNull(root["name"]);
//            Assert.AreEqual(1, root["items"]?.Count());
//            Assert.AreEqual("2", root["items"]?[0]?["value"]?.ToString());
//            Assert.IsNull(root["content"]?["headers[0]"]);
//        }

//        [TestMethod]
//        public void DifferenceResponseWriterShouldSyncExpectedToCurrentExceptIgnoredDifferences()
//        {
//            var expectedJson = /*lang=json,strict*/ """
//                                                    {
//                                                        "id": 1,
//                                                        "name": "Old",
//                                                        "obsolete": "remove",
//                                                        "items": [
//                                                            {
//                                                                "value": 1
//                                                            }
//                                                        ]
//                                                    }
//                                                    """;

//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "id": 2,
//                                                       "name": "New",
//                                                       "added": "yes",
//                                                       "items": [
//                                                           {
//                                                               "value": 1,
//                                                               "extra": true
//                                                           }
//                                                       ]
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(tempFile, expectedJson);

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", expectedJson, new FileInfo(tempFile));
//                var writer = new DifferenceResponseWriter(new JsonDiffer(), new JsonPathWriter());
//                var originalDifferenceFunc = AssertObjectExtensions.DifferenceFunc;

//                try
//                {
//                    AssertObjectExtensions.DifferenceFunc = differences => differences;

//                    writer.Write(new WriteResponseRequest
//                                 {
//                                     CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                                     CurrentResponseAsString = currentJson,
//                                     ExpectedResult = expectedInfo,
//                                     Parameters = Array.Empty<(string key, object? Value)>(),
//                                     DifferenceFunc = diffs => diffs.Where(d => !string.Equals(d.MemberPath, "id", StringComparison.OrdinalIgnoreCase)),
//                                     Mode = ResponseWriteMode.DifferencesOnly
//                                 });
//                }
//                finally
//                {
//                    AssertObjectExtensions.DifferenceFunc = originalDifferenceFunc;
//                }

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                var expectedUpdated = JToken.Parse("""
//                                                   {
//                                                       "id": 1,
//                                                       "name": "New",
//                                                       "added": "yes",
//                                                       "items": [
//                                                           {
//                                                               "value": 1,
//                                                               "extra": true
//                                                           }
//                                                       ]
//                                                   }
//                                                   """);

//                Assert.IsTrue(JToken.DeepEquals(expectedUpdated, updated));
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void DifferenceResponseWriterShouldRestoreIgnoredArrayEntriesFromExpected()
//        {
//            var expectedJson = /*lang=json,strict*/ """
//                                                   {
//                                                        "items": [
//                                                            {
//                                                                "id": 1,
//                                                                "value": "keep"
//                                                            },
//                                                            {
//                                                                "id": 2,
//                                                                "value": "expected"
//                                                            }
//                                                        ]
//                                                    }
//                                                   """;

//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "items": [
//                                                           {
//                                                               "id": 1,
//                                                               "value": "current"
//                                                           },
//                                                           {
//                                                               "id": 2,
//                                                               "value": "current"
//                                                           },
//                                                           {
//                                                               "id": 3,
//                                                               "value": "added"
//                                                           }
//                                                       ]
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(tempFile, expectedJson);

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", expectedJson, new FileInfo(tempFile));
//                var writer = new DifferenceResponseWriter(new JsonDiffer(), new JsonPathWriter());
//                var originalDifferenceFunc = AssertObjectExtensions.DifferenceFunc;

//                try
//                {
//                    AssertObjectExtensions.DifferenceFunc = differences => differences;

//                    writer.Write(new WriteResponseRequest
//                                 {
//                                     CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                                     CurrentResponseAsString = currentJson,
//                                     ExpectedResult = expectedInfo,
//                                     Parameters = Array.Empty<(string key, object? Value)>(),
//                                     DifferenceFunc = diffs => diffs.Where(d => !d.MemberPath.StartsWith("items[1]", StringComparison.OrdinalIgnoreCase)),
//                                     Mode = ResponseWriteMode.DifferencesOnly
//                                 });
//                }
//                finally
//                {
//                    AssertObjectExtensions.DifferenceFunc = originalDifferenceFunc;
//                }

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                var expectedUpdated = JToken.Parse("""
//                                                   {
//                                                       "items": [
//                                                           {
//                                                               "id": 1,
//                                                               "value": "current"
//                                                           },
//                                                           {
//                                                               "id": 2,
//                                                               "value": "expected"
//                                                           },
//                                                           {
//                                                               "id": 3,
//                                                               "value": "added"
//                                                           }
//                                                       ]
//                                                   }
//                                                   """);

//                Assert.IsTrue(JToken.DeepEquals(expectedUpdated, updated));
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void OverwriteAllResponseWriterShouldWriteCurrentResponseWithSmartReplacements()
//        {
//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "userId": "123",
//                                                       "message": "User 123",
//                                                       "value": 5
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(tempFile, "{}");

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
//                var writer = new OverwriteAllResponseWriter();

//                writer.Write(new WriteResponseRequest
//                             {
//                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                                 CurrentResponseAsString = currentJson,
//                                 ExpectedResult = expectedInfo,
//                                 Parameters = [("$userId", "123")],
//                                 DifferenceFunc = diffs => diffs,
//                                 Mode = ResponseWriteMode.OverwriteAll
//                             });

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                Assert.AreEqual("$userId", updated["userId"]?.ToString());
//                Assert.AreEqual("User $userId", updated["message"]?.ToString());
//                Assert.AreEqual("5", updated["value"]?.ToString());
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void OverwriteAllResponseWriterShouldReplaceHyphenatedSubstrings()
//        {
//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "message": "AWS S3 Bucket name 'rps-lenovo-p16-1234-sdc' is already in used by: Type"
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(tempFile, "{}");

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
//                var writer = new OverwriteAllResponseWriter();

//                writer.Write(new WriteResponseRequest
//                             {
//                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                                 CurrentResponseAsString = currentJson,
//                                 ExpectedResult = expectedInfo,
//                                 Parameters = [("$mayvar$", "rps-lenovo-p16")],
//                                 DifferenceFunc = diffs => diffs,
//                                 Mode = ResponseWriteMode.OverwriteAll
//                             });

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                Assert.AreEqual("AWS S3 Bucket name '$mayvar$-1234-sdc' is already in used by: Type",
//                                updated["message"]?.ToString());
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void OverwriteAllResponseWriterShouldReplaceNonStringPropertyValues()
//        {
//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "id": 1,
//                                                       "name": "Goku",
//                                                       "age": 42,
//                                                       "active": true
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(tempFile, "{}");

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
//                var writer = new OverwriteAllResponseWriter();

//                writer.Write(new WriteResponseRequest
//                             {
//                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                                 CurrentResponseAsString = currentJson,
//                                 ExpectedResult = expectedInfo,
//                                 Parameters = [("$Age$", 42), ("$Active$", true)],
//                                 DifferenceFunc = diffs => diffs,
//                                 Mode = ResponseWriteMode.OverwriteAll
//                             });

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                Assert.AreEqual("$Age$", updated["age"]?.ToString());
//                Assert.AreEqual("$Active$", updated["active"]?.ToString());
//                Assert.AreEqual("1", updated["id"]?.ToString());
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }

//        [TestMethod]
//        public void OverwriteAllResponseWriterShouldNotReplaceShortValuesInsideOtherStrings()
//        {
//            var currentJson = /*lang=json,strict*/ """
//                                                   {
//                                                       "status": "A",
//                                                       "role": "Admin",
//                                                       "are": "$Are$",
//                                                       "template": "$Status$re$",
//                                                       "createdAt": "2025-09-01T06:45:29.187544Z"
//                                                   }
//                                                   """;

//            var tempFile = Path.GetTempFileName();
//            try
//            {
//                File.WriteAllText(tempFile, "{}");

//                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
//                var writer = new OverwriteAllResponseWriter();

//                writer.Write(new WriteResponseRequest
//                {
//                    CallingAssembly = typeof(ResponseWriterTests).Assembly,
//                    CurrentResponseAsString = currentJson,
//                    ExpectedResult = expectedInfo,
//                    Parameters = [("$Status$", "A")],
//                    DifferenceFunc = diffs => diffs,
//                    Mode = ResponseWriteMode.OverwriteAll
//                });

//                var updated = JToken.Parse(File.ReadAllText(tempFile));

//                Assert.AreEqual("$Status$", updated["status"]?.ToString());
//                Assert.AreEqual("Admin", updated["role"]?.ToString());
//                Assert.AreEqual("$Are$", updated["are"]?.ToString());
//                Assert.AreEqual("$Status$re$", updated["template"]?.ToString());

//                var createdAt = updated["createdAt"]?.ToObject<DateTime>();
//                var expectedCreatedAt = DateTime.Parse("2025-09-01T06:45:29.187544Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
//                Assert.AreEqual(expectedCreatedAt, createdAt);
//            }
//            finally
//            {
//                File.Delete(tempFile);
//            }
//        }
//    }
//}

