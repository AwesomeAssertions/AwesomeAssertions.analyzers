using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests
{
    [TestClass]
    public class ExceptionsTests
    {
        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().Be(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().Be(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldBe_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldBe);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().Contain(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().Contain(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldContain);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().EndWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().EndWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldEndWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldEndWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().StartWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.Message.Should().StartWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldStartWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldStartWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().Be(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().Be(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldBe_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldBe);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().Contain(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().Contain(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldContain);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().EndWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().EndWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldEndWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldEndWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().StartWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.Message.Should().StartWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldStartWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldStartWith);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().Contain(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"*{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().Contain(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"*{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().Contain(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"*a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().Contain(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"*a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().Be(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(expectedMessage{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().Be(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(expectedMessage{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().Be(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().Be(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().StartWith(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().StartWith(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().StartWith(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().StartWith(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().EndWith(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"*{{expectedMessage}}\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().EndWith(expectedMessage{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage($\"*{{expectedMessage}}\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.Message.Should().EndWith(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"*a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.Message.Should().EndWith(\"a constant string\"{0});",
            newAssertion: "action.Should().Throw<Exception>().WithMessage(\"*a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowWithMessage_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().Be(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().Be(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldBe_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldBe);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().Contain(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().Contain(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldContain);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().EndWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().EndWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldEndWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldEndWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().StartWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.Message.Should().StartWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldStartWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldStartWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().Be(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().Be(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldBe_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldBe);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().Contain(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().Contain(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldContain);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().EndWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().EndWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldEndWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldEndWith);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().StartWith(expectedMessage{0});")]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.Message.Should().StartWith(\"a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldStartWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldStartWith);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().Contain(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"*{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().Contain(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"*{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().Contain(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"*a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().Contain(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"*a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().Be(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(expectedMessage{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().Be(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(expectedMessage{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().Be(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().Be(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().StartWith(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().StartWith(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"{{expectedMessage}}*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().StartWith(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().StartWith(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"a constant string*\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().EndWith(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"*{{expectedMessage}}\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().EndWith(expectedMessage{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage($\"*{{expectedMessage}}\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.Message.Should().EndWith(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"*a constant string\"{0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.Message.Should().EndWith(\"a constant string\"{0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithMessage(\"*a constant string\"{0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithMessage_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.InnerException.Should().BeOfType<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeOfType);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.InnerException.Should().BeOfType<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeOfType);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.InnerException.Should().BeOfType<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeOfType);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.InnerException.Should().BeOfType<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeOfType);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.InnerException.Should().BeOfType<ArgumentException>({0});",
            newAssertion: "action.Should().Throw<Exception>().WithInnerExceptionExactly<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.InnerException.Should().BeOfType<ArgumentException>({0});",
            newAssertion: "action.Should().Throw<Exception>().WithInnerExceptionExactly<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.InnerException.Should().BeOfType<ArgumentException>({0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithInnerExceptionExactly<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.InnerException.Should().BeOfType<ArgumentException>({0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithInnerExceptionExactly<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerExceptionExactly_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().And.InnerException.Should().BeAssignableTo<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeAssignableTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeAssignableTo);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().ThrowExactly<Exception>().Which.InnerException.Should().BeAssignableTo<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeAssignableTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeAssignableTo);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().And.InnerException.Should().BeAssignableTo<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeAssignableTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeAssignableTo);
        
        [TestMethod]
        [AssertionDiagnostic("action.Should().Throw<Exception>().Which.InnerException.Should().BeAssignableTo<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeAssignableTo_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeAssignableTo);

        [TestMethod]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().And.InnerException.Should().BeAssignableTo<ArgumentException>({0});",
            newAssertion: "action.Should().Throw<Exception>().WithInnerException<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().Throw<Exception>().Which.InnerException.Should().BeAssignableTo<ArgumentException>({0});",
            newAssertion: "action.Should().Throw<Exception>().WithInnerException<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().And.InnerException.Should().BeAssignableTo<ArgumentException>({0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithInnerException<ArgumentException>({0});")]
        [AssertionCodeFix(
            oldAssertion: "action.Should().ThrowExactly<Exception>().Which.InnerException.Should().BeAssignableTo<ArgumentException>({0});",
            newAssertion: "action.Should().ThrowExactly<Exception>().WithInnerException<ArgumentException>({0});")]
        [Implemented]
        public void ExceptionShouldThrowWithInnerException_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

        private void VerifyCSharpDiagnostic(string sourceAssertion, DiagnosticMetadata metadata)
        {
            var source = GenerateCode.ExceptionAssertion(sourceAssertion);

            DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source, new DiagnosticResult
            {
                Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                Message = metadata.Message,
                VisitorName = metadata.Name,
                Locations = new DiagnosticResultLocation[]
                {
                    new DiagnosticResultLocation("Test0.cs", 9,13)
                },
                Severity = DiagnosticSeverity.Info
            });
        }

        private void VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion)
        {
            var oldSource = GenerateCode.ExceptionAssertion(oldSourceAssertion);
            var newSource = GenerateCode.ExceptionAssertion(newSourceAssertion);

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
