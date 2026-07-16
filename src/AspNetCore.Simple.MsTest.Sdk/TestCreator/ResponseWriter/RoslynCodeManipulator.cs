using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface IRoslynCodeManipulator
    {
        void ReplaceEmptyAnonymousObject(string filePath,
                                         int lineNumber,
                                         string variableName,
                                         string newInitializerCode);

        (string typeName, ObjectConstructionType constructionType) ExtractTypeInfo(string filePath,
                                                                                   int lineNumber,
                                                                                   string variableName);
    }

    internal sealed class RoslynCodeManipulator : IRoslynCodeManipulator
    {
        public void ReplaceEmptyAnonymousObject(string filePath,
                                                int lineNumber,
                                                string variableName,
                                                string newInitializerCode)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Read and parse file
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            // Find the line containing the empty anonymous object
            var targetLine = lineNumber - 1; // Roslyn uses 0-based line numbers
            var lines = tree.GetText().Lines;

            if (targetLine < 0 || targetLine >= lines.Count)
            {
                throw new InvalidOperationException($"Line number {lineNumber} is out of range");
            }

            var targetPosition = lines[targetLine].Start;

            // Find the method containing the position (we need to search the whole method for the variable)
            var method = (root.FindToken(targetPosition)
                              .Parent
                              ?.AncestorsAndSelf()
                              .OfType<MethodDeclarationSyntax>()
                              .FirstOrDefault()) ?? throw new InvalidOperationException($"Could not find method at line {lineNumber}");

            // Find the object creation expression in this method
            // Strategy: Look for any object creation (ObjectCreationExpressionSyntax, ImplicitObjectCreationExpressionSyntax, AnonymousObjectCreationExpressionSyntax)
            // that is being assigned to the variable with the given name

            SyntaxNode? targetNode = null;

            // Try 1: Find variable declarator with matching identifier (search in whole method)
            var variableDeclarator = method.DescendantNodes()
                                           .OfType<VariableDeclaratorSyntax>()
                                           .FirstOrDefault(v => v.Identifier.Text == variableName);

            if (variableDeclarator != null && variableDeclarator.Initializer != null)

                // Get the initializer expression
            {
                targetNode = variableDeclarator.Initializer.Value;
            }

            // Try 2: Find assignment expression with matching left side
            if (targetNode == null)
            {
                var assignmentExpression = method.DescendantNodes()
                                                 .OfType<AssignmentExpressionSyntax>()
                                                 .FirstOrDefault(a => a.Left.ToString().Contains(variableName, StringComparison.Ordinal));

                if (assignmentExpression != null)
                {
                    targetNode = assignmentExpression.Right;
                }
            }

            // Try 3: Fallback - find first object creation expression
            targetNode ??= method.DescendantNodes()
                                 .OfType<ObjectCreationExpressionSyntax>()
                                 .FirstOrDefault();

            targetNode ??= method.DescendantNodes()
                                 .OfType<AnonymousObjectCreationExpressionSyntax>()
                                 .FirstOrDefault();

            if (targetNode == null)
            {
                throw new InvalidOperationException($"No object creation expression found for variable '{variableName}' at line {lineNumber}");
            }

            // Parse the new initializer code
            var newNode = SyntaxFactory.ParseExpression(newInitializerCode)
                                       .WithTriviaFrom(targetNode); // Preserve surrounding whitespace

            // Replace node
            var newRoot = root.ReplaceNode(targetNode, newNode);

            // Write back to file
            File.WriteAllText(filePath, newRoot.ToFullString());
        }

        public (string typeName, ObjectConstructionType constructionType) ExtractTypeInfo(
            string filePath,
            int lineNumber,
            string variableName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            // Read and parse file
            var code = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetCompilationUnitRoot();

            // Find the line containing the call
            var targetLine = lineNumber - 1;
            var lines = tree.GetText().Lines;

            if (targetLine < 0 || targetLine >= lines.Count)
            {
                return ("object", ObjectConstructionType.AnonymousObject);
            }

            var targetPosition = lines[targetLine].Start;

            // Find the method containing the position
            var method = root.FindToken(targetPosition)
                             .Parent
                             ?.AncestorsAndSelf()
                             .OfType<MethodDeclarationSyntax>()
                             .FirstOrDefault();

            if (method == null)
            {
                return ("object", ObjectConstructionType.AnonymousObject);
            }

            // Find variable declarator
            var variableDeclarator = method.DescendantNodes()
                                           .OfType<VariableDeclaratorSyntax>()
                                           .FirstOrDefault(v => v.Identifier.Text == variableName);

            if (variableDeclarator == null || variableDeclarator.Initializer == null)
            {
                return ("object", ObjectConstructionType.AnonymousObject);
            }

            var initializerValue = variableDeclarator.Initializer.Value;

            // Extract type info from the initializer
            return initializerValue switch
            {
                ObjectCreationExpressionSyntax objectCreation => ExtractTypeFromObjectCreation(objectCreation),
                ImplicitObjectCreationExpressionSyntax => ("object", ObjectConstructionType.ClassNominal),
                _ => ("object", ObjectConstructionType.AnonymousObject)
            };
        }

        private static (string typeName, ObjectConstructionType constructionType) ExtractTypeFromObjectCreation(ObjectCreationExpressionSyntax objectCreation)
        {
            var typeName = objectCreation.Type.ToString();

            // Determine construction type based on initializer presence
            if (objectCreation.ArgumentList != null && objectCreation.ArgumentList.Arguments.Count > 0 && objectCreation.Initializer == null)

                // Positional: new Person(1, "a", "b")
            {
                return (typeName, ObjectConstructionType.RecordPositional);
            }

            if (objectCreation.Initializer != null)

                // Nominal: new Person { Id = 1, Name = "a" }
            {
                return (typeName, ObjectConstructionType.RecordNominal);
            }

            // Default
            return (typeName, ObjectConstructionType.RecordPositional);
        }
    }

    public static class AddRoslynCodeManipulatorExtension
    {
        public static void AddRoslynCodeManipulator(this IServiceCollection services)
        {
            services.AddSingleton<IRoslynCodeManipulator, RoslynCodeManipulator>();
        }
    }
}