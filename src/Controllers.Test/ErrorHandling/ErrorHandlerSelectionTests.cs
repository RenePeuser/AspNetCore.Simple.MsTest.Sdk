using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers;
using AspNetCore.Simple.MsTest.Sdk.Validation;
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

            StringAssert.Contains(output, "SNAPSHOT FILE NOT FOUND");
            Assert.IsFalse(output.Contains("UNEXPECTED TEST SDK ERROR", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task AMissingPayloadMustBeAnsweredAsAPayload()
        {
            var exception = new SnapshotNotFoundException("Request.json",
                                                          "payload",
                                                          typeof(ErrorHandlerSelectionTests).Assembly,
                                                          isPayload: true);

            var output = await HandleAsync(exception).ConfigureAwait(false);

            StringAssert.Contains(output, "PAYLOAD FILE NOT FOUND");
        }

        [TestMethod]
        public async Task BrokenSnapshotJsonMustBeAnsweredByTheSyntaxHandler()
        {
            var output = await HandleAsync(BrokenJson()).ConfigureAwait(false);

            StringAssert.Contains(output, "IS NOT VALID JSON");
            Assert.IsFalse(output.Contains("UNEXPECTED TEST SDK ERROR", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task AnExceptionWithoutASpecificHandlerMustFallBackToTheDefault()
        {
            var output = await HandleAsync(new InvalidOperationException("something nobody planned for")).ConfigureAwait(false);

            StringAssert.Contains(output, "UNEXPECTED TEST SDK ERROR");
            StringAssert.Contains(output, "something nobody planned for");
        }

        /// <summary>
        /// NonSeekableBodyErrorHandler narrows NotSupportedException down to a rewind attempt. An
        /// ordinary NotSupportedException must not be dressed up as a buffering problem.
        /// </summary>
        [TestMethod]
        public async Task AnUnrelatedNotSupportedExceptionMustNotBeTakenForARewoundBody()
        {
            var output = await HandleAsync(new NotSupportedException("this operation is not supported")).ConfigureAwait(false);

            Assert.IsFalse(output.Contains("READ TWICE WITHOUT BUFFERING", StringComparison.Ordinal));
            StringAssert.Contains(output, "UNEXPECTED TEST SDK ERROR");
        }

        /// <summary>
        /// The catch-all has to be registered last. Anywhere else it wins every selection and no
        /// specific handler is ever reached.
        /// </summary>
        [TestMethod]
        public void TheDefaultHandlerMustBeRegisteredLast()
        {
            var handlers = BuildProvider().GetServices<ITestErrorHandler>().ToImmutableList();

            Assert.IsGreaterThan(1, handlers.Count, "No specific handlers are registered at all.");

            Assert.IsInstanceOfType<DefaultErrorHandler>(handlers[^1],
                                                         "The catch-all is not last - it would swallow every exception.");

            for (var index = 0; index < handlers.Count - 1; index++)
            {
                Assert.IsNotInstanceOfType<DefaultErrorHandler>(handlers[index],
                                                                $"The catch-all is also registered at position {index}.");
            }
        }

        /// <summary>
        /// A handler that cannot serve the given context answers with an empty string. That must hand
        /// the exception to the next compatible handler - not drop straight to the bare fallback.
        /// </summary>
        [TestMethod]
        public async Task AHandlerThatCannotServeTheContextMustNotBlockTheNextOne()
        {
            var strategy = new TestErrorHandlingStrategy([new SilentHandler(), new DefaultErrorHandler()]);

            var output = await strategy.HandleAsync(Context(), new InvalidOperationException("passed along")).ConfigureAwait(false);

            StringAssert.Contains(output, "UNEXPECTED TEST SDK ERROR");
            Assert.IsFalse(output.Contains("UNHANDLED EXCEPTION", StringComparison.Ordinal),
                           "The bare fallback ran even though a capable handler was registered.");
        }

        [TestMethod]
        public async Task WithNoHandlerAtAllTheFallbackMustStillNameTheException()
        {
            var strategy = new TestErrorHandlingStrategy([]);

            var output = await strategy.HandleAsync(Context(), new InvalidOperationException("nothing left")).ConfigureAwait(false);

            StringAssert.Contains(output, "UNHANDLED EXCEPTION");
            StringAssert.Contains(output, "nothing left");
        }

        private static Task<string> HandleAsync(Exception exception)
        {
            var strategy = BuildProvider().GetRequiredService<ITestErrorHandlingStrategy>();

            return strategy.HandleAsync(Context(), exception);
        }

        private static ServiceProvider BuildProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
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
                ExpectedResultFile = new EmbeddedFileInfo("Expected.json", "Expected.json", null, false),
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
