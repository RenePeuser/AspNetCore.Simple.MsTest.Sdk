using System.Collections.Immutable;
using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    ///     Covers every <c>ObjectsAreEqual</c> overload that takes the calling assembly explicitly.
    ///     The overloads without that parameter read the caller through <c>Assembly.GetCallingAssembly()</c>,
    ///     which is only correct while the assert stands directly in the test. A shared helper, a base class
    ///     or a wrapper living in another assembly hands the SDK its OWN assembly, and the embedded snapshot
    ///     is then looked up in the wrong manifest. The explicit overloads let such a caller pass the test
    ///     assembly on instead.
    ///     Three properties are worth holding, and each combination is driven through all of them:
    ///     1. it RESOLVES - a shape that is ambiguous with a sibling is a compile error, and one that
    ///     silently bound to a different sibling would quietly drop the assembly again;
    ///     2. it still COMPARES - an overload that swallowed its input would pass the equal-objects test
    ///     just as happily, so every shape has to report a real difference too;
    ///     3. the assembly is HONOURED - the value handed in has to reach the snapshot lookup instead of
    ///     being re-derived further down the delegation chain.
    ///     (3) is observable because a failed snapshot lookup names the assembly it searched.
    /// </summary>
    [TestClass]
    [TestCategory("CallingAssembly")]
    public sealed class AssertThatCallingAssemblyOverloadsTests
    {
        private const string MissingSnapshot = "ThisSnapshotDoesNotExist.json";

        private const string Current = "current";

        private static readonly Sample Expected = new("Goku", 99);

        private static readonly Sample Equal = new("Goku", 99);

        private static readonly Sample Different = new("Vegeta", 99);

        private static readonly Assembly TestAssembly = typeof(AssertThatCallingAssemblyOverloadsTests).Assembly;

        // An assembly that is demonstrably not this one. It owns no snapshot of ours either, so the
        // lookup has to fail while naming exactly this assembly - which is the whole point: a chain
        // that re-derived the caller would name the test assembly instead.
        private static readonly Assembly ForeignAssembly = typeof(Assert).Assembly;

        private static readonly Assembly SdkAssembly = typeof(AssertObjectExtensions).Assembly;

        private static readonly (string Key, object? Value)[] NoParameters = [];

        private static Func<Sample?, Sample?> Identity => item => item;

        private static Func<T?, T?> IdentityOf<T>()
        {
            return item => item;
        }

        private static IEnumerable<Difference> KeepAll(ImmutableList<Difference> differences)
        {
            return differences;
        }

        // ============================================================
        // Observing which assembly was searched
        // ============================================================

        /// <summary>
        ///     The lookup names the assembly it searched in one of two shapes, depending on whether that
        ///     assembly carries embedded resources at all: one without any is rejected outright, one with
        ///     resources reaches the full snapshot-not-found report and is named in its "Project" row
        ///     (rebuilt with the very same format string, so the expectation cannot drift on padding).
        ///     Either way the assembly NAME is the observable, and that is all these tests need.
        /// </summary>
        private static bool Searched(string message,
                                     Assembly assembly)
        {
            var name = assembly.GetName().Name;

            return message.Contains($"Assembly '{name}' contains no embedded resources", StringComparison.Ordinal) ||
                   message.Contains($"{"Project",-10} : {name}", StringComparison.Ordinal);
        }

        private static string CaptureFailure(Action action)
        {
            try
            {
                action();
            }
#pragma warning disable CA1031 // the shape of the failure is exactly what these tests inspect
            catch (Exception exception)
#pragma warning restore CA1031
            {
                return exception.Message;
            }

            return string.Empty;
        }

        private static void AssertSearched(Action action,
                                           Assembly assembly,
                                           string shape)
        {
            Assert.That.IsTrue(Searched(CaptureFailure(action), assembly),
                               $"the {shape} overload has to look the snapshot up in the assembly it was handed",
                               "forward the callingAssembly argument to the core overload instead of dropping it");
        }

        private static void AssertNotSearched(string message,
                                              Assembly assembly,
                                              string because)
        {
            Assert.That.IsFalse(Searched(message, assembly),
                                because,
                                "capture Assembly.GetCallingAssembly() and forward it when delegating");
        }

        private static void AssertReportsDifference(Action action,
                                                    string shape)
        {
            Assert.That.Contains(CaptureFailure(action),
                                 "Vegeta",
                                 $"the {shape} overload has to run the same comparison as its assembly-less sibling",
                                 "check that the overload delegates to the core overload instead of short-circuiting");
        }

        // ============================================================
        // 1. Object route - all 15 explicit-assembly shapes
        // ============================================================

        /// <summary>
        ///     Every object-route shape, driven with equal objects. Compiling this method is half the test:
        ///     an ambiguous shape never gets here. Passing it is the other half.
        /// </summary>
        [TestMethod]
        public void ObjectOverloads_WithExplicitAssembly_AcceptEqualObjects()
        {
            Assert.That.ObjectsAreEqual(Expected, Equal, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, NoParameters,
                                        TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, "title",
                                        TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, "title",
                                        NoParameters, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        NoParameters, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, KeepAll,
                                        TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, NoParameters,
                                        KeepAll, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        "title", TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        "title", NoParameters, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        KeepAll, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        KeepAll, NoParameters, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        "title", KeepAll, TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        "title", KeepAll, NoParameters,
                                        TestAssembly);

            Assert.That.ObjectsAreEqual(Expected, Equal, Identity,
                                        "title", KeepAll, "curl",
                                        TestAssembly);
        }

        /// <summary>
        ///     The counterpart: a shape that swallowed its input would pass the test above just as happily.
        ///     Every shape still has to report a real difference.
        /// </summary>
        [TestMethod]
        public void ObjectOverloads_WithExplicitAssembly_StillReportDifferences()
        {
            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, TestAssembly),
                                    "plain");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, NoParameters,
                                                                      TestAssembly),
                                    "parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, "title",
                                                                      TestAssembly),
                                    "title");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, "title",
                                                                      NoParameters, TestAssembly),
                                    "title + parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      TestAssembly),
                                    "orderFunc");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      NoParameters, TestAssembly),
                                    "orderFunc + parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, KeepAll,
                                                                      TestAssembly),
                                    "differenceFunc");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, NoParameters,
                                                                      KeepAll, TestAssembly),
                                    "parameters + differenceFunc");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      "title", TestAssembly),
                                    "orderFunc + title");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      "title", NoParameters, TestAssembly),
                                    "orderFunc + title + parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      KeepAll, TestAssembly),
                                    "orderFunc + differenceFunc");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      KeepAll, NoParameters, TestAssembly),
                                    "orderFunc + differenceFunc + parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      "title", KeepAll, TestAssembly),
                                    "orderFunc + title + differenceFunc");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      "title", KeepAll, NoParameters,
                                                                      TestAssembly),
                                    "orderFunc + title + differenceFunc + parameters");

            AssertReportsDifference(() => Assert.That.ObjectsAreEqual(Expected, Different, Identity,
                                                                      "title", KeepAll, "curl",
                                                                      TestAssembly),
                                    "orderFunc + title + differenceFunc + curl");
        }

        /// <summary>
        ///     Every shape again, this time against an unresolvable snapshot reference, so the assembly it
        ///     searched becomes visible. A string expected is a snapshot reference in this SDK, which is
        ///     what makes the lookup observable at all.
        ///     Three of these shapes place the assembly in the same slot as a from-file sibling
        ///     (bare / title / orderFunc), so the call binds to the from-file route instead. That is the
        ///     intended reading of a string expected, and the property under test is identical either way:
        ///     whichever overload wins has to search the assembly it was handed.
        /// </summary>
        [TestMethod]
        public void ObjectOverloads_WithExplicitAssembly_SearchThatAssembly()
        {
            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, TestAssembly),
                           TestAssembly, "plain");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, NoParameters,
                                                             TestAssembly),
                           TestAssembly, "parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, "title",
                                                             TestAssembly),
                           TestAssembly, "title");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, "title",
                                                             NoParameters, TestAssembly),
                           TestAssembly, "title + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             TestAssembly),
                           TestAssembly, "orderFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             NoParameters, TestAssembly),
                           TestAssembly, "orderFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, KeepAll,
                                                             TestAssembly),
                           TestAssembly, "differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, NoParameters,
                                                             KeepAll, TestAssembly),
                           TestAssembly, "parameters + differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             "title", TestAssembly),
                           TestAssembly, "orderFunc + title");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             "title", NoParameters, TestAssembly),
                           TestAssembly, "orderFunc + title + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             KeepAll, TestAssembly),
                           TestAssembly, "orderFunc + differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             KeepAll, NoParameters, TestAssembly),
                           TestAssembly, "orderFunc + differenceFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             "title", KeepAll, TestAssembly),
                           TestAssembly, "orderFunc + title + differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             "title", KeepAll, NoParameters,
                                                             TestAssembly),
                           TestAssembly, "orderFunc + title + differenceFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Current, IdentityOf<string>(),
                                                             "title", KeepAll, "curl",
                                                             TestAssembly),
                           TestAssembly, "orderFunc + title + differenceFunc + curl");
        }

        /// <summary>
        ///     The contrast that gives the test above its teeth: the identical call, differing only in the
        ///     assembly, has to name that other assembly. Without this a chain that ignored the parameter
        ///     and re-derived the caller would satisfy every assertion above.
        /// </summary>
        [TestMethod]
        public void ObjectOverload_WithForeignAssembly_SearchesTheForeignAssembly()
        {
            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot,
                                                             Current,
                                                             IdentityOf<string>(),
                                                             "title",
                                                             KeepAll,
                                                             "curl",
                                                             ForeignAssembly),
                           ForeignAssembly,
                           "curl shape with a foreign assembly");
        }

        /// <summary>
        ///     Regression for the curl overload that takes no assembly: it delegated on WITHOUT capturing
        ///     the caller, so the sibling it landed on ran GetCallingAssembly() from inside the SDK and
        ///     searched the SDK's own manifest. It has to name the test assembly, and must not name the SDK.
        /// </summary>
        [TestMethod]
        public void CurlOverload_WithoutExplicitAssembly_SearchesTheTestAssembly()
        {
            var message = CaptureFailure(() => Assert.That.ObjectsAreEqual(MissingSnapshot,
                                                                           Current,
                                                                           IdentityOf<string>(),
                                                                           "title",
                                                                           KeepAll,
                                                                           "curl"));

            Assert.That.IsTrue(Searched(message, TestAssembly),
                               "an overload without an assembly parameter still has to capture its own caller",
                               "capture Assembly.GetCallingAssembly() and forward it when delegating");

            AssertNotSearched(message,
                              SdkAssembly,
                              "delegating on without capturing the caller makes the SDK search its own manifest");
        }

        // ============================================================
        // 2. From-file route - all 12 explicit-assembly shapes
        //    (9 that already existed, plus the 3 that were missing a twin)
        // ============================================================

        /// <summary>
        ///     The from-file route is where the assembly matters most - the whole overload exists to look a
        ///     snapshot up in a manifest. Each shape is driven against an unresolvable reference, so a
        ///     single assert covers both concerns: it only reaches the report if the overload resolved, and
        ///     the report only names the assembly if it was carried through.
        /// </summary>
        [TestMethod]
        public void FromFileOverloads_WithExplicitAssembly_SearchThatAssembly()
        {
            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, TestAssembly),
                           TestAssembly, "plain");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, TestAssembly,
                                                             NoParameters),
                           TestAssembly, "parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, "title",
                                                             TestAssembly),
                           TestAssembly, "title");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, "title",
                                                             TestAssembly, NoParameters),
                           TestAssembly, "title + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             TestAssembly),
                           TestAssembly, "orderFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             TestAssembly, NoParameters),
                           TestAssembly, "orderFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             "title", TestAssembly, KeepAll),
                           TestAssembly, "orderFunc + title + differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             "title", TestAssembly, KeepAll,
                                                             NoParameters),
                           TestAssembly, "orderFunc + title + differenceFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             "title", TestAssembly, KeepAll,
                                                             "curl", NoParameters),
                           TestAssembly, "orderFunc + title + differenceFunc + curl + parameters");

            // The three shapes that had no twin until now.
            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, TestAssembly,
                                                             KeepAll),
                           TestAssembly, "differenceFunc");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, Identity,
                                                             TestAssembly, KeepAll, NoParameters),
                           TestAssembly, "orderFunc + differenceFunc + parameters");

            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, TestAssembly,
                                                             KeepAll, NoParameters),
                           TestAssembly, "differenceFunc + parameters");
        }

        /// <summary>
        ///     Same contrast as on the object route, driven through one of the newly added shapes: the
        ///     from-file twins must not re-derive the caller either.
        /// </summary>
        [TestMethod]
        public void FromFileOverload_WithForeignAssembly_SearchesTheForeignAssembly()
        {
            AssertSearched(() => Assert.That.ObjectsAreEqual(MissingSnapshot, Equal, ForeignAssembly,
                                                             KeepAll),
                           ForeignAssembly,
                           "differenceFunc shape with a foreign assembly");
        }

        private sealed record Sample(string Name,
                                     int Age);
    }
}