using System;
using System.IO;

namespace AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocsGenerator;

public static class AwesomeAssertionAnalyzerDocsUtils
{
    private static readonly string _awesomeAssertionsAnalyzersDocs = "AwesomeAssertions.Analyzers.AwesomeAssertionAnalyzerDocs";
    private static readonly string _awesomeAssertionsAnalyzersDocsDirectory = Path.Combine("..", _awesomeAssertionsAnalyzersDocs);
    private static readonly string _awesomeAssertionsAnalyzersProjectPath = Path.Combine(_awesomeAssertionsAnalyzersDocsDirectory, _awesomeAssertionsAnalyzersDocs + ".csproj");
    private static readonly char _unixDirectorySeparator = '/';
    private static readonly string _unixNewLine = "\n";

    public static string ReplaceStackTrace(string messageIncludingStacktrace)
    {
        var currentFullPath = Path.GetFullPath(_awesomeAssertionsAnalyzersDocsDirectory) + Path.DirectorySeparatorChar;
        var repoRootIndex = currentFullPath.LastIndexOf(Path.DirectorySeparatorChar + "AwesomeAssertions.analyzers" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
        var unixFullPath = currentFullPath
            .Replace(currentFullPath.Substring(0, repoRootIndex), "/Users/runner/work")
            .Replace(Path.DirectorySeparatorChar, _unixDirectorySeparator);

        return messageIncludingStacktrace
            .Replace(currentFullPath, unixFullPath)
            .Replace(Environment.NewLine, _unixNewLine);
    }
}
