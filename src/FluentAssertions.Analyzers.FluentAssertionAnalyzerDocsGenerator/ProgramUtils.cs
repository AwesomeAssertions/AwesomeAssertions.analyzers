using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FluentAssertions.Analyzers.FluentAssertionAnalyzerDocsGenerator;

public static class ProgramUtils
{
    public static Task RunMain<TDocsGenerator, TDocsVerifier>(string[] args)
        where TDocsGenerator : DocsGenerator, new()
        where TDocsVerifier : DocsVerifier, new()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        return args switch
        {
        ["generate"] => new TDocsGenerator().Execute(),
        ["verify"] => new TDocsVerifier().Execute(),
            _ => throw new ArgumentException("Invalid arguments, use 'generate' or 'verify' as argument.")
        };
    }
}