using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips;

public static class FluentChainedAssertionEditAction
{
    public static Action<DocumentEditor, FluentChainedAssertionEditActionContext> CombineAssertionsWithNameAndArguments(string newName, CombineAssertionArgumentsStrategy strategy)
    {
        return CombineAssertionsWithName(newName, (_, context) =>
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = strategy switch
            {
                CombineAssertionArgumentsStrategy.FirstAssertionFirst => context.AssertionAExpression.ArgumentList.Arguments.AddRange(context.AssertionBExpression.ArgumentList.Arguments),
                CombineAssertionArgumentsStrategy.InsertFirstAssertionIntoIndex1OfSecondAssertion => context.AssertionBExpression.ArgumentList.Arguments.InsertRange(1, context.AssertionAExpression.ArgumentList.Arguments),
                CombineAssertionArgumentsStrategy.InsertSecondAssertionIntoIndex1OfFirstAssertion => context.AssertionAExpression.ArgumentList.Arguments.InsertRange(1, context.AssertionBExpression.ArgumentList.Arguments),
                _ => throw new NotSupportedException(),
            };
            return SyntaxFactory.ArgumentList(arguments);
        });
    }

    public static Action<DocumentEditor, FluentChainedAssertionEditActionContext> CombineAssertionsWithName(string newName, Func<DocumentEditor, FluentChainedAssertionEditActionContext, ArgumentListSyntax> argumentsGenerator)
    {
        return (editor, context) =>
        {
            var newNameNode = (IdentifierNameSyntax)editor.Generator.IdentifierName(newName);

            var assertionMemberAccess = (MemberAccessExpressionSyntax)context.AssertionAExpression.Expression;

            ArgumentListSyntax allArguments = argumentsGenerator(editor, context);
            InvocationExpressionSyntax newAssertion = context.AssertionAExpression
                .WithExpression(assertionMemberAccess.WithName(newNameNode))
                .WithArgumentList(allArguments);

            editor.ReplaceNode(context.AssertionBExpression, newAssertion);
        };
    }
}
