using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// UNCONDITIONAL FAILURE with context.
    ///
    /// The Fail(message) overload in AssertHttpExtensions takes an already rendered message and is
    /// what the output pipeline uses. This one is for the hand-written case: it asks for the same
    /// because/fix as every other assertion and renders the standard sections around them.
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Fails the test unconditionally, explaining why the reached state is wrong and what to do.
        /// Use it for the branch a test must never reach.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="because">Why reaching this point is a failure (context)</param>
        /// <param name="fix">How to fix it (guidance)</param>
        /// <param name="details">Optional extra detail lines shown in the Details section</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Always thrown</exception>
        public static void Fail(this Assert _,
                                string because,
                                string fix,
                                string? details = null,
                                [CallerFilePath] string callerFilePath = "",
                                [CallerMemberName] string callerMemberName = "",
                                [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            AssertOutputHelper.BuildHeader(sb, "ASSERTION FAILED - UNREACHABLE STATE", textDecorator);

            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            AssertOutputHelper.BuildProblemSection(sb, "The test reached a point it was never supposed to reach.",
                                                   textDecorator);

            if (!string.IsNullOrWhiteSpace(details))
            {
                AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
                sb.AppendLine(details);
                sb.AppendLine();
            }

            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);
            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator);
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            throw new AssertFailedException(sb.ToString());
        }
    }
}