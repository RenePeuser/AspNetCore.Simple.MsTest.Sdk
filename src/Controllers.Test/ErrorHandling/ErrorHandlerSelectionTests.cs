using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ErrorHandling
{
    /// <summary>
    /// Which handler answers which exception. Getting this wrong is silent: the catch-all always
    /// produces SOMETHING, so a mis-registered specific handler shows up as "❌ UNEXPECTED TEST SDK
    /// ERROR ... this could indicate a bug in the Test SDK itself" plus a stack trace, for what is
    /// really a typo in a file name.
    /// </summary>
    [TestClass]
    [TestCategory("ErrorHandling")]
    public sealed class ErrorHandlerSelectionTests
    {
        [TestMethod]
        public async Task AMissingSnapshotMustBeAnsweredByTheSnapshotHandler()
        {
            var exception = new SnapshotNotFoundException("Persons.json",
                                                          nameof(AMissingSnapshotMustBeAnsweredByTheSnapshotHandler),
                                                          typeof(ErrorHandlerSelectionTests).Assembly,
                                                          isPayload: false);

            var output = await HandleAsync(exception).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "RESPONSE SNAPSHOT FILE NOT FOUND",
                                 because: "A SnapshotNotFoundException has to reach SnapshotNotFoundErrorHandler, which names the missing file. That heading is what tells the author it is a typo in a file name, not a broken sdk.",
                                 fix: "Check that the specific handler is registered and that its CanHandle accepts SnapshotNotFoundException with isPayload false.");

            Assert.That.DoesNotContain(output,
                                       "UNEXPECTED TEST SDK ERROR",
                                       because: "The catch-all always produces something, so a mis-registered specific handler does not fail loudly - it just prints this heading plus a stack trace and sends the author hunting for an sdk bug.",
                                       fix: "The specific handler has to be selected before DefaultErrorHandler; check the registration order in AddTestErrorHandlingStrategy.");
        }

        [TestMethod]
        public async Task AMissingPayloadMustBeAnsweredAsAPayload()
        {
            var exception = new SnapshotNotFoundException("Request.json",
                                                          "payload",
                                                          typeof(ErrorHandlerSelectionTests).Assembly,
                                                          isPayload: true);

            var output = await HandleAsync(exception).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "REQUEST JSON FILE NOT FOUND",
                                 because: "The same exception type carries isPayload, and a missing request payload is a different mistake from a missing response snapshot - the wording has to say which one it is.",
                                 fix: "Check that the handler branches on SnapshotNotFoundException.IsPayload instead of always printing the snapshot wording.");
        }

        /// <summary>
        /// Recording is gated on a Debug build and the gate returns false without a word. So in a
        /// Release run the author follows the "pass writeResponse: true" advice, no file appears, no
        /// reason is given, and the identical error comes back - the advice itself becomes the loop.
        /// </summary>
        [TestMethod]
        public async Task AMissingSnapshotInANonDebugAssemblyMustSayThatRecordingCannotWork()
        {
            // Any assembly shipped in Release does - the framework's own is the one guaranteed to be there.
            var releaseAssembly = typeof(string).Assembly;

            var exception = new SnapshotNotFoundException("Persons.json",
                                                          nameof(AMissingSnapshotInANonDebugAssemblyMustSayThatRecordingCannotWork),
                                                          releaseAssembly,
                                                          isPayload: false);

            Assert.That.IsFalse(exception.CanRecord,
                                because: "The warning below only appears when recording is impossible. If CanRecord came out true for a Release assembly the warning would never be reachable, and a string match alone would not notice.",
                                fix: "Check that SnapshotNotFoundException.CanRecord is taken from callingAssembly.IsCompiledInDebug().");

            var output = await HandleAsync(exception).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "NOT compiled in Debug",
                                 because: "Without this line the output repeats the advice that just silently did nothing, and the author cannot tell a Release build from a broken sdk.",
                                 fix: "Check that the handler branches on SnapshotNotFoundException.CanRecord in the 'Creating A New Snapshot' section.");
        }

        /// <summary>
        /// A bare file name never says which folder was searched. The resolved target does, and it is
        /// what separates a missing file from a reference pointing into the wrong folder.
        /// </summary>
        [TestMethod]
        public async Task AMissingSnapshotMustNameTheLocationItWasExpectedIn()
        {
            var expected = new EmbeddedFileInfo("Controllers.Test.Api.Persons.V1.Get.Responses.Persons.json",
                                                "Persons.json",
                                                null,
                                                false,
                                                ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "Responses"));

            var exception = new SnapshotNotFoundException("Persons.json",
                                                          nameof(AMissingSnapshotMustNameTheLocationItWasExpectedIn),
                                                          typeof(ErrorHandlerSelectionTests).Assembly,
                                                          isPayload: false,
                                                          expected);

            var output = await HandleAsync(exception).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "Controllers.Test.Api.Persons.V1.Get.Responses.Persons.json",
                                 because: "The reference on its own is a file name. Where the sdk looked for it is what turns 'not found' into an actionable path, and it is already known at the point the error is raised.",
                                 fix: "Check that SnapshotReferenceGuard passes the EmbeddedFileInfo into SnapshotNotFoundException and that the handler prints ExpectedResourceName.");
        }

        [TestMethod]
        public async Task BrokenSnapshotJsonMustBeAnsweredByTheSyntaxHandler()
        {
            var output = await HandleAsync(BrokenJson()).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "IS NOT VALID JSON",
                                 because: "A syntax error in the snapshot is the author's own file, so the output has to point at the json and its position instead of at the sdk.",
                                 fix: "Check that InvalidSnapshotJsonErrorHandler is registered and that its CanHandle accepts InvalidSnapshotJsonException.");

            Assert.That.DoesNotContain(output,
                                       "UNEXPECTED TEST SDK ERROR",
                                       because: "Falling through to the catch-all here would blame the sdk for a stray comma in a fixture.",
                                       fix: "The specific handler has to be selected before DefaultErrorHandler; check the registration order in AddTestErrorHandlingStrategy.");
        }

        [TestMethod]
        public async Task AnExceptionWithoutASpecificHandlerMustFallBackToTheDefault()
        {
            var output = await HandleAsync(new InvalidOperationException("something nobody planned for")).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "UNEXPECTED TEST SDK ERROR",
                                 because: "For an exception nobody planned for the catch-all is the right answer - it exists so that no failure ever disappears without output.",
                                 fix: "Check that DefaultErrorHandler is registered at all and that its CanHandle returns true unconditionally.");

            Assert.That.Contains(output,
                                 "something nobody planned for",
                                 because: "The catch-all has no domain knowledge, so the original exception message is the only clue the author gets - swallowing it leaves nothing to act on.",
                                 fix: "Check that DefaultErrorHandler writes exception.Message into its output instead of only the type name.");
        }

        /// <summary>
        /// NonSeekableBodyErrorHandler narrows NotSupportedException down to a rewind attempt. An
        /// ordinary NotSupportedException must not be dressed up as a buffering problem.
        /// </summary>
        [TestMethod]
        public async Task AnUnrelatedNotSupportedExceptionMustNotBeTakenForARewoundBody()
        {
            var output = await HandleAsync(new NotSupportedException("this operation is not supported")).ConfigureAwait(false);

            Assert.That.DoesNotContain(output,
                                       "READ TWICE WITHOUT BUFFERING",
                                       because: "NonSeekableBodyErrorHandler narrows NotSupportedException down to a rewind attempt. Claiming a buffering problem for an unrelated NotSupportedException sends the author off to fix something that is not broken.",
                                       fix: "Make NonSeekableBodyErrorHandler.CanHandle check the exception's origin or message for the rewind case instead of accepting every NotSupportedException.");

            Assert.That.Contains(output,
                                 "UNEXPECTED TEST SDK ERROR",
                                 because: "With no specific handler willing to take it, this exception has to land on the catch-all - which is at least honest about not knowing what happened.",
                                 fix: "Check that declining the exception in NonSeekableBodyErrorHandler really lets the selection continue to DefaultErrorHandler.");
        }

        /// <summary>
        /// The catch-all has to be registered last. Anywhere else it wins every selection and no
        /// specific handler is ever reached.
        /// </summary>
        [TestMethod]
        public void TheDefaultHandlerMustBeRegisteredLast()
        {
            var handlers = BuildProvider().GetServices<ITestErrorHandler>().ToImmutableList();

            Assert.That.IsGreaterThan(handlers.Count,
                                      1,
                                      because: "With only the catch-all registered, every test above would still pass its 'falls back to default' half while every specific handler silently does nothing.",
                                      fix: "Check AddTestErrorHandlingStrategy - the specific handlers have to be registered next to DefaultErrorHandler, not instead of it.");

            Assert.That.IsAssignableTo<DefaultErrorHandler>(handlers[^1],
                                                            because: "The catch-all has to come last. Anywhere else it wins every selection and no specific handler is ever reached - exactly the silent failure this class exists for.",
                                                            fix: "Move the DefaultErrorHandler registration to the end of AddTestErrorHandlingStrategy; DI preserves registration order for GetServices.");

            for (var index = 0; index < handlers.Count - 1; index++)
            {
                Assert.That.IsNotAssignableTo<DefaultErrorHandler>(handlers[index],
                                                                   because: $"The catch-all appears at position {index} as well. A duplicate registration in front of the specific handlers has the same effect as registering it first - it swallows everything.",
                                                                   fix: "Check AddTestErrorHandlingStrategy for a second DefaultErrorHandler registration, e.g. one added by a nested Add... extension.");
            }
        }

        /// <summary>
        /// A handler that cannot serve the given context answers with an empty string. That must hand
        /// the exception to the next compatible handler - not drop straight to the bare fallback.
        /// </summary>
        [TestMethod]
        public async Task AHandlerThatCannotServeTheContextMustNotBlockTheNextOne()
        {
            // Build a provider with specific handlers: SilentHandler first, DefaultErrorHandler second
            var services = new ServiceCollection();
            services.AddSingletonIfNotExists<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddTestClassNameResolver();
            services.AddEndpointSourceResolver();
            services.AddSingleton<ITestErrorHandler, SilentHandler>();
            services.AddSingleton<ITestErrorHandler, DefaultErrorHandler>();
            services.AddSingleton<ITestErrorHandlingStrategy, TestErrorHandlingStrategy>();

            var strategy = services.BuildServiceProvider().GetRequiredService<ITestErrorHandlingStrategy>();

            var output = await strategy.HandleAsync(Context(), new InvalidOperationException("passed along")).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "UNEXPECTED TEST SDK ERROR",
                                 because: "SilentHandler claims it can handle the exception but returns an empty string. The strategy has to carry on to the next compatible handler instead of accepting that nothing as the answer.",
                                 fix: "Check TestErrorHandlingStrategy.HandleAsync - an empty result has to count as declined and the loop has to continue.");

            Assert.That.DoesNotContain(output,
                                       "UNHANDLED EXCEPTION",
                                       because: "The bare fallback is only for the case where no handler produced anything. Reaching it here would mean a perfectly capable handler was skipped.",
                                       fix: "The fallback in TestErrorHandlingStrategy has to run only after every registered handler declined - not after the first empty result.");
        }

        [TestMethod]
        public async Task WithNoHandlerAtAllTheFallbackMustStillNameTheException()
        {
            // Build a provider with NO handlers to test the fallback logic
            var services = new ServiceCollection();
            services.AddSingleton<ITestErrorHandlingStrategy, TestErrorHandlingStrategy>();

            var strategy = services.BuildServiceProvider().GetRequiredService<ITestErrorHandlingStrategy>();

            var output = await strategy.HandleAsync(Context(), new InvalidOperationException("nothing left")).ConfigureAwait(false);

            Assert.That.Contains(output,
                                 "UNHANDLED EXCEPTION",
                                 because: "Even with no handler registered at all the strategy has to produce output - an empty message turns a failure into a test that fails for no visible reason.",
                                 fix: "Check the last-resort branch in TestErrorHandlingStrategy.HandleAsync; it has to run when the handler list is empty.");

            Assert.That.Contains(output,
                                 "nothing left",
                                 because: "The bare fallback knows nothing about the exception, so passing its message through is the only information the author gets.",
                                 fix: "Check that the fallback in TestErrorHandlingStrategy includes exception.Message, not just a generic heading.");
        }

        private static Task<string> HandleAsync(Exception exception)
        {
            var strategy = BuildProvider().GetRequiredService<ITestErrorHandlingStrategy>();

            return strategy.HandleAsync(Context(), exception);
        }

        private static ServiceProvider BuildProvider()
        {
            var services = new ServiceCollection();

            services.AddSingletonIfNotExists<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddPlainTextDecorator();
            services.AddSourceCodeExtractor();
            services.AddTestErrorHandlingStrategy();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// A plain object context - no request, no response. The http-only handlers have to decline it
        /// instead of reaching for members that are not there.
        /// </summary>
        private static ObjectAssertContext<string> Context()
        {
            return new ObjectAssertContext<string>
            {
                CallerFilePath = ThisFile(),
                CallerLineNumber = 1,
                CallerMemberName = nameof(Context),
                CallingAssembly = typeof(ErrorHandlerSelectionTests).Assembly,
                Current = "current",
                CurrentObject = "current",
                CurrentResultParameterName = "current",
                DifferenceFunc = differences => differences,
                Expected = "expected",
                ExpectedType = typeof(string),
                ExpectedObjectAsJson = "Expected.json",
                ExpectedResultFile = new EmbeddedFileInfo("Expected.json", "Expected.json", null,
                                                                 false),
                ExpectedResultParameterName = "expected",
                OrderFunc = item => item,
                Parameters = [],
                ResolvedExpectedJson = null,
                TypeIsPrimitiveType = true,
                WriteResponse = false
            };
        }

        private static InvalidSnapshotJsonException BrokenJson()
        {
            try
            {
                using var _ = JsonDocument.Parse("{ \"a\": }");

                throw new InvalidOperationException("The fixture json has to be broken.");
            }
            catch (JsonException parseError)
            {
                return new InvalidSnapshotJsonException("Responses.Broken.json",
                                                        filePath: null,
                                                        content: "{ \"a\": }",
                                                        isPayload: false,
                                                        parseError);
            }
        }

        private static string ThisFile([System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            return callerFilePath;
        }

        /// <summary>Stands in for a handler that has nothing to say about the given context.</summary>
        private sealed class SilentHandler : ITestErrorHandler
        {
            public bool CanHandle(Exception exception)
            {
                return true;
            }

            public Task<string> HandleAsync(IObjectAssertContext context,
                                            Exception exception)
            {
                return Task.FromResult(string.Empty);
            }
        }
    }
}