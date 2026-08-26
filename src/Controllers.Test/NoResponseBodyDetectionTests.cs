using System.Collections.Generic;
using System.Threading.Tasks;
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
        [TestMethod]
        public void ShouldUnwrapGenericTaskAndValueTask()
        {
            Assert.AreEqual(typeof(string), EndpointParsingHelpers.UnwrapTaskType(typeof(Task<string>)));
            Assert.AreEqual(typeof(string), EndpointParsingHelpers.UnwrapTaskType(typeof(ValueTask<string>)));
            Assert.AreEqual(typeof(IEnumerable<int>), EndpointParsingHelpers.UnwrapTaskType(typeof(Task<IEnumerable<int>>)));
        }

        [TestMethod]
        public void ShouldLeaveNonAwaitableTypesUntouched()
        {
            Assert.AreEqual(typeof(string), EndpointParsingHelpers.UnwrapTaskType(typeof(string)));
        }

        [TestMethod]
        public void ShouldRecogniseReturnTypesThatCarryNoBody()
        {
            Assert.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(Task)));
            Assert.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(ValueTask)));
            Assert.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(void)));
            Assert.IsTrue(EndpointParsingHelpers.ReturnsNoResponseBody(null));
        }

        [TestMethod]
        public void ShouldNotTreatARealPayloadTypeAsBodyless()
        {
            Assert.IsFalse(EndpointParsingHelpers.ReturnsNoResponseBody(typeof(string)));

            // Task<T> must be unwrapped first - the unwrapped T is a real body.
            Assert.IsFalse(EndpointParsingHelpers.ReturnsNoResponseBody(EndpointParsingHelpers.UnwrapTaskType(typeof(Task<string>))));
        }
    }
}
