using System;
using AwesomeAssertions.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;

namespace AwesomeAssertions.Analyzers.Tips;

public sealed partial class AwesomeAssertionsCodeFixProvider
{
    // <subject>.<invocationBeforeShould>().Should().<assertion>(<arguments>)
    private static CreateChangedDocument RewriteAwesomeAssertion(IInvocationOperation assertion, CodeFixContext context, Action<DocumentEditor, AwesomeAssertionEditActionContext>[] actions)
    {
        var assertionExpression = (InvocationExpressionSyntax)assertion.Syntax;

        assertion.TryGetFirstDescendent(out IInvocationOperation should);
        IOperation subject = should?.Arguments[0].Value.UnwrapConversion();

        IInvocationOperation invocationBeforeShould = default;
        should?.TryGetFirstDescendent(out invocationBeforeShould);

        var actionContext = new AwesomeAssertionEditActionContext(assertion, assertionExpression, should, subject, invocationBeforeShould);

        return async token =>
        {
            DocumentEditor editor = await DocumentEditor.CreateAsync(context.Document, token);
            foreach (Action<DocumentEditor, AwesomeAssertionEditActionContext> action in actions)
            {
                action(editor, actionContext);
            }

            return editor.GetChangedDocument();
        };
    }

    // <subject>.Should().<assertionA>(argumentsA).<andOrWhich>.<assertionB>(<argumentsB>)
    private static CreateChangedDocument RewriteFluentChainedAssertion(IInvocationOperation assertionB, CodeFixContext context, Action<DocumentEditor, FluentChainedAssertionEditActionContext>[] actions)
    {
        var assertionExpressionB = (InvocationExpressionSyntax)assertionB.Syntax;

        assertionB.TryGetFirstDescendent(out IPropertyReferenceOperation andOrWhich);

        andOrWhich.TryGetFirstDescendent(out IInvocationOperation assertionA);

        var assertionExpressionA = (InvocationExpressionSyntax)assertionA.Syntax;

        assertionA.TryGetFirstDescendent(out IInvocationOperation should);

        IOperation subject = should?.Arguments[0].Value;

        var actionContext = new FluentChainedAssertionEditActionContext(assertionA, assertionExpressionA, andOrWhich, assertionB, assertionExpressionB, should, subject);

        return async token =>
        {
            DocumentEditor editor = await DocumentEditor.CreateAsync(context.Document, token);
            foreach (Action<DocumentEditor, FluentChainedAssertionEditActionContext> action in actions)
            {
                action(editor, actionContext);
            }

            return editor.GetChangedDocument();
        };
    }
}
