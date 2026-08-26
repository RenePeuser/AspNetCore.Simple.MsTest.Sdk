using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Create
{
    /// <summary>
    ///     Fluent API tests for POST /api/v1/persons endpoint (Endpoint-Stil: chain ends in ExecuteAsync).
    /// </summary>
    public partial class PersonCreateTests
    {
        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Should_Create_Person_With_Object()
        {
            var person = TestHelpers.CreateValidPerson();

            await Client.AssertPost("api/v1/persons")
                        .Accepts(person)
                        .Produces<Person>(HttpStatusCode.Created)
                        .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                        .ExecuteAsync().ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Emails()
        {
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Json()
        {
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonFull.json")
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_Ignore_Id()
        {
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                         .IgnoreDifferences(TestHelpers.IgnoreIdDifferences)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Should_Create_Person_With_Type_Safe_Property_Ignore()
        {
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .IgnoreProperty<Person>(p => p.Id)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Should_Create_Person_Without_Body_Comparison()
        {
            var person = TestHelpers.CreateValidPerson();

            // Body-less path: only status + typed result, no golden-file comparison (§15.6).
            var result = await Client.AssertPost("api/v1/persons")
                                     .Accepts(person)
                                     .Produces<Person>(HttpStatusCode.Created)
                                     .ExecuteAsync().ConfigureAwait(false);

            Assert.That.IsNotNull(result,
                                  because: "This is the body-less path: no golden file is compared, so the typed result coming back is the only proof the request went through and was deserialized.",
                                  fix: "Check that ExecuteAsync returns the deserialized body for a Produces<T>(status) expectation instead of only asserting the status code.");
        }

        // ============================================================
        // Request body input — the three explicit Accepts… variants (Schema A, no rate-heuristic).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Accepts_Object_Should_Create_Person()
        {
            // IHttpRequestConfiguring.Accepts<T>(T) — C# object serialized to JSON.
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponse(person)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_AcceptsFromJsonString_Should_Create_Person()
        {
            // IHttpRequestConfiguring.AcceptsFromJsonString(string) — raw JSON body, used verbatim.
            const string bodyJson = /*lang=json,strict*/
                "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":42,\"Emails\":[]}";

            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromJsonString(bodyJson)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_AcceptsFromEmbeddedJson_Should_Create_Person()
        {
            // IHttpRequestConfiguring.AcceptsFromEmbeddedJson(string) — embedded resource file name.
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonFull.json")
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonFull.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Expected response input — the three ExpectedResponse… variants.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_ExpectedResponse_Object_Should_Match()
        {
            // IHttpResponseConfiguring.ExpectedResponse(TResult) — expected body from a C# object.
            var person = TestHelpers.CreatePersonWithEmails();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponse(person)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_ExpectedResponseFromJsonString_Should_Match()
        {
            // IHttpResponseConfiguring.ExpectedResponseFromJsonString(string) — expected body as raw JSON.
            var person = TestHelpers.CreateValidPerson();

            const string expectedJson = /*lang=json,strict*/
                "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":42,\"Emails\":[]}";

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromJsonString(expectedJson)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_ExpectedResponseFromEmbeddedJson_Should_Match()
        {
            // IHttpResponseConfiguring.ExpectedResponseFromEmbeddedJson(string) — expected body from embedded file.
            var person = TestHelpers.CreateValidPerson();

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Placeholder parameters — WithParameter / WithParameters ($Token$ substitution, §10.1).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameter_Should_Substitute_Placeholders()
        {
            // IHttpRequestConfiguring.WithParameter(key, value) — naked names, SDK adds the delimiters.
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameter("Name", "Son")
                         .WithParameter("Age", 42)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameters_Tuples_Should_Substitute_Placeholders()
        {
            // IHttpRequestConfiguring.WithParameters(params (string, object?)[]) — many params at once.
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameters(("Name", "Son"), ("Age", 42))
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithParameters_Object_Should_Substitute_Placeholders()
        {
            // IHttpRequestConfiguring.WithParameters(object) — params from an object's public properties (§10.1.1).
            return Client.AssertPost("api/v1/persons")
                         .AcceptsFromEmbeddedJson("CreatePersonParameterized.json")
                         .WithParameters(new
                         {
                             Name = "Son",
                             Age = 42
                         })
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePersonParameterized.json")
                         .ExecuteAsync();
        }

        // ============================================================
        // Custom headers — WithHeader, on BOTH paths.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WithHeader_Should_Create_Person()
        {
            var person = TestHelpers.CreateValidPerson();

            // IHttpRequestConfiguring.WithHeader(key, value) — a custom request header rides along.
            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .WithHeader("X-Correlation-Id", "fluent-test-42")
                         .Produces(HttpStatusCode.Created)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        [TestCategory("Header")]
        public async Task Fluent_WithHeader_Should_Reach_Endpoint_On_Typed_Path()
        {
            // The header used to be collected by the builder and then silently dropped: the typed path
            // never passed it to the engine, and the engine had no way to send one. Nothing failed - the
            // request simply went out without the header, which is why the documented example in
            // EXAMPLES.md ("Custom headers") never actually did anything. Echoing it back is the only way
            // to prove it left the client.
            var echo = await Client.AssertGet("api/v1/persons/echo-header")
                                   .WithHeader("X-Correlation-Id", "fluent-typed-42")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExecuteAsync()
                                   .ConfigureAwait(false);

            Assert.That.AreEqual("fluent-typed-42", echo.CorrelationId,
                                 because: "WithHeader has to reach the endpoint on the typed Produces<T> path too, not just on the body-less one. The endpoint reflects the header it received, so anything else here means the header never left the client.",
                                 fix: "Check that HttpResponseBuilder passes its collected headers to AssertHttpCallAsync and that HttpRequestMessageBuilder applies context.RequestHeaders to the outgoing request.");
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("GET")]
        [TestCategory("Header")]
        public async Task Fluent_Without_Header_Should_Reach_Endpoint_Without_It()
        {
            // Counter-test: proves the assertion above measures the header and not a constant.
            var echo = await Client.AssertGet("api/v1/persons/echo-header")
                                   .Produces<EchoHeaderResponse>(HttpStatusCode.OK)
                                   .ExecuteAsync()
                                   .ConfigureAwait(false);

            Assert.That.AreEqual("(absent)", echo.CorrelationId,
                                 because: "Without WithHeader the endpoint must see no correlation id. If this echoed a value, the header assertion next to it would be passing on leftover state instead of on what the chain sent.",
                                 fix: "Check that the builder starts with an empty header collection per chain and does not share one across chains.");
        }

        // ============================================================
        // IgnoreProperty — member-path matching and composition.
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_IgnoreProperty_Should_Ignore_Nested_Property()
        {
            // The differ reports qualified paths ("content.value.emails[0].emailAddress"), never the bare
            // property name. Comparing the WHOLE path against "EmailAddress" matched nothing, so this
            // ignore silently did nothing and the test only stayed green while the values happened to
            // agree. Here they deliberately do not.
            var person = TestHelpers.CreatePersonWithEmails();

            var expected = person with
            {
                Emails = ImmutableList.Create(new Email("wrong@example.com", person.Emails[0].Type))
                                      .AddRange(person.Emails.RemoveAt(0))
            };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponse(expected)
                         .IgnoreProperty<Email>(e => e.EmailAddress)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_IgnoreProperty_Twice_Should_Compose()
        {
            // Both ignores have to survive. They used to share one field, so the second call threw the
            // first away and the difference it was hiding failed the test with no hint why.
            var person = TestHelpers.CreateValidPerson();

            var expected = person with
            {
                Name = "WrongName",
                FirstName = "WrongFirstName"
            };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
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
            // Ignoring Name must not also swallow FirstName. Matching the path segment by "contains"
            // instead of by equality would do exactly that, and every later regression in FirstName
            // would go green.
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { FirstName = "WrongFirstName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                    .Accepts(person)
                                                                                    .Produces<Person>(HttpStatusCode.Created)
                                                                                    .ExpectedResponse(expected)
                                                                                    .IgnoreProperty<Person>(p => p.Name)
                                                                                    .ExecuteAsync(),
                                                                        because: "IgnoreProperty may hide exactly the property it names and nothing else. 'firstName' merely ENDS WITH 'name', so a substring match would drop it too and switch off a real part of the comparison.",
                                                                        fix: "Check that the member-path match compares the last path segment for equality instead of using Contains/EndsWith.").ConfigureAwait(false);
        }

        // ============================================================
        // Status-code overloads — Produces<T>(int) and body-less Produces(int) / Produces(HttpStatusCode).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public async Task Fluent_Produces_Typed_Int_Overload_Should_Create_Person()
        {
            var person = TestHelpers.CreateValidPerson();

            // IHttpRequestConfiguring.Produces<T>(int) — status as int (e.g. StatusCodes.Status201Created).
            var result = await Client.AssertPost("api/v1/persons")
                                     .Accepts(person)
                                     .Produces<Person>(201)
                                     .ExecuteAsync().ConfigureAwait(false);

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

            // IHttpRequestConfiguring.Produces(HttpStatusCode) — status-only expectation, no body compare.
            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces(HttpStatusCode.Created)
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_Produces_BodyLess_Int_Overload_Should_Assert_Status_Only()
        {
            var person = TestHelpers.CreateValidPerson();

            // IHttpRequestConfiguring.Produces(int) — status-only expectation via the int overload.
            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces(201)
                         .ExecuteAsync();
        }

        // ============================================================
        // Comparison config — FilterResponse and WriteSnapshot (IgnoreDifferences / IgnoreProperty above).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_FilterResponse_Should_Normalize_Before_Comparison()
        {
            // The endpoint echoes the posted emails back in send order (alf, abc). The expected object
            // is given in the OPPOSITE order (abc, alf); FilterResponse re-sorts it descending by address
            // back to (alf, abc) so it lines up. Without the hook the two orders wouldn't match — which
            // is exactly what proves FilterResponse runs.
            var person = TestHelpers.CreatePersonWithEmails();

            var expected = person with { Emails = person.Emails.OrderBy(e => e.EmailAddress).ToImmutableList() };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponse(expected)
                         .FilterResponse(p => p is null
                                                  ? null
                                                  : p with { Emails = p.Emails.OrderByDescending(e => e.EmailAddress).ToImmutableList() })
                         .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        public Task Fluent_WriteSnapshot_Disabled_Should_Compare_Normally()
        {
            var person = TestHelpers.CreateValidPerson();

            // IHttpComparisonConfiguring.WriteSnapshot(false) — snapshot writing explicitly OFF, so the
            // golden file is compared (not overwritten). Exercises the builder method without touching disk.
            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
                         .ExpectedResponseFromEmbeddedJson("CreatePerson.json")
                         .WriteSnapshot(false)
                         .ExecuteAsync();
        }

        // ============================================================
        // Per-difference predicate — DifferenceFilter (keep only differences the predicate accepts).
        // ============================================================

        [TestMethod]
        [TestCategory("Fluent")]
        [TestCategory("POST")]
        [TestCategory("DifferenceFilter")]
        public Task Fluent_DifferenceFilter_Should_Ignore_Filtered_Difference()
        {
            // The endpoint echoes the posted person back. The expected object has a deliberately wrong
            // Age (99 vs 42); DifferenceFilter keeps only NON-age differences, so the age diff is dropped
            // and the assertion passes.
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { Age = 99 };

            return Client.AssertPost("api/v1/persons")
                         .Accepts(person)
                         .Produces<Person>(HttpStatusCode.Created)
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
            // Name is wrong; the filter only drops age differences, so the name difference must still fail.
            var person = TestHelpers.CreateValidPerson();
            var expected = person with { Name = "WrongName" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost("api/v1/persons")
                                                                                    .Accepts(person)
                                                                                    .Produces<Person>(HttpStatusCode.Created)
                                                                                    .ExpectedResponse(expected)
                                                                                    .DifferenceFilter(d => !d.MemberPath.Contains("age", StringComparison.OrdinalIgnoreCase))
                                                                                    .ExecuteAsync(),
                                                                        because: "A per-assert differenceFilter may hide exactly what it names and nothing else. The unrelated difference in this test has to keep failing - otherwise a single filter would quietly switch off the whole comparison and every later regression would go green.",
                                                                        fix: "Check that the filter predicate is evaluated per difference and only drops the ones it matches, instead of skipping the comparison as soon as a differenceFilter is present.").ConfigureAwait(false);
        }
    }
}