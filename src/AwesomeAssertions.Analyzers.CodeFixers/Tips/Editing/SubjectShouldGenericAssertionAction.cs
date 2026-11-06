using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace AwesomeAssertions.Analyzers.Tips.Editing;

public class SubjectShouldGenericAssertionAction(int argumentIndex, string assertion, ImmutableArray<ITypeSymbol> types)
    : SubjectShouldAssertionAction(argumentIndex, assertion)
{
    private readonly ImmutableArray<ITypeSymbol> types = types;

    protected override SyntaxNode GenerateAssertion(SyntaxGenerator generator) => generator.GenericName(Assertion, types);
}
