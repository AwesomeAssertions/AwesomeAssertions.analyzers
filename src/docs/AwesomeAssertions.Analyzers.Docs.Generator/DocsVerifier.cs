using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AwesomeAssertions.Analyzers.Docs.Generator;

public abstract class DocsVerifier
{
    protected abstract string TestAttribute { get; }

    protected abstract string TestFile { get; }

    public async Task Execute()
    {
        CompilationUnitSyntax compilation = SyntaxFactory.ParseCompilationUnit(await File.ReadAllTextAsync(TestFile));
        SyntaxTree tree = compilation.SyntaxTree;

        var issues = new StringBuilder();

        void ValidateAssertions(IEnumerable<ExpressionStatementSyntax> oldAssertions, ExpressionStatementSyntax newAssertion, MethodDeclarationSyntax method)
        {
            foreach (ExpressionStatementSyntax oldAssertion in oldAssertions.Where(oldAssertion => !oldAssertion.WithoutTrivia().IsEquivalentTo(newAssertion.WithoutTrivia())))
            {
                issues.AppendLine($"[{TestFile.Split('\\')[^1]}] {method.Identifier} - actual: {oldAssertion.ToFullString()} expected: {newAssertion.ToFullString()}");
            }
        }

        Console.WriteLine($"File: {Path.GetFileName(TestFile)}");

        SyntaxNode root = await tree.GetRootAsync();
        IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        var methodsMap = methods.ToDictionary(m => m.Identifier.Text);

        foreach (MethodDeclarationSyntax method in methods.Where(m => m.AttributeLists.Any(list => list.Attributes.Count is 1 && list.Attributes[0].Name.ToString() == TestAttribute)))
        {
            Console.WriteLine($"### scenario: {method.Identifier}");

            (IEnumerable<ExpressionStatementSyntax> oldAssertions, ExpressionStatementSyntax newAssertion) = GetAssertionsFromMethod(method);

            ValidateAssertions(oldAssertions, newAssertion, method);

            if (methodsMap.TryGetValue($"{method.Identifier.Text}_Failure", out MethodDeclarationSyntax methodFailure))
            {
                (IEnumerable<ExpressionStatementSyntax> oldAssertionsFailure, ExpressionStatementSyntax newAssertionFailure) = GetAssertionsFromMethod(methodFailure);

                ValidateAssertions(oldAssertionsFailure, newAssertionFailure, methodFailure);
            }

            if (methodsMap.TryGetValue($"{method.Identifier.Text}_Failure_NewAssertion", out MethodDeclarationSyntax testWithFailureNewAssertion))
            {
                IEnumerable<MethodDeclarationSyntax> testWithFailureOldAssertions = methodsMap.Where(x => x.Key.StartsWith($"{method.Identifier.Text}_Failure_OldAssertion")).Select(x => x.Value);

                (IEnumerable<ExpressionStatementSyntax> _, ExpressionStatementSyntax newAssertionFailure) = GetAssertionsFromMethod(testWithFailureNewAssertion);
                foreach (MethodDeclarationSyntax testWithFailureOldAssertion in testWithFailureOldAssertions)
                {
                    (IEnumerable<ExpressionStatementSyntax> oldAssertionsFailure, ExpressionStatementSyntax _) = GetAssertionsFromMethod(testWithFailureOldAssertion);

                    ValidateAssertions(oldAssertionsFailure, newAssertionFailure, testWithFailureOldAssertion);
                }
            }
        }

        if (issues.Length > 0)
        {
            throw new InvalidOperationException(issues.ToString());
        }
    }

    private static (IEnumerable<ExpressionStatementSyntax> oldAssertions, ExpressionStatementSyntax newAssertion) GetAssertionsFromMethod(MethodDeclarationSyntax method)
    {
        SyntaxTrivia oldAssertionComment = method.DescendantTrivia().FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia) && x.ToString().Equals("// old assertion:", StringComparison.Ordinal));
        SyntaxTrivia newAssertionComment = method.DescendantTrivia().FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia) && x.ToString().Equals("// new assertion:", StringComparison.Ordinal));

        IEnumerable<ExpressionStatementSyntax> statements = method.Body.Statements.OfType<ExpressionStatementSyntax>();

        IEnumerable<ExpressionStatementSyntax> oldAssertions = statements.Where(x => x.Span.CompareTo(oldAssertionComment.Span) > 0 && x.Span.CompareTo(newAssertionComment.Span) < 0);
        ExpressionStatementSyntax newAssertion = statements.SingleOrDefault(x => x.Span.CompareTo(newAssertionComment.Span) > 0);

        return (oldAssertions, newAssertion);
    }
}
