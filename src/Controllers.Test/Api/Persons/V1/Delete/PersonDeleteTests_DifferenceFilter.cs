using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Delete
{
    /// <summary>
    /// Per-assert <c>differenceFilter</c> tests for DELETE /api/v1/persons/1/with-response on the
    /// CONTROLLER stack. Uses the filter-only twin <c>AssertDeleteAsync&lt;T&gt;(url, expectedResult,
    /// differenceFilter)</c> (DELETE has no object+filter overload, so expected is JSON).
    ///
    /// Controllers.Test is ClassLevel-parallel → per-assert filter only, no static global filter.
    /// The dynamic <c>deletedAt</c> timestamp is omitted from the expected JSON.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    public partial class PersonDeleteTests_DifferenceFilter : ApiTestBase
    {
        private const string DeleteUrl = "api/v1/persons/1/with-response";

        [TestMethod]
        [TestCategory("ObjectResponse")]
        [TestCategory("DELETE")]
        [TestCategory("DifferenceFilter")]
        public Task PerAssertFilter_Should_Ignore_Filtered_Difference()
        {
            // firstName is intentionally wrong; the filter drops firstName differences.
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