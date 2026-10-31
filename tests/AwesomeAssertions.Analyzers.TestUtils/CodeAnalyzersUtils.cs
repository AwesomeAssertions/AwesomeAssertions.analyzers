using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AwesomeAssertions.Analyzers.TestUtils;

public static class CodeAnalyzersUtils
{
    private static readonly DiagnosticAnalyzer[] AllAnalyzers = CreateAllAnalyzers();

    public static DiagnosticAnalyzer[] GetAllAnalyzers() => AllAnalyzers;

    private static DiagnosticAnalyzer[] CreateAllAnalyzers()
    {
        Assembly assembly = typeof(AwesomeAssertionsAnalyzer).Assembly;
        IEnumerable<Type> analyzersTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(DiagnosticAnalyzer).IsAssignableFrom(type));
        IEnumerable<DiagnosticAnalyzer> analyzers = analyzersTypes.Select(type => (DiagnosticAnalyzer)Activator.CreateInstance(type));

        return analyzers.ToArray();
    }
}
