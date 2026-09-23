using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AwesomeAssertions.Analyzers.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class NotImplementedAttribute : TestCategoryBaseAttribute
{
    public string Reason { get; set; }

    public override IList<string> TestCategories { get; } = ["NotImplemented"];
}

[AttributeUsage(AttributeTargets.Method)]
public class ImplementedAttribute : TestCategoryBaseAttribute
{
    public string Reason { get; set; }

    public override IList<string> TestCategories { get; } = ["Completed"];
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class AssertionDiagnosticBaseAttribute(string assertion, params string[] additionalParameters) : Attribute, ITestDataSource
{
    public string Assertion { get; } = assertion;

    public string[] AdditionalParameters { get; } = additionalParameters ?? [];

    private protected abstract IEnumerable<string> GetTestCases();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var assertion in GetTestCases())
        {
            yield return new object[] { assertion }.Concat(AdditionalParameters).ToArray();
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
        => $"{methodInfo.Name}(\"{string.Join("\", \"", data)}\")";
}

public sealed class AssertionDiagnosticAttribute(string assertion, params string[] additionalParameters)
    : AssertionDiagnosticBaseAttribute(assertion, additionalParameters)
{
    private protected override IEnumerable<string> GetTestCases()
        => TestCasesInputUtils.GetTestCases(Assertion, MessageFormat.Simple | MessageFormat.Because | MessageFormat.FormattedBecause);
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IgnoreAssertionDiagnosticAttribute(string assertion) : Attribute
{
    public string Assertion { get; } = assertion;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public abstract class AssertionCodeFixBaseAttribute(string oldAssertion, string newAssertion, params string[] additionalParameters)
    : Attribute, ITestDataSource
{
    public string OldAssertion { get; } = oldAssertion;
    public string NewAssertion { get; } = newAssertion;
    public string[] AdditionalParameters { get; } = additionalParameters ?? [];

    private protected abstract IEnumerable<(string oldAssertion, string newAssertion)> GetTestCases();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var (oldAssertion, newAssertion) in GetTestCases())
        {
            yield return new object[] { oldAssertion, newAssertion }.Concat(AdditionalParameters).ToArray();
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        if (AdditionalParameters.Length == 0)
        {
            return $"{methodInfo.Name}(\"old: {data[0]}\", \"new: {data[1]}\")";
        }

        return $"{methodInfo.Name}(\"old: {data[0]}\", \"new: {data[1]}\", \"{string.Join("\", \"", AdditionalParameters)}\")";
    }
}

public class AssertionCodeFixAttribute(string oldAssertion, string newAssertion, params string[] additionalParameters)
    : AssertionCodeFixBaseAttribute(oldAssertion, newAssertion, additionalParameters)
{
    private protected override IEnumerable<(string oldAssertion, string newAssertion)> GetTestCases()
        => TestCasesInputUtils.GetTestCases(OldAssertion, NewAssertion);
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IgnoreAssertionCodeFixAttribute : Attribute
{
    public string OldAssertion { get; }
    public string NewAssertion { get; }

    public IgnoreAssertionCodeFixAttribute(string oldAssertion, string newAssertion) => (OldAssertion, NewAssertion) = (oldAssertion, newAssertion);
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AssertionMethodCodeFixAttribute : Attribute, ITestDataSource
{
    public string MethodArguments { get; }
    public string OldAssertion { get; }
    public string NewAssertion { get; }

    public AssertionMethodCodeFixAttribute(string methodArguments, string oldAssertion, string newAssertion)
        => (MethodArguments, OldAssertion, NewAssertion) = (methodArguments, oldAssertion, newAssertion);

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var (oldAssertion, newAssertion) in TestCasesInputUtils.GetTestCases(OldAssertion, NewAssertion))
        {
            yield return new object[] { MethodArguments, oldAssertion, newAssertion };
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name}(\"arguments\":{data[0]}, \"old: {data[1]}\", new: {data[2]}\")";
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IgnoreAssertionMethodCodeFixAttribute : Attribute
{
    public string MethodArguments { get; }
    public string OldAssertion { get; }
    public string NewAssertion { get; }

    public IgnoreAssertionMethodCodeFixAttribute(string methodArguments, string oldAssertion, string newAssertion)
        => (MethodArguments, OldAssertion, NewAssertion) = (methodArguments, oldAssertion, newAssertion);
}

[Flags]
internal enum MessageFormat
{
    Simple = 1,
    Because = 2,
    FormattedBecause = 4,
    InterpolatedBecause = 8,
    Default = Simple | Because | FormattedBecause,
}

internal static class TestCasesInputUtils
{
    private const string Empty = "";
    private const string Because = "\"because it's possible\"";
    private const string FormattedBecause = "\"because message with {0} placeholders {1} at {2}\", 3, \"is awesome\", DateTime.Now.Add(2.Seconds())";
    private const string InterpolatedBecause = "$\"because message with {3} placeholders {DateTime.Now}\"";

    public static IEnumerable<string> GetTestCases(string assertion, MessageFormat messageFormat = MessageFormat.Default)
    {
        if (!assertion.Contains("{0}"))
        {
            yield return assertion;
            yield break;
        }

        if (messageFormat.HasFlag(MessageFormat.Simple))
            yield return SafeFormat(assertion, Empty);

        if (messageFormat.HasFlag(MessageFormat.Because))
            yield return SafeFormat(assertion, Because);

        if (messageFormat.HasFlag(MessageFormat.FormattedBecause))
            yield return SafeFormat(assertion, FormattedBecause);

        if (messageFormat.HasFlag(MessageFormat.InterpolatedBecause))
            yield return SafeFormat(assertion, InterpolatedBecause);
    }

    public static IEnumerable<(string oldAssertion, string newAssertion)> GetTestCases(string oldAssertion, string newAssertion, MessageFormat messageFormat = MessageFormat.Default)
    {
        if (!oldAssertion.Contains("{0}") && !newAssertion.Contains("{0}"))
        {
            yield return (oldAssertion, newAssertion);
            yield break;
        }

        if (messageFormat.HasFlag(MessageFormat.Simple))
            yield return (SafeFormat(oldAssertion, Empty), SafeFormat(newAssertion, Empty));

        if (messageFormat.HasFlag(MessageFormat.Because))
            yield return (SafeFormat(oldAssertion, Because), SafeFormat(newAssertion, Because));

        if (messageFormat.HasFlag(MessageFormat.FormattedBecause))
            yield return (SafeFormat(oldAssertion, FormattedBecause), SafeFormat(newAssertion, FormattedBecause));

        if (messageFormat.HasFlag(MessageFormat.InterpolatedBecause))
            yield return (SafeFormat(oldAssertion, InterpolatedBecause), SafeFormat(newAssertion, InterpolatedBecause));
    }

    /// <summary>
    /// Adds an comma before the additional argument if needed.
    /// </summary>
    private static string SafeFormat(string assertion, string arg)
    {
        var index = assertion.IndexOf("{0}");
        if (!string.IsNullOrEmpty(arg) && assertion[index - 1] != '(')
        {
            return string.Format(assertion, ", " + arg);
        }
        return string.Format(assertion, arg);
    }
}
