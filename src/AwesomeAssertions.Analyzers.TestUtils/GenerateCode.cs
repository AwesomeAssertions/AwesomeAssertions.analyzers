using System;

namespace AwesomeAssertions.Analyzers.TestUtils
{
    public static class GenerateCode
    {
        public static string GenericArrayCodeBlockAssertion(string assertion) => GenericArrayAssertion(
            "        {" + Environment.NewLine +
            "            " + assertion + Environment.NewLine +
            "        }");

        private static string GenericArrayAssertion(string bodyExpression) => $$"""
            using System.Collections.Generic;
            using System.Linq;
            using System;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                public class TestClass
                {
                    public void TestMethod(TestComplexClass[] actual, TestComplexClass[] expected, TestComplexClass[] unexpected, TestComplexClass expectedItem, TestComplexClass unexpectedItem, int k)
                        {{bodyExpression}}
                }
                public class TestComplexClass
                {
                    public bool BooleanProperty { get; set; }
                    public string Message { get; set; }
                }
            }
            """;

        public static string GenericIListCodeBlockAssertion(string assertion) => GenericIListAssertion(
            "        {" + Environment.NewLine +
            "            " + assertion + Environment.NewLine +
            "        }");

        public static string GenericIListExpressionBodyAssertion(string assertion) => GenericIListAssertion(
            $"            => {assertion}");

        public static string GenericAssertionOnClassWhichImplementsIReadOnlyList(string genericType, string assertion) => $$"""
            using System;
            using System.Collections;
            using System.Collections.Generic;

            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestNamespace
            {
                public class RandomClass : IReadOnlyList<{{genericType}}>
                {
                    public {{genericType}} this[int index] => 42;
                    public int Count { get; }
                    public IEnumerator<{{genericType}}> GetEnumerator() => throw new System.NotImplementedException();
                    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                }
                public sealed class TestClass
                {
                    public void TestMethod(RandomClass actual) =>
                        {{assertion}}
                }
            }
            """;

        public static string GenericAssertionOnClassWhichImplementsIList(string genericType, string assertion) => $$"""
            using System;
            using System.Collections;
            using System.Collections.Generic;

            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;

            namespace TestNamespace
            {
                public class RandomClass : IList<{{genericType}}>
                {
                    public {{genericType}} this[int index]
                    {
                        get => throw new System.NotImplementedException();
                        set => throw new System.NotImplementedException();
                    }
                    public int Count { get; }
                    public bool IsReadOnly { get; }
                    public void Add({{genericType}} item) => throw new System.NotImplementedException();
                    public void Clear() => throw new System.NotImplementedException();
                    public bool Contains({{genericType}} item) => throw new System.NotImplementedException();
                    public void CopyTo({{genericType}}[] array, int arrayIndex) => throw new System.NotImplementedException();
                    public IEnumerator<{{genericType}}> GetEnumerator() => throw new System.NotImplementedException();
                    public int IndexOf({{genericType}} item) => throw new System.NotImplementedException();
                    public void Insert(int index, {{genericType}} item) => throw new System.NotImplementedException();
                    public bool Remove({{genericType}} item) => throw new System.NotImplementedException();
                    public void RemoveAt(int index) => throw new System.NotImplementedException();
                    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                }
                public sealed class TestClass
                {
                    public void TestMethod(RandomClass actual) =>
                        {{assertion}}
                }
            }
            """;

        private static string GenericIListAssertion(string bodyExpression) => $$"""
            using System.Collections.Generic;
            using System.Linq;
            using System;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                public class TestClass
                {
                    public void TestMethod(IList<TestComplexClass> actual, IList<TestComplexClass> expected, IList<TestComplexClass> unexpected, TestComplexClass expectedItem, TestComplexClass unexpectedItem, int k)
                        {{bodyExpression}}
                }
                public class TestComplexClass
                {
                    public bool BooleanProperty { get; set; }
                    public string Message { get; set; }
                    public TestComplexClass[] Children { get; set; }
                    public TestComplexClass Parent { get; set; }
                    public TestComplexClass this[int index] => throw new NotImplementedException();
                }
            }
            """;

        public static string GenericIEnumerableAssertion(string assertion) => $$"""
            using System.Collections.Generic;
            using System.Linq;
            using System;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                public class TestClass
                {
                    public void TestMethod<T>(IEnumerable<T> actual, IEnumerable<T> expected)
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string GenericIDictionaryAssertion(string assertion) => $$"""
            using System.Collections.Generic;
            using System.Linq;
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                public class TestClass
                {
                    public void TestMethod(Dictionary<string, TestComplexClass> actual, IDictionary<string, TestComplexClass> expected, IDictionary<string, TestComplexClass> unexpected, string expectedKey, TestComplexClass expectedValue, string unexpectedKey, TestComplexClass unexpectedValue, KeyValuePair<string, TestComplexClass> pair, KeyValuePair<string, TestComplexClass> otherPair)
                    {
                        {{assertion}}
                    }
                }
                public class TestComplexClass
                {
                    public bool BooleanProperty { get; set; }
                }
            }
            """;

        public static string GenericSimpleIDictionaryAssertion(string assertion, string simpleType) => $$"""
           using System.Collections.Generic;
           using System.Linq;
           using System;
           using AwesomeAssertions;
           using AwesomeAssertions.Extensions;
           namespace TestNamespace
           {
               public class TestClass
               {
                   public void TestMethod(Dictionary<string, {{simpleType}}> actual, IDictionary<string, {{simpleType}}> expected)
                   {
                       {{assertion}}
                   }
               }
           }
           """;

        public static string DoubleAssertion(string assertion) => NumericAssertion(assertion, "double");

        public static string NumericAssertion(string assertion, string type) => $$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod({{type}} actual, {{type}} expected, {{type}} lower, {{type}} upper, {{type}} delta)
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string StringAssertion(string assertion) => $$"""
            using System;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod(string actual, string expected, int k)
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string ExceptionAssertion(string assertion) => $$"""
            using System;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod(Action action, string expectedMessage)
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string AsyncFunctionStatement(string statement) => $$"""
            using System;
            using System.Threading.Tasks;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod()
                    {
                        {{statement}}
                    }
                    async void AsyncVoidMethod() { await Task.CompletedTask; }
                }
            }
            """;

        public static string ObjectStatement(string statement) => $$"""
            using System;
            using System.Threading.Tasks;
            using AwesomeAssertions;using AwesomeAssertions.Extensions;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod(object actual, object expected)
                    {
                        {{statement}}
                    }
                }
            }
            """;

        public static string MsTestAssertion(string methodArguments, string assertion) => $$"""
            using System;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using System.Threading.Tasks;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod({{methodArguments}})
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string XunitAssertion(string methodArguments, string assertion) => $$"""
            using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Text.RegularExpressions;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using Xunit;
            using System.Threading.Tasks;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod({{methodArguments}})
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string Nunit3Assertion(string methodArguments, string assertion) => $$"""
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Text.RegularExpressions;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using NUnit.Framework;
            using System.Threading.Tasks;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod({{methodArguments}})
                    {
                        {{assertion}}
                    }
                }
            }
            """;

        public static string Nunit4Assertion(string methodArguments, string assertion) => $$"""
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Text.RegularExpressions;
            using AwesomeAssertions;
            using AwesomeAssertions.Extensions;
            using NUnit.Framework; using NUnit.Framework.Legacy;
            using System.Threading.Tasks;
            namespace TestNamespace
            {
                class TestClass
                {
                    void TestMethod({{methodArguments}})
                    {
                        {{assertion}}
                    }
                }
            }
            """;
    }
}
