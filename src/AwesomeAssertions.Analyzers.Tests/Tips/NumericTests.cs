using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests
{
    [TestClass]
    public class NumericTests
    {
        [TestMethod]
        [AssertionDiagnostic("actual.Should().BeGreaterThan(0{0});")]
        [AssertionDiagnostic("actual.Should().BeGreaterThan(0{0}).ToString();")]
        [Implemented]
        public void NumericShouldBePositive_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBePositive_ShouldBeGreaterThan);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().BeGreaterThan(0{0});",
            newAssertion: "actual.Should().BePositive({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().BeGreaterThan(0{0}).ToString();",
            newAssertion: "actual.Should().BePositive({0}).ToString();")]
        [Implemented]
        public void NumericShouldBePositive_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("actual.Should().BeLessThan(0{0});")]
        [AssertionDiagnostic("actual.Should().BeLessThan(0{0}).ToString();")]
        [Implemented]
        public void NumericShouldBeNegative_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeNegative_ShouldBeLessThan);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().BeLessThan(0{0});",
            newAssertion: "actual.Should().BeNegative({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().BeLessThan(0{0}).ToString();",
            newAssertion: "actual.Should().BeNegative({0}).ToString();")]
        [Implemented]
        public void NumericShouldBeNegative_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("actual.Should().BeGreaterThanOrEqualTo(lower{0}).And.BeLessThanOrEqualTo(upper);")]
        [AssertionDiagnostic("actual.Should().BeGreaterThanOrEqualTo(lower).And.BeLessThanOrEqualTo(upper{0});")]
        [Implemented]
        public void NumericShouldBeInRange_BeGreaterThanOrEqualToAndBeLessThanOrEqualTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeInRange_BeGreaterThanOrEqualToAndBeLessThanOrEqualTo);

        [TestMethod]
        [AssertionDiagnostic("actual.Should().BeLessThanOrEqualTo(upper{0}).And.BeGreaterThanOrEqualTo(lower);")]
        [AssertionDiagnostic("actual.Should().BeLessThanOrEqualTo(upper).And.BeGreaterThanOrEqualTo(lower{0});")]
        [Implemented]
        public void NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo);

        [TestMethod]
        [DataRow("actual.Should().BeLessThanOrEqualTo(upper, \"because reason 1\").And.BeGreaterThanOrEqualTo(lower, \"because reason 2\");")]
        [DataRow("actual.Should().BeGreaterThanOrEqualTo(lower, \"because reason 1\").And.BeLessThanOrEqualTo(upper, \"because reason 2\");")]
        [Implemented]
        public void NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo_WithMessagesInBothAssertions_TestAnalyzer(string assertion)
        {
            verifyNoDiagnostic("double");
            verifyNoDiagnostic("float");
            verifyNoDiagnostic("decimal");

            void verifyNoDiagnostic(string type)
            {
                var source = GenerateCode.NumericAssertion(assertion, type);
                DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
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
        [NotImplemented, Ignore]
        public void NumericShouldBeInRange_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("Math.Abs(expected - actual).Should().BeLessThanOrEqualTo(delta{0});")]
        [Implemented]
        public void NumericShouldBeApproximately_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.NumericShouldBeApproximately_MathAbsShouldBeLessThanOrEqualTo);

        [TestMethod]
        [AssertionDiagnostic("(Math.Abs(expected - actual) + 1).Should().BeLessThanOrEqualTo(delta{0});")]
        [Implemented]
        public void NumericShouldBeApproximately_TestNoAnalyzer(string assertion) => DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(GenerateCode.NumericAssertion(assertion, "double"));

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "Math.Abs(expected - actual).Should().BeLessThanOrEqualTo(delta{0});",
            newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
        [Implemented]
        public void NumericShouldBeApproximately_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        private void VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata)
        {
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "double");
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "float");
            VerifyCSharpDiagnostic(sourceAssertion, metadata, "decimal");
        }

        private void VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata, string numericType)
        {
            var source = GenerateCode.NumericAssertion(sourceAssertion, numericType);

            DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
                .WithSources(source)
                .WithAllAnalyzers()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
                .WithExpectedDiagnostics(new DiagnosticResult
                {
                    Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                    Message = metadata.Message,
                    VisitorName = metadata.Name,
                    Locations = new DiagnosticResultLocation[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 10, 13)
                    },
                    Severity = DiagnosticSeverity.Info
                })
            );
        }

        private void VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion)
        {
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "double");
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "float");
            VerifyCSharpFix(oldSourceAssertion, newSourceAssertion, "decimal");
        }

        private void VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion, string numericType)
        {
            var oldSource = GenerateCode.NumericAssertion(oldSourceAssertion, numericType);
            var newSource = GenerateCode.NumericAssertion(newSourceAssertion, numericType);

            DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
                .WithCodeFixProvider<AwesomeAssertionsCodeFixProvider>()
                .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
                .WithSources(oldSource)
                .WithFixedSources(newSource)
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            );
        }
    }
}
