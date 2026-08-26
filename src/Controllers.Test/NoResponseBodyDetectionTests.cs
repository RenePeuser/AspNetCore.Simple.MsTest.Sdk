using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// A controller action declared as <c>Task DeleteAsync(...)</c> has no response body. Before this
    /// was detected, the validator reported "Actual Endpoint Type: Task" and suggested
    /// <c>AssertDeleteAsync&lt;Task&gt;(...)</c> - a fix that can never work, because there is nothing
    /// to deserialize into Task.
    /// </summary>
    [TestClass]
    [TestCategory("NoResponseBody")]
    public sealed class NoResponseBodyDetectionTests
    {
        private const string UnwrapBecause = "The validator compares the type a test declares against the endpoint's payload type. Without unwrapping the awaitable it would compare against Task<T> and suggest a fix that cannot work.";

        private const string UnwrapFix = "Check EndpointParsingHelpers.UnwrapTaskType - it has to take the single generic argument of Task<T> and ValueTask<T>.";

        private const string BodylessBecause = "These return types carry no response body at all. Reporting a payload type for them is what produced the impossible suggestion AssertDeleteAsync<Task>(...).";

        private const string BodylessFix = "Check EndpointParsingHelpers.ReturnsNoResponseBody - it has to cover non-generic Task, non-generic ValueTask, void and a null type alike.";

        [TestMethod]
        public void ShouldUnwrapGenericTaskAndValueTask()
        {
            Assert.That.AreEqual(typeof(string),
                                 EndpointParsingHelpers.UnwrapTaskType(typeof(Task<string>)),
                                 because: UnwrapBecause,
                                 fix: UnwrapFix);

            Assert.That.AreEqual(typeof(string),
                                 EndpointParsingHelpers.UnwrapTaskType(typeof(ValueTask<string>)),
                                 because: UnwrapBecause,
                                 fix: UnwrapFix);

            Assert.That.AreEqual(typeof(IEnumerable<int>),
                                 EndpointParsingHelpers.UnwrapTaskType(typeof(Task<IEnumerable<int>>)),
                                 because: $"{UnwrapBecause} A generic payload has to survive unwrapping intact - IEnumerable<int>, not int.",
                                 fix: $"{UnwrapFix} Unwrap exactly one level; do not recurse into the payload's own generic arguments.");
        }

        [TestMethod]
        public void ShouldLeaveNonAwaitableTypesUntouched()
        {
            Assert.That.AreEqual(typeof(string),
                                 EndpointParsingHelpers.UnwrapTaskType(typeof(string)),
                                 because: "A synchronous action returns its payload directly. Unwrapping something that is not awaitable would strip a real payload type away.",
                                 fix: "EndpointParsingHelpers.UnwrapTaskType has to return the type unchanged unless it really is Task<T> or ValueTask<T>.");
        }

        [TestMethod]
        public void ShouldRecogniseReturnTypesThatCarryNoBody()
        {
            Assert.That.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(Task)),
                               because: BodylessBecause,
                               fix: BodylessFix);

            Assert.That.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(ValueTask)),
                               because: BodylessBecause,
                               fix: BodylessFix);

            Assert.That.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(void)),
                               because: BodylessBecause,
                               fix: BodylessFix);

            Assert.That.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(null),
                               because: $"{BodylessBecause} A null type is what the parser hands over when it could not determine a return type - guessing a payload there is the worst case.",
                               fix: BodylessFix);
        }

        [TestMethod]
        public void ShouldNotTreatARealPayloadTypeAsBodyless()
        {
            Assert.That.IsFalse(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(string)),
                                because: "A real payload type must not be classified as bodyless, otherwise the validator would tell the author to drop the type argument from an assert that genuinely needs one.",
                                fix: "EndpointParsingHelpers.ReturnsNoResponseBody has to answer false for anything that is not Task/ValueTask/void/null.");

            // Task<T> must be unwrapped first - the unwrapped T is a real body.
            Assert.That.IsFalse(EndpointParsingHelpers.ReturnsNoResponseBody(EndpointParsingHelpers.UnwrapTaskType(typeof(Task<string>))),
                                because: "Task<string> unwraps to string, which is a real body. Checking the awaitable before unwrapping is exactly how a payload-carrying action gets misreported as bodyless.",
                                fix: "Make sure callers run UnwrapTaskType before ReturnsNoResponseBody - the order of the two is what decides the outcome here.");
        }
    }
}