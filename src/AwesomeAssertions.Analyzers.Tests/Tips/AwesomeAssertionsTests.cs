using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests
{
    [TestClass]
    public class AwesomeAssertionsTests
    {
        [TestMethod]
        [Implemented]
        public void ShouldNotReturnEarly_WhenAwesomeAssertionsIsNotLoaded()
        {
            const string source = @"
namespace TestProject
{
    public class TestClass
    {
    }
}";

            DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
                .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
                .WithSources(source)
            );
        }

        [TestMethod]
        [Implemented]
        public void ShouldNotReturnEarly_WhenShouldInvocationIsNotFromAwesomeAssertions()
        {
            const string source = @"
namespace TestProject
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test = new TestClassA();
            test.Length.Should().BeTrue();
        }
    }
    public class TestClassA
    {
        public TestClassA Length => this;
        public TestClassB Should() => new TestClassB();
    }
    public class TestClassB
    {
        public void BeTrue() { }
    }
}";

            DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
                .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
                .WithSources(source)
            );
        }

        [TestMethod]
        [Implemented]
        public void ShouldAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsNotInScope_ForXunit()
            => ShouldAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsNotInScope<XunitCodeFixProvider>("True", "using Xunit;", PackageReference.XunitAssert_2_5_1);

        [TestMethod]
        [Implemented]
        public void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInGlobalScope_ForXunit()
            => ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInGlobalScope<XunitCodeFixProvider>("True", "using Xunit;", PackageReference.XunitAssert_2_5_1);

        [TestMethod]
        [Implemented]
        public void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInAnyScope_ForXunit()
            => ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInAnyScope<XunitCodeFixProvider>("True", "using Xunit;", PackageReference.XunitAssert_2_5_1);

        [TestMethod]
        [Implemented]
        public void ShouldAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsNotInScope_ForMsTest()
            => ShouldAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsNotInScope<MsTestCodeFixProvider>("IsTrue", "using Microsoft.VisualStudio.TestTools.UnitTesting;", PackageReference.MSTestTestFramework_3_1_1);

        [TestMethod]
        [Implemented]
        public void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInGlobalScope_ForMsTest()
            => ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInGlobalScope<MsTestCodeFixProvider>("IsTrue", "using Microsoft.VisualStudio.TestTools.UnitTesting;", PackageReference.MSTestTestFramework_3_1_1);

        [TestMethod]
        [Implemented]
        public void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInAnyScope_ForMsTest()
            => ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInAnyScope<MsTestCodeFixProvider>("IsTrue", "using Microsoft.VisualStudio.TestTools.UnitTesting;", PackageReference.MSTestTestFramework_3_1_1);

        private void ShouldAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsNotInScope<TCodeFixProvider>(string assertTrue, string usingDirective, PackageReference testingLibraryReference) where TCodeFixProvider : CodeFixProvider, new()
        {
            string source = $@"
{usingDirective}
namespace TestProject
{{
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            Assert.{assertTrue}(subject);
        }}
    }}
}}";
            string newSource = @$"
using AwesomeAssertions;
{usingDirective}
namespace TestProject
{{
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            subject.Should().BeTrue();
        }}
    }}
}}";
            DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
                .WithDiagnosticAnalyzer<AssertAnalyzer>()
                .WithCodeFixProvider<TCodeFixProvider>()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest, testingLibraryReference)
                .WithSources(source)
                .WithFixedSources(newSource)
            );
        }

        private void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInGlobalScope<TCodeFixProvider>(string assertTrue, string usingDirective, PackageReference testingLibraryReference) where TCodeFixProvider : CodeFixProvider, new()
        {
            string source = $@"
{usingDirective}
namespace TestProject
{{
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            Assert.{assertTrue}(subject);
        }}
    }}
}}";
            const string globalUsings = "global using AwesomeAssertions;";
            string newSource = @$"
{usingDirective}
namespace TestProject
{{
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            subject.Should().BeTrue();
        }}
    }}
}}";

            DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
                .WithDiagnosticAnalyzer<AssertAnalyzer>()
                .WithCodeFixProvider<TCodeFixProvider>()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest, testingLibraryReference)
                .WithSources(source, globalUsings)
                .WithFixedSources(newSource)
            );
        }

        private void ShouldNotAddAwesomeAssertionsUsing_WhenAwesomeAssertionIsInAnyScope<TCodeFixProvider>(string assertTrue, string usingDirective, PackageReference testingLibraryReference) where TCodeFixProvider : CodeFixProvider, new()
        {
            string source = $@"
{usingDirective}
namespace TestProject
{{
    using AwesomeAssertions;
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            Assert.{assertTrue}(subject);
        }}
    }}
}}";
            string newSource = @$"
{usingDirective}
namespace TestProject
{{
    using AwesomeAssertions;
    public class TestClass
    {{
        public void TestMethod(bool subject)
        {{
            subject.Should().BeTrue();
        }}
    }}
}}";

            DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
                .WithDiagnosticAnalyzer<AssertAnalyzer>()
                .WithCodeFixProvider<TCodeFixProvider>()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest, testingLibraryReference)
                .WithSources(source)
                .WithFixedSources(newSource)
            );
        }
    }
}