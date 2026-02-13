using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    [TestCategory("ResponseWriter")]
    public sealed class ResponseWriterTests
    {
        [TestMethod]
        public void JsonPathWriterAddOrUpdateShouldUpdatePropertiesAndArrayItems()
        {
            var writer = new JsonPathWriter();

            var root = JToken.Parse("""
                                    {
                                        "name": "old",
                                        "items": [
                                            {
                                                "value": 1
                                            }
                                        ],
                                        "content": {
                                            "headers": []
                                        }
                                    }
                                    """);

            writer.AddOrUpdate(root, "name", JToken.FromObject("new"));
            writer.AddOrUpdate(root, "items[1]", JToken.Parse("{ \"value\": 2 }"));
            writer.AddOrUpdate(root, "content.headers[0]", JToken.Parse("{ \"key\": \"Content-Type\" }"));

            Assert.AreEqual("new", root["name"]?.ToString());
            Assert.AreEqual(2, root["items"]?.Count());
            Assert.AreEqual("2", root["items"]?[1]?["value"]?.ToString());
            Assert.AreEqual("Content-Type", root["content"]?["headers"]?[0]?["key"]?.ToString());
        }

        [TestMethod]
        public void JsonPathWriterRemoveShouldRemovePropertiesAndArrayItems()
        {
            var writer = new JsonPathWriter();

            var root = JToken.Parse("""
                                    {
                                        "name": "value",
                                        "items": [
                                            {
                                                "value": 1
                                            },
                                            {
                                                "value": 2
                                            }
                                        ],
                                        "content": {
                                            "headers[0]": {
                                                "key": "Content-Type"
                                            }
                                        }
                                    }
                                    """);

            writer.Remove(root, "name");
            writer.Remove(root, "items[0]");
            writer.Remove(root, "content.headers[0]");

            Assert.IsNull(root["name"]);
            Assert.AreEqual(1, root["items"]?.Count());
            Assert.AreEqual("2", root["items"]?[0]?["value"]?.ToString());
            Assert.IsNull(root["content"]?["headers[0]"]);
        }

        [TestMethod]
        public void DifferenceResponseWriterShouldSyncExpectedToCurrentExceptIgnoredDifferences()
        {
            var expectedJson = /*lang=json,strict*/ """
                                                    {
                                                        "id": 1,
                                                        "name": "Old",
                                                        "obsolete": "remove",
                                                        "items": [
                                                            {
                                                                "value": 1
                                                            }
                                                        ]
                                                    }
                                                    """;

            var currentJson = /*lang=json,strict*/ """
                                                   {
                                                       "id": 2,
                                                       "name": "New",
                                                       "added": "yes",
                                                       "items": [
                                                           {
                                                               "value": 1,
                                                               "extra": true
                                                           }
                                                       ]
                                                   }
                                                   """;

            var tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, expectedJson);

                var expectedInfo = new EmbeddedFileInfo("Expected.json", expectedJson, new FileInfo(tempFile));
                var writer = new DifferenceResponseWriter(new JsonDiffer(), new JsonPathWriter());
                var originalDifferenceFunc = AssertObjectExtensions.DifferenceFunc;

                try
                {
                    AssertObjectExtensions.DifferenceFunc = differences => differences;

                    writer.Write(new WriteResponseRequest
                                 {
                                     CallingAssembly = typeof(ResponseWriterTests).Assembly,
                                     CurrentResponseAsString = currentJson,
                                     ExpectedResult = expectedInfo,
                                     Parameters = Array.Empty<(string key, object? Value)>(),
                                     DifferenceFunc = diffs => diffs.Where(d => !string.Equals(d.MemberPath, "id", StringComparison.OrdinalIgnoreCase)),
                                     Mode = ResponseWriteMode.DifferencesOnly
                                 });
                }
                finally
                {
                    AssertObjectExtensions.DifferenceFunc = originalDifferenceFunc;
                }

                var updated = JToken.Parse(File.ReadAllText(tempFile));

                var expectedUpdated = JToken.Parse("""
                                                   {
                                                       "id": 1,
                                                       "name": "New",
                                                       "added": "yes",
                                                       "items": [
                                                           {
                                                               "value": 1,
                                                               "extra": true
                                                           }
                                                       ]
                                                   }
                                                   """);

                Assert.IsTrue(JToken.DeepEquals(expectedUpdated, updated));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void DifferenceResponseWriterShouldRestoreIgnoredArrayEntriesFromExpected()
        {
            var expectedJson = /*lang=json,strict*/ """
                                                    {
                                                        "items": [
                                                            {
                                                                "id": 1,
                                                                "value": "keep"
                                                            },
                                                            {
                                                                "id": 2,
                                                                "value": "expected"
                                                            }
                                                        ]
                                                    }
                                                    """;

            var currentJson = /*lang=json,strict*/ """
                                                   {
                                                       "items": [
                                                           {
                                                               "id": 1,
                                                               "value": "current"
                                                           },
                                                           {
                                                               "id": 2,
                                                               "value": "current"
                                                           },
                                                           {
                                                               "id": 3,
                                                               "value": "added"
                                                           }
                                                       ]
                                                   }
                                                   """;

            var tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, expectedJson);

                var expectedInfo = new EmbeddedFileInfo("Expected.json", expectedJson, new FileInfo(tempFile));
                var writer = new DifferenceResponseWriter(new JsonDiffer(), new JsonPathWriter());
                var originalDifferenceFunc = AssertObjectExtensions.DifferenceFunc;

                try
                {
                    AssertObjectExtensions.DifferenceFunc = differences => differences;

                    writer.Write(new WriteResponseRequest
                                 {
                                     CallingAssembly = typeof(ResponseWriterTests).Assembly,
                                     CurrentResponseAsString = currentJson,
                                     ExpectedResult = expectedInfo,
                                     Parameters = Array.Empty<(string key, object? Value)>(),
                                     DifferenceFunc = diffs => diffs.Where(d => !d.MemberPath.StartsWith("items[1]", StringComparison.OrdinalIgnoreCase)),
                                     Mode = ResponseWriteMode.DifferencesOnly
                                 });
                }
                finally
                {
                    AssertObjectExtensions.DifferenceFunc = originalDifferenceFunc;
                }

                var updated = JToken.Parse(File.ReadAllText(tempFile));

                var expectedUpdated = JToken.Parse("""
                                                   {
                                                       "items": [
                                                           {
                                                               "id": 1,
                                                               "value": "current"
                                                           },
                                                           {
                                                               "id": 2,
                                                               "value": "expected"
                                                           },
                                                           {
                                                               "id": 3,
                                                               "value": "added"
                                                           }
                                                       ]
                                                   }
                                                   """);

                Assert.IsTrue(JToken.DeepEquals(expectedUpdated, updated));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void OverwriteAllResponseWriterShouldWriteCurrentResponseWithSmartReplacements()
        {
            var currentJson = /*lang=json,strict*/ """
                                                   {
                                                       "userId": "123",
                                                       "message": "User 123",
                                                       "value": 5
                                                   }
                                                   """;

            var tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, "{}");

                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
                var writer = new OverwriteAllResponseWriter();

                writer.Write(new WriteResponseRequest
                             {
                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
                                 CurrentResponseAsString = currentJson,
                                 ExpectedResult = expectedInfo,
                                 Parameters = [("$userId", "123")],
                                 DifferenceFunc = diffs => diffs,
                                 Mode = ResponseWriteMode.OverwriteAll
                             });

                var updated = JToken.Parse(File.ReadAllText(tempFile));

                Assert.AreEqual("$userId", updated["userId"]?.ToString());
                Assert.AreEqual("User $userId", updated["message"]?.ToString());
                Assert.AreEqual("5", updated["value"]?.ToString());
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void OverwriteAllResponseWriterShouldReplaceHyphenatedSubstrings()
        {
            var currentJson = /*lang=json,strict*/ """
                                                   {
                                                       "message": "AWS S3 Bucket name 'rps-lenovo-p16-1234-sdc' is already in used by: Type"
                                                   }
                                                   """;

            var tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, "{}");

                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
                var writer = new OverwriteAllResponseWriter();

                writer.Write(new WriteResponseRequest
                             {
                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
                                 CurrentResponseAsString = currentJson,
                                 ExpectedResult = expectedInfo,
                                 Parameters = [("$mayvar$", "rps-lenovo-p16")],
                                 DifferenceFunc = diffs => diffs,
                                 Mode = ResponseWriteMode.OverwriteAll
                             });

                var updated = JToken.Parse(File.ReadAllText(tempFile));

                Assert.AreEqual("AWS S3 Bucket name '$mayvar$-1234-sdc' is already in used by: Type",
                                updated["message"]?.ToString());
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void OverwriteAllResponseWriterShouldReplaceNonStringPropertyValues()
        {
            var currentJson = /*lang=json,strict*/ """
                                                   {
                                                       "id": 1,
                                                       "name": "Goku",
                                                       "age": 42,
                                                       "active": true
                                                   }
                                                   """;

            var tempFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFile, "{}");

                var expectedInfo = new EmbeddedFileInfo("Expected.json", "{}", new FileInfo(tempFile));
                var writer = new OverwriteAllResponseWriter();

                writer.Write(new WriteResponseRequest
                             {
                                 CallingAssembly = typeof(ResponseWriterTests).Assembly,
                                 CurrentResponseAsString = currentJson,
                                 ExpectedResult = expectedInfo,
                                 Parameters = [("$Age$", 42), ("$Active$", true)],
                                 DifferenceFunc = diffs => diffs,
                                 Mode = ResponseWriteMode.OverwriteAll
                             });

                var updated = JToken.Parse(File.ReadAllText(tempFile));

                Assert.AreEqual("$Age$", updated["age"]?.ToString());
                Assert.AreEqual("$Active$", updated["active"]?.ToString());
                Assert.AreEqual("1", updated["id"]?.ToString());
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
