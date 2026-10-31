using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions.Analyzers.TestUtils;
using AwesomeAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace AwesomeAssertions.Analyzers.Tests;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high: To be refactored later
/// <summary>
/// Superclass of all Unit Tests for DiagnosticAnalyzers
/// </summary>
public static class DiagnosticVerifier
{
    #region CodeFixVerifier

    public static Task VerifyCSharpFix<TCodeFixProvider, TDiagnosticAnalyzer>(string oldSource, string newSource)
        where TCodeFixProvider : CodeFixProvider, new()
        where TDiagnosticAnalyzer : DiagnosticAnalyzer, new()
    {
        return VerifyFix(new CodeFixVerifierArguments()
            .WithDiagnosticAnalyzer<TDiagnosticAnalyzer>()
            .WithCodeFixProvider<TCodeFixProvider>()
            .WithSources(oldSource)
            .WithFixedSources(newSource)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
        );
    }

    public static Task VerifyFix(CodeFixVerifierArguments arguments)
        => VerifyFix(arguments, arguments.DiagnosticAnalyzers.Single(), arguments.CodeFixProviders.Single(), arguments.FixedSources.Single());

    public static Task VerifyNoFix(CodeFixVerifierArguments arguments)
        => VerifyNoFix(arguments, arguments.DiagnosticAnalyzers.Single(), arguments.CodeFixProviders.Single());

    private static async Task VerifyNoFix(CsProjectArguments arguments, DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider)
    {
        Document document = CsProjectGenerator.CreateDocument(arguments);
        Diagnostic[] analyzerDiagnostics = await GetSortedDiagnosticsFromDocuments([analyzer], [document]);
        var attempts = analyzerDiagnostics.Length;

        for (int i = 0; i < attempts; ++i)
        {
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, _) => actions.Add(a), CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(context);

            actions.Should().BeEmpty("There should be no code fix actions available to suppress the diagnostic.");
        }
    }

    /// <summary>
    /// General verifier for codefixes.
    /// Creates a Document from the source string, then gets diagnostics on it and applies the relevant codefixes.
    /// Then gets the string after the codefix is applied and compares it with the expected result.
    /// Note: If any codefix causes new diagnostics to show up, the test fails unless allowNewCompilerDiagnostics is set to true.
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="analyzer">The analyzer to be applied to the source code</param>
    /// <param name="codeFixProvider">The codefix to be applied to the code wherever the relevant Diagnostic is found</param>
    /// <param name="newSource">A class in the form of a string after the CodeFix was applied to it</param>
    /// <exception cref="AssertionFailedException">Thrown if the code fix does not produce the expected result</exception>
    private static async Task VerifyFix(CsProjectArguments arguments, DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider, string newSource)
    {
        Document document = CsProjectGenerator.CreateDocument(arguments);
        Diagnostic[] analyzerDiagnostics = await GetSortedDiagnosticsFromDocuments([analyzer], [document]);
        IEnumerable<Diagnostic> compilerDiagnostics = GetCompilerDiagnostics(document);
        var attempts = analyzerDiagnostics.Length;

        for (int i = 0; i < attempts; ++i)
        {
            var actions = new List<CodeAction>();
            var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, _) => actions.Add(a), CancellationToken.None);
            await codeFixProvider.RegisterCodeFixesAsync(context);

#pragma warning disable S2583 // Conditionally executed code should be reachable: False positive
            if (actions.Count == 0)
            {
                break;
            }
#pragma warning restore S2583 // Conditionally executed code should be reachable

            document = ApplyFix(document, actions[0]);
            analyzerDiagnostics = await GetSortedDiagnosticsFromDocuments([analyzer], [document]);

            IEnumerable<Diagnostic> newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

            // check if applying the code fix introduced any new compiler diagnostics
            if (newCompilerDiagnostics.Any())
            {
                // Format and get the compiler diagnostics again so that the locations make sense in the output
                document = document.WithSyntaxRoot(Formatter.Format(await document.GetSyntaxRootAsync(context.CancellationToken), Formatter.Annotation, document.Project.Solution.Workspace, cancellationToken: context.CancellationToken));
                newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                throw new AssertionFailedException(string.Format("Fix introduced new compiler diagnostics:\r\n{0}\r\n\r\nNew document:\r\n{1}\r\n",
                    string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString())),
                    (await document.GetSyntaxRootAsync(context.CancellationToken)).ToFullString()));
            }

            // check if there are analyzer diagnostics left after the code fix
            if (analyzerDiagnostics.Length > 0)
            {
                break;
            }
        }

        codeFixProvider.FixableDiagnosticIds.Should().BeSubsetOf(analyzer.SupportedDiagnostics.Select(d => d.Id));

        // after applying all of the code fixes, compare the resulting string to the inputted one
        var actual = GetStringFromDocument(document);
        actual.Should().Be(newSource);
    }

    /// <summary>
    /// Apply the inputted CodeAction to the inputted document.
    /// Meant to be used to apply codefixes.
    /// </summary>
    /// <param name="document">The Document to apply the fix on</param>
    /// <param name="codeAction">A CodeAction that will be applied to the Document.</param>
    /// <returns>A Document with the changes from the CodeAction</returns>
    private static Document ApplyFix(Document document, CodeAction codeAction)
    {
        ImmutableArray<CodeActionOperation> operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
        Solution solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
        return solution.GetDocument(document.Id);
    }

    /// <summary>
    /// Compare two collections of Diagnostics,and return a list of any new diagnostics that appear only in the second collection.
    /// Note: Considers Diagnostics to be the same if they have the same Ids.  In the case of multiple diagnostics with the same Id in a row,
    /// this method may not necessarily return the new one.
    /// </summary>
    /// <param name="diagnostics">The Diagnostics that existed in the code before the CodeFix was applied</param>
    /// <param name="newDiagnostics">The Diagnostics that exist in the code after the CodeFix was applied</param>
    /// <returns>A list of Diagnostics that only surfaced in the code after the CodeFix was applied</returns>
    private static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics, IEnumerable<Diagnostic> newDiagnostics)
    {
        Diagnostic[] oldArray = [.. diagnostics.OrderBy(d => d.Location.SourceSpan.Start)];
        Diagnostic[] newArray = [.. newDiagnostics.OrderBy(d => d.Location.SourceSpan.Start)];

        int oldIndex = 0;
        int newIndex = 0;

        while (newIndex < newArray.Length)
        {
            if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
            {
                ++oldIndex;
                ++newIndex;
            }
            else
            {
                yield return newArray[newIndex++];
            }
        }
    }

    /// <summary>
    /// Get the existing compiler diagnostics on the inputted document.
    /// </summary>
    /// <param name="document">The Document to run the compiler diagnostic analyzers on</param>
    /// <returns>The compiler diagnostics that were found in the code</returns>
    private static ImmutableArray<Diagnostic> GetCompilerDiagnostics(Document document)
    {
        Compilation compilation = document.GetSemanticModelAsync().Result.Compilation;
        return compilation.WithOptions(compilation.Options
            .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>(StringComparer.Ordinal)
            {
                ["CS1701"] = ReportDiagnostic.Suppress, // Binding redirects
                ["CS1702"] = ReportDiagnostic.Suppress,
                ["CS1705"] = ReportDiagnostic.Suppress,
                ["CS8019"] = ReportDiagnostic.Suppress // Unnecessary using directive
            })
        ).GetDiagnostics();
    }

    /// <summary>
    /// Given a document, turn it into a string based on the syntax root
    /// </summary>
    /// <param name="document">The Document to be converted to a string</param>
    /// <returns>A string containing the syntax of the Document after formatting</returns>
    private static string GetStringFromDocument(Document document)
    {
        Document simplifiedDoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
        SyntaxNode root = simplifiedDoc.GetSyntaxRootAsync().Result;
        root = Formatter.Format(root, Formatter.Annotation, simplifiedDoc.Project.Solution.Workspace);
        return root.GetText().ToString();
    }

    #endregion

    #region  Get Diagnostics

    /// <summary>
    /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
    /// The returned diagnostics are then ordered by location in the source document.
    /// </summary>
    /// <param name="analyzers">The analyzer to run on the documents</param>
    /// <param name="documents">The Documents that the analyzer will be run on</param>
    /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
    private static Task<Diagnostic[]> GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer[] analyzers, Document[] documents) => GetSortedDiagnosticsFromDocuments(analyzers, TestAnalyzerConfigOptionsProvider.Empty, documents);

    private static async Task<Diagnostic[]> GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer[] analyzers, AnalyzerConfigOptionsProvider analyzerConfigOptions, Document[] documents)
    {
        var projects = new HashSet<Project>();
        foreach (Document document in documents)
        {
            projects.Add(document.Project);
        }

        var diagnostics = new List<Diagnostic>();
        foreach (Project project in projects)
        {
            Compilation compilation = await project.GetCompilationAsync();
            CompilationWithAnalyzers compilationWithAnalyzers = compilation
                .WithOptions(compilation.Options
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary)
                    .WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>(StringComparer.Ordinal)
                    {
                        ["CS1701"] = ReportDiagnostic.Suppress, // Binding redirects
                        ["CS1702"] = ReportDiagnostic.Suppress,
                        ["CS1705"] = ReportDiagnostic.Suppress,
                        ["CS8019"] = ReportDiagnostic.Suppress // Unnecessary using directive
                    }))
                .WithAnalyzers(ImmutableArray.Create(analyzers), new AnalyzerOptions([], analyzerConfigOptions));
            ImmutableArray<Diagnostic> relevantDiagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            ImmutableArray<Diagnostic> allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync();
            Diagnostic[] other = allDiagnostics.Except(relevantDiagnostics).ToArray();

            var code = (await documents[0].GetSyntaxRootAsync()).ToFullString();

            other.Should().BeEmpty("there should be no error diagnostics that are not related to the test.{0}code: {1}", Environment.NewLine, code);

            foreach (Diagnostic diag in relevantDiagnostics)
            {
                if (diag.Location == Location.None || diag.Location.IsInMetadata)
                {
                    diagnostics.Add(diag);
                }
                else
                {
                    for (int i = 0; i < documents.Length; i++)
                    {
                        Document document = documents[i];
                        SyntaxTree tree = await document.GetSyntaxTreeAsync();
                        if (tree == diag.Location.SourceTree)
                        {
                            diagnostics.Add(diag);
                        }
                    }
                }
            }
        }

        Diagnostic[] results = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        diagnostics.Clear();
        return results;
    }

    #endregion

    #region Verifier wrappers

    public static async Task VerifyDiagnostic(DiagnosticVerifierArguments arguments)
    {
        Project project = CsProjectGenerator.CreateProject(arguments);
        Document[] documents = project.Documents.ToArray();

        Diagnostic[] diagnostics = await GetSortedDiagnosticsFromDocuments(arguments.DiagnosticAnalyzers.ToArray(), arguments.CreateAnalyzerConfigOptionsProvider(), documents);
        VerifyDiagnosticResults(diagnostics, arguments.DiagnosticAnalyzers.ToArray(), arguments.ExpectedDiagnostics.ToArray());
    }

    public static Task VerifyCSharpDiagnosticUsingAllAnalyzers(string source, params DiagnosticResult[] expected)
    {
        return VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest)
            .WithExpectedDiagnostics(expected));
    }

    #endregion

    #region Actual comparisons and verifications

    private static void VerifyDiagnosticResults(Diagnostic[] actualResults, DiagnosticAnalyzer[] analyzers, params DiagnosticResult[] expectedResults)
    {
        int expectedCount = expectedResults.Length;
        int actualCount = actualResults.Length;

        if (expectedCount != actualCount)
        {
            string diagnosticsOutput = actualResults.Length > 0 ? FormatDiagnostics(analyzers, actualResults) : "    NONE.";

            throw new AssertionFailedException(
                string.Format("Mismatch between number of diagnostics returned, expected \"{0}\" actual \"{1}\"\r\n\r\nDiagnostics:\r\n{2}\r\n", expectedCount, actualCount, diagnosticsOutput));
        }

        for (int i = 0; i < expectedResults.Length; i++)
        {
            Diagnostic actual = actualResults[i];
            DiagnosticResult expected = expectedResults[i];

            if (expected.Line == -1 && expected.Column == -1)
            {
                if (actual.Location != Location.None)
                {
                    throw new AssertionFailedException(
                        string.Format("Expected:\nA project diagnostic with No location\nActual:\n{0}",
                        FormatDiagnostics(analyzers, actual)));
                }
            }
            else
            {
                VerifyDiagnosticLocation(analyzers, actual, actual.Location, expected.Locations[0]);
                IReadOnlyList<Location> additionalLocations = actual.AdditionalLocations;

                if (additionalLocations.Count != expected.Locations.Length - 1)
                {
                    throw new AssertionFailedException(
                        string.Format("Expected {0} additional locations but got {1} for Diagnostic:\r\n    {2}\r\n",
                            expected.Locations.Length - 1, additionalLocations.Count,
                            FormatDiagnostics(analyzers, actual)));
                }

                for (int j = 0; j < additionalLocations.Count; ++j)
                {
                    VerifyDiagnosticLocation(analyzers, actual, additionalLocations[j], expected.Locations[j + 1]);
                }
            }

            if (actual.Id != expected.Id)
            {
                throw new AssertionFailedException(
                    string.Format("Expected diagnostic id to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Id, actual.Id, FormatDiagnostics(analyzers, actual)));
            }

            if (actual.Severity != expected.Severity)
            {
                throw new AssertionFailedException(
                    string.Format("Expected diagnostic severity to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Severity, actual.Severity, FormatDiagnostics(analyzers, actual)));
            }

            if (actual.GetMessage() != expected.Message)
            {
                throw new AssertionFailedException(
                    string.Format("Expected diagnostic message to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.Message, actual.GetMessage(), FormatDiagnostics(analyzers, actual)));
            }

            if (expected.VisitorName != null && actual.Properties[Constants.DiagnosticProperties.VisitorName] != expected.VisitorName)
            {
                throw new AssertionFailedException(
                    string.Format("Expected diagnostic visitorName property to be \"{0}\" was \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                        expected.VisitorName, actual.Properties[Constants.DiagnosticProperties.VisitorName], FormatDiagnostics(analyzers, actual)));
            }
        }
    }

    /// <summary>
    /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and compares it with the location in the expected DiagnosticResult.
    /// </summary>
    /// <param name="analyzers">The analyzer that was being run on the sources</param>
    /// <param name="diagnostic">The diagnostic that was found in the code</param>
    /// <param name="actual">The Location of the Diagnostic found in the code</param>
    /// <param name="expected">The DiagnosticResultLocation that should have been found</param>
    /// <exception cref="AssertionFailedException">Thrown if assertion failed</exception>
    private static void VerifyDiagnosticLocation(DiagnosticAnalyzer[] analyzers, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
    {
        FileLinePositionSpan actualSpan = actual.GetLineSpan();

        (actualSpan.Path == expected.Path || (actualSpan.Path != null && actualSpan.Path.Contains("Test0.") && expected.Path.Contains("Test.")))
            .Should().BeTrue(string.Format("Expected diagnostic to be in file \"{0}\" was actually in file \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                expected.Path, actualSpan.Path, FormatDiagnostics(analyzers, diagnostic)));

        LinePosition actualLinePosition = actualSpan.StartLinePosition;

        // Only check line position if there is an actual line in the real diagnostic
        if (actualLinePosition.Line > 0 && actualLinePosition.Line + 1 != expected.Line)
        {
            throw new AssertionFailedException(string.Format("Expected diagnostic to be on line \"{0}\" was actually on line \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                expected.Line, actualLinePosition.Line + 1, FormatDiagnostics(analyzers, diagnostic)));
        }

        // Only check column position if there is an actual column position in the real diagnostic
        if (actualLinePosition.Character > 0 && actualLinePosition.Character + 1 != expected.Column)
        {
            throw new AssertionFailedException(
                string.Format("Expected diagnostic to start at column \"{0}\" was actually at column \"{1}\"\r\n\r\nDiagnostic:\r\n    {2}\r\n",
                    expected.Column, actualLinePosition.Character + 1, FormatDiagnostics(analyzers, diagnostic)));
        }
    }
    #endregion

    #region Formatting Diagnostics

    /// <summary>
    /// Helper method to format a Diagnostic into an easily readable string
    /// </summary>
    /// <param name="analyzers">The analyzer that this verifier tests</param>
    /// <param name="diagnostics">The Diagnostics to be formatted</param>
    /// <returns>The Diagnostics formatted as a string</returns>
    private static string FormatDiagnostics(DiagnosticAnalyzer[] analyzers, params Diagnostic[] diagnostics)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < diagnostics.Length; ++i)
        {
            builder.Append("// ").Append(diagnostics[i]).AppendLine();

            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                Type analyzerType = analyzer.GetType();
                foreach (DiagnosticDescriptor rule in analyzer.SupportedDiagnostics)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        Location location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.AppendFormat("GetGlobalResult({0}.{1})", analyzerType.Name, rule.Id);
                        }
                        else
                        {
                            location.IsInSource.Should().BeTrue($"Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata: {diagnostics[i]}\r\n");

                            string resultMethodName = diagnostics[i].Location.SourceTree.FilePath.EndsWith(".cs") ? "GetCSharpResultAt" : "GetBasicResultAt";
                            LinePosition linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.AppendFormat("{0}({1}, {2}, {3}.{4})",
                                resultMethodName,
                                linePosition.Line + 1,
                                linePosition.Character + 1,
                                analyzerType.Name,
                                rule.Id);
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }
        }

        return builder.ToString();
    }
    #endregion
}
