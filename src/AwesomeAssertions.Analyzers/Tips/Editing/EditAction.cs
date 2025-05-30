using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers;

public static class EditAction
{
    public static Action<EditActionContext> RemoveNode(SyntaxNode node)
        => context => context.Editor.RemoveNode(node);

    public static Action<EditActionContext> RemoveInvocationArgument(int argumentIndex)
        => context => context.Editor.RemoveNode(context.InvocationExpression.ArgumentList.Arguments[argumentIndex]);

    public static Action<EditActionContext> SubjectShouldAssertion(int argumentIndex, string assertion)
        => context => new SubjectShouldAssertionAction(argumentIndex, assertion).Apply(context);

    public static Action<EditActionContext> ReplaceAssertionArgument(int index, Func<SyntaxGenerator, SyntaxNode> expressionFactory)
        => context =>
    {
        var argument = context.InvocationExpression.ArgumentList.Arguments[index];
        var newArgumentExpression = (ExpressionSyntax)expressionFactory(context.Editor.Generator);
        context.Editor.ReplaceNode(argument.Expression, newArgumentExpression);
    };

    public static Action<EditActionContext> SubjectShouldGenericAssertion(int argumentIndex, string assertion, ImmutableArray<ITypeSymbol> genericTypes)
        => context => new SubjectShouldGenericAssertionAction(argumentIndex, assertion, genericTypes).Apply(context);

    public static Action<EditActionContext> CreateEquivalencyAssertionOptionsLambda(int optionsIndex)
        => context => new CreateEquivalencyAssertionOptionsLambdaAction(optionsIndex).Apply(context.Editor, context.InvocationExpression);

    public static Action<EditActionContext> AddArgumentToAssertionArguments(int index, Func<SyntaxGenerator, SyntaxNode> expressionFactory)
        => context =>
    {
        var argument = (ArgumentSyntax)context.Editor.Generator.Argument(expressionFactory(context.Editor.Generator));
        var arguments = context.AwesomeAssertion.ArgumentList.Arguments.Insert(index, argument);
        context.Editor.ReplaceNode(context.InvocationExpression.ArgumentList, context.InvocationExpression.ArgumentList.WithArguments(arguments));
    };
}