using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Update
{
    /// <summary>
    /// Tests for the per-assert <c>differenceFilter</c> predicate on PUT and PATCH /api/v1/persons.
    /// Both endpoints echo the sent person back (200 OK), so a deliberately wrong field in the
    /// expected object is only tolerated when the filter drops it.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonUpdateTestsDifferenceFilter : ApiTestBase
    {
        private static Person PersonToUpdate()
        {
            return new Person(Id: 1,
                              Name: "Son",
                              FirstName: "Goku Updated",
                              Age: 100,
                              Emails: ImmutableList<Email>.Empty);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PUT")]
        [TestCategory("DifferenceFilter")]
        public Task Put_PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            var personToUpdate = PersonToUpdate();

            // Expected age is wrong (1 vs 100) - the per-assert filter drops the age difference.
            var expectedPerson = personToUpdate with { Age = 1 };

            return Client.AssertPutAsync("api/v1/persons",
                                         personToUpdate,
                                         expectedPerson,
                                         differenceFunc: diffs => diffs,
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PUT")]
        [TestCategory("DifferenceFilter")]
        public async Task Put_PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var personToUpdate = PersonToUpdate();

            // FirstName is wrong; the filter only ignores age, so this difference must still fail.
            var expectedPerson = personToUpdate with { FirstName = "WrongName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPutAsync("api/v1/persons",
                                                                                                    personToUpdate,
                                                                                                    expectedPerson,
                                                                                                    differenceFunc: diffs => diffs,
                                                                                                    differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PATCH")]
        [TestCategory("DifferenceFilter")]
        public Task Patch_PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            var personToPatch = PersonToUpdate();

            // Expected age is wrong (1 vs 100) - the per-assert filter drops the age difference.
            var expectedPerson = personToPatch with { Age = 1 };

            return Client.AssertPatchAsync("api/v1/persons",
                                           personToPatch,
                                           expectedPerson,
                                           differenceFunc: diffs => diffs,
                                           differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("PATCH")]
        [TestCategory("DifferenceFilter")]
        public async Task Patch_PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var personToPatch = PersonToUpdate();

            // FirstName is wrong; the filter only ignores age, so this difference must still fail.
            var expectedPerson = personToPatch with { FirstName = "WrongName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPatchAsync("api/v1/persons",
                                                                                                      personToPatch,
                                                                                                      expectedPerson,
                                                                                                      differenceFunc: diffs => diffs,
                                                                                                      differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }
    }
}