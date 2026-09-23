using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// Truth-table tests for the core difference-filtering combinator
    /// <c>IDifferenceFiltering.Apply</c>, exercised through the
    /// non-HTTP <c>Assert.That.ObjectsAreEqual(expected, current, differenceFunc, differenceFilter)</c>
    /// overload.
    ///
    /// Four orthogonal mechanisms can drop a difference before the assert decides pass/fail:
    ///   1. the global <see cref="TestSdkSettings.DifferenceFunc"/>                   (list → list)
    ///   2. the per-assert <c>differenceFunc</c>                                       (list → list)
    ///   3. the global <see cref="TestSdkSettings.DifferenceFilter"/>                  (per-item predicate)
    ///   4. the per-assert <c>differenceFilter</c>                                     (per-item predicate)
    ///
    /// Composition order is: globalFunc → perAssertFunc → (globalFilter AND perAssertFilter).
    /// A difference is KEPT (and thus fails the assert) only if it survives every stage; the two
    /// filters combine with AND semantics — either returning <c>false</c> drops the difference.
    ///
    /// Tests that configure the global settings via <c>HttpClientAssertExtensions.Setup(settings => ...)</c>
    /// are <see cref="DoNotParallelizeAttribute"/> and reset them in a finally block
    /// (Core.Test runs MethodLevel parallelization).
    /// </summary>
    [TestClass]
    [TestCategory("DifferenceFilter")]
    public sealed class AssertThatDifferenceFilteringTests
    {
        // The endpoint-free subject: two persons differing only in the fields a test chooses.
        private sealed record Sample(string Name,
                                     int Age);

        private static readonly Sample Actual = new(Name: "Goku", Age: 99);

        // Identity func: keeps every difference.
        private static readonly Func<ImmutableList<Difference>, IEnumerable<Difference>> KeepAllFunc = diffs => diffs;

        // Func that drops any "age" difference (list-transform style).
        private static IEnumerable<Difference> DropAgeFunc(ImmutableList<Difference> diffs)
        {
            return diffs.Where(d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        // Predicate that KEEPS a difference unless it is an "age" difference (filter style).
        private static bool KeepUnlessAge(Difference d)
        {
            return !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase);
        }

        private static void AssertThrows(Action action,
                                         string message)
        {
            var threw = false;

            try
            {
                action();
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            Assert.IsTrue(threw, message);
        }

        // ============================================================
        // Baseline: no mechanism drops the difference → assert fails.
        // ============================================================

        [TestMethod]
        public void NoFilter_WhenDifferenceExists_ShouldFail()
        {
            var expected = Actual with { Age = 42 };

            AssertThrows(() => Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc),
                         "An unfiltered age difference must fail the assert.");
        }

        [TestMethod]
        public void NoDifference_ShouldPass()
        {
            // Identical objects: nothing to filter, assert passes.
            Assert.That.ObjectsAreEqual(Actual, Actual, differenceFunc: KeepAllFunc);
        }

        // ============================================================
        // Each mechanism ALONE drops the age difference → assert passes.
        // ============================================================

        [TestMethod]
        public void PerAssertFunc_Alone_ShouldDropDifference()
        {
            var expected = Actual with { Age = 42 };

            Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: DropAgeFunc);
        }

        [TestMethod]
        public void PerAssertFilter_Alone_ShouldDropDifference()
        {
            var expected = Actual with { Age = 42 };

            Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                        differenceFilter: KeepUnlessAge);
        }

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFunc_Alone_ShouldDropDifference()
        {
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFunc = diffs => DropAgeFunc(diffs));

            try
            {
                var expected = Actual with { Age = 42 };

                // No per-assert func/filter: only the global func drops the age difference.
                Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc);
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFilter_Alone_ShouldDropDifference()
        {
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFilter = KeepUnlessAge);

            try
            {
                var expected = Actual with { Age = 42 };

                Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc);
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        // ============================================================
        // A filter that keeps nothing makes everything pass.
        // ============================================================

        [TestMethod]
        public void PerAssertFilter_ThatDropsEverything_ShouldPass()
        {
            var expected = new Sample(Name: "Totally", Age: 1);

            Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                        differenceFilter: _ => false);
        }

        // ============================================================
        // A filter is not a mask for UNRELATED differences.
        // ============================================================

        [TestMethod]
        public void PerAssertFilter_ShouldNotHideUnrelatedDifference()
        {
            // Both name and age differ; the filter only drops age, so the name difference still fails.
            var expected = new Sample(Name: "Vegeta", Age: 42);

            AssertThrows(() => Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                                           differenceFilter: KeepUnlessAge),
                         "A filter that only ignores age must not hide a name difference.");
        }

        // ============================================================
        // AND semantics: a difference is dropped when EITHER filter says drop.
        // ============================================================

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFilterKeeps_PerAssertFilterDrops_ShouldPass_AndSemantics()
        {
            // Global keeps everything (true), per-assert drops age (false) → AND → dropped.
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFilter = static _ => true);

            try
            {
                var expected = Actual with { Age = 42 };

                Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                            differenceFilter: KeepUnlessAge);
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFilterDrops_PerAssertFilterKeeps_ShouldPass_AndSemantics()
        {
            // Global drops age (false), per-assert keeps everything (true) → AND → dropped.
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFilter = KeepUnlessAge);

            try
            {
                var expected = Actual with { Age = 42 };

                Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                            differenceFilter: static _ => true);
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFilterKeeps_PerAssertFilterKeeps_ShouldFail()
        {
            // Both keep the age difference → it survives → assert fails.
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFilter = static _ => true);

            try
            {
                var expected = Actual with { Age = 42 };

                AssertThrows(() => Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                                               differenceFilter: static _ => true),
                             "When neither filter drops the difference it must fail the assert.");
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        // ============================================================
        // Composition: global func + per-assert filter both apply (the id+age scenario).
        // ============================================================

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFunc_And_PerAssertFilter_ShouldCombine()
        {
            // Global func drops "name"; per-assert filter drops "age". Both differ → both dropped → pass.
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFunc = diffs =>
                diffs.Where(d => !d.MemberPath.Contains("name", StringComparison.OrdinalIgnoreCase)));

            try
            {
                var expected = new Sample(Name: "Vegeta", Age: 42);

                Assert.That.ObjectsAreEqual(expected, Actual, differenceFunc: KeepAllFunc,
                                            differenceFilter: KeepUnlessAge);
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        // ============================================================
        // Composition order: per-assert func receives the output of the global func.
        // ============================================================

        [TestMethod]
        [DoNotParallelize]
        public void GlobalFunc_RunsBefore_PerAssertFunc()
        {
            // Global func drops "name". Per-assert func asserts it never sees a name difference
            // (proving order) and then drops "age". Both differ → pass, and the order invariant holds.
            HttpClientAssertExtensions.Setup(settings => settings.DifferenceFunc = diffs =>
                diffs.Where(d => !d.MemberPath.Contains("name", StringComparison.OrdinalIgnoreCase)));

            try
            {
                var expected = new Sample(Name: "Vegeta", Age: 42);

                Assert.That.ObjectsAreEqual(expected,
                                            Actual,
                                            differenceFunc: incoming =>
                                            {
                                                Assert.IsFalse(incoming.Any(d => d.MemberPath.Contains("name", StringComparison.OrdinalIgnoreCase)),
                                                               "Per-assert func must receive the list AFTER the global func removed the name difference.");

                                                return DropAgeFunc(incoming);
                                            });
            }
            finally
            {
                HttpClientAssertExtensions.Setup(_ => { });
            }
        }

        // ============================================================
        // String-comparison path (StringComparisonStrategy): line-by-line filtering.
        //
        // When T is string, StringComparisonStrategy emits differences with MemberPath "Line N" and runs
        // them through the same IDifferenceFiltering.Apply combinator. The expected string, however, first
        // passes through EmbeddedFileLocalizer, which treats a non-raw-JSON string as a FILE reference.
        // So a plain multi-line string must be wrapped so IsRawJson() sees it as raw content (starts+ends
        // with a quote) — otherwise the localizer tries to load it as a file and throws before comparison.
        // Here both sides are JSON string literals whose VALUE contains newlines.
        // ============================================================

        // A raw JSON string literal (starts and ends with a quote) whose decoded value is 3 lines.
        private const string BaselineText = "\"alpha\nbeta\ngamma\"";

        [TestMethod]
        [TestCategory("StringComparison")]
        public void StringPath_NoFilter_WhenLineDiffers_ShouldFail()
        {
            // Line 2 differs and nothing drops it → fail.
            var expected = "\"alpha\nWRONG\ngamma\"";

            AssertThrows(() => Assert.That.ObjectsAreEqual(expected, BaselineText, differenceFunc: KeepAllFunc),
                         "An unfiltered line difference must fail the string-path assert.");
        }

        [TestMethod]
        [TestCategory("StringComparison")]
        public void StringPath_PerAssertFilter_DropsSpecificLine_ShouldPass()
        {
            // Only line 2 differs; a filter that drops "Line 2" makes the assert pass.
            var expected = "\"alpha\nWRONG\ngamma\"";

            Assert.That.ObjectsAreEqual(expected,
                                        BaselineText,
                                        differenceFunc: KeepAllFunc,
                                        differenceFilter: d => !string.Equals(d.MemberPath, "Line 2", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("StringComparison")]
        public void StringPath_PerAssertFilter_ShouldNotHideUnrelatedLine()
        {
            // Lines 2 and 3 differ; the filter only drops line 2, so line 3 still fails.
            var expected = "\"alpha\nWRONG\nALSO-WRONG\"";

            AssertThrows(() => Assert.That.ObjectsAreEqual(expected,
                                                           BaselineText,
                                                           differenceFunc: KeepAllFunc,
                                                           differenceFilter: d => !string.Equals(d.MemberPath, "Line 2", StringComparison.OrdinalIgnoreCase)),
                         "A filter dropping only Line 2 must not hide a Line 3 difference.");
        }

        [TestMethod]
        [TestCategory("StringComparison")]
        public void StringPath_PerAssertFunc_DropsLine_ShouldPass()
        {
            // The list-transform func drops the differing line 2.
            var expected = "\"alpha\nWRONG\ngamma\"";

            Assert.That.ObjectsAreEqual(expected,
                                        BaselineText,
                                        differenceFunc: diffs => diffs.Where(d => !string.Equals(d.MemberPath, "Line 2", StringComparison.OrdinalIgnoreCase)));
        }
    }
}