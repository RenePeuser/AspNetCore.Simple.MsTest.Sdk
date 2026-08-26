using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Tests for the per-assert <c>differenceFilter</c> predicate on POST /api/v1/persons.
    /// The Create endpoint echoes the posted person back (201 Created), so a deliberately
    /// wrong field in the expected object is only tolerated when the filter drops it.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonCreateTests_DifferenceFilter : ApiTestBase
    {
        private static Person PersonToCreate()
        {
            return new Person(Id: 1,
                              Name: "Son",
                              FirstName: "Goku",
                              Age: 42,
                              Emails: ImmutableList<Email>.Empty);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            var personToCreate = PersonToCreate();

            // Expected age is wrong (99 vs 42) - the per-assert filter drops the age difference.
            var expectedPerson = personToCreate with { Age = 99 };

            return Client.AssertPostAsync("api/v1/persons",
                                          personToCreate,
                                          expectedPerson,
                                          differenceFunc: diffs => diffs,
                                          differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("POST")]
        [TestCategory("DifferenceFilter")]
        public async Task PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var personToCreate = PersonToCreate();

            // Name is wrong; the filter only ignores age, so the name difference must still fail.
            var expectedPerson = personToCreate with { Name = "WrongName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPostAsync("api/v1/persons",
                                                                                                     personToCreate,
                                                                                                     expectedPerson,
                                                                                                     differenceFunc: diffs => diffs,
                                                                                                     differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }
    }
}