using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class SanityTests
{
    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/11")]
    public Task CountWithPredicate()
    {
        const string assertion = "actual.Count(d => d.Message.Contains(\"a\")).Should().Be(2);";
        var source = GenerateCode.GenericIListCodeBlockAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/10")]
    public Task AssertionCallMultipleMethodWithTheSameNameAndArguments()
    {
        const string assertion = "actual.Should().Contain(d => d.Message.Contains(\"a\")).And.Contain(d => d.Message.Contains(\"c\"));";
        var source = GenerateCode.GenericIListCodeBlockAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/44")]
    public Task CollectionShouldHaveElementAt_ShouldIgnoreDictionaryTypes()
    {
        string source = GenerateCode.GenericIDictionaryAssertion("actual[\"key\"].Should().Be(expectedValue);");
        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/13")]
    public Task PropertyOfIndexerShouldBe_ShouldNotThrowException()
    {
        const string assertion = "actual[0].Message.Should().Be(\"test\");";
        var source = GenerateCode.GenericIListCodeBlockAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/13")]
    public Task PropertyOfElementAtShouldBe_ShouldNotTriggerDiagnostic()
    {
        const string assertion = "actual.ElementAt(0).Message.Should().Be(\"test\");";
        var source = GenerateCode.GenericIListCodeBlockAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/10")]
    public Task NestedAssertions_ShouldNotTrigger()
    {
        const string declaration = "var nestedList = new List<List<int>>();";
        const string assertion = "nestedList.Should().NotBeNull().And.ContainSingle().Which.Should().NotBeEmpty();";
        var source = GenerateCode.GenericIListCodeBlockAssertion(declaration + assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/18")]
    public Task DictionaryShouldContainPair_WhenPropertiesOfDifferentVariables_ShouldNotTrigger()
    {
        const string assertion = "actual.Should().ContainValue(pair.Value).And.ContainKey(otherPair.Key);";
        var source = GenerateCode.GenericIDictionaryAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/41")]
    public Task ExpressionBasedFunction_ShouldNotThrow()
    {
        const string source = """
            public class TestClass
            {
                private SomeClass CreateSomeClass() => new SomeClass();

                public class SomeClass
                { }
                public static void Main() { }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/58")]
    public Task StaticWithNameof_ShouldNotThrow()
    {
        const string source = """
            public class TestClass
            {
                private static string StaticResult { get; set; }

                public static void Main()
                {
                    StaticResult = nameof(Main);
                }
            }
            """;
        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/49")]
    public Task WritingToConsole_ShouldNotThrow()
    {
        const string source = """
            public class TestClass
            {
                public static void Main()
                {
                    System.Console.WriteLine();
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/63")]
    public Task Collection_SelectWhereShouldOnlyHaveUniqueItems_ShouldNotTrigger()
    {
        const string source = """
            using System.Linq;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestNamespace
            {
                public class Program
                {
                    public static void Main()
                    {
                        var list = new[] { 1, 2, 3 };

                        list.Select(e => e.ToString())
                            .Where(e => e != string.Empty)
                            .Should()
                            .OnlyHaveUniqueItems();
                    }
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/63")]
    public Task StringShouldNotBeEmptyAndShouldNotBeNull_ShouldNotTrigger()
    {
        const string assertion = "actual.Should().NotBeEmpty().And.Subject.Should().NotBeNull();";
        var source = GenerateCode.StringAssertion(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/65")]
    public Task CustomClass_ShouldNotTrigger_DictionaryAnalyzers()
    {
        const string source = """
            using System.Linq;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestNamespace
            {
                class MyDict<TKey, TValue>
                {
                    public bool ContainsKey(TKey key) => false;
                }

                public class Program
                {
                    public static void Main()
                    {
                        var dict = new MyDict<int, string>();
                        dict.ContainsKey(0).Should().BeTrue();
                    }
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/64")]
    public Task CollectionShouldNotContainProperty_WhenAssertionIsIdiomatic_ShouldNotTrigger()
    {
        const string source = """
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            public class TestClass
            {
                public static void Main()
                {
                    var list = new[] { string.Empty };
                    list.Should().OnlyContain(e => e.Contains(string.Empty));
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/66")]
    public Task CollectionShouldHaveElementAt_ShouldNotThrow()
    {
        const string source = """
            using System.Linq;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestNamespace
            {
                public class Program
                {
                    public static void Main()
                    {
                        var list = new[] { " FOO " };
                        list[0].Trim().Should().Be("FOO");
                    }
                }
            }
            """;

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/172")]
    public Task AssertAreEqualDoesNotCompile()
    {
        const string oldSource = """
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestProject
            {
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public class Program
                {
                    public static void Main()
                    {
                        double x = 5;

                        Assert.AreEqual(1, (int)x);
                    }
                }
            }
            """;
        const string newSource = """
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestProject
            {
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public class Program
                {
                    public static void Main()
                    {
                        double x = 5;

                        ((int)x).Should().Be(1);
                    }
                }
            }
            """;

        return DiagnosticVerifier.VerifyFix(new CodeFixVerifierArguments()
            .WithCodeFixProvider<MsTestCodeFixProvider>()
            .WithDiagnosticAnalyzer<AssertAnalyzer>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.MSTestTestFramework_3_1_1)
        );
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/163")]
    public Task GlobalUsingsWithAssert()
    {
        const string globalUsings = """
            global using Xunit;
            global using AwesomeAssertions;
            global using AwesomeAssertions.Extensions;
            """;
        const string source = """
            public class TestClass
            {
                public static void Main()
                {
                    var x = false;
                    Assert.True(x);
                    TestClass.True(x);
                }

                public static void True(bool input)
                {
                }
            }
            """;

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source, globalUsings)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.XunitAssert_2_5_1, PackageReference.AwesomeAssertions_latest)
            .WithExpectedDiagnostics(new DiagnosticResult()
            {
                Id = AssertAnalyzer.XunitRule.Id,
                Message = AssertAnalyzer.Message,
                Severity = DiagnosticSeverity.Info,
                Locations = [new DiagnosticResultLocation("Test0.cs", 6, 9)]
            })
        );
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/207")]
    public Task PropertiesOfTypes()
    {
        const string source = """
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Collections.Generic;
            using System.Linq;
            public class TestClass
            {
                public static void Main()
                {
                    var x = new TestType1();
                    x.Prop1.Prop2.List.Any().Should().BeTrue();
                    x.Prop1.Prop2.Count.Should().BeGreaterThan(10);
                }
            }

            public class TestType1
            {
                public TestType2 Prop1 { get; set; }
            }
            public class TestType2
            {
                public TestType3 Prop2 { get; set; }
            }
            public class TestType3
            {
                public List<int> List { get; set; }
                public int Count { get; set; }
            }
            """;

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            .WithExpectedDiagnostics(new DiagnosticResult()
            {
                Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                Message = DiagnosticMetadata.CollectionShouldNotBeEmpty_AnyShouldBeTrue.Message,
                Severity = DiagnosticSeverity.Info,
                Locations = [new DiagnosticResultLocation("Test0.cs", 10, 9)]
            })
        );
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/215")]
    public Task ShouldNotFailToAnalyze1()
    {
        const string source = """
            #nullable enable
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System;
            public class TestClass
            {
                public static void Main()
                {
                    var otherComponent = new OtherComponent();
                    otherComponent.Name.Should().Be("SomeOtherComponent");
                    otherComponent.Version.Should().Be("1.2.3");
                    otherComponent.DownloadUrl.Should().Be(new Uri("https://sampleurl.com"));
                    otherComponent.Hash.Should().Be("SampleHash");
                }
            }

            public class OtherComponent
            {
                public string? Name { get; set; }

                public string? Version { get; set; }

                public Uri? DownloadUrl { get; set; }

                public string? Hash { get; set; }
            }
            """;

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/276")]
    public Task ShouldNotFailToAnalyze2()
    {
        const string source = """
            #nullable enable            
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System.Linq;
            using System.Collections.Generic;

            public class TestClass
            {
                public static void Main()
                {
                    var response = new MyResponse();
                    response.MyCollection?.Count().Should().Be(2);
                }
            }

            public class MyResponse
            {
                public IEnumerable<MyCollectionType>? MyCollection { get; set; }
            }
            public class MyCollectionType { }
            """;

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            .WithExpectedDiagnostics(new DiagnosticResult()
            {
                Id = AwesomeAssertionsAnalyzer.DiagnosticId,
                Message = DiagnosticMetadata.NullConditionalMayNotExecute.Message,
                Severity = DiagnosticSeverity.Info,
                Locations = [new DiagnosticResultLocation("Test0.cs", 12, 9)]
            })
        );
    }

    [TestMethod]
    [Implemented(Reason = "https://github.com/fluentassertions/fluentassertions.analyzers/issues/290")]
    public Task ShouldNotReportIssue290()
    {
        const string source = """
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using System;
            using System.Collections.Generic;
            using System.Linq;
            public class TestClass
            {
                public static void Main()
                {
                    IEnumerable<Item> expectedOrderedNames = new[] { new Item("Alpha"), new Item("Bravo"), new Item("Charlie") };
                    IEnumerable<Parent> actual = GetSortedItems();

                    actual.Select(x => x.Item).Should().Equal(expectedOrderedNames);
                }

                static IEnumerable<Parent> GetSortedItems()
                {
                    yield return new Parent("Bravo");
                    yield return new Parent("Charlie");
                    yield return new Parent("Alpha");
                }
            }

            public class Item
            {
                public string Name { get; set; }
                public Guid Id { get; set; }

                public Item(string name)
                {
                    Name = name;
                    Id = Guid.NewGuid();
                }
            }

            public class Parent
            {
                public Item Item { get; set; }

                public Parent(string name)
                {
                    Item = new Item(name);
                }
            }
            """;

        return DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithSources(source)
            .WithAllAnalyzers()
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
    }
}
