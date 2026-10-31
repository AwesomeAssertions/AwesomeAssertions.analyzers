using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers.Tips;

public partial class AwesomeAssertionsAnalyzer
{
    private static bool IsEnumerableMethodWithoutArguments(IInvocationOperation invocation, AwesomeAssertionsMetadata metadata)
        => invocation.IsContainedInType(metadata.Enumerable) && invocation.Arguments.Length == 1;

    private static bool IsEnumerableMethodWithPredicate(IInvocationOperation invocation, AwesomeAssertionsMetadata metadata)
        => invocation.IsContainedInType(metadata.Enumerable) && invocation.Arguments.Length == 2 && invocation.Arguments[1].IsLambda(); // invocation.Arguments[0] is `this` argument

    private static bool TryGetExceptionPropertyAssertion(IInvocationOperation assertion, out string awesomeAssertionProperty, out string exceptionProperty, out IInvocationOperation nextAssertion)
    {
        if (assertion.Parent is IPropertyReferenceOperation chainProperty
        && chainProperty.Parent is IPropertyReferenceOperation exception
        && exception.Parent.UnwrapParentConversion() is IArgumentOperation argument
        && argument.Parent is IInvocationOperation { TargetMethod.Name: "Should" } should)
        {
            nextAssertion = should.Parent as IInvocationOperation;
            awesomeAssertionProperty = chainProperty.Property.Name;
            exceptionProperty = exception.Property.Name;
            return nextAssertion is not null;
        }

        nextAssertion = default;
        awesomeAssertionProperty = default;
        exceptionProperty = default;
        return false;
    }

    private static bool HasConditionalAccessAncestor(IInvocationOperation invocation)
    {
        IOperation current = invocation.Parent;
        while (current is not null)
        {
            if (current.Kind is OperationKind.ConditionalAccess)
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }

#pragma warning disable AV1704 // Identifier contains one or more digits in its name: Correct here because it matches the type name
    private sealed class AwesomeAssertionsMetadata(Compilation compilation)
    {
        public INamedTypeSymbol AssertionExtensions { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.AssertionExtensions");

        public INamedTypeSymbol ReferenceTypeAssertionsOfT2 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.ReferenceTypeAssertions`2");

        public INamedTypeSymbol ObjectAssertionsOfT2 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.ObjectAssertions`2");

        public INamedTypeSymbol GenericCollectionAssertionsOfT3 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Collections.GenericCollectionAssertions`3");

        public INamedTypeSymbol GenericDictionaryAssertionsOfT4 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Collections.GenericDictionaryAssertions`4");

        public INamedTypeSymbol StringAssertionsOfT1 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.StringAssertions`1");

        public INamedTypeSymbol ExceptionAssertionsOfT1 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Specialized.ExceptionAssertions`1");

        public INamedTypeSymbol DelegateAssertionsOfT2 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Specialized.DelegateAssertions`2");

        public INamedTypeSymbol IDictionaryOfT2 { get; } = compilation.GetTypeByMetadataName(typeof(IDictionary<,>).FullName);

        public INamedTypeSymbol DictionaryOfT2 { get; } = compilation.GetTypeByMetadataName(typeof(Dictionary<,>).FullName);

        public INamedTypeSymbol IReadonlyDictionaryOfT2 { get; } = compilation.GetTypeByMetadataName(typeof(IReadOnlyDictionary<,>).FullName);

        public INamedTypeSymbol IListOfT { get; } = compilation.GetTypeByMetadataName(typeof(IList<>).FullName);

        public INamedTypeSymbol IReadonlyListOfT { get; } = compilation.GetTypeByMetadataName(typeof(IReadOnlyList<>).FullName);

        public INamedTypeSymbol ICollectionOfT { get; } = compilation.GetTypeByMetadataName(typeof(ICollection<>).FullName);

        public INamedTypeSymbol IReadonlyCollectionOfT { get; } = compilation.GetTypeByMetadataName(typeof(IReadOnlyCollection<>).FullName);

        public INamedTypeSymbol BooleanAssertionsOfT1 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.BooleanAssertions`1");

        public INamedTypeSymbol NumericAssertionsBaseOfT3 { get; } = compilation.GetTypeByMetadataName("AwesomeAssertions.Numeric.NumericAssertionsBase`3");

        public INamedTypeSymbol Enumerable { get; } = compilation.GetTypeByMetadataName(typeof(Enumerable).FullName);

        public INamedTypeSymbol IEnumerable { get; } = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName);

        public INamedTypeSymbol Math { get; } = compilation.GetTypeByMetadataName(typeof(Math).FullName);

        public INamedTypeSymbol TaskCompletionSourceOfT1 { get; } = compilation.GetTypeByMetadataName(typeof(TaskCompletionSource<>).FullName);

        public INamedTypeSymbol Stream { get; } = compilation.GetTypeByMetadataName(typeof(Stream).FullName);
    }
}
