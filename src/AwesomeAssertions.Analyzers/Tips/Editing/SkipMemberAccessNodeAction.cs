using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers;

public class SkipMemberAccessNodeAction(MemberAccessExpressionSyntax skipMemberAccess) : IEditAction
{
    public void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression)
    {
        editor.ReplaceNode(skipMemberAccess, skipMemberAccess.Expression);
    }
}