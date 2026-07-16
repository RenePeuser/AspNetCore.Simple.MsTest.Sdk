using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Tests for the per-assert <c>differenceFilter</c> predicate on DELETE /api/v1/persons/1/with-response.
    /// The endpoint returns the deleted person (200 OK) with a dynamic <c>DeletedAt</c> timestamp, so a
    /// deliberately wrong field in the expected JSON is only tolerated when the filter drops it.
    ///
    /// Exercises the filter-only twin overload
    /// <c>AssertDeleteAsync&lt;T&gt;(url, expectedResult, differenceFilter)</c> — the only filter path DELETE
    /// exposes (DELETE has no object-based expected overload with a filter, so the expected value is JSON).
    ///
    /// Note: the assembly-wide <see cref="AssertObjectExtensions.DifferenceFunc"/> is set to
    /// <c>IgnoreIdDifferences</c> in <see cref="ApiTestBase"/>, which already drops <c>id</c> and
    /// <c>deletedAt</c> differences before the filter runs.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonDeleteTests_DifferenceFilter : ApiTestBase
    {
        private const string DeleteUrl = "api/v1/persons/1/with-response";

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            // firstName is intentionally wrong ("Wrong" vs "Goku"); the filter drops firstName differences.
            var expectedResponse = /*lang=json,strict*/ """
                                                        {
                                                            "id": 1,
                                                            "name": "Son",
                                                            "firstName": "Wrong",
                                                            "deleted": true
                                                        }
                                                        """;

            return Client.AssertDeleteAsync<DeletePersonResponse>(DeleteUrl,
                                                                  expectedResponse,
                                                                  differenceFilter: d => !d.MemberPath.Contains("firstName", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        [TestCategory("DifferenceFilter")]
        public async Task PerAssertFilter_Should_Not_Hide_Unrelated_Difference()
        {
            // name is wrong; the filter only ignores firstName, so the name difference must still fail.
            var expectedResponse = /*lang=json,strict*/ """
                                                        {
                                                            "id": 1,
                                                            "name": "WrongName",
                                                            "firstName": "Wrong",
                                                            "deleted": true
                                                        }
                                                        """;

            await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertDeleteAsync<DeletePersonResponse>(DeleteUrl,
                                                                                                                        expectedResponse,
                                                                                                                        differenceFilter: d => !d.MemberPath.Contains("firstName", StringComparison.OrdinalIgnoreCase)))
                        .ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_That_Ignores_Everything_Should_Pass()
        {
            // Every field differs, but a filter that keeps nothing makes the assert pass.
            var expectedResponse = /*lang=json,strict*/ """
                                                        {
                                                            "id": 999,
                                                            "name": "Totally",
                                                            "firstName": "Different",
                                                            "deleted": false
                                                        }
                                                        """;

            return Client.AssertDeleteAsync<DeletePersonResponse>(DeleteUrl,
                                                                  expectedResponse,
                                                                  differenceFilter: _ => false);
        }
    }
}