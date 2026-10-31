using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class ShouldEqualsTests
{
    [TestMethod]
    [Implemented]
    public Task ShouldEquals_TestAnalyzer()
        => VerifyCSharpDiagnosticExpressionBody("actual.Should().Equals(expected);", DiagnosticMetadata.ShouldBe_ShouldEquals);

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ShouldBe_ObjectType_TestCodeFix()
    {
        var oldSource = GenerateCode.ObjectStatement("actual.Should().Equals(expected);");
        var newSource = GenerateCode.ObjectStatement("actual.Should().Be(expected);");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_NestedInsideIfBlock_TestAnalyzer()
        => VerifyCSharpDiagnosticExpressionBody("if(true) { actual.Should().Equals(expected); }", 10, 24, DiagnosticMetadata.ShouldBe_ShouldEquals);

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_NestedInsideIfBlock_ShouldBe_ObjectType_TestCodeFix()
    {
        var oldSource = GenerateCode.ObjectStatement("if(true) { actual.Should().Equals(expected); }");
        var newSource = GenerateCode.ObjectStatement("if(true) { actual.Should().Be(expected); }");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_NestedInsideWhileBlock_TestAnalyzer()
        => VerifyCSharpDiagnosticExpressionBody("while(true) { actual.Should().Equals(expected); }", 10, 27, DiagnosticMetadata.ShouldBe_ShouldEquals);

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_NestedInsideWhileBlock_ShouldBe_ObjectType_TestCodeFix()
    {
        var oldSource = GenerateCode.ObjectStatement("while(true) { actual.Should().Equals(expected); }");
        var newSource = GenerateCode.ObjectStatement("while(true) { actual.Should().Be(expected); }");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ActualIsMethodInvoaction_TestAnalyzer()
        => VerifyCSharpDiagnosticExpressionBody("object ResultSupplier() { return null; } \n"
        + "ResultSupplier().Should().Equals(expected);", 11, 0, DiagnosticMetadata.ShouldBe_ShouldEquals);

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ActualIsMethodInvoaction_ShouldBe_ObjectType_TestCodeFix()
    {
        const string methodInvocation = "object ResultSupplier() { return null; } \n";
        var oldSource = GenerateCode.ObjectStatement(methodInvocation + "ResultSupplier().Should().Equals(expected);");
        var newSource = GenerateCode.ObjectStatement(methodInvocation + "ResultSupplier().Should().Be(expected);");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ShouldBe_NumberType_TestCodeFix()
    {
        var oldSource = GenerateCode.DoubleAssertion("actual.Should().Equals(expected);");
        var newSource = GenerateCode.DoubleAssertion("actual.Should().Be(expected);");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ShouldBe_StringType_TestCodeFix()
    {
        var oldSource = GenerateCode.StringAssertion("actual.Should().Equals(expected);");
        var newSource = GenerateCode.StringAssertion("actual.Should().Be(expected);");

        return VerifyFix(oldSource, newSource);
    }

    [TestMethod]
    [Implemented]
    public Task ShouldEquals_ShouldEqual_EnumerableType_TestCodeFix()
    {
        var oldSource = GenerateCode.GenericIListCodeBlockAssertion("actual.Should().Equals(expected);");
        var newSource = GenerateCode.GenericIListCodeBlockAssertion("actual.Should().Equal(expected);");

        return VerifyFix(oldSource, newSource);
    }

    private Task VerifyCSharpDiagnosticExpressionBody(string sourceAssertion, DiagnosticMetadata metadata) => VerifyCSharpDiagnosticExpressionBody(sourceAssertion, 10, 13, metadata);

    private Task VerifyCSharpDiagnosticExpressionBody(string sourceAssertion, int line, int column, DiagnosticMetadata metadata)
    {
        var source = GenerateCode.ObjectStatement(sourceAssertion);
        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source, new DiagnosticResult
        {
            Id = AwesomeAssertionsAnalyzer.DiagnosticId,
            Message = metadata.Message,
            VisitorName = metadata.Name,
            Locations =
            [
                new("Test0.cs", line, column)
            ],
            Severity = DiagnosticSeverity.Info
        });
    }

    private static Task VerifyFix(string oldSource, string newSource)
        => DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
            .WithCodeFixProvider<AwesomeAssertionsCodeFixProvider>()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
}
