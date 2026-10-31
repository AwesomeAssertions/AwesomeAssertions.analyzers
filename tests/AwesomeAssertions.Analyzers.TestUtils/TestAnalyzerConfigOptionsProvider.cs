using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AwesomeAssertions.Analyzers.TestUtils;

public sealed class TestAnalyzerConfigOptionsProvider(IDictionary<string, string> values) : AnalyzerConfigOptionsProvider
{
    public static TestAnalyzerConfigOptionsProvider Empty { get; } = new(ImmutableDictionary<string, string>.Empty);

    private readonly IDictionary<string, string> values = values;

    public override AnalyzerConfigOptions GlobalOptions => new TestAnalyzerConfigOptions(values);

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new TestAnalyzerConfigOptions(values);

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new TestAnalyzerConfigOptions(values);

    private sealed class TestAnalyzerConfigOptions(IDictionary<string, string> values) : AnalyzerConfigOptions
    {
        private readonly IDictionary<string, string> values = values;

        public override bool TryGetValue(string key, out string value)
        {
            return values.TryGetValue(key, out value);
        }
    }
}
