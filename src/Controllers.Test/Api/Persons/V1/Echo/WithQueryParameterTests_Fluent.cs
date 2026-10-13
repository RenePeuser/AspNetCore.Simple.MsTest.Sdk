using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.Persons.V1.Echo
{
    /// <summary>
    /// Controller twin of <c>MinimalApi.Test.FluentBuilders.WithQueryParameterTests</c>: <c>WithQueryParameter</c>
    /// against controller actions - <c>[AcceptVerbs]</c> routing next to <c>persons/{id}</c>, <c>[ProducesResponseType]</c>
    /// validation and <c>[FromQuery]</c> model binding.
    ///
    /// <para>
    /// <c>persons/echo-query</c> sends the query back raw (every escape visible) and decoded. Every positive case is
    /// paired with a test that MUST fail.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Controllers")]
    [TestCategory("Fluent")]
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
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet(EchoQueryUrl + "?name=" + Reserved)
                                                                                    .Produces<EchoQueryResponse>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(Echo("?name=" + ReservedEncoded, ("name", [Reserved])))
                                                                                    .ExecuteAsync(),
                                                                        because: "An unencoded query is mangled on the way. If this passed, the echo action would not measure the query and the encoding test next to it would prove nothing.",
                                                                        fix: EncodingFix).ConfigureAwait(false);
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
        // [FromQuery] model binding - the controller filters by the bound value.
        // ============================================================

        [TestMethod]
        public Task Bound_Query_Value_Should_Filter_The_Persons()
        {
            return Client.AssertGet("api/v1/persons")
                         .WithQueryParameter("name", "Veg")
                         .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                         .ExpectedResponse(new[] { Vegeta() })
                         .ExecuteAsync();
        }

        [TestMethod]
        public async Task Bound_Query_Value_Expecting_Every_Person_Should_Fail()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGet("api/v1/persons")
                                                                                    .WithQueryParameter("name", "Veg")
                                                                                    .Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                                                                                    .ExpectedResponse(new[] { Goku(), Vegeta() })
                                                                                    .ExecuteAsync(),
                                                                        because: "[FromQuery] name binds 'Veg' and the action returns Vegeta only - if both persons came back, the query never reached model binding.",
                                                                        fix: EncodingFix).ConfigureAwait(false);
        }

        private static Person Goku()
        {
            return new Person(1, "Son", "Goku", 99, ImmutableList.Create(new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft")));
        }

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }
    }
}
