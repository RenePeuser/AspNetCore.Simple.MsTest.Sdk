using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;
using MinimalApi.Test.Api.Persons.V1.Shared;

namespace MinimalApi.Test.Api.Persons.V1.Get
{
    /// <summary>
    /// Native API tests for GET /api/v1/persons endpoint.
    /// Uses the classic AssertGetAsync extension methods.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    public partial class PersonGetTests : ApiTestBase
    {
        // ============================================================
        // NATIVE API TESTS
        // ============================================================

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Filtering()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              TestHelpers.OrderByIdFilter);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_Ignore_Id()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              TestHelpers.IgnoreIdDifferences);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Status_Code()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetAllPersons.json",
                                                              expectedHttpStatusCode: System.Net.HttpStatusCode.OK);
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_Persons_By_Query_Parameter()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons?name=Son",
                                                              "GetPersonByQuery.json");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public Task Native_Should_Get_All_Persons_With_Inline_Json()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              /*lang=json,strict*/
                                                              "{\"content\":{\"headers\":[{\"key\":\"Content-Type\",\"value\":[\"application/json; charset=utf-8\"]}],\"value\":[{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]},{\"id\":2,\"name\":\"Vegeta\",\"firstName\":\"Unknown\",\"age\":77,\"emails\":[{\"emailAddress\":\"abc@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"maxmustermann@hotmail.de\",\"type\":\"Microsoft\"}]}]},\"statusCode\":\"OK\",\"headers\":[],\"trailingHeaders\":[],\"isSuccessStatusCode\":true}");
        }

        /// <summary>
        /// Used to compare the whole rendered error block against a text snapshot, which made it fail
        /// in CI for reasons that had nothing to do with the endpoint: the block embeds the assert
        /// source code, its line number and box drawing, so reformatting this file was enough to break
        /// it. What the test is actually about is that a wrong response type is reported as a type
        /// mismatch and names the type the endpoint really returns - so that is what it asserts now.
        /// </summary>
        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        public async Task Native_Should_Throw_When_Response_Type_Invalid()
        {
            var error = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<UnknownResponse>("api/v1/persons",
                                                                                                                                 "GetAllPersons.json"),
                                                                                    because: "The test declares a type the endpoint never returns. Endpoint validation has to catch that up front instead of letting the call go out and fail later on a deserialization error nobody can read.",
                                                                                    fix: "Check that endpoint validation compares the declared type against the endpoint's real return type before the request is built.")
                                    .ConfigureAwait(false);

            Assert.That.Contains(error.Message,
                                 "HTTP RESPONSE TYPE MISMATCH",
                                 because: "The heading has to name the real problem - a wrong type argument in the test - instead of surfacing as a json or serialization error further downstream.",
                                 fix: "Check that endpoint validation raises the response-type mismatch and that its handler is selected before the generic ones.");

            // The declared type and the one the endpoint returns - without both the message is useless.
            Assert.That.Contains(error.Message,
                                 nameof(UnknownResponse),
                                 because: "Naming the declared type tells the author which of possibly several asserts in the file is the wrong one.",
                                 fix: "Check that the validation output prints the type argument that was passed in.");

            Assert.That.Contains(error.Message,
                                 "IEnumerable<Person>",
                                 because: "The type the endpoint really returns is the answer to the question the failure raises. Without it the author has to go read the controller - and the readable C# form matters, the CLR name is not something you can paste into the test.",
                                 fix: "Check that the validation output renders the endpoint's return type through TypeNameFormatter and that it unwraps Task<T> first.");

            // And it has to say what to do about it.
            Assert.That.Contains(error.Message,
                                 "Suggested Fix",
                                 because: "This is the whole point of the AI-friendly output: naming the problem is not enough, the message has to say what to change.",
                                 fix: "Check that the handler renders the Suggested Fix section - see AssertOutputHelper.BuildFixSection.");

            // No request may go out - the mismatch is caught by endpoint validation beforehand.
            Assert.That.Contains(error.Message,
                                 "No HTTP call was made",
                                 because: "Validation runs before the request, so the author has to be told the endpoint was never touched - otherwise they will go looking for a server-side cause of a purely local mistake.",
                                 fix: "Check that endpoint validation runs before the http call and that its output says so explicitly.");
        }

        [TestMethod]
        [TestCategory("Native")]
        [TestCategory("GET")]
        [DataRow("{dsdmsd")]
        [DataRow("[invalid")]
        [DataRow("{incomplete")]
        public async Task Native_Should_Throw_When_Json_Malformed(string invalidJson)
        {
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", invalidJson),
                                                                                        because: $"Inline json that does not parse ({invalidJson}) has to stop the test. Treating it as an unresolvable snapshot reference and carrying on would compare against nothing.",
                                                                                        fix: "Check that the inline-json path reports a parse error instead of falling through to the snapshot lookup.")
                                        .ConfigureAwait(false);

            // Just verify an exception was thrown with invalid JSON
            Assert.That.IsNotNull(exception,
                                  because: "ThrowsExactlyAsync hands the caught exception back so the message can be inspected - a null here would mean the assert helper itself lost it.",
                                  fix: "Check the return value of Assert.That.ThrowsExactlyAsync.");
        }
    }

    /// <summary>
    /// Test helper class for invalid response type testing.
    /// </summary>
    internal sealed record UnknownResponse
    {
        public required string Property { get; init; }
    }
}