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
    /// Consumer tests for <c>WithParameters(object)</c> and <c>WithParameters(dictionary)</c> against
    /// GET /api/v1/persons?name=$Name$ - the response proves which value replaced the placeholder.
    ///
    /// <para>
    /// <c>WithParameters(object)</c> used to reflect over EVERY public property of whatever it got: a string
    /// crashed on its <c>Chars</c> indexer, a dictionary produced <c>$Count$</c>/<c>$Keys$</c> and crashed on
    /// <c>Item</c>, and an int silently added nothing - leaving <c>$Name$</c> unreplaced.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    [TestCategory("WithParameters")]
    public sealed class WithParametersTests : ApiTestBase
    {
        private const string QueryByNameUrl = "api/v1/persons?name=$Name$";

        private const string ValueFix = "Check that WithParameters(object) rejects values and collections with an ArgumentException before reflecting over their properties.";

        private static Person Son()
        {
            return new Person(1, "Son", "Goku", 99, ImmutableList.Create(new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft")));
        }

        private static Person Vegeta()
        {
            return new Person(2, "Vegeta", "Unknown", 77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")));
        }

        private static Task<IEnumerable<Person>> QueryExpecting(IHttpRequestConfiguring request,
                                                                Person expected)
        {
            return request.Produces<IEnumerable<Person>>(HttpStatusCode.OK)
                          .ExpectedResponse([expected])
                          .ExecuteAsync();
        }

        /// <summary>A parameter object that also carries an indexer - the indexer must be skipped, not invoked.</summary>
        private sealed class NameWithIndexer
        {
            public string Name { get; init; } = string.Empty;

            public string this[int index] => Name;
        }

        // ============================================================
        // Objects — public properties become placeholders.
        // ============================================================

        [TestMethod]
        public Task Anonymous_Object_Should_Replace_Placeholder()
        {
            return QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(new { Name = "Vegeta" }), Vegeta());
        }

        [TestMethod]
        public async Task Anonymous_Object_Should_Send_Its_Own_Value()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(new { Name = "Vegeta" }), Son()),
                                                                        because: "Name=Vegeta returns Vegeta - expecting Son has to fail, or the parameter never reached the url.",
                                                                        fix: "Check that WithParameters(object) maps each property to its $Name$ placeholder.").ConfigureAwait(false);
        }

        [TestMethod]
        public Task Object_With_Indexer_Should_Skip_The_Indexer()
        {
            return QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(new NameWithIndexer { Name = "Vegeta" }), Vegeta());
        }

        // ============================================================
        // Dictionaries — entries become placeholders.
        // ============================================================

        [TestMethod]
        public Task Object_Dictionary_Should_Replace_Placeholder()
        {
            var parameters = new Dictionary<string, object?> { ["Name"] = "Vegeta" };

            return QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(parameters), Vegeta());
        }

        [TestMethod]
        public async Task Object_Dictionary_Should_Send_Its_Own_Value()
        {
            var parameters = new Dictionary<string, object?> { ["Name"] = "Vegeta" };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(parameters), Son()),
                                                                        because: "The dictionary entry Name=Vegeta returns Vegeta - expecting Son has to fail.",
                                                                        fix: "Check that WithParameters(IReadOnlyDictionary) maps each entry to its $Key$ placeholder.").ConfigureAwait(false);
        }

        [TestMethod]
        public Task String_Dictionary_Should_Replace_Placeholder()
        {
            // Dictionary<string, string> does not convert to IReadOnlyDictionary<string, object?> and binds to
            // WithParameters(object) - its entries must still be used, not its Count/Keys properties.
            var parameters = new Dictionary<string, string> { ["Name"] = "Vegeta" };

            return QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(parameters), Vegeta());
        }

        [TestMethod]
        public async Task String_Dictionary_Should_Send_Its_Own_Value()
        {
            var parameters = ImmutableDictionary<string, string>.Empty.Add("Name", "Vegeta");

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => QueryExpecting(Client.AssertGet(QueryByNameUrl).WithParameters(parameters), Son()),
                                                                        because: "The dictionary entry Name=Vegeta returns Vegeta - expecting Son has to fail.",
                                                                        fix: "Check that WithParameters(object) recognizes IDictionary and maps its entries.").ConfigureAwait(false);
        }

        // ============================================================
        // Values and collections — rejected with a clear message.
        // ============================================================

        [TestMethod]
        public void String_Should_Be_Rejected()
        {
            var exception = Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl).WithParameters("Vegeta"),
                                                                         because: "A string has no properties that name placeholders - reflecting over it crashed on the Chars indexer.",
                                                                         fix: ValueFix);

            Assert.That.Contains(exception.Message,
                                 "WithParameter(\"Name\", value)",
                                 "The message has to point to the method the caller most likely wanted.",
                                 ValueFix);
        }

        [TestMethod]
        public void Number_Should_Be_Rejected()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl).WithParameters(42),
                                                         because: "An int has no properties - accepting it silently adds nothing and leaves $Name$ unreplaced.",
                                                         fix: ValueFix);
        }

        [TestMethod]
        public void Guid_Should_Be_Rejected()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl).WithParameters(Guid.NewGuid()),
                                                         because: "A Guid is a value, not a parameter object.",
                                                         fix: ValueFix);
        }

        [TestMethod]
        public void List_Should_Be_Rejected()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Client.AssertGet(QueryByNameUrl).WithParameters(new List<string> { "Vegeta" }),
                                                         because: "A list would map $Capacity$/$Count$ - names no placeholder means.",
                                                         fix: ValueFix);
        }
    }
}
