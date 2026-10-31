using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips.Editing;

public interface IEditAction
{
    void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression);
}
