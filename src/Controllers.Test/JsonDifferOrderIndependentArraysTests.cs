using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// An order-independent array is compared by MATCHING its elements, not by reordering either
    /// document. Every test here exists because the reordering variant got one of them wrong: a
    /// sort key is computed top down, is sensitive to property order and runs through the current
    /// culture, and the indices it produces address elements that exist in neither document.
    /// </summary>
    [TestClass]
    [TestCategory("JsonDiffer")]
    [TestCategory("OrderIndependentArrays")]
    public sealed class JsonDifferOrderIndependentArraysTests
    {
        private static IJsonDiffer _jsonDiffer = null!;

        [TestInitialize]
        public void Initialize()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddJsonDiffer();
            _jsonDiffer = serviceCollection.BuildServiceProvider().GetRequiredService<IJsonDiffer>();
        }

        private static Predicate<JsonArrayContext> ByName(params string[] names)
        {
            return array => names.Contains(array.PropertyName);
        }

        [TestMethod]
        public void WithoutFilterArrayOrderIsStillADifference()
        {
            var expected = """{ "items": [ { "$ref": "TypeA" }, { "$ref": "TypeB" } ] }""";
            var current = """{ "items": [ { "$ref": "TypeB" }, { "$ref": "TypeA" } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current);

            Assert.That.HasCount(2,
                                 diffs,
                                 because: "No filter was passed, so every array keeps index-by-index semantics and a swap is a real difference.",
                                 fix: "Check that JsonDiffer only takes the matching route when the order-independent predicate answers true.");

            Assert.That.Any(diffs,
                            difference => difference.MemberPath == "items[0].$ref",
                            predicateDescription: "items[0].$ref is reported",
                            because: "The first element changed from TypeA to TypeB.",
                            fix: "Check the index-wise array comparison in JsonDiffer.CompareArrays.");
        }

        [TestMethod]
        public void ReorderedSetIsNotADifference()
        {
            var expected = """{ "anyOf": [ { "$ref": "TypeA" }, { "$ref": "TypeB" }, { "$ref": "TypeC" } ] }""";
            var current = """{ "anyOf": [ { "$ref": "TypeC" }, { "$ref": "TypeA" }, { "$ref": "TypeB" } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.IsEmpty(diffs,
                                because: "Both sides hold the same three elements. For an order-independent array that is equality, whatever order the producer emitted them in.",
                                fix: "Check pass 1 of JsonDiffer.CompareAsSet - identical canonical signatures have to consume each other.");
        }

        [TestMethod]
        public void NestedOrderIndependentArraysAreMatchedBottomUp()
        {
            // Both documents describe the same outer multiset { {1,2}, {1,3} }. Reordering top down
            // builds the outer key from still-unsorted inner arrays and lands on two different
            // orders - which is why the canonical signature is computed by recursing first.
            var expected = """{ "outer": [ { "inner": [2,1] }, { "inner": [1,3] } ] }""";
            var current = """{ "outer": [ { "inner": [1,2] }, { "inner": [3,1] } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("outer", "inner"));

            Assert.That.IsEmpty(diffs,
                                because: "Outer and inner are both order independent, so both documents describe the same nested set.",
                                fix: "Check that CanonicalSignature recurses BEFORE it sorts, so a parent fingerprint is built from already canonical children.");
        }

        [TestMethod]
        public void PropertyOrderInsideElementsIsIrrelevant()
        {
            // JsonDiffer never treated property order as a difference (see
            // FindDifferencesShouldIgnoreDifferentPropertyOrder). A raw ToString() key does, which
            // made the matching disagree with the equality it is supposed to implement.
            var expected = """{ "anyOf": [ { "a": 1, "b": 2 }, { "a": 2, "b": 1 } ] }""";
            var current = """{ "anyOf": [ { "b": 1, "a": 2 }, { "b": 2, "a": 1 } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.IsEmpty(diffs,
                                because: "The same two objects are present on both sides; only the order of their properties and of the elements differs, and neither is a difference.",
                                fix: "Check that CanonicalSignature writes object properties in ordinal name order.");
        }

        [TestMethod]
        public void MissingElementIsStillReportedAtItsExpectedIndex()
        {
            var expected = """{ "anyOf": [ { "$ref": "TypeA" }, { "$ref": "TypeB" }, { "$ref": "TypeC" } ] }""";
            var current = """{ "anyOf": [ { "$ref": "TypeA" }, { "$ref": "TypeB" } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.HasCount(1,
                                 diffs,
                                 because: "Matching must not swallow a surplus element - order being irrelevant does not make a missing element irrelevant.",
                                 fix: "Check that CompareAsSet reports every leftover that could not be paired.");

            Assert.That.AreEqual(MismatchType.MissingInSecond,
                                 diffs[0].MismatchType,
                                 because: "TypeC exists in the expected document and not in the current one.",
                                 fix: "Check the mismatch type CompareAsSet assigns to an unpaired expected element.");

            Assert.That.AreEqual("anyOf[2]",
                                 diffs[0].MemberPath,
                                 because: "TypeC really does sit at index 2 of the expected document, so that is where the report has to point.",
                                 fix: "Check that leftovers keep their own index instead of a position in a reordered copy.");
        }

        [TestMethod]
        public void ExtraElementIsReportedAtItsCurrentIndex()
        {
            var expected = """{ "anyOf": [ { "$ref": "TypeA" } ] }""";
            var current = """{ "anyOf": [ { "$ref": "TypeB" }, { "$ref": "TypeA" }, { "$ref": "TypeC" } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            var missingInFirst = diffs.Where(difference => difference.MismatchType == MismatchType.MissingInFirst).ToList();

            Assert.That.HasCount(2,
                                 missingInFirst,
                                 because: "TypeA is matched away, and with nothing left over on the expected side both TypeB and TypeC are pure surplus - there is no partner to describe them as a change of.",
                                 fix: "Check the leftover handling in JsonDiffer.PairLeftovers.");

            Assert.That.All(missingInFirst,
                            difference => difference.MemberPath is "anyOf[0]" or "anyOf[2]",
                            predicateDescription: "the surplus elements are reported at their own current indices 0 and 2",
                            because: "TypeB sits at index 0 and TypeC at index 2 of the current document, and a surplus element can only be addressed there.",
                            fix: "Check that an unpaired current element keeps its current index instead of a position in a reordered copy.");
        }

        [TestMethod]
        public void ChangedElementReadsAsAValueDifferenceAndCarriesBothAddresses()
        {
            // A matched pair that sits at different indices is exactly the case that makes
            // Difference.CurrentMemberPath necessary: reading from expected and patching current
            // need different paths.
            var expected = """
                           {
                               "anyOf": [
                                   { "name": "A", "description": "a" },
                                   { "name": "B", "description": "b" },
                                   { "name": "C", "description": "c" }
                               ]
                           }
                           """;

            var current = """
                          {
                              "anyOf": [
                                  { "name": "C", "description": "c" },
                                  { "name": "A", "description": "a" },
                                  { "name": "B", "description": "CHANGED" }
                              ]
                          }
                          """;

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.HasCount(1,
                                 diffs,
                                 because: "A and C match exactly; B changed in one field, which is one difference and not one removal plus one addition.",
                                 fix: "Check the identity pass in JsonDiffer.PairLeftovers - two elements with the same name describe the same thing.");

            Assert.That.AreEqual(MismatchType.ValueDifference,
                                 diffs[0].MismatchType,
                                 because: "The element is present on both sides, only its description differs.",
                                 fix: "Check that a paired leftover is compared with CompareTokens instead of being reported structurally.");

            Assert.That.AreEqual("anyOf[1].description",
                                 diffs[0].MemberPath,
                                 because: "B sits at index 1 in the expected document, which is what anything reading the snapshot has to use.",
                                 fix: "Check NodePath.Expected in JsonDiffer.");

            Assert.That.AreEqual("anyOf[2].description",
                                 diffs[0].CurrentMemberPath,
                                 because: "The same element sits at index 2 in the current document, which is what anything patching the response has to use.",
                                 fix: "Check NodePath.Current and JsonDiffer.Add - the current path is kept whenever it differs from the expected one.");
        }

        [TestMethod]
        public void IdenticallyAddressedDifferenceCarriesNoCurrentPath()
        {
            var expected = """{ "anyOf": [ { "name": "A", "value": 1 } ] }""";
            var current = """{ "anyOf": [ { "name": "A", "value": 2 } ] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.HasCount(1,
                                 diffs,
                                 because: "One element, one changed field.",
                                 fix: "Check CompareAsSet for single element arrays.");

            Assert.That.IsNull(diffs[0].CurrentMemberPath,
                               because: "Both documents address this element identically, so a second path would only be noise for every consumer.",
                               fix: "Check JsonDiffer.Add - CurrentMemberPath stays null while the two paths are equal.");
        }

        [TestMethod]
        public void DuplicatesAreComparedAsAMultiset()
        {
            var expected = """{ "tags": ["a", "a", "b"] }""";
            var current = """{ "tags": ["a", "b", "b"] }""";

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("tags"));

            Assert.That.HasCount(1,
                                 diffs,
                                 because: "Two a plus one b against one a plus two b differ by exactly one element - a set-like array must not lose the count.",
                                 fix: "Check that CompareAsSet keeps a QUEUE per signature instead of a flag, so three identical elements need three partners.");
        }

        [TestMethod]
        public void OnlyTheMatchedArraysBecomeOrderIndependent()
        {
            var expected = """
                           {
                               "anyOf": [ { "$ref": "TypeB" }, { "$ref": "TypeA" } ],
                               "items": [ { "$ref": "TypeX" }, { "$ref": "TypeY" } ]
                           }
                           """;

            var current = """
                          {
                              "anyOf": [ { "$ref": "TypeA" }, { "$ref": "TypeB" } ],
                              "items": [ { "$ref": "TypeY" }, { "$ref": "TypeX" } ]
                          }
                          """;

            var diffs = _jsonDiffer.FindDifferences(expected, current, ByName("anyOf"));

            Assert.That.IsNotEmpty(diffs,
                                   because: "items was not declared order independent, so its swap is still a difference.",
                                   fix: "Check that the predicate result is honoured per array.");

            Assert.That.All(diffs,
                            difference => difference.MemberPath.StartsWith("items["),
                            predicateDescription: "every difference is inside items",
                            because: "anyOf was matched and is equal; only items is compared by index.",
                            fix: "Check the predicate evaluation in JsonDiffer.IsOrderIndependent.");
        }

        [TestMethod]
        public void PathScopesTheFilterToOneArrayOnly()
        {
            var expected = """
                           {
                               "a": { "anyOf": ["1", "2"] },
                               "b": { "anyOf": ["1", "2"] }
                           }
                           """;

            var current = """
                          {
                              "a": { "anyOf": ["2", "1"] },
                              "b": { "anyOf": ["2", "1"] }
                          }
                          """;

            var diffs = _jsonDiffer.FindDifferences(expected, current, array => array.Path == "a.anyOf");

            Assert.That.IsNotEmpty(diffs,
                                   because: "Only a.anyOf was declared order independent, so b.anyOf keeps index semantics.",
                                   fix: "Check that JsonArrayContext.Path carries the full path and not just the property name.");

            Assert.That.All(diffs,
                            difference => difference.MemberPath.StartsWith("b.anyOf["),
                            predicateDescription: "every difference is inside b.anyOf",
                            because: "A path-scoped filter is the whole point of handing the predicate a context instead of a bare name.",
                            fix: "Check the Path built in JsonDiffer.NodePath.Property.");
        }

        [TestMethod]
        public void PathIsIndexFreeSoItCanBeWrittenDownInAPredicate()
        {
            var expected = """
                           {
                               "items": [
                                   { "tags": ["x", "y"] },
                                   { "tags": ["p", "q"] }
                               ]
                           }
                           """;

            var current = """
                          {
                              "items": [
                                  { "tags": ["y", "x"] },
                                  { "tags": ["q", "p"] }
                              ]
                          }
                          """;

            var seenPaths = new List<string>();

            var diffs = _jsonDiffer.FindDifferences(expected, current,
                                                    array =>
                                                    {
                                                        seenPaths.Add(array.Path);

                                                        return array.Path == "items.tags";
                                                    });

            Assert.That.IsEmpty(diffs,
                                because: "Both tags arrays were declared order independent through one index-free path.",
                                fix: "Check that array elements contribute no segment to JsonArrayContext.Path.");

            Assert.That.All(seenPaths,
                            path => !path.Contains('['),
                            predicateDescription: "no path handed to the predicate contains an index",
                            because: "A predicate cannot be written against items[0].tags, items[1].tags, and so on - and an index would also make the expected and the current side disagree.",
                            fix: "Check JsonDiffer.NodePath - only Property() extends the schema path, Index() must leave it alone.");
        }

        [TestMethod]
        public void FilterThatNeverMatchesBehavesLikeNoFilter()
        {
            var expected = """{ "anyOf": [1, 2] }""";
            var current = """{ "anyOf": [2, 1] }""";

            var withoutFilter = _jsonDiffer.FindDifferences(expected, current);
            var neverMatches = _jsonDiffer.FindDifferences(expected, current, _ => false);
            var alwaysMatches = _jsonDiffer.FindDifferences(expected, current, _ => true);

            Assert.That.IsNotEmpty(withoutFilter,
                                   because: "Without a filter every array is compared by index.",
                                   fix: "Check the parameterless FindDifferences overload.");

            Assert.That.HasCount(withoutFilter.Count,
                                 neverMatches,
                                 because: "A predicate that answers false everywhere has to be indistinguishable from passing none.",
                                 fix: "Check that IsOrderIndependent only routes to CompareAsSet on a true answer.");

            Assert.That.IsEmpty(alwaysMatches,
                                because: "With every array order independent the two documents hold the same set.",
                                fix: "Check that the predicate is consulted for arrays at any depth.");
        }
    }
}