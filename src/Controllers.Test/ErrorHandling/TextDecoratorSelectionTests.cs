using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.ErrorHandling
{
    /// <summary>
    /// Whether output carries ANSI colour has to follow the CONSUMER test assembly, never how the sdk
    /// itself was compiled.
    ///
    /// This used to be decided by <c>#if DEBUG</c> inside the sdk. Since the sdk ships as a RELEASE
    /// built nuget package that condition is permanently false for consumers, so a DEBUG test project
    /// received escape codes and Visual Studio Test Explorer rendered them as raw characters. The
    /// mismatch output was rebuilt per assert and looked right, while the error handler output was
    /// wired to a static field frozen at type init and did not - the same run showed both.
    /// </summary>
    [TestClass]
    [TestCategory("ErrorHandling")]
    public sealed partial class TextDecoratorSelectionTests
    {
        [GeneratedRegex(@"\x1b\[[0-9;]*m", RegexOptions.Compiled)]
        private static partial Regex AnsiEscapeCode();

        private static Assembly ConsumerAssembly => typeof(TextDecoratorSelectionTests).Assembly;

        [TestMethod]
        public async Task ErrorHandlerOutputMustFollowTheConsumerAssemblyAndNotTheSdkBuild()
        {
            var exception = new SnapshotNotFoundException("Persons.json",
                                                          "\"Persons.json\"",
                                                          ConsumerAssembly,
                                                          isPayload: false);

            var output = await HandleAsync(exception).ConfigureAwait(false);

            var carriesAnsi = AnsiEscapeCode().IsMatch(output);

            // A debugger suffers the same artifacts as the test explorer, so it always forces plain.
            var expectPlain = Debugger.IsAttached || ConsumerAssembly.IsCompiledInDebug();

            if (expectPlain)
            {
                Assert.That.IsFalse(carriesAnsi,
                                    because: "This assembly is a DEBUG build (or a debugger is attached), so the handler output must be plain - ANSI codes show up as literal characters in the Visual Studio Test Explorer.",
                                    fix: "Check that the error handlers resolve through ITextDecoratorProvider using context.CallingAssembly, and that nothing froze a concrete decorator into a static field.");
            }
            else
            {
                Assert.That.IsTrue(carriesAnsi,
                                   because: "This assembly is a RELEASE build with no debugger, which is the CI case - colour is wanted there.",
                                   fix: "Check TextDecoratorProvider.For - a RELEASE consumer without a debugger has to end up on the ANSI decorator.");
            }
        }

        /// <summary>
        /// Deliberately registers the production decorator rather than a plain one: registering plain
        /// here would make this test pass no matter which decorator the sdk actually picks. The
        /// handlers themselves resolve through ITextDecoratorProvider from context.CallingAssembly.
        /// </summary>
        private static Task<string> HandleAsync(Exception exception)
        {
            var services = new ServiceCollection();

            services.AddSingletonIfNotExists<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddTextDecorator(ConsumerAssembly);
            services.AddSourceCodeExtractor();
            services.AddTestErrorHandlingStrategy();

            var strategy = services.BuildServiceProvider().GetRequiredService<ITestErrorHandlingStrategy>();

            return strategy.HandleAsync(Context(), exception);
        }

        private static ObjectAssertContext<string> Context()
        {
            return new ObjectAssertContext<string>
                   {
                       CallerFilePath = string.Empty,
                       CallerLineNumber = 1,
                       CallerMemberName = nameof(Context),
                       CallingAssembly = ConsumerAssembly,
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
    }
}