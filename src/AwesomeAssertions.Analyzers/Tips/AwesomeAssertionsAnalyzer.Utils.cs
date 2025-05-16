using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers;

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
        var current = invocation.Parent;
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

    private class AwesomeAssertionsMetadata
    {
        public AwesomeAssertionsMetadata(Compilation compilation)
        {
            AssertionExtensions = compilation.GetTypeByMetadataName("AwesomeAssertions.AssertionExtensions");
            ReferenceTypeAssertionsOfT2 = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.ReferenceTypeAssertions`2");
            ObjectAssertionsOfT2 = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.ObjectAssertions`2");
            NumericAssertionsBaseOfT3 = compilation.GetTypeByMetadataName("AwesomeAssertions.Numeric.NumericAssertionsBase`3");
            BooleanAssertionsOfT1 = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.BooleanAssertions`1");
            GenericCollectionAssertionsOfT3 = compilation.GetTypeByMetadataName("AwesomeAssertions.Collections.GenericCollectionAssertions`3");
            GenericDictionaryAssertionsOfT4 = compilation.GetTypeByMetadataName("AwesomeAssertions.Collections.GenericDictionaryAssertions`4");
            StringAssertionsOfT1 = compilation.GetTypeByMetadataName("AwesomeAssertions.Primitives.StringAssertions`1");
            ExceptionAssertionsOfT1 = compilation.GetTypeByMetadataName("AwesomeAssertions.Specialized.ExceptionAssertions`1");
            DelegateAssertionsOfT2 = compilation.GetTypeByMetadataName("AwesomeAssertions.Specialized.DelegateAssertions`2");
            IDictionaryOfT2 = compilation.GetTypeByMetadataName(typeof(IDictionary<,>).FullName);
            DictionaryOfT2 = compilation.GetTypeByMetadataName(typeof(Dictionary<,>).FullName);
            IReadonlyDictionaryOfT2 = compilation.GetTypeByMetadataName(typeof(IReadOnlyDictionary<,>).FullName);
            IListOfT = compilation.GetTypeByMetadataName(typeof(IList<>).FullName);
            IReadonlyListOfT = compilation.GetTypeByMetadataName(typeof(IReadOnlyList<>).FullName);
            ICollectionOfT = compilation.GetTypeByMetadataName(typeof(ICollection<>).FullName);
            IReadonlyCollectionOfT = compilation.GetTypeByMetadataName(typeof(IReadOnlyCollection<>).FullName);
            Enumerable = compilation.GetTypeByMetadataName(typeof(Enumerable).FullName);
            IEnumerable = compilation.GetTypeByMetadataName(typeof(IEnumerable).FullName);
            Math = compilation.GetTypeByMetadataName(typeof(Math).FullName);
            TaskCompletionSourceOfT1 = compilation.GetTypeByMetadataName(typeof(TaskCompletionSource<>).FullName);
            Stream = compilation.GetTypeByMetadataName(typeof(Stream).FullName);
        }
        public INamedTypeSymbol AssertionExtensions { get; }
        public INamedTypeSymbol ReferenceTypeAssertionsOfT2 { get; }
        public INamedTypeSymbol ObjectAssertionsOfT2 { get; }
        public INamedTypeSymbol GenericCollectionAssertionsOfT3 { get; }
        public INamedTypeSymbol GenericDictionaryAssertionsOfT4 { get; }
        public INamedTypeSymbol StringAssertionsOfT1 { get; }
        public INamedTypeSymbol ExceptionAssertionsOfT1 { get; }
        public INamedTypeSymbol DelegateAssertionsOfT2 { get; }
        public INamedTypeSymbol IDictionaryOfT2 { get; }
        public INamedTypeSymbol DictionaryOfT2 { get; }
        public INamedTypeSymbol IReadonlyDictionaryOfT2 { get; }
        public INamedTypeSymbol IListOfT { get; }
        public INamedTypeSymbol IReadonlyListOfT { get; }
        public INamedTypeSymbol ICollectionOfT { get; }
        public INamedTypeSymbol IReadonlyCollectionOfT { get; }
        public INamedTypeSymbol BooleanAssertionsOfT1 { get; }
        public INamedTypeSymbol NumericAssertionsBaseOfT3 { get; }
        public INamedTypeSymbol Enumerable { get; }
        public INamedTypeSymbol IEnumerable { get; }
        public INamedTypeSymbol Math { get; }
        public INamedTypeSymbol TaskCompletionSourceOfT1 { get; }
        public INamedTypeSymbol Stream { get; }
    }
}