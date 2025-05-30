using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers;

public class CreateEquivalencyAssertionOptionsLambdaAction(int argumentIndex) : IEditAction
{
    public void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression)
    {
        const string lambdaParameter = "options";
        const string equivalencyAssertionOptionsMethod = "Using";

        var generator = editor.Generator;
        var optionsParameter = invocationExpression.ArgumentList.Arguments[argumentIndex];

        var equivalencyAssertionLambda = generator.ValueReturningLambdaExpression(lambdaParameter, generator.InvocationExpression(generator.MemberAccessExpression(generator.IdentifierName(lambdaParameter), equivalencyAssertionOptionsMethod), optionsParameter));
        editor.ReplaceNode(optionsParameter.Expression, equivalencyAssertionLambda);
    }
}