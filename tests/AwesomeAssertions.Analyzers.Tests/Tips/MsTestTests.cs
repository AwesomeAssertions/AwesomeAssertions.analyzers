using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class MsTestTests
{
    [TestMethod]
    [Implemented]
    public Task SupportExcludingMethods()
    {
        var source = GenerateCode.MsTestAssertion("bool actual", "Assert.IsTrue(actual);");
        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.MSTestTestFramework_3_1_1)
            .WithAnalyzerConfigOption("ffa_excluded_methods", "M:Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(System.Boolean)")
            .WithExpectedDiagnostics()
        );
    }

    [TestMethod]
    [AssertionDiagnostic("Assert.Inconclusive({0});")]
    [AssertionDiagnostic("Assert.Fail({0});")]
    [Implemented]
    public Task MsTest_NotReportedAsserts_TestAnalyzer(string assertion)
    {
        return VerifyNoCSharpDiagnostic(string.Empty, assertion);
    }

    [TestMethod]
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace1_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace2_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace3_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace4_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace5_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace6_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace7_TestAnalyzer(string assertion)
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
    [AssertionDiagnostic("Assert.IsTrue(actual{0});")]
    [AssertionDiagnostic("Assert.IsTrue(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsTrue_NestedUsingInNamespace8_TestAnalyzer(string assertion)
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
    [AssertionCodeFix(
        oldAssertion: "Assert.IsTrue(actual{0});",
        newAssertion: "actual.Should().BeTrue({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsTrue(bool.Parse(\"true\"){0});",
        newAssertion: "bool.Parse(\"true\").Should().BeTrue({0});")]
    [Implemented]
    public Task AssertIsTrue_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsFalse(actual{0});")]
    [AssertionDiagnostic("Assert.IsFalse(bool.Parse(\"true\"){0});")]
    [Implemented]
    public Task AssertIsFalse_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsFalse(actual{0});",
        newAssertion: "actual.Should().BeFalse({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsFalse(bool.Parse(\"true\"){0});",
        newAssertion: "bool.Parse(\"true\").Should().BeFalse({0});")]
    [Implemented]
    public Task AssertIsFalse_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsNull(actual{0});")]
    [Implemented]
    public Task AssertIsNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsNull(actual{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [Implemented]
    public Task AssertIsNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsNotNull(actual{0});")]
    [Implemented]
    public Task AssertIsNotNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsNotNull(actual{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [Implemented]
    public Task AssertIsNotNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsInstanceOfType(actual, type{0});")]
    [AssertionDiagnostic("Assert.IsInstanceOfType(actual, typeof(string){0});")]
    [AssertionDiagnostic("Assert.IsInstanceOfType<string>(actual{0});")]
    [Implemented]
    public Task AssertIsInstanceOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsInstanceOfType(actual, type{0});",
        newAssertion: "actual.Should().BeOfType(type{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsInstanceOfType(actual, typeof(string){0});",
        newAssertion: "actual.Should().BeOfType<string>({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsInstanceOfType<string>(actual{0});",
        newAssertion: "actual.Should().BeOfType<string>({0});")]
    [Implemented]
    public Task AssertIsInstanceOfType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.IsNotInstanceOfType(actual, type{0});")]
    [AssertionDiagnostic("Assert.IsNotInstanceOfType(actual, typeof(string){0});")]
    [AssertionDiagnostic("Assert.IsNotInstanceOfType<string>(actual{0});")]
    [Implemented]
    public Task AssertIsNotInstanceOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsNotInstanceOfType(actual, type{0});",
        newAssertion: "actual.Should().NotBeOfType(type{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsNotInstanceOfType(actual, typeof(string){0});",
        newAssertion: "actual.Should().NotBeOfType<string>({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.IsNotInstanceOfType<string>(actual{0});",
        newAssertion: "actual.Should().NotBeOfType<string>({0});")]
    [Implemented]
    public Task AssertIsNotInstanceOfType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual{0});")]
    [Implemented]
    public Task AssertObjectAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [Implemented]
    public Task AssertObjectAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual{0});")]
    [Implemented]
    public Task AssertOptionalIntegerAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual, int? expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [Implemented]
    public Task AssertOptionalIntegerAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual, int? expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(actual, null{0});")]
    [AssertionDiagnostic("Assert.AreEqual(null, actual{0});")]
    [Implemented]
    public Task AssertOptionalIntegerAndNullAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, null{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(null, actual{0});",
        newAssertion: "actual.Should().BeNull({0});")]
    [Implemented]
    public Task AssertOptionalIntegerAndNullAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, delta{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, 0.6{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, 4.2d, 0.6{0});")]
    [AssertionDiagnostic("Assert.AreEqual(4.2d, actual, 0.6{0});")]
    [Implemented]
    public Task AssertDoubleAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("double actual, double expected, double delta", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(expected, 0.6{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, 4.2d, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(4.2d, 0.6{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(4.2d, actual, 0.6{0});",
        newAssertion: "actual.Should().BeApproximately(4.2d, 0.6{0});")]
    [Implemented]
    public Task AssertDoubleAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, double delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, delta{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, 0.6f{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, 4.2f, 0.6f{0});")]
    [AssertionDiagnostic("Assert.AreEqual(4.2f, actual, 0.6f{0});")]
    [Implemented]
    public Task AssertFloatAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("float actual, float expected, float delta", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().BeApproximately(expected, delta{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(expected, 0.6f{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, 4.2f, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(4.2f, 0.6f{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(4.2f, actual, 0.6f{0});",
        newAssertion: "actual.Should().BeApproximately(4.2f, 0.6f{0});")]
    [Implemented]
    public Task AssertFloatAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("float actual, float expected, float delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, \"literal\"{0});")]
    [AssertionDiagnostic("Assert.AreEqual(\"literal\", actual{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, false{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, \"literal\", false{0});")]
    [AssertionDiagnostic("Assert.AreEqual(\"literal\", actual, false{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, true{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, \"literal\", true{0});")]
    [AssertionDiagnostic("Assert.AreEqual(\"literal\", actual, true{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, \"literal\", false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreEqual(\"literal\", actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreEqual(actual, \"literal\", true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreEqual(\"literal\", actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [Implemented]
    public Task AssertStringAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, \"literal\"{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(\"literal\", actual{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, false{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", false{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, false{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, true{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().Be(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(actual, \"literal\", true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreEqual(\"literal\", actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().BeEquivalentTo(\"literal\"{0});")]
    [Implemented]
    public Task AssertStringAreEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public Task AssertObjectAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [Implemented]
    public Task AssertObjectAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, delta{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, 0.6{0});")]
    [Implemented]
    public Task AssertDoubleAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("double actual, double expected, double delta", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, delta{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, 0.6{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, 0.6{0});")]
    [Implemented]
    public Task AssertDoubleAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, double delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public Task AssertOptionalIntAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual, int? expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [Implemented]
    public Task AssertOptionalIntAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual, int? expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(actual, null{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(null, actual{0});")]
    [Implemented]
    public Task AssertOptionalIntAndNullAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("int? actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(actual, null{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(null, actual{0});",
        newAssertion: "actual.Should().NotBeNull({0});")]
    [Implemented]
    public Task AssertOptionalIntAndNullAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("int? actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, delta{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, 0.6f{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(actual, 4.2f, 0.6f{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(4.2f, actual, 0.6f{0});")]
    [Implemented]
    public Task AssertFloatAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("float actual, float expected, float delta", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, delta{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, delta{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(expected, 0.6f{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(actual, 4.2f, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2f, 0.6f{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(4.2f, actual, 0.6f{0});",
        newAssertion: "actual.Should().NotBeApproximately(4.2f, 0.6f{0});")]
    [Implemented]
    public Task AssertFloatAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("float actual, float expected, float delta", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, false{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, true{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [AssertionDiagnostic("Assert.AreNotEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});")]
    [Implemented]
    public Task AssertStringAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, false{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, true{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, false, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().NotBe(expected{0});")]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotEqual(expected, actual, true, System.Globalization.CultureInfo.CurrentCulture{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [Implemented]
    public Task AssertStringAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreSame(expected, actual{0});")]
    [Implemented]
    public Task AssertAreSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreSame(expected, actual{0});",
        newAssertion: "actual.Should().BeSameAs(expected{0});")]
    [Implemented]
    public Task AssertAreSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.AreNotSame(expected, actual{0});")]
    [Implemented]
    public Task AssertAreNotSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.AreNotSame(expected, actual{0});",
        newAssertion: "actual.Should().NotBeSameAs(expected{0});")]
    [Implemented]
    public Task AssertAreNotSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.ThrowsException<ArgumentException>(action{0});")]
    [Implemented]
    public Task AssertThrowsException_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("Action action", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.ThrowsException<ArgumentException>(action{0});",
        newAssertion: "action.Should().ThrowExactly<ArgumentException>({0});")]
    [Implemented]
    public Task AssertThrowsException_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("Action action", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("Assert.ThrowsExceptionAsync<ArgumentException>(action{0});")]
    [Implemented]
    public Task AssertThrowsExceptionAsync_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("Func<Task> action", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "Assert.ThrowsExceptionAsync<ArgumentException>(action{0});",
        newAssertion: "action.Should().ThrowExactlyAsync<ArgumentException>({0});")]
    [Implemented]
    public Task AssertThrowsExceptionAsync_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("Func<Task> action", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreInstancesOfType(actual, type{0});")]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(int){0});")]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(Int32){0});")]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.Int32){0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreInstancesOfType_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, Type type", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, type{0});",
        newAssertion: "actual.Should().AllBeOfType(type{0});")]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(int){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(Int32){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.Int32){0});",
        newAssertion: "actual.Should().AllBeOfType<int>({0});")]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(DateTime){0});",
        newAssertion: "actual.Should().AllBeOfType<DateTime>({0});")]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreInstancesOfType(actual, typeof(System.DateTime){0});",
        newAssertion: "actual.Should().AllBeOfType<DateTime>({0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreInstancesOfType_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, Type type", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AreEqual(expected, actual{0});")]
    [Implemented]
    public Task CollectionAssertAreEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AreEqual(expected, actual{0});",
        newAssertion: "actual.Should().Equal(expected{0});")]
    [Implemented]
    public Task CollectionAssertAreEqual_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AreNotEqual(expected, actual{0});")]
    [Implemented]
    public Task CollectionAssertAreNotEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AreNotEqual(expected, actual{0});",
        newAssertion: "actual.Should().NotEqual(expected{0});")]
    [Implemented]
    public Task CollectionAssertAreNotEqual_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AreEquivalent(expected, actual{0});")]
    [Implemented]
    public Task CollectionAssertAreEquivalent_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AreEquivalent(expected, actual{0});",
        newAssertion: "actual.Should().BeEquivalentTo(expected{0});")]
    [Implemented]
    public Task CollectionAssertAreEquivalent_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AreNotEquivalent(expected, actual{0});")]
    [Implemented]
    public Task CollectionAssertAreNotEquivalent_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AreNotEquivalent(expected, actual{0});",
        newAssertion: "actual.Should().NotBeEquivalentTo(expected{0});")]
    [Implemented]
    public Task CollectionAssertAreNotEquivalent_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, System.Collections.Generic.List<int> expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreNotNull(actual{0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreNotNull_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreNotNull(actual{0});",
        newAssertion: "actual.Should().NotContainNulls({0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreNotNull_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.AllItemsAreUnique(actual{0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreUnique_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.AllItemsAreUnique(actual{0});",
        newAssertion: "actual.Should().OnlyHaveUniqueItems({0});")]
    [Implemented]
    public Task CollectionAssertAllItemsAreUnique_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.Contains(actual, expected{0});")]
    [Implemented]
    public Task CollectionAssertContains_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, int expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.Contains(actual, expected{0});",
        newAssertion: "actual.Should().Contain(expected{0});")]
    [Implemented]
    public Task CollectionAssertContains_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, int expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.DoesNotContain(actual, expected{0});")]
    [Implemented]
    public Task CollectionAssertDoesNotContain_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> actual, int expected", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.DoesNotContain(actual, expected{0});",
        newAssertion: "actual.Should().NotContain(expected{0});")]
    [Implemented]
    public Task CollectionAssertDoesNotContain_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> actual, int expected", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.IsSubsetOf(subset, superset{0});")]
    [Implemented]
    public Task CollectionAssertIsSubsetOf_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.IsSubsetOf(subset, superset{0});",
        newAssertion: "subset.Should().BeSubsetOf(superset{0});")]
    [Implemented]
    public Task CollectionAssertIsSubsetOf_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("CollectionAssert.IsNotSubsetOf(subset, superset{0});")]
    [Implemented]
    public Task CollectionAssertIsNotSubsetOf_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "CollectionAssert.IsNotSubsetOf(subset, superset{0});",
        newAssertion: "subset.Should().NotBeSubsetOf(superset{0});")]
    [Implemented]
    public Task CollectionAssertIsNotSubsetOf_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("System.Collections.Generic.List<int> subset, System.Collections.Generic.List<int> superset", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("StringAssert.Contains(actual, substring{0});")]
    [Implemented]
    public Task StringAssertContains_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "StringAssert.Contains(actual, substring{0});",
        newAssertion: "actual.Should().Contain(substring{0});")]
    [Implemented]
    public Task StringAssertContains_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("StringAssert.StartsWith(actual, substring{0});")]
    [Implemented]
    public Task StringAssertStartsWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "StringAssert.StartsWith(actual, substring{0});",
        newAssertion: "actual.Should().StartWith(substring{0});")]
    [Implemented]
    public Task StringAssertStartsWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("StringAssert.EndsWith(actual, substring{0});")]
    [Implemented]
    public Task StringAssertEndsWith_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, string substring", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "StringAssert.EndsWith(actual, substring{0});",
        newAssertion: "actual.Should().EndWith(substring{0});")]
    [Implemented]
    public Task StringAssertEndsWith_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, string substring", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("StringAssert.Matches(actual, pattern{0});")]
    [Implemented]
    public Task StringAssertMatches_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, System.Text.RegularExpressions.Regex pattern", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "StringAssert.Matches(actual, pattern{0});",
        newAssertion: "actual.Should().MatchRegex(pattern{0});")]
    [Implemented]
    public Task StringAssertMatches_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, System.Text.RegularExpressions.Regex pattern", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("StringAssert.DoesNotMatch(actual, pattern{0});")]
    [Implemented]
    public Task StringAssertDoesNotMatch_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("string actual, System.Text.RegularExpressions.Regex pattern", assertion);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "StringAssert.DoesNotMatch(actual, pattern{0});",
        newAssertion: "actual.Should().NotMatchRegex(pattern{0});")]
    [Implemented]
    public Task StringAssertDoesNotMatch_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix("string actual, System.Text.RegularExpressions.Regex pattern", oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("throw new AssertFailedException();")]
    [Implemented]
    public Task ThrowAssertFailedException_TestAnalyzer(string assertion)
        => VerifyCSharpDiagnostic(string.Empty, assertion);

    [TestMethod]
    [AssertionDiagnostic("throw new AssertInconclusiveException();")]
    [Implemented]
    public Task ThrowAssertInconclusiveException_TestNoAnalyzer(string assertion)
        => VerifyNoCSharpDiagnostic(string.Empty, assertion);

    private static Task VerifyCSharpDiagnostic(string source)
    {
        return VerifyCSharpDiagnosticUsingAllAnalyzers(source,
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

    private static Task VerifyCSharpDiagnostic(string methodArguments, string assertion)
        => VerifyCSharpDiagnostic(GenerateCode.MsTestAssertion(methodArguments, assertion));

    private static Task VerifyNoCSharpDiagnostic(string methodArguments, string assertion)
        => VerifyCSharpDiagnosticUsingAllAnalyzers(GenerateCode.MsTestAssertion(methodArguments, assertion));

    private static Task VerifyCSharpDiagnosticUsingAllAnalyzers(string source, params DiagnosticResult[] expectedDiagnostics)
    {
        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.MSTestTestFramework_3_1_1)
            .WithExpectedDiagnostics(expectedDiagnostics)
        );
    }

    private static Task VerifyCSharpFix(string methodArguments, string oldAssertion, string newAssertion)
    {
        var oldSource = GenerateCode.MsTestAssertion(methodArguments, oldAssertion);
        var newSource = GenerateCode.MsTestAssertion(methodArguments, newAssertion);

        return DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithCodeFixProvider<MsTestCodeFixProvider>()
            .WithDiagnosticAnalyzer<AssertAnalyzer>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.MSTestTestFramework_3_1_1)
        );
    }
}
