using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using CreateChangedDocument = System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document>>;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AwesomeAssertions.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AwesomeAssertionsCodeFixProvider)), Shared]
public sealed partial class AwesomeAssertionsCodeFixProvider : CodeFixProviderBase<AwesomeAssertionsCodeFixProvider.EmptyTestContext>
{
    protected override string Title => AwesomeAssertionsAnalyzer.Title;
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(AwesomeAssertionsAnalyzer.DiagnosticId);

    protected override CreateChangedDocument TryComputeFix(IInvocationOperation assertion, CodeFixContext context, EmptyTestContext t, Diagnostic diagnostic)
    {
        if (!diagnostic.Properties.TryGetValue(Constants.DiagnosticProperties.VisitorName, out var visitorName))
        {
            return null;
        }

        if (visitorName is nameof(DiagnosticMetadata.ShouldEquals) or nameof(DiagnosticMetadata.NullConditionalMayNotExecute))
        {
            return null;
        }

        // oldAssertion: subject.<property>.Should().<assertion>([arg1, arg2, arg3...]);
        // oldAssertion: subject.<method>().Should().<assertion>([arg1, arg2, arg3...]);
        // newAssertion: subject.Should().<newName>([arg2, arg3...]);
        CreateChangedDocument RemoveExpressionBeforeShouldAndRenameAssertion(string newName)
        {
            return RewriteAwesomeAssertion(assertion, context, [
                AwesomeAssertionsEditAction.RenameAssertion(newName),
                AwesomeAssertionsEditAction.SkipExpressionBeforeShould()
            ]);
        }

        // oldAssertion: subject.Should().<assertion>([arg1, arg2, arg3...]);
        // newAssertion: subject.Should().<newName>([arg2, arg3...]);
        CreateChangedDocument RenameAssertionAndRemoveFirstAssertionArgument(string newName)
        {
            return RewriteAwesomeAssertion(assertion, context, [
                AwesomeAssertionsEditAction.RemoveAssertionArgument(index: 0),
                AwesomeAssertionsEditAction.RenameAssertion(newName)
            ]);
        }

        // oldAssertion: subject.<method>(<arguments>).Should().<assertion>([arg1, arg2, arg3...]);
        // newAssertion: subject.Should().<newName>([arguments, arg1, arg2, arg3...]);
        CreateChangedDocument RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved(string newName)
        {
            return RewriteAwesomeAssertion(assertion, context, [
                AwesomeAssertionsEditAction.RenameAssertion(newName),
                AwesomeAssertionsEditAction.SkipInvocationBeforeShould(),
                AwesomeAssertionsEditAction.PrependArgumentsFromInvocationBeforeShouldToAssertion()
            ]);
        }

        // oldAssertion: subject.<method>(<argument1>).Should().<assertion>([arg1, arg2, arg3...]);
        // newAssertion: subject.Should().<newName>([argument1, arg1, arg2, arg3...]);
        CreateChangedDocument RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved(string newName)
        {
            return RewriteAwesomeAssertion(assertion, context, [
                AwesomeAssertionsEditAction.RenameAssertion(newName),
                AwesomeAssertionsEditAction.SkipInvocationBeforeShould(),
                AwesomeAssertionsEditAction.RemoveAssertionArgument(index: 0),
                AwesomeAssertionsEditAction.PrependArgumentsFromInvocationBeforeShouldToAssertion(skipAssertionArguments: 1)
            ]);
        }

        // oldAssertion: subject.<method>(<argument1>).Should().<assertion>([arg1, arg2, arg3...]);
        // oldAssertion: subject.<property>.Should().<assertion>([arg1, arg2, arg3...]);
        // newAssertion: subject.Should().<newName>([argument1, arg1, arg2, arg3...]);
        CreateChangedDocument RemoveExpressionBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved(string newName)
        {
            return RewriteAwesomeAssertion(assertion, context, [
                AwesomeAssertionsEditAction.RenameAssertion(newName),
                AwesomeAssertionsEditAction.SkipExpressionBeforeShould(),
                AwesomeAssertionsEditAction.RemoveAssertionArgument(index: 0)
            ]);
        }

        switch (visitorName)
        {
            case nameof(DiagnosticMetadata.CollectionShouldBeEmpty_AnyShouldBeFalse):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("BeEmpty"),
                    AwesomeAssertionsEditAction.SkipInvocationBeforeShould()
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldNotBeEmpty_AnyShouldBeTrue):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("NotBeEmpty"),
                    AwesomeAssertionsEditAction.SkipInvocationBeforeShould()
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldBeEmpty_ShouldHaveCount0):
                return RenameAssertionAndRemoveFirstAssertionArgument("BeEmpty");
            case nameof(DiagnosticMetadata.CollectionShouldBeInAscendingOrder_OrderByShouldEqual):
                return RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("BeInAscendingOrder");
            case nameof(DiagnosticMetadata.CollectionShouldBeInDescendingOrder_OrderByDescendingShouldEqual):
                return RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("BeInDescendingOrder");
            case nameof(DiagnosticMetadata.CollectionShouldContainItem_ContainsShouldBeTrue):
            case nameof(DiagnosticMetadata.CollectionShouldContainProperty_AnyWithLambdaShouldBeTrue):
            case nameof(DiagnosticMetadata.CollectionShouldContainProperty_WhereShouldNotBeEmpty):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("Contain");
            case nameof(DiagnosticMetadata.CollectionShouldContainSingle_ShouldHaveCount1):
                return RenameAssertionAndRemoveFirstAssertionArgument("ContainSingle");
            case nameof(DiagnosticMetadata.CollectionShouldContainSingle_WhereShouldHaveCount1):
                return RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("ContainSingle");
            case nameof(DiagnosticMetadata.CollectionShouldEqualOtherCollectionByComparer_SelectShouldEqualOtherCollectionSelect):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.SkipInvocationBeforeShould(),
                    (editor, context) => {
                        var firstLambda = (SimpleLambdaExpressionSyntax)context.InvocationBeforeShould.Arguments[1].GetFirstDescendent<IAnonymousFunctionOperation>().Syntax;
                        var selectInvocation = context.Assertion.Arguments[0].GetFirstDescendent<IInvocationOperation>();
                        var secondLambda = (SimpleLambdaExpressionSyntax)selectInvocation.Arguments[1].Value.Syntax;

                        var newLambda = editor.Generator.ValueReturningLambdaExpression([firstLambda.Parameter, secondLambda.Parameter], SF.BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            left: (ExpressionSyntax)firstLambda.Body,
                            right: (ExpressionSyntax)secondLambda.Body
                        ));

                        var arguments = SF.ArgumentList()
                            .AddArguments((ArgumentSyntax)editor.Generator.Argument(selectInvocation.Arguments[0].Syntax))
                            .AddArguments((ArgumentSyntax)editor.Generator.Argument(newLambda))
                            .AddArguments([..context.AssertionExpression.ArgumentList.Arguments.Skip(1)]);
                        editor.ReplaceNode(context.AssertionExpression.ArgumentList, arguments);
                    }
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldBeEmpty_CountPropertyShouldBe0):
                return RemoveExpressionBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("BeEmpty");
            case nameof(DiagnosticMetadata.CollectionShouldBeEmpty_CountShouldBe0):
                return RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("BeEmpty");
            case nameof(DiagnosticMetadata.CollectionShouldContainSingle_CountShouldBe1):
                return RemoveMethodBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("ContainSingle");
            case nameof(DiagnosticMetadata.CollectionShouldContainSingle_CountPropertyShouldBe1):
                return RemoveExpressionBeforeShouldAndRenameAssertionWithoutFirstArgumentWithArgumentsFromRemoved("ContainSingle");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCount_CountShouldBe):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCount");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCount_LengthShouldBe):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCount");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCountGreaterThanOrEqualTo_CountShouldBeGreaterThanOrEqualTo):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCountGreaterThanOrEqualTo");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCountGreaterThan_CountShouldBeGreaterThan):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCountGreaterThan");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCountLessThanOrEqualTo_CountShouldBeLessThanOrEqualTo):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCountLessThanOrEqualTo");
            case nameof(DiagnosticMetadata.CollectionShouldHaveCountLessThan_CountShouldBeLessThan):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveCountLessThan");
            case nameof(DiagnosticMetadata.CollectionShouldIntersectWith_IntersectShouldNotBeEmpty):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("IntersectWith");
            case nameof(DiagnosticMetadata.CollectionShouldHaveSameCount_ShouldHaveCountOtherCollectionCount):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("HaveSameCount"),
                    AwesomeAssertionsEditAction.RemoveInvocationOnAssertionArgument(assertionArgumentIndex: 0, invocationArgumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldNotContainItem_ContainsShouldBeFalse):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotContain");
            case nameof(DiagnosticMetadata.CollectionShouldNotContainNulls_SelectShouldNotContainNulls):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotContainNulls");
            case nameof(DiagnosticMetadata.CollectionShouldNotContainProperty_AnyLambdaShouldBeFalse):
            case nameof(DiagnosticMetadata.CollectionShouldNotContainProperty_WhereShouldBeEmpty):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotContain");
            case nameof(DiagnosticMetadata.CollectionShouldNotContainProperty_ShouldOnlyContainNot):
                break; // TODO: support negation of lambda, rename to "NotContain"
            case nameof(DiagnosticMetadata.CollectionShouldNotHaveCount_CountShouldNotBe):
                return RemoveExpressionBeforeShouldAndRenameAssertion("NotHaveCount");
            case nameof(DiagnosticMetadata.CollectionShouldNotHaveSameCount_CountShouldNotBeOtherCollectionCount):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("NotHaveSameCount"),
                    AwesomeAssertionsEditAction.SkipInvocationBeforeShould(),
                    AwesomeAssertionsEditAction.RemoveInvocationOnAssertionArgument(assertionArgumentIndex: 0, invocationArgumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldNotIntersectWith_IntersectShouldBeEmpty):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotIntersectWith");
            case nameof(DiagnosticMetadata.CollectionShouldOnlyContainProperty_AllShouldBeTrue):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("OnlyContain");
            case nameof(DiagnosticMetadata.CollectionShouldOnlyHaveUniqueItems_ShouldHaveSameCountThisCollectionDistinct):
                return RenameAssertionAndRemoveFirstAssertionArgument("OnlyHaveUniqueItems");
            case nameof(DiagnosticMetadata.CollectionShouldOnlyHaveUniqueItemsByComparer_SelectShouldOnlyHaveUniqueItems):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("OnlyHaveUniqueItems");
            case nameof(DiagnosticMetadata.CollectionShouldNotBeNullOrEmpty_ShouldNotBeNullAndNotBeEmpty):
            case nameof(DiagnosticMetadata.CollectionShouldNotBeNullOrEmpty_ShouldNotBeEmptyAndNotBeNull):
            case nameof(DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringShouldNotBeNullAndNotBeEmpty):
            case nameof(DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringShouldNotBeEmptyAndNotBeNull):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithNameAndArguments("NotBeNullOrEmpty", strategy: CombineAssertionArgumentsStrategy.FirstAssertionFirst),
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldHaveElementAt_ElementAtIndexShouldBe):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("HaveElementAt");
            case nameof(DiagnosticMetadata.CollectionShouldHaveElementAt_IndexerShouldBe):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("HaveElementAt"),
                    (editor, context) => {
                        var (subject, index) = context.Subject switch {
                            IPropertyReferenceOperation indexer => (indexer.Instance, indexer.Arguments[0].Value),
                            IArrayElementReferenceOperation arrayElementReference => (arrayElementReference.ArrayReference, arrayElementReference.Indices[0]),
                            _ => throw new NotSupportedException("Invalid subject for CollectionShouldHaveElementAt_IndexerShouldBe")
                        };

                        editor.ReplaceNode(context.Subject.Syntax, subject.Syntax.WithTriviaFrom(context.Subject.Syntax));

                        var arguments = SF.ArgumentList()
                            .AddArguments((ArgumentSyntax)editor.Generator.Argument(index.Syntax))
                            .AddArguments([..context.AssertionExpression.ArgumentList.Arguments]);

                        editor.ReplaceNode(context.AssertionExpression.ArgumentList, arguments);
                    }
                ]);
            case nameof(DiagnosticMetadata.CollectionShouldHaveElementAt_SkipFirstShouldBe):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("HaveElementAt"),
                    (editor, context) => {
                        var firstInvocation = context.InvocationBeforeShould;
                        var skipInvocation = firstInvocation.GetFirstDescendent<IInvocationOperation>();

                        var skipValue = skipInvocation.Arguments[1].Value;
                        var subject = skipInvocation.ChildOperations.First().UnwrapConversion();

                        editor.ReplaceNode(firstInvocation.Syntax, subject.Syntax.WithTriviaFrom(firstInvocation.Syntax));

                        var arguments = SF.ArgumentList()
                            .AddArguments((ArgumentSyntax)editor.Generator.Argument(skipValue.Syntax))
                            .AddArguments([..context.AssertionExpression.ArgumentList.Arguments]);
                        editor.ReplaceNode(context.AssertionExpression.ArgumentList, arguments);
                    }
                ]);
            case nameof(DiagnosticMetadata.NumericShouldBePositive_ShouldBeGreaterThan):
                return RenameAssertionAndRemoveFirstAssertionArgument("BePositive");
            case nameof(DiagnosticMetadata.NumericShouldBeNegative_ShouldBeLessThan):
                return RenameAssertionAndRemoveFirstAssertionArgument("BeNegative");
            case nameof(DiagnosticMetadata.NumericShouldBeInRange_BeGreaterThanOrEqualToAndBeLessThanOrEqualTo):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithNameAndArguments("BeInRange", strategy: CombineAssertionArgumentsStrategy.InsertFirstAssertionIntoIndex1OfSecondAssertion),
                ]);
            case nameof(DiagnosticMetadata.NumericShouldBeInRange_BeLessThanOrEqualToAndBeGreaterThanOrEqualTo):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithNameAndArguments("BeInRange", strategy: CombineAssertionArgumentsStrategy.InsertSecondAssertionIntoIndex1OfFirstAssertion)
                ]);
            case nameof(DiagnosticMetadata.NumericShouldBeApproximately_MathAbsShouldBeLessThanOrEqualTo):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("BeApproximately"),
                    (editor, context) => {
                        var abs = (IInvocationOperation)context.Subject;
                        var subtract = (IBinaryOperation)abs.Arguments[0].Value;

                        var subject = subtract.RightOperand;
                        var expected = subtract.LeftOperand;

                        editor.ReplaceNode(context.Subject.Syntax, subject.Syntax.WithTriviaFrom(context.Subject.Syntax));

                        var arguments = SF.ArgumentList()
                            .AddArguments((ArgumentSyntax)editor.Generator.Argument(expected.Syntax))
                            .AddArguments([..context.AssertionExpression.ArgumentList.Arguments]);

                        editor.ReplaceNode(context.AssertionExpression.ArgumentList, arguments);
                    }
                ]);
            case nameof(DiagnosticMetadata.StringShouldBeNullOrEmpty_StringIsNullOrEmptyShouldBeTrue):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("BeNullOrEmpty"),
                    AwesomeAssertionsEditAction.UnwrapInvocationOnSubject(argumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.StringShouldBeNullOrWhiteSpace_StringIsNullOrWhiteSpaceShouldBeTrue):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("BeNullOrWhiteSpace"),
                    AwesomeAssertionsEditAction.UnwrapInvocationOnSubject(argumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.StringShouldEndWith_EndsWithShouldBeTrue):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("EndWith");
            case nameof(DiagnosticMetadata.StringShouldStartWith_StartsWithShouldBeTrue):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("StartWith");
            case nameof(DiagnosticMetadata.StringShouldNotBeNullOrWhiteSpace_StringShouldNotBeNullOrWhiteSpace):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("NotBeNullOrWhiteSpace"),
                    AwesomeAssertionsEditAction.UnwrapInvocationOnSubject(argumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.StringShouldNotBeNullOrEmpty_StringIsNullOrEmptyShouldBeFalse):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("NotBeNullOrEmpty"),
                    AwesomeAssertionsEditAction.UnwrapInvocationOnSubject(argumentIndex: 0)
                ]);
            case nameof(DiagnosticMetadata.StringShouldHaveLength_LengthShouldBe):
                return RemoveExpressionBeforeShouldAndRenameAssertion("HaveLength");
            case nameof(DiagnosticMetadata.DictionaryShouldContainKey_ContainsKeyShouldBeTrue):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("ContainKey");
            case nameof(DiagnosticMetadata.DictionaryShouldNotContainKey_ContainsKeyShouldBeFalse):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotContainKey");
            case nameof(DiagnosticMetadata.DictionaryShouldContainValue_ContainsValueShouldBeTrue):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("ContainValue");
            case nameof(DiagnosticMetadata.DictionaryShouldNotContainValue_ContainsValueShouldBeFalse):
                return RemoveMethodBeforeShouldAndRenameAssertionWithArgumentsFromRemoved("NotContainValue");
            case nameof(DiagnosticMetadata.DictionaryShouldContainKeyAndValue_ShouldContainKeyAndContainValue):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithNameAndArguments("Contain", strategy: CombineAssertionArgumentsStrategy.InsertSecondAssertionIntoIndex1OfFirstAssertion),
                ]);
            case nameof(DiagnosticMetadata.DictionaryShouldContainKeyAndValue_ShouldContainValueAndContainKey):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithNameAndArguments("Contain", strategy: CombineAssertionArgumentsStrategy.InsertFirstAssertionIntoIndex1OfSecondAssertion),
                ]);
            case nameof(DiagnosticMetadata.DictionaryShouldContainPair_ShouldContainKeyAndContainValue):
            case nameof(DiagnosticMetadata.DictionaryShouldContainPair_ShouldContainValueAndContainKey):
                return RewriteFluentChainedAssertion(assertion, context, [
                    FluentChainedAssertionEditAction.CombineAssertionsWithName("Contain", (editor, context) =>
                    {
                        var pairAccessor = context.AssertionA.Arguments[0].GetFirstDescendent<IPropertyReferenceOperation>();
                        var pairArgument = (ArgumentSyntax)editor.Generator.Argument(pairAccessor.Instance.Syntax);

                        return SF.ArgumentList()
                            .AddArguments(pairArgument)
                            .AddArguments([..context.AssertionAExpression.ArgumentList.Arguments.Skip(1)])
                            .AddArguments([..context.AssertionBExpression.ArgumentList.Arguments.Skip(1)]);
                    }),
                ]);
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeOfType):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeOfType):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeOfType):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeOfType):
                return ReplaceShouldThrowWithInnerException(assertion, context, "WithInnerExceptionExactly");
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowWhichInnerExceptionShouldBeAssignableTo):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyWhichInnerExceptionShouldBeAssignableTo):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithInnerException_ShouldThrowAndInnerExceptionShouldBeAssignableTo):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithInnerException_ShouldThrowExactlyAndInnerExceptionShouldBeAssignableTo):
                return ReplaceShouldThrowWithInnerException(assertion, context, "WithInnerException");
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldContain):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldContain):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldContain):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldContain):
                return ReplaceShouldThrowWithMessage(assertion, context, prefix: "*", postfix: "*");
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldBe):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldBe):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldBe):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldBe):
                return ReplaceShouldThrowWithMessage(assertion, context);
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldStartWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldStartWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldStartWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldStartWith):
                return ReplaceShouldThrowWithMessage(assertion, context, postfix: "*");
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowWhichMessageShouldEndWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyWhichMessageShouldEndWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowWithMessage_ShouldThrowAndMessageShouldEndWith):
            case nameof(DiagnosticMetadata.ExceptionShouldThrowExactlyWithMessage_ShouldThrowExactlyAndMessageShouldEndWith):
                return ReplaceShouldThrowWithMessage(assertion, context, prefix: "*");
            case nameof(DiagnosticMetadata.CollectionShouldEqual_CollectionShouldEquals):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("Equal")
                ]);
            case nameof(DiagnosticMetadata.StringShouldBe_StringShouldEquals):
            case nameof(DiagnosticMetadata.ShouldBe_ShouldEquals):
                return RewriteAwesomeAssertion(assertion, context, [
                    AwesomeAssertionsEditAction.RenameAssertion("Be")
                ]);
            default:
                return null;
        }

        return null;
    }

    protected override EmptyTestContext CreateTestContext(SemanticModel semanticModel) => new();

    public class EmptyTestContext
    {

    }
}
