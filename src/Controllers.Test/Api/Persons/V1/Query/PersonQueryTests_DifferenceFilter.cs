using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Query
{
    /// <summary>
    /// Per-assert <c>differenceFilter</c> tests for QUERY /api/v1/persons/search on the CONTROLLER stack.
    /// Uses the object-based Query overloads (added to close the Query.AsObjectResponse gap) incl. the
    /// differenceFilter-only twin.
    ///
    /// Controllers.Test is ClassLevel-parallel → per-assert filter only, no static global filter.
    /// The assembly-wide <c>DifferenceFunc</c> already ignores <c>id</c>.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonQueryTests_DifferenceFilter : ApiTestBase
    {
        private const string SearchUrl = "api/v1/persons/search";

        // Query {Name="Son", MinAge=50} returns exactly Person 1 (Son/Goku/99).
        private static Person[] ExpectedSonPersons(int age = 99,
                                                   string name = "Son")
        {
            return
            [
                new Person(Id: 1,
                           Name: name,
                           FirstName: "Goku",
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
            var queryRequest = new
                               {
                                   Name = "Son",
                                   MinAge = 50
                               };

            return Client.AssertQueryAsync<IEnumerable<Person>>(SearchUrl,
                                                                queryRequest,
                                                                ExpectedSonPersons(age: 42),
                                                                differenceFunc: diffs => diffs,
                                                                differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public async Task PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var queryRequest = new
                               {
                                   Name = "Son",
                                   MinAge = 50
                               };

            await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertQueryAsync<IEnumerable<Person>>(SearchUrl,
                                                                                                                      queryRequest,
                                                                                                                      ExpectedSonPersons(age: 42, name: "WrongName"),
                                                                                                                      differenceFunc: diffs => diffs,
                                                                                                                      differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)))
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("QUERY")]
        [TestCategory("DifferenceFilter")]
        public Task DifferenceFilterOnly_Twin_Should_Ignore_Filtered_Difference()
        {
            var queryRequest = new
                               {
                                   Name = "Son",
                                   MinAge = 50
                               };

            return Client.AssertQueryAsync<IEnumerable<Person>>(SearchUrl,
                                                                queryRequest,
                                                                ExpectedSonPersons(age: 42),
                                                                differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }
    }
}