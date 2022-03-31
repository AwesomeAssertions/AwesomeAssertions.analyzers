﻿using System;
using System.Text;

namespace FluentAssertions.Analyzers.Tests
{
    public static class GenerateCode
    {
        public static string GenericIListCodeBlockAssertion(string assertion) => GenericIListAssertion(
            "        {" + Environment.NewLine +
            "            " + assertion + Environment.NewLine +
            "        }");
        public static string GenericIListExpressionBodyAssertion(string assertion) => GenericIListAssertion(
            "            => " + assertion);

        private static string GenericIListAssertion(string bodyExpression) => new StringBuilder()
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Linq;")
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    public class TestClass")
            .AppendLine("    {")
            .AppendLine("        public void TestMethod(IList<TestComplexClass> actual, IList<TestComplexClass> expected, IList<TestComplexClass> unexpected, TestComplexClass expectedItem, TestComplexClass unexpectedItem, int k)")
            .AppendLine(bodyExpression)
            .AppendLine("    }")
            .AppendLine("    public class TestComplexClass")
            .AppendLine("    {")
            .AppendLine("        public bool BooleanProperty { get; set; }")
            .AppendLine("        public string Message { get; set; }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string GenericIEnumerableAssertion(string assertion) => new StringBuilder()
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Linq;")
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    public class TestClass")
            .AppendLine("    {")
            .AppendLine("        public void TestMethod<T>(IEnumerable<T> actual, IEnumerable<T> expected)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string GenericIDictionaryAssertion(string assertion) => new StringBuilder()
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Linq;")
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;")
            .AppendLine("using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    public class TestClass")
            .AppendLine("    {")
            .AppendLine("        public void TestMethod(Dictionary<string, TestComplexClass> actual, IDictionary<string, TestComplexClass> expected, IDictionary<string, TestComplexClass> unexpected, string expectedKey, TestComplexClass expectedValue, string unexpectedKey, TestComplexClass unexpectedValue, KeyValuePair<string, TestComplexClass> pair, KeyValuePair<string, TestComplexClass> otherPair)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendLine("    public class TestComplexClass")
            .AppendLine("    {")
            .AppendLine("        public bool BooleanProperty { get; set; }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string DoubleAssertion(string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;")
            .AppendLine("using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod(double actual, double expected, double lower, double upper, double delta)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string ComparableInt32Assertion(string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;")
            .AppendLine("using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod(IComparable<int> actual, int expected)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string StringAssertion(string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod(string actual, string expected, int k)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string ExceptionAssertion(string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod(Action action, string expectedMessage)")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string AsyncFunctionStatement(string statement) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod()")
            .AppendLine("        {")
            .AppendLine($"            {statement}")
            .AppendLine("        }")
            .AppendLine("        async void AsyncVoidMethod() { await Task.CompletedTask; }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string ObjectStatement(string statement) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using FluentAssertions;using FluentAssertions.Extensions;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine("        void TestMethod(object actual, object expected)")
            .AppendLine("        {")
            .AppendLine($"            {statement}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string MsTestAssertion(string methodArguments, string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;")
            .AppendLine("using FluentAssertions.Extensions;")
            .AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine($"        void TestMethod({methodArguments})")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static string XunitAssertion(string methodArguments, string assertion) => new StringBuilder()
            .AppendLine("using System;")
            .AppendLine("using FluentAssertions;")
            .AppendLine("using FluentAssertions.Extensions;")
            .AppendLine("using Xunit;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("namespace TestNamespace")
            .AppendLine("{")
            .AppendLine("    class TestClass")
            .AppendLine("    {")
            .AppendLine($"        void TestMethod({methodArguments})")
            .AppendLine("        {")
            .AppendLine($"            {assertion}")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendMainMethod()
            .AppendLine("}")
            .ToString();

        public static StringBuilder AppendMainMethod(this StringBuilder builder) => builder
            .AppendLine("    class Program")
            .AppendLine("    {")
            .AppendLine("        public static void Main()")
            .AppendLine("        {")
            .AppendLine("        }")
            .AppendLine("    }");
    }
}
