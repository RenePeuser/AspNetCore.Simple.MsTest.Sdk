using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Shared
{
    /// <summary>
    /// Combination coverage for the difference-filtering parameters that were previously never
    /// exercised together through the HTTP asserts:
    ///   - <c>filterFunc</c> + <c>differenceFunc</c> + <c>differenceFilter</c> at the same time (F+D+df),
    ///   - <c>parameters</c> combined with a <c>differenceFilter</c>,
    ///   - <c>AsError</c> asserts combined with a <c>differenceFilter</c> (POST and QUERY — the only
    ///     verbs the MinimalApi sample exposes an error endpoint for).
    ///
    /// GET /api/v1/persons/1 always returns Person(1, "Son", "Goku", 99, [alf@gmx.de/GMX, abc@hotmail.de/Microsoft]).
    /// The assembly-wide <see cref="AssertObjectExtensions.DifferenceFunc"/> already ignores <c>id</c>.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("DifferenceFilter")]
    public partial class DifferenceFilterCombinationTests : ApiTestBase
    {
        private const string PersonUrl = "api/v1/persons/1";

        private const string ErrorUrl = "api/v1/errors/not-implemented";

        private static Person ExpectedPerson(int age = 99,
                                             string name = "Son",
                                             string firstName = "Goku")
        {
            return new Person(Id: 1,
                              Name: name,
                              FirstName: firstName,
                              Age: age,
                              Emails: ImmutableList.Create(new Email("alf@gmx.de", "GMX"),
                                                           new Email("abc@hotmail.de", "Microsoft")));
        }

        // ============================================================
        // F + D + df together (GET).
        //
        // Note on filterFunc semantics: filterFunc (internally "OrderFunc") normalizes BOTH sides of the
        // diff - the expected side read from the snapshot and the current side coming off the wire. Here
        // it is the identity on purpose: its job is to exercise the F+D+df overload wiring, while
        // differenceFunc (drops firstName) and differenceFilter (drops age) do the actual dropping.
        // See Controllers.Test FilterFunc/OrderFuncTests for what a non-identity filterFunc has to do.
        // ============================================================

        [TestMethod]
        [TestCategory("GET")]
        public Task FilterFunc_And_DifferenceFunc_And_DifferenceFilter_Should_All_Apply()
        {
            // firstName wrong (dropped by differenceFunc) and age wrong (dropped by differenceFilter);
            // emails/name are correct. All three params supplied together → assert passes.
            var expected = ExpectedPerson(age: 1, firstName: "WrongFirst");

            return Client.AssertGetAsync(PersonUrl,
                                         expected,
                                         filterFunc: person => person,
                                         differenceFunc: diffs => diffs.Where(d => !d.MemberPath.Contains("firstName", StringComparison.OrdinalIgnoreCase)),
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("GET")]
        public async Task FilterFunc_And_DifferenceFunc_And_DifferenceFilter_Should_Not_Hide_Unrelated()
        {
            // Same three mechanisms, but now "name" also differs and nothing drops it → must still fail.
            var expected = ExpectedPerson(age: 1, name: "WrongName", firstName: "WrongFirst");

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync(PersonUrl,
                                                                                                    expected,
                                                                                                    filterFunc: person => person,
                                                                                                    differenceFunc: diffs => diffs.Where(d => !d.MemberPath.Contains("firstName", StringComparison.OrdinalIgnoreCase)),
                                                                                                    differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }

        // ============================================================
        // parameters[] combined with differenceFilter (GET).
        // ============================================================

        [TestMethod]
        [TestCategory("GET")]
        public Task Parameters_Combined_With_DifferenceFilter_Should_Apply()
        {
            // No placeholders to resolve here, but this exercises the (parameters, differenceFilter)
            // overload wiring end-to-end: an empty parameter set plus a filter that drops the age diff.
            var expected = ExpectedPerson(age: 42);

            return Client.AssertGetAsync(PersonUrl,
                                         expected,
                                         parameters: [],
                                         differenceFilter: d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase));
        }

        // ============================================================
        // POST AsError + differenceFilter (positive + negative).
        // ============================================================

        [TestMethod]
        [TestCategory("POST")]
        public Task PostAsError_With_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            // The expected file intentionally has a wrong "detail"; the filter drops that difference.
            return Client.AssertPostAsErrorAsync<ProblemDetails>(ErrorUrl,
                                                                 "Responses.ErrorResponseWrongDetail.json",
                                                                 differenceFilter: d => !d.MemberPath.Contains("detail", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("POST")]
        public async Task PostAsError_With_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            // The filter only drops "detail"; a wrong "title" must still fail.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPostAsErrorAsync<ProblemDetails>(ErrorUrl,
                                                                                                                            "Responses.ErrorResponseWrongDetail.json",
                                                                                                                            differenceFilter: d => !d.MemberPath.Contains("title", StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }
    }
}