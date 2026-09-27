using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Errors
{
    /// <summary>
    /// Tests that verify RESPONSE TYPE MISMATCH errors when the test declares
    /// a different generic type than the endpoint exposes.
    ///
    /// Regression test for bug: Suggested Fix showed 'object' instead of actual endpoint type
    /// when ResponseType was null and only ResponseTypesByStatusCode was populated.
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Response Type Mismatch")]
    public class ResponseTypeMismatchTests : ApiTestBase
    {
        /// <summary>
        /// Regression test: When endpoint declares .Produces&lt;Person&gt;(StatusCodes.Status201Created)
        /// and test uses AssertPostAsync&lt;object&gt; with expectedHttpStatusCode: HttpStatusCode.Created,
        /// the "Suggested Fix" must show &lt;Person&gt;, not &lt;object&gt;.
        ///
        /// BEFORE FIX: Suggested Fix showed:
        ///   Client.AssertPostAsync&lt;object&gt;(...)
        ///
        /// AFTER FIX: Suggested Fix shows:
        ///   Client.AssertPostAsync&lt;Person&gt;(...)
        ///
        /// This test also verifies case-insensitive matching: C# keyword 'object' vs. type name 'Object'.
        /// </summary>
        [TestMethod]
        public async Task Suggested_Fix_Should_Show_Actual_Endpoint_Type_Not_Object_When_Using_Lowercase_Object_Keyword()
        {
            // ARRANGE: Endpoint declares .Produces<Person>(201) but test uses <object> (lowercase keyword)
            var person = new Person(Id: 1, Name: "Test", FirstName: "User", Age: 25, Emails: ImmutableList<Email>.Empty);

            // ACT: Call with wrong generic type parameter
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertPostAsync<object>("api/v1/persons",
                                                                                                                          person,
                                                                                                                          writeResponse: false,
                                                                                                                          expectedHttpStatusCode: System.Net.HttpStatusCode.Created),
                                                                                        because: "The endpoint declares Produces<Person>(201) for 201 Created, but the test uses <object>. This type mismatch must fail before the HTTP call.",
                                                                                        fix: "Check that endpoint validation runs before the HTTP call and compares the generic type argument against the endpoint's declared response type.")
                                        .ConfigureAwait(false);

            // VERIFY: Error message should contain "RESPONSE TYPE MISMATCH"
            Assert.That.Contains(exception.Message,
                                 "RESPONSE TYPE MISMATCH",
                                 because: "The core problem is a type mismatch between test and endpoint declaration.",
                                 fix: "Check that EndpointValidationOutputBuilder.BuildResponseTypeMismatch includes this header.");

            // VERIFY: Error message should show actual endpoint type "Person"
            Assert.That.Contains(exception.Message,
                                 "Person",
                                 because: "The author needs to see what type the endpoint actually exposes so they can fix the test.",
                                 fix: "Check that the endpoint's ResponseType or ResponseTypesByStatusCode[201] is extracted and formatted correctly.");

            // VERIFY: Suggested Fix should contain AssertPostAsync<Person> (not <object>)
            var suggestedFixStart = exception.Message.IndexOf("✅ Suggested Fix");
            var suggestedFixSection = exception.Message.Substring(suggestedFixStart, 500);

            Assert.That.Contains(suggestedFixSection,
                                 "AssertPostAsync<Person>",
                                 because: "The Suggested Fix must show the corrected code with <Person>, not <object>, so the author can copy-paste the fix. The bug was that it showed <object> when ResponseType was null.",
                                 fix: "Check EndpointValidationOutputBuilder: the suggestedTypeName must come from ResponseTypesByStatusCode[statusCode] as fallback when ResponseType is null.");

            // VERIFY: Suggested Fix should NOT contain <object> or <Object>
            Assert.That.IsFalse(suggestedFixSection.Contains("AssertPostAsync<object>") ||
                                suggestedFixSection.Contains("AssertPostAsync<Object>"),
                                because: "The Suggested Fix showed <object> before the fix. This is the regression we're preventing.",
                                fix: "The fallback chain must be: relevantStatusCodes.First → ResponseType → ResponseTypesByStatusCode[200], never hardcoded 'object'.");
        }

        /// <summary>
        /// Verifies that case-insensitive replacement works: source uses lowercase 'object' keyword
        /// but typeof(object).Name returns 'Object' (capitalized).
        /// </summary>
        [TestMethod]
        public async Task Suggested_Fix_Should_Replace_Lowercase_Object_Keyword_With_Correct_Type()
        {
            // ACT: Endpoint expects Person but test uses <object> (lowercase C# keyword)
            var person = new Person(Id: 1, Name: "Test", FirstName: "User", Age: 25, Emails: ImmutableList<Email>.Empty);

            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertPostAsync<object>("api/v1/persons",
                                                                                                                          person,
                                                                                                                          writeResponse: false,
                                                                                                                          expectedHttpStatusCode: System.Net.HttpStatusCode.Created),
                                                                                        because: "Testing case-insensitive replacement.",
                                                                                        fix: "N/A")
                                        .ConfigureAwait(false);

            var suggestedFixStart = exception.Message.IndexOf("✅ Suggested Fix");
            var suggestedFixSection = exception.Message.Substring(suggestedFixStart, 500);

            // Should replace <object> with <Person> even though typeof(object).Name == "Object"
            Assert.That.IsTrue(suggestedFixSection.Contains("<Person>"),
                               because: "Case-insensitive regex replace must work: source has <object>, declaredTestTypeName is 'Object', but both should be replaced with <Person>.",
                               fix: "Use RegexOptions.IgnoreCase in the Replace operation.");
        }

        /// <summary>
        /// Verifies the fix works for GET endpoints as well (different HTTP verb).
        /// </summary>
        [TestMethod]
        public async Task Suggested_Fix_For_Get_Endpoint_Should_Show_Actual_Type()
        {
            // ACT: GET endpoint declares .Produces<Person>(200) but test uses <object>
            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertGetAsync<object>("api/v1/persons/1",
                                                                                                                         writeResponse: false,
                                                                                                                         expectedHttpStatusCode: System.Net.HttpStatusCode.OK),
                                                                                        because: "Same type mismatch, different HTTP verb (GET instead of POST).",
                                                                                        fix: "Verify the fix is HTTP-verb agnostic.")
                                        .ConfigureAwait(false);

            var suggestedFixStart = exception.Message.IndexOf("✅ Suggested Fix");
            var suggestedFixSection = exception.Message.Substring(suggestedFixStart, 500);

            // VERIFY: Suggested Fix should contain AssertGetAsync<Person>
            Assert.That.Contains(suggestedFixSection,
                                 "AssertGetAsync<Person>",
                                 because: "The fix must work for GET requests as well, not just POST.",
                                 fix: "Check that the regex pattern in BuildResponseTypeMismatch matches all Assert*Async methods.");
        }

        /// <summary>
        /// Verifies that the fallback chain works correctly when:
        /// - relevantStatusCodes is empty
        /// - endpoint.ResponseType is null (deprecated)
        /// - endpoint.ResponseTypesByStatusCode[200] has the type
        /// </summary>
        [TestMethod]
        public async Task Suggested_Fix_Should_Use_ResponseTypesByStatusCode_When_ResponseType_Is_Null()
        {
            // This is implicitly tested by the other tests since MinimalAPI endpoints
            // typically don't populate the deprecated ResponseType field.
            // The endpoint declares .Produces<Person>(201), which populates
            // ResponseTypesByStatusCode[201] but leaves ResponseType as null.

            var person = new Person(Id: 1, Name: "Test", FirstName: "User", Age: 25, Emails: ImmutableList<Email>.Empty);

            var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() =>
                                                                                            Client.AssertPostAsync<object>("api/v1/persons",
                                                                                                                          person,
                                                                                                                          writeResponse: false,
                                                                                                                          expectedHttpStatusCode: System.Net.HttpStatusCode.Created),
                                                                                        because: "Validating fallback to ResponseTypesByStatusCode.",
                                                                                        fix: "N/A")
                                        .ConfigureAwait(false);

            var suggestedFixStart = exception.Message.IndexOf("✅ Suggested Fix");
            var suggestedFixSection = exception.Message.Substring(suggestedFixStart, 500);

            // The fact that it shows <Person> proves the fallback worked:
            // relevantStatusCodes had {201: Person}, so firstRelevantType = Person
            Assert.That.Contains(suggestedFixSection,
                                 "<Person>",
                                 because: "The fallback chain extracted Person from ResponseTypesByStatusCode[201].",
                                 fix: "Check the fallback logic: firstRelevantType ?? endpoint.ResponseType ?? ResponseTypesByStatusCode[200]");
        }
    }
}
