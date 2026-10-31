using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AwesomeAssertions.Analyzers.BenchmarkTests;

[JsonExporter]
public class AwesomeAssertionsBenchmarks
{
    private CompilationWithAnalyzers minimalCompilationWithAnalyzersObjectStatement;
    private CompilationWithAnalyzers smallCompilationWithAnalyzersStringAssertions;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        minimalCompilationWithAnalyzersObjectStatement = await CreateCompilationFromAsync(GenerateCode.ObjectStatement("actual.Should().Equals(expected);"));
        smallCompilationWithAnalyzersStringAssertions = await CreateCompilationFromAsync(
            GenerateCode.StringAssertion("actual.StartsWith(expected).Should().BeTrue();"),
            GenerateCode.StringAssertion("actual.EndsWith(expected).Should().BeTrue();"),
            GenerateCode.StringAssertion("actual.Should().NotBeNull().And.NotBeEmpty();"),
            GenerateCode.StringAssertion("string.IsNullOrEmpty(actual).Should().BeTrue();"),
            GenerateCode.StringAssertion("string.IsNullOrEmpty(actual).Should().BeFalse();"),
            GenerateCode.StringAssertion("string.IsNullOrWhiteSpace(actual).Should().BeTrue();"),
            GenerateCode.StringAssertion("string.IsNullOrWhiteSpace(actual).Should().BeFalse();"),
            GenerateCode.StringAssertion("actual.Length.Should().Be(k);")
        );
    }

    [Benchmark]
    public Task MinimalCompilation_SingleSource_ObjectStatement_Analyzing()
    {
        return minimalCompilationWithAnalyzersObjectStatement.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }

    [Benchmark]
    public Task SmallCompilation_MultipleSources_StringAssertions_Analyzing()
    {
        return smallCompilationWithAnalyzersStringAssertions.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
    }

    private static async Task<CompilationWithAnalyzers> CreateCompilationFromAsync(params string[] sources)
    {
        Project project = CsProjectGenerator.CreateProject(new() { Sources = sources });
        Compilation compilation = (await project.GetCompilationAsync())
            ?? throw new InvalidOperationException("Compilation is null");

        return compilation.WithOptions(compilation.Options.WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>(StringComparer.Ordinal)
        {
            ["CS1701"] = ReportDiagnostic.Suppress, // Binding redirects
            ["CS1702"] = ReportDiagnostic.Suppress,
            ["CS1705"] = ReportDiagnostic.Suppress,
            ["CS8019"] = ReportDiagnostic.Suppress // Unnecessary using directive
        })).WithAnalyzers([.. CodeAnalyzersUtils.GetAllAnalyzers()]);
    }
}
