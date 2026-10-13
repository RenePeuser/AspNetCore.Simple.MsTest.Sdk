using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for the chain rules that depend on a runtime value and therefore cannot be type-state:
    ///
    /// <list type="bullet">
    /// <item>A parameter name given twice used to send the FIRST value silently (the placeholder is replaced once).</item>
    /// <item>A header name given twice used to send the LAST value silently - the opposite rule.</item>
    /// <item>A second <c>FilterResponse</c> used to drop the first one silently.</item>
    /// </list>
    ///
    /// Repeating a name now fails at the repeating line with the sdk's usual what / why / how-to-fix output;
    /// filters compose. Every positive case is paired with a test that MUST fail.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentChainRulesTests : ApiTestBase
    {
        private const string QueryByNameUrl = "api/v1/persons?name=$Name$";

        private const string EchoHeaderUrl = "api/v1/persons/echo-header";

        private const string CorrelationIdHeader = "X-Correlation-Id";

        private const string ProfileUrl = "api/v1/persons/1/profile";

        private const string DuplicateFix = "Check that HttpRequestBuilder.AddParameters / AddHeader reject a name the chain already holds (parameters ordinal, headers case-insensitive) and throw FluentUsageError's ArgumentException.";

        private const string FilterFix = "Check that HttpResponseBuilder.FilterResponse composes with the previous filter (second runs on the first's output) instead of replacing it.";

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }

        // The real profile is "Son Goku" in "Kame House 1" - this one differs in BOTH, so only a comparison that
        // normalizes both properties goes green.
        private static PersonProfile ProfileWithWrongNameAndStreet()
        {
            return new PersonProfile(Id: 1,
                                     Name: "Wrong",
                                     Address: new ProfileAddress("Home", "Wrong Street 1", "12345"),
                                     Contacts: ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                    new ProfileContact("Private", "goku@kame.house")));
        }

        private static PersonProfile? NormalizeName(PersonProfile? profile)
        {
            return profile is null ? null : profile with { Name = "normalized" };
        }

        private static PersonProfile? NormalizeStreet(PersonProfile? profile)
        {
            return profile is null ? null : profile with { Address = profile.Address with { Street = "normalized" } };
        }

        // ============================================================
        // Parameters.
        // ============================================================

        [TestMethod]
        public void Parameter_Set_Twice_Should_Throw_With_Guidance()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl)
                                                                                     .WithParameter("Name", "Son")
                                                                                     .WithParameter("Name", "Vegeta"),
                                                                         because: "The engine replaces $Name$ with the FIRST value - the second would be dropped silently, so the chain has to stop at the repeating line.",
                                                                         fix: DuplicateFix);

            AssertGuidance(exception.Message, "PLACEHOLDER PARAMETER SET TWICE", "$Name$", "\"Son\"", "\"Vegeta\"", "Client.AssertGet(\"api/v1/persons?name=$Name$\")");
        }

        [TestMethod]
        public void Parameter_Set_Twice_In_One_Call_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl)
                                                                     .WithParameters(("Name", "Son"), ("Name", "Vegeta")),
                                                         because: "A name repeated inside one WithParameters call is the same silent first-wins as across two calls.",
                                                         fix: DuplicateFix);
        }

        [TestMethod]
        public void Parameter_Given_Naked_And_Wrapped_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl)
                                                                     .WithParameter("Name", "Son")
                                                                     .WithParameters(new Dictionary<string, object?> { ["$Name$"] = "Vegeta" }),
                                                         because: "\"Name\" and \"$Name$\" are the same placeholder - the wrapped spelling must not slip past the check.",
                                                         fix: DuplicateFix);
        }

        [TestMethod]
        public Task Parameter_Set_Once_Should_Reach_The_Url()
        {
            return Client.AssertGet(QueryByNameUrl)
                         .WithParameter("Name", "Vegeta")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponse([Vegeta()])
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Parameter_Set_Once_Expecting_Another_Person_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(QueryByNameUrl)
                                                                                    .WithParameter("Name", "Vegeta")
                                                                                    .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse([])
                                                                                    .ExecuteAsync(),
                                                                        because: "Name=Vegeta returns Vegeta - expecting nobody has to fail, so the duplicate check does not swallow the single parameter.",
                                                                        fix: DuplicateFix).ConfigureAwait(false);
        }

        // ============================================================
        // Headers.
        // ============================================================

        [TestMethod]
        public void Header_Set_Twice_Should_Throw_With_Guidance()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoHeaderUrl)
                                                                                     .WithHeader(CorrelationIdHeader, "first")
                                                                                     .WithHeader(CorrelationIdHeader, "second"),
                                                                         because: "The second value used to replace the first silently - the chain has to stop at the repeating line.",
                                                                         fix: DuplicateFix);

            AssertGuidance(exception.Message, "REQUEST HEADER SET TWICE", CorrelationIdHeader, "\"first\"", "\"second\"", "first, second");
        }

        [TestMethod]
        public void Header_Set_Twice_In_Another_Casing_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoHeaderUrl)
                                                                     .WithHeader(CorrelationIdHeader, "first")
                                                                     .WithHeader(CorrelationIdHeader.ToUpperInvariant(), "second"),
                                                         because: "HTTP header names are case-insensitive - X-CORRELATION-ID is the same header and would have been sent as a second value.",
                                                         fix: DuplicateFix);
        }

        [TestMethod]
        public Task Header_Set_Once_Should_Be_Sent()
        {
            return Client.AssertGet(EchoHeaderUrl)
                         .WithHeader(CorrelationIdHeader, "once")
                         .WithHeader("X-Other", "other")
                         .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(new EchoHeaderResponse("once"))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Header_Set_Once_Expecting_Another_Value_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoHeaderUrl)
                                                                                    .WithHeader(CorrelationIdHeader, "once")
                                                                                    .WithHeader("X-Other", "other")
                                                                                    .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new EchoHeaderResponse("other"))
                                                                                    .ExecuteAsync(),
                                                                        because: "Two DIFFERENT headers are fine and each keeps its own value - echoing X-Other's value has to fail.",
                                                                        fix: DuplicateFix).ConfigureAwait(false);
        }

        // ============================================================
        // FilterResponse composes.
        // ============================================================

        [TestMethod]
        public Task Two_Filters_Should_Both_Apply()
        {
            return Client.AssertGet(ProfileUrl)
                         .Produces<PersonProfile>(HttpStatusCode.OK)
                         .ExpectedResponse(ProfileWithWrongNameAndStreet())
                         .FilterResponse(NormalizeName)
                         .FilterResponse(NormalizeStreet)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Only_The_Second_Filter_Should_Leave_The_Name_Difference()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(ProfileUrl)
                                                                                    .Produces<PersonProfile>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(ProfileWithWrongNameAndStreet())
                                                                                    .FilterResponse(NormalizeStreet)
                                                                                    .ExecuteAsync(),
                                                                        because: "Without the name filter the name still differs - the green two-filter test is green only because BOTH filters ran.",
                                                                        fix: FilterFix).ConfigureAwait(false);
        }

        private static void AssertGuidance(string message,
                                           params string[] expectedParts)
        {
            foreach (var part in expectedParts)
            {
                Assert.That.Contains(message,
                                     part,
                                     because: "The usage error has to say what was repeated, with both values, why it fails and how to fix it - in the sdk's usual layout.",
                                     fix: "Check FluentUsageError: title, What Happened (name + both values), Why This Fails, How To Fix with a concrete snippet.");
            }

            foreach (var section in new[] { "What Happened", "Why This Fails", "How To Fix", "Test Information" })
            {
                Assert.That.Contains(message,
                                     section,
                                     because: "Every usage error carries the same sections as the sdk's assertion output.",
                                     fix: "Check FluentUsageError.Build.");
            }
        }
    }
}
