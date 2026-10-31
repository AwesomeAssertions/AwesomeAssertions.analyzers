using System;

namespace AwesomeAssertions.Analyzers.Docs;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ExpectedAssertionExceptionAttribute : Attribute;
