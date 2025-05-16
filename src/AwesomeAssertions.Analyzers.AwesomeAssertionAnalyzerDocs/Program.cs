using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocsGenerator;

namespace AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Task.WhenAll(
            ProgramUtils.RunMain<MsTestDocsGenerator, MsTestDocsVerifier>(args),
            ProgramUtils.RunMain<AwesomeAssertionsDocsGenerator, AwesomeAssertionsDocsVerifier>(args)
        );
    }

    private abstract class BaseDocsDocsGenerator : DocsGenerator
    {
        protected override Assembly TestAssembly { get; } = typeof(Program).Assembly;
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
    }

    private class MsTestDocsGenerator : BaseDocsDocsGenerator
    {
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "MsTestAnalyzerTests.cs");
    }
    private class AwesomeAssertionsDocsGenerator : BaseDocsDocsGenerator
    {
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "AwesomeAssertionsAnalyzerTests.cs");
    }


    private abstract class BaseDocsVerifier : DocsVerifier
    {
        protected override string TestAttribute => "TestMethod"; // Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute
    }

    private class MsTestDocsVerifier : BaseDocsVerifier
    {
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "MsTestAnalyzerTests.cs");
    }
    private class AwesomeAssertionsDocsVerifier : BaseDocsVerifier
    {
        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "AwesomeAssertionsAnalyzerTests.cs");
    }
}