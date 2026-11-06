using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips.Editing;

public sealed class CreateEquivalencyAssertionOptionsLambdaAction(int argumentIndex) : IEditAction
{
    public void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression)
    {
        const string lambdaParameter = "options";
        const string equivalencyAssertionOptionsMethod = "Using";

        SyntaxGenerator generator = editor.Generator;
        ArgumentSyntax optionsParameter = invocationExpression.ArgumentList.Arguments[argumentIndex];

        SyntaxNode equivalencyAssertionLambda = generator.ValueReturningLambdaExpression(lambdaParameter, generator.InvocationExpression(generator.MemberAccessExpression(generator.IdentifierName(lambdaParameter), equivalencyAssertionOptionsMethod), optionsParameter));
        editor.ReplaceNode(optionsParameter.Expression, equivalencyAssertionLambda);
    }
}
