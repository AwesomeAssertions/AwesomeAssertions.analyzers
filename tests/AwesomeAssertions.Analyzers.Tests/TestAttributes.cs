using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests;

#pragma warning disable SA1649 // File name should match first type name: Keep all attributes to gether, for the moment
[AttributeUsage(AttributeTargets.Method)]
public class NotImplementedAttribute : TestCategoryBaseAttribute
{
    public string Reason { get; set; }

    public override IList<string> TestCategories => new[] { "NotImplemented" };
}

[AttributeUsage(AttributeTargets.Method)]
public class ImplementedAttribute : TestCategoryBaseAttribute
{
    public string Reason { get; set; }

    public override IList<string> TestCategories => new[] { "Completed" };
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AssertionDiagnosticAttribute : Attribute, ITestDataSource
{
    public string Assertion { get; }

    public string[] AdditionalParameters { get; }

    public AssertionDiagnosticAttribute(string assertion, params string[] additionalParameters)
    {
        Assertion = assertion;
        AdditionalParameters = additionalParameters ?? [];
    }

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var assertion in TestCasesInputUtils.GetTestCases(Assertion))
        {
            yield return new object[] { assertion }.Concat(AdditionalParameters).ToArray();
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name}(\"{string.Join("\", \"", data)}\")";
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IgnoreAssertionDiagnosticAttribute : Attribute
{
    public string Assertion { get; }

    public IgnoreAssertionDiagnosticAttribute(string assertion) => Assertion = assertion;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AssertionCodeFixAttribute : Attribute, ITestDataSource
{
    public string OldAssertion { get; }

    public string NewAssertion { get; }

    public string[] AdditionalParameters { get; }

    public AssertionCodeFixAttribute(string oldAssertion, string newAssertion, params string[] additionalParameters)
    {
        OldAssertion = oldAssertion;
        NewAssertion = newAssertion;
        AdditionalParameters = additionalParameters ?? [];
    }

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach ((string oldAssertion, string newAssertion) in TestCasesInputUtils.GetTestCases(OldAssertion, NewAssertion))
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
        foreach ((string oldAssertion, string newAssertion) in TestCasesInputUtils.GetTestCases(OldAssertion, NewAssertion))
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

public static class TestCasesInputUtils
{
    private const string Because = "\"because it's possible\"";
    private const string FormattedBecause = "\"because message with {0} placeholders {1} at {2}\", 3, \"is awesome\", DateTime.Now.Add(2.Seconds())";

    public static IEnumerable<string> GetTestCases(string assertion)
    {
        if (!assertion.Contains("{0}"))
        {
            yield return assertion;
            yield break;
        }

        yield return SafeFormat(assertion, string.Empty);
        yield return SafeFormat(assertion, Because);
        yield return SafeFormat(assertion, FormattedBecause);
    }

    public static IEnumerable<(string oldAssertion, string newAssertion)> GetTestCases(string oldAssertion, string newAssertion)
    {
        if (!oldAssertion.Contains("{0}") && !newAssertion.Contains("{0}"))
        {
            yield return (oldAssertion, newAssertion);
            yield break;
        }

        yield return (SafeFormat(oldAssertion, string.Empty), SafeFormat(newAssertion, string.Empty));
        yield return (SafeFormat(oldAssertion, Because), SafeFormat(newAssertion, Because));
        yield return (SafeFormat(oldAssertion, FormattedBecause), SafeFormat(newAssertion, FormattedBecause));
    }

    /// <summary>
    /// Adds an comma before the additional argument if needed.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="arg"></param>
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
