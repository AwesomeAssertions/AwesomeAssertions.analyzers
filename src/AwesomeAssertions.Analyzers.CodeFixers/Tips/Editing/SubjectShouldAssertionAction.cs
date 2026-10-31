using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips.Editing;

public class SubjectShouldAssertionAction(int argumentIndex, string assertion)
{
    private readonly int argumentIndex = argumentIndex;

    protected string Assertion { get; } = assertion;

    public void Apply(EditActionContext context)
    {
        SyntaxGenerator generator = context.Editor.Generator;
        SeparatedSyntaxList<ArgumentSyntax> arguments = context.InvocationExpression.ArgumentList.Arguments;

        ArgumentSyntax subject = arguments[argumentIndex];
        SyntaxNode should = generator.InvocationExpression(generator.MemberAccessExpression(subject.Expression, "Should"));
        context.Editor.RemoveNode(subject);

        var memberAccess = (MemberAccessExpressionSyntax)generator.MemberAccessExpression(should, GenerateAssertion(generator)).WithTriviaFrom(context.InvocationExpression.Expression);

        context.Editor.ReplaceNode(context.InvocationExpression.Expression, memberAccess);
        context.AwesomeAssertion = context.InvocationExpression
            .WithExpression(memberAccess)
            .WithArgumentList(SyntaxFactory.ArgumentList(arguments.RemoveAt(argumentIndex)));
    }

    protected virtual SyntaxNode GenerateAssertion(SyntaxGenerator generator) => generator.IdentifierName(Assertion);
}
