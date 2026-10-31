using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AwesomeAssertions.Analyzers.Tips;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncVoidAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = Constants.CodeSmell.AsyncVoid;
    public const string Title = "Code Smell";
    public const string Message = "The assertions might not be executed when assigning an async void lambda to a Action";

    public static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, Message, Constants.CodeSmell.Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCodeBlockAction(AnalyzeCodeBlock);
    }

    private static void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
    {
        if (context.CodeBlock is not MethodDeclarationSyntax method)
        {
            return;
        }

        if (method.Body != null)
        {
            foreach (LocalDeclarationStatementSyntax statement in method.Body.Statements.OfType<LocalDeclarationStatementSyntax>())
            {
                Diagnostic diagnostic = AnalyzeStatement(context.SemanticModel, statement);
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static Diagnostic AnalyzeStatement(SemanticModel semanticModel, LocalDeclarationStatementSyntax statement)
    {
        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(statement.Declaration.Type);
        if (symbolInfo.Symbol?.Name != nameof(Action))
        {
            return null;
        }

        foreach (EqualsValueClauseSyntax initializer in statement.Declaration.Variables.Select(x => x.Initializer))
        {
            if (initializer is null)
            {
                continue;
            }

            if (initializer.Value is not ParenthesizedLambdaExpressionSyntax lambda)
            {
                continue;
            }

            if (lambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
            {
                return Diagnostic.Create(Rule, statement.GetLocation());
            }
        }

        return null;
    }
}
