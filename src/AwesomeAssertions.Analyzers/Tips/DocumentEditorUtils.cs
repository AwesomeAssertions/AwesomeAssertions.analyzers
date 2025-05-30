using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;

namespace AwesomeAssertions.Analyzers;

public class DocumentEditorUtils
{
    public static CreateChangedDocument RenameMethodToSubjectShouldAssertion(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
    {
        return ctx => RewriteExpressionCore(invocation, [
            ..Array.ConvertAll(argumentsToRemove, arg => EditAction.RemoveInvocationArgument(arg)),
            EditAction.SubjectShouldAssertion(subjectIndex, newName)
        ], context, ctx);
    }

    public static CreateChangedDocument RenameGenericMethodToSubjectShouldGenericAssertion(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
        => RenameMethodToSubjectShouldGenericAssertion(invocation, invocation.TargetMethod.TypeArguments, context, newName, subjectIndex, argumentsToRemove);
    public static CreateChangedDocument RenameMethodToSubjectShouldGenericAssertion(IInvocationOperation invocation, ImmutableArray<ITypeSymbol> genericTypes, CodeFixContext context, string newName, int subjectIndex, int[] argumentsToRemove)
    {
        return ctx => RewriteExpressionCore(invocation, [
            ..Array.ConvertAll(argumentsToRemove, arg => EditAction.RemoveInvocationArgument(arg)),
            EditAction.SubjectShouldGenericAssertion(subjectIndex, newName, genericTypes)
         ], context, ctx);
    }

    public static CreateChangedDocument RenameMethodToSubjectShouldAssertionWithOptionsLambda(IInvocationOperation invocation, CodeFixContext context, string newName, int subjectIndex, int optionsIndex)
    {
        return ctx => RewriteExpressionCore(invocation, [
            EditAction.SubjectShouldAssertion(subjectIndex, newName),
            EditAction.CreateEquivalencyAssertionOptionsLambda(optionsIndex)
        ], context, ctx);
    }

    public static CreateChangedDocument RewriteExpression(IInvocationOperation invocation, Action<EditActionContext>[] actions, CodeFixContext context)
        => ctx => RewriteExpressionCore(invocation, actions, context, ctx);

    private static async Task<Document> RewriteExpressionCore(IInvocationOperation invocation, Action<EditActionContext>[] actions, CodeFixContext context, CancellationToken cancellationToken)
    {
        var invocationExpression = (InvocationExpressionSyntax)invocation.Syntax;

        var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
        var editActionContext = new EditActionContext(editor, invocationExpression);

        foreach (var action in actions)
        {
            action(editActionContext);
        }

        return editor.GetChangedDocument();
    }
}

public class EditActionContext(DocumentEditor editor, InvocationExpressionSyntax invocationExpression) {
    public DocumentEditor Editor { get; } = editor;
    public InvocationExpressionSyntax InvocationExpression { get; } = invocationExpression;
    
    public InvocationExpressionSyntax AwesomeAssertion { get; set; }
}