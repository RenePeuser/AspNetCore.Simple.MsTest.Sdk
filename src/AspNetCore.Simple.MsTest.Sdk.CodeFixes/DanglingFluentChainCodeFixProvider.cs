using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCore.Simple.MsTest.Sdk.CodeFixes
{
    /// <summary>
    ///     Repairs MSTESTSDK001 (dangling fluent assertion chain) with a single click.
    ///     <para>
    ///         The chain lacks its terminal, so the request is never sent and the test can pass
    ///         silently. The fix appends the terminal <c>.ExecuteAsync()</c>, wraps the whole chain in
    ///         <c>await</c>, and — if needed — turns the enclosing method/local function into
    ///         <c>async</c> (converting a <c>void</c> return type to <c>Task</c>) so the result compiles.
    ///     </para>
    ///     <para>
    ///         FixAll (BatchFixer) is supported so an entire file/project/solution can be migrated at once
    ///         — relevant when the fluent API flips from <c>internal</c> to <c>public</c>.
    ///     </para>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DanglingFluentChainCodeFixProvider))]
    [Shared]
    public sealed class DanglingFluentChainCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Terminate chain with 'await …ExecuteAsync()'";

        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(DanglingFluentChainAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
            // Batch-fix whole document/project/solution — needed for bulk migration.
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document
                                    .GetSyntaxRootAsync(context.CancellationToken)
                                    .ConfigureAwait(false);

            if (root is null)
            {
                return;
            }

            var diagnostic = context.Diagnostics[0];
            var node = root.FindNode(diagnostic.Location.SourceSpan);

            var statement = node.FirstAncestorOrSelf<ExpressionStatementSyntax>();

            // Only the bare "chain();" expression-statement shape is fixable — matches the analyzer.
            if (statement?.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            var codeAction = CodeAction.Create(Title,
                                               cancellationToken =>
                                                   FluentChainFixer.AwaitStatementAsync(context.Document,
                                                                                        statement,
                                                                                        appendTerminal: true,
                                                                                        cancellationToken),
                                               equivalenceKey: nameof(DanglingFluentChainCodeFixProvider));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}