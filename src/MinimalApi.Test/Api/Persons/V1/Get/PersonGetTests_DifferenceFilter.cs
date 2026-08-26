using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Tests for the new per-assert <c>differenceFilter</c> predicate and the global
    /// <see cref="AssertObjectExtensions.DifferenceFilter"/> predicate.
    ///
    /// The endpoint GET /api/v1/persons/1 always returns
    /// Person(1, "Son", "Goku", 99, [alf@gmx.de/GMX, abc@hotmail.de/Microsoft]).
    ///
    /// Note: the assembly-wide <see cref="AssertObjectExtensions.DifferenceFunc"/> is set to
    /// <c>IgnoreIdDifferences</c> in <see cref="ApiTestBase"/>, so <c>id</c> differences are
    /// already ignored before the filter runs.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonGetTests_DifferenceFilter : ApiTestBase
    {
        private const string PersonUrl = "api/v1/persons/1";

        private static Person ExpectedPerson(int age = 99)
        {
            return new Person(Id: 1,
                              Name: "Son",
                              FirstName: "Goku",
                              Age: age,
                              Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                           new Email("abc@hotmail.de", "Microsoft")));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            // expected age is wrong (42 vs 99) - the per-assert filter drops the age difference
            var expectedPerson = ExpectedPerson(age: 42);

            return Client.AssertGetAsync(PersonUrl,
                                         expectedPerson,
                                         differenceFunc: diffs => diffs,
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public async Task PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            // Name is wrong; the filter only ignores age, so the name difference must still fail the assert.
            var expectedPerson = new Person(Id: 1,
                                            Name: "WrongName",
                                            FirstName: "Goku",
                                            Age: 99,
                                            Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                         new Email("abc@hotmail.de", "Microsoft")));

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync(PersonUrl,
                                                                                                    expectedPerson,
                                                                                                    differenceFunc: diffs => diffs,
                                                                                                    differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public async Task GlobalFilter_Should_Ignore_Filtered_Difference()
        {
            // The global DifferenceFilter is static state - set it for this test only and always restore it.
            var originalFilter = AssertObjectExtensions.DifferenceFilter;

            try
            {
                AssertObjectExtensions.DifferenceFilter = d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase);

                // Wrong age is ignored purely via the global filter (no per-assert filter passed).
                await Client.AssertGetAsync(PersonUrl, ExpectedPerson(age: 42)).ConfigureAwait(false);
            }
            finally
            {
                AssertObjectExtensions.DifferenceFilter = originalFilter;
            }
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public Task GlobalFunc_And_PerAssertFilter_Should_Combine()
        {
            // id is wrong (ignored by the global IgnoreIdDifferences func) AND age is wrong
            // (ignored by the per-assert filter). Both mechanisms must apply together (AND semantics).
            var expectedPerson = new Person(Id: 999,
                                            Name: "Son",
                                            FirstName: "Goku",
                                            Age: 42,
                                            Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                                         new Email("abc@hotmail.de", "Microsoft")));

            return Client.AssertGetAsync(PersonUrl,
                                         expectedPerson,
                                         differenceFunc: diffs => diffs,
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_That_Ignores_Everything_Should_Pass()
        {
            // Every field differs, but a filter that keeps nothing makes the assert pass.
            var expectedPerson = new Person(Id: 123,
                                            Name: "Totally",
                                            FirstName: "Different",
                                            Age: 1,
                                            Emails: ImmutableList<Email>.Empty);

            return Client.AssertGetAsync(PersonUrl,
                                         expectedPerson,
                                         differenceFunc: diffs => diffs,
                                         differenceFilter: _ => false);
        }
    }
}