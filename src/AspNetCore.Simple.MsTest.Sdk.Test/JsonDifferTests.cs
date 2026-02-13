using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    [TestCategory("JsonDiffer")]
    public sealed class JsonDifferTests
    {
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

            var differ = new JsonDiffer();
            var diffs = differ.FindDifferences(left, right);

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

            var differ = new JsonDiffer();
            var diffs = differ.FindDifferences(left, right);

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

            var differ = new JsonDiffer();
            var diffs = differ.FindDifferences(left, right);

            Assert.IsTrue(diffs.Any(d => d.MemberPath == "items[0].value" && d.MismatchType == MismatchType.ValueDifference));
            Assert.IsTrue(diffs.Any(d => d.MemberPath == "items[1]" && d.MismatchType == MismatchType.MissingInFirst));
        }
    }
}
