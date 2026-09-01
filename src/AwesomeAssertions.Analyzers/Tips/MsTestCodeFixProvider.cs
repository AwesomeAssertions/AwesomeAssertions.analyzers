using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;

namespace AwesomeAssertions.Analyzers;

#pragma warning disable S1192 // String literals should not be duplicated: ok here

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MsTestCodeFixProvider)), Shared]
public class MsTestCodeFixProvider : TestingFrameworkCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AssertAnalyzer.MSTestsRule.Id);

    protected override CreateChangedDocument TryComputeFixCore(IInvocationOperation invocation, CodeFixContext context, TestingFrameworkCodeFixContext t, Diagnostic diagnostic)
    {
        var assertType = invocation.TargetMethod.ContainingType;
        return assertType.Name switch
        {
            "Assert" => TryComputeFixForAssert(invocation, context, t),
            "StringAssert" => TryComputeFixForStringAssert(invocation, context, t),
            "CollectionAssert" => TryComputeFixForCollectionAssert(invocation, context, t),
            _ => null
        };
    }

    private CreateChangedDocument TryComputeFixForAssert(IInvocationOperation invocation, CodeFixContext context, TestingFrameworkCodeFixContext t)
    {
        var actualSubjectIndex = invocation.Arguments.Length is 1 ? 0 
            : invocation.Arguments[1].IsLiteralValue() ? 0 : 1;
        switch (invocation.TargetMethod.Name)
        {
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo): // AreEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.Identity): // AreEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.Identity, t.String, t.String): // AreEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.String, t.ObjectArray): // AreEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? message, params object?[]? parameters)
                {
                    var ignoreCase = invocation.Arguments.Length >= 3 && invocation.Arguments[2].IsLiteralValue(true);
                    var newMethod = ignoreCase ? "BeEquivalentTo" : "Be";

                    if (!invocation.Arguments[3].IsStaticPropertyReference(t.CultureInfo, "CurrentCulture"))
                    {
                        break; // TODO: Handle other cultures
                    }

                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, newMethod, actualSubjectIndex, argumentsToRemove: [2, 3]);
                }
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean): // AreEqual(string? expected, string? actual, bool ignoreCase)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.Identity): // AreEqual(string? expected, string? actual, bool ignoreCase, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.Identity, t.String, t.String): // AreEqual(string? expected, string? actual, bool ignoreCase, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.String, t.ObjectArray): // AreEqual(string? expected, string? actual, bool ignoreCase, string? message, params object?[]? parameters)
                {
                    var ignoreCase = invocation.Arguments.Length >= 3 && invocation.Arguments[2].IsLiteralValue(true);
                    var newMethod = ignoreCase ? "BeEquivalentTo" : "Be";

                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, newMethod, actualSubjectIndex, argumentsToRemove: [2]);
                }
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float): // AreEqual(float expected, float actual, float delta)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.Identity): // AreEqual(float expected, float actual, float delta, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.Identity, t.String, t.String): // AreEqual(float expected, float actual, float delta, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.String, t.ObjectArray): // AreEqual(float expected, float actual, float delta, string? message, params object?[]? parameters)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double): // AreEqual(double expected, double actual, double delta)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.Identity): // AreEqual(double expected, double actual, double delta, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.Identity, t.String, t.String): // AreEqual(double expected, double actual, double delta, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.String, t.ObjectArray): // AreEqual(double expected, double actual, double delta, string? message, params object?[]? parameters)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal): // AreEqual(decimal expected, decimal actual, decimal delta)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.Identity): // AreEqual(decimal expected, decimal actual, decimal delta, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.Identity, t.String, t.String): // AreEqual(decimal expected, decimal actual, decimal delta, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.String, t.ObjectArray): // AreEqual(decimal expected, decimal actual, decimal delta, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeApproximately", actualSubjectIndex, argumentsToRemove: []);
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity): // AreEqual<T>(T? expected, T? actual)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity): // AreEqual<T>(T? expected, T? actual, string? | InterpolatedStringHandler message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity, t.String, t.String): // AreEqual<T>(T? expected, T? actual, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.String, t.ObjectArray): // AreEqual<T>(T? expected, T? actual, string? message, params object?[]? parameters)
                if (invocation.Arguments[0].IsLiteralNull())
                {
                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeNull", subjectIndex: 1, argumentsToRemove: [0]);
                }
                else if (invocation.Arguments[1].IsLiteralNull())
                {
                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeNull", subjectIndex: 0, argumentsToRemove: [1]);
                }
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "Be", actualSubjectIndex, argumentsToRemove: []);
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo): // AreNotEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.Identity): // AreNotEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.Identity, t.String, t.String): // AreNotEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.CultureInfo, t.String, t.ObjectArray): // AreNotEqual(string? expected, string? actual, bool ignoreCase, CultureInfo? culture, string? message, params object?[]? parameters)
                {
                    var ignoreCase = invocation.Arguments.Length >= 3 && invocation.Arguments[2].IsLiteralValue(true);
                    var newMethod = ignoreCase ? "NotBeEquivalentTo" : "NotBe";

                    if (!invocation.Arguments[3].IsStaticPropertyReference(t.CultureInfo, "CurrentCulture"))
                    {
                        break; // TODO: Handle other cultures
                    }

                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, newMethod, actualSubjectIndex, argumentsToRemove: [2, 3]);
                }
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean): // AreNotEqual(string? expected, string? actual, bool ignoreCase)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.Identity): // AreNotEqual(string? expected, string? actual, bool ignoreCase, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.Identity, t.String, t.String): // AreNotEqual(string? expected, string? actual, bool ignoreCase, string? message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.Boolean, t.String, t.ObjectArray): // AreNotEqual(string? expected, string? actual, bool ignoreCase, string? message, params object?[]? parameters)
                {
                    var ignoreCase = invocation.Arguments.Length >= 3 && invocation.Arguments[2].IsLiteralValue(true);
                    var newMethod = ignoreCase ? "NotBeEquivalentTo" : "NotBe";
                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, newMethod, actualSubjectIndex, argumentsToRemove: [2]);
                }
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float): // AreNotEqual(float expected, float actual, float delta)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.Identity): // AreNotEqual(float expected, float actual, float delta, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.Identity, t.String, t.String): // AreNotEqual(float expected, float actual, float delta, string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Float, t.Float, t.Float, t.String, t.ObjectArray): // AreNotEqual(float expected, float actual, float delta, string? message, params object?[]? parameters)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double): // AreNotEqual(double expected, double actual, double delta)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.Identity): // AreNotEqual(double expected, double actual, double delta, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.Identity, t.String, t.String): // AreNotEqual(double expected, double actual, double delta, string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Double, t.Double, t.Double, t.String, t.ObjectArray): // AreNotEqual(double expected, double actual, double delta, string? message, params object?[]? parameters)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal): // AreNotEqual(decimal expected, decimal actual, decimal delta)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.Identity): // AreNotEqual(decimal expected, decimal actual, decimal delta, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.Identity, t.String, t.String): // AreNotEqual(decimal expected, decimal actual, decimal delta, string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Decimal, t.Decimal, t.Decimal, t.String, t.ObjectArray): // AreNotEqual(decimal expected, decimal actual, decimal delta, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeApproximately", actualSubjectIndex, argumentsToRemove: []);
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity): // AreNotEqual<T>(T? expected, T? actual)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity): // AreNotEqual<T>(T? expected, T? actual, string? | InterpolatedStringHandler message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.String, t.ObjectArray): // AreNotEqual<T>(T? expected, T? actual, string? message, params object?[]? parameters)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity, t.String, t.String): // AreNotEqual<T>(T? expected, T? actual, string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
                if (invocation.Arguments[0].IsLiteralNull())
                {
                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeNull", subjectIndex: 1, argumentsToRemove: [0]);
                }
                else if (invocation.Arguments[1].IsLiteralNull())
                {
                    return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeNull", subjectIndex: 0, argumentsToRemove: [1]);
                }
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBe", actualSubjectIndex, argumentsToRemove: []);
            case "AreSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity): // AreSame(T? expected, T? actual)
            case "AreSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity): // AreSame(T? expected, T? actual, string? | InterpolatedStringHandler message)
            case "AreSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity, t.String, t.String): // AreNotSame(T? expected, T? actual, string? | InterpolatedStringHandler message, string expectedExpression, string actualExpression)
            case "AreSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.String, t.ObjectArray): // AreSame(T? expected, T? actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeSameAs", actualSubjectIndex, argumentsToRemove: []);
            case "AreNotSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity): // AreNotSame(T? expected, T? actual)
            case "AreNotSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity): // AreNotSame(T? expected, T? actual, string? | InterpolatedStringHandler message)
            case "AreNotSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.Identity, t.String, t.String): // AreNotSame(T? expected, T? actual, string? | InterpolatedStringHandler message, string notExpectedExpression, string actualExpression)
            case "AreNotSame" when ArgumentsAreTypeOf(invocation, t.Identity, t.Identity, t.String, t.ObjectArray): // AreNotSame(T? expected, T? actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeSameAs", actualSubjectIndex, argumentsToRemove: []);
            case "IsTrue" when ArgumentsAreTypeOf(invocation, t.Boolean): // IsTrue(bool condition)
            case "IsTrue" when ArgumentsAreTypeOf(invocation, t.Boolean, t.Identity): // IsTrue(bool condition, string? message)
            case "IsTrue" when ArgumentsAreTypeOf(invocation, t.NullableBoolean, t.Identity, t.String): // IsTrue(bool? condition, string? | InterpolatedStringHandler message, string? conditionExpression)
            case "IsTrue" when ArgumentsAreTypeOf(invocation, t.Boolean, t.String, t.ObjectArray): // IsTrue(bool condition, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeTrue", subjectIndex: 0, argumentsToRemove: []);
            case "IsFalse" when ArgumentsAreTypeOf(invocation, t.Boolean): // IsFalse(bool condition)
            case "IsFalse" when ArgumentsAreTypeOf(invocation, t.Boolean, t.Identity): // IsFalse(bool condition, string? | InterpolatedStringHandler message)
            case "IsFalse" when ArgumentsAreTypeOf(invocation, t.NullableBoolean, t.Identity, t.String): // IsFalse(bool condition, string? | InterpolatedStringHandler message, string? conditionExpression)
            case "IsFalse" when ArgumentsAreTypeOf(invocation, t.Boolean, t.String, t.ObjectArray): // IsFalse(bool condition, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeFalse", subjectIndex: 0, argumentsToRemove: []);
            case "IsNull" when ArgumentsAreTypeOf(invocation, t.Object): // IsNull(object? value)
            case "IsNull" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity): // IsNull(object? value, string? | InterpolatedStringHandler message)
            case "IsNull" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity, t.String): // IsNull(object? value, string? | InterpolatedStringHandler message, string? valueExpression)
            case "IsNull" when ArgumentsAreTypeOf(invocation, t.Object, t.String, t.ObjectArray): // IsNull(object? value, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeNull", subjectIndex: 0, argumentsToRemove: []);
            case "IsNotNull" when ArgumentsAreTypeOf(invocation, t.Object): // IsNotNull(object? value)
            case "IsNotNull" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity): // IsNotNull(object? value, string? | InterpolatedStringHandler message)
            case "IsNotNull" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity, t.String): // IsNotNull(object? value, string? | InterpolatedStringHandler message, string valueExpression)
            case "IsNotNull" when ArgumentsAreTypeOf(invocation, t.Object, t.String, t.ObjectArray): // IsNotNull(object? value, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeNull", subjectIndex: 0, argumentsToRemove: []);
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.Action): // ThrowsException<T>(Action action)
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.Action, t.String): // ThrowsException<T>(Action action, string? message)
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.Action, t.String, t.ObjectArray): // ThrowsException<T>(Action action, string? message, params object?[]? parameters)
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.FuncOfObject): // ThrowsException<T>(Func<object?> action)
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.String): // ThrowsException<T>(Func<object?> action, string? message)
            case "ThrowsException" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.String, t.ObjectArray): // ThrowsException<T>(Func<object?> action, string? message, params object?[]? parameters)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.Action): // ThrowsExactly<T>(Action action)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.Action, t.Identity): // ThrowsExactly<T>(Action action, string? | InterpolatedStringHandler message)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.Action, t.Identity, t.String): // ThrowsExactly<T>(Action action, string? | InterpolatedStringHandler message, string actionExpression)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.Action, t.String, t.ObjectArray): // ThrowsExactly<T>(Action action, string? message, params object?[]? parameters)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.FuncOfObject): // ThrowsExactly<T>(Func<object?> action)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.Identity): // ThrowsExactly<T>(Func<object?> action, string? | InterpolatedStringHandler message)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.Identity, t.String): // ThrowsExactly<T>(Func<object?> action, string? | InterpolatedStringHandler message, string actionExpression)
            case "ThrowsExactly" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.String, t.ObjectArray): // ThrowsExactly<T>(Func<object?> action, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "ThrowExactly", subjectIndex: 0, argumentsToRemove: []);
            case "Throws" when ArgumentsAreTypeOf(invocation, t.Action): // Throws<T>(Action action)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.Action, t.Identity): // Throws<T>(Action action, string? | InterpolatedStringHandler message)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.Action, t.Identity, t.String): // Throws<T>(Action action, string? | InterpolatedStringHandler message, string actionExpression)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.Action, t.String, t.ObjectArray): // Throws<T>(Action action, string? message, params object?[]? parameters)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.FuncOfObject): // Throws<T>(Func<object?> action)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.Identity): // Throws<T>(Func<object?> action, string? | InterpolatedStringHandler message)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.Identity, t.String): // Throws<T>(Func<object?> action, string? | InterpolatedStringHandler message, string actionExpression)
            case "Throws" when ArgumentsAreTypeOf(invocation, t.FuncOfObject, t.String, t.ObjectArray): // Throws<T>(Func<object?> action, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "Throw", subjectIndex: 0, argumentsToRemove: []);
            case "ThrowsExceptionAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask): // ThrowsExceptionAsync<T>(Func<Task> action)
            case "ThrowsExceptionAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.String): // ThrowsExceptionAsync<T>(Func<Task> action, string? message)
            case "ThrowsExceptionAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.String, t.ObjectArray): // ThrowsExceptionAsync<T>(Func<Task> action, string? message, params object?[]? parameters)
            case "ThrowsExactlyAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask): // ThrowsExactlyAsync<T>(Func<Task> action)
            case "ThrowsExactlyAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.Identity): // ThrowsExactlyAsync<T>(Func<Task> action, string? message)
            case "ThrowsExactlyAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.Identity, t.String): // ThrowsExactlyAsync<T>(Func<Task> action, string? | InterpolatedStringHandler message, string actionExpression)
            case "ThrowsExactlyAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.String, t.ObjectArray): // ThrowsExactlyAsync<T>(Func<Task> action, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "ThrowExactlyAsync", subjectIndex: 0, argumentsToRemove: []);
            case "ThrowsAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask): // ThrowsAsync<T>(Func<Task> action)
            case "ThrowsAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.Identity): // ThrowsAsync<T>(Func<Task> action, string? | InterpolatedStringHandler message)
            case "ThrowsAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.Identity, t.String): // ThrowsAsync<T>(Func<Task> action, string? | InterpolatedStringHandler message, string actionExpression)
            case "ThrowsAsync" when ArgumentsAreTypeOf(invocation, t.FuncOfTask, t.String, t.ObjectArray): // ThrowsAsync<T>(Func<Task> action, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "ThrowAsync", subjectIndex: 0, argumentsToRemove: []);
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type): // IsInstanceOfType(object? value, Type expectedType)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.Identity): // IsInstanceOfType(object? value, Type expectedType, string? | InterpolatedStringHandler message)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.Identity, t.String): // IsInstanceOfType(object? value, Type expectedType, string? | InterpolatedStringHandler message, string valueExpression)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.String, t.ObjectArray): // IsInstanceOfType(object? value, Type expectedType, string? message, params object?[]? parameters)
                {
                    if (invocation.Arguments[1].Value is not ITypeOfOperation typeOf)
                    {
                        return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeOfType", subjectIndex: 0, argumentsToRemove: []);
                    }

                    return DocumentEditorUtils.RenameMethodToSubjectShouldGenericAssertion(invocation, ImmutableArray.Create(typeOf.TypeOperand), context, "BeOfType", subjectIndex: 0, argumentsToRemove: [1]);
                }
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object): // IsInstanceOfType<T>(object? value)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity): // IsInstanceOfType<T>(object? value, string? | InterpolatedStringHandler message)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity, t.String): // IsInstanceOfType<T>(object? value, string? | InterpolatedStringHandler message, string valueExpression)
            case "IsInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.String, t.ObjectArray): // IsInstanceOfType<T>(object? value, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "BeOfType", subjectIndex: 0, argumentsToRemove: []);
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type): // IsNotInstanceOfType(object? value, Type expectedType)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.Identity): // IsNotInstanceOfType(object? value, Type expectedType, string? | InterpolatedStringHandler message)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.Identity, t.String): // IsNotInstanceOfType(object? value, Type expectedType, string? | InterpolatedStringHandler message, string valueExpression)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Type, t.String, t.ObjectArray): // IsNotInstanceOfType(object? value, Type expectedType, string? message, params object?[]? parameters)
                {
                    if (invocation.Arguments[1].Value is not ITypeOfOperation typeOf)
                    {
                        return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeOfType", subjectIndex: 0, argumentsToRemove: []);
                    }

                    return DocumentEditorUtils.RenameMethodToSubjectShouldGenericAssertion(invocation, ImmutableArray.Create(typeOf.TypeOperand), context, "NotBeOfType", subjectIndex: 0, argumentsToRemove: [1]);
                }
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object): // IsNotInstanceOfType<T>(object? value)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity): // IsNotInstanceOfType<T>(object? value, string? | InterpolatedStringHandler message)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.Identity, t.String): // IsNotInstanceOfType<T>(object? value, string? | InterpolatedStringHandler message, string valueExpression)
            case "IsNotInstanceOfType" when ArgumentsAreTypeOf(invocation, t.Object, t.String, t.ObjectArray): // IsNotInstanceOfType<T>(object? value, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameGenericMethodToSubjectShouldGenericAssertion(invocation, context, "NotBeOfType", subjectIndex: 0, argumentsToRemove: []);
        }

        return null;
    }

    private CreateChangedDocument TryComputeFixForStringAssert(IInvocationOperation invocation, CodeFixContext context, TestingFrameworkCodeFixContext t)
    {
        switch (invocation.TargetMethod.Name)
        {
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String): // Contains(string? value, string? substring)
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String): // Contains(string? value, string? substring, string? message)
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.ObjectArray): // Contains(string? value, string? substring, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "Contain", subjectIndex: 0, argumentsToRemove: []);
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.StringComparison): // Contains(string? value, string? substring, StringComparison comparisonType)
                break; // TODO: support StringComparison constant values
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison): // Contains(string? value, string? substring, string? message, StringComparison comparisonType)
            case "Contains" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison, t.ObjectArray): // Contains(string? value, string? substring, string? message, StringComparison comparisonType, params object?[]? parameters)
                break; // TODO: support StringComparison constant values
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String): // StartsWith(string? value, string? substring)
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String): // StartsWith(string? value, string? substring, string? message)
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.ObjectArray): // StartsWith(string? value, string? substring, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "StartWith", subjectIndex: 0, argumentsToRemove: []);
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.StringComparison): // StartsWith(string? value, string? substring, StringComparison comparisonType)
                break; // TODO: support StringComparison constant values
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison): // StartsWith(string? value, string? substring, string? message, StringComparison comparisonType)
            case "StartsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison, t.ObjectArray): // StartsWith(string? value, string? substring, string? message, StringComparison comparisonType, params object?[]? parameters)
                break; // TODO: support StringComparison constant values
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String): // EndsWith(string? value, string? substring)
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String): // EndsWith(string? value, string? substring, string? message)
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.ObjectArray): // EndsWith(string? value, string? substring, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "EndWith", subjectIndex: 0, argumentsToRemove: []);
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.StringComparison): // EndsWith(string? value, string? substring, StringComparison comparisonType)
                break; // TODO: support StringComparison constant values
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison): // EndsWith(string? value, string? substring, string? message, StringComparison comparisonType)
            case "EndsWith" when ArgumentsAreTypeOf(invocation, t.String, t.String, t.String, t.StringComparison, t.ObjectArray): // EndsWith(string? value, string? substring, string? message, StringComparison comparisonType, params object?[]? parameters)
                break; // TODO: support StringComparison constant values
            case "Matches" when ArgumentsAreTypeOf(invocation, t.String, t.Regex): // Matches(string? value, Regex? pattern)
            case "Matches" when ArgumentsAreTypeOf(invocation, t.String, t.Regex, t.String): // Matches(string? value, Regex? pattern, string? message)
            case "Matches" when ArgumentsAreTypeOf(invocation, t.String, t.Regex, t.String, t.ObjectArray): // Matches(string? value, Regex? pattern, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "MatchRegex", subjectIndex: 0, argumentsToRemove: []);
            case "DoesNotMatch" when ArgumentsAreTypeOf(invocation, t.String, t.Regex): // DoesNotMatch(string? value, Regex? pattern)
            case "DoesNotMatch" when ArgumentsAreTypeOf(invocation, t.String, t.Regex, t.String): // DoesNotMatch(string? value, Regex? pattern, string? message)
            case "DoesNotMatch" when ArgumentsAreTypeOf(invocation, t.String, t.Regex, t.String, t.ObjectArray): // DoesNotMatch(string? value, Regex? pattern, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotMatchRegex", subjectIndex: 0, argumentsToRemove: []);
        }
        return null;
    }

    private static CreateChangedDocument TryComputeFixForCollectionAssert(IInvocationOperation invocation, CodeFixContext context, TestingFrameworkCodeFixContext t)
    {
        if (!invocation.Arguments[0].ImplementsOrIsInterface(SpecialType.System_Collections_Generic_IEnumerable_T))
        {
            return null;
        }

        switch (invocation.TargetMethod.Name)
        {
            case "Contains" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object): // Contains(ICollection collection, object? element)
            case "Contains" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object, t.String): // Contains(ICollection collection, object? element, string? message)
            case "Contains" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object, t.String, t.ObjectArray): // Contains(ICollection collection, object? element, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "Contain", subjectIndex: 0, argumentsToRemove: []);
            case "DoesNotContain" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object): // DoesNotContain(ICollection collection, object? element)
            case "DoesNotContain" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object, t.String): // DoesNotContain(ICollection collection, object? element, string? message)
            case "DoesNotContain" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Object, t.String, t.ObjectArray): // DoesNotContain(ICollection collection, object? element, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotContain", subjectIndex: 0, argumentsToRemove: []);
            case "AllItemsAreNotNull" when ArgumentsAreTypeOf(invocation, t.ICollection): // AllItemsAreInstancesOfType(ICollection collection)
            case "AllItemsAreNotNull" when ArgumentsAreTypeOf(invocation, t.ICollection, t.String): // AllItemsAreInstancesOfType(ICollection collection, string? message)
            case "AllItemsAreNotNull" when ArgumentsAreTypeOf(invocation, t.ICollection, t.String, t.ObjectArray): // AllItemsAreInstancesOfType(ICollection collection, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotContainNulls", subjectIndex: 0, argumentsToRemove: []);
            case "AllItemsAreUnique" when ArgumentsAreTypeOf(invocation, t.ICollection): // AllItemsAreUnique(ICollection collection)
            case "AllItemsAreUnique" when ArgumentsAreTypeOf(invocation, t.ICollection, t.String): // AllItemsAreUnique(ICollection collection, string? message)
            case "AllItemsAreUnique" when ArgumentsAreTypeOf(invocation, t.ICollection, t.String, t.ObjectArray): // AllItemsAreUnique(ICollection collection, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "OnlyHaveUniqueItems", subjectIndex: 0, argumentsToRemove: []);
            case "IsSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // IsSubsetOf(ICollection subset, ICollection superset)
            case "IsSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // IsSubsetOf(ICollection subset, ICollection superset, string? message)
            case "IsSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // IsSubsetOf(ICollection subset, ICollection superset, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeSubsetOf", subjectIndex: 0, argumentsToRemove: []);
            case "IsNotSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // IsNotSubsetOf(ICollection subset, ICollection superset)
            case "IsNotSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // IsNotSubsetOf(ICollection subset, ICollection superset, string? message)
            case "IsNotSubsetOf" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // IsNotSubsetOf(ICollection subset, ICollection superset, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeSubsetOf", subjectIndex: 0, argumentsToRemove: []);
            case "AreEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // AreEquivalent(ICollection expected, ICollection actual)
            case "AreEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // AreEquivalent(ICollection expected, ICollection actual, string? message)
            case "AreEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // AreEquivalent(ICollection expected, ICollection actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "BeEquivalentTo", subjectIndex: 1, argumentsToRemove: []);
            case "AreNotEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // AreNotEquivalent(ICollection expected, ICollection actual)
            case "AreNotEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // AreNotEquivalent(ICollection expected, ICollection actual, string? message)
            case "AreNotEquivalent" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // AreNotEquivalent(ICollection expected, ICollection actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotBeEquivalentTo", subjectIndex: 1, argumentsToRemove: []);
            case "AllItemsAreInstancesOfType" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Type): // AllItemsAreInstancesOfType(ICollection collection, Type expectedType)
            case "AllItemsAreInstancesOfType" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Type, t.String): // AllItemsAreInstancesOfType(ICollection collection, Type expectedType, string? message)
            case "AllItemsAreInstancesOfType" when ArgumentsAreTypeOf(invocation, t.ICollection, t.Type, t.String, t.ObjectArray): // AllItemsAreInstancesOfType(ICollection collection, Type expectedType, string? message, params object?[]? parameters)
                {
                    if (invocation.Arguments[1].Value is not ITypeOfOperation typeOf)
                    {
                        return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "AllBeOfType", subjectIndex: 0, argumentsToRemove: []);
                    }

                    return DocumentEditorUtils.RenameMethodToSubjectShouldGenericAssertion(invocation, ImmutableArray.Create(typeOf.TypeOperand), context, "AllBeOfType", subjectIndex: 0, argumentsToRemove: [1]);
                }
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // AreEqual(ICollection expected, ICollection actual)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // AreEqual(ICollection expected, ICollection actual, string? message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // AreEqual(ICollection expected, ICollection actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "Equal", subjectIndex: 1, argumentsToRemove: []);
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer): // AreEqual(ICollection expected, ICollection actual, IComparer? comparer)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer, t.String): // AreEqual(ICollection expected, ICollection actual, IComparer? comparer, string? message)
            case "AreEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer, t.String, t.ObjectArray): // AreEqual(ICollection expected, ICollection actual, IComparer? comparer, string? message, params object?[]? parameters)
                break; // TODO: support IComparer
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection): // AreNotEqual(ICollection notExpected, ICollection actual)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String): // AreNotEqual(ICollection notExpected, ICollection actual, string? message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.String, t.ObjectArray): // AreNotEqual(ICollection notExpected, ICollection actual, string? message, params object?[]? parameters)
                return DocumentEditorUtils.RenameMethodToSubjectShouldAssertion(invocation, context, "NotEqual", subjectIndex: 1, argumentsToRemove: []);
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer): // AreNotEqual(ICollection notExpected, ICollection actual, IComparer? comparer)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer, t.String): // AreNotEqual(ICollection notExpected, ICollection actual, IComparer? comparer, string? message)
            case "AreNotEqual" when ArgumentsAreTypeOf(invocation, t.ICollection, t.ICollection, t.IComparer, t.String, t.ObjectArray): // AreNotEqual(ICollection notExpected, ICollection actual, IComparer? comparer, string? message, params object?[]? parameters)
                break; // TODO: support IComparer
        }
        return null;
    }
}
