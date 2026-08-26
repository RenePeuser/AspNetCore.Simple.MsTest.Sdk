using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Query
{
    /// <summary>
    /// Tests for the per-assert <c>differenceFilter</c> predicate on QUERY /api/v1/persons/search.
    /// The endpoint echoes the matching persons back (200 OK); a deliberately wrong field in the
    /// expected object is only tolerated when the filter drops it.
    ///
    /// Uses the object-based <c>AssertQueryAsync&lt;T&gt;(url, payload, expectedResponse, differenceFunc,
    /// differenceFilter)</c> overloads (added to close the Query.AsObjectResponse gap), so the expected
    /// value is a typed <see cref="Person"/> list rather than a JSON string.
    ///
    /// Note: the assembly-wide <see cref="AssertObjectExtensions.DifferenceFunc"/> is set to
    /// <c>IgnoreIdDifferences</c> in <see cref="ApiTestBase"/>, so <c>id</c> differences are already
    /// ignored before the filter runs.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonQueryTests_DifferenceFilter : ApiTestBase
    {
        private const string SearchUrl = "api/v1/persons/search";

        // Query {Name="Son", MinAge=50} returns exactly Person 1 (Son/Goku/99).
        private static Person[] ExpectedSonPersons(int age = 99, string name = "Son", string firstName = "Goku")
        {
            return
            [
                new Person(Id: 1,
                           Name: name,
                           FirstName: firstName,
                           Age: age,
                           Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                        new Email("abc@hotmail.de", "Microsoft")))
            ];
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            var queryRequest = new { Name = "Son", MinAge = 50 };

            // Expected age is wrong (42 vs 99) - the per-assert filter drops the age difference.
            var expectedPersons = ExpectedSonPersons(age: 42);

            return Client.AssertQueryAsync<IEnumerable<Person>>(
                       SearchUrl,
                       queryRequest,
                       expectedPersons,
                       differenceFunc: diffs => diffs,
                       differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public async Task PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var queryRequest = new { Name = "Son", MinAge = 50 };

            // name is wrong; the filter only ignores age, so the name difference must still fail.
            var expectedPersons = ExpectedSonPersons(age: 42, name: "WrongName");

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(
                           () => Client.AssertQueryAsync<IEnumerable<Person>>(
                                     SearchUrl,
                                     queryRequest,
                                     expectedPersons,
                                     differenceFunc: diffs => diffs,
                                     differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                  .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_That_Ignores_Everything_Should_Pass()
        {
            var queryRequest = new { Name = "Son", MinAge = 50 };

            // Every field differs, but a filter that keeps nothing makes the assert pass.
            var expectedPersons = new[]
                                  {
                                      new Person(Id: 123,
                                                 Name: "Totally",
                                                 FirstName: "Different",
                                                 Age: 1,
                                                 Emails: ImmutableList<Email>.Empty)
                                  };

            return Client.AssertQueryAsync<IEnumerable<Person>>(
                       SearchUrl,
                       queryRequest,
                       expectedPersons,
                       differenceFunc: diffs => diffs,
                       differenceFilter: _ => false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public Task DifferenceFilterOnly_Twin_Should_Ignore_Filtered_Difference()
        {
            var queryRequest = new { Name = "Son", MinAge = 50 };

            // Exercises the differenceFilter-only twin (no differenceFunc) added to Query.AsObjectResponse.
            var expectedPersons = ExpectedSonPersons(age: 42);

            return Client.AssertQueryAsync<IEnumerable<Person>>(
                       SearchUrl,
                       queryRequest,
                       expectedPersons,
                       differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }
    }
}