using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class NumericTests
{
    [TestMethod]
    [AssertionDiagnostic("actual.Should().BeGreaterThan(0{0});")]
    [AssertionDiagnostic("actual.Should().BeGreaterThan(0{0}).ToString();")]
    [Implemented]
    public Task NumericShouldBePositive_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBePositive_ShouldBeGreaterThan);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeGreaterThan(0{0});",
        newAssertion: "actual.Should().BePositive({0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeGreaterThan(0{0}).ToString();",
        newAssertion: "actual.Should().BePositive({0}).ToString();")]
    [Implemented]
    public Task NumericShouldBePositive_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().BeLessThan(0{0});")]
    [AssertionDiagnostic("actual.Should().BeLessThan(0{0}).ToString();")]
    [Implemented]
    public Task NumericShouldBeNegative_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeNegative_ShouldBeLessThan);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeLessThan(0{0});",
        newAssertion: "actual.Should().BeNegative({0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeLessThan(0{0}).ToString();",
        newAssertion: "actual.Should().BeNegative({0}).ToString();")]
    [Implemented]
    public Task NumericShouldBeNegative_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().BeGreaterThanOrEqualTo(lower{0}).And.BeLessThanOrEqualTo(upper);")]
    [AssertionDiagnostic("actual.Should().BeGreaterThanOrEqualTo(lower).And.BeLessThanOrEqualTo(upper{0});")]
    [Implemented]
    public Task NumericShouldBeInRange_BeGreaterThanOrEqualToAndBeLessThanOrEqualTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeInRange_BeGreaterThanOrEqualToAndBeLessThanOrEqualTo);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().BeLessThanOrEqualTo(upper{0}).And.BeGreaterThanOrEqualTo(lower);")]
    [AssertionDiagnostic("actual.Should().BeLessThanOrEqualTo(upper).And.BeGreaterThanOrEqualTo(lower{0});")]
    [Implemented]
    public Task NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo);

    [TestMethod]
    [DataRow("actual.Should().BeLessThanOrEqualTo(upper, \"because reason 1\").And.BeGreaterThanOrEqualTo(lower, \"because reason 2\");")]
    [DataRow("actual.Should().BeGreaterThanOrEqualTo(lower, \"because reason 1\").And.BeLessThanOrEqualTo(upper, \"because reason 2\");")]
    [Implemented]
    public Task NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo_WithMessagesInBothAssertions_TestAnalyzer(string assertion)
    {
        return Task.WhenAll(
            VerifyNoDiagnostic("double"),
            VerifyNoDiagnostic("float"),
            VerifyNoDiagnostic("decimal"));

        Task VerifyNoDiagnostic(string type)
        {
            var source = GenerateCode.NumericAssertion(assertion, type);
            return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
                .WithSources(source)
                .WithAllAnalyzers()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            );
        }
    }

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeGreaterThanOrEqualTo(lower{0}).And.BeLessThanOrEqualTo(upper);",
        newAssertion: "actual.Should().BeInRange(lower, upper{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeGreaterThanOrEqualTo(lower).And.BeLessThanOrEqualTo(upper{0});",
        newAssertion: "actual.Should().BeInRange(lower, upper{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeLessThanOrEqualTo(upper{0}).And.BeGreaterThanOrEqualTo(lower);",
        newAssertion: "actual.Should().BeInRange(lower, upper{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().BeLessThanOrEqualTo(upper).And.BeGreaterThanOrEqualTo(lower{0});",
        newAssertion: "actual.Should().BeInRange(lower, upper{0});")]
    [NotImplemented]
    [Ignore("Reason unknown")]
    public Task NumericShouldBeInRange_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Math.Abs(expected - actual).Should().BeLessThanOrEqualTo(delta{0});")]
    [Implemented]
    public Task NumericShouldBeApproximately_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeApproximately_MathAbsShouldBeLessThanOrEqualTo);

    [TestMethod]
    [AssertionDiagnostic("(Math.Abs(expected - actual) + 1).Should().BeLessThanOrEqualTo(delta{0});")]
    [Implemented]
    public Task NumericShouldBeApproximately_TestNoAnalyzer(string assertion) => DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(GenerateCode.NumericAssertion(assertion, "double"));

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Math.Abs(expected - actual).Should().BeLessThanOrEqualTo(delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [Implemented]
    public Task NumericShouldBeApproximately_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    private Task VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata)
    {
        return Task.WhenAll(
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "double"),
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "float"),
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "decimal"));
    }

    private Task VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata, string numericType)
    {
        var source = GenerateCode.NumericAssertion(sourceAssertion, numericType);

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            .WithExpectedDiagnostics(new DiagnosticResult
            {
                Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                Message = metadata.Message,
                VisitorName = metadata.Name,
                Locations =
                [
                    new("Test0.cs", 10, 13)
                ],
                Severity = DiagnosticSeverity.Info
            })
        );
    }

    private Task VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion)
    {
        return Task.WhenAll(
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "double"),
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "float"),
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "decimal"));
    }

    private static Task VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion, string numericType)
    {
        var oldSource = GenerateCode.NumericAssertion(oldSourceAssertion, numericType);
        var newSource = GenerateCode.NumericAssertion(newSourceAssertion, numericType);

        return DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithCodeFixProvider<AwesomeAssertionsCodeFixProvider>()
            .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
    }
}
