using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AwesomeAssertions.Analyzers.Docs.Generator;

public abstract class DocsGenerator
{
    private const string OldAssertionComment = "// old assertion:";
    private const string NewAssertionComment = "// new assertion:";

    protected abstract Assembly TestAssembly { get; }

    protected abstract string TestAttribute { get; }

    protected abstract string TestFile { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Fix later")]
    public async Task Execute()
    {
        Console.WriteLine($"Input file: {TestFile}");

        CompilationUnitSyntax compilation = SyntaxFactory.ParseCompilationUnit(await File.ReadAllTextAsync(TestFile));
        SyntaxTree tree = compilation.SyntaxTree;

        Console.WriteLine($"File: {Path.GetFileName(TestFile)}");

        var docsName = Path.GetFileNameWithoutExtension(TestFile).Replace("Tests", ".md", StringComparison.Ordinal);

        var docs = new StringBuilder();
        var toc = new StringBuilder();
        var scenarios = new StringBuilder();

        docs.AppendLine("<!--")
            .AppendLine("This is a generated file, please edit src\\docs\\AwesomeAssertions.Analyzers.Docs.Generator\\DocsGenerator.cs to change the contents")
            .AppendLine("-->")
            .AppendLine();

        var subject = Path.GetFileNameWithoutExtension(TestFile).Replace("AnalyzerTests", string.Empty, StringComparison.Ordinal);
        docs.AppendLine($"# {subject} Analyzer Docs")
            .AppendLine();

        scenarios.AppendLine("## Scenarios")
            .AppendLine();

        SyntaxNode root = await tree.GetRootAsync();
        ClassDeclarationSyntax classDef = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
        var methodsMap = methods.ToDictionary(m => m.Identifier.Text);

        Type classType = TestAssembly.GetType($"AwesomeAssertions.Analyzers.Docs.{classDef.Identifier.Text}");
        var classInstance = Activator.CreateInstance(classType);

        foreach (MethodDeclarationSyntax method in methods.Where(m => m.AttributeLists.Any(list => list.Attributes.Count is 1 && list.Attributes[0].Name.ToString() == TestAttribute)))
        {
            // success scenario:
            {
                scenarios.AppendLine($"### scenario: {method.Identifier}")
                    .AppendLine();
                var bodyLines = method.Body.ToFullString().Split(Environment.NewLine)[1..^2];
                var paddingToRemove = bodyLines[0].IndexOf(bodyLines[0].TrimStart());
                var normalizedBody = bodyLines.Select(l => l.Length > paddingToRemove ? l.Substring(paddingToRemove) : l).Aggregate((a, b) => a + Environment.NewLine + b);
                var methodBody = $"```cs{Environment.NewLine}{normalizedBody}{Environment.NewLine}```";
                scenarios.AppendLine(methodBody)
                    .AppendLine();

                var newAssertion = bodyLines[^1].Trim();

#pragma warning disable CA1308 // Normalize strings to uppercase: wrong here
                toc.AppendLine($"- [{method.Identifier}](#scenario-{method.Identifier.Text.ToLowerInvariant()}) - `{newAssertion}`");
#pragma warning restore CA1308 // Normalize strings to uppercase
            }

            // AwesomeAssertion failures scenario:
            if (methodsMap.TryGetValue($"{method.Identifier.Text}_Failure", out MethodDeclarationSyntax testWithFailure))
            {
                MethodInfo testMethodWithFailure = classType.GetMethod(testWithFailure.Identifier.Text);

                var failureMessages = SplitAssertionFailureMessages(GetMethodExceptionMessage(classInstance, testMethodWithFailure));

                var bodyLines = testWithFailure.Body.ToFullString().Split(Environment.NewLine)[2..^2];
                var paddingToRemove = bodyLines[0].IndexOf(bodyLines[0].TrimStart());
                SyntaxTrivia oldAssertionComment = GetOldAssertionComment(testWithFailure);
                SyntaxTrivia newAssertionComment = GetNewAssertionComment(testWithFailure);

                IEnumerable<ExpressionStatementSyntax> statements = testWithFailure.Body.Statements.OfType<ExpressionStatementSyntax>();

                var oldAssertionStatements = statements.Where(x => x.Span.CompareTo(oldAssertionComment.Span) > 0 && x.Span.CompareTo(newAssertionComment.Span) < 0).ToArray();
                var newAssertionStatement = statements.Single(x => x.Span.CompareTo(newAssertionComment.Span) > 0);
                string[] oldFailureMessages;
                string newFailureMessage;
                if (failureMessages.Length == oldAssertionStatements.Length + 1)
                {
                    oldFailureMessages = failureMessages[..^1];
                    newFailureMessage = failureMessages[^1];
                }
                else if (failureMessages.Length == oldAssertionStatements.Length && failureMessages[^1].StartsWith("Expected exception message ", StringComparison.Ordinal))
                {
                    var oldMessagesBeforeDuplicate = failureMessages[..^1];
                    oldFailureMessages = oldMessagesBeforeDuplicate.Append(oldMessagesBeforeDuplicate[^1]).ToArray();
                    newFailureMessage = failureMessages[^1];
                }
                else if (failureMessages.Length == oldAssertionStatements.Length)
                {
                    oldFailureMessages = failureMessages;
                    newFailureMessage = failureMessages[^1];
                }
                else
                {
                    throw new InvalidOperationException($"Expected one failure message per assertion in {testWithFailure.Identifier}, but found {failureMessages.Length} messages for {oldAssertionStatements.Length + 1} assertions.");
                }

                var oldAssertions = oldAssertionStatements
                    .Select((x, i) => x.ToString().TrimStart() + " \t// fail message: " + string.Join($"{Environment.NewLine}// ", oldFailureMessages[i].Split(Environment.NewLine)));
                var newAssertion = newAssertionStatement.ToString().TrimStart() + " \t// fail message: " + string.Join($"{Environment.NewLine}// ", newFailureMessage.Split(Environment.NewLine));

                var arrange = bodyLines.TakeWhile(x => !string.IsNullOrEmpty(x))
                    .Select(l => l.Length > paddingToRemove ? l.Substring(paddingToRemove) : l).Aggregate((a, b) => a + Environment.NewLine + b);

                scenarios.AppendLine("#### Failure messages")
                    .AppendLine()
                    .AppendLine("```cs")
                    .AppendLine(arrange)
                    .AppendLine()
                    .AppendLine(OldAssertionComment);
                foreach (var oldAssertion in oldAssertions)
                {
                    scenarios.AppendLine(oldAssertion);
                }

                scenarios.AppendLine()
                    .AppendLine(NewAssertionComment)
                    .AppendLine(newAssertion)
                    .AppendLine("```")
                    .AppendLine();
            }

            // Testing Libraries failures scenarios:
            if (methodsMap.TryGetValue($"{method.Identifier.Text}_Failure_NewAssertion", out MethodDeclarationSyntax testWithFailureNewAssertion))
            {
                IEnumerable<MethodDeclarationSyntax> testWithFailureOldAssertions = methodsMap.Where(x => x.Key.StartsWith($"{method.Identifier.Text}_Failure_OldAssertion")).Select(x => x.Value);

                IEnumerable<MethodInfo> testMethodWithFailureOldAssertions = testWithFailureOldAssertions.Select(m => classType.GetMethod(m.Identifier.Text));
                MethodInfo testMethodWithFailureNewAssertion = classType.GetMethod(testWithFailureNewAssertion.Identifier.Text);

                var exceptionMessageLinesOldAssertions = testMethodWithFailureOldAssertions.Select(m => GetMethodExceptionMessage(classInstance, m)).ToArray();
                var exceptionMessageLinesNewAssertion = GetMethodExceptionMessage(classInstance, testMethodWithFailureNewAssertion);

                SyntaxTrivia[] oldAssertionComment = testWithFailureOldAssertions.Select(x => GetOldAssertionComment(x)).ToArray();
                SyntaxTrivia newAssertionComment = GetNewAssertionComment(testWithFailureNewAssertion);

                var bodyLines = testWithFailureNewAssertion.Body.ToFullString().Split(Environment.NewLine)[2..^2];
                var paddingToRemove = bodyLines[0].IndexOf(bodyLines[0].TrimStart());

                IEnumerable<string> oldAssertions = testWithFailureOldAssertions.Select((x, i) => x.Body.Statements.OfType<ExpressionStatementSyntax>().Single(x => x.Span.CompareTo(oldAssertionComment[i].Span) > 0).ToString().TrimStart() + " /* fail message: " + AwesomeAssertionAnalyzerDocsUtils.ReplaceStackTrace(exceptionMessageLinesOldAssertions[i]) + " */");
                var newAssertion = testWithFailureNewAssertion.Body.Statements.OfType<ExpressionStatementSyntax>().Single(x => x.Span.CompareTo(newAssertionComment.Span) > 0).ToString().TrimStart() + " /* fail message: " + exceptionMessageLinesNewAssertion + " */";

                var arrange = bodyLines.TakeWhile(x => !string.IsNullOrEmpty(x))
                    .Select(l => l.Length > paddingToRemove ? l.Substring(paddingToRemove) : l).Aggregate((a, b) => a + Environment.NewLine + b);

                scenarios.AppendLine("#### Failure messages")
                    .AppendLine()
                    .AppendLine("```cs")
                    .AppendLine(arrange)
                    .AppendLine()
                    .AppendLine(OldAssertionComment);
                foreach (var oldAssertion in oldAssertions)
                {
                    scenarios.AppendLine(oldAssertion);
                }

                scenarios.AppendLine()
                    .AppendLine(NewAssertionComment)
                    .AppendLine(newAssertion)
                    .AppendLine("```")
                    .AppendLine();
            }
        }

        docs.Append(toc).AppendLine()
            .AppendLine()
            .Append(scenarios).AppendLine();

        var docsPath = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "docs", docsName);
        Directory.CreateDirectory(Path.GetDirectoryName(docsPath));
        await File.WriteAllTextAsync(docsPath, docs.ToString());
    }

    private static SyntaxTrivia GetNewAssertionComment(MethodDeclarationSyntax testWithFailure) =>
        testWithFailure.DescendantTrivia().First(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia) && x.ToString().Equals(NewAssertionComment, StringComparison.Ordinal));

    private static SyntaxTrivia GetOldAssertionComment(MethodDeclarationSyntax testWithFailure) =>
        testWithFailure.DescendantTrivia().First(x => x.IsKind(SyntaxKind.SingleLineCommentTrivia) && x.ToString().Equals(OldAssertionComment, StringComparison.Ordinal));

    private string GetMethodExceptionMessage(object instance, MethodInfo method)
    {
        ResetTestFramework();

        try
        {
            var result = method.Invoke(instance, null);
            if (result is Task task)
            {
                task.GetAwaiter().GetResult();
            }
        }
        catch (Exception ex) when (ex.InnerException is null)
        {
            return ex.Message;
        }
        catch (Exception ex) when (ex.InnerException is Exception exception)
        {
            return exception.Message;
        }

        throw new InvalidOperationException($"Method {instance.GetType().Name}.{method.Name} did not throw an exception");
    }

    private static string[] SplitAssertionFailureMessages(string exceptionMessage)
    {
        var lines = exceptionMessage.Split(Environment.NewLine);
        var failureStartIndexes = Enumerable.Range(0, lines.Length)
            .Where(index => lines[index].StartsWith("Expected ", StringComparison.Ordinal) || lines[index].StartsWith("Did not expect ", StringComparison.Ordinal))
            .ToArray();

        return failureStartIndexes.Select((start, index) =>
        {
            var end = index + 1 < failureStartIndexes.Length ? failureStartIndexes[index + 1] : lines.Length;
            return string.Join(Environment.NewLine, lines[start..end]).TrimEnd();
        }).ToArray();
    }

    protected virtual void ResetTestFramework()
    {
    }
}
