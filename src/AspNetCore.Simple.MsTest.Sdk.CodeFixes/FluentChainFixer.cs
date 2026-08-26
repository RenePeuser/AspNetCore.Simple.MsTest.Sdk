using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace AspNetCore.Simple.MsTest.Sdk.CodeFixes
{
    /// <summary>
    ///     Shared rewrite for both fluent-chain code fixes: wrap a statement in <c>await</c> and make the
    ///     enclosing scope <c>async</c> (turning a <c>void</c> return type into <c>Task</c>) so the result
    ///     compiles.
    ///     <para>
    ///         MSTESTSDK001 additionally appends the missing <c>.ExecuteAsync()</c>; MSTESTSDK002 only adds
    ///         the <c>await</c> because the terminal is already there. That single difference is the
    ///         <c>appendTerminal</c> flag — everything else is identical, including the trivia handling
    ///         both fixes depend on.
    ///     </para>
    /// </summary>
    internal static class FluentChainFixer
    {
        private const string TerminalMethodName = "ExecuteAsync";

        /// <summary>
        ///     Rewrites <paramref name="statement" /> to <c>await …;</c>, optionally appending the terminal,
        ///     and makes the enclosing method/local function/lambda async.
        /// </summary>
        internal static async Task<Document> AwaitStatementAsync(Document document,
                                                                 ExpressionStatementSyntax statement,
                                                                 bool appendTerminal,
                                                                 CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root is null)
            {
                return document;
            }

            var invocation = (InvocationExpressionSyntax)statement.Expression;

            var awaited = appendTerminal
                              ? BuildTerminalInvocation(invocation)
                              : (ExpressionSyntax)invocation.WithoutTrivia();

            // chain  →  await chain[.ExecuteAsync()];  (leading/trailing trivia preserved verbatim from the
            // original statement; the await prefix and .ExecuteAsync() suffix add no newlines, so NO
            // Formatter.Annotation — the formatter would otherwise rewrite EOLs to Environment.NewLine.)
            var awaitExpressionSyntax = SyntaxFactory.AwaitExpression(BuildAwaitKeyword(), awaited);

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
