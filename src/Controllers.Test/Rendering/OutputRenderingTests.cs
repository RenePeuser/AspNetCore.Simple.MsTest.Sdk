using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.SdkScenarios;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Rendering
{
    /// <summary>
    /// <see cref="TestSdkSettings.OutputRendering"/> decides how a failure LOOKS.
    ///
    /// The VSTest TRX logger writes ESC and every emoji surrogate half as literal <c>\uXXXX</c> text, so
    /// a CI summary built from the TRX (dorny/test-reporter, ...) showed
    /// <c>\u001b[1;31m📋 SCHEMA MISMATCH\u001b[0m</c>. Colour used to follow only the build
    /// configuration of the consumer and emojis were hard coded, so neither could be turned off.
    ///
    /// The settings instance is the host's singleton, shared by every class - hence
    /// <see cref="DoNotParallelizeAttribute"/> and a reset after every test.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    [TestCategory("OutputRendering")]
    public sealed class OutputRenderingTests : ApiTestBase
    {
        // 500 ProblemDetails while the test expects success - an UNEXPECTED STATUS CODE output strategy.
        private const string FailingUrl = "api/v1/errors/not-implemented";

        private const string BlogPostUrl = "api/v1/sdk-scenarios/blog-post";

        [TestMethod]
        public async Task PlainTextMustCarryNeitherColourNorEmojiInAMismatchOutput()
        {
            var message = await StatusCodeMismatchMessage(OutputRendering.PlainText).ConfigureAwait(false);

            AssertPlainText(message);

            Assert.That.Contains(message,
                                 "UNEXPECTED STATUS CODE",
                                 because: "Plain text drops decoration, never information - the header text has to stay.",
                                 fix: "Check PlainTextRenderStrategy - a lead symbol is dropped, the text it leads is kept.");

            Assert.That.Contains(message,
                                 "══════",
                                 because: "Box drawing borders are valid XML and render in every Markdown summary - they are no reason to give up the layout.",
                                 fix: "Check that PlainTextRenderStrategy.Error returns the border unchanged.");
        }

        [TestMethod]
        public async Task PlainTextMustCarryNeitherColourNorEmojiInAnErrorHandlerOutput()
        {
            // The error handlers resolve their decorator per assert from context.CallingAssembly - a
            // second route next to the injected one of the output strategies.
            var message = await FailureMessage(OutputRendering.PlainText,
                                               () => Client.AssertGetAsync<BlogPost>(BlogPostUrl,
                                                                                     "Responses.OutputRenderingDoesNotExist.json",
                                                                                     writeResponse: false))
                              .ConfigureAwait(false);

            AssertPlainText(message);

            Assert.That.Contains(message,
                                 "FILE NOT FOUND",
                                 because: "The snapshot reference does not exist, so the snapshot not found handler has to answer - otherwise this test checked a different route.",
                                 fix: "Check that the reference Responses.OutputRenderingDoesNotExist.json really does not exist next to this test.");
        }

        [TestMethod]
        public async Task GraphicsMustCarryColourAndEmojiEvenInADebugBuild()
        {
            var message = await StatusCodeMismatchMessage(OutputRendering.Graphics).ConfigureAwait(false);

            Assert.That.Contains(message,
                                 "\x1b[",
                                 because: "Graphics forces colour - also for a DEBUG test assembly, which Auto would render without.",
                                 fix: "Check TextDecoratorProvider - OutputRendering.Graphics has to map to TextStyle.Graphics regardless of the consumer assembly.");

            Assert.That.Contains(message,
                                 "🚫",
                                 because: "Graphics renders every symbol as its emoji.",
                                 fix: "Check GraphicsTextRenderStrategy.LeadSymbol - it has to return OutputSymbols.Emoji.");
        }

        [TestMethod]
        public async Task AutoMustKeepTheEmojisAndFollowTheConsumerBuildForColour()
        {
            var message = await StatusCodeMismatchMessage(OutputRendering.Auto).ConfigureAwait(false);

            Assert.That.Contains(message,
                                 "🚫",
                                 because: "Auto is the default and has to look exactly like before - with emojis in DEBUG and RELEASE alike.",
                                 fix: "Check that Auto resolves to the Monochrome or Graphics strategy, never to PlainText.");

            var expectColour = !(System.Diagnostics.Debugger.IsAttached || typeof(OutputRenderingTests).Assembly.IsCompiledInDebug());

            Assert.That.AreEqual(expectColour,
                                 message.Contains('\x1b'),
                                 because: "Under Auto a DEBUG build or an attached debugger gets no colour (the test explorer shows the codes as raw characters), a RELEASE build gets colour.",
                                 fix: "Check TextDecoratorProvider.ResolveAuto.");
        }

        [TestMethod]
        public async Task ThePlainTextCheckMustRejectAGraphicsOutput()
        {
            // Must fail: a check that cannot tell graphics from plain text would keep every test above green.
            var message = await StatusCodeMismatchMessage(OutputRendering.Graphics).ConfigureAwait(false);

            Assert.That.ThrowsExactly<AssertFailedException>(() => AssertPlainText(message),
                                                             because: "A graphics output carries ANSI codes and emojis, so the plain text check has to reject it.",
                                                             fix: "Check AssertPlainText - it has to look for ESC and for emoji characters.");
        }

        [TestMethod]
        public void TheRenderingMustBindFromConfiguration()
        {
            // TestSdkSettings__OutputRendering=PlainText in a pipeline arrives as this key.
            var configuration = new ConfigurationBuilder().AddInMemoryCollection([new("TestSdkSettings:OutputRendering", "plaintext")])
                                                          .Build();
            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration);

            using var provider = services.BuildServiceProvider();

            Assert.That.AreEqual(OutputRendering.PlainText,
                                 provider.GetRequiredService<TestSdkSettings>().OutputRendering,
                                 because: "A pipeline switches the rendering through the environment, case-insensitively, without touching code.",
                                 fix: "Check that TestSdkSettings.OutputRendering is a settable property bound by AddTestSdkSettings(configuration).");
        }

        private static Task<string> StatusCodeMismatchMessage(OutputRendering rendering)
        {
            return FailureMessage(rendering,
                                  () => Client.AssertPostAsync<ProblemDetails>(FailingUrl,
                                                                               writeResponse: false,
                                                                               skipEndpointValidation: true));
        }

        private static async Task<string> FailureMessage(OutputRendering rendering,
                                                         Func<Task> failingAssert)
        {
            var settings = Services.GetRequiredService<TestSdkSettings>();
            var previous = settings.OutputRendering;

            settings.OutputRendering = rendering;

            try
            {
                var exception = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(failingAssert,
                                                                                            because: "The assert is built to fail - its message is what this test inspects.",
                                                                                            fix: "Check the endpoint and the snapshot reference used by this test.")
                                            .ConfigureAwait(false);

                return exception.Message;
            }
            finally
            {
                settings.OutputRendering = previous;
            }
        }

        private static void AssertPlainText(string message)
        {
            Assert.That.IsFalse(message.Contains('\x1b'),
                                because: "Plain text must not carry ANSI escape codes - the TRX logger turns ESC into literal \\u001b text.",
                                fix: "Check that every decorator call resolves through TextDecoratorProvider and that nothing writes an escape code itself.");

            var emojis = message.Where(IsEmoji).Distinct().Select(character => $"U+{(int)character:X4}").ToList();

            Assert.That.IsTrue(emojis.Count == 0,
                               because: $"Plain text must not carry emojis - the TRX logger escapes every surrogate half. Found: {string.Join(", ", emojis)}",
                               fix: "Replace the literal emoji with an OutputSymbol and render it through ITextDecorator.");
        }

        private static bool IsEmoji(char character)
        {
            // Surrogates cover every emoji beyond the BMP, the range below the dingbats (⚠ ❌ ✅ ✓ ✗ ✍).
            return char.IsSurrogate(character) || character is >= '☀' and <= '➿' || character == '️';
        }
    }
}
