using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers.Tips;

public record struct AwesomeAssertionEditActionContext(
    IInvocationOperation Assertion,
    InvocationExpressionSyntax AssertionExpression,
    IInvocationOperation Should,
    IOperation Subject,
    IInvocationOperation InvocationBeforeShould);
