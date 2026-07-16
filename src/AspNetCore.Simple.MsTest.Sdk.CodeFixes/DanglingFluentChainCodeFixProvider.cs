using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using AspNetCore.Simple.MsTest.Sdk.Analyzers;

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

        private const string TerminalMethodName = "ExecuteAsync";

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
                                                   TerminateChainAsync(context.Document,
                                                                       statement,
                                                                       cancellationToken),
                                               equivalenceKey: nameof(DanglingFluentChainCodeFixProvider));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> TerminateChainAsync(Document document,
                                                                ExpressionStatementSyntax statement,
                                                                CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root is null)
            {
                return document;
            }

            var invocation = (InvocationExpressionSyntax)statement.Expression;

            // chain  →  await chain.ExecuteAsync();  (leading/trailing trivia preserved verbatim from the
            // original statement; the await prefix and .ExecuteAsync() suffix add no newlines, so NO
            // Formatter.Annotation — the formatter would otherwise rewrite EOLs to Environment.NewLine.)
            var awaitExpressionSyntax = SyntaxFactory.AwaitExpression(BuildAwaitKeyword(),
                                                                      BuildTerminalInvocation(invocation));

            var terminated = SyntaxFactory.ExpressionStatement(awaitExpressionSyntax)
                                          .WithTriviaFrom(statement);

            var enclosing = FindEnclosingExecutable(statement);

            SyntaxNode newRoot;

            if (enclosing is null)
            {
                // No recognizable async scope (e.g. top-level statements without a wrapper) — just
                // replace the statement; the host will surface any remaining await-context error.
                newRoot = root.ReplaceNode(statement, terminated);
            }
            else
            {
                // Replace the statement WITHIN the enclosing node first, then make that node async, so
                // both edits land in one non-overlapping ReplaceNode on the root.
                var updatedEnclosing = enclosing.ReplaceNode(statement, terminated);
                updatedEnclosing = MakeAsync(updatedEnclosing);

                newRoot = root.ReplaceNode(enclosing, updatedEnclosing);
            }

            newRoot = EnsureTasksUsing(newRoot);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxToken BuildAwaitKeyword()
        {
            // 'await' needs a trailing space so it doesn't glue onto the chain identifier.
            return SyntaxFactory.Token(SyntaxKind.AwaitKeyword).WithTrailingTrivia(SyntaxFactory.Space);
        }

        private static InvocationExpressionSyntax BuildTerminalInvocation(InvocationExpressionSyntax chain)
        {
            // Strip leading trivia (the original indentation moves to the 'await' keyword via the
            // statement's trivia) and trailing trivia (so ".ExecuteAsync()" attaches cleanly).
            var chainCore = chain.WithoutTrivia();

            var terminalAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                      chainCore,
                                                                      SyntaxFactory.IdentifierName(TerminalMethodName));

            return SyntaxFactory.InvocationExpression(terminalAccess);
        }

        private static SyntaxNode? FindEnclosingExecutable(SyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax:
                    case LocalFunctionStatementSyntax:
                    case ParenthesizedLambdaExpressionSyntax:
                    case SimpleLambdaExpressionSyntax:
                    case AnonymousMethodExpressionSyntax:
                        return ancestor;
                }
            }

            return null;
        }

        // NOTE: no Formatter.Annotation here — the added 'async' token and the replacement return type
        // carry their own trivia, so the enclosing node is NOT reformatted. Reformatting it would let
        // the Formatter rewrite line endings to Environment.NewLine and fight the document's own EOL.
        private static SyntaxNode MakeAsync(SyntaxNode executable)
        {
            switch (executable)
            {
                case MethodDeclarationSyntax method:
                    return MakeMethodAsync(method);

                case LocalFunctionStatementSyntax localFunction:
                    return MakeLocalFunctionAsync(localFunction);

                case ParenthesizedLambdaExpressionSyntax lambda:
                    return lambda.WithModifiers(AddAsync(lambda.Modifiers));

                case SimpleLambdaExpressionSyntax lambda:
                    return lambda.WithModifiers(AddAsync(lambda.Modifiers));

                case AnonymousMethodExpressionSyntax anonymous:
                    return anonymous.WithModifiers(AddAsync(anonymous.Modifiers));

                default:
                    return executable;
            }
        }

        private static MethodDeclarationSyntax MakeMethodAsync(MethodDeclarationSyntax method)
        {
            var (modifiers, returnType) =
                AddAsyncBeforeReturnType(method.Modifiers, TaskReturnType(method.ReturnType));

            return method.WithModifiers(modifiers).WithReturnType(returnType);
        }

        private static LocalFunctionStatementSyntax MakeLocalFunctionAsync(LocalFunctionStatementSyntax localFunction)
        {
            var (modifiers, returnType) =
                AddAsyncBeforeReturnType(localFunction.Modifiers, TaskReturnType(localFunction.ReturnType));

            return localFunction.WithModifiers(modifiers).WithReturnType(returnType);
        }

        // Adds 'async' in front of a method/local-function return type. When there are no existing
        // modifiers, 'async' becomes the FIRST token, so the return type's leading trivia (newline +
        // indentation) must move — verbatim, non-elastic — onto 'async', or Roslyn re-expands it to
        // Environment.NewLine and fights the document's own line endings.
        private static (SyntaxTokenList Modifiers, TypeSyntax ReturnType) AddAsyncBeforeReturnType(
            SyntaxTokenList modifiers,
            TypeSyntax returnType)
        {
            if (modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                return (modifiers, returnType);
            }

            if (modifiers.Count > 0)
            {
                // Existing modifiers already hold the leading trivia; just append 'async'.
                return (AddAsync(modifiers), returnType);
            }

            var asyncToken = SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
                                          .WithLeadingTrivia(returnType.GetLeadingTrivia())
                                          .WithTrailingTrivia(SyntaxFactory.Space);

            return (SyntaxFactory.TokenList(asyncToken), returnType.WithoutLeadingTrivia());
        }

        private static SyntaxTokenList AddAsync(SyntaxTokenList modifiers)
        {
            if (modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                return modifiers;
            }

            var asyncToken = SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
                                          .WithTrailingTrivia(SyntaxFactory.Space);

            return modifiers.Add(asyncToken);
        }

        // void → Task so 'await' is legal. Task<T> / Task and everything else stay untouched.
        private static TypeSyntax TaskReturnType(TypeSyntax returnType)
        {
            if (returnType is PredefinedTypeSyntax predefined
                && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return SyntaxFactory.IdentifierName("Task").WithTriviaFrom(returnType);
            }

            return returnType;
        }

        private static SyntaxNode EnsureTasksUsing(SyntaxNode root)
        {
            if (root is not CompilationUnitSyntax compilationUnit)
            {
                return root;
            }

            const string tasksNamespace = "System.Threading.Tasks";

            var alreadyImported = compilationUnit.Usings
                                                 .Any(u => u.Name?.ToString() == tasksNamespace);

            if (alreadyImported)
            {
                return compilationUnit;
            }

            var usingDirective = SyntaxFactory
                                 .UsingDirective(SyntaxFactory.ParseName(tasksNamespace))
                                 .WithAdditionalAnnotations(Formatter.Annotation);

            return compilationUnit.AddUsings(usingDirective);
        }
    }
}