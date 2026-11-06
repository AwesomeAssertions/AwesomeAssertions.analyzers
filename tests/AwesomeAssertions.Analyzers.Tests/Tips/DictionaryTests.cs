using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class DictionaryTests
{
    [TestMethod]
    [AssertionDiagnostic("actual.ContainsKey(expectedKey).Should().BeTrue({0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).ContainsKey(expectedKey).Should().BeTrue({0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainKey_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainKey_ContainsKeyShouldBeTrue);

    [TestMethod]
    [Implemented]
    public Task DictionaryMethods_CustomMethods_TestNoAnalyzer()
    {
        const string code = """
            using System.Collections.Generic;
            using AwesomeAssertions;

            namespace TestNamespace
            {
                public class MultiKeyDict<TKey1, TKey2, TValue> : Dictionary<TKey1, Dictionary<TKey2, TValue>>
                {
                    public bool ContainsKey(TKey1 key1, TKey2 key2) => false;
                    public bool ContainsValue(TKey1 key1, TKey2 key2) => false;
                }

                public class TestClass
                {
                    public void TestMethod(MultiKeyDict<int, int, string> actual)
                    {
                        actual.ContainsKey(0, 1).Should().BeTrue();
                        actual.ContainsValue(0, 1).Should().BeTrue();
                    }
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(code);
    }

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.ContainsKey(expectedKey).Should().BeTrue({0});",
        newAssertion: "actual.Should().ContainKey(expectedKey{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).ContainsKey(expectedKey).Should().BeTrue({0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(expectedKey{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainKey_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.ContainsKey(expectedKey).Should().BeFalse({0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).ContainsKey(expectedKey).Should().BeFalse({0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldNotContainKey_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldNotContainKey_ContainsKeyShouldBeFalse);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.ContainsKey(expectedKey).Should().BeFalse({0});",
        newAssertion: "actual.Should().NotContainKey(expectedKey{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).ContainsKey(expectedKey).Should().BeFalse({0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().NotContainKey(expectedKey{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldNotContainKey_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.ContainsValue(expectedValue).Should().BeTrue({0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).ContainsValue(expectedValue).Should().BeTrue({0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainValue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainValue_ContainsValueShouldBeTrue);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.ContainsValue(expectedValue).Should().BeTrue({0});",
        newAssertion: "actual.Should().ContainValue(expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).ContainsValue(expectedValue).Should().BeTrue({0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(expectedValue{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainValue_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.ContainsValue(expectedValue).Should().BeFalse({0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).ContainsValue(expectedValue).Should().BeFalse({0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldNotContainValue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldNotContainValue_ContainsValueShouldBeFalse);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.ContainsValue(expectedValue).Should().BeFalse({0});",
        newAssertion: "actual.Should().NotContainValue(expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).ContainsValue(expectedValue).Should().BeFalse({0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().NotContainValue(expectedValue{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldNotContainValue_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().ContainKey(expectedKey{0}).And.ContainValue(expectedValue);")]
    [AssertionDiagnostic("actual.Should().ContainKey(expectedKey).And.ContainValue(expectedValue{0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(expectedKey{0}).And.ContainValue(expectedValue).And.ToString();")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(expectedKey).And.ContainValue(expectedValue{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainKeyAndValue_ShouldContainKeyAndContainValue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainKeyAndValue_ShouldContainKeyAndContainValue);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().ContainValue(expectedValue{0}).And.ContainKey(expectedKey);")]
    [AssertionDiagnostic("actual.Should().ContainValue(expectedValue).And.ContainKey(expectedKey{0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(expectedValue{0}).And.ContainKey(expectedKey).And.ToString();")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(expectedValue).And.ContainKey(expectedKey{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainKeyAndValue_ShouldContainValueAndContainKey_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainKeyAndValue_ShouldContainValueAndContainKey);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainKey(expectedKey{0}).And.ContainValue(expectedValue);",
        newAssertion: "actual.Should().Contain(expectedKey, expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(expectedKey{0}).And.ContainValue(expectedValue).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(expectedKey, expectedValue{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainKey(expectedKey).And.ContainValue(expectedValue{0});",
        newAssertion: "actual.Should().Contain(expectedKey, expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(expectedKey).And.ContainValue(expectedValue{0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(expectedKey, expectedValue{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainValue(expectedValue{0}).And.ContainKey(expectedKey);",
        newAssertion: "actual.Should().Contain(expectedKey, expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(expectedValue).And.ContainKey(expectedKey{0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(expectedKey, expectedValue{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainValue(expectedValue).And.ContainKey(expectedKey{0});",
        newAssertion: "actual.Should().Contain(expectedKey, expectedValue{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(expectedValue{0}).And.ContainKey(expectedKey).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(expectedKey, expectedValue{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainKeyAndValue_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().ContainKey(pair.Key{0}).And.ContainValue(pair.Value);")]
    [AssertionDiagnostic("actual.Should().ContainKey(pair.Key).And.ContainValue(pair.Value{0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(pair.Key{0}).And.ContainValue(pair.Value).And.ToString();")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(pair.Key).And.ContainValue(pair.Value{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainPair_ShouldContainKeyAndContainValue_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainPair_ShouldContainKeyAndContainValue);

    [TestMethod]
    [AssertionDiagnostic("actual.Should().ContainValue(pair.Value{0}).And.ContainKey(pair.Key);")]
    [AssertionDiagnostic("actual.Should().ContainValue(pair.Value).And.ContainKey(pair.Key{0});")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(pair.Value{0}).And.ContainKey(pair.Key).And.ToString();")]
    [AssertionDiagnostic("actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(pair.Value).And.ContainKey(pair.Key{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainPair_ShouldContainValueAndContainKey_TestAnalyzer(string assertion) => VerifyCSharpDiagnostic(assertion, DiagnosticMetadata.DictionaryShouldContainPair_ShouldContainValueAndContainKey);

    [TestMethod]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainKey(pair.Key{0}).And.ContainValue(pair.Value);",
        newAssertion: "actual.Should().Contain(pair{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(pair.Key{0}).And.ContainValue(pair.Value).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(pair{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainKey(pair.Key).And.ContainValue(pair.Value{0});",
        newAssertion: "actual.Should().Contain(pair{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainKey(pair.Key).And.ContainValue(pair.Value{0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(pair{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainValue(pair.Value{0}).And.ContainKey(pair.Key);",
        newAssertion: "actual.Should().Contain(pair{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(pair.Value{0}).And.ContainKey(pair.Key).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(pair{0}).And.ToString();")]
    [AssertionCodeFix(
        oldAssertion: "actual.Should().ContainValue(pair.Value).And.ContainKey(pair.Key{0});",
        newAssertion: "actual.Should().Contain(pair{0});")]
    [AssertionCodeFix(
        oldAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().ContainValue(pair.Value).And.ContainKey(pair.Key{0}).And.ToString();",
        newAssertion: "actual.ToDictionary(p => p.Key, p=> p.Value).Should().Contain(pair{0}).And.ToString();")]
    [Implemented]
    public Task DictionaryShouldContainPair_TestCodeFix(string oldAssertion, string newAssertion) => VerifyCSharpFix(oldAssertion, newAssertion);

    [TestMethod]
    [Implemented]
    public Task DictionaryWithIntValue_ShouldBe_NoDiagnostic()
    {
        string assertion = "actual[\"one\"].Should().Be(1);";
        string source = GenerateCode.GenericSimpleIDictionaryAssertion(assertion, "int");
        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    private Task VerifyCSharpDiagnostic(string sourceAssersion, DiagnosticMetadata metadata)
    {
        var source = GenerateCode.GenericIDictionaryAssertion(sourceAssersion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source, new DiagnosticResult
        {
            Id = AwesomeAssertionsAnalyzer.DiagnosticId,
            Message = metadata.Message,
            VisitorName = metadata.Name,
            Locations =
            [
                new("Test0.cs", 12,13)
            ],
            Severity = DiagnosticSeverity.Info
        });
    }

    private static Task VerifyCSharpFix(string oldSourceAssertion, string newSourceAssertion)
    {
        var oldSource = GenerateCode.GenericIDictionaryAssertion(oldSourceAssertion);
        var newSource = GenerateCode.GenericIDictionaryAssertion(newSourceAssertion);

        return DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithDiagnosticAnalyzer<AwesomeAssertionsAnalyzer>()
            .WithCodeFixProvider<AwesomeAssertionsCodeFixProvider>()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
    }
}
