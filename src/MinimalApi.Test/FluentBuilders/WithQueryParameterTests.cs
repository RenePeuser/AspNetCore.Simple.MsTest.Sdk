using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.FluentBuilders
{
    /// <summary>
    /// Consumer tests for <c>WithQueryParameter(key, value)</c>: the query string is built from parts and encoded by
    /// the SDK instead of by hand in the url. A hand-written <c>?name=Son Goku &amp; Co</c> silently sends
    /// <c>name=Son Goku </c> plus a stray key <c> Co</c> - the test reads one value and the endpoint gets another.
    ///
    /// <para>
    /// <c>persons/echo-query</c> sends the query back raw (every escape visible) and decoded (what the endpoint
    /// binds), so each test proves both what left the client and what arrived. Every positive case is paired with
    /// a test that MUST fail.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("Builder")]
    public sealed class WithQueryParameterTests : ApiTestBase
    {
        private const string EchoQueryUrl = "api/v1/persons/echo-query";

        private const string Reserved = "Son Goku & Co+1/2?=#%";

        private const string ReservedEncoded = "Son%20Goku%20%26%20Co%2B1%2F2%3F%3D%23%25";

        private static readonly string[] TagsAB = ["a", "b"];

        private const string EncodingFix = "Check HttpRequestBuilder.RequestUrl: key and value go through Uri.EscapeDataString and the composed url is what Produces… hands to the engine.";

        private static EchoQueryResponse Echo(string rawQuery,
                                              params (string Key, string[] Values)[] values)
        {
            var decoded = new Dictionary<string, string[]>();

            foreach (var (key, keyValues) in values)
            {
                decoded[key] = keyValues;
            }

            return new EchoQueryResponse(rawQuery, decoded);
        }

        // ============================================================
        // Encoding.
        // ============================================================

        [TestMethod]
        public Task Reserved_Characters_Should_Be_Encoded_And_Arrive_Unchanged()
        {
            return Client.AssertGet(EchoQueryUrl)
                         .WithQueryParameter("name", Reserved)
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?name=" + ReservedEncoded, ("name", [Reserved])))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Hand_Written_Query_With_Reserved_Characters_Should_Not_Arrive_Unchanged()
        {
            // Why the method exists: the same value written into the url by hand is split at '&', '+' turns into a
            // space and '#' cuts the rest off - the endpoint never sees what the test reads.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl + "?name=" + Reserved)
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?name=" + ReservedEncoded, ("name", [Reserved])))
                                                                                    .ExecuteAsync(),
                                                                        because: "An unencoded query is mangled on the way. If this passed, the echo endpoint would not measure the query and the encoding test next to it would prove nothing.",
                                                                        fix: EncodingFix).ConfigureAwait(false);
        }

        [TestMethod]
        public Task Key_Should_Be_Encoded_Too()
        {
            return Client.AssertGet(EchoQueryUrl)
                         .WithQueryParameter("filter[name]", "Goku")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?filter%5Bname%5D=Goku", ("filter[name]", ["Goku"])))
                         .ExecuteAsync();
        }

        // ============================================================
        // null, collections, existing query.
        // ============================================================

        [TestMethod]
        public Task Null_Should_Add_Nothing()
        {
            string? optionalName = null;

            return Client.AssertGet(EchoQueryUrl)
                         .WithQueryParameter("name", optionalName)
                         .WithQueryParameter("age", 42)
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?age=42", ("age", ["42"])))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Null_Expecting_An_Empty_Key_Should_Fail()
        {
            string? optionalName = null;

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl)
                                                                                    .WithQueryParameter("name", optionalName)
                                                                                    .WithQueryParameter("age", 42)
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?name=&age=42", ("name", [""]), ("age", ["42"])))
                                                                                    .ExecuteAsync(),
                                                                        because: "A null value must not be sent as an empty key - 'name=' binds to an empty string and an optional filter would filter for it.",
                                                                        fix: "Check that HttpRequestBuilder.AddQueryParameter returns the builder unchanged when the value is null.").ConfigureAwait(false);
        }

        [TestMethod]
        public Task Collection_Should_Repeat_The_Key()
        {
            return Client.AssertGet(EchoQueryUrl)
                         .WithQueryParameter("tag", new[] { "a b", null, "c&d" })
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?tag=a%20b&tag=c%26d", ("tag", ["a b", "c&d"])))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Collection_Expecting_Only_The_First_Element_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl)
                                                                                    .WithQueryParameter("tag", TagsAB)
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?tag=a", ("tag", ["a"])))
                                                                                    .ExecuteAsync(),
                                                                        because: "Every element of a collection is sent - expecting only the first has to fail.",
                                                                        fix: "Check that HttpRequestBuilder.QueryValues yields one entry per non-null element.").ConfigureAwait(false);
        }

        [TestMethod]
        public Task Url_With_Query_Should_Be_Continued_With_Ampersand()
        {
            return Client.AssertGet(EchoQueryUrl + "?fixed=1")
                         .WithQueryParameter("name", "Goku")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?fixed=1&name=Goku", ("fixed", ["1"]), ("name", ["Goku"])))
                         .ExecuteAsync();
        }

        // ============================================================
        // Culture-invariant formatting.
        // ============================================================

        [TestMethod]
        public async Task Values_Should_Be_Formatted_Culture_Invariant()
        {
            // The test host runs in globalization-invariant mode (no "de-DE"), so a German-style culture is built by
            // hand: decimal comma and day-first dates - what a current-culture ToString() would leak into the url.
            var german = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            german.NumberFormat.NumberDecimalSeparator = ",";
            german.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
            german.DateTimeFormat.DateSeparator = ".";

            var culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = german;

            try
            {
                await Client.AssertGet(EchoQueryUrl)
                            .WithQueryParameter("amount", 1.5m)
                            .WithQueryParameter("active", true)
                            .WithQueryParameter("status", HttpStatusCode.Created)
                            .WithQueryParameter("day", new DateOnly(2026, 10, 13))
                            .WithQueryParameter("at", new DateTimeOffset(2026, 10, 13, 8, 30, 0, TimeSpan.FromHours(2)))
                            .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                            .ExpectedResponse(Echo("?amount=1.5&active=true&status=Created&day=2026-10-13&at=2026-10-13T08%3A30%3A00.0000000%2B02%3A00",
                                                   ("amount", ["1.5"]),
                                                   ("active", ["true"]),
                                                   ("status", ["Created"]),
                                                   ("day", ["2026-10-13"]),
                                                   ("at", ["2026-10-13T08:30:00.0000000+02:00"])))
                            .ExecuteAsync()
                            .ConfigureAwait(false);
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
        }

        // ============================================================
        // Body stage — before and after Accepts…
        // ============================================================

        [TestMethod]
        public Task Body_Verb_Should_Send_Query_Before_And_After_Accepts()
        {
            return Client.AssertPost(EchoQueryUrl)
                         .WithQueryParameter("before", "a b")
                         .Accepts(new { Name = "Goku" })
                         .WithQueryParameter("after", "c&d")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?before=a%20b&after=c%26d", ("before", ["a b"]), ("after", ["c&d"])))
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Body_Verb_Expecting_No_Query_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertPost(EchoQueryUrl)
                                                                                    .WithQueryParameter("before", "a")
                                                                                    .Accepts(new { Name = "Goku" })
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo(string.Empty))
                                                                                    .ExecuteAsync(),
                                                                        because: "The query set on the body stage has to survive Accepts… - an empty query on the server means it was dropped.",
                                                                        fix: "Check that Accepts… copies the builder (With) and keeps _query.").ConfigureAwait(false);
        }

        // ============================================================
        // Immutability — a stored base request branches.
        // ============================================================

        [TestMethod]
        public async Task Branches_Of_A_Stored_Request_Should_Not_Share_Query()
        {
            var request = Client.AssertGet(EchoQueryUrl).WithQueryParameter("fixed", 1);

            await request.WithQueryParameter("name", "Goku")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?fixed=1&name=Goku", ("fixed", ["1"]), ("name", ["Goku"])))
                         .ExecuteAsync()
                         .ConfigureAwait(false);

            await request.WithQueryParameter("name", "Vegeta")
                         .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                         .ExpectedResponse(Echo("?fixed=1&name=Vegeta", ("fixed", ["1"]), ("name", ["Vegeta"])))
                         .ExecuteAsync()
                         .ConfigureAwait(false);
        }

        // ============================================================
        // Misuse — runtime values, so no type-state can prevent it.
        // ============================================================

        [TestMethod]
        public void Key_Set_Twice_In_Another_Casing_Should_Throw_With_Guidance()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoQueryUrl)
                                                                                     .WithQueryParameter("name", "Son")
                                                                                     .WithQueryParameter("NAME", "Vegeta"),
                                                                         because: "ASP.NET Core binds query keys case-insensitively and takes one value for a scalar - the second would be ignored silently, so the chain has to stop at the repeating line.",
                                                                         fix: "Check that HttpRequestBuilder.AddQueryParameter rejects a key the chain already holds (OrdinalIgnoreCase) with FluentUsageError.DuplicateQueryParameter.");

            foreach (var part in new[] { "QUERY PARAMETER SET TWICE", "\"Son\"", "\"Vegeta\"", "new[] { \"Son\", \"Vegeta\" }", "What Happened", "Why This Fails", "How To Fix" })
            {
                Assert.That.Contains(exception.Message,
                                     part,
                                     because: "The usage error has to name the key, both values and a concrete fix, in the sdk's usual layout.",
                                     fix: "Check FluentUsageError.DuplicateQueryParameter.");
            }
        }

        [TestMethod]
        public void Null_Before_A_Value_Should_Not_Count_As_Set()
        {
            // null adds nothing, so a later value for the same key is the first one actually sent.
            var request = Client.AssertGet(EchoQueryUrl)
                                .WithQueryParameter("name", null)
                                .WithQueryParameter("name", "Goku");

            Assert.That.IsNotNull(request,
                                  because: "A skipped null value is not a value - the key is still free.",
                                  fix: "Check that AddQueryParameter returns before the duplicate check when there is nothing to send.");
        }

        [TestMethod]
        public void Object_Value_Should_Throw()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(EchoQueryUrl)
                                                                     .WithQueryParameter("person", new { Name = "Goku" }),
                                                         because: "An object has no single query value - its ToString() would send the type name.",
                                                         fix: "Check that HttpRequestBuilder.FormatQueryValue rejects types IsValue does not know.");
        }
    }
}
