using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Analyzers.Tips;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class AsyncVoidTests
{
    [TestMethod]
    [Implemented]
    public Task AssignAsyncVoidMethodToAction_TestAnalyzer()
    {
        const string statement = "Action action = AsyncVoidMethod;";
        var source = GenerateCode.AsyncFunctionStatement(statement);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [TestMethod]
    [Implemented]
    public Task AssignVoidLambdaToAction_TestAnalyzer()
    {
        const string statement = "Action action = () => {};";
        var source = GenerateCode.AsyncFunctionStatement(statement);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source);
    }

    [DataRow("Action action = async () => { await Task.CompletedTask; };")]
    [DataRow("Action action1 = async () => { await Task.CompletedTask; }, action2 = async () => { await Task.CompletedTask; };")]
    [DataRow("Action action1 = () => { }, action2 = async () => { await Task.CompletedTask; };")]
    [DataRow("Action action1 = async () => { await Task.CompletedTask; }, action2 = () => { };")]
    [TestMethod]
    [Implemented]
    public Task AssignAsyncVoidLambdaToAction_TestAnalyzer(string assertion)
    {
        var source = GenerateCode.AsyncFunctionStatement(assertion);

        return DiagnosticVerifier.VerifyCSharpDiagnosticUsingAllAnalyzers(source, new DiagnosticResult
        {
            Id = AsyncVoidAnalyzer.DiagnosticId,
            Message = AsyncVoidAnalyzer.Message,
            Locations =
            [
                new("Test0.cs", 10,13)
            ],
            Severity = DiagnosticSeverity.Warning
        });
    }

    [AssertionCodeFix(
        oldAssertion: "Action action = async () => { await Task.CompletedTask; };",
        newAssertion: "Func<Task> action = async () => { await Task.CompletedTask; };")]
    [AssertionCodeFix(
        oldAssertion: "Action action1 = async () => { await Task.CompletedTask; }, action2 = async () => { await Task.CompletedTask; };",
        newAssertion: "Func<Task> action1 = async () => { await Task.CompletedTask; }, action2 = async () => { await Task.CompletedTask; };")]
    [TestMethod]
    [NotImplemented]
    [Ignore("Reason unknown")]
    public Task AssignAsyncVoidLambdaToAction_TestCodeFix(string oldAssertion, string newAssertion)
    {
        var oldSource = GenerateCode.AsyncFunctionStatement(oldAssertion);
        var newSource = GenerateCode.AsyncFunctionStatement(newAssertion);

        return DiagnosticVerifier.VerifyCSharpFix<AsyncVoidCodeFixProvider, AsyncVoidAnalyzer>(oldSource, newSource);
    }
}
