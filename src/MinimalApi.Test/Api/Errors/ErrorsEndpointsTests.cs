using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    [TestClass]
    [TestCategory("Minimal Api")]
    public class ErrorsEndpointsTests : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json");
        }

        [TestMethod]
        public Task Should_Handle_Error_Response_With_Filter_Func()
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 DifferenceFunc);

            static IEnumerable<Difference> DifferenceFunc(ImmutableList<Difference> differences)
            {
                foreach (var difference in differences)
                {
                    yield return difference;
                }
            }
        }

        [TestMethod]
        public Task Should_Return_500_When_Expected_Status_Code_Is_Specified()
        {
            // Test with explicit expectedHttpStatusCode parameter (500 InternalServerError)
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task Should_Fail_When_Expected_Status_Code_Does_Not_Match()
        {
            // This test should fail because we expect 404 but get 500
            // The endpoint returns 500 (InternalServerError)
            // We don't provide an expected JSON file, so only the status code is checked
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                            Client.AssertPostAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                   writeResponse: false,
                                                                                                                   skipEndpointValidation: true,
                                                                                                                   expectedHttpStatusCode: System.Net.HttpStatusCode.NotFound),
                                                                        because: "The endpoint answers 500, the test declares 404. With no expected json only the status code is compared, so this is the narrowest possible check that the comparison happens at all.",
                                                                        fix: "Check that the status code is compared against expectedHttpStatusCode even when no snapshot is given - skipping the check for a bodyless assert is the likely cause.")
                        .ConfigureAwait(false);
        }

        [TestMethod]
        public Task Should_Pass_When_Expected_Status_Code_Matches()
        {
            // This test should pass because we explicitly expect 500
            // and the endpoint returns 500 (InternalServerError)
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponse.json",
                                                                 differenceFilter: DifferenceFilter,
                                                                 expectedHttpStatusCode: System.Net.HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [TestCategory("DifferenceFilter")]
        public Task DifferenceFilter_Only_Should_Ignore_Filtered_Difference()
        {
            // The expected file intentionally has a wrong "detail" value; the standalone
            // differenceFilter (no differenceFunc) drops that difference so the assert passes.
            // Proves the filter-only overload actually wires the filter into the comparison.
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                 "ErrorResponseWrongDetail.json",
                                                                 differenceFilter: d => !d.MemberPath.Contains("detail", System.StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [TestCategory("DifferenceFilter")]
        public async Task DifferenceFilter_Only_Should_Not_Hide_Unrelated_Difference()
        {
            // The filter only drops "detail" differences, so a wrong "title" must still fail.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/errors/not-implemented",
                                                                                                                            "ErrorResponseWrongDetail.json",
                                                                                                                            differenceFilter: d => !d.MemberPath.Contains("title", System.StringComparison.OrdinalIgnoreCase)),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.")
                        .ConfigureAwait(false);
        }

        private bool DifferenceFilter(Difference obj)
        {
            return true;
        }
    }
}