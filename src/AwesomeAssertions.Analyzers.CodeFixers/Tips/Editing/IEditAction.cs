using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers;

public interface IEditAction
{
    void Apply(DocumentEditor editor, InvocationExpressionSyntax invocationExpression);
}
