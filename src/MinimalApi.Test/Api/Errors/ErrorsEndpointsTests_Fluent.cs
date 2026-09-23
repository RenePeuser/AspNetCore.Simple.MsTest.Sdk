using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Fluent API tests for the error path (Endpoint-Stil: chain ends in ExecuteAsync).
    ///
    /// <para>
    /// The fluent API derives <c>isSuccessStatusCode</c> from the status handed to <c>Produces</c>
    /// (2xx → success test, everything else → error test). Until now every fluent test named a 2xx, so
    /// the whole error branch — ProblemDetails deserialization, error-specific output, the
    /// success/error-vs-status-code cross-check — was never reached through this API even though the
    /// native twin covers it thoroughly.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Error")]
    public sealed class ErrorsEndpointsTestsFluent : ApiTestBase
    {
        private const string NotImplementedUrl = "api/v1/errors/not-implemented";

        // ============================================================
        // Typed error response — Produces<ProblemDetails>(5xx).
        // ============================================================

        [TestMethod]
        public Task Fluent_Should_Return_ProblemDetails_For_Error_Status()
        {
            return Client.AssertPost(NotImplementedUrl)
                         .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
                         .ExpectedResponseFromEmbeddedJson("ErrorResponse.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_Should_Return_Deserialized_ProblemDetails()
        {
            // Body-less error path: no snapshot, but the typed result still has to come back so a test
            // can assert on it directly.
            var problem = await Client.AssertPost(NotImplementedUrl)
                                      .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
                                      .ExecuteAsync()
                                      .ConfigureAwait(false);

            Assert.That.AreEqual("Implementation is missing", problem.Title,
                                 because: "On the error path ExecuteAsync must still deserialize and return the body. Without that a test can only check the status code and never what the api actually reported.",
                                 fix: "Check that the error branch (isSuccessStatusCode false) runs the same deserialization as the success branch instead of short-circuiting after the status check.");
        }

        [TestMethod]
        public Task Fluent_Should_Return_ProblemDetails_Via_Int_Overload()
        {
            return Client.AssertPost(NotImplementedUrl)
                         .Produces<ProblemDetails>(500)
                         .ExpectedResponseFromEmbeddedJson("ErrorResponse.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Status-code mismatch must FAIL on the error path too.
        // ============================================================

        [TestMethod]
        public async Task Fluent_Should_Fail_When_Error_Status_Does_Not_Match()
        {
            // The endpoint answers 500, the chain declares 404. This is the narrowest check that the
            // status comparison happens at all on the error path.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(NotImplementedUrl)
                                                                                    .Produces<ProblemDetails>(HttpStatusCode.NotFound)
                                                                                    .ExecuteAsync(),
                                                                        because: "A declared error status is part of the contract. If a wrong one passed, every fluent error test would assert nothing but 'something went wrong'.",
                                                                        fix: "Check that expectedHttpStatusCode is compared even when the chain carries no expected body, on the error branch as well as the success one.").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Fluent_Should_Fail_When_Success_Status_Declared_For_Failing_Endpoint()
        {
            // Declaring 200 for an endpoint that throws flips isSuccessStatusCode to true — the
            // success/error classification and the actual response then disagree, which must fail.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(NotImplementedUrl)
                                                                                    .Produces<ProblemDetails>(HttpStatusCode.OK)
                                                                                    .ExecuteAsync(),
                                                                        because: "Produces(200) against an endpoint that answers 500 must not pass. The status drives whether the SDK treats the call as a success or an error test, so a mismatch here would silently reclassify the whole assertion.",
                                                                        fix: "Check that the status code validation runs before/independently of the success-vs-error classification derived from it.").ConfigureAwait(false);
        }

        // ============================================================
        // Comparison config on the error path.
        // ============================================================

        [TestMethod]
        [TestCategory("DifferenceFilter")]
        public Task Fluent_DifferenceFilter_Should_Ignore_Filtered_Difference_On_Error_Path()
        {
            // ErrorResponseWrongDetail.json carries a deliberately wrong "detail"; the filter drops it.
            return Client.AssertPost(NotImplementedUrl)
                         .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
                         .ExpectedResponseFromEmbeddedJson("ErrorResponseWrongDetail.json")
                         .DifferenceFilter(d => !d.MemberPath.Contains("detail", StringComparison.OrdinalIgnoreCase))
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("DifferenceFilter")]
        public async Task Fluent_DifferenceFilter_Should_Not_Hide_Unrelated_Difference_On_Error_Path()
        {
            // The filter only drops "title" differences, so the wrong "detail" must still fail.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(NotImplementedUrl)
                                                                                    .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
                                                                                    .ExpectedResponseFromEmbeddedJson("ErrorResponseWrongDetail.json")
                                                                                    .DifferenceFilter(d => !d.MemberPath.Contains("title", StringComparison.OrdinalIgnoreCase))
                                                                                    .ExecuteAsync(),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference has to keep failing - otherwise a single filter would quietly switch off the whole comparison on the error path.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.").ConfigureAwait(false);
        }

        [TestMethod]
        public Task Fluent_IgnoreProperty_Should_Work_On_Error_Path()
        {
            return Client.AssertPost(NotImplementedUrl)
                         .Produces<ProblemDetails>(HttpStatusCode.InternalServerError)
                         .ExpectedResponseFromEmbeddedJson("ErrorResponseWrongDetail.json")
                         .IgnoreProperty<ProblemDetails>(p => p.Detail)
                         .ExecuteAsync();
        }

        // ============================================================
        // Body-less error expectation — status only.
        // ============================================================

        [TestMethod]
        public Task Fluent_BodyLess_Should_Assert_Error_Status_Only()
        {
            return Client.AssertPost(NotImplementedUrl)
                         .Produces(HttpStatusCode.InternalServerError)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Fluent_BodyLess_Should_Fail_On_Wrong_Error_Status()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(NotImplementedUrl)
                                                                                    .Produces(HttpStatusCode.BadRequest)
                                                                                    .ExecuteAsync(),
                                                                        because: "The body-less path asserts nothing BUT the status code, so if a wrong one passed there the chain would assert nothing at all.",
                                                                        fix: "Check that the body-less builder forwards expectedHttpStatusCode to the engine and that the status validation step runs for it.").ConfigureAwait(false);
        }
    }
}
