using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddUnresolvedParameterSectionBuilderExtension
    {
        public static void AddUnresolvedParameterSectionBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IUnresolvedParameterSectionBuilder, UnresolvedParameterSectionBuilder>();
        }
    }

    public interface IUnresolvedParameterSectionBuilder
    {
        /// <summary>
        /// A hint about placeholders the snapshot still carries because nobody supplied a parameter for
        /// them. Empty when there is nothing to say.
        /// </summary>
        string Build(IObjectAssertContext context,
                     ImmutableList<Difference> differences);
    }

    /// <summary>
    /// A snapshot may hold placeholders - <c>"name": "$name$"</c> - that the assert resolves from its
    /// <c>parameters</c>. Forget one and the placeholder survives into the comparison, where it turns into
    /// a diff nobody can read: <c>"$age$"</c> against <c>99</c>, or, for a placeholder standing bare in
    /// place of a number, a parse error instead of a diff. Neither says the one thing that would end the
    /// search - that a parameter is missing.
    ///
    /// This adds that sentence. It runs only while an assert is already failing, which is what keeps it
    /// quiet for the legitimate case: a payload can carry <c>$SurveyLink$</c> as DATA, travel through the
    /// api and come back unchanged. Both sides then hold the same text, the assert passes, and no output
    /// is ever built.
    ///
    /// When there are differences it only reports placeholders sitting on the expected side of one, so an
    /// unrelated failure in a test that happens to carry such data stays free of red herrings. Only when
    /// the comparison produced no differences at all - the parse error - does it fall back to scanning the
    /// whole resolved snapshot, because then there is nothing more precise to go on.
    /// </summary>
    internal sealed class UnresolvedParameterSectionBuilder(ITextDecorator textDecorator) : IUnresolvedParameterSectionBuilder
    {
        public string Build(IObjectAssertContext context,
                            ImmutableList<Difference> differences)
        {
            if (context.IsNull())
            {
                return string.Empty;
            }

            var supplied = SuppliedParameterNames(context);
            var findings = Findings(context, differences, supplied).ToImmutableList();

            if (findings.IsEmpty)
            {
                return string.Empty;
            }

            return Render(context, findings, supplied);
        }

        private static ImmutableHashSet<string> SuppliedParameterNames(IObjectAssertContext context)
        {
            var parameters = context.Parameters;

            if (parameters.IsNullOrEmpty())
            {
                return ImmutableHashSet.Create<string>(StringComparer.OrdinalIgnoreCase);
            }

            return parameters.Where(parameter => parameter.Key.IsNotNullOrWhiteSpace())
                             .Select(parameter => parameter.Key.Trim('$'))
                             .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<(string Name, string MemberPath)> Findings(IObjectAssertContext context,
                                                                              ImmutableList<Difference> differences,
                                                                              ImmutableHashSet<string> supplied)
        {
            // With differences to go on, only a placeholder that actually sits on the expected side of one
            // is worth reporting.
            if (differences.IsNotNull() && differences.IsEmpty.IsFalse())
            {
                return differences.SelectMany(difference => PlaceholderJson.AllTokens(difference.Value1)
                                                                           .Select(name => (Name: name, difference.MemberPath)))
                                  .Where(finding => supplied.Contains(finding.Name).IsFalse())
                                  .DistinctBy(finding => finding.Name);
            }

            // No differences means the comparison never got that far - a snapshot that does not parse. The
            // whole document is all there is to look at.
            return PlaceholderJson.AllTokens(context.ResolvedExpectedJson)
                                  .Where(name => supplied.Contains(name).IsFalse())
                                  .Select(name => (Name: name, MemberPath: string.Empty));
        }

        private string Render(IObjectAssertContext context,
                              ImmutableList<(string Name, string MemberPath)> findings,
                              ImmutableHashSet<string> supplied)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("💡 Unresolved Parameter"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();

            var snapshotName = context.ExpectedResultFile?.EmbeddedFileName;
            var snapshot = snapshotName.IsNullOrWhiteSpace() ? "The expected snapshot" : snapshotName;

            stringBuilder.AppendLine(findings.Count == 1
                                         ? $"{snapshot} carries a placeholder that was never replaced:"
                                         : $"{snapshot} carries placeholders that were never replaced:");

            stringBuilder.AppendLine();

            foreach (var (name, memberPath) in findings)
            {
                stringBuilder.AppendLine(memberPath.IsNullOrWhiteSpace()
                                             ? $"  ${name}$"
                                             : $"  ${name}$   on   {memberPath}");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("What went wrong");
            stringBuilder.AppendLine(supplied.IsEmpty
                                         ? "  The assert was called without any parameters, so the placeholder stayed in the"
                                         : "  No parameter of that name was supplied, so the placeholder stayed in the");

            stringBuilder.AppendLine("  expected side and is now being compared against the value the api really sent.");

            if (supplied.IsEmpty.IsFalse())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"  Supplied: {string.Join(", ", supplied.Order().Select(name => $"${name}$"))}");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("What to do");
            stringBuilder.AppendLine($"  Pass it to the assert:   parameters: [(\"${findings[0].Name}$\", theValue)]");
            stringBuilder.AppendLine("  Or, if it is meant to be literal data the api echoes back unchanged, make sure the");
            stringBuilder.AppendLine("  request really sends it - then both sides carry the same text and it compares equal.");

            return stringBuilder.ToString();
        }
    }
}
