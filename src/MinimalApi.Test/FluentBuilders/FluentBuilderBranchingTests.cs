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
    /// Consumer tests for reusing a stored fluent stage as the base of several chains.
    ///
    /// <para>
    /// The builders used to hand their parameter list and header dictionary BY REFERENCE through
    /// <c>Produces…</c> and to return <c>this</c> from every configuring call. Configuring a stored base request
    /// further therefore changed chains that had already been derived from it: the second branch's header
    /// overwrote the first branch's, the second branch's parameter was appended to the first branch's list,
    /// and a lenient ignore rule leaked into the strict chain - which then went green without comparing.
    /// </para>
    ///
    /// <para>
    /// Every branching test is paired with a test that MUST fail: it proves the branch really sends / compares
    /// its own values, so the green test is green for the right reason.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class FluentBuilderBranchingTests : ApiTestBase
    {
        private const string EchoHeaderUrl = "api/v1/persons/echo-header";

        private const string CorrelationIdHeader = "X-Correlation-Id";

        private const string QueryByNameUrl = "api/v1/persons?name=$Name$";

        private const string ProfileUrl = "api/v1/persons/1/profile";

        private const string ImmutabilityFix = "Check that every configuring call on the fluent builders returns a NEW builder (copy-on-write over immutable collections) instead of mutating and returning this.";

        private static Person Son()
        {
            return new Person(1, "Son", "Goku", 99, ImmutableList.Create(new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft")));
        }

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }

        private static PersonProfile ProfileWithWrongName()
        {
            return new PersonProfile(Id: 1,
                                     Name: "Wrong",
                                     Address: new ProfileAddress("Home", "Kame House 1", "12345"),
                                     Contacts: ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                    new ProfileContact("Private", "goku@kame.house")));
        }

        // ============================================================
        // Headers — sibling branches of one base request each send their own value. (Overriding a header the
        // base already holds is a usage error since the duplicate-name rule - see FluentChainRulesTests.)
        // ============================================================

        [TestMethod]
        public async Task Header_Branches_Of_One_Base_Request_Should_Not_Overwrite_Each_Other()
        {
            var baseRequest = Client.AssertGet(EchoHeaderUrl);

            var first = baseRequest.WithHeader(CorrelationIdHeader, "first")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExpectedResponse(new EchoHeaderResponse("first"));

            var second = baseRequest.WithHeader(CorrelationIdHeader, "second")
                                    .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                    .ExpectedResponse(new EchoHeaderResponse("second"));

            await first.ExecuteAsync().ConfigureAwait(false);
            await second.ExecuteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Header_Branch_Should_Send_Its_Own_Value()
        {
            var baseRequest = Client.AssertGet(EchoHeaderUrl);

            var first = baseRequest.WithHeader(CorrelationIdHeader, "first")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExpectedResponse(new EchoHeaderResponse("second"));

            _ = baseRequest.WithHeader(CorrelationIdHeader, "second");

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => first.ExecuteAsync(),
                                                                        because: "The branch sends 'first' - a sibling branch of the base request must not turn it into 'second'. Expecting 'second' has to fail.",
                                                                        fix: ImmutabilityFix).ConfigureAwait(false);
        }

        // ============================================================
        // Parameters — two branches of one base request each send their own value.
        // ============================================================

        [TestMethod]
        public async Task Parameter_Branches_Of_One_Base_Request_Should_Not_Accumulate()
        {
            var baseRequest = Client.AssertGet(QueryByNameUrl);

            var sons = baseRequest.WithParameter("Name", "Son")
                                  .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                  .ExpectedResponse([Son()]);

            var vegetas = baseRequest.WithParameter("Name", "Vegeta")
                                     .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                     .ExpectedResponse([Vegeta()]);

            await sons.ExecuteAsync().ConfigureAwait(false);
            await vegetas.ExecuteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Parameter_Branch_Should_Send_Its_Own_Value()
        {
            var baseRequest = Client.AssertGet(QueryByNameUrl);

            _ = baseRequest.WithParameter("Name", "Son");

            var vegetas = baseRequest.WithParameter("Name", "Vegeta")
                                     .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                     .ExpectedResponse([Son()]);

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => vegetas.ExecuteAsync(),
                                                                        because: "The branch sends Name=Vegeta, so the response holds Vegeta. Expecting Son has to fail - otherwise the other branch's parameter leaked in.",
                                                                        fix: ImmutabilityFix).ConfigureAwait(false);
        }

        // ============================================================
        // Comparison — a lenient branch must not loosen the strict chain.
        // ============================================================

        [TestMethod]
        public Task Lenient_Branch_Of_A_Stored_Comparison_Should_Ignore_Its_Property()
        {
            var strict = Client.AssertGet(ProfileUrl)
                               .Produces<PersonProfile>(HttpStatusCode.OK)
                               .ExpectedResponse(ProfileWithWrongName());

            return strict.IgnoreProperty(p => p.Name)
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Strict_Chain_Should_Still_Fail_After_A_Lenient_Branch_Was_Derived()
        {
            var strict = Client.AssertGet(ProfileUrl)
                               .Produces<PersonProfile>(HttpStatusCode.OK)
                               .ExpectedResponse(ProfileWithWrongName());

            _ = strict.IgnoreProperty(p => p.Name);

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => strict.ExecuteAsync(),
                                                                        because: "The strict chain has no ignore rule - the wrong name must fail. A lenient branch derived from it must not switch the comparison off.",
                                                                        fix: ImmutabilityFix).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Expected_Response_Branches_Of_One_Produces_Should_Not_Overwrite_Each_Other()
        {
            var produces = Client.AssertGet(QueryByNameUrl)
                                 .WithParameter("Name", "Son")
                                 .Produces<IEnumerable<Person>>(HttpStatusCode.OK);

            var correct = produces.ExpectedResponse([Son()]);

            _ = produces.ExpectedResponse([Vegeta()]);

            await correct.ExecuteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Expected_Response_Branch_Should_Compare_Its_Own_Expectation()
        {
            var produces = Client.AssertGet(QueryByNameUrl)
                                 .WithParameter("Name", "Son")
                                 .Produces<IEnumerable<Person>>(HttpStatusCode.OK);

            var wrong = produces.ExpectedResponse([Vegeta()]);

            _ = produces.ExpectedResponse([Son()]);

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => wrong.ExecuteAsync(),
                                                                        because: "This branch expects Vegeta while the response holds Son - it has to fail, no matter what a sibling branch expects.",
                                                                        fix: ImmutabilityFix).ConfigureAwait(false);
        }
    }
}
