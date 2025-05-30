﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using AwesomeAssertions.Analyzers.TestUtils;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AwesomeAssertions.Analyzers.BenchmarkTests
{
    [SimpleJob(RuntimeMoniker.Net60, baseline: true)]
    // Net8.0 has a larger regression in one benchmark (https://github.com/AwesomeAssertions/AwesomeAssertions.analyzers/discussions/14),
    // and since the other projects doesn't use net9.0, we leave it commented for the moment.
    // [SimpleJob(RuntimeMoniker.Net90)]
    [JsonExporter]
    public class AwesomeAssertionsBenchmarks
    {
        private CompilationWithAnalyzers MinimalCompilationWithAnalyzers_ObjectStatement;
        private CompilationWithAnalyzers SmallCompilationWithAnalyzers_StringAssertions;

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            MinimalCompilationWithAnalyzers_ObjectStatement = await CreateCompilationFromAsync(GenerateCode.ObjectStatement("actual.Should().Equals(expected);"));
            SmallCompilationWithAnalyzers_StringAssertions = await CreateCompilationFromAsync(
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
            return MinimalCompilationWithAnalyzers_ObjectStatement.GetAnalyzerDiagnosticsAsync();
        }

        [Benchmark]
        public Task SmallCompilation_MultipleSources_StringAssertions_Analyzing()
        {
            return SmallCompilationWithAnalyzers_StringAssertions.GetAnalyzerDiagnosticsAsync();
        }

        private async Task<CompilationWithAnalyzers> CreateCompilationFromAsync(params string[] sources)
        {
            var project = CsProjectGenerator.CreateProject(sources);
            var compilation = await project.GetCompilationAsync();

            if (compilation is null)
            {
                throw new InvalidOperationException("Compilation is null");
            }

            return compilation.WithOptions(compilation.Options.WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
            {
                ["CS1701"] = ReportDiagnostic.Suppress, // Binding redirects
                ["CS1702"] = ReportDiagnostic.Suppress,
                ["CS1705"] = ReportDiagnostic.Suppress,
                ["CS8019"] = ReportDiagnostic.Suppress // TODO: Unnecessary using directive
            })).WithAnalyzers(CodeAnalyzersUtils.GetAllAnalyzers().ToImmutableArray());
        }
    }
}