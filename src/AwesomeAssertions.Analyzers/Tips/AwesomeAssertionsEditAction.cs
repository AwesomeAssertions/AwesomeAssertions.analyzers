using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers;


public record struct AwesomeAssertionEditActionContext(
    IInvocationOperation Assertion,
    InvocationExpressionSyntax AssertionExpression,
    IInvocationOperation Should,
    IOperation Subject,
    IInvocationOperation InvocationBeforeShould
);

public static class AwesomeAssertionsEditAction
{
    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> RenameAssertion(string newName)
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            var newNameNode = (IdentifierNameSyntax)editor.Generator.IdentifierName(newName);
            var memberAccess = (MemberAccessExpressionSyntax)context.AssertionExpression.Expression;
            editor.ReplaceNode(memberAccess.Name, newNameNode);
        };
    }

    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> SkipInvocationBeforeShould()
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            var invocationExpressionBeforeShould = (InvocationExpressionSyntax)context.InvocationBeforeShould.Syntax;
            var methodMemberAccess = (MemberAccessExpressionSyntax)invocationExpressionBeforeShould.Expression;

            editor.ReplaceNode(invocationExpressionBeforeShould, methodMemberAccess.Expression);
        };
    }

    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> SkipExpressionBeforeShould()
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            IEditAction skipExpressionNodeAction = context.Subject switch
            {
                IInvocationOperation invocationBeforeShould => new SkipInvocationNodeAction((InvocationExpressionSyntax)invocationBeforeShould.Syntax),
                IPropertyReferenceOperation propertyReferenceBeforeShould => new SkipMemberAccessNodeAction((MemberAccessExpressionSyntax)propertyReferenceBeforeShould.Syntax),
                _ => throw new NotSupportedException("[SkipExpressionBeforeShouldEditAction] Invalid expression before should invocation")
            };

            skipExpressionNodeAction.Apply(editor, context.AssertionExpression);
        };
    }

    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> RemoveAssertionArgument(int index)
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            editor.RemoveNode(context.AssertionExpression.ArgumentList.Arguments[index]);
        };
    }

    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> PrependArgumentsFromInvocationBeforeShouldToAssertion(int skipAssertionArguments = 0)
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            var invocationExpressionBeforeShould = (InvocationExpressionSyntax)context.InvocationBeforeShould.Syntax;
            var argumentList = invocationExpressionBeforeShould.ArgumentList;

            var combinedArguments = SyntaxFactory.ArgumentList(argumentList.Arguments.AddRange(context.AssertionExpression.ArgumentList.Arguments.Skip(skipAssertionArguments)));
            editor.ReplaceNode(context.AssertionExpression.ArgumentList, combinedArguments);
        };
    }
    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> RemoveInvocationOnAssertionArgument(int assertionArgumentIndex, int invocationArgumentIndex)
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            var invocationArgument = (IInvocationOperation)context.Assertion.Arguments[assertionArgumentIndex].Value;
            var expected = invocationArgument.Arguments[invocationArgumentIndex].Value.UnwrapConversion();

            editor.ReplaceNode(invocationArgument.Syntax, expected.Syntax);
        };
    }

    public static Action<DocumentEditor, AwesomeAssertionEditActionContext> UnwrapInvocationOnSubject(int argumentIndex)
    {
        return (DocumentEditor editor, AwesomeAssertionEditActionContext context) =>
        {
            var subjectReference = ((IInvocationOperation)context.Subject).Arguments[argumentIndex].Value;

            editor.ReplaceNode(context.Subject.Syntax, subjectReference.Syntax.WithTriviaFrom(context.Subject.Syntax));
        };
    }
}