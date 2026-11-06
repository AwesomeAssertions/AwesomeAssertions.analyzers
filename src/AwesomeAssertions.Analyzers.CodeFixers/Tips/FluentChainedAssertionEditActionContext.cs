using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers.Tips;

public record struct FluentChainedAssertionEditActionContext(
    IInvocationOperation AssertionA,
    InvocationExpressionSyntax AssertionAExpression,
    IPropertyReferenceOperation AndOrWhich,
    IInvocationOperation AssertionB,
    InvocationExpressionSyntax AssertionBExpression,
    IInvocationOperation Should,
    IOperation Subject);
