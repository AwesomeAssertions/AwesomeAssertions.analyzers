using System.Collections.Generic;
using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AwesomeAssertions.Analyzers.Tests;

public sealed class CodeFixVerifierArguments : CsProjectArguments
{
    public List<string> FixedSources { get; } = [];

    public List<DiagnosticAnalyzer> DiagnosticAnalyzers { get; } = [];

    public List<CodeFixProvider> CodeFixProviders { get; } = [];

    public CodeFixVerifierArguments WithDiagnosticAnalyzer<TDiagnosticAnalyzer>()
        where TDiagnosticAnalyzer : DiagnosticAnalyzer, new()
    {
        DiagnosticAnalyzers.Add(new TDiagnosticAnalyzer());
        return this;
    }

    public CodeFixVerifierArguments WithCodeFixProvider<TCodeFixProvider>()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        CodeFixProviders.Add(new TCodeFixProvider());
        return this;
    }

    public CodeFixVerifierArguments WithFixedSources(params string[] fixedSources)
    {
        FixedSources.AddRange(fixedSources);
        return this;
    }
}
