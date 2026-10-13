using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers
{
    /// <summary>
    ///     MSTESTSDK003 — reports a <c>WriteSnapshot()</c> left on a fluent chain, but ONLY in a Release build.
    ///     <para>
    ///         Recording is a Debug-only developer feature: in Debug the expected file is re-recorded on every
    ///         run, so a committed <c>WriteSnapshot()</c> keeps the test green whatever the endpoint returns.
    ///         In Release the sdk ignores it (see <c>WriteResponseService</c>) — harmless at runtime, but the
    ///         Release build is where a forgotten call can still be caught before it reaches the next Debug run.
    ///     </para>
    ///     <para>
    ///         Silent in Debug on purpose: that is where the author records and debugs, and a warning there
    ///         would fight the very workflow the method exists for. The gate is the compilation's optimization
    ///         level — the same thing the runtime check (<c>IsCompiledInDebug</c>) reads off the assembly.
    ///     </para>
    ///     <para>
    ///         Deliberately conservative: only <c>WriteSnapshot()</c> and a constant <c>true</c> are reported.
    ///         <c>WriteSnapshot(false)</c> records nothing, and a non-constant argument is taken as a deliberate
    ///         switch the author controls.
    ///     </para>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class WriteSnapshotInReleaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MSTESTSDK003";

        private const string WriteSnapshotMethodName = "WriteSnapshot";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId,
                                                                "WriteSnapshot() left on a fluent assertion chain",
                                                                "'WriteSnapshot()' is ignored in Release but re-records the expected file on every Debug run, which keeps the test green whatever the endpoint returns — remove it before committing. To update snapshots in bulk set the environment variable 'TestSdkSettings__WriteResponse=true' instead.",
                                                                "Reliability",
                                                                DiagnosticSeverity.Warning,
                                                                true,
                                                                "'WriteSnapshot()' is a Debug-only recording switch. Committed, every Debug run overwrites the expected file with the actual response, so the assertion can never fail there. This diagnostic is reported in Release builds only, so recording and debugging in Debug stay undisturbed.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(OnCompilationStart);
        }

        private static void OnCompilationStart(CompilationStartAnalysisContext context)
        {
            // Debug is where WriteSnapshot() is meant to be used - stay silent there.
            if (context.Compilation.Options.OptimizationLevel != OptimizationLevel.Release)
            {
                return;
            }

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            if (invocation.Expression is not MemberAccessExpressionSyntax { Name.Identifier.ValueText: WriteSnapshotMethodName } memberAccess)
            {
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;

            if (symbol is not IMethodSymbol method || !FluentBuilderMarker.CarriesFluentBuilderAttribute(method.ContainingType))
            {
                return;
            }

            if (!RecordsForSure(invocation, context))
            {
                return;
            }

            // From the method name to the closing parenthesis - the part the author has to delete.
            var location = Location.Create(invocation.SyntaxTree,
                                           TextSpan.FromBounds(memberAccess.Name.SpanStart, invocation.Span.End));

            context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }

        /// <summary>
        ///     True for <c>WriteSnapshot()</c> (the parameter defaults to <c>true</c>) and for a constant
        ///     <c>true</c> argument, positional or named.
        /// </summary>
        private static bool RecordsForSure(InvocationExpressionSyntax invocation,
                                           SyntaxNodeAnalysisContext context)
        {
            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count == 0)
            {
                return true;
            }

            var constant = context.SemanticModel.GetConstantValue(arguments[0].Expression, context.CancellationToken);

            return constant is { HasValue: true, Value: true };
        }
    }
}
