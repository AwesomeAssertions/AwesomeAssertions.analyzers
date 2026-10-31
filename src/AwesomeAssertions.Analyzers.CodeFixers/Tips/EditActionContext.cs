using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips;

public class EditActionContext(DocumentEditor editor, InvocationExpressionSyntax invocationExpression)
{
    public DocumentEditor Editor { get; } = editor;

    public InvocationExpressionSyntax InvocationExpression { get; } = invocationExpression;

    public InvocationExpressionSyntax AwesomeAssertion { get; set; }
}
