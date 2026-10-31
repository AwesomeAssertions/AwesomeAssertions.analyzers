using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.Tips.Editing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;

namespace AwesomeAssertions.Analyzers.Tips;

public static class DocumentEditorUtils
{
    public static CreateChangedDocument RenameMethodToSubjectShouldAssertion(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
    {
        return token => RewriteExpressionCore(invocation, [
            ..Array.ConvertAll(argumentsToRemove, arg => EditAction.RemoveInvocationArgument(arg)),
            EditAction.SubjectShouldAssertion(subjectIndex, newName)
        ], context, token);
    }

    public static CreateChangedDocument RenameGenericMethodToSubjectShouldGenericAssertion(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
        => RenameMethodToSubjectShouldGenericAssertion(invocation, invocation.TargetMethod.TypeArguments, context, newName, subjectIndex, argumentsToRemove);

    public static CreateChangedDocument RenameMethodToSubjectShouldGenericAssertion(IInvocationOperation invocation, ImmutableArray<ITypeSymbol> genericTypes, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
    {
        return token => RewriteExpressionCore(invocation, [
            ..Array.ConvertAll(argumentsToRemove, arg => EditAction.RemoveInvocationArgument(arg)),
            EditAction.SubjectShouldGenericAssertion(subjectIndex, newName, genericTypes)
         ], context, token);
    }

    public static CreateChangedDocument RenameMethodToSubjectShouldAssertionWithOptionsLambda(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int optionsIndex)
    {
        return token => RewriteExpressionCore(invocation, [
            EditAction.SubjectShouldAssertion(subjectIndex, newName),
            EditAction.CreateEquivalencyAssertionOptionsLambda(optionsIndex)
        ], context, token);
    }

    public static CreateChangedDocument RewriteExpression(IInvocationOperation invocation, Action<EditActionContext>[] actions, CodeFixContext context)
        => token => RewriteExpressionCore(invocation, actions, context, token);

    private static async Task<Document> RewriteExpressionCore(IInvocationOperation invocation, Action<EditActionContext>[] actions, CodeFixContext context, CancellationToken cancellationToken)
    {
        var invocationExpression = (InvocationExpressionSyntax)invocation.Syntax;

        DocumentEditor editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
        var editActionContext = new EditActionContext(editor, invocationExpression);

        foreach (Action<EditActionContext> action in actions)
        {
            action(editActionContext);
        }

        return editor.GetChangedDocument();
    }
}
