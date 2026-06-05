using System;
using System.Reflection;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers
{
    /// <summary>
    /// Shared helper for building beautiful, consistent assert failure outputs.
    /// Provides standard sections: Header, Test Info, Problem, Details, Context, Fix.
    /// </summary>
    internal static class AssertOutputHelper
    {
        /// <summary>
        /// Builds the standard header section for assert failures.
        /// Format: ❌ {failureTitle}
        /// </summary>
        public static void BuildHeader(StringBuilder sb,
                                       string failureTitle,
                                       ITextDecorator textDecorator)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error($"❌ {failureTitle}"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();
        }

        /// <summary>
        /// Builds the standard Test Information section.
        /// Shows: Project, Class, Method, Line, File (clickable URI)
        /// </summary>
        public static void BuildTestInfoSection(StringBuilder sb,
                                                string callerFilePath,
                                                string callerMemberName,
                                                int callerLineNumber,
                                                ITextDecorator textDecorator,
                                                Assembly? callingAssembly = null)
        {
            var projectName = callingAssembly?.GetName().Name ?? GetProjectNameFromPath(callerFilePath);

            var className = callingAssembly != null
                                ? TestContextHelper.ExtractFullyQualifiedClassName(callerFilePath, callingAssembly)
                                : GetClassNameFromPath(callerFilePath);

            var fileUri = $"file:///{callerFilePath.Replace('\\', '/')}:{callerLineNumber}";

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {projectName}");
            sb.AppendLine($"{"Class",-10} : {className}");
            sb.AppendLine($"{"Method",-10} : {callerMemberName}");
            sb.AppendLine($"{"Line",-10} : {callerLineNumber}");
            sb.AppendLine($"{"File",-10} : {fileUri}");
            sb.AppendLine();
        }

        /// <summary>
        /// Builds the Problem section header.
        /// </summary>
        public static void BuildProblemSection(StringBuilder sb,
                                               string problemDescription,
                                               ITextDecorator textDecorator)
        {
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(problemDescription);
            sb.AppendLine();
        }

        /// <summary>
        /// Builds the Details section header (caller must add detail lines).
        /// </summary>
        public static void BuildDetailsSectionHeader(StringBuilder sb,
                                                     ITextDecorator textDecorator)
        {
            sb.AppendLine(textDecorator.SectionTitle("📊 Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
        }

        /// <summary>
        /// Builds the Context (Why) section.
        /// </summary>
        public static void BuildContextSection(StringBuilder sb,
                                               string because,
                                               ITextDecorator textDecorator)
        {
            sb.AppendLine(textDecorator.SectionTitle("💭 Context (Why this matters)"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(because);
            sb.AppendLine();
        }

        /// <summary>
        /// Builds the Fix (How) section with multiple options.
        /// </summary>
        public static void BuildFixSection(StringBuilder sb,
                                           string primaryFix,
                                           ITextDecorator textDecorator,
                                           params string[] additionalOptions)
        {
            sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"Option 1: {primaryFix}");

            if (additionalOptions != null && additionalOptions.Length > 0)
            {
                for (int i = 0; i < additionalOptions.Length; i++)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Option {i + 2}: {additionalOptions[i]}");
                }
            }

            sb.AppendLine();
        }

        /// <summary>
        /// Builds the standard footer.
        /// </summary>
        public static void BuildFooter(StringBuilder sb,
                                       ITextDecorator textDecorator)
        {
            sb.AppendLine(textDecorator.Dim("══════════════════════════════════════════════════════════════"));
        }

        /// <summary>
        /// Extracts project name from file path.
        /// </summary>
        private static string GetProjectNameFromPath(string filePath)
        {
#pragma warning disable CA1031 // Do not catch general exception types - fallback to Unknown is acceptable
            try
            {
                var pathSegments = filePath.Replace("\\", "/").Split('/');

                // Look for .csproj pattern or common project folder names
                for (int i = pathSegments.Length - 1; i >= 0; i--)
                {
                    if (pathSegments[i].EndsWith(".Test", StringComparison.OrdinalIgnoreCase) ||
                        pathSegments[i].EndsWith("Tests", StringComparison.OrdinalIgnoreCase))
                    {
                        return pathSegments[i];
                    }
                }

                // Fallback: return parent folder of file
                return pathSegments[^2];
            }
            catch
            {
                return "Unknown";
            }
#pragma warning restore CA1031
        }

        /// <summary>
        /// Extracts class name from file path.
        /// </summary>
        private static string GetClassNameFromPath(string filePath)
        {
#pragma warning disable CA1031 // Do not catch general exception types - fallback to Unknown is acceptable
            try
            {
                return System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            catch
            {
                return "Unknown";
            }
#pragma warning restore CA1031
        }
    }
}