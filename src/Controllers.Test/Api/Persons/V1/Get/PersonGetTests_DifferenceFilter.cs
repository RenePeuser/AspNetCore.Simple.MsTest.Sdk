using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Per-assert <c>differenceFilter</c> tests for GET /api/v1/persons/1 on the CONTROLLER stack
    /// (a separate code path from the Minimal API).
    ///
    /// IMPORTANT: Controllers.Test runs <c>Parallelize(Scope = ExecutionScope.ClassLevel)</c>, so these
    /// tests deliberately use ONLY the per-assert <c>differenceFilter</c> — never the static global
    /// <see cref="TestSdkSettings.DifferenceFilter"/>, which would race across parallel classes.
    ///
    /// The assembly-wide <see cref="TestSdkSettings.DifferenceFunc"/> is set to
    /// <c>IgnoreIdDifferences</c> in <see cref="ApiTestBase"/>, so <c>id</c> differences are already ignored.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonGetTestsDifferenceFilter : ApiTestBase
    {
        private const string PersonUrl = "api/v1/persons/1";

        private static Person ExpectedPerson(int age = 99,
                                             string name = "Son")
        {
            return new Person(Id: 1,
                              Name: name,
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
            var expectedPerson = ExpectedPerson(name: "WrongName");

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync(PersonUrl,
                                                                                                    expectedPerson,
                                                                                                    differenceFunc: diffs => diffs,
                                                                                                    differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The name is wrong here for an unrelated reason and still has to fail - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_That_Ignores_Everything_Should_Pass()
        {
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

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("GET")]
        [TestCategory("DifferenceFilter")]
        public Task DifferenceFilterOnly_Twin_Should_Ignore_Filtered_Difference()
        {
            // Exercises the differenceFilter-only twin (no differenceFunc).
            var expectedPerson = ExpectedPerson(age: 42);

            return Client.AssertGetAsync(PersonUrl,
                                         expectedPerson,
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }
    }
}