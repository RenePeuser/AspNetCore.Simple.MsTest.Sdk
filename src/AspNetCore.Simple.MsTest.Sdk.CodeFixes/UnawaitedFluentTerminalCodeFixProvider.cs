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
    ///     Repairs MSTESTSDK002 (fluent terminal never awaited) with a single click.
    ///     <para>
    ///         The chain already ends in <c>ExecuteAsync()</c>, so — unlike the MSTESTSDK001 fix — nothing
    ///         is appended. Only the missing <c>await</c> is added, and the enclosing method/local function
    ///         is made <c>async</c> (<c>void</c> → <c>Task</c>) so it compiles.
    ///     </para>
    ///     <para>
    ///         FixAll (BatchFixer) is supported so a whole file/project/solution can be repaired at once.
    ///     </para>
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnawaitedFluentTerminalCodeFixProvider))]
    [Shared]
    public sealed class UnawaitedFluentTerminalCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Await the assertion ('await …ExecuteAsync()')";

        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(UnawaitedFluentTerminalAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
        {
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

            // Only the bare "chain.ExecuteAsync();" expression-statement shape is fixable — matches the analyzer.
            if (statement?.Expression is not InvocationExpressionSyntax)
            {
                return;
            }

            var codeAction = CodeAction.Create(Title,
                                               cancellationToken =>
                                                   FluentChainFixer.AwaitStatementAsync(context.Document,
                                                                                        statement,
                                                                                        appendTerminal: false,
                                                                                        cancellationToken),
                                               equivalenceKey: nameof(UnawaitedFluentTerminalCodeFixProvider));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}