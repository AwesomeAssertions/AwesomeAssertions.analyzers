using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips.Editing;

public class SkipInvocationNodeAction(InvocationExpressionSyntax skipInvocation) : IEditAction
{
    public void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression)
    {
        var methodMemberAccess = (MemberAccessExpressionSyntax)skipInvocation.Expression;
        editor.ReplaceNode(skipInvocation, methodMemberAccess.Expression);
    }
}
