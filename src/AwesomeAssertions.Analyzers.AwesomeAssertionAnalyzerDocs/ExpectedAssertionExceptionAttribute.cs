using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs;

/// <summary>
/// Marks a test method that is expected to throw an <see cref="AssertFailedException"/>.
/// Used in analyzer documentation tests to verify that the "bad" code samples (the ones the analyzer warns against)
/// actually produce assertion failures at runtime, confirming the docs examples are accurate.
/// Derives from <see cref="TestMethodAttribute"/> and overrides <see cref="ExecuteAsync"/> because MSTest 4
/// removed the old <c>[ExpectedException]</c> hook and requires custom test execution via attribute inheritance.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ExpectedAssertionExceptionAttribute : TestMethodAttribute
{
    public ExpectedAssertionExceptionAttribute(
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        : base(filePath, lineNumber)
    {
    }

    public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
    {
        var results = await base.ExecuteAsync(testMethod);

        for (var i = 0; i < results.Length; i++)
        {
            NormalizeResult(results[i]);
        }

        return results;
    }

    private static void NormalizeResult(TestResult result)
    {
        if (result.Outcome == UnitTestOutcome.Passed)
        {
            result.Outcome = UnitTestOutcome.Failed;
            result.TestFailureException = new AssertFailedException($"Expected {nameof(AssertFailedException)} but no exception was thrown.");
            return;
        }
        else if (result.Outcome == UnitTestOutcome.Failed)
        {
            switch (result.TestFailureException?.InnerException)
            {
                case AssertFailedException:
                    result.Outcome = UnitTestOutcome.Passed;
                    result.TestFailureException = null;
                    break;
                case null:
                    result.TestFailureException = new AssertFailedException($"Expected {nameof(AssertFailedException)} but no exception was thrown.");
                    break;
                default:
                    var innerException = result.TestFailureException;
                    result.TestFailureException = new AssertFailedException($"Expected {nameof(AssertFailedException)} but got {innerException.GetType().Name}.", innerException);
                    break;
            }
        }
    }
}