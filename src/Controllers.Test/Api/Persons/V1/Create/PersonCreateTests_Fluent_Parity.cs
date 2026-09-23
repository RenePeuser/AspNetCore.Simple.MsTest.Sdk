using System;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Controllers.Api.Persons;
using Controllers.Test.Api.Persons.V1.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Create
{
    /// <summary>
    /// Fluent coverage the CONTROLLER stack was missing while the minimal-api stack already had it:
    /// the three Accepts…/ExpectedResponse… variants, placeholder parameters, custom headers, the int
    /// status overloads, the body-less path and DifferenceFilter.
    ///
    /// <para>
    /// The asymmetry mattered: every fluent bug found so far sat in exactly these members, and the
    /// controller pipeline reaches them through a different endpoint-metadata path than minimal api —
    /// so a green minimal-api suite says nothing about them here.
    /// </para>
    /// </summary>
    public partial class PersonCreateTests
    {
        // ============================================================
        // Request body — the three explicit Accepts… variants.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_AcceptsFromJsonString_Should_Create_Person()
        {
            const string bodyJson = /*lang=json,strict*/
                "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":42,\"Emails\":[]}";

            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromJsonString(bodyJson)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Expected response — object and raw-json variants.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_ExpectedResponse_Object_Should_Match()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponse(person)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_ExpectedResponseFromJsonString_Should_Match()
        {
            var person = TestHelpers.CreateValidPerson();

            const string expectedJson = /*lang=json,strict*/
                "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":42,\"Emails\":[]}";

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromJsonString(expectedJson)
                         .ExecuteAsync();
        }

        // ============================================================
        // Placeholder parameters.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameter_Should_Substitute_Placeholders()
        {
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameter("Name", "Son")
                         .WithParameter("Age", 42)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameters_Tuples_Should_Substitute_Placeholders()
        {
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameters(("Name", "Son"), ("Age", 42))
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameters_Object_Should_Substitute_Placeholders()
        {
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameters(new
                                         {
                                             Name = "Son",
                                             Age = 42
                                         })
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Custom headers — typed path (the one that used to drop them).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        [TestCategory("Header")]
        public async Task Fluent_WithHeader_Should_Reach_Action_On_Typed_Path()
        {
            var echo = await Client.AssertGet("api/v1/persons/echo-header")
                                   .WithHeader("X-Correlation-Id", "controller-typed-42")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExecuteAsync()
                                   .ConfigureAwait(false);

            Assert.That.AreEqual("controller-typed-42", echo.CorrelationId,
                                 because: "WithHeader has to reach the action on the typed Produces<T> path in the controller stack too. The action reflects the header it received, so anything else here means the header never left the client.",
                                 fix: "Check that HttpResponseBuilder passes its collected headers to AssertHttpCallAsync and that HttpRequestMessageBuilder applies context.RequestHeaders to the outgoing request.");
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        [TestCategory("Header")]
        public async Task Fluent_Without_Header_Should_Reach_Action_Without_It()
        {
            var echo = await Client.AssertGet("api/v1/persons/echo-header")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExecuteAsync()
                                   .ConfigureAwait(false);

            Assert.That.AreEqual("(absent)", echo.CorrelationId,
                                 because: "Without WithHeader the action must see no correlation id. If this echoed a value, the header assertion next to it would be passing on leftover state instead of on what the chain sent.",
                                 fix: "Check that the builder starts with an empty header collection per chain and does not share one across chains.");
        }

        // ============================================================
        // Status overloads + body-less path.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Produces_Typed_Int_Overload_Should_Create_Person()
        {
            var person = TestHelpers.CreateValidPerson();

            var result = await Client.AssertPost("api/v1/persons")
                                     .Accepts(person)
                                     .Produces<Person>(200)
                                     .ExecuteAsync()
                                     .ConfigureAwait(false);

            Assert.That.IsNotNull(result,
                                  because: "This is the body-less path: no golden file is compared, so the typed result coming back is the only proof the request went through and was deserialized.",
                                  fix: "Check that ExecuteAsync returns the deserialized body for a Produces<T>(status) expectation instead of only asserting the status code.");
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Produces_BodyLess_Should_Assert_Status_Only()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Produces_BodyLess_Int_Overload_Should_Assert_Status_Only()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces(200)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_BodyLess_Should_Resolve_Embedded_Body()
        {
            // The body-less path used to post the file NAME instead of its content.
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonFull.json")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }

        // ============================================================
        // DifferenceFilter.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        [TestCategory("DifferenceFilter")]
        public Task Fluent_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { Age = 99 };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponse(expected)
                         .DifferenceFilter(d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase))
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        [TestCategory("DifferenceFilter")]
        public async Task Fluent_DifferenceFilter_Should_Not_Hide_Unrelated_Difference()
        {
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { Name = "WrongName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                    .Accepts(person)
                                                                                    .Produces<Person>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(expected)
                                                                                    .DifferenceFilter(d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase))
                                                                                    .ExecuteAsync(),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference has to keep failing - otherwise a single filter would quietly switch off the whole comparison.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.").ConfigureAwait(false);
        }

        // ============================================================
        // IgnoreProperty — path matching and composition.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_IgnoreProperty_Twice_Should_Compose()
        {
            var person = TestHelpers.CreateValidPerson();

            var expected = person with
                           {
                               Name = "WrongName",
                               FirstName = "WrongFirstName"
                           };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.OK)
                         .ExpectedResponse(expected)
                         .IgnoreProperty<Person>(p => p.Name)
                         .IgnoreProperty<Person>(p => p.FirstName)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_IgnoreProperty_Should_Not_Hide_Unrelated_Difference()
        {
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { FirstName = "WrongFirstName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                    .Accepts(person)
                                                                                    .Produces<Person>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(expected)
                                                                                    .IgnoreProperty<Person>(p => p.Name)
                                                                                    .ExecuteAsync(),
                                                                        because: "IgnoreProperty may hide exactly the property it names and nothing else. 'firstName' merely ENDS WITH 'name', so a substring match would drop it too and switch off a real part of the comparison.",
                                                                        fix: "Check that the member-path match compares the last path segment for equality instead of using Contains/EndsWith.").ConfigureAwait(false);
        }
    }
}