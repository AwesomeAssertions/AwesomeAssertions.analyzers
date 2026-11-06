using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace AwesomeAssertions.Analyzers.Tips;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncVoidCodeFixProvider))]
#pragma warning disable RS1016 // Code fix providers should provide FixAll support: Fix later
public sealed class AsyncVoidCodeFixProvider : CodeFixProvider
#pragma warning restore RS1016 // Code fix providers should provide FixAll support
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Constants.CodeSmell.AsyncVoid);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        return Task.CompletedTask;
    }
}
