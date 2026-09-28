using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        [AssertionDiagnostic("actual.StartsWith(expected).Should().BeTrue({0});")]
        [AssertionDiagnostic("actual.ToString().StartsWith(expected).Should().BeTrue({0}).And.ToString();")]
        [Implemented]
        public void StringShouldStartWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldStartWith_StartsWithShouldBeTrue);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.StartsWith(expected).Should().BeTrue({0});",
            newAssertion: "actual.Should().StartWith(expected{0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().StartsWith(expected).Should().BeTrue({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().StartWith(expected{0}).And.ToString();")]
        [Implemented]
        public void StringShouldStartWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("actual.EndsWith(expected).Should().BeTrue({0});")]
        [AssertionDiagnostic("actual.ToString().EndsWith(expected).Should().BeTrue({0}).And.ToString();")]
        [Implemented]
        public void StringShouldEndWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldEndWith_EndsWithShouldBeTrue);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.EndsWith(expected).Should().BeTrue({0});",
            newAssertion: "actual.Should().EndWith(expected{0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().EndsWith(expected).Should().BeTrue({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().EndWith(expected{0}).And.ToString();")]
        [Implemented]
        public void StringShouldEndWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("string.IsNullOrEmpty(actual).Should().BeFalse({0});")]
        [AssertionDiagnostic("string.IsNullOrEmpty(actual.ToString()).Should().BeFalse({0}).And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrEmpty_StringIsNullOrEmptyShouldBeFalse_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringIsNullOrEmptyShouldBeFalse);

        [TestMethod]
        [AssertionDiagnostic("actual.Should().NotBeEmpty().And.NotBeNull({0});")]
        [AssertionDiagnostic("actual.Should().NotBeEmpty({0}).And.NotBeNull();")]
        [AssertionDiagnostic("actual.ToString().Should().NotBeEmpty({0}).And.NotBeNull().And.ToString();")]
        [AssertionDiagnostic("actual.ToString().Should().NotBeEmpty().And.NotBeNull({0}).And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrEmpty_StringShouldNotBeEmptyAndNotBeNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringShouldNotBeEmptyAndNotBeNull);

        [TestMethod]
        [AssertionDiagnostic("actual.Should().NotBeNull().And.NotBeEmpty({0});")]
        [AssertionDiagnostic("actual.Should().NotBeNull({0}).And.NotBeEmpty();")]
        [AssertionDiagnostic("actual.ToString().Should().NotBeNull().And.NotBeEmpty({0}).And.ToString();")]
        [AssertionDiagnostic("actual.ToString().Should().NotBeNull({0}).And.NotBeEmpty().And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrEmpty_StringShouldNotBeNullAndNotBeEmpty_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringShouldNotBeNullAndNotBeEmpty);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().NotBeNull({0}).And.NotBeEmpty();",
            newAssertion: "actual.Should().NotBeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().NotBeNull().And.NotBeEmpty({0});",
            newAssertion: "actual.Should().NotBeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().NotBeEmpty({0}).And.NotBeNull();",
            newAssertion: "actual.Should().NotBeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.Should().NotBeEmpty().And.NotBeNull({0});",
            newAssertion: "actual.Should().NotBeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrEmpty(actual).Should().BeFalse({0});",
            newAssertion: "actual.Should().NotBeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().Should().NotBeNull({0}).And.NotBeEmpty().And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrEmpty({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().Should().NotBeNull().And.NotBeEmpty({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrEmpty({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().Should().NotBeEmpty({0}).And.NotBeNull().And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrEmpty({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().Should().NotBeEmpty().And.NotBeNull({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrEmpty({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrEmpty(actual.ToString()).Should().BeFalse({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrEmpty({0}).And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrEmpty_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("string.IsNullOrEmpty(actual).Should().BeTrue({0});")]
        [AssertionDiagnostic("string.IsNullOrEmpty(actual.ToString()).Should().BeTrue({0}).And.ToString();")]
        [Implemented]
        public void StringShouldBeNullOrEmpty_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldBeNullOrEmpty_StringIsNullOrEmptyShouldBeTrue);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrEmpty(actual).Should().BeTrue({0});",
            newAssertion: "actual.Should().BeNullOrEmpty({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrEmpty(actual.ToString()).Should().BeTrue({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().BeNullOrEmpty({0}).And.ToString();")]
        [Implemented]
        public void StringShouldBeNullOrEmpty_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual).Should().BeTrue({0});")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual).Should().BeTrue({0}).And.ToString();")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual.ToString()).Should().BeTrue({0});")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual.ToString()).Should().BeTrue({0}).And.ToString();")]
        [Implemented]
        public void StringShouldBeNullOrWhiteSpace_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldBeNullOrWhiteSpace_StringIsNullOrWhiteSpaceShouldBeTrue);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual).Should().BeTrue({0});",
            newAssertion: "actual.Should().BeNullOrWhiteSpace({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual).Should().BeTrue({0}).And.ToString();",
            newAssertion: "actual.Should().BeNullOrWhiteSpace({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual.ToString()).Should().BeTrue({0});",
            newAssertion: "actual.ToString().Should().BeNullOrWhiteSpace({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual.ToString()).Should().BeTrue({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().BeNullOrWhiteSpace({0}).And.ToString();")]
        [Implemented]
        public void StringShouldBeNullOrWhiteSpace_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual).Should().BeFalse({0});")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual).Should().BeFalse({0}).And.ToString();")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual.ToString()).Should().BeFalse({0});")]
        [AssertionDiagnostic("string.IsNullOrWhiteSpace(actual.ToString()).Should().BeFalse({0}).And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrWhiteSpace_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldNotBeNullOrWhiteSpace_StringShouldNotBeNullOrWhiteSpace);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual).Should().BeFalse({0});",
            newAssertion: "actual.Should().NotBeNullOrWhiteSpace({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual).Should().BeFalse({0}).And.ToString();",
            newAssertion: "actual.Should().NotBeNullOrWhiteSpace({0}).And.ToString();")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual.ToString()).Should().BeFalse({0});",
            newAssertion: "actual.ToString().Should().NotBeNullOrWhiteSpace({0});")]
        [AssertionCodeFix(
            oldAssertion: "string.IsNullOrWhiteSpace(actual.ToString()).Should().BeFalse({0}).And.ToString();",
            newAssertion: "actual.ToString().Should().NotBeNullOrWhiteSpace({0}).And.ToString();")]
        [Implemented]
        public void StringShouldNotBeNullOrWhiteSpace_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("actual.Length.Should().Be(k{0});")]
        [AssertionDiagnostic("actual.ToString().Length.Should().Be(k{0}).And.ToString();")]
        [Implemented]
        public void StringShouldHaveLength_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.StringShouldHaveLength_LengthShouldBe);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "actual.Length.Should().Be(k{0});",
            newAssertion: "actual.Should().HaveLength(k{0});")]
        [AssertionCodeFix(
            oldAssertion: "actual.ToString().Length.Should().Be(k{0}).And.ToString();",
            newAssertion: "actual.ToString().Should().HaveLength(k{0}).And.ToString();")]
        [Implemented]
        public void StringShouldHaveLength_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        private void VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata)
        {
            var source = GenerateCode.StringAssertion(sourceAssertion);

            DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source, new DiagnosticResult
            {
                Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                Message = metadata.Message,
                VisitorName = metadata.Name,
                Locations = new DiagnosticResultLocation[]
                {
                    new DiagnosticResultLocation("Test0.cs", 9, 13)
                },
                Severity = DiagnosticSeverity.Info
            });
        }

        private void VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion)
        {
            var oldSource = GenerateCode.StringAssertion(oldSourceAssertion);
            var newSource = GenerateCode.StringAssertion(newSourceAssertion);

            DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
                .WithSources(oldSource)
                .WithFixedSources(newSource)
                .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
                .WithCodeFixProvider<AwesomeAssertionsCodeFixProvider>()
                .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            );
        }
    }
}
