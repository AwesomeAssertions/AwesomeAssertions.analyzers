using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.Docs.Generator;

namespace AwesomeAssertions.Analyzers.Docs;

public static class Program
{
    public static Task Main(string[] args) => ProgramUtils.RunMain<Nunit4DocsGenerator, Nunit4DocsVerifier>(args);

    private sealed class Nunit4DocsGenerator : DocsGenerator
    {
        protected override Assembly TestAssembly { get; } = typeof(Program).Assembly;

        protected override string TestAttribute => "Test"; // NUnit.Framework.TestAttribute

        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "Nunit4AnalyzerTests.cs");

        protected override void ResetTestFramework()
        {
            Type testContext = typeof(NUnit.Framework.Internal.TestExecutionContext);
            testContext.GetProperty("CurrentContext").SetValue(null, new NUnit.Framework.Internal.TestExecutionContext.AdhocContext());
        }
    }

    private sealed class Nunit4DocsVerifier : DocsVerifier
    {
        protected override string TestAttribute => "Test"; // NUnit.Framework.TestAttribute

        protected override string TestFile => Path.Join(Environment.CurrentDirectory, "Nunit4AnalyzerTests.cs");
    }
}
