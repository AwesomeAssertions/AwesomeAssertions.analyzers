using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace AwesomeAssertions.Analyzers.Tips;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncVoidCodeFix)), Shared]
public class AsyncVoidCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.CodeSmell.AsyncVoid);
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        return Task.CompletedTask;
    }
}
