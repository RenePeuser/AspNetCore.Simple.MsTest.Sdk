using System;
using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ErrorHandling
{
    /// <summary>
    /// Which handler answers which failure. Getting this wrong is silent: the catch-all always
    /// produces SOMETHING, so a mis-registered specific handler shows up as "❌ UNEXPECTED TEST SDK
    /// ERROR ... this could indicate a bug in the Test SDK itself" plus a stack trace, for what is
    /// really a typo in a file name.
    ///
    /// Every test provokes the failure through a real assert and reads the message the author sees.
    /// The missing payload and the broken http snapshot are covered in SnapshotResolutionTests.
    /// </summary>
    [TestClass]
    [TestCategory("ErrorHandling")]
    public sealed class ErrorHandlerSelectionTests : ApiTestBase
    {
        private const string CatchAllHeading = "UNEXPECTED TEST SDK ERROR";

        [TestMethod]
        public void AMissingSnapshotMustBeAnsweredByTheSnapshotHandler()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual("Responses.MissingForHandlerSelection.json", JsonNode.Parse("{}")));

            Assert.That.Contains(output,
                                 "RESPONSE SNAPSHOT FILE NOT FOUND",
                                 because: "A missing snapshot has to reach SnapshotNotFoundErrorHandler, which names the missing file. That heading is what tells the author it is a typo in a file name, not a broken sdk.",
                                 fix: "Check that the specific handler is registered and that its CanHandle accepts SnapshotNotFoundException with isPayload false.");

            Assert.That.DoesNotContain(output,
                                       CatchAllHeading,
                                       because: "The catch-all always produces something, so a mis-registered specific handler does not fail loudly - it just prints this heading plus a stack trace and sends the author hunting for an sdk bug.",
                                       fix: "The specific handler has to be selected before DefaultErrorHandler; check the ordering in TestErrorHandlingStrategy.");
        }

        /// <summary>
        /// A bare file name never says which folder was searched. The resolved target does, and it is
        /// what separates a missing file from a reference pointing into the wrong folder.
        /// </summary>
        [TestMethod]
        public void AMissingSnapshotMustNameTheLocationItWasExpectedIn()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual("Responses.MissingForHandlerSelection.json", JsonNode.Parse("{}")));

            Assert.That.Contains(output,
                                 "Controllers.Test.ErrorHandling.Responses.MissingForHandlerSelection.json",
                                 because: "The reference on its own is a file name. Where the sdk looked for it is what turns 'not found' into an actionable path, and it is already known at the point the error is raised.",
                                 fix: "Check that SnapshotReferenceGuard passes the EmbeddedFileInfo into SnapshotNotFoundException and that the handler prints ExpectedResourceName.");
        }

        /// <summary>
        /// Recording is gated on a Debug build and the gate refuses without a word. So in a Release run
        /// the author follows the "pass writeResponse: true" advice, no file appears, no reason is given,
        /// and the identical error comes back - the advice itself becomes the loop.
        /// </summary>
        [TestMethod]
        public void AMissingSnapshotInANonDebugAssemblyMustSayThatRecordingCannotWork()
        {
            // Any assembly shipped in Release does - the framework's own is the one guaranteed to be there.
            var releaseAssembly = typeof(string).Assembly;

            var output = FailureOf(() => Assert.That.ObjectsAreEqual("Responses.MissingForHandlerSelection.json", JsonNode.Parse("{}"), releaseAssembly));

            Assert.That.Contains(output,
                                 "NOT compiled in Debug",
                                 because: "Without this line the output repeats the advice that just silently did nothing, and the author cannot tell a Release build from a broken sdk.",
                                 fix: "Check that SnapshotNotFoundException.CanRecord is taken from callingAssembly.IsCompiledInDebug() and that the handler branches on it.");
        }

        [TestMethod]
        public void BrokenSnapshotJsonMustBeAnsweredByTheSyntaxHandler()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual("Responses.BrokenJson.json", JsonNode.Parse("{}")));

            Assert.That.Contains(output,
                                 "IS NOT VALID JSON",
                                 because: "A syntax error in the snapshot is the author's own file, so the output has to point at the json and its position instead of at the sdk.",
                                 fix: "Check that InvalidSnapshotJsonErrorHandler is registered and that its CanHandle accepts InvalidSnapshotJsonException.");

            Assert.That.DoesNotContain(output,
                                       CatchAllHeading,
                                       because: "Falling through to the catch-all here would blame the sdk for a stray comma in a fixture.",
                                       fix: "The specific handler has to be selected before DefaultErrorHandler; check the ordering in TestErrorHandlingStrategy.");
        }

        [TestMethod]
        public void AnExceptionWithoutASpecificHandlerMustStillNameTheException()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual(new Throwing(null), new Throwing(new InvalidOperationException("something nobody planned for"))));

            Assert.That.Contains(output,
                                 "something nobody planned for",
                                 because: "For an exception nobody planned for there is no domain knowledge, so the original exception message is the only clue the author gets - swallowing it leaves nothing to act on.",
                                 fix: "Check that DefaultErrorHandler (or the object route's own fallback) writes exception.Message into its output instead of only the type name.");
        }

        /// <summary>
        /// NonSeekableBodyErrorHandler narrows NotSupportedException down to a rewind attempt. An
        /// ordinary NotSupportedException must not be dressed up as a buffering problem.
        /// </summary>
        [TestMethod]
        public void AnUnrelatedNotSupportedExceptionMustNotBeTakenForARewoundBody()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual(new Throwing(null), new Throwing(new NotSupportedException("this operation is not supported"))));

            Assert.That.DoesNotContain(output,
                                       "READ TWICE WITHOUT BUFFERING",
                                       because: "Claiming a buffering problem for an unrelated NotSupportedException sends the author off to fix something that is not broken.",
                                       fix: "Make NonSeekableBodyErrorHandler.CanHandle check the exception's origin or message for the rewind case instead of accepting every NotSupportedException.");

            Assert.That.Contains(output,
                                 "this operation is not supported",
                                 because: "With no specific handler willing to take it, the exception has to land on the catch-all - which at least passes its message on.",
                                 fix: "Check that declining the exception in NonSeekableBodyErrorHandler really lets the selection continue.");
        }

        // ============================================================
        // Own handlers - the public extension point.
        // ============================================================

        /// <summary>
        /// The catch-all accepts every exception. A consumer registers its own handler after
        /// AddAssertableHttpClient - if position decided, the catch-all would win and the own handler
        /// would never be reached.
        /// </summary>
        [TestMethod]
        public void AnOwnHandlerMustWinOverTheCatchAllEvenWhenRegisteredAfterIt()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual(new Throwing(null), new Throwing(new DomainRuleViolatedException("age must not be negative"))));

            Assert.That.Contains(output,
                                 $"{DomainRuleErrorHandler.Heading}: age must not be negative",
                                 because: "An own ITestErrorHandler is the public way to explain failures only the project understands. It has to be asked before the catch-all, no matter in which order it was registered.",
                                 fix: "Check TestErrorHandlingStrategy.HandleAsync - DefaultErrorHandler has to be ordered last among the compatible handlers.");

            Assert.That.DoesNotContain(output,
                                       CatchAllHeading,
                                       because: "Reaching the catch-all means the own handler was skipped.",
                                       fix: "See above - order the catch-all last.");
        }

        /// <summary>
        /// A handler that cannot serve the given context answers with an empty string. That must hand
        /// the exception to the next compatible handler - not drop straight to the bare fallback.
        /// </summary>
        [TestMethod]
        public void AHandlerThatHasNothingToSayMustNotBlockTheNextOne()
        {
            var output = FailureOf(() => Assert.That.ObjectsAreEqual(new Throwing(null), new Throwing(new NothingToSayException("passed along"))));

            Assert.That.Contains(output,
                                 "passed along",
                                 because: "SilentErrorHandler claims the exception but returns an empty string. The next compatible handler has to take over and pass the message on.",
                                 fix: "Check TestErrorHandlingStrategy.HandleAsync - an empty result has to count as declined and the loop has to continue.");

            Assert.That.DoesNotContain(output,
                                       "UNHANDLED EXCEPTION",
                                       because: "The bare fallback is only for the case where no handler produced anything. Reaching it here would mean a perfectly capable handler was skipped.",
                                       fix: "The fallback in TestErrorHandlingStrategy has to run only after every registered handler declined - not after the first empty result.");
        }

        private static string FailureOf(Action assert)
        {
            return Assert.That.ThrowsExactly<AssertFailedException>(assert,
                                                                    because: "The provoked failure has to stop the test - a passing assert would leave nothing to inspect.",
                                                                    fix: "Check the fixture of this test.")
                         .Message;
        }

        /// <summary>Throws the given exception the moment the sdk serializes it.</summary>
        private sealed class Throwing(Exception? exception)
        {
            public string Value => exception is null ? "fine" : throw exception;
        }
    }
}
