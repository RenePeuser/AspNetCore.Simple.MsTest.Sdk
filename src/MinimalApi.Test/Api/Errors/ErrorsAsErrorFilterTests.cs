using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Exercises <c>differenceFilter</c> on the <c>AsError</c> asserts for every verb that previously had
    /// no error-endpoint coverage (GET/PUT/PATCH/DELETE). The <c>errors/{verb}-not-implemented</c> endpoints
    /// all throw the same ProblemDetails (500), so a deliberately wrong "detail" in the expected file is
    /// tolerated only when the filter drops it — and a wrong "title" must still fail.
    ///
    /// Before this, only POST-AsError was covered (ErrorsEndpointsTests). PUT/PATCH send a throwaway body.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("DifferenceFilter")]
    public class ErrorsAsErrorFilterTests : ApiTestBase
    {
        private const string WrongDetailFile = "ErrorResponseWrongDetail.json";

        private static bool KeepUnlessDetail(Difference d)
        {
            return !d.MemberPath.Contains("detail", StringComparison.OrdinalIgnoreCase);
        }

        private static bool KeepUnlessTitle(Difference d)
        {
            return !d.MemberPath.Contains("title", StringComparison.OrdinalIgnoreCase);
        }

        private static readonly object ThrowawayBody = new { Value = "ignored" };

        // ============================================================
        // GET
        // ============================================================

        [TestMethod]
        [TestCategory("GET")]
        public Task GetAsError_With_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            return Client.AssertGetAsErrorAsync<ProblemDetails>("api/v1/errors/get-not-implemented",
                                                                WrongDetailFile,
                                                                differenceFilter: KeepUnlessDetail);
        }

        [TestMethod]
        [TestCategory("GET")]
        public async Task GetAsError_With_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            await Assert.ThrowsExactlyAsync<AssertFailedException>(
                      () => Client.AssertGetAsErrorAsync<ProblemDetails>("api/v1/errors/get-not-implemented",
                                                                         WrongDetailFile,
                                                                         differenceFilter: KeepUnlessTitle))
                  .ConfigureAwait(false);
        }

        // ============================================================
        // DELETE
        // ============================================================

        [TestMethod]
        [TestCategory("DELETE")]
        public Task DeleteAsError_With_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            return Client.AssertDeleteAsErrorAsync<ProblemDetails>("api/v1/errors/delete-not-implemented",
                                                                   WrongDetailFile,
                                                                   differenceFilter: KeepUnlessDetail);
        }

        [TestMethod]
        [TestCategory("DELETE")]
        public async Task DeleteAsError_With_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            await Assert.ThrowsExactlyAsync<AssertFailedException>(
                      () => Client.AssertDeleteAsErrorAsync<ProblemDetails>("api/v1/errors/delete-not-implemented",
                                                                            WrongDetailFile,
                                                                            differenceFilter: KeepUnlessTitle))
                  .ConfigureAwait(false);
        }

        // ============================================================
        // PUT
        // ============================================================

        [TestMethod]
        [TestCategory("PUT")]
        public Task PutAsError_With_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            return Client.AssertPutAsErrorAsync<ProblemDetails>("api/v1/errors/put-not-implemented",
                                                                ThrowawayBody,
                                                                WrongDetailFile,
                                                                differenceFilter: KeepUnlessDetail);
        }

        [TestMethod]
        [TestCategory("PUT")]
        public async Task PutAsError_With_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            await Assert.ThrowsExactlyAsync<AssertFailedException>(
                      () => Client.AssertPutAsErrorAsync<ProblemDetails>("api/v1/errors/put-not-implemented",
                                                                         ThrowawayBody,
                                                                         WrongDetailFile,
                                                                         differenceFilter: KeepUnlessTitle))
                  .ConfigureAwait(false);
        }

        // ============================================================
        // PATCH
        // ============================================================

        [TestMethod]
        [TestCategory("PATCH")]
        public Task PatchAsError_With_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            return Client.AssertPatchAsErrorAsync<ProblemDetails>("api/v1/errors/patch-not-implemented",
                                                                  ThrowawayBody,
                                                                  WrongDetailFile,
                                                                  differenceFilter: KeepUnlessDetail);
        }

        [TestMethod]
        [TestCategory("PATCH")]
        public async Task PatchAsError_With_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            await Assert.ThrowsExactlyAsync<AssertFailedException>(
                      () => Client.AssertPatchAsErrorAsync<ProblemDetails>("api/v1/errors/patch-not-implemented",
                                                                           ThrowawayBody,
                                                                           WrongDetailFile,
                                                                           differenceFilter: KeepUnlessTitle))
                  .ConfigureAwait(false);
        }
    }
}
