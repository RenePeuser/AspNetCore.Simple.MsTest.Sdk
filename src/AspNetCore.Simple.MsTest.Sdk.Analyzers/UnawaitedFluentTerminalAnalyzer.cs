using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers
{
    /// <summary>
    ///     MSTESTSDK002 — reports a fluent assertion chain that IS terminated with
    ///     <c>ExecuteAsync()</c> but whose <see cref="System.Threading.Tasks.Task" /> is never consumed.
    ///     <para>
    ///         This is the second stage of the silent-green failure mode (DESIGN_VISION §2/§5).
    ///         MSTESTSDK001 catches the missing terminal; it cannot catch a present-but-unawaited one,
    ///         because the statement's type is then a <c>Task</c> and no longer a <c>[FluentBuilder]</c>.
    ///     </para>
    ///     <para>
    ///         The compiler does not close the gap either: <c>CS4014</c> only fires inside an
    ///         <c>async</c> method. In a synchronous test method the fire-and-forget call is entirely
    ///         silent — the assertion runs detached, its failure lands on no test, and the test is GREEN
    ///         even when the expectation is wrong.
    ///     </para>
    ///     <para>
    ///         Deliberately conservative, mirroring MSTESTSDK001: only the bare expression-statement
    ///         shape is flagged. <c>await</c>, <c>return</c>, and assignment all consume the task and are
    ///         never reported. An explicit discard (<c>_ = …ExecuteAsync();</c>) is read as intentional
    ///         fire-and-forget and left alone.
    ///     </para>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UnawaitedFluentTerminalAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MSTESTSDK002";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId,
                                                                "Fluent assertion result is never awaited",
                                                                "Assertion chain is terminated but never awaited — the 'await' before 'ExecuteAsync()' is missing. The assertion runs detached and the test can pass silently.",
                                                                "Reliability",
                                                                DiagnosticSeverity.Error,
                                                                true,
                                                                "'ExecuteAsync()' returns a Task that must be awaited or returned. An unawaited terminal detaches the assertion from the test: a failing expectation never fails the test. In a synchronous test method not even CS4014 warns about it.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
        }

        private static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var expressionStatement = (ExpressionStatementSyntax)context.Node;

            // Only bare "chain.ExecuteAsync();" statements dangle. await/return/assignment consume the task.
            if (expressionStatement.Expression is not InvocationExpressionSyntax invocation)
            {
                return;
            }

            if (!ChainContainsFluentTerminal(invocation, context))
            {
                return;
            }

            // Span the invocation including the trailing semicolon, matching MSTESTSDK001 so the code fix
            // can resolve the whole statement from the diagnostic location.
            var start = invocation.Span.Start;
            var end = expressionStatement.SemicolonToken.Span.End;

            var location = Location.Create(expressionStatement.SyntaxTree,
                                           Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(start, end));

            context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }

        /// <summary>
        ///     Walks the invocation chain outside-in looking for the fluent terminal. Descending is what
        ///     makes the repo's own idiom <c>…ExecuteAsync().ConfigureAwait(false);</c> reportable — there
        ///     the outermost call is <c>ConfigureAwait</c> and the terminal sits one level in.
        /// </summary>
        private static bool ChainContainsFluentTerminal(ExpressionSyntax? expression,
                                                        SyntaxNodeAnalysisContext context)
        {
            while (expression is InvocationExpressionSyntax invocation)
            {
                if (IsFluentTerminal(invocation, context))
                {
                    return true;
                }

                expression = (invocation.Expression as MemberAccessExpressionSyntax)?.Expression;
            }

            return false;
        }

        private static bool IsFluentTerminal(InvocationExpressionSyntax invocation,
                                             SyntaxNodeAnalysisContext context)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;

            if (symbol is not IMethodSymbol { Name: FluentBuilderMarker.TerminalMethodName } method)
            {
                return false;
            }

            // Interface dispatch puts the marked interface in ContainingType; a call through the concrete
            // builder puts the class there, which reaches the marker via its interfaces.
            return FluentBuilderMarker.CarriesFluentBuilderAttribute(method.ContainingType);
        }
    }
}
