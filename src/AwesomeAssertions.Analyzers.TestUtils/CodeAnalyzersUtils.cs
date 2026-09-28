using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace AwesomeAssertions.Analyzers.TestUtils;

public static class CodeAnalyzersUtils
{
    private static readonly DiagnosticAnalyzer[] AllAnalyzers = CreateAllAnalyzers();

    public static DiagnosticAnalyzer[] GetAllAnalyzers() => AllAnalyzers;

    private static DiagnosticAnalyzer[] CreateAllAnalyzers()
    {
        var assembly = typeof(AwesomeAssertionsAnalyzer).Assembly;
        var analyzersTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(type));
        var analyzers = analyzersTypes.Select(type => (DiagnosticAnalyzer)Activator.CreateInstance(type));

        return analyzers.ToArray();
    }
}