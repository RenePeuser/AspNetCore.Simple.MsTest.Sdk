using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// A controller action declared as <c>Task DeleteAsync(...)</c> has no response body. Before this
    /// was detected, the validator reported "Actual Endpoint Type: Task" and suggested
    /// <c>AssertDeleteAsync&lt;Task&gt;(...)</c> - a fix that can never work, because there is nothing
    /// to deserialize into Task.
    ///
    /// Unwrapping <c>Task&lt;T&gt;</c> is exercised by every typed assert against an async action, e.g.
    /// GET api/v1/persons/{id}.
    /// </summary>
    [TestClass]
    [TestCategory("NoResponseBody")]
    public sealed class NoResponseBodyDetectionTests : ApiTestBase
    {
        private const string Url = "api/v1/sdk-scenarios/bodyless/1";

        [TestMethod]
        public Task AnAwaitableWithoutAResultMustBeAssertableWithoutABody()
        {
            return Client.AssertDeleteAsync(Url);
        }

        [TestMethod]
        public async Task ATypedAssertAgainstAnAwaitableWithoutAResultMustNotSuggestTask()
        {
            var error = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertDeleteAsync<Person>(Url, "Responses.Bodyless.json"),
                                                                                    because: "The endpoint has no response body, so a typed assert has to be stopped by endpoint validation.",
                                                                                    fix: "Check EndpointParsingHelpers.ReturnsNoResponseBody - it has to cover non-generic Task.")
                                    .ConfigureAwait(false);

            Assert.That.DoesNotContain(error.Message,
                                       "SNAPSHOT FILE NOT FOUND",
                                       because: "The failure has to come from endpoint validation. A missing fixture would fail the test too - and pass the check below for the wrong reason.",
                                       fix: "Restore Responses\\Bodyless.json in Controllers.Test.");

            Assert.That.DoesNotContain(error.Message,
                                       "<Task>",
                                       because: "Suggesting AssertDeleteAsync<Task>(...) can never work - there is nothing to deserialize into Task. The awaitable has to be recognised as 'no body'.",
                                       fix: "Check EndpointParsingHelpers.ReturnsNoResponseBody - it has to cover non-generic Task, non-generic ValueTask, void and a null type alike.");
        }
    }
}
