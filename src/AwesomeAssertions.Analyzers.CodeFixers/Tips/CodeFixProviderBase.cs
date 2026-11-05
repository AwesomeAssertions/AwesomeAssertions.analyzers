using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;

namespace AwesomeAssertions.Analyzers.Tips;

public abstract class CodeFixProviderBase<TTestContext> : CodeFixProvider
    where TTestContext : class
{
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    protected abstract string Title { get; }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

        TTestContext testContext = CreateTestContext(semanticModel);
        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan);
            if (node is not InvocationExpressionSyntax invocationExpression)
            {
                continue;
            }

            IOperation operation = semanticModel.GetOperation(invocationExpression, context.CancellationToken);
            if (operation is not IInvocationOperation invocation)
            {
                continue;
            }

            CreateChangedDocument fix = TryComputeFix(invocation, context, testContext, diagnostic);
            if (fix is not null)
            {
                context.RegisterCodeFix(CodeAction.Create(Title, fix, Title), diagnostic);
            }
        }
    }

    protected abstract TTestContext CreateTestContext(SemanticModel semanticModel);

    protected abstract CreateChangedDocument TryComputeFix(IInvocationOperation invocation, CodeFixContext context, TTestContext t, Diagnostic diagnostic);
}
