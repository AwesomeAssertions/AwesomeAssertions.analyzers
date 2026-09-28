using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.Docs.Generator;

namespace AwesomeAssertions.Analyzers.Docs;

public static class Program
{
    public static Task Main(string[] args) => ProgramUtils.RunMain<AwesomeAssertionsDocsGenerator, AwesomeAssertionsDocsVerifier>(args);

    private sealed class AwesomeAssertionsDocsGenerator : DocsGenerator
    {
        protected override Assembly TestAssembly { get; } = typeof(Program).Assembly;
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "AwesomeAssertionsAnalyzerTests.cs");
    }

    private sealed class AwesomeAssertionsDocsVerifier : DocsVerifier
    {
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "AwesomeAssertionsAnalyzerTests.cs");
    }
}