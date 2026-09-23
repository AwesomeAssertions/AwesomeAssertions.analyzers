using System.Collections.Generic;
using AwesomeAssertions.Analyzers.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class MsTest4Tests
{
    private static readonly PackageReference PackageReference = PackageReference.MSTestTestFramework_4_0_1;

    [TestMethod]
    [Implemented]
    public void SupportExcludingMethods()
    {
        var source = GenerateCode.MsTestAssertion("bool actual", "Assert.Fail(\"message\");");
        DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference)
            .WithAnalyzerConfigOption("ffa_excluded_methods", "M:Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(System.String)")
            .WithExpectedDiagnostics()
        );
    }

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.Inconclusive({0});")]
    [AssertionDiagnosticMsTestV4("Assert.Fail({0});")]
    [Implemented]
    public void MsTest_NotReportedAsserts_TestAnalyzer(string assertion)
    {
        VerifyNoCSharpDiagnostic(string.Empty, assertion);
    }

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace1_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            namespace Microsoft.VisualStudio
            {
                using TestTools.UnitTesting;
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace2_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            namespace Microsoft.VisualStudio
            {
                using TestTools.UnitTesting;
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace3_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            namespace Microsoft
            { namespace VisualStudio {
                using TestTools.UnitTesting;
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                } }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace4_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            namespace Microsoft
            { namespace VisualStudio {
                using TestTools   .   UnitTesting;
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                } }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace5_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            using Microsoft . VisualStudio . TestTools . UnitTesting;
            namespace Testing
            {
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace6_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks; using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
            using Microsoft . VisualStudio . TestTools . UnitTesting;
            namespace Testing
            {
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace7_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks; using MsAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
            using Microsoft . VisualStudio . TestTools . UnitTesting;
            namespace Testing
            {
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsTrue_NestedUsingInNamespace8_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic($$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Threading.Tasks;
            namespace Testing
            {
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                class TestClass
                {
                    void TestMethod(bool actual)
                    {
                        {{assertion}}
                    }
                }
            }
            """);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsTrue(actual{0});",
        newAssertion: "actual.Should().BeTrue({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsTrue(bool.Parse(\"true\"){0});",
        newAssertion: "bool.Parse(\"true\").Should().BeTrue({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsTrue(bool.Parse(\"true\"), $\"string interpolation {Environment.ProcessorCount}\");",
        newAssertion: "bool.Parse(\"true\").Should().BeTrue($\"string interpolation {Environment.ProcessorCount}\");")]
    [Implemented]
    public void AssertIsTrue_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsFalse(actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsFalse(bool.Parse(\"true\"){0});")]
    [Implemented]
    public void AssertIsFalse_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsFalse(actual{0});",
        newAssertion: "actual.Should().BeFalse({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsFalse(bool.Parse(\"true\"){0});",
        newAssertion: "bool.Parse(\"true\").Should().BeFalse({0});")]
    [Implemented]
    public void AssertIsFalse_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsNull(actual{0});")]
    [Implemented]
    public void AssertIsNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsNull(actual{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [Implemented]
    public void AssertIsNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsNotNull(actual{0});")]
    [Implemented]
    public void AssertIsNotNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsNotNull(actual{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [Implemented]
    public void AssertIsNotNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsInstanceOfType(actual, type{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsInstanceOfType(actual, typeof(string){0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsInstanceOfType<string>(actual{0});")]
    [Implemented]
    public void AssertIsInstanceOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsInstanceOfType(actual, type{0});",
        newAssertion: "actual.Should().BeOfType(type{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsInstanceOfType(actual, typeof(string){0});",
        newAssertion: "actual.Should().BeOfType<string>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsInstanceOfType<string>(actual{0});",
        newAssertion: "actual.Should().BeOfType<string>({0});")]
    [Implemented]
    public void AssertIsInstanceOfType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.IsNotInstanceOfType(actual, type{0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsNotInstanceOfType(actual, typeof(string){0});")]
    [AssertionDiagnosticMsTestV4("Assert.IsNotInstanceOfType<string>(actual{0});")]
    [Implemented]
    public void AssertIsNotInstanceOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsNotInstanceOfType(actual, type{0});",
        newAssertion: "actual.Should().NotBeOfType(type{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsNotInstanceOfType(actual, typeof(string){0});",
        newAssertion: "actual.Should().NotBeOfType<string>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.IsNotInstanceOfType<string>(actual{0});",
        newAssertion: "actual.Should().NotBeOfType<string>({0});")]
    [Implemented]
    public void AssertIsNotInstanceOfType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual{0});")]
    [Implemented]
    public void AssertObjectAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [Implemented]
    public void AssertObjectAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual{0});")]
    [Implemented]
    public void AssertOptionalIntegerAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual, int? expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [Implemented]
    public void AssertOptionalIntegerAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual, int? expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, null{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(null, actual{0});")]
    [Implemented]
    public void AssertOptionalIntegerAndNullAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, null{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(null, actual{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [Implemented]
    public void AssertOptionalIntegerAndNullAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, 0.6{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, 4.2d, 0.6{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(4.2d, actual, 0.6{0});")]
    [Implemented]
    public void AssertDoubleAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("double actual, double expected, double delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(expected, 0.6{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, 4.2d, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(4.2d, 0.6{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(4.2d, actual, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(4.2d, 0.6{0});")]
    [Implemented]
    public void AssertDoubleAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, double delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, 4.2f, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(4.2f, actual, 0.6f{0});")]
    [Implemented]
    public void AssertFloatAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("float actual, float expected, float delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(expected, 0.6f{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, 4.2f, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(4.2f, 0.6f{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(4.2f, actual, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(4.2f, 0.6f{0});")]
    [Implemented]
    public void AssertFloatAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("float actual, float expected, float delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, 0.6m{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, 4.2m, 0.6m{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(4.2m, actual, 0.6m{0});")]
    [Implemented]
    public void AssertDecimalAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("decimal actual, decimal expected, decimal delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, 0.6m{0});",
        newAssertion: "actual.Should().BeApproximately(expected, 0.6m{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, 4.2m, 0.6m{0});",
        newAssertion: "actual.Should().BeApproximately(4.2m, 0.6m{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(4.2m, actual, 0.6m{0});",
        newAssertion: "actual.Should().BeApproximately(4.2m, 0.6m{0});")]
    [Implemented]
    public void AssertDecimalAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("decimal actual, decimal expected, decimal delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, \"literal\"{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(\"literal\", actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, false{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, \"literal\", false{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(\"literal\", actual, false{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, true{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, \"literal\", true{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(\"literal\", actual, true{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, \"literal\", false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(\"literal\", actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(actual, \"literal\", true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreEqual(\"literal\", actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [Implemented]
    public void AssertStringAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, \"literal\"{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(\"literal\", actual{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, false{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", false{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, false{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [Implemented]
    public void AssertStringAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public void AssertObjectAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [Implemented]
    public void AssertObjectAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, 0.6{0});")]
    [Implemented]
    public void AssertDoubleAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("double actual, double expected, double delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, 0.6{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, 0.6{0});")]
    [Implemented]
    public void AssertDoubleAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, double delta", oldAssertion, newAssertion);


    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public void AssertOptionalIntAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual, int? expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [Implemented]
    public void AssertOptionalIntAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual, int? expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(actual, null{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(null, actual{0});")]
    [Implemented]
    public void AssertOptionalIntAndNullAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(actual, null{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(null, actual{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [Implemented]
    public void AssertOptionalIntAndNullAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(actual, 4.2f, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(4.2f, actual, 0.6f{0});")]
    [Implemented]
    public void AssertFloatAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("float actual, float expected, float delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, 0.6f{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(actual, 4.2f, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2f, 0.6f{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(4.2f, actual, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2f, 0.6f{0});")]
    [Implemented]
    public void AssertFloatAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("float actual, float expected, float delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, delta{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(actual, 4.2f, 0.6f{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(4.2f, actual, 0.6f{0});")]
    [Implemented]
    public void AssertDecimalAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("float actual, float expected, float delta", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, delta{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, 0.6m{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, 0.6m{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(actual, 4.2m, 0.6m{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2m, 0.6m{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(4.2m, actual, 0.6m{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2m, 0.6m{0});")]
    [Implemented]
    public void AssertDecimalAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("decimal actual, decimal expected, decimal delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, false{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, true{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnosticMsTestV4("Assert.AreNotEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [Implemented]
    public void AssertStringAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, false{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, true{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [Implemented]
    public void AssertStringAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreSame(expected, actual{0});")]
    [Implemented]
    public void AssertAreSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreSame(expected, actual{0});",
        newAssertion: "actual.Should().BeSameAs(expected{0});")]
    [Implemented]
    public void AssertAreSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.AreNotSame(expected, actual{0});")]
    [Implemented]
    public void AssertAreNotSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.AreNotSame(expected, actual{0});",
        newAssertion: "actual.Should().NotBeSameAs(expected{0});")]
    [Implemented]
    public void AssertAreNotSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.ThrowsExactly<ArgumentException>(action{0});")]
    [AssertionDiagnosticMsTestV4("Assert.Throws<ArgumentException>(action{0});")]
    [Implemented]
    public void AssertThrows_Action_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("Action action", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.ThrowsExactly<ArgumentException>(action{0});",
        newAssertion: "action.Should().ThrowExactly<ArgumentException>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.Throws<ArgumentException>(action{0});",
        newAssertion: "action.Should().Throw<ArgumentException>({0});")]
    [Implemented]
    public void AssertThrows_Action_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("Action action", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV3("Assert.ThrowsException<ArgumentException>(func{0});")]
    [AssertionDiagnosticMsTestV3("Assert.ThrowsExactly<ArgumentException>(func{0});")]
    [AssertionDiagnosticMsTestV3("Assert.Throws<ArgumentException>(func{0});")]
    [Implemented]
    public void AssertThrowsException_Func_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("Func<object> func", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.ThrowsExactly<ArgumentException>(func{0});",
        newAssertion: "func.Should().ThrowExactly<ArgumentException>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.Throws<ArgumentException>(func{0});",
        newAssertion: "func.Should().Throw<ArgumentException>({0});")]
    [Implemented]
    public void AssertThrows_Func_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("Func<object> func", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("Assert.ThrowsExactlyAsync<ArgumentException>(action{0});")]
    [AssertionDiagnosticMsTestV4("Assert.ThrowsAsync<ArgumentException>(action{0});")]
    [Implemented]
    public void AssertThrowsAsync_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("Func<Task> action", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.ThrowsExactlyAsync<ArgumentException>(action{0});",
        newAssertion: "action.Should().ThrowExactlyAsync<ArgumentException>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "Assert.ThrowsAsync<ArgumentException>(action{0});",
        newAssertion: "action.Should().ThrowAsync<ArgumentException>({0});")]
    [Implemented]
    public void AssertThrowsAsync_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("Func<Task> action", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreInstancesOfType(actual, type{0});")]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(int){0});")]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(Int32){0});")]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.Int32){0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreInstancesOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, type{0});",
        newAssertion: "actual.Should().AllBeOfType(type{0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(int){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(Int32){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.Int32){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(DateTime){0});",
        newAssertion: "actual.Should().AllBeOfType<DateTime>({0});")]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.DateTime){0});",
        newAssertion: "actual.Should().AllBeOfType<DateTime>({0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreInstancesOfType_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AreEqual(expected, actual{0});")]
    [Implemented]
    public void CollectionAssertAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Equal(expected{0});")]
    [Implemented]
    public void CollectionAssertAreEqual_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public void CollectionAssertAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotEqual(expected{0});")]
    [Implemented]
    public void CollectionAssertAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AreEquivalent(expected, actual{0});")]
    [Implemented]
    public void CollectionAssertAreEquivalent_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AreEquivalent(expected, actual{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [Implemented]
    public void CollectionAssertAreEquivalent_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AreNotEquivalent(expected, actual{0});")]
    [Implemented]
    public void CollectionAssertAreNotEquivalent_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AreNotEquivalent(expected, actual{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [Implemented]
    public void CollectionAssertAreNotEquivalent_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreNotNull(actual{0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreNotNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreNotNull(actual{0});",
        newAssertion: "actual.Should().NotContainNulls({0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreNotNull_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.AllItemsAreUnique(actual{0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreUnique_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.AllItemsAreUnique(actual{0});",
        newAssertion: "actual.Should().OnlyHaveUniqueItems({0});")]
    [Implemented]
    public void CollectionAssertAllItemsAreUnique_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.Contains(actual, expected{0});")]
    [Implemented]
    public void CollectionAssertContains_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, int expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.Contains(actual, expected{0});",
        newAssertion: "actual.Should().Contain(expected{0});")]
    [Implemented]
    public void CollectionAssertContains_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, int expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.DoesNotContain(actual, expected{0});")]
    [Implemented]
    public void CollectionAssertDoesNotContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, int expected", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.DoesNotContain(actual, expected{0});",
        newAssertion: "actual.Should().NotContain(expected{0});")]
    [Implemented]
    public void CollectionAssertDoesNotContain_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, int expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.IsSubsetOf(subset, superset{0});")]
    [Implemented]
    public void CollectionAssertIsSubsetOf_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.IsSubsetOf(subset, superset{0});",
        newAssertion: "subset.Should().BeSubsetOf(superset{0});")]
    [Implemented]
    public void CollectionAssertIsSubsetOf_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("CollectionAssert.IsNotSubsetOf(subset, superset{0});")]
    [Implemented]
    public void CollectionAssertIsNotSubsetOf_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "CollectionAssert.IsNotSubsetOf(subset, superset{0});",
        newAssertion: "subset.Should().NotBeSubsetOf(superset{0});")]
    [Implemented]
    public void CollectionAssertIsNotSubsetOf_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("StringAssert.Contains(actual, substring{0});")]
    [Implemented]
    public void StringAssertContains_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "StringAssert.Contains(actual, substring{0});",
        newAssertion: "actual.Should().Contain(substring{0});")]
    [Implemented]
    public void StringAssertContains_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("StringAssert.StartsWith(actual, substring{0});")]
    [Implemented]
    public void StringAssertStartsWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "StringAssert.StartsWith(actual, substring{0});",
        newAssertion: "actual.Should().StartWith(substring{0});")]
    [Implemented]
    public void StringAssertStartsWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("StringAssert.EndsWith(actual, substring{0});")]
    [Implemented]
    public void StringAssertEndsWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "StringAssert.EndsWith(actual, substring{0});",
        newAssertion: "actual.Should().EndWith(substring{0});")]
    [Implemented]
    public void StringAssertEndsWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("StringAssert.Matches(actual, pattern{0});")]
    [Implemented]
    public void StringAssertMatches_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, System.Text.RegularExpressions.Regex pattern", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "StringAssert.Matches(actual, pattern{0});",
        newAssertion: "actual.Should().MatchRegex(pattern{0});")]
    [Implemented]
    public void StringAssertMatches_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, System.Text.RegularExpressions.Regex pattern", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("StringAssert.DoesNotMatch(actual, pattern{0});")]
    [Implemented]
    public void StringAssertDoesNotMatch_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, System.Text.RegularExpressions.Regex pattern", assertion);

    [TestMethod]
    [AssertionCodeFixMsTestV4(
        oldAssertion: "StringAssert.DoesNotMatch(actual, pattern{0});",
        newAssertion: "actual.Should().NotMatchRegex(pattern{0});")]
    [Implemented]
    public void StringAssertDoesNotMatch_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, System.Text.RegularExpressions.Regex pattern", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("throw new AssertFailedException();")]
    [Implemented]
    public void ThrowAssertFailedException_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic(string.Empty, assertion);

    [TestMethod]
    [AssertionDiagnosticMsTestV4("throw new AssertInconclusiveException();")]
    [Implemented]
    public void ThrowAssertInconclusiveException_TestNoAnalyzer(string assertion)
        => VerifyNoCSharpDiagnostic(string.Empty, assertion);

    private static void VerifyCSharpDiagnostic(string source)
    {
        VerifyCSharpDiagnosticUsingAllAnalyzers(source,
            new DiagnosticResult
            {
                Id = AssertAnalyzer.MSTestsRule.Id,
                Message = AssertAnalyzer.Message,
                Locations =
                [
                    new DiagnosticResultLocation("Test0.cs", 12, 13)
                ],
                Severity = DiagnosticSeverity.Info
            });
    }

    private static void VerifyCSharpDiagnosticUsingAllAnalyzers(string source, params DiagnosticResult[] expectedDiagnostics)
    {
        DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference)
            .WithExpectedDiagnostics(expectedDiagnostics)
        );
    }

    private static void VerifyCSharpDiagnostic(string methodArguments, string assertion)
        => VerifyCSharpDiagnostic(GenerateCode.MsTestAssertion(methodArguments, assertion));

    private static void VerifyNoCSharpDiagnostic(string methodArguments, string assertion)
        => VerifyCSharpDiagnosticUsingAllAnalyzers(GenerateCode.MsTestAssertion(methodArguments, assertion));

    private static void VerifyCSharpFix(string methodArguments, string oldAssertion, string newAssertion)
    {
        var oldSource = GenerateCode.MsTestAssertion(methodArguments, oldAssertion);
        var newSource = GenerateCode.MsTestAssertion(methodArguments, newAssertion);

        DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithCodeFixProvider<MsTestCodeFixProvider>()
            .WithDiagnosticAnalyzer<AssertAnalyzer>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference)
        );
    }
}

public sealed class AssertionDiagnosticMsTestV4Attribute(string assertion, params string[] additionalParameters)
    : AssertionDiagnosticBaseAttribute(assertion, additionalParameters)
{
    private protected override IEnumerable<string> GetTestCases()
        => TestCasesInputUtils.GetTestCases(Assertion, MessageFormat.Simple | MessageFormat.Because | MessageFormat.InterpolatedBecause);
}

public class AssertionCodeFixMsTestV4Attribute(string oldAssertion, string newAssertion, params string[] additionalParameters)
    : AssertionCodeFixBaseAttribute(oldAssertion, newAssertion, additionalParameters)
{
    private protected override IEnumerable<(string oldAssertion, string newAssertion)> GetTestCases()
        => TestCasesInputUtils.GetTestCases(OldAssertion, NewAssertion, MessageFormat.Simple | MessageFormat.Because | MessageFormat.InterpolatedBecause);
}
