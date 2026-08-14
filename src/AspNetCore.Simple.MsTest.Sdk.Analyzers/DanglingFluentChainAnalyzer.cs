using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AspNetCore.Simple.MsTest.Sdk.Analyzers
{
    /// <summary>
    ///     MSTESTSDK001 — reports a fluent assertion chain that is never terminated (never executed).
    ///     <para>
    ///         A chain whose final expression type is marked with <c>[FluentBuilder]</c> but is used as a
    ///         bare expression statement (not awaited, returned, or assigned) means the request is NEVER
    ///         sent — the worst failure mode for a test SDK: the test silently passes. Because the builder is
    ///         not a <see cref="System.Threading.Tasks.Task" />, the compiler does not even emit CS4014.
    ///     </para>
    ///     <para>
    ///         Deliberately conservative: only the obvious dangling-expression-statement case is flagged.
    ///         Chains stored in a variable and terminated later are intentionally NOT reported (avoids false
    ///         positives).
    ///     </para>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DanglingFluentChainAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MSTESTSDK001";

        private const string FluentBuilderAttributeName =
            "AspNetCore.Simple.MsTest.Sdk.FluentAssertions.FluentBuilderAttribute";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId,
                                                                "Fluent assertion chain never executed",
                                                                "Assertion chain is never executed — the terminal 'ExecuteAsync()' is missing. The request is never sent and the test can pass silently.",
                                                                "Reliability",
                                                                DiagnosticSeverity.Error,
                                                                true,
                                                                "A fluent HTTP assertion chain must end with 'ExecuteAsync()'. A dangling builder expression is never sent, so the test is silently green.");

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

            // Only bare "chain();" statements can dangle. await/return/assignment consume the value.
            if (expressionStatement.Expression is not InvocationExpressionSyntax invocation)
            {
                return;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(invocation, context.CancellationToken);
            var type = typeInfo.Type;

            if (type is null || !CarriesFluentBuilderAttribute(type))
            {
                return;
            }

            // Span the entire statement including the semicolon.
            var start = invocation.Span.Start;
            var end = expressionStatement.SemicolonToken.Span.End;
            var location = Location.Create(expressionStatement.SyntaxTree,
                                           Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(start, end));
            context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }

        private static bool CarriesFluentBuilderAttribute(ITypeSymbol type)
        {
            if (HasAttribute(type))
            {
                return true;
            }

            return type.AllInterfaces.Any(HasAttribute);
        }

        private static bool HasAttribute(ISymbol symbol)
        {
            return symbol.GetAttributes()
                         .Any(a => a.AttributeClass?.ToDisplayString() == FluentBuilderAttributeName);
        }
    }
}