using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocsGenerator;

namespace AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs;

public static class Program
{
    public static Task Main(string[] args) => ProgramUtils.RunMain<MsTestDocsGenerator, MsTestDocsVerifier>(args);

    private sealed class MsTestDocsGenerator : DocsGenerator
    {
        protected override Assembly TestAssembly { get; } = typeof(Program).Assembly;
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "MsTestAnalyzerTests.cs");
    }
    
    private sealed class MsTestDocsVerifier : DocsVerifier
    {
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "MsTestAnalyzerTests.cs");
    }
}
