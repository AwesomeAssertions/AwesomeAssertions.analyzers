using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class XunitTests
{
    [TestMethod]
    [Implemented]
    public Task SupportExcludingMethods()
    {
        var source = GenerateCode.XunitAssertion("bool actual", "Assert.True(actual);");
        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.XunitAssert_2_5_1)
            .WithAnalyzerConfigOption("ffa_excluded_methods", "M:Xunit.Assert.True(System.Boolean)")
            .WithExpectedDiagnostics()
        );
    }

    [TestMethod]
    [DataRow("Assert.True(actual);")]
    [DataRow("Assert.True(actual, \"because it's possible\");")]
    [DataRow("Assert.True(bool.Parse(\"true\"));")]
    [DataRow("Assert.True(bool.Parse(\"true\"), \"because it's possible\");")]
    [Implemented]
    public Task AssertTrue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.True(actual);",
        /* newAssertion: */ "actual.Should().BeTrue();")]
    [DataRow(
        /* oldAssertion: */ "Assert.True(actual, \"because it's possible\");",
        /* newAssertion: */ "actual.Should().BeTrue(\"because it's possible\");")]
    [DataRow(
        /* oldAssertion: */ "Assert.True(bool.Parse(\"true\"));",
        /* newAssertion: */ "bool.Parse(\"true\").Should().BeTrue();")]
    [DataRow(
        /* oldAssertion: */ "Assert.True(bool.Parse(\"true\"), \"because it's possible\");",
        /* newAssertion: */ "bool.Parse(\"true\").Should().BeTrue(\"because it's possible\");")]
    [DataRow(
        /* oldAssertion: */ "Assert.True(!actual);",
        /* newAssertion: */ "(!actual).Should().BeTrue();")]
    [DataRow(
        /* oldAssertion: */ "Assert.True(actual == false);",
        /* newAssertion: */ "(actual == false).Should().BeTrue();")]
    [Implemented]
    public Task AssertTrue_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.False(actual);")]
    [DataRow("Assert.False(actual, \"because it's possible\");")]
    [DataRow("Assert.False(bool.Parse(\"false\"));")]
    [DataRow("Assert.False(bool.Parse(\"false\"), \"because it's possible\");")]
    [Implemented]
    public Task AssertFalse_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("bool actual", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.False(actual);",
        /* newAssertion: */ "actual.Should().BeFalse();")]
    [DataRow(
        /* oldAssertion: */ "Assert.False(actual, \"because it's possible\");",
        /* newAssertion: */ "actual.Should().BeFalse(\"because it's possible\");")]
    [DataRow(
        /* oldAssertion: */ "Assert.False(bool.Parse(\"false\"));",
        /* newAssertion: */ "bool.Parse(\"false\").Should().BeFalse();")]
    [DataRow(
        /* oldAssertion: */ "Assert.False(bool.Parse(\"false\"), \"because it's possible\");",
        /* newAssertion: */ "bool.Parse(\"false\").Should().BeFalse(\"because it's possible\");")]
    [Implemented]
    public Task AssertFalse_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("bool actual", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Same(expected, actual);")]
    [Implemented]
    public Task AssertSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Same(expected, actual);",
        /* newAssertion: */ "actual.Should().BeSameAs(expected);")]
    [Implemented]
    public Task AssertSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotSame(expected, actual);")]
    [Implemented]
    public Task AssertNotSame_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotSame(expected, actual);",
        /* newAssertion: */ "actual.Should().NotBeSameAs(expected);")]
    [Implemented]
    public Task AssertNotSame_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, tolerance);")]
    [DataRow("Assert.Equal(expected, actual, 0.6f);")]
    [Implemented]
    public Task AssertFloatEqualWithTolerance_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("float actual, float expected, float tolerance", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, tolerance);",
        /* newAssertion: */ "actual.Should().BeApproximately(expected, tolerance);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, 0.6f);",
        /* newAssertion: */ "actual.Should().BeApproximately(expected, 0.6f);")]
    [Implemented]
    public Task AssertFloatEqualWithTolerance_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("float actual, float expected, float tolerance", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, tolerance);")]
    [DataRow("Assert.Equal(expected, actual, 0.6);")]
    [Implemented]
    public Task AssertDoubleEqualWithTolerance_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("double actual, double expected, double tolerance", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, tolerance);",
        /* newAssertion: */ "actual.Should().BeApproximately(expected, tolerance);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, 0.6);",
        /* newAssertion: */ "actual.Should().BeApproximately(expected, 0.6);")]
    [Implemented]
    public Task AssertDoubleEqualWithTolerance_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, double tolerance", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, precision);")]
    [DataRow("Assert.Equal(expected, actual, 5);")]
    [Implemented]
    public Task AssertDoubleEqualWithPrecision_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("double actual, double expected, int precision", assertion);

    // There is no corresponding API in AwesomeAssertions
    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, precision);",
        /* newAssertion: */ "Assert.Equal(expected, actual, precision);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, 5);",
        /* newAssertion: */ "Assert.Equal(expected, actual, 5);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, precision, rounding);",
        /* newAssertion: */ "Assert.Equal(expected, actual, precision, rounding);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, 5, MidpointRounding.ToEven);",
        /* newAssertion: */ "Assert.Equal(expected, actual, 5, MidpointRounding.ToEven);")]
    [Implemented]
    public Task AssertDoubleEqualWithPrecision_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, int precision, MidpointRounding rounding", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, precision);")]
    [DataRow("Assert.Equal(expected, actual, 5);")]
    [Implemented]
    public Task AssertDecimalEqualWithPrecision_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("decimal actual, decimal expected, int precision", assertion);

    // There is no corresponding API in AwesomeAssertions
    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, precision);",
        /* newAssertion: */ "Assert.Equal(expected, actual, precision);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, 5);",
        /* newAssertion: */ "Assert.Equal(expected, actual, 5);")]
    [Implemented]
    public Task AssertDecimalEqualWithPrecision_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("decimal actual, decimal expected, int precision", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, precision);")]
    [DataRow("Assert.Equal(expected, actual, TimeSpan.FromSeconds(1));")]
    [Implemented]
    public Task AssertDateTimeEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("DateTime actual, DateTime expected, TimeSpan precision", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, precision);",
        /* newAssertion: */ "actual.Should().BeCloseTo(expected, precision);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, TimeSpan.FromSeconds(1));",
        /* newAssertion: */ "actual.Should().BeCloseTo(expected, TimeSpan.FromSeconds(1));")]
    [Implemented]
    public Task AssertDateTimeEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("DateTime actual, DateTime expected, TimeSpan precision", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual, comparer);")]
    [DataRow("Assert.Equal(expected, actual, ReferenceEqualityComparer.Instance);")]
    [Implemented]
    public Task AssertObjectEqualWithComparer_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual, object expected, IEqualityComparer<object> comparer", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, comparer);",
        /* newAssertion: */ "actual.Should().BeEquivalentTo(expected, options => options.Using(comparer));")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual, ReferenceEqualityComparer.Instance);",
        /* newAssertion: */ "actual.Should().BeEquivalentTo(expected, options => options.Using(ReferenceEqualityComparer.Instance));")]
    [Implemented]
    public Task AssertObjectEqualWithComparer_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected, IEqualityComparer<object> comparer", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual);")]
    [Implemented]
    public Task AssertObjectEqual_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual);",
        /* newAssertion: */ "actual.Should().Be(expected);")]
    [Implemented]
    public Task AssertObjectEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Equal(expected, actual);")]
    [Implemented]
    public Task AssertStringEqual_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equal(expected, actual);",
        /* newAssertion: */ "actual.Should().Be(expected);")]
    [Implemented]
    public Task AssertStringEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.StrictEqual(expected, actual);")]
    [Implemented]
    public Task AssertStrictEqual_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.StrictEqual(expected, actual);",
        /* newAssertion: */ "actual.Should().Be(expected);")]
    [Implemented]
    public Task AssertStrictEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotEqual(expected, actual, precision);")]
    [DataRow("Assert.NotEqual(expected, actual, 5);")]
    [Implemented]
    public Task AssertDoubleNotEqualWithPrecision_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("double actual, double expected, int precision", assertion);

    // There is no corresponding API in AwesomeAssertions
    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, precision);",
        /* newAssertion: */ "Assert.NotEqual(expected, actual, precision);")]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, 5);",
        /* newAssertion: */ "Assert.NotEqual(expected, actual, 5);")]
    [Implemented]
    public Task AssertDoubleNotEqualWithPrecision_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("double actual, double expected, int precision", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotEqual(expected, actual, precision);")]
    [DataRow("Assert.NotEqual(expected, actual, 5);")]
    [Implemented]
    public Task AssertDecimalNotEqualWithPrecision_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("decimal actual, decimal expected, int precision", assertion);

    // There is no corresponding API in AwesomeAssertions
    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, precision);",
        /* newAssertion: */ "Assert.NotEqual(expected, actual, precision);")]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, 5);",
        /* newAssertion: */ "Assert.NotEqual(expected, actual, 5);")]
    [Implemented]
    public Task AssertDecimalNotEqualWithPrecision_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("decimal actual, decimal expected, int precision", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotEqual(expected, actual, comparer);")]
    [DataRow("Assert.NotEqual(expected, actual, ReferenceEqualityComparer.Instance);")]
    [Implemented]
    public Task AssertObjectNotEqualWithComparer_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual, object expected, IEqualityComparer<object> comparer", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, comparer);",
        /* newAssertion: */ "actual.Should().NotBeEquivalentTo(expected, options => options.Using(comparer));")]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual, ReferenceEqualityComparer.Instance);",
        /* newAssertion: */ "actual.Should().NotBeEquivalentTo(expected, options => options.Using(ReferenceEqualityComparer.Instance));")]
    [Implemented]
    public Task AssertObjectNotEqualWithComparer_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected, IEqualityComparer<object> comparer", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotEqual(expected, actual);")]
    [Implemented]
    public Task AssertObjectNotEqual_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotEqual(expected, actual);",
        /* newAssertion: */ "actual.Should().NotBe(expected);")]
    [Implemented]
    public Task AssertObjectNotEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotStrictEqual(expected, actual);")]
    [Implemented]
    public Task AssertObjectNotStrictEqual_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual, object expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotStrictEqual(expected, actual);",
        /* newAssertion: */ "actual.Should().NotBe(expected);")]
    [Implemented]
    public Task AssertObjectNotStrictEqual_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Null(actual);")]
    [Implemented]
    public Task AssertNull_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Null(actual);",
        /* newAssertion: */ "actual.Should().BeNull();")]
    [Implemented]
    public Task AssertNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.NotNull(actual);")]
    [Implemented]
    public Task AssertNotNull_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("object actual", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.NotNull(actual);",
        /* newAssertion: */ "actual.Should().NotBeNull();")]
    [Implemented]
    public Task AssertNotNull_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Contains(expected, actual);")]
    [Implemented]
    public Task AssertStringContains_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Contains(expected, actual);",
        /* newAssertion: */ "actual.Should().Contain(expected);")]
    [Implemented]
    public Task AssertStringContains_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Contains(expected, actual);", "ISet<string> actual, string expected")]
    [DataRow("Assert.Contains(expected, actual);", "IReadOnlySet<string> actual, string expected")]
    [DataRow("Assert.Contains(expected, actual);", "HashSet<string> actual, string expected")]
    [DataRow("Assert.Contains(expected, actual);", "ImmutableHashSet<string> actual, string expected")]
    [Implemented]
    public Task AssertSetContains_TestAnalyzer(string assertion, string arguments) =>
        VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Contains(expected, actual);",
        /* newAssertion: */ "actual.Should().Contain(expected);",
        /* arguments: */ "ISet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.Contains(expected, actual);",
        /* newAssertion: */ "actual.Should().Contain(expected);",
        /* arguments: */ "IReadOnlySet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.Contains(expected, actual);",
        /* newAssertion: */ "actual.Should().Contain(expected);",
        /* arguments: */ "HashSet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.Contains(expected, actual);",
        /* newAssertion: */ "actual.Should().Contain(expected);",
        /* arguments: */ "ImmutableHashSet<string> actual, string expected")]
    [Implemented]
    public Task AssertSetContains_TestCodeFix(string oldAssertion, string newAssertion, string arguments)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.DoesNotContain(expected, actual);")]
    [Implemented]
    public Task AssertStringDoesNotContain_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotContain(expected, actual);",
        /* newAssertion: */ "actual.Should().NotContain(expected);")]
    [Implemented]
    public Task AssertStringDoesNotContain_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.DoesNotContain(expected, actual);", "ISet<string> actual, string expected")]
    [DataRow("Assert.DoesNotContain(expected, actual);", "IReadOnlySet<string> actual, string expected")]
    [DataRow("Assert.DoesNotContain(expected, actual);", "HashSet<string> actual, string expected")]
    [DataRow("Assert.DoesNotContain(expected, actual);", "ImmutableHashSet<string> actual, string expected")]
    [Implemented]
    public Task AssertSetDoesNotContain_TestAnalyzer(string assertion, string arguments) =>
        VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotContain(expected, actual);",
        /* newAssertion: */ "actual.Should().NotContain(expected);",
        /* arguments: */ "ISet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotContain(expected, actual);",
        /* newAssertion: */ "actual.Should().NotContain(expected);",
        /* arguments: */ "IReadOnlySet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotContain(expected, actual);",
        /* newAssertion: */ "actual.Should().NotContain(expected);",
        /* arguments: */ "HashSet<string> actual, string expected")]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotContain(expected, actual);",
        /* newAssertion: */ "actual.Should().NotContain(expected);",
        /* arguments: */ "ImmutableHashSet<string> actual, string expected")]
    [Implemented]
    public Task AssertSetDoesNotContain_TestCodeFix(string oldAssertion, string newAssertion, string arguments)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Matches(expectedRegexPattern, actual);")]
    [Implemented]
    public Task AssertStringMatches_String_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expectedRegexPattern", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Matches(expectedRegexPattern, actual);",
        /* newAssertion: */ "actual.Should().MatchRegex(expectedRegexPattern);")]
    [Implemented]
    public Task AssertStringMatches_String_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expectedRegexPattern", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Matches(expectedRegex, actual);")]
    [Implemented]
    public Task AssertStringMatches_Regex_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Regex expectedRegex", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Matches(expectedRegex, actual);",
        /* newAssertion: */ "actual.Should().MatchRegex(expectedRegex);")]
    [Implemented]
    public Task AssertStringMatches_Regex_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Regex expectedRegex", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.DoesNotMatch(expectedRegexPattern, actual);")]
    [Implemented]
    public Task AssertStringDoesNotMatch_String_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expectedRegexPattern", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotMatch(expectedRegexPattern, actual);",
        /* newAssertion: */ "actual.Should().NotMatchRegex(expectedRegexPattern);")]
    [Implemented]
    public Task AssertStringDoesNotMatch_String_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expectedRegexPattern", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.DoesNotMatch(expectedRegex, actual);")]
    [Implemented]
    public Task AssertStringDoesNotMatch_Regex_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Regex expectedRegex", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.DoesNotMatch(expectedRegex, actual);",
        /* newAssertion: */ "actual.Should().NotMatchRegex(expectedRegex);")]
    [Implemented]
    public Task AssertStringDoesNotMatch_Regex_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Regex expectedRegex", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Empty(actual);")]
    [Implemented]
    public Task AssertEmpty_String_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Empty(actual);",
        /* newAssertion: */ "actual.Should().BeEmpty();")]
    [Implemented]
    public Task AssertEmpty_String_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.EndsWith(expected, actual);")]
    [Implemented]
    public Task AssertEndsWith_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.EndsWith(expected, actual);",
        /* newAssertion: */ "actual.Should().EndWith(expected);")]
    [Implemented]
    public Task AssertEndsWith_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.StartsWith(expected, actual);")]
    [Implemented]
    public Task AssertStartsWith_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, string expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.StartsWith(expected, actual);",
        /* newAssertion: */ "actual.Should().StartWith(expected);")]
    [Implemented]
    public Task AssertStartsWith_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, string expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.Subset(expected, actual);")]
    [Implemented]
    public Task AssertSubset_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("ISet<string> actual, ISet<string> expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Subset(expected, actual);",
        /* newAssertion: */ "actual.Should().BeSubsetOf(expected);")]
    [Implemented]
    public Task AssertSubset_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("ISet<string> actual, ISet<string> expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.IsAssignableFrom(expected, actual);")]
    [DataRow("Assert.IsAssignableFrom(typeof(string), actual);")]
    [DataRow("Assert.IsAssignableFrom<string>(actual);")]
    [Implemented]
    public Task AssertIsAssignableFrom_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Type expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.IsAssignableFrom(expected, actual);",
        /* newAssertion: */ "actual.Should().BeAssignableTo(expected);")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsAssignableFrom(typeof(string), actual);",
        /* newAssertion: */ "actual.Should().BeAssignableTo<string>();")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsAssignableFrom<string>(actual);",
        /* newAssertion: */ "actual.Should().BeAssignableTo<string>();")]
    [Implemented]
    public Task AssertIsAssignableFrom_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Type expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.IsNotAssignableFrom(expected, actual);")]
    [DataRow("Assert.IsNotAssignableFrom(typeof(string), actual);")]
    [DataRow("Assert.IsNotAssignableFrom<string>(actual);")]
    [Implemented]
    public Task AssertIsNotAssignableFrom_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Type expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotAssignableFrom(expected, actual);",
        /* newAssertion: */ "actual.Should().NotBeAssignableTo(expected);")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotAssignableFrom(typeof(string), actual);",
        /* newAssertion: */ "actual.Should().NotBeAssignableTo<string>();")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotAssignableFrom<string>(actual);",
        /* newAssertion: */ "actual.Should().NotBeAssignableTo<string>();")]
    [Implemented]
    public Task AssertIsNotAssignableFrom_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Type expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.IsType(expected, actual);")]
    [DataRow("Assert.IsType(typeof(string), actual);")]
    [DataRow("Assert.IsType<string>(actual);")]
    [Implemented]
    public Task AssertIsType_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Type expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.IsType(expected, actual);",
        /* newAssertion: */ "actual.Should().BeOfType(expected);")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsType(typeof(string), actual);",
        /* newAssertion: */ "actual.Should().BeOfType<string>();")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsType<string>(actual);",
        /* newAssertion: */ "actual.Should().BeOfType<string>();")]
    [Implemented]
    public Task AssertIsType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Type expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.IsNotType(expected, actual);")]
    [DataRow("Assert.IsNotType(typeof(string), actual);")]
    [DataRow("Assert.IsNotType<string>(actual);")]
    [Implemented]
    public Task AssertIsNotType_TestAnalyzer(string assertion) =>
        VerifyCSharpDiagnostic("string actual, Type expected", assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotType(expected, actual);",
        /* newAssertion: */ "actual.Should().NotBeOfType(expected);")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotType(typeof(string), actual);",
        /* newAssertion: */ "actual.Should().NotBeOfType<string>();")]
    [DataRow(
        /* oldAssertion: */ "Assert.IsNotType<string>(actual);",
        /* newAssertion: */ "actual.Should().NotBeOfType<string>();")]
    [Implemented]
    public Task AssertIsNotType_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("string actual, Type expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Assert.InRange(actual, low, high);")]
    [Implemented]
    public Task AssertInRange_TestAnalyzer(string assertion)
    {
        return Task.WhenAll(
            VerifyCSharpDiagnostic("double actual, double low, double high", assertion),
            VerifyCSharpDiagnostic("float actual, float low, float high", assertion),
            VerifyCSharpDiagnostic("int actual, int low, int high", assertion),
            VerifyCSharpDiagnostic("long actual, long low, long high", assertion));
    }

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.InRange(actual, low, high);",
        /* newAssertion: */ "actual.Should().BeInRange(low, high);")]
    [Implemented]
    public Task AssertInRange_TestCodeFix(string oldAssertion, string newAssertion)
    {
        return Task.WhenAll(
            VerifyCSharpFix("double actual, double low, double high", oldAssertion, newAssertion),
            VerifyCSharpFix("float actual, float low, float high", oldAssertion, newAssertion),
            VerifyCSharpFix("int actual, int low, int high", oldAssertion, newAssertion),
            VerifyCSharpFix("long actual, long low, long high", oldAssertion, newAssertion));
    }

    [TestMethod]
    [DataRow("object actual, object expected", "Assert.Equivalent(expected, actual);")]
    [DataRow("object actual, object expected", "Assert.Equivalent(expected, actual, false);")]
    [DataRow("DateTime actual, DateTime expected", "Assert.Equivalent(expected, actual);")]
    [DataRow("DateTime actual, DateTime expected", "Assert.Equivalent(expected, actual, false);")]
    [DataRow("int actual, int expected", "Assert.Equivalent(expected, actual);")]
    [DataRow("int actual, int expected", "Assert.Equivalent(expected, actual, false);")]
    [Implemented]
    public Task AssertEquivalent_TestAnalyzer(string arguments, string assertion) =>
        VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow(
        /* oldAssertion: */ "Assert.Equivalent(expected, actual);",
        /* newAssertion: */ "actual.Should().BeEquivalentTo(expected);")]
    [DataRow(
        /* oldAssertion: */ "Assert.Equivalent(expected, actual, false);",
        /* newAssertion: */ "actual.Should().BeEquivalentTo(expected);")]
    [Implemented]
    public Task AssertEquivalent_TestCodeFix(string oldAssertion, string newAssertion)
        => VerifyCSharpFix("object actual, object expected", oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Action action", "Assert.Throws(typeof(ArgumentException), action);")]
    [DataRow("Action action, Type exceptionType", "Assert.Throws(exceptionType, action);")]
    [DataRow("Action action", "Assert.Throws<NullReferenceException>(action);")]
    [DataRow("Action action", "Assert.Throws<ArgumentException>(\"propertyName\", action);")]
    [Implemented]
    public Task AssertThrows_TestAnalyzer(string arguments, string assertion)
        => VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow("Action action",
        /* oldAssertion */ "Assert.Throws(typeof(ArgumentException), action);",
        /* newAssertion */ "action.Should().ThrowExactly<ArgumentException>();")]
    [DataRow("Action action",
        /* oldAssertion */ "Assert.Throws<NullReferenceException>(action);",
        /* newAssertion */ "action.Should().ThrowExactly<NullReferenceException>();")]
    [DataRow("Action action",
        /* oldAssertion */ "Assert.Throws<ArgumentException>(\"propertyName\", action);",
        /* newAssertion */ "action.Should().ThrowExactly<ArgumentException>().WithParameterName(\"propertyName\");")]
    [Implemented]
    public Task AssertThrows_TestCodeFix(string arguments, string oldAssertion, string newAssertion)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Func<Task> action", "Assert.ThrowsAsync(typeof(ArgumentException), action);")]
    [DataRow("Func<Task> action, Type exceptionType", "Assert.ThrowsAsync(exceptionType, action);")]
    [DataRow("Func<Task> action", "Assert.ThrowsAsync<NullReferenceException>(action);")]
    [DataRow("Func<Task> action", "Assert.ThrowsAsync<ArgumentException>(\"propertyName\", action);")]
    [Implemented]
    public Task AssertThrowsAsync_TestAnalyzer(string arguments, string assertion)
        => VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow("Func<Task> action",
        /* oldAssertion */ "Assert.ThrowsAsync(typeof(ArgumentException), action);",
        /* newAssertion */ "action.Should().ThrowExactlyAsync<ArgumentException>();")]
    [DataRow("Func<Task> action",
        /* oldAssertion */ "Assert.ThrowsAsync<NullReferenceException>(action);",
        /* newAssertion */ "action.Should().ThrowExactlyAsync<NullReferenceException>();")]
    [DataRow("Func<Task> action",
        /* oldAssertion */ "Assert.ThrowsAsync<ArgumentException>(\"propertyName\", action);",
        /* newAssertion */ "action.Should().ThrowExactlyAsync<ArgumentException>().WithParameterName(\"propertyName\");")]
    [Implemented]
    public Task AssertThrowsAsync_TestCodeFix(string arguments, string oldAssertion, string newAssertion)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Action action", "Assert.ThrowsAny<NullReferenceException>(action);")]
    [Implemented]
    public Task AssertThrowsAny_TestAnalyzer(string arguments, string assertion)
        => VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow("Action action",
        /* oldAssertion */ "Assert.ThrowsAny<NullReferenceException>(action);",
        /* newAssertion */ "action.Should().Throw<NullReferenceException>();")]
    [Implemented]
    public Task AssertThrowsAny_TestCodeFix(string arguments, string oldAssertion, string newAssertion)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    [TestMethod]
    [DataRow("Func<Task> action", "Assert.ThrowsAnyAsync<NullReferenceException>(action);")]
    [Implemented]
    public Task AssertThrowsAnyAsync_TestAnalyzer(string arguments, string assertion)
        => VerifyCSharpDiagnostic(arguments, assertion);

    [TestMethod]
    [DataRow("Func<Task> action",
        /* oldAssertion */ "Assert.ThrowsAnyAsync<NullReferenceException>(action);",
        /* newAssertion */ "action.Should().ThrowAsync<NullReferenceException>();")]
    [Implemented]
    public Task AssertThrowsAnyAsync_TestCodeFix(string arguments, string oldAssertion, string newAssertion)
        => VerifyCSharpFix(arguments, oldAssertion, newAssertion);

    private static Task VerifyCSharpDiagnostic(string methodArguments, string assertion)
    {
        var source = GenerateCode.XunitAssertion(methodArguments, assertion);
        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.XunitAssert_2_5_1)
            .WithExpectedDiagnostics(new DiagnosticResult
            {
                Id = AssertAnalyzer.XunitRule.Id,
                Message = AssertAnalyzer.Message,
                Locations =
                [
                    new("Test0.cs", 15, 13)
                ],
                Severity = DiagnosticSeverity.Info
            })
        );
    }

    private static Task VerifyCSharpFix(string methodArguments, string oldAssertion, string newAssertion)
    {
        var oldSource = GenerateCode.XunitAssertion(methodArguments, oldAssertion);
        var newSource = GenerateCode.XunitAssertion(methodArguments, newAssertion);

        return DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithDiagnosticAnalyzer<AssertAnalyzer>()
            .WithCodeFixProvider<XunitCodeFixProvider>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.XunitAssert_2_5_1)
        );
    }
}
