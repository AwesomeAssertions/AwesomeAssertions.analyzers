using System;
using System.IO;

namespace AwesomeAssertions.Analyzers.Docs.Generator;

public static class AwesomeAssertionAnalyzerDocsUtils
{
    private const string AwesomeAssertionsAnalyzersDocs = "AwesomeAssertions.Analyzers.Docs";
    private const char UnixDirectorySeparator = '/';
    private const string UnixNewLine = "\n";
    private static readonly string AwesomeAssertionsAnalyzersDocsDirectory = Path.Combine("..", AwesomeAssertionsAnalyzersDocs);

    public static string ReplaceStackTrace(string messageIncludingStacktrace)
    {
        var currentFullPath = Path.GetFullPath(AwesomeAssertionsAnalyzersDocsDirectory) + Path.DirectorySeparatorChar;
        var repoRootIndex = currentFullPath.LastIndexOf(Path.DirectorySeparatorChar + "AwesomeAssertions.analyzers" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
        var unixFullPath = currentFullPath
            .Replace(currentFullPath.Substring(0, repoRootIndex), "/Users/runner/work", StringComparison.Ordinal)
            .Replace(Path.DirectorySeparatorChar, UnixDirectorySeparator);

        return messageIncludingStacktrace
            .Replace(currentFullPath, unixFullPath, StringComparison.Ordinal)
            .Replace(Environment.NewLine, UnixNewLine, StringComparison.Ordinal);
    }
}
